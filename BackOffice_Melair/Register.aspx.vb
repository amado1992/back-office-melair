Public Class Register1
    Inherits System.Web.UI.Page

    Dim sqlAccess As New SQL


    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
        'If Session("login") <> "Y" Then
        '    Response.Redirect("Login.aspx")
        'End If

        Page.MaintainScrollPositionOnPostBack = True

        If Not IsPostBack Then
            Dim flightsData As New Dictionary(Of Integer, Dictionary(Of String, String))
            Dim detailsData As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String)))
            Dim pendingDetails As New Dictionary(Of Integer, Char)

            Session("img1") = ""
            Session("img2") = ""
            Session("img3") = ""
            Session("img4") = ""
            Session("img5") = ""
            Session("itinCode") = ""
            Session("itinBackup") = ""
            Session("repDone") = "N"
            Session("flightsData") = flightsData
            Session("detailsData") = detailsData
            Session("pendingData") = pendingDetails

            fillShips()
            initGrid()

            notesField.Text = "• Com a confirmação da reserva, deve efetuar o pagamento do depósito no valor de €300 por pessoa." & Environment.NewLine() & Environment.NewLine() &
                              "• Camarotes com ocupação tripla ou quádrupla deve solicitar uma proposta à medida." & Environment.NewLine() & Environment.NewLine() &
                              "• 1 criança a partilhar o camarote com 1 adulto, terá a redução correspondente à parte aérea." & Environment.NewLine() & Environment.NewLine() &
                              "• Todos os camarotes são de Categoria Garantida.  A atribuição do número do camarote é feita pela Companhia à sua descrição e poderá acontecer até à hora de embarque. A mesma garante sempre a categoria mínima reservada, podendo ser atribuído um camarote em categoria superior sem custos extras. Depois do camarote atribuído não são aceites quaisquer alterações pedidas pelo passageiro. Em nenhum momento serão efetuadas mudanças de camarote depois do cruzeiro iniciado." & Environment.NewLine() & Environment.NewLine() &
                              "• Turno de refeição: 20h45."

            If Not IO.Directory.Exists(Server.MapPath("Images/Temp")) Then
                IO.Directory.CreateDirectory(Server.MapPath("Images/Temp"))
            Else
                Dim images() As String = IO.Directory.GetFiles(Server.MapPath("Images/Temp"))
                For Each file In images
                    IO.File.Delete(file)
                Next
            End If
        Else
            recoverItinerary()
        End If
    End Sub

    Private Sub fillShips()
        Dim shipList As DataTable = sqlAccess.retrieveShips()

        For Each ship In shipList.Rows
            shipSelector.Items.Add(New ListItem(ship("ShipName"), ship("ShipCode")))
        Next
    End Sub

    ' Procedimiento que rellena por primera vez el GridView con una fila para introducir información
    Private Sub initGrid()
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Companhia"))
        dt.Columns.Add(New DataColumn("Logo"))
        dt.Columns.Add(New DataColumn("Cidade"))
        dt.Columns.Add(New DataColumn("Cidade DESTINO"))
        dt.Columns.Add(New DataColumn("Descrição"))
        dt.Columns.Add(New DataColumn("Número de voos"))
        dt.Columns.Add(New DataColumn("Quota"))
        dt.Columns.Add(New DataColumn("Estado"))

        dr = dt.NewRow()
        dt.Rows.Add(dr)

        Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
        Dim pendingDetails As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
        detailsData.Add(0, New Dictionary(Of Integer, Dictionary(Of String, String)))
        pendingDetails.Add(0, "Y")
        Session("detailsData") = detailsData
        Session("pendingData") = pendingDetails

        Session("dataBackup") = dt
        fillGrid(dt)
    End Sub

    Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "$('#popup').hide();", True)

        If Session("pendingRep") = "Y" Then
            Session("pendingRep") = ""
            prices.Visible = True
        End If
        If Session("pendingDetails") = "Y" Then
            Session("pendingDetails") = ""
            prices.Visible = True
        End If
        If Session("packageRegistered") = "Y" Then
            Session("packageRegistered") = ""
            Response.Redirect("Extra.aspx")
        End If
    End Sub

    ' Procedimiento que recoge el itinerario a partir de la BD y lo muestra en la tabla reservada para ello
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

                For Each fila As TableRow In itinTbl.Rows
                    fila.Cells(1).Font.Size = 9
                Next

                nightsText.Text = nights
                destinyText.Text = sqlAccess.getItineraryDestination(sailDate, shipCode, Session("itinCode"))
                'registerDiv.Style.Item("width") = "1383px"
                hideDiv.Style.Remove("display")
                col1El2Hide.Style.Remove("display")
                col2Hide.Style.Remove("display")
                col3Hide.Style.Remove("display")
                extraData.Style.Remove("display")
                idHide1.Style.Remove("display")

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

    ' Procedimiento que recupera y carga el itinerario previamente obtenido para mantenerlo entre los distintos PostBack que puedan ocurrir en la página
    Private Sub recoverItinerary()
        If Session("itinBackup") <> "" Then
            Dim itinStr As String = CType(Session("itinBackup"), String)

            Dim splitScale() As String

            ' Entrada -> Día|Puerto|Hora de llegada|Hora de salida;Día|Puerto|Hora de llegada|Hora de salida;...
            ' Variable "scale" -> (Día|Puerto|Hora de llegada|Hora de salida), (Día|Puerto|Hora de llegada|Hora de salida), ...
            ' Variable "splitScale" -> (Día), (Puerto), (Hora de llegada), (Hora de salida) 
            For Each scale In itinStr.Split(";")
                If scale <> "" Then
                    Dim tblRow As New TableRow
                    Dim dayTc, portTc, arrTc, depTc As New TableCell

                    splitScale = scale.Split("|")

                    dayTc.Text = splitScale(0)
                    portTc.Text = splitScale(1)
                    arrTc.Text = splitScale(2)
                    depTc.Text = splitScale(3)

                    tblRow.Cells.Add(dayTc)
                    tblRow.Cells.Add(portTc)
                    tblRow.Cells.Add(arrTc)
                    tblRow.Cells.Add(depTc)
                    itinTbl.Rows.Add(tblRow)
                End If
            Next
        End If
    End Sub

    Protected Sub goBack(sender As Object, e As EventArgs) Handles backBtn.Click
        Response.Redirect("Packages.aspx")
    End Sub

    Private Sub fillGrid(dt As DataTable)
        gridView.DataSource = dt
        gridView.DataBind()
    End Sub

    ' Función que comprueba si una determinada fila de la tabla de vuelos ha sido rellenada con la información básica
    Private Function rowIsFilled(Optional rowIndex As Integer = -1) As Boolean
        Dim result As Boolean = True

        Dim descField, numField, quotaField, destinoField As TextBox
        Dim citySelector As DropDownList

        If rowIndex <> -1 Then
            citySelector = CType(gridView.Rows(rowIndex).Cells(1).FindControl("citySelector"), DropDownList)
            destinoField = CType(gridView.Rows(rowIndex).Cells(2).FindControl("destinoField"), TextBox)
            descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
            numField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
            quotaField = CType(gridView.Rows(rowIndex).Cells(5).FindControl("quotaField"), TextBox)

            If (citySelector.Items(citySelector.SelectedIndex).Value = "-1") Or (descField.Text = "") Or (numField.Text = "") Or (quotaField.Text = "") Then
                result = False
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na linha " & (rowIndex + 1) & " antes de prosseguir.');", True)
            End If
        Else
            For i As Integer = 0 To gridView.Rows.Count - 1
                citySelector = CType(gridView.Rows(i).Cells(1).FindControl("citySelector"), DropDownList)

                descField = CType(gridView.Rows(i).Cells(2).FindControl("descField"), TextBox)
                numField = CType(gridView.Rows(i).Cells(3).FindControl("numField"), TextBox)
                quotaField = CType(gridView.Rows(i).Cells(4).FindControl("quotaField"), TextBox)

                If (citySelector.Items(citySelector.SelectedIndex).Value = "-1") Or (descField.Text = "") Or (numField.Text = "") Or (quotaField.Text = "") Then
                    'popup.Visible = True
                    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na tabela antes de prosseguir.');", True)
                    Return False
                End If
            Next
        End If

        rowIsFilled = result
    End Function

    Private Function createCustomSelector(name As String) As DropDownList
        Dim result As New DropDownList

        Select Case name
            Case "citySelector"
                result.Items.Add(New ListItem("Selecione uma cidade", "-1"))
                result.Items.Add(New ListItem("Lisboa (Portugal)", "LIS"))
                result.Items.Add(New ListItem("Porto (Portugal)", "OPO"))
                result.Items.Add(New ListItem("Madrid (Espanha)", "MAD"))
                result.Items.Add(New ListItem("Estocolmo (Suecia)", "STO"))
            Case "statusSelector"
                result.Items.Add(New ListItem("GTD", "GTD"))
                result.Items.Add(New ListItem("RQ", "RQ"))
        End Select
        result.Items(0).Selected = True
        result.ID = name

        createCustomSelector = result
    End Function

    ' Procedimiento que añade una fila a la tabla de vuelos, a la vez que mantiene toda la información presente en la misma entre PostBacks
    Protected Sub addRow(sender As Object, e As EventArgs) Handles flightBtn.Click
        Dim howMany As Integer = gridView.Rows.Count
        If rowIsFilled() Then
            If Not Session("dataBackup") Is Nothing Then
                Dim dtCurrentTable As DataTable = CType(Session("dataBackup"), DataTable)
                Dim drCurrentRow As DataRow = Nothing

                If dtCurrentTable.Rows.Count > 0 Then
                    Dim comp, logo, city, desc, cityDest, num, quota, status As String

                    Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                    Dim flightData As Dictionary(Of String, String)

                    For rowIndex As Integer = 0 To dtCurrentTable.Rows.Count - 1
                        Dim compSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
                        Dim logoImg As HtmlImage = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
                        Dim citySelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
                        Dim destinoField As TextBox = CType(gridView.Rows(rowIndex).Cells(3).FindControl("destinoField"), TextBox)
                        Dim descField As TextBox = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
                        Dim numField As TextBox = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
                        Dim quotaField As TextBox = CType(gridView.Rows(rowIndex).Cells(5).FindControl("quotaField"), TextBox)
                        Dim statusSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(6).FindControl("statusSelector"), DropDownList)

                        drCurrentRow = dtCurrentTable.NewRow()

                        comp = compSelector.SelectedItem.Text
                        logo = logoImg.Src
                        city = citySelector.SelectedItem.Text
                        cityDest = Trim(destinoField.Text)
                        desc = Trim(descField.Text)
                        num = Trim(numField.Text)
                        quota = Trim(quotaField.Text)
                        status = statusSelector.SelectedItem.Text

                        ' Recolección de valores en las filas existentes hasta el momento
                        dtCurrentTable.Rows(rowIndex)("Companhia") = comp
                        dtCurrentTable.Rows(rowIndex)("Logo") = logo
                        dtCurrentTable.Rows(rowIndex)("Cidade") = city
                        dtCurrentTable.Rows(rowIndex)("Cidade DESTINO") = cityDest
                        dtCurrentTable.Rows(rowIndex)("Descrição") = desc
                        dtCurrentTable.Rows(rowIndex)("Número de voos") = num
                        dtCurrentTable.Rows(rowIndex)("Quota") = quota
                        dtCurrentTable.Rows(rowIndex)("Estado") = status

                        ' Guardamos por cada fila los datos en una variable de tipo Dictionary, para posteriormente añadirla a la variable global "flightsData"
                        flightData = New Dictionary(Of String, String)
                        flightData.Add("Company", comp)
                        flightData.Add("Logo", logo)
                        flightData.Add("City", city)
                        flightData.Add("CityDest", cityDest)
                        flightData.Add("Description", desc)
                        flightData.Add("Number", num)
                        flightData.Add("Quota", quota)
                        flightData.Add("Status", status)
                        If Not flightsData.ContainsKey(rowIndex) Then
                            flightsData.Add(rowIndex, flightData)
                        Else
                            flightsData.Item(rowIndex) = flightData
                        End If
                    Next

                    ' Recreamos los objetos DropDownList que sirven de selectores para la compañía y la ciudad de origen, respectivamente
                    drCurrentRow("Companhia") = "Tap Air Portugal"
                    drCurrentRow("Logo") = "Images/Logos/tap_logo.png"
                    Dim newCitySel As DropDownList = createCustomSelector("citySelector")
                    drCurrentRow("Cidade") = newCitySel.SelectedItem.Text
                    Dim newStatusSel As DropDownList = createCustomSelector("statusSelector")
                    drCurrentRow("Estado") = newStatusSel.SelectedItem.Text
                    dtCurrentTable.Rows.Add(drCurrentRow) ' Adición de la nueva fila vacía

                    Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
                    Dim detailsMap As New Dictionary(Of Integer, Dictionary(Of String, String))
                    Dim detailData As New Dictionary(Of String, String)
                    detailData.Add("Code", "")
                    detailsMap.Add(0, detailData)
                    If Not detailsData.ContainsKey(howMany) Then
                        detailsData.Add(howMany, detailsMap)
                    Else
                        detailsData.Item(howMany) = detailsMap
                    End If
                    Session("detailsData") = detailsData
                    Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
                    If Not pendingData.ContainsKey(howMany) Then
                        pendingData.Add(howMany, "Y") ' Marcamos la nueva fila como pendiente de rellenar sus detalles
                    Else
                        pendingData.Item(howMany) = "Y"
                    End If
                    Session("pendingData") = pendingData

                    Session("flightsData") = flightsData
                    Session("dataBackup") = dtCurrentTable
                    fillGrid(dtCurrentTable)
                Else
                    initGrid()
                    Exit Sub
                End If
            Else
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Houve um erro na memória.');", True)
            End If

            setPreviousData()
        End If
    End Sub

    Private Sub addRemoveRow(sender As Object, e As EventArgs) Handles tipoVuelo.TextChanged

        Dim drCurrentRow As DataRow = Nothing
        Dim dtCurrentTable As DataTable = CType(Session("dataBackup"), DataTable)
        Dim howMany As Integer = gridView.Rows.Count

        If tipoVuelo.Text.Equals("0") Then
            dtCurrentTable.Rows.RemoveAt(1)
            Session("dataBackup") = dtCurrentTable
            fillGrid(dtCurrentTable)
        Else
            If howMany > 0 And howMany < 2 Then
                Dim comp, logo, city, desc, cityDest, num, quota, status As String

                Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                Dim flightData As Dictionary(Of String, String)

                For rowIndex As Integer = 0 To dtCurrentTable.Rows.Count - 1
                    Dim compSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
                    Dim logoImg As HtmlImage = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
                    Dim citySelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
                    Dim destinoField As TextBox = CType(gridView.Rows(rowIndex).Cells(3).FindControl("destinoField"), TextBox)
                    Dim descField As TextBox = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
                    Dim numField As TextBox = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
                    Dim quotaField As TextBox = CType(gridView.Rows(rowIndex).Cells(5).FindControl("quotaField"), TextBox)
                    Dim statusSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(6).FindControl("statusSelector"), DropDownList)

                    drCurrentRow = dtCurrentTable.NewRow()

                    comp = compSelector.SelectedItem.Text
                    logo = logoImg.Src
                    city = citySelector.SelectedItem.Text
                    cityDest = Trim(destinoField.Text)
                    desc = Trim(descField.Text)
                    num = Trim(numField.Text)
                    quota = Trim(quotaField.Text)
                    status = statusSelector.SelectedItem.Text

                    ' Recolección de valores en las filas existentes hasta el momento
                    dtCurrentTable.Rows(rowIndex)("Companhia") = comp
                    dtCurrentTable.Rows(rowIndex)("Logo") = logo
                    dtCurrentTable.Rows(rowIndex)("Cidade") = city
                    dtCurrentTable.Rows(rowIndex)("Cidade DESTINO") = cityDest
                    dtCurrentTable.Rows(rowIndex)("Descrição") = desc
                    dtCurrentTable.Rows(rowIndex)("Número de voos") = num
                    dtCurrentTable.Rows(rowIndex)("Quota") = quota
                    dtCurrentTable.Rows(rowIndex)("Estado") = status

                    ' Guardamos por cada fila los datos en una variable de tipo Dictionary, para posteriormente añadirla a la variable global "flightsData"
                    flightData = New Dictionary(Of String, String)
                    flightData.Add("Company", comp)
                    flightData.Add("Logo", logo)
                    flightData.Add("City", city)
                    flightData.Add("CityDest", cityDest)
                    flightData.Add("Description", desc)
                    flightData.Add("Number", num)
                    flightData.Add("Quota", quota)
                    flightData.Add("Status", status)
                    If Not flightsData.ContainsKey(rowIndex) Then
                        flightsData.Add(rowIndex, flightData)
                    Else
                        flightsData.Item(rowIndex) = flightData
                    End If
                Next

                ' Recreamos los objetos DropDownList que sirven de selectores para la compañía y la ciudad de origen, respectivamente
                drCurrentRow("Companhia") = "Tap Air Portugal"
                drCurrentRow("Logo") = "Images/Logos/tap_logo.png"
                Dim newCitySel As DropDownList = createCustomSelector("citySelector")
                drCurrentRow("Cidade") = newCitySel.SelectedItem.Text
                Dim newStatusSel As DropDownList = createCustomSelector("statusSelector")
                drCurrentRow("Estado") = newStatusSel.SelectedItem.Text
                dtCurrentTable.Rows.Add(drCurrentRow) ' Adición de la nueva fila vacía

                Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
                Dim detailsMap As New Dictionary(Of Integer, Dictionary(Of String, String))
                Dim detailData As New Dictionary(Of String, String)
                detailData.Add("Code", "")
                detailsMap.Add(0, detailData)
                If Not detailsData.ContainsKey(howMany) Then
                    detailsData.Add(howMany, detailsMap)
                Else
                    detailsData.Item(howMany) = detailsMap
                End If
                Session("detailsData") = detailsData
                Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
                If Not pendingData.ContainsKey(howMany) Then
                    pendingData.Add(howMany, "Y") ' Marcamos la nueva fila como pendiente de rellenar sus detalles
                Else
                    pendingData.Item(howMany) = "Y"
                End If
                Session("pendingData") = pendingData

                Session("flightsData") = flightsData
                Session("dataBackup") = dtCurrentTable
                fillGrid(dtCurrentTable)
            Else
                'initGrid()
                Exit Sub
            End If
        End If
    End Sub
    ' Procedimiento que controla el evento de borrado de filas en la tabla de vuelos
    Protected Sub deleteRow(sender As Object, e As GridViewDeleteEventArgs)
        Dim rowIndex As Integer = e.RowIndex

        Dim currentDt As DataTable = CType(Session("dataBackup"), DataTable)
        Dim currentFlights As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
        Dim currentDetails As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
        Dim currentPending As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))

        currentDt.Rows.RemoveAt(rowIndex)
        currentFlights.Remove(rowIndex)
        currentDetails.Remove(rowIndex)
        currentPending.Remove(rowIndex)

        Session("flightsData") = currentFlights
        Session("detailsData") = currentDetails
        Session("pendingData") = currentPending
        Session("dataBackup") = currentDt
        fillGrid(currentDt)

        setPreviousData()
    End Sub

    Protected Sub deleteFlight(sender As Object, e As EventArgs)
        Dim savedData As New Dictionary(Of Integer, Dictionary(Of String, String))
        Dim detailsData As Dictionary(Of String, String)

        ' Para saber en qué item del Repeater se encuentra el botón pulsado, habrá que obtener su índice (la posición dentro del popup)
        Dim btn As Button = CType(sender, Button)
        Dim repeaterItem As RepeaterItem = CType(btn.NamingContainer, RepeaterItem)
        Dim selectedIndex As Integer = repeaterItem.ItemIndex

        ' Una vez obtenido dicho índice cuyo item contiene el vuelo a eliminar, almacenamos los datos del resto
        For Each repItem As RepeaterItem In rep.Items
            Dim itemIndex As Integer = repItem.ItemIndex
            detailsData = New Dictionary(Of String, String)

            If itemIndex <> selectedIndex Then
                Dim basicDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightBasicDiv"), HtmlGenericControl)
                Dim codeField As TextBox = CType(basicDiv.FindControl("flightCode"), TextBox)
                Dim dateField As TextBox = CType(basicDiv.FindControl("flightDate"), TextBox)
                Dim hourField As TextBox = CType(basicDiv.FindControl("flightHour"), TextBox)
                Dim taxesField As TextBox = CType(basicDiv.FindControl("flightTaxes"), TextBox)
                Dim returnFlight As TextBox = CType(basicDiv.FindControl("returnFlight"), TextBox)
                Dim returnFlightTime As TextBox = CType(basicDiv.FindControl("returnFlightTime"), TextBox)

                Dim extraDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightExtraDiv"), HtmlGenericControl)
                Dim transferField As TextBox = CType(extraDiv.FindControl("flightTransfer"), TextBox)
                Dim hotelField As TextBox = CType(extraDiv.FindControl("flightHotel"), TextBox)
                Dim offerField As TextBox = CType(extraDiv.FindControl("offerField"), TextBox)

                Dim code As String = Trim(codeField.Text)
                Dim flightDate As String = Trim(dateField.Text)
                Dim hour As String = Trim(hourField.Text)
                Dim taxes As String = Trim(taxesField.Text)
                Dim retFlight As String = Trim(returnFlight.Text)
                Dim retFlightTime As String = Trim(returnFlightTime.Text)
                Dim transfer As String = Trim(transferField.Text)
                Dim hotel As String = Trim(hotelField.Text)
                Dim offer As String = Trim(offerField.Text)
                detailsData.Add("Code", code)
                detailsData.Add("Date", flightDate)
                detailsData.Add("Hour", hour)
                detailsData.Add("Taxes", taxes)
                detailsData.Add("Return", retFlight)
                'detailsData.Add("ReturnTime", retFlightTime)
                detailsData.Add("Transfer", transfer)
                detailsData.Add("Hotel", hotel)
                detailsData.Add("Offer", offer)

        Dim pricesTbl As Table = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightPricesDiv").FindControl("pricesTbl"), Table)
                Dim controlId As String = ""
                Dim ctrl As TextBox
                Dim prices As String = ""
                For rowIndex As Integer = 1 To 4
                    For colIndex As Integer = 1 To 6
                        Select Case rowIndex
                            Case 1
                                controlId = "int" & colIndex
                            Case 2
                                controlId = "ext" & colIndex
                            Case 3
                                controlId = "bal" & colIndex
                            Case 4
                                controlId = "su" & colIndex
                        End Select

                        ctrl = CType(pricesTbl.Rows(rowIndex).Cells(colIndex).FindControl(controlId), TextBox)
                        prices &= Trim(ctrl.Text) & "|"
                    Next
                Next

                detailsData.Add("Prices", prices)

                Dim discounts As String = ""
                For rowIndex As Integer = 1 To 4
                    Select Case rowIndex
                        Case 1
                            controlId = "int7"
                        Case 2
                            controlId = "ext7"
                        Case 3
                            controlId = "bal7"
                        Case 4
                            controlId = "su7"
                    End Select

                    ctrl = CType(pricesTbl.Rows(rowIndex).Cells(7).FindControl(controlId), TextBox)
                    discounts &= Trim(ctrl.Text) & "|"
                Next

                detailsData.Add("Discounts", discounts)
            Else
                detailsData.Add("Code", "")
                detailsData.Add("Date", "")
                detailsData.Add("Hour", "")
                detailsData.Add("Taxes", "")
                detailsData.Add("Return", "")
                'detailsData.Add("ReturnTime", "")
                detailsData.Add("Transfer", "")
                detailsData.Add("Hotel", "")
                detailsData.Add("Offer", "")
                detailsData.Add("Prices", "")
                detailsData.Add("Discounts", "")
            End If

            savedData.Add(itemIndex, detailsData)
        Next

        ' El siguiente paso es reconstruir el Repeater solamente con el resto de vuelos
        Dim howMany As Integer = rep.Items.Count - 1

        Dim repeatTimes(howMany - 1) As Integer
        For index As Integer = 0 To howMany - 1
            repeatTimes.SetValue(index + 1, index)
        Next
        rep.DataSource = repeatTimes
        rep.DataBind()

        ' Rellenamos los campos con la información recogida
        Dim aux As Byte = 0
        For Each repItem As RepeaterItem In rep.Items
            Dim itemIndex As Integer = repItem.ItemIndex

            If savedData.Item(itemIndex).Item("Code") = "" Then
                aux = 1
            End If

            Dim basicDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightBasicDiv"), HtmlGenericControl)
            Dim codeField As TextBox = CType(basicDiv.FindControl("flightCode"), TextBox)
            Dim dateField As TextBox = CType(basicDiv.FindControl("flightDate"), TextBox)
            Dim hourField As TextBox = CType(basicDiv.FindControl("flightHour"), TextBox)
            Dim taxesField As TextBox = CType(basicDiv.FindControl("flightTaxes"), TextBox)
            Dim returnFlight As TextBox = CType(basicDiv.FindControl("returnFlight"), TextBox)
            Dim returnFlightTime As TextBox = CType(basicDiv.FindControl("returnFlightTime"), TextBox)
            Dim extraDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightExtraDiv"), HtmlGenericControl)
            Dim transferField As TextBox = CType(extraDiv.FindControl("flightTransfer"), TextBox)
            Dim hotelField As TextBox = CType(extraDiv.FindControl("flightHotel"), TextBox)
            Dim offerField As TextBox = CType(extraDiv.FindControl("offerField"), TextBox)

            codeField.Text = savedData.Item(itemIndex + aux).Item("Code")
            dateField.Text = savedData.Item(itemIndex + aux).Item("Date")
            hourField.Text = savedData.Item(itemIndex + aux).Item("Hour")
            taxesField.Text = savedData.Item(itemIndex + aux).Item("Taxes")
            returnFlight.Text = savedData.Item(itemIndex + aux).Item("Return")
            returnFlightTime.Text = savedData.Item(itemIndex + aux).Item("ReturnTime")
            transferField.Text = savedData.Item(itemIndex + aux).Item("Transfer")
            hotelField.Text = savedData.Item(itemIndex + aux).Item("Hotel")
            offerField.Text = savedData.Item(itemIndex + aux).Item("Offer")

            Dim pricesTbl As Table = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightPricesDiv").FindControl("pricesTbl"), Table)
            Dim controlId As String = ""
            Dim ctrl As TextBox
            Dim prices() As String = savedData.Item(itemIndex + aux).Item("Prices").Split("|")
            Dim priceIndex As Integer = 0
            For rowIndex As Byte = 1 To 4
                For colIndex As Byte = 1 To 6
                    Select Case rowIndex
                        Case 1
                            controlId = "int" & colIndex
                        Case 2
                            controlId = "ext" & colIndex
                        Case 3
                            controlId = "bal" & colIndex
                        Case 4
                            controlId = "su" & colIndex
                    End Select

                    ctrl = CType(pricesTbl.Rows(rowIndex).Cells(colIndex).FindControl(controlId), TextBox)
                    ctrl.Text = prices(priceIndex)

                    priceIndex += 1
                Next
            Next

            Dim discounts() As String = savedData.Item(itemIndex + aux).Item("Discounts").Split("|")
            Dim discountIndex As Integer = 0
            For rowIndex As Integer = 1 To 4
                Select Case rowIndex
                    Case 1
                        controlId = "int7"
                    Case 2
                        controlId = "ext7"
                    Case 3
                        controlId = "bal7"
                    Case 4
                        controlId = "su7"
                End Select

                ctrl = CType(pricesTbl.Rows(rowIndex).Cells(7).FindControl(controlId), TextBox)
                ctrl.Text = discounts(discountIndex)

                discountIndex += 1
            Next
        Next

        Dim currentDetails As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
        Dim rowSelected As Integer = CType(Session("rowIndex"), Integer)
        currentDetails.Item(rowSelected).Remove(selectedIndex)
        Session("detailsData") = currentDetails
    End Sub

    ' Procedimiento auxiliar de "addRow" y "deleteRow" que sirve para recolocar los valores en los controles que forman la tabla de vuelos
    Private Sub setPreviousData()
        If Not Session("dataBackup") Is Nothing Then
            Dim dt As DataTable = CType(Session("dataBackup"), DataTable)

            If dt.Rows.Count > 0 Then
                Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))

                For rowIndex As Integer = 0 To dt.Rows.Count - 1
                    Dim compSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
                    Dim logoImg As HtmlImage = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
                    Dim citySelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
                    Dim descField As TextBox = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
                    Dim numField As TextBox = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
                    Dim quotaField As TextBox = CType(gridView.Rows(rowIndex).Cells(5).FindControl("quotaField"), TextBox)
                    Dim statusSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(6).FindControl("statusSelector"), DropDownList)

                    Dim comp As String = dt.Rows(rowIndex)("Companhia").ToString()
                    compSelector.ClearSelection()
                    compSelector.Items.FindByText(comp).Selected = True
                    Dim logo As String = dt.Rows(rowIndex)("Logo").ToString()
                    logoImg.Src = logo
                    Dim city As String = dt.Rows(rowIndex)("Cidade").ToString()
                    citySelector.ClearSelection()
                    citySelector.Items.FindByText(city).Selected = True
                    descField.Text = dt.Rows(rowIndex)("Descrição").ToString()
                    numField.Text = dt.Rows(rowIndex)("Número de voos").ToString()
                    quotaField.Text = dt.Rows(rowIndex)("Quota").ToString()
                    Dim status As String = dt.Rows(rowIndex)("Estado").ToString()
                    statusSelector.ClearSelection()
                    statusSelector.Items.FindByText(status).Selected = True
                Next
            End If
        End If
    End Sub

    Protected Sub showDetails(sender As Object, e As EventArgs)
        ' Previo a mostrar el popup de precios, se hace una comprobación para ver si el usuario ha rellenado la fila asociada.
        ' La manera de obtener el índice correspondiente a la fila es realizarle un Cast al contenedor que incluye el botón
        Dim btn As Button = CType(sender, Button)
        Dim btnRow As GridViewRow = CType(btn.NamingContainer, GridViewRow)

        Dim rowIndex As Integer = btnRow.RowIndex
        Session("rowIndex") = rowIndex

        Dim nDiasVuelos As Double
        nDiasVuelos = rblDiasVuelo.SelectedValue
        nDiasVuelos = -nDiasVuelos
        Session("nDiasVuelos") = nDiasVuelos

        Dim fechaVuelo As Date = sailField.Text
        fechaVuelo = DateAdd(DateInterval.Day, nDiasVuelos, fechaVuelo)

        If rowIsFilled(rowIndex) Then
            Dim numField As TextBox = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
            Dim numValue As Integer = Trim(numField.Text)

            Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
            If pendingData.Item(rowIndex) = "Y" Then
                ' Inicialización del control Repeater para mostrar tantas secciones como número de vuelos escogido previamente
                Dim repeatTimes(numValue - 1) As Integer
                For index As Integer = 0 To numValue - 1
                    repeatTimes.SetValue(index + 1, index)
                Next
                rep.DataSource = repeatTimes
                rep.DataBind()

                ' Antes de mostrar el popup al usuario, ocultamos la segunda sección en adelante (si es más de un vuelo)
                If numValue > 1 Then
                    For itemIndex As Integer = 1 To numValue - 1
                        Dim flightDiv As HtmlGenericControl = CType(rep.Items(itemIndex).FindControl("flightDetailsDiv"), HtmlGenericControl)
                        flightDiv.Style.Add("display", "none")
                    Next
                End If
            Else
                Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
                numValue = detailsData.Item(rowIndex).Count

                Dim repeatTimes(numValue - 1) As Integer
                For index As Integer = 0 To numValue - 1
                    repeatTimes.SetValue(index + 1, index)
                Next
                rep.DataSource = repeatTimes
                rep.DataBind()

                fillExistentDetails()
                Session("repDone") = "Y"
            End If


            For Each repItem As RepeaterItem In rep.Items
                Dim basicDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightBasicDiv"), HtmlGenericControl)
                Dim dateField As TextBox = CType(basicDiv.FindControl("flightDate"), TextBox)
                dateField.Text = fechaVuelo.ToString
            Next

            'rep_flightDate_0

            prices.Visible = True
        Else
            'popup.Visible = True
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na linha antes de adicionar a informação extra de/dos vôo/s.');", True)
        End If
    End Sub

    Private Sub fillExistentDetails()
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
        Dim rowIndex As Integer = CInt(Session("rowIndex"))

        Dim basicDiv, extraDiv As HtmlGenericControl
        Dim pricesTbl As Table
        Dim flightCode, flightDate, flightHour, flightTaxes, returnFlight, returnFlightTime, flightTransfer, flightHotel, flightOffer As TextBox
        Dim int1, int2, int3, int4, int5, int6, int7, ext1, ext2, ext3, ext4, ext5, ext6, ext7, bal1, bal2, bal3, bal4, bal5, bal6, bal7, su1, su2, su3, su4, su5, su6, su7 As TextBox

        Dim itemIndex As Integer = 0
        For Each repItem As RepeaterItem In rep.Items
            basicDiv = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightBasicDiv"), HtmlGenericControl)
            flightCode = CType(basicDiv.FindControl("flightCode"), TextBox)
            flightDate = CType(basicDiv.FindControl("flightDate"), TextBox)
            flightHour = CType(basicDiv.FindControl("flightHour"), TextBox)
            flightTaxes = CType(basicDiv.FindControl("flightTaxes"), TextBox)
            returnFlight = CType(basicDiv.FindControl("returnFlight"), TextBox)
            returnFlightTime = CType(basicDiv.FindControl("returnFlightTime"), TextBox)
            extraDiv = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightExtraDiv"), HtmlGenericControl)
            flightTransfer = CType(extraDiv.FindControl("flightTransfer"), TextBox)
            flightHotel = CType(extraDiv.FindControl("flightHotel"), TextBox)
            flightOffer = CType(extraDiv.FindControl("offerField"), TextBox)

            flightCode.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Code")
            flightDate.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Date")
            flightHour.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Hour")
            flightTaxes.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Taxes")
            returnFlight.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Return")
            returnFlightTime.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("ReturnFlightTime")
            flightTransfer.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Transfer")
            flightHotel.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Hotel")
            flightOffer.Text = detailsData.Item(rowIndex).Item(itemIndex).Item("Offer")

            pricesTbl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightPricesDiv").FindControl("pricesTbl"), Table)
            int1 = CType(pricesTbl.Rows(0).Cells(0).FindControl("int1"), TextBox)
            int2 = CType(pricesTbl.Rows(0).Cells(1).FindControl("int2"), TextBox)
            int3 = CType(pricesTbl.Rows(0).Cells(2).FindControl("int3"), TextBox)
            int4 = CType(pricesTbl.Rows(0).Cells(3).FindControl("int4"), TextBox)
            int5 = CType(pricesTbl.Rows(0).Cells(4).FindControl("int5"), TextBox)
            int6 = CType(pricesTbl.Rows(0).Cells(5).FindControl("int6"), TextBox)
            int7 = CType(pricesTbl.Rows(0).Cells(6).FindControl("int7"), TextBox)
            ext1 = CType(pricesTbl.Rows(1).Cells(0).FindControl("ext1"), TextBox)
            ext2 = CType(pricesTbl.Rows(1).Cells(1).FindControl("ext2"), TextBox)
            ext3 = CType(pricesTbl.Rows(1).Cells(2).FindControl("ext3"), TextBox)
            ext4 = CType(pricesTbl.Rows(1).Cells(3).FindControl("ext4"), TextBox)
            ext5 = CType(pricesTbl.Rows(1).Cells(4).FindControl("ext5"), TextBox)
            ext6 = CType(pricesTbl.Rows(1).Cells(5).FindControl("ext6"), TextBox)
            ext7 = CType(pricesTbl.Rows(1).Cells(6).FindControl("ext7"), TextBox)
            bal1 = CType(pricesTbl.Rows(2).Cells(0).FindControl("bal1"), TextBox)
            bal2 = CType(pricesTbl.Rows(2).Cells(1).FindControl("bal2"), TextBox)
            bal3 = CType(pricesTbl.Rows(2).Cells(2).FindControl("bal3"), TextBox)
            bal4 = CType(pricesTbl.Rows(2).Cells(3).FindControl("bal4"), TextBox)
            bal5 = CType(pricesTbl.Rows(2).Cells(4).FindControl("bal5"), TextBox)
            bal6 = CType(pricesTbl.Rows(2).Cells(5).FindControl("bal6"), TextBox)
            bal7 = CType(pricesTbl.Rows(2).Cells(6).FindControl("bal7"), TextBox)
            su1 = CType(pricesTbl.Rows(3).Cells(0).FindControl("su1"), TextBox)
            su2 = CType(pricesTbl.Rows(3).Cells(1).FindControl("su2"), TextBox)
            su3 = CType(pricesTbl.Rows(3).Cells(2).FindControl("su3"), TextBox)
            su4 = CType(pricesTbl.Rows(3).Cells(3).FindControl("su4"), TextBox)
            su5 = CType(pricesTbl.Rows(3).Cells(4).FindControl("su5"), TextBox)
            su6 = CType(pricesTbl.Rows(3).Cells(5).FindControl("su6"), TextBox)
            su7 = CType(pricesTbl.Rows(3).Cells(6).FindControl("su7"), TextBox)

            Dim splitPrices() As String = detailsData.Item(rowIndex).Item(itemIndex).Item("Prices").Split("|")
            int1.Text = splitPrices(0)
            int2.Text = splitPrices(1)
            int3.Text = splitPrices(2)
            int4.Text = splitPrices(3)
            int5.Text = splitPrices(4)
            int6.Text = splitPrices(5)
            ext1.Text = splitPrices(6)
            ext2.Text = splitPrices(7)
            ext3.Text = splitPrices(8)
            ext4.Text = splitPrices(9)
            ext5.Text = splitPrices(10)
            ext6.Text = splitPrices(11)
            bal1.Text = splitPrices(12)
            bal2.Text = splitPrices(13)
            bal3.Text = splitPrices(14)
            bal4.Text = splitPrices(15)
            bal5.Text = splitPrices(16)
            bal6.Text = splitPrices(17)
            su1.Text = splitPrices(18)
            su2.Text = splitPrices(19)
            su3.Text = splitPrices(20)
            su4.Text = splitPrices(21)
            su5.Text = splitPrices(22)
            su6.Text = splitPrices(23)
            Dim splitDiscounts() As String = detailsData.Item(rowIndex).Item(itemIndex).Item("Discounts").Split("|")
            int7.Text = splitDiscounts(0)
            ext7.Text = splitDiscounts(1)
            bal7.Text = splitDiscounts(2)
            su7.Text = splitDiscounts(3)

            itemIndex += 1
        Next
    End Sub

    Protected Sub onRowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim num As Integer = e.Row.RowIndex
            Dim compSelector As DropDownList = CType(e.Row.FindControl("compSelector"), DropDownList)
            compSelector.Items.Clear()

            compSelector.DataSource = sqlAccess.retrieveAirCompanies()
            compSelector.DataTextField = "Company"
            compSelector.DataValueField = "Company"
            compSelector.DataBind()
            compSelector.Items.FindByText("Tap Air Portugal").Selected = True
        End If
    End Sub

    Private Function transformTime(time As String) As String
        Dim result As String = ""

        Dim hour As String = "", min As String = "", mode As String = ""
        If time.Length = 9 Then ' Hora < 10
            hour = time.Substring(0, 1).PadLeft(2, "0")
            min = time.Substring(4, 2)
            mode = time.Substring(7, 2)
        ElseIf time.Length = 10 Then ' Hora >= 10
            hour = time.Substring(0, 2)
            min = time.Substring(5, 2)
            mode = time.Substring(8, 2)
        End If

        If mode = "PM" Then
            Select Case hour
                Case "01"
                    hour = "13"
                Case "02"
                    hour = "14"
                Case "03"
                    hour = "15"
                Case "04"
                    hour = "16"
                Case "05"
                    hour = "17"
                Case "06"
                    hour = "18"
                Case "07"
                    hour = "19"
                Case "08"
                    hour = "20"
                Case "09"
                    hour = "21"
                Case "10"
                    hour = "22"
                Case "11"
                    hour = "23"
                Case "12"
                    hour = "00"
            End Select
        End If

        result = hour & min & "00"
        transformTime = result
    End Function

    ' Procedimiento que gestiona el comportamiento del botón de "Guardar precios" para el popup
    Protected Sub savePrices(sender As Object, e As EventArgs) Handles saveBtn.Click
        If Session("repDone") = "N" Then
            Dim datosObligatoriosRellenados As Boolean = False
            Dim pendingData As Boolean = False
            Dim rowSelected As Integer = CInt(Session("rowIndex"))

            Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
            Dim itemsMap As New Dictionary(Of Integer, Dictionary(Of String, String))
            Dim detailsMap As Dictionary(Of String, String)

            'Dim codeField, dateField, hourField, taxesField, returnField, returnTimeField, transferField, hotelField, offerField As TextBox
            Dim firstCode As String = "", firstDate As String = "", firstHour As String = "", firstTaxes As String = "", firstReturn As String = "", firstReturnFlightTime As String = "", firstTransfer As String = "", firstHotel As String = "", firstOffer As String = ""

            ' Para ir recogiendo los valores de cada vuelo en detalle, se realiza una iteración de cada sección del popup (código, fecha, hora, tasas, vuelo de regreso, transfer, hotel y precios asociados)

            Dim itemIndex As Integer = 0
            Dim htmlControl As HtmlGenericControl = CType(rep.Items(0).FindControl("flightDetailsDiv"), HtmlGenericControl)

            Dim flightDetails As New FlightDetailsDiv(htmlControl)

            datosObligatoriosRellenados = comprobarDatosObligatorios(flightDetails)

            detailsMap = New Dictionary(Of String, String)

            ' Antes de proseguir con la búsqueda de campos, verificamos si el usuario ha rellenado el código de vuelo, fecha, hora de salida, vuelo de regreso y transfer
            If (datosObligatoriosRellenados = False) Then
                pendingData = True
            Else
                ' Ha rellenado los seis campos básicos para el vuelo (o primero, si seleccionó más de uno en el selector).
                ' Por lo tanto, los guardamos en variables que luego emplearemos en el rellenado dinámico de ser el caso
                firstCode = Trim(flightDetails.flightCode)
                firstDate = Trim(flightDetails.flightDate)
                firstHour = Trim(flightDetails.flightHour)
                firstTaxes = Trim(flightDetails.flightTaxes)
                firstReturn = Trim(flightDetails.flightReturn)
                firstReturnFlightTime = Trim(flightDetails.flightReturnTime)
                firstTransfer = Trim(flightDetails.flightTransfer)
                firstHotel = Trim(flightDetails.flightHotel)
                firstOffer = Trim(flightDetails.flightOffer)

                Session("firstPrices") = rellenarSessionFirstPrices(flightDetails.flightDetailsPrices)
                
                Session("firstDiscounts") = rellenarSessionFirstDiscounts(flightDetails.flightDetailsPrices)

                detailsMap.Add("Code", firstCode)
                detailsMap.Add("Date", firstDate)
                detailsMap.Add("Hour", firstHour)
                detailsMap.Add("Taxes", firstTaxes)
                detailsMap.Add("Return", firstReturn)
                detailsMap.Add("ReturnFlightTime", firstReturnFlightTime)
                detailsMap.Add("Transfer", firstTransfer)
                detailsMap.Add("Hotel", firstHotel)
                detailsMap.Add("Offer", firstOffer)
                detailsMap.Add("Prices", Session("firstPrices"))
                detailsMap.Add("Discounts", Session("firstDiscounts"))
            End If

            itemsMap.Add(itemIndex, detailsMap)
            'Next

            If Not detailsData.ContainsKey(rowSelected) Then
                detailsData.Add(rowSelected, itemsMap)
            Else
                detailsData.Item(rowSelected) = itemsMap
            End If
            Session("detailsData") = detailsData

            If Not pendingData Then
                ' Toda la información inicial ha sido introducida, por lo que hay que mostrar el resto de tablas de precios rellenadas con los mismos datos (si se especificó más de un vuelo)
                Dim flightsNum As Integer = CInt(Session("flightsNum"))

                If flightsNum > 1 Then
                    ' Aprovechamos para dejar rellenados los tres campos básicos (código, fecha y hora) del primer vuelo, mediante los valores guardados con anterioridad
                    Dim basicDiv2 As HtmlGenericControl = CType(rep.Items(0).FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightBasicDiv"), HtmlGenericControl)
                    Dim firstCodeField As TextBox = CType(basicDiv2.FindControl("flightCode"), TextBox)
                    Dim firstDateField As TextBox = CType(basicDiv2.FindControl("flightDate"), TextBox)
                    Dim firstHourField As TextBox = CType(basicDiv2.FindControl("flightHour"), TextBox)
                    Dim firstTaxesField As TextBox = CType(basicDiv2.FindControl("flightTaxes"), TextBox)
                    Dim firstReturnField As TextBox = CType(basicDiv2.FindControl("returnFlight"), TextBox)
                    Dim firstReturnTimeField As TextBox = CType(basicDiv2.FindControl("returnFlightTime"), TextBox)
                    Dim extraDiv2 As HtmlGenericControl = CType(rep.Items(0).FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightExtraDiv"), HtmlGenericControl)
                    Dim firstTransferField As TextBox = CType(extraDiv2.FindControl("flightTransfer"), TextBox)
                    Dim firstHotelField As TextBox = CType(extraDiv2.FindControl("flightTransfer"), TextBox)
                    Dim firstOfferField As TextBox = CType(extraDiv2.FindControl("offerField"), TextBox)

                    firstCodeField.Text = firstCode
                    firstDateField.Text = firstDate
                    firstHourField.Text = firstHour
                    firstTaxesField.Text = firstTaxes
                    firstReturnField.Text = firstReturn
                    firstReturnTimeField.Text = firstReturnFlightTime
                    firstTransferField.Text = firstTransfer
                    firstHotelField.Text = firstHotel
                    firstOfferField.Text = firstOffer

                    Dim firstPrices As String = Session("firstPrices")
                    Dim splitPrices() As String = firstPrices.Split("|")
                    Dim firstDiscounts As String = Session("firstDiscounts")
                    Dim splitDiscounts() As String = firstDiscounts.Split("|")
                    Dim priceIndex, discountIndex As Integer
                    ' A partir de este punto, todos los campos que están dentro del Repeater estarán vacíos (incluyendo la que se rellenó previamente), por lo que tenemos que replicarla a lo largo del popup
                    For Each repItem As RepeaterItem In rep.Items

                        htmlControl = CType(repItem.FindControl("flightDetailsDiv"), HtmlGenericControl)

                        flightDetails = New FlightDetailsDiv(htmlControl)

                        Dim pricesTbl As Table = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightPricesDiv").FindControl("pricesTbl"), Table)

                        priceIndex = 0
                        Dim ctrl As TextBox
                        Dim controlId As String = ""
                        For rowIndex As Byte = 1 To 4
                            For colIndex As Byte = 1 To 6
                                Select Case rowIndex
                                    Case 1
                                        controlId = "int" & colIndex
                                    Case 2
                                        controlId = "ext" & colIndex
                                    Case 3
                                        controlId = "bal" & colIndex
                                    Case 4
                                        controlId = "su" & colIndex
                                End Select

                                ctrl = CType(pricesTbl.Rows(rowIndex).Cells(colIndex).FindControl(controlId), TextBox)
                                ctrl.Text = splitPrices(priceIndex)

                                priceIndex += 1
                            Next
                        Next

                        discountIndex = 0
                        For rowIndex As Byte = 1 To 4
                            Select Case rowIndex
                                Case 1
                                    controlId = "int7"
                                Case 2
                                    controlId = "ext7"
                                Case 3
                                    controlId = "bal7"
                                Case 4
                                    controlId = "su7"
                            End Select

                            ctrl = CType(pricesTbl.Rows(rowIndex).Cells(7).FindControl(controlId), TextBox)
                            ctrl.Text = splitDiscounts(discountIndex)

                            discountIndex += 1
                        Next
                    Next

                    For itemIndex2 As Integer = 1 To flightsNum - 1
                        Dim flightDiv As HtmlGenericControl = CType(rep.Items(itemIndex2).FindControl("flightDetailsDiv"), HtmlGenericControl)
                        flightDiv.Style.Remove("display")
                    Next
                    scriptManager.RegisterStartupScript(Page, Me.GetType, "setFields", "setFormats();", True)

                    Session("repDone") = "Y"
                Else
                    Session("pendingDetails") = "N"

                    Dim pendingDetails As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
                    Dim rowIndex As Integer = CInt(Session("rowIndex"))
                    If Not pendingDetails.ContainsKey(rowIndex) Then
                        pendingDetails.Add(rowIndex, "N")
                    Else
                        pendingDetails.Item(rowIndex) = "N"
                    End If

                    prices.Visible = False
                    'popup.Visible = True
                    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Os detalhes do voo foram salvas com sucesso.');", True)
                End If
            Else
                Session("pendingDetails") = "Y"

                prices.Visible = False
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Há ainda estão pendentes de preencher os dados. Por favor, insira as informações.');", True)
            End If
        ElseIf Session("repDone") = "Y" Then ' En este momento ya está rellenado el Repeater con los precios por defecto, así que vamos verificando y guardando los datos de los campos al mismo tiempo
            Session("repDone") = "N"
            Dim pendingData As Boolean = False
            Dim rowSelected As Integer = CInt(Session("rowIndex"))

            Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
            Dim itemsMap As New Dictionary(Of Integer, Dictionary(Of String, String))
            Dim detailsMap As Dictionary(Of String, String)

            For Each repItem As RepeaterItem In rep.Items
                Dim itemIndex As Integer = repItem.ItemIndex
                Dim basicDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightBasicDiv"), HtmlGenericControl)
                Dim codeField As TextBox = CType(basicDiv.FindControl("flightCode"), TextBox)
                Dim dateField As TextBox = CType(basicDiv.FindControl("flightDate"), TextBox)
                Dim hourField As TextBox = CType(basicDiv.FindControl("flightHour"), TextBox)
                Dim taxesField As TextBox = CType(basicDiv.FindControl("flightTaxes"), TextBox)
                Dim returnField As TextBox = CType(basicDiv.FindControl("returnFlight"), TextBox)
                Dim returnFlightTimeField As TextBox = CType(basicDiv.FindControl("returnFlightTime"), TextBox)

                Dim extraDiv As HtmlGenericControl = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightInfoDiv").FindControl("flightExtraDiv"), HtmlGenericControl)
                Dim transferField As TextBox = CType(extraDiv.FindControl("flightTransfer"), TextBox)
                Dim hotelField As TextBox = CType(extraDiv.FindControl("flightHotel"), TextBox)
                Dim offerField As TextBox = CType(extraDiv.FindControl("offerField"), TextBox)

                Dim code As String = Trim(codeField.Text)
                Dim flightDate As String = Trim(dateField.Text)
                Dim hour As String = Trim(hourField.Text)
                Dim taxes As String = Trim(taxesField.Text)
                Dim retFlight As String = Trim(returnField.Text)
                Dim retFlightFlightTime As String = Trim(returnFlightTimeField.Text)
                Dim transfer As String = Trim(transferField.Text)
                Dim hotel As String = Trim(hotelField.Text)
                Dim offer As String = Trim(offerField.Text)

                detailsMap = New Dictionary(Of String, String)

                If (code <> "") And (flightDate <> "") And (hour <> "") And (taxes <> "") And (retFlight <> "") And (transfer <> "") And (hotel <> "") And (offer <> "") Then
                    Dim pricesTbl As Table = CType(repItem.FindControl("flightDetailsDiv").FindControl("flightPricesDiv").FindControl("pricesTbl"), Table)

                    Session("pricesAfterRep") = ""
                    Dim ctrl As TextBox
                    Dim controlId As String = ""
                    Dim price As String
                    For rowIndex As Byte = 1 To 4
                        For colIndex As Byte = 1 To 6
                            Select Case rowIndex
                                Case 1
                                    controlId = "int" & colIndex
                                Case 2
                                    controlId = "ext" & colIndex
                                Case 3
                                    controlId = "bal" & colIndex
                                Case 4
                                    controlId = "su" & colIndex
                            End Select

                            ctrl = CType(pricesTbl.Rows(rowIndex).Cells(colIndex).FindControl(controlId), TextBox)
                            price = Trim(ctrl.Text)
                            If price.Contains(",") Then
                                price = Replace(price, ",", ".")
                            End If
                            Session("pricesAfterRep") &= price & "|"
                        Next
                    Next

                    Session("discountsAfterRep") = ""
                    Dim discount As String
                    For rowIndex As Byte = 1 To 4
                        Select Case rowIndex
                            Case 1
                                controlId = "int7"
                            Case 2
                                controlId = "ext7"
                            Case 3
                                controlId = "bal7"
                            Case 4
                                controlId = "su7"
                        End Select

                        ctrl = CType(pricesTbl.Rows(rowIndex).Cells(7).FindControl(controlId), TextBox)
                        discount = Trim(ctrl.Text)
                        If discount.Contains(",") Then
                            discount = Replace(discount, ",", ".")
                        End If
                        Session("discountsAfterRep") &= discount & "|"
                    Next

                    detailsMap.Add("Code", code)
                    detailsMap.Add("Date", flightDate)
                    detailsMap.Add("Hour", hour)
                    detailsMap.Add("Taxes", taxes)
                    detailsMap.Add("Return", retFlight)
                    detailsMap.Add("ReturnFlightTime", retFlightFlightTime)
                    detailsMap.Add("Transfer", transfer)
                    detailsMap.Add("Hotel", hotel)
                    detailsMap.Add("Offer", offer)
                    detailsMap.Add("Prices", Session("pricesAfterRep"))
                    detailsMap.Add("Discounts", Session("discountsAfterRep"))
                Else
                    pendingData = True
                    GoTo check_pendingData2
                End If

                itemsMap.Add(itemIndex, detailsMap)
            Next

            If Not detailsData.ContainsKey(rowSelected) Then
                detailsData.Add(rowSelected, itemsMap)
            Else
                detailsData.Item(rowSelected) = itemsMap
            End If
            Session("detailsData") = detailsData

check_pendingData2:
            If Not pendingData Then
                Session("pendingDetails") = "N"

                Dim pendingDetails As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
                Dim rowIndex As Integer = CInt(Session("rowIndex"))
                If Not pendingDetails.ContainsKey(rowIndex) Then
                    pendingDetails.Add(rowIndex, "N")
                Else
                    pendingDetails.Item(rowIndex) = "N"
                End If

                prices.Visible = False
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Os detalhes do voo foram salvas com sucesso.');", True)
            Else
                Session("pendingDetails") = "Y"

                prices.Visible = False
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Há ainda estão pendentes de preencher os dados. Por favor, insira as informações.');", True)
            End If
        End If
    End Sub

    Protected Sub exitPrices(sender As Object, e As EventArgs) Handles exitBtn.Click
        prices.Visible = False

        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showConfirm", "showConfirm();", True)
    End Sub

    Protected Sub recoverPopup(sender As Object, e As EventArgs) Handles noBtn.Click
        'confirm.Visible = False
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirm').modal('hide');", True)
        prices.Visible = True

        scriptManager.RegisterStartupScript(Page, Me.GetType, "setFields", "setFormats();", True)
    End Sub

    Protected Sub closeConfirm(sender As Object, e As EventArgs) Handles yesBtn.Click
        If Session("repDone") = "Y" Then
            Session("repDone") = "N"
        End If
        If Session("pendingDetails") = "Y" Then
            Dim rowIndex As Integer = CInt(Session("rowIndex"))
            Dim pendingDetails As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
            If Not pendingDetails.ContainsKey(rowIndex) Then
                pendingDetails.Add(rowIndex, "Y")
            Else
                pendingDetails.Item(rowIndex) = "Y"
            End If
            Session("pendingData") = pendingDetails
        End If

        'confirm.Visible = False
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirm').modal('hide');", True)
    End Sub

    Protected Sub saveImages(sender As Object, e As EventArgs) Handles saveImagesBtn.Click
        If uploadCtrl1.HasFile Then
            Dim fileName As String = uploadCtrl1.FileName

            If Session("img1") = "" Then
                uploadCtrl1.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img1")
                My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                uploadCtrl1.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If

            Session("img1") = fileName
        End If
        If uploadCtrl2.HasFile Then
            Dim fileName As String = uploadCtrl2.FileName

            If Session("img2") = "" Then
                uploadCtrl2.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img2")
                My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                uploadCtrl2.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If
            
            Session("img2") = fileName
        End If
        If uploadCtrl3.HasFile Then
            Dim fileName As String = uploadCtrl3.FileName

            If Session("img3") = "" Then
                uploadCtrl3.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img3")
                My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                uploadCtrl3.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If
            
            Session("img3") = fileName
        End If
        If uploadCtrl4.HasFile Then
            Dim fileName As String = uploadCtrl4.FileName

            If Session("img4") = "" Then
                uploadCtrl4.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img4")
                My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                uploadCtrl4.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If
            
            Session("img4") = fileName
        End If
        If uploadCtrl5.HasFile Then
            Dim fileName As String = uploadCtrl5.FileName

            If Session("img5") = "" Then
                uploadCtrl5.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img5")
                My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                uploadCtrl5.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If
            
            Session("img5") = fileName
        End If

        imagesLbl.Text = Session("img1") & " " & Session("img2") & " " & Session("img3") & " " & Session("img4") & " " & Session("img5")

        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideImages", "$('#imagesPopup').modal('hide');", True)
    End Sub

    Protected Sub onItemCreated(sender As Object, e As RepeaterItemEventArgs) Handles rep.ItemCreated
        scriptManager.RegisterStartupScript(Page, Me.GetType, "setFields", "setFormats();", True)
    End Sub

    Protected Sub companyChanged(sender As Object, e As EventArgs)
        Dim compSelector As DropDownList = CType(sender, DropDownList)
        Dim gridRow As GridViewRow = CType(compSelector.NamingContainer, GridViewRow)
        Dim rowIndex As Integer = gridRow.RowIndex

        Dim compName As String = compSelector.SelectedItem.Text
        Dim fileName As String = sqlAccess.getCompanyLogo(compName)

        Dim logoImg As HtmlImage = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
        logoImg.Src = "Images/Logos/" & fileName
    End Sub

    ' Procedimiento que se encarga de recoger todos los datos manejados hasta el momento y enviarlos al procedimiento SQL de inserción de una manera estructurada
    Protected Sub registerPackageOr(sender As Object, e As EventArgs) 'Handles registerBtn.Click
        loadPanel.Visible = True

        Dim errorStr As String = ""
        Dim currentData As New DataTable
        Dim newRow As DataRow

        Dim compSelector, citySelector, statusSelector As DropDownList
        Dim logoImg As HtmlImage
        Dim descField, numField, quotaField, destinoField As TextBox
        Dim logo, cityValue, description, num, quota, status As String

        currentData.Columns.Add(New DataColumn("Companhia"))
        currentData.Columns.Add(New DataColumn("Logo"))
        currentData.Columns.Add(New DataColumn("Cidade"))
        currentData.Columns.Add(New DataColumn("Cidade DEST"))
        currentData.Columns.Add(New DataColumn("Descrição"))
        currentData.Columns.Add(New DataColumn("Número de voos"))
        currentData.Columns.Add(New DataColumn("Quota"))
        currentData.Columns.Add(New DataColumn("Estado"))

        Dim flightsData As New Dictionary(Of Integer, Dictionary(Of String, String))
        Dim flightData As Dictionary(Of String, String)
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))

        Dim splitLogo() As String

        For rowIndex As Integer = 0 To gridView.Rows.Count - 1
            newRow = currentData.NewRow()
            flightData = New Dictionary(Of String, String)

            compSelector = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
            logoImg = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
            citySelector = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
            descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
            destinoField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("destinoField"), TextBox)
            numField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
            quotaField = CType(gridView.Rows(rowIndex).Cells(5).FindControl("quotaField"), TextBox)
            statusSelector = CType(gridView.Rows(rowIndex).Cells(6).FindControl("statusSelector"), DropDownList)
            splitLogo = logoImg.Src.Split("/")
            logo = splitLogo(splitLogo.Length - 1)
            cityValue = citySelector.Items(citySelector.SelectedIndex).Value
            description = Trim(descField.Text)
            num = Trim(numField.Text)
            quota = Trim(quotaField.Text)
            status = statusSelector.Items(statusSelector.SelectedIndex).Text

            If (cityValue = "-1") Or (description = "") Or (num = "") Or (quota = "") Then
                errorStr = "Eles foram detectadas preenchendo as mudanças " & (rowIndex + 1) & " pendentes linha da tabela de voos. Por favor, inclua esta informação."
                Exit For
            End If

            newRow("Companhia") = compSelector.Items(compSelector.SelectedIndex).Text
            newRow("Logo") = logo
            newRow("Cidade") = citySelector.Items(citySelector.SelectedIndex).Text
            newRow("Cidade DEST") = destinoField.Text
            newRow("Descrição") = description
            newRow("Número de voos") = num
            newRow("Quota") = quota
            newRow("Estado") = status
            currentData.Rows.Add(newRow)

            flightData.Add("Company", compSelector.Items(compSelector.SelectedIndex).Text)
            flightData.Add("Logo", logo)
            flightData.Add("City", citySelector.Items(citySelector.SelectedIndex).Text)
            flightData.Add("CityDest", destinoField.Text)
            flightData.Add("Description", description)
            flightData.Add("Number", num)
            flightData.Add("Quota", quota)
            flightData.Add("Status", status)
            flightsData.Add(rowIndex, flightData)

            Session("detailsData") = detailsData
        Next

        If errorStr = "" Then
            Session("dataBackup") = currentData
            Session("flightsData") = flightsData

            Dim flightsNum As Integer = gridView.Rows.Count
            If flightsNum <> 0 Then
                Dim pendingDetails As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))

                For Each item In pendingDetails
                    If item.Value = "Y" Then
                        errorStr = "Eles foram detectados por preencher os detalhes da linha " & (item.Key + 1) & " alterações pendentes. Por favor, reveja esta informação."
                        Exit For
                    End If
                Next

                If errorStr = "" Then
                    Dim packageData(2) As Object
                    Dim basicData As New Dictionary(Of String, String)

                    Dim packName, shipCode, sailDate, itinName, itinCode, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes, diasVuelo As String

                    packName = Trim(nameField.Text)
                    shipCode = shipSelector.Items(shipSelector.SelectedIndex).Value
                    sailDate = sailField.Text
                    itinName = Trim(itinNameField.Text)
                    itinCode = Session("itinCode")
                    img1 = Session("img1")
                    img2 = Session("img2")
                    img3 = Session("img3")
                    img4 = Session("img4")
                    img5 = Session("img5")
                    dining = Trim(diningField.Text)
                    includedTxt = Trim(includedField.Text)
                    notIncludedTxt = Trim(notIncludedField.Text)
                    notes = Trim(notesField.Text)
                    diasVuelo = rblDiasVuelo.SelectedValue

                    If packName = "" Then
                        errorStr = "Você deve digitar o nome do pacote. Por favor, inclua esta informação."
                    ElseIf itinName = "" Then
                        errorStr = "É necessário introduzir o nome para o itinerário. Por favor, inclua esta informação."
                    ElseIf dining = "" Then
                        errorStr = "Você entra a refeição turno. Por favor, inclua esta informação."
                    ElseIf includedTxt = "" Then
                        errorStr = "Você deve especificar que é o que está incluído neste pacote. Por favor, adicione esta informação."
                    ElseIf (img1 = "" Or img2 = "") Then
                        errorStr = "Você deve digitar as duas primeiras imagens. Por favor, adicione esta informação."
                    ElseIf notIncludedTxt = "" Then
                        errorStr = "É necessário especificar que não está incluído neste pacote. Por favor, adicione esta informação."
                    End If

                    If errorStr = "" Then
                        basicData.Add("PackageName", packName)
                        basicData.Add("ShipCode", shipCode)
                        basicData.Add("SailingDate", sailDate)
                        basicData.Add("ItineraryName", itinName)
                        basicData.Add("ItineraryCode", itinCode)
                        basicData.Add("Img1", img1)
                        basicData.Add("Img2", img2)
                        basicData.Add("Img3", img3)
                        basicData.Add("Img4", img4)
                        basicData.Add("Img5", img5)
                        basicData.Add("Dining", dining)
                        basicData.Add("Included", includedTxt)
                        basicData.Add("NotIncluded", notIncludedTxt)
                        basicData.Add("Notes", notes)
                        basicData.Add("DiasVuelo", diasVuelo)

                        Dim currentFlights As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                        Dim currentDetails As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))

                        packageData.SetValue(basicData, 0)
                        packageData.SetValue(currentFlights, 1)
                        packageData.SetValue(currentDetails, 2)

                        Dim someError As String = sqlAccess.insertPackage(packageData)
                        If someError = "" Then
                            Dim packIds As String = sqlAccess.getPackageIds(packName)

                            For Each packId In packIds.Split("|")
                                If packId <> "" Then
                                    IO.Directory.CreateDirectory(Server.MapPath("Images/Uploads/" & packId))
                                    Dim images As String() = IO.Directory.GetFiles(Server.MapPath("Images/Temp"))

                                    For Each image In images
                                        Dim splitImage() As String = image.Split("\")
                                        Dim fileName As String = splitImage(splitImage.Length - 1)
                                        IO.File.Copy(image, Server.MapPath("Images/Uploads/" & packId & "/" & fileName))
                                    Next
                                End If
                            Next
                            IO.Directory.Delete(Server.MapPath("Images/Temp"), True)

                            Session("packName") = packName
                            Session("packageRegistered") = "Y"
                        Else
                            errorStr = someError
                        End If
                    End If
                End If
            Else
                errorStr = "Ainda não adicionou informações sobre qualquer voo. Por favor, insira pelo menos um para o pacote."
            End If
        End If

        If errorStr = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'O pacote foi descarregada com sucesso. Em seguida, você pode incorporar linhas associadas com os voos para a rota.');", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & errorStr & "');", True)
        End If
        loadPanel.Visible = False
    End Sub


    Protected Sub registerPackage(sender As Object, e As EventArgs) Handles registerBtn.Click
        loadPanel.Visible = True

        Dim errorStr As String = ""

        If errorStr = "" Then
            Dim packageData(0) As Object
            Dim basicData As New Dictionary(Of String, String)

            Dim packName, shipCode, shipDescription, sailDate, itinName, itinCode, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes, flight, transfer, hotel As String

            packName = Trim(nameField.Text)
            shipCode = shipSelector.Items(shipSelector.SelectedIndex).Value
            sailDate = sailField.Text
            itinName = Trim(itinNameField.Text)
            itinCode = Session("itinCode")
            img1 = Session("img1")
            img2 = Session("img2")
            img3 = Session("img3")
            img4 = Session("img4")
            img5 = Session("img5")
            dining = Trim(diningField.Text)
            includedTxt = Trim(includedField.Text)
            notIncludedTxt = Trim(notIncludedField.Text)
            notes = Trim(notesField.Text)

            shipDescription = Trim(descripNavio.Text)
            flight = flightCheckBox.Checked
            transfer = transferCheckBox.Checked
            hotel = hotelCheckBox.Checked

            Session("CheckedFlight") = Boolean.Parse(flight)
            Session("CheckedTransfer") = Boolean.Parse(transfer)
            Session("CheckedHotel") = Boolean.Parse(hotel)


            If packName = "" Then
                errorStr = "Você deve digitar o nome do pacote. Por favor, inclua esta informação."
            ElseIf itinName = "" Then
                errorStr = "É necessário introduzir o nome para o itinerário. Por favor, inclua esta informação."
            ElseIf dining = "" Then
                errorStr = "Você entra a refeição turno. Por favor, inclua esta informação."
            ElseIf includedTxt = "" Then
                errorStr = "Você deve especificar que é o que está incluído neste pacote. Por favor, adicione esta informação."
            ElseIf (img1 = "" Or img2 = "") Then
                errorStr = "Você deve digitar as duas primeiras imagens. Por favor, adicione esta informação."
            ElseIf notIncludedTxt = "" Then
                errorStr = "É necessário especificar que não está incluído neste pacote. Por favor, adicione esta informação."
            End If

            If errorStr = "" Then
                basicData.Add("PackageName", packName)
                basicData.Add("ShipCode", shipCode)
                basicData.Add("ShipDesc", shipDescription)
                basicData.Add("SailingDate", sailDate)
                basicData.Add("ItineraryName", itinName)
                basicData.Add("ItineraryCode", itinCode)
                basicData.Add("Img1", img1)
                basicData.Add("Img2", img2)
                basicData.Add("Img3", img3)
                basicData.Add("Img4", img4)
                basicData.Add("Img5", img5)
                basicData.Add("Dining", dining)
                basicData.Add("Included", includedTxt)
                basicData.Add("NotIncluded", notIncludedTxt)
                basicData.Add("Notes", notes)
                basicData.Add("Flight", flight)
                basicData.Add("Transfer", transfer)
                basicData.Add("Hotel", hotel)

                packageData.SetValue(basicData, 0)

                Dim someError As String = sqlAccess.insertPackage(packageData)
                If someError = "" Then
                    Dim packIds As String = sqlAccess.getPackageIds(packName)

                    For Each packId In packIds.Split("|")
                        If packId <> "" Then
                            IO.Directory.CreateDirectory(Server.MapPath("Images/Uploads/" & packId))
                            Dim images As String() = IO.Directory.GetFiles(Server.MapPath("Images/Temp"))

                            For Each image In images
                                Dim splitImage() As String = image.Split("\")
                                Dim fileName As String = splitImage(splitImage.Length - 1)
                                IO.File.Copy(image, Server.MapPath("Images/Uploads/" & packId & "/" & fileName))
                            Next
                        End If
                    Next
                    IO.Directory.Delete(Server.MapPath("Images/Temp"), True)

                    Session("packName") = packName
                    Session("packageRegistered") = "Y"
                Else
                    errorStr = someError
                End If
            End If
        End If

        If errorStr = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'O pacote foi descarregada com sucesso. Em seguida, você pode incorporar linhas associadas com os voos para a rota.');", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & errorStr & "');", True)
        End If
        loadPanel.Visible = False

        Session("IdPack") = GolbalVariable.Id
        Response.Redirect("Cabin.aspx")
    End Sub

    Sub Check_Hotel(sender As Object, e As EventArgs)
        Dim isHotel = ""
        If HotelCheckBox.Checked Then
            isHotel = "www"
        Else
            isHotel = "org"
        End If

    End Sub

    Sub Check_Transfer(sender As Object, e As EventArgs)

    End Sub

    Sub Check_Flight(sender As Object, e As EventArgs)

    End Sub

    Protected Sub registerPackageTest(sender As Object, e As EventArgs) 'Handles registerBtn.Click
        loadPanel.Visible = True

        Dim errorStr As String = ""
        Dim currentData As New DataTable
        Dim newRow As DataRow

        Dim compSelector, citySelector, statusSelector As DropDownList
        Dim logoImg As HtmlImage
        Dim descField, numField, quotaField, destinoField As TextBox
        Dim logo, cityValue, description, num, quota, status As String

        currentData.Columns.Add(New DataColumn("Companhia"))
        currentData.Columns.Add(New DataColumn("Logo"))
        currentData.Columns.Add(New DataColumn("Cidade"))
        currentData.Columns.Add(New DataColumn("Cidade DEST"))
        currentData.Columns.Add(New DataColumn("Descrição"))
        currentData.Columns.Add(New DataColumn("Número de voos"))
        currentData.Columns.Add(New DataColumn("Quota"))
        currentData.Columns.Add(New DataColumn("Estado"))

        Dim flightsData As New Dictionary(Of Integer, Dictionary(Of String, String))
        Dim flightData As Dictionary(Of String, String)
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))

        Dim splitLogo() As String

        For rowIndex As Integer = 0 To gridView.Rows.Count - 1
            newRow = currentData.NewRow()
            flightData = New Dictionary(Of String, String)

            compSelector = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
            logoImg = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
            citySelector = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
            descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
            destinoField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("destinoField"), TextBox)
            numField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("numField"), TextBox)
            quotaField = CType(gridView.Rows(rowIndex).Cells(5).FindControl("quotaField"), TextBox)
            statusSelector = CType(gridView.Rows(rowIndex).Cells(6).FindControl("statusSelector"), DropDownList)
            splitLogo = logoImg.Src.Split("/")
            logo = splitLogo(splitLogo.Length - 1)
            cityValue = citySelector.Items(citySelector.SelectedIndex).Value
            description = Trim(descField.Text)
            num = Trim(numField.Text)
            quota = Trim(quotaField.Text)
            status = statusSelector.Items(statusSelector.SelectedIndex).Text

            'If (cityValue = "-1") Or (description = "") Or (num = "") Or (quota = "") Then
            'errorStr = "Eles foram detectadas preenchendo as mudanças " & (rowIndex + 1) & " pendentes linha da tabela de voos. Por favor, inclua esta informação."
            'Exit For
            'End If

            newRow("Companhia") = compSelector.Items(compSelector.SelectedIndex).Text
            newRow("Logo") = logo
            newRow("Cidade") = citySelector.Items(citySelector.SelectedIndex).Text
            newRow("Cidade DEST") = destinoField.Text
            newRow("Descrição") = description
            newRow("Número de voos") = num
            newRow("Quota") = quota
            newRow("Estado") = status
            currentData.Rows.Add(newRow)

            flightData.Add("Company", compSelector.Items(compSelector.SelectedIndex).Text)
            flightData.Add("Logo", logo)
            flightData.Add("City", citySelector.Items(citySelector.SelectedIndex).Text)
            flightData.Add("CityDest", destinoField.Text)
            flightData.Add("Description", description)
            flightData.Add("Number", num)
            flightData.Add("Quota", quota)
            flightData.Add("Status", status)
            flightsData.Add(rowIndex, flightData)

            Session("detailsData") = detailsData
        Next

        If errorStr = "" Then
            Session("dataBackup") = currentData
            Session("flightsData") = flightsData

            Dim flightsNum As Integer = gridView.Rows.Count
            'If flightsNum <> 0 Then
            Dim pendingDetails As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))

            'For Each item In pendingDetails
            'If item.Value = "Y" Then
            'errorStr = "Eles foram detectados por preencher os detalhes da linha " & (item.Key + 1) & " alterações pendentes. Por favor, reveja esta informação."
            'Exit For
            'End If
            'Next

            If errorStr = "" Then
                Dim packageData(2) As Object
                Dim basicData As New Dictionary(Of String, String)

                Dim packName, shipCode, sailDate, itinName, itinCode, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes, diasVuelo As String

                packName = Trim(nameField.Text)
                shipCode = shipSelector.Items(shipSelector.SelectedIndex).Value
                sailDate = sailField.Text
                itinName = Trim(itinNameField.Text)
                itinCode = Session("itinCode")
                img1 = Session("img1")
                img2 = Session("img2")
                img3 = Session("img3")
                img4 = Session("img4")
                img5 = Session("img5")
                dining = Trim(diningField.Text)
                includedTxt = Trim(includedField.Text)
                notIncludedTxt = Trim(notIncludedField.Text)
                notes = Trim(notesField.Text)
                diasVuelo = rblDiasVuelo.SelectedValue

                If packName = "" Then
                    errorStr = "Você deve digitar o nome do pacote. Por favor, inclua esta informação."
                ElseIf itinName = "" Then
                    errorStr = "É necessário introduzir o nome para o itinerário. Por favor, inclua esta informação."
                ElseIf dining = "" Then
                    errorStr = "Você entra a refeição turno. Por favor, inclua esta informação."
                ElseIf includedTxt = "" Then
                    errorStr = "Você deve especificar que é o que está incluído neste pacote. Por favor, adicione esta informação."
                ElseIf (img1 = "" Or img2 = "") Then
                    errorStr = "Você deve digitar as duas primeiras imagens. Por favor, adicione esta informação."
                ElseIf notIncludedTxt = "" Then
                    errorStr = "É necessário especificar que não está incluído neste pacote. Por favor, adicione esta informação."
                End If

                If errorStr = "" Then
                    basicData.Add("PackageName", packName)
                    basicData.Add("ShipCode", shipCode)
                    basicData.Add("SailingDate", sailDate)
                    basicData.Add("ItineraryName", itinName)
                    basicData.Add("ItineraryCode", itinCode)
                    basicData.Add("Img1", img1)
                    basicData.Add("Img2", img2)
                    basicData.Add("Img3", img3)
                    basicData.Add("Img4", img4)
                    basicData.Add("Img5", img5)
                    basicData.Add("Dining", dining)
                    basicData.Add("Included", includedTxt)
                    basicData.Add("NotIncluded", notIncludedTxt)
                    basicData.Add("Notes", notes)
                    basicData.Add("DiasVuelo", diasVuelo)

                    Dim currentFlights As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                    Dim currentDetails As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))

                    packageData.SetValue(basicData, 0)
                    packageData.SetValue(currentFlights, 1)
                    packageData.SetValue(currentDetails, 2)

                    Dim someError As String = sqlAccess.insertPackage(packageData)
                    If someError = "" Then
                        Dim packIds As String = sqlAccess.getPackageIds(packName)

                        For Each packId In packIds.Split("|")
                            If packId <> "" Then
                                IO.Directory.CreateDirectory(Server.MapPath("Images/Uploads/" & packId))
                                Dim images As String() = IO.Directory.GetFiles(Server.MapPath("Images/Temp"))

                                For Each image In images
                                    Dim splitImage() As String = image.Split("\")
                                    Dim fileName As String = splitImage(splitImage.Length - 1)
                                    IO.File.Copy(image, Server.MapPath("Images/Uploads/" & packId & "/" & fileName))
                                Next
                            End If
                        Next
                        IO.Directory.Delete(Server.MapPath("Images/Temp"), True)

                        Session("packName") = packName
                        Session("packageRegistered") = "Y"
                    Else
                        errorStr = someError
                    End If
                End If
            End If
        Else
            errorStr = "Ainda não adicionou informações sobre qualquer voo. Por favor, insira pelo menos um para o pacote."
        End If
        'End If

        If errorStr = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'O pacote foi descarregada com sucesso. Em seguida, você pode incorporar linhas associadas com os voos para a rota.');", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & errorStr & "');", True)
        End If
        loadPanel.Visible = False
    End Sub

    Private Function comprobarDatosObligatorios(ByVal flightDetails As FlightDetailsDiv) As Boolean
        Dim resultado As Boolean = True

        If (flightDetails.flightCode = "") Then
            resultado = False
        ElseIf (flightDetails.flightDate = "") Then
            resultado = False
        ElseIf (flightDetails.flightHour = "") Then
            resultado = False
        ElseIf (flightDetails.flightTaxes = "") Then
            resultado = False
        ElseIf (flightDetails.flightReturn = "") Then
            resultado = False
        ElseIf (flightDetails.flightReturnTime = "") Then
            resultado = False
        ElseIf (flightDetails.flightTransfer = "") Then
            resultado = False
        End If

        Return resultado
    End Function

    Private Function rellenarSessionFirstPrices(ByVal flightDetailsPrices As FlightDetailsPrices) As String
        Dim firstPrices As String = ""

        For Each price In flightDetailsPrices.priceInterior
            firstPrices &= price & "|"
        Next
        For Each price In flightDetailsPrices.priceExterior
            firstPrices &= price & "|"
        Next
        For Each price In flightDetailsPrices.priceBalcony
            firstPrices &= price & "|"
        Next
        For Each price In flightDetailsPrices.priceSuite
            firstPrices &= price & "|"
        Next

        Return firstPrices
    End Function

    Private Function rellenarSessionFirstDiscounts(flightDetailsPrices As FlightDetailsPrices) As String
        Dim firstDiscounts As String = ""

        firstDiscounts &= flightDetailsPrices.offerInterior & "|"
        firstDiscounts &= flightDetailsPrices.offerExterior & "|"
        firstDiscounts &= flightDetailsPrices.offerBalcony & "|"
        firstDiscounts &= flightDetailsPrices.offerSuite & "|"

        Return firstDiscounts
    End Function

End Class