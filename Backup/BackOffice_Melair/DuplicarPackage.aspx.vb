
Imports System.Data
Imports System.Data.SqlClient

Public Class WebForm1
    Inherits System.Web.UI.Page

    Dim sqlAccess As New SQL
    Dim idPack As Integer


    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        idPack = Request.QueryString("idPack")

        fillShips()

    End Sub


    Protected Sub showItinerary(sender As Object, e As EventArgs) Handles searchBtn.Click
        loadPanel.Visible = True

        Dim shipCode As String = shipSelector.Items(shipSelector.SelectedIndex).Value
        Dim sailDate As String = sailField.Text

        If shipCode <> "-1" And sailDate <> "" Then
            Dim dbData As DataTable = sqlAccess.getItinerary(shipCode, sailDate)

            If (Not dbData Is Nothing) And (dbData.Rows.Count > 0) Then
                'emptyItin()

                Session("shipCode") = shipCode
                Session("sailDate") = sailDate
                Session("itinCode") = dbData.Rows(0).Item("ItineraryCode")
                Dim nights As String = dbData.Rows(0).Item("Nnoches")

                Session("itinBackup") = ""
                Dim day As Integer = 1
                For Each row In dbData.Rows
                    Dim tblRow As New TableRow
                    Dim dayTc, portTc, arrTc, depTc As New TableCell

                    Dim arrTime As String = dbData.Rows.Item(day - 1).Item("ArrivalTime")
                    Dim depTime As String = dbData.Rows.Item(day - 1).Item("DepartureTime")

                    dayTc.Text = day
                    portTc.Text = dbData.Rows.Item(day - 1).Item("LocationName")
                    If portTc.Text <> "NAVEGAÇÃO" Then
                        If day = 1 Then
                            arrTc.Text = "-"
                            depTc.Text = depTime.Substring(0, 2) & ":" & depTime.Substring(2, 2)
                        ElseIf day = dbData.Rows.Count Then
                            arrTc.Text = arrTime.Substring(0, 2) & ":" & arrTime.Substring(2, 2)
                            depTc.Text = "-"
                        Else
                            arrTc.Text = arrTime.Substring(0, 2) & ":" & arrTime.Substring(2, 2)
                            depTc.Text = depTime.Substring(0, 2) & ":" & depTime.Substring(2, 2)
                        End If
                    Else
                        arrTc.Text = "-"
                        depTc.Text = "-"
                    End If

                    tblRow.Cells.Add(dayTc)
                    tblRow.Cells.Add(portTc)
                    tblRow.Cells.Add(arrTc)
                    tblRow.Cells.Add(depTc)
                    itinTbl.Rows.Add(tblRow)
                    itinTbl.Style.Add("text-align", "center")

                    ' Mantendremos el itinerario en una variable que se usará para recuperarlo en futuros PostBack de la web
                    ' FORMATO de entrada -> Día|Puerto|Hora de llegada|Hora de salida;Día|Puerto|Hora de llegada|Hora de salida;...
                    Session("itinBackup") &= dayTc.Text & "|" & portTc.Text & "|" & arrTc.Text & "|" & depTc.Text & ";"
                    day += 1
                Next

                nightsText.Text = nights
                destinyText.Text = sqlAccess.getItineraryDestination(sailDate, shipCode, Session("itinCode"))
                registerDiv.Style.Item("width") = "1383px"
                hideDiv.Style.Remove("display")

                shipSelector.Enabled = False
                sailField.Enabled = False
                searchBtn.Style.Add("display", "none")
                basicDiv.Style.Item("padding-bottom") = "15px;"
                registerBtn.Style.Item("color") = "White;"
                registerBtn.Attributes.Add("onmouseover", "")
                registerBtn.Enabled = True
            Else
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Nada encontrado nenhuma tomada a data e barco selecionada. Por favor, tente novamente.');", True)
            End If
        Else
            'popup.Visible = True
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Você precisa selecionar um barco e partida antes de pressionar o botão. Por favor, tente novamente.');", True)
        End If

        loadPanel.Visible = False
    End Sub

    Protected Sub goBack(sender As Object, e As EventArgs) Handles backBtn.Click
        Response.Redirect("Packages.aspx")
    End Sub

    Protected Sub registerPackage(sender As Object, e As EventArgs) Handles registerBtn.Click
        Dim strShip, strNombre, strFecha As String
        Dim packTable, fightTable, priceTable, linesTable, destination, strQuery As String

        strShip = shipSelector.SelectedValue
        strFecha = sailField.Text
        strNombre = nuevoNombre.Text

        'En Session("itinCode") queda el código de itinerario que se usará para traer todas las fechas del par barco/itinerary code

        Try

            Dim itinDates As String = sqlAccess.getItineraryDates(Session("itinCode"), strShip)
            destination = sqlAccess.getDestination(Session("itinCode"), strShip, strFecha)
            Dim howManyDates As Integer = 0

            packTable = ConfigurationManager.AppSettings.Get("MelairPackages")
            fightTable = ConfigurationManager.AppSettings.Get("MelairFlights")
            priceTable = ConfigurationManager.AppSettings.Get("MelairPackPrices")
            linesTable = ConfigurationManager.AppSettings.Get("MelairItineraryLines")

            Dim sqlConn As New SqlConnection
            Dim sqlCmd As New SqlCommand
            Dim sqlAdapter As New SqlDataAdapter
            Dim sqlData As New DataTable
            Dim newidPack As Integer

            sqlConn.ConnectionString = ConfigurationManager.AppSettings.Get("BDSalidas")
            sqlConn.Open()
            sqlCmd.Connection = sqlConn

            For Each sail In itinDates.Split("|")
                If sail <> "" Then                    
                    sqlData.Clear()
                    strQuery = "select * from " & packTable & " where idPack=" & idPack
                    sqlCmd.CommandText = strQuery
                    sqlAdapter.SelectCommand = sqlCmd
                    sqlAdapter.Fill(sqlData)

                    For Each row In sqlData.Rows

                        strQuery = "insert into " & packTable & " VALUES ("
                        strQuery &= "'" & strNombre & "','" & strShip & "',convert(datetime, '" & sail & "', 103),"
                        strQuery &= "'" & row("ItineraryName") & "','" & Session("itinCode") & "','" & destination & "',"
                        strQuery &= "'" & row("Img1") & "','" & row("Img2") & "','" & row("Img3") & "','" & row("Img4") & "','" & row("Img5") & "',"
                        strQuery &= "'" & row("Dining") & "','" & row("Included") & "','" & row("NotIncluded") & "','" & row("Notes") & "'); SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]; "
                        sqlCmd.CommandText = strQuery
                        newidPack = sqlCmd.ExecuteScalar()
                        'En newidPack quedará el idPackCreado. Hay que copiar las imagenes a la carpeta con ese nombre. Luego insertar los vuelos, precios y líneas de itinerario

                        IO.Directory.CreateDirectory(Server.MapPath("Images/Uploads/" & newidPack))
                        Dim images As String() = IO.Directory.GetFiles(Server.MapPath("Images/Uploads/" & idPack))

                        For Each image In images
                            Dim splitImage() As String = image.Split("\")
                            Dim fileName As String = splitImage(splitImage.Length - 1)
                            IO.File.Copy(image, Server.MapPath("Images/Uploads/" & newidPack & "/" & fileName))
                        Next

                        Dim sqlCmd1 As New SqlCommand
                        Dim sqlAdapter1 As New SqlDataAdapter
                        Dim sqlData1 As New DataTable

                        Dim fechaVuelo As Date
                        Dim nDiasVuelo As Double
                        nDiasVuelo = (-(CDbl(rblDiasVuelo.SelectedValue)))
                        fechaVuelo = DateAdd(DateInterval.Day, nDiasVuelo, CDate(sail))


                Dim newIdVuelo As Integer

                strQuery = "select idFlight from " & fightTable & " where idPack=" & idPack

                sqlData1.Clear()
                sqlCmd1.Connection = sqlConn
                sqlCmd1.CommandText = strQuery
                sqlAdapter1.SelectCommand = sqlCmd1
                sqlAdapter1.Fill(sqlData1)

                For Each vuelo In sqlData1.Rows

                    strQuery = "insert into " & fightTable
                    strQuery &= " select " & newidPack & ",Description, Company, Logo, City, FlightCode, convert(datetime, '" & fechaVuelo & "', 103) , FlightTime, FlightTaxes, ReturnFlight, ReturnFlightTime,"
                    strQuery &= " FlightTransfer, FlightHotel, OfferDescription, CabinsperCity, FlightStatus from " & fightTable & " where idPack=" & idPack & "; SELECT SCOPE_IDENTITY() AS [SCOPE_IDENTITY]; "

                    sqlCmd1.CommandText = strQuery
                    newIdVuelo = sqlCmd1.ExecuteScalar()

                    strQuery = "insert into " & priceTable
                    strQuery &= " select " & newIdVuelo & ","
                    strQuery &= "IDouble, ITriCua, I34Ad, I34Cr, I34Bb, IInd, IOffer, ODouble, OTriCua, O34Ad, O34Cr, O34Bb, OInd, OOffer, BDouble, BTriCua, B34Ad, B34Cr, B34Bb, BInd, BOffer,"
                    strQuery &= " SDouble, STriCua, S34Ad, S34Cr, S34Bb, SInd, SOffer from " & priceTable & " where idFlight=" & vuelo("idFlight")
                    sqlCmd1.CommandText = strQuery
                    sqlCmd1.ExecuteNonQuery()

                    strQuery = "insert into " & linesTable
                    strQuery &= " select " & newIdVuelo & ","
                    strQuery &= "FlightCode, Day, Description, BeforeAfter from " & linesTable & " where idFlight=" & vuelo("idFlight")
                    sqlCmd1.CommandText = strQuery
                    sqlCmd1.ExecuteNonQuery()
                Next

            Next

                End If
            Next
            sqlConn.Close()
            Session("DupMsg") = 1
            Response.Redirect("Packages.aspx?Dup=1")
        Catch ex As Exception
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "show", "showPopup('Error', " & ex.Message.ToString & ");", True)
        End Try


    End Sub

    Private Sub fillShips()
        Dim shipList As DataTable = sqlAccess.retrieveShips()

        For Each ship In shipList.Rows
            shipSelector.Items.Add(New ListItem(ship("ShipName"), ship("ShipCode")))
        Next
    End Sub


    Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "$('#popup').hide();", True)

    End Sub

End Class