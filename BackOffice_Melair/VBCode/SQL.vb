Imports System.Data
Imports System.Data.SqlClient

Public Class SQL
    Dim sqlConn As New SqlConnection
    Dim sqlCmd As New SqlCommand
    Dim sqlAdapter As New SqlDataAdapter

    Dim connTable As String
    Dim query As String


  Public Function retrieveShips() As DataTable
    Dim result As New DataTable

    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDPricingFeed")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      connTable = "BarcosAPI"
      query = "SELECT DISTINCT ShipName, ShipCode "
      query &= "FROM " & connTable & " "
      query &= "WHERE CruiseLineCode = 'RCC' OR  CruiseLineCode = 'CEL'"
      query &= "ORDER BY 1"

      sqlCmd.CommandText = query
      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(result)

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
    End Try

    retrieveShips = result
  End Function

    Public Function retrieveAirCompanies() As DataTable
        Dim result As New DataTable

        Dim sqlConn As New SqlConnection

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairCompañiasAereas")
            query = "SELECT DISTINCT Company FROM " & connTable
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)

            result = sqlData

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrieveAirCompanies = result
    End Function

    Public Function getItinerary(shipCode As String, sailDate As String) As DataTable
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDPricingFeed")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            'sailDate = translateDate(sailDate)

            connTable = ConfigurationManager.AppSettings.Get("BDPricingFeed_Itinerary")
            query = "SELECT ItineraryCode, LocationName, ArrivalTime, DepartureTime, Nnoches "
            query &= "FROM " & connTable & " "
            query &= "WHERE SailingOnlyFlag='Y' AND ShipCode = '" & shipCode & "' AND SailDate = CONVERT(datetime, '" & sailDate & "', 103) "
            query &= "ORDER BY ActivityDate"

            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            ' Antes de devolver el resultado, es necesario traducir las escalas al portugués
            Dim portTrans As String
            For Each row In result.Rows
                portTrans = translatePort(row("LocationName"))

                If portTrans <> "" Then ' De haber una traducción del puerto en la tabla PuertosAPI, se utiliza ese valor
                    row("LocationName") = portTrans
                End If
            Next

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getItinerary = result
    End Function

    Private Function translatePort(port As String) As String
        Dim result As String = ""

        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand
        Dim sqlAdapter As New SqlDataAdapter
        Dim sqlData As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDPricingFeed")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            query = "SELECT portugues "
            query &= "FROM PuertosAPI "
            query &= "WHERE PortName = '" & port & "'"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)

            If (Not sqlData Is Nothing) And (sqlData.Rows.Count > 0) Then
                result = sqlData.Rows.Item(0).Item("portugues")
            End If
            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        translatePort = result
    End Function

  Private Function getShipName(shipCode As String) As String
    Dim result As String = ""

    Dim sqlConn As New SqlConnection
    Dim sqlCmd As New SqlCommand
    Dim sqlAdapter As New SqlDataAdapter
    Dim sqlData As New DataSet

    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDPricingFeed")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      query = "SELECT ShipName "
      query &= "FROM BarcosAPI "
      query &= "WHERE ShipCode = '" & shipCode & "'"
      sqlCmd.CommandText = query

      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(sqlData)
      result = sqlData.Tables(0).Rows(0).Item("ShipName")

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
    End Try

    getShipName = result
  End Function

    Public Function retrievePackages() As DataTable
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            'query = "SELECT PackageName, CONVERT(varchar(10), SailingDate, 103) AS SailingDate, ShipCode FROM " & connTable
            query = "SELECT DISTINCT PackageName, ShipCode FROM " & connTable
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)
            For Each row In result.Rows
                row("ShipCode") = getShipName(row("ShipCode"))
            Next

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrievePackages = result
    End Function

    Public Function retrievePackage(packName As String, sailDate As String) As Object
        Dim packageData(3) As Object

        Dim basicData As New Dictionary(Of String, String)
        Dim flightsData As New Dictionary(Of Integer, Dictionary(Of String, String))
        Dim flightData As Dictionary(Of String, String)
        Dim detailsData As New Dictionary(Of Integer, Dictionary(Of String, String))
        Dim detailData As Dictionary(Of String, String)
        Dim linesData As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String)))
        Dim lineMap As Dictionary(Of Integer, Dictionary(Of String, String))
        Dim lineData As Dictionary(Of String, String) = Nothing

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT * "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packName & "'"
            If sailDate <> "" Then
                query &= " AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"
            End If
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            For Each row In sqlData.Rows
                basicData.Add("PackageName", row("PackageName"))
                basicData.Add("ItineraryName", row("ItineraryName"))
                basicData.Add("ItineraryCode", row("ItineraryCode"))
                basicData.Add("Destination", row("Destination"))
                basicData.Add("Img1", row("Img1"))
                basicData.Add("Img2", row("Img2"))
                basicData.Add("Img3", row("Img3"))
                basicData.Add("Img4", row("Img4"))
                basicData.Add("Img5", row("Img5"))
                basicData.Add("Dining", row("Dining"))
                basicData.Add("Included", row("Included"))
                basicData.Add("NotIncluded", row("NotIncluded"))
                basicData.Add("Notes", row("Notes"))

                basicData.Add("Flight", row("Flight"))
                basicData.Add("Transfer", row("Transfer"))
                basicData.Add("Hotel", row("Hotel"))
                basicData.Add("ShipDescription", row("ShipDescription"))
            Next

            Dim packId As Integer = sqlData.Rows(0).Item("idPack")

            connTable = ConfigurationManager.AppSettings.Get("MelairFlights")
            query = "SELECT * "
            query &= "FROM " & connTable & " "
            query &= "WHERE idPack = " & packId
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlData.Reset()
            sqlAdapter.Fill(sqlData)
            Dim flightIndex As Integer = 0
            For Each row In sqlData.Rows
                connTable = ConfigurationManager.AppSettings.Get("MelairFlights")

                flightData = New Dictionary(Of String, String)
                flightData.Add("Id", row("idFlight"))
                flightData.Add("Company", row("Company"))
                flightData.Add("Logo", row("Logo"))
                flightData.Add("City", row("City"))
                flightData.Add("CityDest", row("CityDest"))
                flightData.Add("Description", row("Description"))
                flightData.Add("Quota", row("CabinsPerCity"))
                flightData.Add("Status", row("FlightStatus"))

                flightsData.Add(flightIndex, flightData)

                Dim flightId As Integer = sqlData.Rows(flightIndex).Item("idFlight")

                detailData = New Dictionary(Of String, String)
                detailData.Add("Code", row("FlightCode"))
                detailData.Add("Date", row("FlightDate"))
                'Dim flightTime As String = row("FlightTime")
                'detailData.Add("Hour", flightTime.Substring(0, 2) & ":" & flightTime.Substring(2, 2))
                detailData.Add("Hour", row("FlightTime"))
                detailData.Add("Taxes", Replace(row("FlightTaxes"), ",", "."))
                detailData.Add("Return", row("ReturnFlight"))
                detailData.Add("ReturnFlightTime", row("ReturnFlightTime"))
                detailData.Add("Transfer", row("FlightTransfer"))
                detailData.Add("Hotel", row("FlightHotel"))
                detailData.Add("Offer", row("OfferDescription"))

                connTable = ConfigurationManager.AppSettings.Get("MelairPackPrices")
                query = "SELECT * "
                query &= "FROM " & connTable & " "
                query &= "WHERE idFlight = " & flightId
                sqlCmd.CommandText = query

                sqlAdapter.SelectCommand = sqlCmd
                Dim sqlData2 As New DataTable
                sqlAdapter.Fill(sqlData2)
                For Each pricesRow In sqlData2.Rows
                    detailData.Add("IDouble", Replace(pricesRow("IDouble"), ",", "."))
                    detailData.Add("ITriCua", Replace(pricesRow("ITriCua"), ",", "."))
                    detailData.Add("I34Ad", Replace(pricesRow("I34Ad"), ",", "."))
                    detailData.Add("I34Cr", Replace(pricesRow("I34Cr"), ",", "."))
                    detailData.Add("I34Bb", Replace(pricesRow("I34Bb"), ",", "."))
                    detailData.Add("IInd", Replace(pricesRow("IInd"), ",", "."))
                    detailData.Add("IOffer", pricesRow("IOffer"))
                    detailData.Add("ODouble", Replace(pricesRow("ODouble"), ",", "."))
                    detailData.Add("OTriCua", Replace(pricesRow("OTriCua"), ",", "."))
                    detailData.Add("O34Ad", Replace(pricesRow("O34Ad"), ",", "."))
                    detailData.Add("O34Cr", Replace(pricesRow("O34Cr"), ",", "."))
                    detailData.Add("O34Bb", Replace(pricesRow("O34Bb"), ",", "."))
                    detailData.Add("OInd", Replace(pricesRow("OInd"), ",", "."))
                    detailData.Add("OOffer", pricesRow("OOffer"))
                    detailData.Add("BDouble", Replace(pricesRow("BDouble"), ",", "."))
                    detailData.Add("BTriCua", Replace(pricesRow("BTriCua"), ",", "."))
                    detailData.Add("B34Ad", Replace(pricesRow("B34Ad"), ",", "."))
                    detailData.Add("B34Cr", Replace(pricesRow("B34Cr"), ",", "."))
                    detailData.Add("B34Bb", Replace(pricesRow("B34Bb"), ",", "."))
                    detailData.Add("BInd", Replace(pricesRow("BInd"), ",", "."))
                    detailData.Add("BOffer", pricesRow("BOffer"))
                    detailData.Add("SDouble", Replace(pricesRow("SDouble"), ",", "."))
                    detailData.Add("STriCua", Replace(pricesRow("STriCua"), ",", "."))
                    detailData.Add("S34Ad", Replace(pricesRow("S34Ad"), ",", "."))
                    detailData.Add("S34Cr", Replace(pricesRow("S34Cr"), ",", "."))
                    detailData.Add("S34Bb", Replace(pricesRow("S34Bb"), ",", "."))
                    detailData.Add("SInd", Replace(pricesRow("SInd"), ",", "."))
                    detailData.Add("SOffer", pricesRow("SOffer"))
                Next
                detailsData.Add(flightIndex, detailData)

                connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines")
                query = "SELECT * "
                query &= "FROM " & connTable & " "
                query &= "WHERE idFlight = " & flightId
                sqlCmd.CommandText = query

                sqlAdapter.SelectCommand = sqlCmd
                sqlData2.Reset()
                sqlAdapter.Fill(sqlData2)
                lineMap = New Dictionary(Of Integer, Dictionary(Of String, String))
                Dim lineIndex As Integer = 0
                For Each linesRow In sqlData2.Rows
                    lineData = New Dictionary(Of String, String)
                    lineData.Add("Day", linesRow("Day"))
                    lineData.Add("Description", linesRow("Description"))
                    lineData.Add("Position", linesRow("BeforeAfter"))
                    lineMap.Add(lineIndex, lineData)

                    lineIndex += 1
                Next
                linesData.Add(flightIndex, lineMap)

                flightIndex += 1
            Next
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        packageData.SetValue(basicData, 0)
        packageData.SetValue(flightsData, 1)
        packageData.SetValue(detailsData, 2)
        packageData.SetValue(linesData, 3)

        retrievePackage = packageData
    End Function

    Public Function retrieveFirstPackage(packageName As String) As MelairPackage
        Dim package As New MelairPackage
        Dim sqlData As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT TOP 1 * "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packageName & "'"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)

            For Each row In sqlData.Rows
                Dim MAX_IMAGENES As Integer = 5

                package.id = row("idPack")
                package.name = row("PackageName")
                package.itineraryName = row("ItineraryName")
                package.itineraryCode = row("ItineraryCode")
                package.destination = row("Destination")
                For numImagen As Integer = 1 To MAX_IMAGENES
                    package.images.Add(row("Img" & numImagen))
                Next
                package.dining = row("Dining")
                package.included = row("Included")
                package.notIncluded = row("NotIncluded")
                package.notes = row("Notes")

                package.Flight = row("Flight")
                package.Transfer = row("Transfer")
                package.Hotel = row("Hotel")

                package.ShipDescription = row("ShipDescription")
            Next

        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        Return package
    End Function

    Public Function getPackageIds(packName As String, Optional sailDate As String = "") As String
        Dim result As String = ""

        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand
        Dim sqlAdapter As New SqlDataAdapter

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT DISTINCT idPack "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packName & "'"
            If sailDate <> "" Then
                query &= " AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"
            End If
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            For Each row In sqlData.Rows
                If sqlData.Rows.Count = 1 Then
                    'Entra por aquí en modo edición, que se modifica solo un paquete
                    result = row("idPack")
                Else
                    result &= row("idPack") & "|"
                End If

            Next

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getPackageIds = result
    End Function

    Public Function getPackFlightCodes(packName As String, sailDate As String) As DataTable
        Dim result As New DataTable

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT idPack "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packName & "' AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            Dim packId As Integer = sqlData.Rows(0).Item("idPack")

            connTable = ConfigurationManager.AppSettings.Get("MelairFlights")
            query = "SELECT FlightCode "
            query &= "FROM " & connTable & " "
            query &= "WHERE idPack = " & packId
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getPackFlightCodes = result
    End Function

    Public Function getDestination(itinCode As String, shipCode As String, sailDate As String) As String
        Dim result As String = ""

        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand
        Dim sqlAdapter As New SqlDataAdapter

        Dim connTable As String
        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("BDSalidas_Table")
            query = "SELECT SubRegion "
            query &= "FROM " & connTable & " "
            query &= "WHERE ShipCode = '" & shipCode & "' AND ItineraryCode = '" & itinCode & "' AND SailDate = CONVERT(datetime, '" & sailDate & "', 103)"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            result = sqlData.Rows(0).Item("SubRegion")

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getDestination = result
    End Function

    Public Function insertPackageOr(packageData As Array) As String
        Dim errorStr As String = ""
        'Dim backupSuccessful, backupRecovered As Boolean

        Dim basicData As Dictionary(Of String, String) = packageData(0)
        Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = packageData(1)
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = packageData(2)

        Dim packName, shipCode, sailDate, itinName, itinCode, destination, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes, diasVuelo As String
        Dim description, company, logo, city, CityDest, flightCode, flightDate, flightTime, flightTaxes, returnFlight, returnFlightTime, flightTransfer, flightHotel, offerDescription, status As String
        Dim cabinsPerCity As Integer
        Dim intDouble, intTriCua, int34Ad, int34Cr, int34Bb, intInd, extDouble, extTriCua, ext34Ad, ext34Cr, ext34Bb, extInd, balDouble, balTriCua, bal34Ad, bal34Cr, bal34Bb, balInd, suDouble, suTriCua, su34Ad, su34Cr, su34Bb, suInd As String
        Dim intOffer, extOffer, balOffer, suOffer As String

        packName = basicData.Item("PackageName")
        shipCode = basicData.Item("ShipCode")
        sailDate = basicData.Item("SailingDate")
        itinName = basicData.Item("ItineraryName")
        itinCode = basicData.Item("ItineraryCode")
        img1 = basicData.Item("Img1")
        img2 = basicData.Item("Img2")
        img3 = basicData.Item("Img3")
        img4 = basicData.Item("Img4")
        img5 = basicData.Item("Img5")
        dining = basicData.Item("Dining")
        includedTxt = basicData.Item("Included")
        notIncludedTxt = basicData.Item("NotIncluded")
        notes = basicData.Item("Notes")
        diasVuelo = basicData.Item("DiasVuelo")

        Dim nDiasVuelo As Double
        nDiasVuelo = (-CDbl(diasVuelo))

        'backupSuccessful = makeBackup()
        'If Not backupSuccessful Then
        '    Return insertFailed
        'End If
        Dim sqlData As New DataSet
        Try
            Dim rowsReturned As Integer

            ' Antes de comenzar con las operaciones, se verifica que no haya ya un paquete con el mismo nombre insertado en la tabla
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT COUNT(PackageName) AS Number "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packName & "'"
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)

            Dim number As Integer = CInt(sqlData.Tables(0).Rows(0).Item("Number"))
            If number = 0 Then
                ' En primer lugar hay que realizar tantas inserciones en la tabla MelairPackages como salidas haya para el itinerario mostrado
                Dim howManyDates As Integer = 0
                Dim itinDates As String = getItineraryDates(itinCode, shipCode)
                destination = getDestination(itinCode, shipCode, sailDate)
                For Each sail In itinDates.Split("|")
                    If sail <> "" Then
                        sailDate = sail
                        query = "INSERT INTO " & connTable & " VALUES ('" & packName & "', '" & shipCode & "', '" & sailDate & "', '" & itinName & "', '" & itinCode & "', '" & destination & "', '" & img1 & "', '" & img2 & "', '" & img3 & "', '" & img4 & "', '" & img5 & "', '" & dining & "', '" & includedTxt & "', '" & notIncludedTxt & "', '" & notes & "')"
                        sqlCmd.CommandText = query
                        rowsReturned = sqlCmd.ExecuteNonQuery()
                        If rowsReturned = 0 Then
                            ' Fallo
                        End If

                        howManyDates += 1
                    End If
                Next

                ' A continuación, habrá que repetir el proceso de asociar vuelos (con sus precios) a cada una de las ID's generadas para cada salida distinta
                Dim splitDates() As String = itinDates.Split("|")
                For i As Integer = 0 To howManyDates - 1
                    'sailDate = basicData.Item("SailingDate")
                    sailDate = splitDates(i)
                    connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
                    query = "SELECT idPack "
                    query &= "FROM " & connTable & " "
                    query &= "WHERE ShipCode = '" & shipCode & "' AND SailingDate = convert(datetime, '" & sailDate & "', 103)"
                    sqlCmd.CommandText = query
                    sqlAdapter.SelectCommand = sqlCmd
                    sqlData.Reset()
                    sqlAdapter.Fill(sqlData)

                    Dim packId As Integer = sqlData.Tables(0).Rows(0).Item(0)

                    For rowIndex As Integer = 0 To flightsData.Count - 1
                        company = flightsData.Item(rowIndex).Item("Company")
                        logo = flightsData.Item(rowIndex).Item("Logo")
                        city = flightsData.Item(rowIndex).Item("City")
                        CityDest = flightsData.Item(rowIndex).Item("CityDest")
                        description = flightsData.Item(rowIndex).Item("Description")
                        cabinsPerCity = flightsData.Item(rowIndex).Item("Quota")
                        status = flightsData.Item(rowIndex).Item("Status")

                        For itemIndex As Integer = 0 To detailsData.Item(rowIndex).Count - 1
                            connTable = ConfigurationManager.AppSettings.Get("MelairFlights")

                            flightCode = detailsData.Item(rowIndex).Item(itemIndex).Item("Code")
                            'flightDate = detailsData.Item(rowIndex).Item(itemIndex).Item("Date")

                            Dim fechaVuelo As Date
                            fechaVuelo = DateAdd(DateInterval.Day, nDiasVuelo, CDate(sailDate))

                            flightDate = fechaVuelo.ToString

                            flightTime = detailsData.Item(rowIndex).Item(itemIndex).Item("Hour")
                            'flightTime = flightTime.Substring(0, 2) & flightTime.Substring(3, 2) & "00"
                            flightTaxes = detailsData.Item(rowIndex).Item(itemIndex).Item("Taxes")
                            returnFlight = detailsData.Item(rowIndex).Item(itemIndex).Item("Return")
                            returnFlightTime = detailsData.Item(rowIndex).Item(itemIndex).Item("ReturnFlightTime")
                            flightTransfer = detailsData.Item(rowIndex).Item(itemIndex).Item("Transfer")
                            flightHotel = detailsData.Item(rowIndex).Item(itemIndex).Item("Hotel")
                            offerDescription = detailsData.Item(rowIndex).Item(itemIndex).Item("Offer")

                            Dim splitPrices() As String = detailsData.Item(rowIndex).Item(itemIndex).Item("Prices").Split("|")
                            intDouble = IIf(splitPrices(0) = "", 0, splitPrices(0))
                            intTriCua = IIf(splitPrices(1) = "", 0, splitPrices(1))
                            int34Ad = IIf(splitPrices(2) = "", 0, splitPrices(2))
                            int34Cr = IIf(splitPrices(3) = "", 0, splitPrices(3))
                            int34Bb = IIf(splitPrices(4) = "", 0, splitPrices(4))
                            intInd = IIf(splitPrices(5) = "", 0, splitPrices(5))
                            extDouble = IIf(splitPrices(6) = "", 0, splitPrices(6))
                            extTriCua = IIf(splitPrices(7) = "", 0, splitPrices(7))
                            ext34Ad = IIf(splitPrices(8) = "", 0, splitPrices(8))
                            ext34Cr = IIf(splitPrices(9) = "", 0, splitPrices(9))
                            ext34Bb = IIf(splitPrices(10) = "", 0, splitPrices(10))
                            extInd = IIf(splitPrices(11) = "", 0, splitPrices(11))
                            balDouble = IIf(splitPrices(12) = "", 0, splitPrices(12))
                            balTriCua = IIf(splitPrices(13) = "", 0, splitPrices(13))
                            bal34Ad = IIf(splitPrices(14) = "", 0, splitPrices(14))
                            bal34Cr = IIf(splitPrices(15) = "", 0, splitPrices(15))
                            bal34Bb = IIf(splitPrices(16) = "", 0, splitPrices(16))
                            balInd = IIf(splitPrices(17) = "", 0, splitPrices(17))
                            suDouble = IIf(splitPrices(18) = "", 0, splitPrices(18))
                            suTriCua = IIf(splitPrices(19) = "", 0, splitPrices(19))
                            su34Ad = IIf(splitPrices(20) = "", 0, splitPrices(20))
                            su34Cr = IIf(splitPrices(21) = "", 0, splitPrices(21))
                            su34Bb = IIf(splitPrices(22) = "", 0, splitPrices(22))
                            suInd = IIf(splitPrices(23) = "", 0, splitPrices(23))

                            Dim splitDiscounts() As String = detailsData.Item(rowIndex).Item(itemIndex).Item("Discounts").Split("|")
                            intOffer = splitDiscounts(0)
                            extOffer = splitDiscounts(1)
                            balOffer = splitDiscounts(2)
                            suOffer = splitDiscounts(3)

                            query = "INSERT INTO " & connTable & " VALUES (" & packId & ", '" & description & "', '" & company & "', '" & logo & "', '" & city & "', '" & flightCode & "', '" & flightDate & "', '" & flightTime & "', " & flightTaxes & ", '" & returnFlight & "','" & returnFlightTime & "', '" & flightTransfer & "', '" & flightHotel & "', '" & offerDescription & "', " & cabinsPerCity & ", '" & status & "','" & CityDest & "')"
                            sqlCmd.CommandText = query
                            rowsReturned = sqlCmd.ExecuteNonQuery()
                            If rowsReturned = 0 Then
                                ' Fallo
                            End If


                            query = "SELECT TOP(1) idFlight "
                            query &= "FROM " & connTable & " "
                            query &= "WHERE idPack = '" & packId & "' "
                            query &= "ORDER BY 1 DESC"
                            sqlCmd.CommandText = query
                            sqlAdapter.SelectCommand = sqlCmd
                            sqlData.Reset()
                            sqlAdapter.Fill(sqlData)

                            Dim flightId As Integer = sqlData.Tables(0).Rows(0).Item("idFlight")

                            connTable = ConfigurationManager.AppSettings.Get("MelairPackPrices")
                            query = "INSERT INTO " & connTable & " VALUES ('" & flightId & "', '" & intDouble & "', '" & intTriCua & "', '" & int34Ad & "', '" & int34Cr & "', '" & int34Bb & "', '" & intInd & "', '" & intOffer & "', '" & extDouble & "', '" & extTriCua & "', '" & ext34Ad & "', '" & ext34Cr & "', '" & ext34Bb & "', '" & extInd & "', '" & extOffer & "', '" & balDouble & "', '" & balTriCua & "', '" & bal34Ad & "', '" & bal34Cr & "', '" & bal34Bb & "', '" & balInd & "', '" & balOffer & "', '" & suDouble & "', '" & suTriCua & "', '" & su34Ad & "', '" & su34Cr & "', '" & su34Bb & "', '" & suInd & "', '" & suOffer & "')"
                            sqlCmd.CommandText = query
                            rowsReturned = sqlCmd.ExecuteNonQuery()
                            If rowsReturned = 0 Then
                                ' Fallo
                            End If
                        Next
                    Next
                Next
            Else
                errorStr = "Já existe um pacote com o mesmo nome que o especificado. Por favor tente um diferente."
                Return errorStr
            End If

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
            errorStr = "Ocorreu um erro durante a alta pacote. Por favor tente novamente."
        End Try

        insertPackageOr = errorStr
    End Function

    Public Function insertPackage(packageData As Array) As String
        Dim errorStr As String = ""
        Dim basicData As Dictionary(Of String, String) = packageData(0)

        Dim packName, shipCode, shipDescription, sailDate, itinName, itinCode, destination, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes, flight, transfer, hotel As String

        packName = basicData.Item("PackageName")
        shipCode = basicData.Item("ShipCode")

        shipDescription = basicData.Item("ShipDesc")

        sailDate = basicData.Item("SailingDate")
        itinName = basicData.Item("ItineraryName")
        itinCode = basicData.Item("ItineraryCode")
        img1 = basicData.Item("Img1")
        img2 = basicData.Item("Img2")
        img3 = basicData.Item("Img3")
        img4 = basicData.Item("Img4")
        img5 = basicData.Item("Img5")
        dining = basicData.Item("Dining")
        includedTxt = basicData.Item("Included")
        notIncludedTxt = basicData.Item("NotIncluded")
        notes = basicData.Item("Notes")

        flight = basicData.Item("Flight")
        transfer = basicData.Item("Transfer")
        hotel = basicData.Item("Hotel")

        Dim sqlData As New DataSet
        Try
            Dim rowsReturned As Integer

            ' Antes de comenzar con las operaciones, se verifica que no haya ya un paquete con el mismo nombre insertado en la tabla
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT COUNT(PackageName) AS Number "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packName & "'"
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)

            Dim number As Integer = CInt(sqlData.Tables(0).Rows(0).Item("Number"))
            If number = 0 Then
                ' En primer lugar hay que realizar tantas inserciones en la tabla MelairPackages como salidas haya para el itinerario mostrado
                Dim howManyDates As Integer = 0
                Dim itinDates As String = getItineraryDates(itinCode, shipCode)
                destination = getDestination(itinCode, shipCode, sailDate)
                For Each sail In itinDates.Split("|")
                    If sail <> "" Then
                        sailDate = sail
                        query = "INSERT INTO " & connTable & " VALUES ('" & packName & "', '" & shipCode & "', '" & shipDescription & "', CONVERT(datetime, '" & sailDate & "', 103), '" & itinName & "', '" & itinCode & "', '" & destination & "', '" & img1 & "', '" & img2 & "', '" & img3 & "', '" & img4 & "', '" & img5 & "', '" & dining & "', '" & includedTxt & "', '" & notIncludedTxt & "', '" & notes & "', '" & flight & "', '" & transfer & "', '" & hotel & "'); SELECT SCOPE_IDENTITY()"
                        sqlCmd.CommandText = query
                        'rowsReturned = sqlCmd.ExecuteNonQuery()
                        Dim currentID As Integer = sqlCmd.ExecuteScalar()
                        rowsReturned = currentID
                        GolbalVariable.Id = currentID
                        If rowsReturned = 0 Then
                            ' Fallo
                        End If

                        howManyDates += 1
                    End If
                Next

                'A continuación, habrá que repetir el proceso de asociar vuelos (con sus precios) a cada una de las ID's generadas para cada salida distinta
                'El código del vuelo (Flight) lo he quitado por el momento
            Else
                errorStr = "Já existe um pacote com o mesmo nome que o especificado. Por favor tente um diferente."
                Return errorStr
            End If

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
            errorStr = "Ocorreu um erro durante a alta pacote. Por favor tente novamente."
        End Try

        insertPackage = errorStr
    End Function

    Private Sub createAuxTables()
        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages") & "Aux"
            query = "SELECT 1 AS Number FROM sys.Tables WHERE Name = '" & connTable & "'"
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            If sqlData.Rows.Count = 0 Then
                query = "CREATE TABLE " & connTable & "(idPack bigint, PackageName nvarchar(50), ShipCode nchar(2), SailingDate date, ItineraryName nvarchar(50), ItineraryCode nvarchar(10), Destination nchar(3), Img1 nvarchar(50), Img2 nvarchar(50), Img3 nvarchar(50), Img4 nvarchar(50), Img5 nvarchar(50), Dining nvarchar(50), Included ntext, NotIncluded ntext, Notes ntext);"
                sqlCmd.CommandText = query
                sqlCmd.ExecuteNonQuery()
            End If

            connTable = ConfigurationManager.AppSettings.Get("MelairFlights") & "Aux"
            query = "SELECT 1 AS Number FROM sys.Tables WHERE Name = '" & connTable & "'"
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlData.Reset()
            sqlAdapter.Fill(sqlData)
            If sqlData.Rows.Count = 0 Then
                query = "CREATE TABLE " & connTable & "(idPack bigint, idFlight bigint, Description nvarchar(50), Company nvarchar(50), Logo nvarchar(50), City nvarchar(20), FlightCode nvarchar(15), FlightDate datetime, FlightTime nvarchar(30), FlightTaxes float, ReturnFlight nvarchar(50), ReturnFlightTime nvarchar(30), FlightTransfer nvarchar(50), FlightHotel nvarchar(50), OfferDescription nvarchar(50), CabinsPerCity smallint, FlightStatus nvarchar(3),CityDest nvarchar(20));"
                sqlCmd.CommandText = query
                sqlCmd.ExecuteNonQuery()
            End If

            connTable = ConfigurationManager.AppSettings.Get("MelairPackPrices") & "Aux"
            query = "SELECT 1 AS Number FROM sys.Tables WHERE Name = '" & connTable & "'"
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlData.Reset()
            sqlAdapter.Fill(sqlData)
            If sqlData.Rows.Count = 0 Then
                query = "CREATE TABLE " & connTable & "(idFlight bigint, IDouble decimal(18, 2), ITriCua decimal(18, 2), I34Ad decimal(18, 2), I34Cr decimal(18, 2), I34Bb decimal(18, 2), IInd decimal(18, 2), IOffer nvarchar(100), ODouble decimal(18, 2), OTriCua decimal(18, 2), O34Ad decimal(18, 2), O34Cr decimal(18, 2), O34Bb decimal(18, 2), OInd decimal(18, 2), OOffer nvarchar(100), BDouble decimal(18, 2), BTriCua decimal(18, 2), B34Ad decimal(18, 2), B34Cr decimal(18, 2), B34Bb decimal(18, 2), BInd decimal(18, 2), BOffer nvarchar(100), SDouble decimal(18, 2), STriCua decimal(18, 2), S34Ad decimal(18, 2), S34Cr decimal(18, 2), S34Bb decimal(18, 2), SInd decimal(18, 2), SOffer nvarchar(100));"
                sqlCmd.CommandText = query
                sqlCmd.ExecuteNonQuery()
            End If

            connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines") & "Aux"
            query = "SELECT 1 AS Number FROM sys.Tables WHERE Name = '" & connTable & "'"
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlData.Reset()
            sqlAdapter.Fill(sqlData)
            If sqlData.Rows.Count = 0 Then
                query = "CREATE TABLE " & connTable & "(idFlight bigint, FlightCode nvarchar(15), Day nvarchar(15), Description ntext, BeforeAfter nchar(1));"
                sqlCmd.CommandText = query
                sqlCmd.ExecuteNonQuery()
            End If

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try
    End Sub

    Private Sub deleteAuxTables()
        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages") & "Aux"
            query = "DROP TABLE " & connTable
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            connTable = ConfigurationManager.AppSettings.Get("MelairFlights") & "Aux"
            query = "DROP TABLE " & connTable
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            connTable = ConfigurationManager.AppSettings.Get("MelairPackPrices") & "Aux"
            query = "DROP TABLE " & connTable
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines") & "Aux"
            query = "DROP TABLE " & connTable
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try
    End Sub

    Public Function getNextId(tblName As String) As Integer
        Dim result As Integer = 1

        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand
        Dim sqlAdapter As New SqlDataAdapter
        Dim sqlData As New DataTable

        Dim connTable As String
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            If tblName = "Packages" Then ' MelairPackages
                connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
                query = "SELECT TOP(1) idPack "
                query &= "FROM " & connTable & " "
                query &= "ORDER BY 1 DESC"
            Else ' MelairFlights
                connTable = ConfigurationManager.AppSettings.Get("MelairFlights")
                query = "SELECT TOP(1) idFlight "
                query &= "FROM " & connTable & " "
                query &= "ORDER BY 1 DESC"
            End If
            sqlCmd.CommandText = query
            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)

            result = sqlData.Rows(0).Item(0) + 1

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getNextId = result
    End Function

    Public Function getItineraryDestination(sailDate As String, shipCode As String, itinCode As String) As String
        Dim result As String = ""

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            query = "SELECT portugues "
            query &= "FROM PORSALIDAS p, ZonasAPI z "
            query &= "WHERE p.SailDate = CONVERT(datetime, '" & sailDate & "', 103) AND p.ShipCode = '" & shipCode & "' AND p.ItineraryCode = '" & itinCode & "' "
            query &= "AND p.RegionCode = z.Region"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            result = sqlData.Rows(0).Item("portugues")

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getItineraryDestination = result
    End Function

    Private Sub insertFromAuxTables()
        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand

        Dim query As String
        Dim connTable As String
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SET IDENTITY_INSERT " & connTable & " ON"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()
            query = "INSERT INTO " & connTable & " (idPack, PackageName, ShipCode, SailingDate, ItineraryName, ItineraryCode, Destination, Img1, Img2, Img3, Img4, Img5, Dining, Included, NotIncluded, Notes) "
            query &= "SELECT idPack, PackageName, ShipCode, SailingDate, ItineraryName, ItineraryCode, Destination, Img1, Img2, Img3, Img4, Img5, Dining, Included, NotIncluded, Notes FROM " & connTable & "Aux"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()
            query = "SET IDENTITY_INSERT " & connTable & " OFF"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            connTable = ConfigurationManager.AppSettings.Get("MelairFlights")
            query = "SET IDENTITY_INSERT " & connTable & " ON"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()
            query = "INSERT INTO " & connTable & " (idPack, idFlight, Description, Company, Logo, City, FlightCode, FlightDate, FlightTime, FlightTaxes, ReturnFlight, ReturnFlightTime, FlightTransfer, FlightHotel, OfferDescription, CabinsPerCity, FlightStatus,CityDest) "
            query &= "SELECT idPack, idFlight, Description, Company, Logo, City, FlightCode, FlightDate, FlightTime, FlightTaxes, ReturnFlight, ReturnFlightTime, FlightTransfer, FlightHotel, OfferDescription, CabinsPerCity, FlightStatus, CityDest FROM " & connTable & "Aux"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()
            query = "SET IDENTITY_INSERT " & connTable & " OFF"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            connTable = ConfigurationManager.AppSettings.Get("MelairPackPrices")
            query = "INSERT INTO " & connTable & " (idFlight, IDouble, ITriCua, I34Ad, I34Cr, I34Bb, IInd, IOffer, ODouble, OTriCua, O34Ad, O34Cr, O34Bb, OInd, OOffer, BDouble, BTriCua, B34Ad, B34Cr, B34Bb, BInd, BOffer, SDouble, STriCua, S34Ad, S34Cr, S34Bb, SInd, SOffer) "
            query &= "SELECT idFlight, IDouble, ITriCua, I34Ad, I34Cr, I34Bb, IInd, IOffer, ODouble, OTriCua, O34Ad, O34Cr, O34Bb, OInd, OOffer, BDouble, BTriCua, B34Ad, B34Cr, B34Bb, BInd, BOffer, SDouble, STriCua, S34Ad, S34Cr, S34Bb, SInd, SOffer FROM " & connTable & "Aux"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines")
            query = "INSERT INTO " & connTable & " (idFlight, FlightCode, Day, Description, BeforeAfter) "
            query &= "SELECT idFlight, FlightCode, Day, Description, BeforeAfter FROM " & connTable & "Aux"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
            deleteAuxTables()
        End Try
    End Sub

  Public Function getIdsFlightFromIdsPackages(idPackages As List(Of String)) As List(Of String)
    Dim ids As New List(Of String)
    Dim table As New DataTable

    query = makeIdsFlightFromIdsPackagesQuery(idPackages)
    table = executeQuery(query, "BDSalidas", True)
    For Each id In table.Rows
      ids.Add(id.itemarray(0).ToString)
    Next

    Return ids
  End Function

  Public Function makeIdsFlightFromIdsPackagesQuery(idPackages As List(Of String)) As String
    Dim query As String = ""
    Dim selectStatement As String = "SELECT idFlight"
    Dim fromStatement As String = ""
    Dim whereStatement As String = ""

    If idPackages.Count > 0 Then
      fromStatement = makeStatement("FROM", "MelairFlights", True)
      whereStatement = makeOrClauses(idPackages, "idPack")
      whereStatement = makeStatement("WHERE", whereStatement, False)
      query = selectStatement & fromStatement & whereStatement
    End If

    Return query
  End Function

    Public Function getIdsFromEqualPackageName(packageName As String) As List(Of String)
        Dim ids As New List(Of String)
        Dim table As New DataTable

        query = makeGetIdsFromEqualPackageNameQuery(packageName)
        table = executeQuery(query, "BDSalidas", True)
        For Each id In table.Rows
            ids.Add(id.itemarray(0).ToString)
        Next

        Return ids
    End Function

    Public Function makeGetIdsFromEqualPackageNameQuery(packageName As String) As String
        Dim query As String = ""
        Dim selectStatement As String = ""
        Dim fromStatement As String = ""
        Dim whereStatement As String = ""
        Dim packageNameToSearch As New Dictionary(Of String, String)

        selectStatement = makeStatement("SELECT", "idPack", False)
        fromStatement = makeStatement("FROM", "MelairPackages", True)
        whereStatement = makeStatement("WHERE", "packageName = '" & packageName & "'", False)
        query = selectStatement & fromStatement & whereStatement

        Return query
  End Function

  Public Sub updatePrices(columnsToUpdate As Dictionary(Of String, String), flights As List(Of String))
    Dim query As String = ""
    Dim columnsWithPoints As Dictionary(Of String, String)

    columnsWithPoints = changeCommas(columnsToUpdate)
    query = makeUpdatePricesQuery(columnsWithPoints, flights)
    executeQuery(query, "BDSalidas")
  End Sub

  Public Function makeUpdatePricesQuery(columnsToUpdate As Dictionary(Of String, String), flights As IList(Of String)) As String
    Dim query As String = ""
    Dim updateStatement As String = ""
    Dim setStatement As String = ""
    Dim whereStatement As String = ""

    If columnsToUpdate.Count > 0 Then
      updateStatement = makeStatement("UPDATE", "MelairPackPrices", True)
      setStatement = makeStatement("SET", columnsToUpdate)
      whereStatement = makeOrClauses(flights, "idFlight")
      whereStatement = makeStatement("WHERE", whereStatement, False)
      query = updateStatement & setStatement & whereStatement
    End If

    Return query
  End Function

  Public Sub updateFlights(columnsToUpdate As Dictionary(Of String, String), flights As List(Of String))
    Dim query As String = ""
    
    query = makeUpdateFlightsQuery(columnsToUpdate, flights)
    Try
      executeQuery(query, "BDSalidas")
    Catch ex As Exception
      Throw
    End Try
  End Sub

  Public Function changeCommas(columnsToUpdate As Dictionary(Of String, String)) As Dictionary(Of String, String)
    Dim result As New Dictionary(Of String, String)

    For Each value In columnsToUpdate
      result.Add(value.Key, Replace(value.Value, ",", "."))
    Next

    Return result
  End Function

  Public Function makeUpdateFlightsQuery(columnsToUpdate As Dictionary(Of String, String),
                                         flights As List(Of String))
    Dim query As String = ""
    Dim updateStatement As String = ""
    Dim setStatement As String = ""
    Dim whereStatement As String = ""

    If columnsToUpdate.Count > 0 Then
      updateStatement = makeStatement("UPDATE", "MelairFlights", True)
      setStatement = makeStatement("SET", columnsToUpdate)
      whereStatement = makeOrClauses(flights, "idPack")
      whereStatement = makeStatement("WHERE", whereStatement, False)
      query = updateStatement & setStatement & whereStatement
    End If

    Return query
  End Function

  Public Function makeOrClauses(values As List(Of String), columnName As String) As String
    Dim result As String = ""

    For Each value In values
      If result <> "" Then
        result &= " OR " & columnName & " = '" & value & "'"
      Else
        result = columnName & " = '" & value & "'"
      End If
    Next

    Return result
  End Function

  Public Sub updatePackage(columnsToUpdate As Dictionary(Of String, String), package As String)
    Dim query As String = ""

    query = makeUpdatePackageQuery(columnsToUpdate, package)
    executeQuery(query, "BDSalidas")
  End Sub

  Public Function makeUpdatePackageQuery(columnsToUpdate As Dictionary(Of String, String),
                                     package As String) As String
    Dim query As String = ""
    Dim updateStatement As String = ""
    Dim setStatement As String = ""
    Dim whereStatement As String = ""

    If columnsToUpdate.Count > 0 Then
      updateStatement = makeStatement("UPDATE", "MelairPackages", True)
      setStatement = makeStatement("SET", columnsToUpdate)
      whereStatement = makeStatement("WHERE", "PackageName = '" & package & "'", False)
      query = updateStatement & setStatement & whereStatement
    End If

    Return query
  End Function

  Public Function getMelairPrices(idFlight As String) As MelairPrices
    Dim prices As New MelairPrices
    Dim query As String = ""
    Dim resultQuery As DataTable

    query = makeGetMelairPrices(idFlight)
    resultQuery = executeQuery(query, "BDSalidas", True)
    prices = New MelairPrices(resultQuery.Rows(0))

    Return prices
  End Function

  Public Function makeGetMelairPrices(idFlight As String) As String
    Dim query As String = ""
    Dim selectStatement As String = "SELECT *"
    Dim fromStatement As String = ""
    Dim whereStatement As String = ""

    If idFlight <> "" Then
      fromStatement = makeStatement("FROM", "MelairPackPrices", True)
      whereStatement = makeStatement("WHERE", "idFlight = '" & idFlight & "'", False)
      query = selectStatement & fromStatement & whereStatement
    End If

    Return query
  End Function

  Public Function getMelairFlight(idPack As String) As MelairFlight
    Dim flight As New MelairFlight
    Dim query As String = ""
    Dim resultQuery As DataTable

    query = makeGetMelairFlightQuery(idPack)
    resultQuery = executeQuery(query, "BDSalidas", True)
    flight = New MelairFlight(resultQuery.Rows(0))

    Return flight
  End Function

  Public Function makeGetMelairFlightQuery(idPack As String) As String
    Dim query As String = ""
    Dim selectStatement As String = "SELECT *"
    Dim fromStatement As String = ""
    Dim whereStatement As String = ""

    If idPack <> "" Then
      fromStatement = makeStatement("FROM", "MelairFlights", True)
      whereStatement = makeStatement("WHERE", "idPack  = '" & idPack & "'", False)
      query = selectStatement & fromStatement & whereStatement
    End If

    Return query
  End Function

  Public Function makeStatement(statement As String, values As Dictionary(Of String, String)) As String
    Dim statementString As String = ""
    Dim separator As String = ""

    separator = getSeparator(statement)
    For Each value In values
      statementString = addToStatement(statementString, value.Key & " = '" & value.Value & "'", separator)
    Next
    statementString = " " & statement & " " & statementString

    Return statementString
  End Function

  Public Function makeStatement(statement As String, values As List(Of String)) As String
    Dim statementString As String = ""
    Dim separator As String = ""

    separator = getSeparator(statement)
    For Each value In values
      statementString = addToStatement(statementString, value, separator)
    Next
    statementString = " " & statement & " " & statementString

    Return statementString
  End Function

  Public Function makeStatement(statement As String, value As String, table As Boolean) As String
    Dim statementString As String = ""
    Dim separator As String = ""

    separator = getSeparator(statement)
    If table = True Then
      statementString = addToStatement(statementString, ConfigurationManager.AppSettings.Get(value), separator)
    Else
      statementString = addToStatement(statementString, value, separator)
    End If
    statementString = " " & statement & " " & statementString

    Return statementString
  End Function

  Public Function getSeparator(statement As String) As String
    Dim separator As String = ""

    If (statement = "SELECT" Or statement = "SET" Or statement = "FROM") Then
      separator = ","
    ElseIf (statement = "WHERE") Then
      separator = "AND"
    End If

    Return separator
  End Function

  Public Function addToStatement(previousStatement As String, expressionToAdd As String,
                                 Optional separatorToAdd As String = "") As String
    Dim result As String = ""

    If previousStatement <> "" Then
      result = previousStatement & " " & separatorToAdd & " " & expressionToAdd
    Else
      result = expressionToAdd
    End If

    Return result
  End Function

  Public Function executeQuery(query As String, bd As String, Optional returnResults As Boolean = False) As DataTable
    Dim sqlConn As New SqlConnection
    Dim sqlCmd As New SqlCommand
    Dim result As New DataTable

    sqlCmd.CommandText = query

    sqlAdapter.SelectCommand = sqlCmd

    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get(bd)
      sqlConn.Open()
      sqlCmd.Connection = sqlConn
      sqlCmd.CommandText = query
      If returnResults = False Then
        sqlCmd.ExecuteNonQuery()
      Else
        sqlAdapter.SelectCommand = sqlCmd
        sqlAdapter.Fill(result)
      End If
      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
      Throw
    End Try
    Return result
  End Function

    Public Function editPackageOld(packageData As Array) As String
        Dim errorStr As String = ""

        Dim basicData As Dictionary(Of String, String) = packageData(0)
        Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = packageData(1)
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = packageData(2)
        Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = packageData(3)

        Dim flightId, cabinsPerCity As Integer
        Dim packName, sailDate, itinName, itinCode, destination, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes As String
        Dim company, logo, city, cityDest, description, flightCode, flightDate, flightTime, flightTaxes, returnFlight, returnFlightTime, flightTransfer, flightHotel, offerDescription, status As String
        Dim IDouble, ITriCua, I34Ad, I34Cr, I34Bb, IInd, ODouble, OTriCua, O34Ad, O34Cr, O34Bb, OInd, BDouble, BTriCua, B34Ad, B34Cr, B34Bb, BInd, SDouble, STriCua, S34Ad, S34Cr, S34Bb, SInd As String
        Dim IOffer, OOffer, BOffer, SOffer As String
        Dim day As String
        Dim lineDescription, position As String

        createAuxTables()

        packName = basicData.Item("PackageName")
        sailDate = basicData.Item("SailingDate")
        itinName = basicData.Item("ItineraryName")
        itinCode = basicData.Item("ItineraryCode")
        destination = basicData.Item("Destination")
        img1 = basicData.Item("Img1")
        img2 = basicData.Item("Img2")
        img3 = basicData.Item("Img3")
        img4 = basicData.Item("Img4")
        img5 = basicData.Item("Img5")
        dining = basicData.Item("Dining")
        includedTxt = basicData.Item("Included")
        notIncludedTxt = basicData.Item("NotIncluded")
        notes = basicData.Item("Notes")

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "SELECT idPack, ShipCode "
            query &= "FROM " & connTable & " "
            query &= "WHERE PackageName = '" & packName & "' AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            Dim packId As Integer = sqlData.Rows(0).Item("idPack")
            Dim shipCode As String = sqlData.Rows(0).Item("ShipCode")

            Dim nextPackId As Integer = getNextId("Packages")

            connTable &= "Aux"
            query = "INSERT INTO " & connTable & " VALUES (" & nextPackId & ", '" & packName & "', '" & shipCode & "', '" & sailDate & "', '" & itinName & "', '" & itinCode & "', '" & destination & "', '" & img1 & "', '" & img2 & "', '" & img3 & "', '" & img4 & "', '" & img5 & "', '" & dining & "', '" & includedTxt & "', '" & notIncludedTxt & "', '" & notes & "')"
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            Dim nextFlightId As Integer = getNextId("Flights")
            For flightIndex As Integer = 0 To flightsData.Count - 1
                If flightsData.Item(flightIndex).Item("Company") <> "" Then
                    description = flightsData.Item(flightIndex).Item("Description")
                    company = flightsData.Item(flightIndex).Item("Company")
                    logo = flightsData.Item(flightIndex).Item("Logo")
                    city = flightsData.Item(flightIndex).Item("City")
                    cityDest = flightsData.Item(flightIndex).Item("CityDest")
                    cabinsPerCity = flightsData.Item(flightIndex).Item("Quota")
                    status = flightsData.Item(flightIndex).Item("Status")
                    flightDate = detailsData.Item(flightIndex).Item("Date")
                    flightCode = detailsData.Item(flightIndex).Item("Code")
                    flightTime = detailsData.Item(flightIndex).Item("Hour")
                    flightTaxes = detailsData.Item(flightIndex).Item("Taxes")
                    returnFlight = detailsData.Item(flightIndex).Item("Return")
                    returnFlightTime = detailsData.Item(flightIndex).Item("ReturnFlightTime")
                    flightTransfer = detailsData.Item(flightIndex).Item("Transfer")
                    flightHotel = detailsData.Item(flightIndex).Item("Hotel")
                    offerDescription = detailsData.Item(flightIndex).Item("Offer")

                    connTable = ConfigurationManager.AppSettings.Get("MelairFlights") & "Aux"
                    query = "INSERT INTO " & connTable & " VALUES (" & nextPackId & ", " & nextFlightId & ", '" & description & "', '" & company & "', '" & logo & "', '" & city & "', '" & flightCode & "', '" & flightDate & "', '" & flightTime & "', " & flightTaxes & ", '" & returnFlight & "','" & returnFlightTime & "', '" & flightTransfer & "', '" & flightHotel & "', '" & offerDescription & "', " & cabinsPerCity & ", '" & status & "','" & cityDest & "')"
                    sqlCmd.CommandText = query
                    sqlCmd.ExecuteNonQuery()

                    IDouble = IIf(detailsData.Item(flightIndex).Item("IDouble") = "", 0, detailsData.Item(flightIndex).Item("IDouble"))
                    ITriCua = IIf(detailsData.Item(flightIndex).Item("ITriCua") = "", 0, detailsData.Item(flightIndex).Item("ITriCua"))
                    I34Ad = IIf(detailsData.Item(flightIndex).Item("I34Ad") = "", 0, detailsData.Item(flightIndex).Item("I34Ad"))
                    I34Cr = IIf(detailsData.Item(flightIndex).Item("I34Cr") = "", 0, detailsData.Item(flightIndex).Item("I34Cr"))
                    I34Bb = IIf(detailsData.Item(flightIndex).Item("I34Bb") = "", 0, detailsData.Item(flightIndex).Item("I34Bb"))
                    IInd = IIf(detailsData.Item(flightIndex).Item("IInd") = "", 0, detailsData.Item(flightIndex).Item("IInd"))
                    ODouble = IIf(detailsData.Item(flightIndex).Item("ODouble") = "", 0, detailsData.Item(flightIndex).Item("ODouble"))
                    OTriCua = IIf(detailsData.Item(flightIndex).Item("OTriCua") = "", 0, detailsData.Item(flightIndex).Item("OTriCua"))
                    O34Ad = IIf(detailsData.Item(flightIndex).Item("O34Ad") = "", 0, detailsData.Item(flightIndex).Item("O34Ad"))
                    O34Cr = IIf(detailsData.Item(flightIndex).Item("O34Cr") = "", 0, detailsData.Item(flightIndex).Item("O34Cr"))
                    O34Bb = IIf(detailsData.Item(flightIndex).Item("O34Bb") = "", 0, detailsData.Item(flightIndex).Item("O34Bb"))
                    OInd = IIf(detailsData.Item(flightIndex).Item("OInd") = "", 0, detailsData.Item(flightIndex).Item("OInd"))
                    BDouble = IIf(detailsData.Item(flightIndex).Item("BDouble") = "", 0, detailsData.Item(flightIndex).Item("BDouble"))
                    BTriCua = IIf(detailsData.Item(flightIndex).Item("BTriCua") = "", 0, detailsData.Item(flightIndex).Item("BTriCua"))
                    B34Ad = IIf(detailsData.Item(flightIndex).Item("B34Ad") = "", 0, detailsData.Item(flightIndex).Item("B34Ad"))
                    B34Cr = IIf(detailsData.Item(flightIndex).Item("B34Cr") = "", 0, detailsData.Item(flightIndex).Item("B34Cr"))
                    B34Bb = IIf(detailsData.Item(flightIndex).Item("B34Bb") = "", 0, detailsData.Item(flightIndex).Item("B34Bb"))
                    BInd = IIf(detailsData.Item(flightIndex).Item("BInd") = "", 0, detailsData.Item(flightIndex).Item("BInd"))
                    SDouble = IIf(detailsData.Item(flightIndex).Item("SDouble") = "", 0, detailsData.Item(flightIndex).Item("SDouble"))
                    STriCua = IIf(detailsData.Item(flightIndex).Item("STriCua") = "", 0, detailsData.Item(flightIndex).Item("STriCua"))
                    S34Ad = IIf(detailsData.Item(flightIndex).Item("S34Ad") = "", 0, detailsData.Item(flightIndex).Item("S34Ad"))
                    S34Cr = IIf(detailsData.Item(flightIndex).Item("S34Cr") = "", 0, detailsData.Item(flightIndex).Item("S34Cr"))
                    S34Bb = IIf(detailsData.Item(flightIndex).Item("S34Bb") = "", 0, detailsData.Item(flightIndex).Item("S34Bb"))
                    SInd = IIf(detailsData.Item(flightIndex).Item("SInd") = "", 0, detailsData.Item(flightIndex).Item("SInd"))

                    IOffer = detailsData.Item(flightIndex).Item("IOffer")
                    OOffer = detailsData.Item(flightIndex).Item("OOffer")
                    BOffer = detailsData.Item(flightIndex).Item("BOffer")
                    SOffer = detailsData.Item(flightIndex).Item("SOffer")

                    query = "SELECT TOP(1) idFlight "
                    query &= "FROM " & connTable & " "
                    query &= "ORDER BY 1 DESC"
                    sqlCmd.CommandText = query

                    sqlAdapter.SelectCommand = sqlCmd
                    sqlData.Reset()
                    sqlAdapter.Fill(sqlData)
                    flightId = sqlData.Rows(0).Item("idFlight")

                    connTable = ConfigurationManager.AppSettings.Get("MelairPackPrices") & "Aux"
                    query = "INSERT INTO " & connTable & " VALUES (" & nextFlightId & ", " & IDouble & ", " & ITriCua & ", " & I34Ad & ", " & I34Cr & ", " & I34Bb & ", " & IInd & ", '" & IOffer & "', " & ODouble & ", " & OTriCua & ", " & O34Ad & ", " & O34Cr & ", " & O34Bb & ", " & OInd & ", '" & OOffer & "', " & BDouble & ", " & BTriCua & ", " & B34Ad & ", " & B34Cr & ", " & B34Bb & ", " & BInd & ", '" & BOffer & "', " & SDouble & ", " & STriCua & ", " & S34Ad & ", " & S34Cr & ", " & S34Bb & ", " & SInd & ", '" & SOffer & "')"
                    sqlCmd.CommandText = query
                    sqlCmd.ExecuteNonQuery()

                    connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines") & "Aux"
                    For itemIndex As Integer = 0 To linesData.Item(flightIndex).Count - 1
                        If linesData.Item(flightIndex).Item(itemIndex).Item("Day") <> "" Then
                            day = linesData.Item(flightIndex).Item(itemIndex).Item("Day")
                            lineDescription = linesData.Item(flightIndex).Item(itemIndex).Item("Description")
                            position = linesData.Item(flightIndex).Item(itemIndex).Item("Position")

                            query = "INSERT INTO " & connTable & " VALUES (" & nextFlightId & ", '" & flightCode & "','" & day & "', '" & lineDescription & "', '" & position & "')"
                            sqlCmd.CommandText = query
                            sqlCmd.ExecuteNonQuery()
                        End If
                    Next
                Else
                    Continue For
                End If

                nextFlightId += 1
            Next

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "DELETE FROM " & connTable & " WHERE idPack = " & packId
            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()

            insertFromAuxTables()
            deleteAuxTables()

            sqlConn.Close()
            Return packId
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
            deleteAuxTables()

            errorStr = "Um erro ocorreu durante a modificação da embalagem. Por favor tente novamente. --> ErrDescription= " & Replace(ex.Message, Environment.NewLine, " ")
        End Try

        editPackageOld = errorStr
    End Function

    Public Function editPackage(packageData As Array) As String
        Dim errorStr As String = ""

        Dim basicData As Dictionary(Of String, String) = packageData(0)
        Dim packName, sailDate, itinName, itinCode, destination, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes As String

        packName = basicData.Item("PackageName")
        sailDate = basicData.Item("SailingDate")
        itinName = basicData.Item("ItineraryName")
        itinCode = basicData.Item("ItineraryCode")
        destination = basicData.Item("Destination")
        img1 = basicData.Item("Img1")
        img2 = basicData.Item("Img2")
        img3 = basicData.Item("Img3")
        img4 = basicData.Item("Img4")
        img5 = basicData.Item("Img5")
        dining = basicData.Item("Dining")
        includedTxt = basicData.Item("Included")
        notIncludedTxt = basicData.Item("NotIncluded")
        notes = basicData.Item("Notes")

        Dim shipDescription = basicData.Item("ShipDescription")
        Dim flight = basicData.Item("Flight")
        Dim transfer = basicData.Item("Transfer")
        Dim hotel = basicData.Item("Hotel")

        Dim sqlData As New DataTable
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            query = "UPDATE " & connTable & " SET PackageName = '" & packName & "', SailingDate = CONVERT(datetime, '" & sailDate & "', 103), ItineraryName = '" & itinName & "', ItineraryCode = '" & itinCode & "', Destination = '" & destination & "', Img1 = '" & img1 & "', Img2 = '" & img2 & "', Img3 = '" & img3 & "', Img4 = '" & img4 & "', Img5 = '" & img5 & "', Dining = '" & dining & "', Included = '" & includedTxt & "', NotIncluded = '" & notIncludedTxt & "', Notes = '" & notes & "', ShipDescription = '" & shipDescription & "', Flight = '" & flight & "', Transfer = '" & transfer & "', Hotel = '" & hotel & "'" & " WHERE PackageName = '" & packName & "' AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"

            sqlCmd.CommandText = query
            sqlCmd.ExecuteNonQuery()
            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
            errorStr = "Um erro ocorreu durante a modificação da embalagem. Por favor tente novamente. --> ErrDescription= " & Replace(ex.Message, Environment.NewLine, " ")
        End Try

        editPackage = errorStr
    End Function

    Public Function deletePackage(packName As String, Optional sailDate As String = "") As String
    Dim deletedIds As String = ""

    Dim sqlData As New DataTable
    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
      query = "SELECT idPack "
      query &= "FROM " & connTable & " "
      query &= "WHERE PackageName = '" & packName & "'"
      If sailDate <> "" Then
        query &= " AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"
      End If
      sqlCmd.CommandText = query

      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(sqlData)
      Dim index As Integer = 0
      For Each row In sqlData.Rows
        deletedIds &= row("idPack") & "|"

        index += 1
      Next

      query = "DELETE FROM " & connTable & " "
      query &= "WHERE PackageName = '" & packName & "'"
      If sailDate <> "" Then
        query &= " AND SailingDate = CONVERT(datetime, '" & sailDate & "', 103)"
      End If
      sqlCmd.CommandText = query
      sqlCmd.ExecuteNonQuery()

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
      deletedIds = ""
    End Try

    deletePackage = deletedIds
  End Function

  Public Function insertItineraryLines(packName As String, linesData As Dictionary(Of Integer, Dictionary(Of String, String))) As String
    Dim errorStr As String = ""

    Dim sqlData As New DataTable
    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
      query = "SELECT DISTINCT idPack "
      query &= "FROM " & connTable & " "
      query &= "WHERE PackageName = '" & packName & "'"
      sqlCmd.CommandText = query

      Dim packId, flightId As Integer
      Dim flightCode, day, description, position As String
      Dim sqlData2 As New DataTable

      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(sqlData)
      For Each row In sqlData.Rows
        packId = row("idPack")

        For Each line In linesData.Values
          flightCode = line.Item("Code")
          day = line.Item("Day")
          description = line.Item("Description")
          position = line.Item("Position")

          connTable = ConfigurationManager.AppSettings.Get("MelairFlights")
          query = "SELECT idFlight "
          query &= "FROM " & connTable & " "
          query &= "WHERE idPack = " & packId & " AND FlightCode = '" & flightCode & "'"
          sqlCmd.CommandText = query

          sqlAdapter.SelectCommand = sqlCmd
          sqlData2.Reset()
          sqlAdapter.Fill(sqlData2)
          flightId = sqlData2.Rows(0).Item("idFlight")

          connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines")
          query = "INSERT INTO " & connTable & " "
          query &= "VALUES (" & flightId & ", '" & flightCode & "', '" & day & "', '" & description & "', '" & position & "')"
          sqlCmd.CommandText = query
          sqlCmd.ExecuteNonQuery()
        Next
      Next

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
    End Try

    insertItineraryLines = errorStr
  End Function

  Public Function getItineraryLines(flightId As Integer) As DataTable
    Dim result As New DataTable

    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      connTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines")
      query = "SELECT Day, Description, BeforeAfter "
      query &= "FROM " & connTable & " "
      query &= "WHERE idFlight = " & flightId
      sqlCmd.CommandText = query

      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(result)

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
    End Try

    getItineraryLines = result
  End Function

  Public Function getItineraryDates(itinCode As String, shipCode As String) As String
    Dim result As String = ""

    Dim sqlConn As New SqlConnection
    Dim sqlCmd As New SqlCommand
    Dim sqlAdapter As New SqlDataAdapter
    Dim sqlData As New DataTable

    Dim connTable As String
    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDPricingFeed")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      connTable = ConfigurationManager.AppSettings.Get("BDPricingFeed_Itinerary")
      query = "SELECT DISTINCT SailDate "
      query &= "FROM " & connTable & " "
      query &= "WHERE ItineraryCode = '" & itinCode & "' AND ShipCode = '" & shipCode & "'"
      sqlCmd.CommandText = query
      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(sqlData)

      For Each row In sqlData.Rows
        result &= row("SailDate") & "|"
      Next

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
    End Try

    getItineraryDates = result
  End Function

  Public Function retrievePackageDates(packName As String) As DataTable
    Dim result As New DataTable

    Try
      sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
      sqlConn.Open()
      sqlCmd.Connection = sqlConn

      connTable = ConfigurationManager.AppSettings.Get("MelairPackages")
      query = "SELECT CONVERT(varchar(10), SailingDate, 103) AS SailingDate,idPack "
      query &= "FROM " & connTable & " "
      query &= "WHERE PackageName = '" & packName & "' "
      'query &= "ORDER BY 1"
      sqlCmd.CommandText = query

      sqlAdapter.SelectCommand = sqlCmd
      sqlAdapter.Fill(result)

      sqlConn.Close()
    Catch ex As Exception
      If sqlConn.State <> ConnectionState.Closed Then
        sqlConn.Close()
      End If
    End Try

    retrievePackageDates = result
  End Function

    Public Function getCompanyLogo(compName As String) As String
        Dim result As String = ""

        Dim sqlConn As New SqlConnection
        Dim sqlCmd As New SqlCommand
        Dim sqlAdapter As New SqlDataAdapter
        Dim sqlData As New DataTable

        Dim connTable As String
        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairCompañiasAereas")
            query = "SELECT Logo "
            query &= "FROM " & connTable & " "
            query &= "WHERE Company = '" & compName & "'"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(sqlData)
            result = sqlData.Rows(0).Item("Logo")

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        getCompanyLogo = result
    End Function


    Public Function retrieveCabins(idPack) As DataTable
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairCabinsLinesTest")
            query = "SELECT DISTINCT idCabinLine, idPack, cabinType, category, price, single, triple, quadruple, promoTitle, promoDescription FROM " & connTable & " WHERE idPack = " & "'" & idPack & "'"
            'query = "SELECT DISTINCT idCabinLine, idPack, cabinType, category, price, single, triple, quadruple, promoTitle, promoDescription FROM " & connTable
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrieveCabins = result
    End Function


    Public Sub insertCabin(data As Array)
        Dim basicData As Dictionary(Of String, String) = data(0)
        Dim idPack, cabinType, category, price, _single, triple, quadruple, promoTitle, promoDescription

        idPack = basicData.Item("IdPack")
        cabinType = basicData.Item("CabinType")
        category = basicData.Item("Category")
        price = basicData.Item("Price")
        _single = basicData.Item("Singl")
        triple = basicData.Item("Triple")
        quadruple = basicData.Item("Quadruple")
        promoTitle = basicData.Item("PromoTitle")
        promoDescription = basicData.Item("PromoDescription")

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairCabinsLinesTest")
        query = "INSERT INTO " & connTable & " VALUES ('" & idPack & "', '" & cabinType & "', '" & category & "', '" & price & "' , '" & _single & "', '" & triple & "', '" & quadruple & "', '" & promoTitle & "', '" & promoDescription & "'); SELECT SCOPE_IDENTITY()"
        sqlCmd.CommandText = query
        'sqlCmd.ExecuteNonQuery()

        Dim currentid = sqlCmd.ExecuteScalar()
        sqlConn.Close()
    End Sub

    Public Sub updateCabin(data As Array)
        Dim basicData As Dictionary(Of String, String) = data(0)
        Dim idCabinLine, idPack, cabinType, category, price, _single, triple, quadruple, promoTitle, promoDescription

        idCabinLine = basicData.Item("IdCabinLine")
        idPack = basicData.Item("IdPack")
        cabinType = basicData.Item("CabinType")
        category = basicData.Item("Category")
        price = basicData.Item("Price")
        _single = basicData.Item("Singl")
        triple = basicData.Item("Triple")
        quadruple = basicData.Item("Quadruple")
        promoTitle = basicData.Item("PromoTitle")
        promoDescription = basicData.Item("PromoDescription")

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairCabinsLinesTest")
        query = "UPDATE " & connTable & " SET idPack = '" & idPack & "', cabinType = '" & cabinType & "', category = '" & category & "', price = '" & price & "', single = '" & _single & "', triple = '" & triple & "', quadruple = '" & quadruple & "', promoTitle = '" & promoTitle & "', promoDescription = '" & promoDescription & "'" & " WHERE idCabinLine = " & "'" & idCabinLine & "'"
        sqlCmd.CommandText = query
        sqlCmd.ExecuteNonQuery()
        sqlConn.Close()
    End Sub


    Public Sub deleteCabin(id)

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairCabinsLinesTest")
        query = "DELETE FROM " & connTable & " WHERE idCabinLine = '" & id & "'"
        sqlCmd.CommandText = query
        sqlCmd.ExecuteNonQuery()
        sqlConn.Close()
    End Sub

    Public Sub deleteQuota(idCabinLine, idOrigin, idPack)

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairQuotasTest")
        query = "DELETE FROM " & connTable & " WHERE idCabinLine = '" & idCabinLine & "' And idPack = '" & idPack & "' And idOrigin = '" & idOrigin & "'"
        sqlCmd.CommandText = query
        sqlCmd.ExecuteNonQuery()
        sqlConn.Close()
    End Sub

    Public Function retrieveLinesAll() As DataTable
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairLinesTest")
            query = "Select DISTINCT idPack, idLine, lineType, description, num, before FROM " & connTable
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrieveLinesAll = result
    End Function

    Public Function retrieveLines(lineType As String, idPack As Integer) As DataTable
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairLinesTest")
            'query = "Select DISTINCT idPack, idLine, lineType, description, num, before FROM " & connTable
            query = "Select DISTINCT idPack, idLine, lineType, description, num, before FROM " & connTable & " WHERE lineType = " & "'" & lineType & "'" & " AND idPack = " & "'" & idPack & "'"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrieveLines = result
    End Function

    Public Sub insertLine(data As Array)
        Dim basicData As Dictionary(Of String, String) = data(0)
        Dim idPack, lineType, description, num, before

        idPack = basicData.Item("IdPack")
        lineType = basicData.Item("LineType")
        description = basicData.Item("Description")
        num = basicData.Item("Num")
        before = basicData.Item("Before")

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairLinesTest")
        query = "INSERT INTO " & connTable & " VALUES ('" & idPack & "', '" & lineType & "', '" & description & "', '" & num & "' , '" & before & "'); SELECT SCOPE_IDENTITY()"
        sqlCmd.CommandText = query
        Dim currentid = sqlCmd.ExecuteScalar()
        sqlConn.Close()
    End Sub

    Public Function retrieveOrigins() As DataTable 'Agencias
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            connTable = ConfigurationManager.AppSettings.Get("MelairOriginsTest")
            query = "SELECT DISTINCT idOrigin, name FROM " & connTable
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrieveOrigins = result
    End Function

    Public Sub insertQuota(data As Array)
        Dim basicData As Dictionary(Of String, String) = data(0)
        Dim idPack, idOrigin, idCabinLine, quota

        idPack = basicData.Item("IdPack")
        idOrigin = basicData.Item("IdOrigin")
        idCabinLine = basicData.Item("IdCabinLine")
        quota = basicData.Item("Quota")

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairQuotasTest")
        query = "INSERT INTO " & connTable & " VALUES ('" & idPack & "', '" & idOrigin & "', '" & idCabinLine & "', '" & quota & "'); SELECT SCOPE_IDENTITY()"
        sqlCmd.CommandText = query
        Dim currentid = sqlCmd.ExecuteScalar()
        sqlConn.Close()
    End Sub

    Public Function retrieveQuotas(idPack, idCabinLine) As DataTable 'Agencias
        Dim result As New DataTable

        Try
            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            'connTable = ConfigurationManager.AppSettings.Get("MelairQuotasTest")
            query = "SELECT DISTINCT MelairQuotasTEST.quota, MelairQuotasTEST.idCabinLine, MelairQuotasTEST.idOrigin, MelairQuotasTEST.idPack, MelairOriginsTEST.name FROM MelairQuotasTEST INNER JOIN MelairOriginsTEST 
                     ON MelairQuotasTEST.idOrigin = MelairOriginsTEST.idOrigin" & " WHERE  MelairQuotasTEST.idPack = " & "'" & idPack & "'" & " AND MelairQuotasTEST.idCabinLine = " & "'" & idCabinLine & "'"
            sqlCmd.CommandText = query

            sqlAdapter.SelectCommand = sqlCmd
            sqlAdapter.Fill(result)

            sqlConn.Close()
        Catch ex As Exception
            If sqlConn.State <> ConnectionState.Closed Then
                sqlConn.Close()
            End If
        End Try

        retrieveQuotas = result
    End Function

    Public Sub updateQuota(data As Array)
        Dim basicData As Dictionary(Of String, String) = data(0)
        Dim idPack, idOrigin, idCabinLine, quota

        idPack = basicData.Item("IdPack")
        idOrigin = basicData.Item("IdOrigin")
        idCabinLine = basicData.Item("IdCabinLine")
        quota = basicData.Item("Quota")

        sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
        sqlConn.Open()
        sqlCmd.Connection = sqlConn

        connTable = ConfigurationManager.AppSettings.Get("MelairQuotasTest")

        query = "UPDATE " & connTable & " SET quota = '" & quota & "'" & " WHERE idPack = " & "'" & idPack & "'" & " AND idOrigin = " & "'" & idOrigin & "'" & " AND idCabinLine = " & "'" & idCabinLine & "'"
        sqlCmd.CommandText = query
        Dim currentid = sqlCmd.ExecuteScalar()
        sqlConn.Close()
    End Sub
End Class
