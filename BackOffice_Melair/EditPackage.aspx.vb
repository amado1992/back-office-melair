Public Class EditPackage
    Inherits System.Web.UI.Page

    Dim sqlAccess As New SQL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Session("login") <> "Y" Then
        '    Response.Redirect("Login.aspx")
        'End If

        Page.MaintainScrollPositionOnPostBack = True

        ' GUILLERMO - Los datos de las líneas de itinerario se obtienen a partir de 
        If Not IsPostBack Then
            Dim packageData() As Object = sqlAccess.retrievePackage(Session("selPackName"), Session("selSailDate"))
            Session("basicData") = packageData(0)
            Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(packageData(1), Dictionary(Of Integer, Dictionary(Of String, String)))
            Session("flightsData") = flightsData
            Session("detailsData") = packageData(2)
            Session("linesData") = packageData(3)
            'Dim linesData As New Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String)))
            'Session("linesData") = linesData

            Dim pendingData As New Dictionary(Of Integer, Char)
            For itemIndex As Integer = 0 To flightsData.Count - 1
                pendingData.Add(itemIndex, "N")
            Next
            Session("pendingData") = pendingData

            Session("img1") = ""
            Session("img2") = ""
            Session("img3") = ""
            Session("img4") = ""
            Session("img5") = ""
            Session("detailPending") = Nothing
            Session("linePending") = Nothing
            Session("fromDetails") = Nothing
            Session("fromLines") = Nothing

            titleLbl.Text = Session("selPackName") & "  -  " & Session("selSailDate")
            initGrid()
            completeData()

            If Not IO.Directory.Exists(Server.MapPath("Images/Temp")) Then
                IO.Directory.CreateDirectory(Server.MapPath("Images/Temp"))
            Else
                Dim images() As String = IO.Directory.GetFiles(Server.MapPath("Images/Temp"))
                For Each file In images
                    IO.File.Delete(file)
                Next
            End If
            recreateImages()
        End If
    End Sub

    Private Sub initGrid(Optional firstTime As Boolean = True)
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Id"))
        dt.Columns.Add(New DataColumn("Companhia"))
        dt.Columns.Add(New DataColumn("Logo"))
        dt.Columns.Add(New DataColumn("Cidade"))
        dt.Columns.Add(New DataColumn("CityDest"))
        dt.Columns.Add(New DataColumn("Descrição"))
        dt.Columns.Add(New DataColumn("Quota"))
        dt.Columns.Add(New DataColumn("Estado"))

        If firstTime Then
            Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))


            Dim howMany As Integer = flightsData.Count
            For i As Integer = 0 To howMany - 1
                dr = dt.NewRow()
                dt.Rows.Add(dr)
            Next
        Else
            dr = dt.NewRow()
            dt.Rows.Add(dr)
        End If
        Session("currentDt") = dt
        fillGrid(dt)
    End Sub

    Private Sub completeData()
        Dim basicData As Dictionary(Of String, String) = CType(Session("basicData"), Dictionary(Of String, String))

        Session("img1") = basicData.Item("Img1")
        Session("img2") = basicData.Item("Img2")
        Session("img3") = basicData.Item("Img3")
        Session("img4") = basicData.Item("Img4")
        Session("img5") = basicData.Item("Img5")
        imagesLbl.Text = Session("img1") & " " & Session("img2") & " " & Session("img3") & " " & Session("img4") & " " & Session("img5")

        nameField.Text = basicData.Item("PackageName")
        itinNameField.Text = basicData.Item("ItineraryName")
        diningField.Text = basicData.Item("Dining")
        includedField.Text = basicData.Item("Included")
        notIncludedField.Text = basicData.Item("NotIncluded")
        notesField.Text = Replace(basicData.Item("Notes"), "<br/><br/>", Environment.NewLine())

        flightCheckBox.Checked = basicData.Item("Flight")
        transferCheckBox.Checked = basicData.Item("Transfer")
        hotelCheckBox.Checked = basicData.Item("Hotel")
        descripNavio.Text = basicData.Item("ShipDescription")

        Session("itinCode") = basicData.Item("ItineraryCode")
        Session("destination") = basicData.Item("Destination")

        Dim currentDt As DataTable = CType(Session("currentDt"), DataTable)

        Dim idField As HiddenField
        Dim compSelector, citySelector, statusSelector As DropDownList
        Dim logoImg As HtmlImage
        Dim descField, quotaField, cityDestField As TextBox

        Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
        For rowIndex As Integer = 0 To flightsData.Count - 1
            currentDt.Rows(rowIndex)("Id") = flightsData.Item(rowIndex).Item("Id")
            currentDt.Rows(rowIndex)("Companhia") = flightsData.Item(rowIndex).Item("Company")
            currentDt.Rows(rowIndex)("Logo") = flightsData.Item(rowIndex).Item("Logo")
            currentDt.Rows(rowIndex)("Cidade") = flightsData.Item(rowIndex).Item("City")
            currentDt.Rows(rowIndex)("CityDest") = flightsData.Item(rowIndex).Item("CityDest")
            currentDt.Rows(rowIndex)("Descrição") = flightsData.Item(rowIndex).Item("Description")
            currentDt.Rows(rowIndex)("Quota") = flightsData.Item(rowIndex).Item("Quota")
            currentDt.Rows(rowIndex)("Estado") = flightsData.Item(rowIndex).Item("Status")

            idField = CType(gridView.Rows(rowIndex).Cells(0).FindControl("flightId"), HiddenField)
            compSelector = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
            logoImg = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
            citySelector = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
            cityDestField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("cityDestField"), TextBox)
            descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
            quotaField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("quotaField"), TextBox)
            statusSelector = CType(gridView.Rows(rowIndex).Cells(5).FindControl("statusSelector"), DropDownList)

            idField.Value = flightsData.Item(rowIndex).Item("Id")
            Dim comp As String = currentDt.Rows(rowIndex)("Companhia").ToString()
            compSelector.ClearSelection()
            compSelector.Items.FindByText(comp).Selected = True
            Dim logo As String = currentDt.Rows(rowIndex)("Logo").ToString()
            logoImg.Src = "Images/Logos/" & logo
            Dim city As String = currentDt.Rows(rowIndex)("Cidade").ToString()
            citySelector.ClearSelection()
            citySelector.Items.FindByText(city).Selected = True
            descField.Text = currentDt.Rows(rowIndex)("Descrição").ToString()
            cityDestField.Text = currentDt.Rows(rowIndex)("CityDest").ToString()
            quotaField.Text = currentDt.Rows(rowIndex)("Quota").ToString()
            Dim status As String = currentDt.Rows(rowIndex)("Estado").ToString()
            statusSelector.ClearSelection()
            statusSelector.Items.FindByText(status).Selected = True
        Next

        Session("currentDt") = currentDt
    End Sub

  Private Sub recreateImages()
    Dim packId As String = sqlAccess.getPackageIds(Session("selPackName"), Session("selSailDate"))
    Dim missingImages As String = ""

    Dim fileName As String
    For imgIndex As Byte = 1 To 5
      fileName = Session("img" & imgIndex)
      If fileName <> "" Then
        If IO.File.Exists(Server.MapPath("Images/Uploads/" & packId.Split("|")(0) & "/" & fileName)) Then
          IO.File.Copy(Server.MapPath("Images/Uploads/" & packId.Split("|")(0) & "/" & fileName), _
                       Server.MapPath("Images/Temp/" & fileName))
        Else
          missingImages &= fileName & " "
        End If
      End If
    Next
    If missingImages <> "" Then
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Alerta', '" _
                                          & "As imagens " & missingImages & "não podem ser encontradas. " _
                                          & "');", True)
    End If
  End Sub

    Private Sub fillGrid(dt As DataTable)
        gridView.DataSource = dt
        gridView.DataBind()
    End Sub

    Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePopup", "$('#popup').modal('hide');", True)

        If Session("detailPending") = "Y" Then
            Session("detailPending") = Nothing
            'prices.Visible = True
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "showPrices();", True)
        ElseIf Session("linePending") = "Y" Then
            Session("linePending") = Nothing
            lines.Visible = True
        End If
        If Session("packageModified") = "Y" Then
            Session("packageModified") = ""
            Response.Redirect("Packages.aspx")
        End If

        scriptManager.RegisterStartupScript(Page, Me.GetType, "setCalendar", "setCalendars();", True)
    End Sub

    Protected Sub closeConfirm(sender As Object, e As EventArgs) Handles yesBtn.Click
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirm').modal('hide');", True)

        Session("fromDetails") = Nothing
        Session("fromLines") = Nothing

        scriptManager.RegisterStartupScript(Page, Me.GetType, "setCalendar", "setCalendars();", True)
    End Sub

    Protected Sub recoverPopup(sender As Object, e As EventArgs) Handles noBtn.Click
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirm').modal('hide');", True)
        If Session("fromDetails") = "Y" Then
            Session("fromDetails") = Nothing
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "showPrices();", True)
        ElseIf Session("fromLines") = "Y" Then
            Session("fromLines") = Nothing
            lines.Visible = True
        End If

        scriptManager.RegisterStartupScript(Page, Me.GetType, "setFields", "setFormats();", True)
    End Sub

    Protected Sub showImages(sender As Object, e As EventArgs) Handles uploadBtn.Click
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showImages", "showImagesPopup('" & Session("img1") & "', '" & Session("img2") & "', '" & Session("img3") & "', '" & Session("img4") & "', '" & Session("img5") & "');", True)
    End Sub

    Protected Sub saveImages(sender As Object, e As EventArgs) Handles saveImagesBtn.Click
        Dim fileName As String

        If uploadCtrl1.HasFile Then
            fileName = uploadCtrl1.FileName

            If Session("img1") = "" Then
                uploadCtrl1.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img1")
                If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
                    My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                End If
                uploadCtrl1.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If

            Session("img1") = fileName
        End If
        If uploadCtrl2.HasFile Then
            fileName = uploadCtrl2.FileName

            If Session("img2") = "" Then
                uploadCtrl2.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img2")
                If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
                    My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                End If
                uploadCtrl2.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If

            Session("img2") = fileName
        End If
        If uploadCtrl3.HasFile Then
            fileName = uploadCtrl3.FileName

            If Session("img3") = "" Then
                uploadCtrl3.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img3")
                If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
                    My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                End If
                uploadCtrl3.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If

            Session("img3") = fileName
        End If
        If uploadCtrl4.HasFile Then
            fileName = uploadCtrl4.FileName

            If Session("img4") = "" Then
                uploadCtrl4.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img4")
                If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
                    My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                End If
                uploadCtrl4.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If

            Session("img4") = fileName
        End If
        If uploadCtrl5.HasFile Then
            fileName = uploadCtrl5.FileName

            If Session("img5") = "" Then
                uploadCtrl5.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            Else
                Dim currentFile As String = Session("img5")
                If My.Computer.FileSystem.FileExists(Server.MapPath("Images/Temp/" & currentFile)) Then
                    My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & currentFile))
                End If
                uploadCtrl5.SaveAs(Server.MapPath("Images/Temp/" & fileName))
            End If

            Session("img5") = fileName
        End If

        imagesLbl.Text = Session("img1") & " " & Session("img2") & " " & Session("img3") & " " & Session("img4") & " " & Session("img5")

        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideImages", "$('#imagesPopup').modal('hide');", True)
    End Sub

    Protected Sub deleteImage(sender As Object, e As EventArgs)
        Dim fileName As String = ""
        Dim imgIndex As Byte = CByte(sender.CommandArgument)
        Select Case imgIndex
            Case 3
                fileName = Session("img3")
            Case 4
                fileName = Session("img4")
            Case 5
                fileName = Session("img5")
        End Select
        My.Computer.FileSystem.DeleteFile(Server.MapPath("Images/Temp/" & fileName))
        Session("img" & imgIndex) = ""

        imagesLbl.Text = Session("img1") & " " & Session("img2") & " " & Session("img3") & " " & Session("img4") & " " & Session("img5")

        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showImages", "showImagesPopup('" & Session("img1") & "', '" & Session("img2") & "', '" & Session("img3") & "', '" & Session("img4") & "', '" & Session("img5") & "');", True)
    End Sub

    Private Function createCustomSelector(name As String) As DropDownList
        Dim result As New DropDownList

        If name = "citySelector" Then
            result.Items.Add(New ListItem("Selecione uma cidade", "-1"))            
            result.Items.Add(New ListItem("Lisboa (Portugal)", "LIS"))
            result.Items.Add(New ListItem("Porto (Portugal)", "OPO"))
            result.Items.Add(New ListItem("Madrid (Espanha)", "MAD"))
            result.Items.Add(New ListItem("Estocolmo (Suecia)", "STO"))
        ElseIf name = "statusSelector" Then
            result.Items.Add(New ListItem("GTD", "GTD"))
            result.Items.Add(New ListItem("RQ", "RQ"))
        ElseIf name = "typeSelector" Then
            result.Items.Add(New ListItem("Antes", "B"))
            result.Items.Add(New ListItem("Depois", "A"))
        End If
        result.Items(0).Selected = True
        result.ID = name

        createCustomSelector = result
    End Function

    Private Sub fillLinesGrid(dt As DataTable)
        linesGrid.DataSource = dt
        linesGrid.DataBind()
    End Sub

    Private Sub initLinesGrid()
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Día"))
        dt.Columns.Add(New DataColumn("Descrição"))
        dt.Columns.Add(New DataColumn("Posição"))

        dr = dt.NewRow()
        dt.Rows.Add(dr)

        Session("linesDt") = dt
        fillLinesGrid(dt)
    End Sub

    Protected Sub addLine(sender As Object, e As EventArgs) Handles lineBtn.Click
        Dim linesDt As DataTable = CType(Session("linesDt"), DataTable)
        If Not linesDt Is Nothing Then
            Dim dtRow As DataRow = Nothing

            Dim rowIndex As Integer = CInt(Session("rowIndex"))
            Dim howMany = linesDt.Rows.Count
            If howMany > 0 Then
                Dim dayField, lineDescField As TextBox
                Dim typeSelector As DropDownList

                Dim day, desc, pos As String

                For lineIndex As Integer = 0 To howMany - 1
                    dayField = CType(linesGrid.Rows(lineIndex).Cells(0).FindControl("dayField"), TextBox)
                    lineDescField = CType(linesGrid.Rows(lineIndex).Cells(1).FindControl("lineDescField"), TextBox)
                    typeSelector = CType(linesGrid.Rows(lineIndex).Cells(2).FindControl("typeSelector"), DropDownList)

                    day = Trim(dayField.Text)
                    desc = Trim(lineDescField.Text)
                    pos = typeSelector.SelectedItem.Value

                    linesDt.Rows(lineIndex)("Día") = day
                    linesDt.Rows(lineIndex)("Descrição") = desc
                    linesDt.Rows(lineIndex)("Posição") = pos
                Next

                dtRow = linesDt.NewRow()
                Dim newTypeSel As DropDownList = createCustomSelector("typeSelector")
                dtRow("Posição") = newTypeSel.SelectedItem.Value

                Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
                Dim lineData As New Dictionary(Of String, String)
                lineData.Add("Day", "")
                If Not linesData.Item(rowIndex).ContainsKey(howMany) Then
                    linesData.Item(rowIndex).Add(howMany, lineData)
                Else
                    linesData.Item(rowIndex).Item(howMany) = lineData
                End If
                Session("linesData") = linesData

                linesDt.Rows.Add(dtRow)
                Session("linesDt") = linesDt
                fillLinesGrid(linesDt)
            Else
                initLinesGrid()
                Exit Sub
            End If
        Else
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Houve um erro na memória.');", True)
        End If

        setPreviousLinesData()
    End Sub

    Protected Sub deleteLine(sender As Object, e As GridViewDeleteEventArgs)
        Dim rowIndex As Integer = CInt(Session("rowIndex"))
        Dim lineIndex As Integer = e.RowIndex
        Dim linesDt As DataTable = CType(Session("linesDt"), DataTable)
        Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))

        linesDt.Rows.RemoveAt(lineIndex)
        linesData.Item(rowIndex).Item(lineIndex).Item("Day") = ""

        Session("linesData") = linesData
        Session("linesDt") = linesDt
        fillLinesGrid(linesDt)

        setPreviousLinesData()
    End Sub

    Private Sub setPreviousLinesData()
        Dim linesDt As DataTable = CType(Session("linesDt"), DataTable)
        If Not linesDt Is Nothing Then
            If linesDt.Rows.Count > 0 Then
                Dim dayField, descField As TextBox
                Dim typeSelector As DropDownList

                For lineIndex As Integer = 0 To linesDt.Rows.Count - 1
                    dayField = CType(linesGrid.Rows(lineIndex).Cells(0).FindControl("dayField"), TextBox)
                    descField = CType(linesGrid.Rows(lineIndex).Cells(1).FindControl("lineDescField"), TextBox)
                    typeSelector = CType(linesGrid.Rows(lineIndex).Cells(2).FindControl("typeSelector"), DropDownList)

                    dayField.Text = linesDt.Rows(lineIndex)("Día").ToString()
                    descField.Text = linesDt.Rows(lineIndex)("Descrição").ToString()
                    Dim pos As String = linesDt.Rows(lineIndex)("Posição")
                    typeSelector.ClearSelection()
                    typeSelector.Items.FindByValue(pos).Selected = True
                Next
            End If
        Else
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Houve um erro na memória.');", True)
        End If
    End Sub

    Protected Sub exitLines(sender As Object, e As EventArgs) Handles exitLinesBtn.Click
        lines.Visible = False

        Session("fromLines") = "Y"
        confirmLbl.Text = "AVISO: Esta ação faz com que a perda de qualquer contribuição adicional para o pop-up ... certeza que você deixar essa janela?"
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showConfirm", "showConfirm();", True)
    End Sub

    Protected Sub saveLines(sender As Object, e As EventArgs) Handles saveLinesBtn.Click
        If linesGrid.Rows.Count > 0 Then
            If lineIsFilled() Then
                Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
                Dim linesMap As New Dictionary(Of Integer, Dictionary(Of String, String))
                Dim lineData As Dictionary(Of String, String)

                Dim dayField, lineDescField As TextBox
                Dim typeSelector As DropDownList

                For lineIndex As Integer = 0 To linesGrid.Rows.Count - 1
                    dayField = CType(linesGrid.Rows(lineIndex).Cells(0).FindControl("dayField"), TextBox)
                    lineDescField = CType(linesGrid.Rows(lineIndex).Cells(1).FindControl("lineDescField"), TextBox)
                    typeSelector = CType(linesGrid.Rows(lineIndex).Cells(2).FindControl("typeSelector"), DropDownList)

                    lineData = New Dictionary(Of String, String)
                    lineData.Add("Day", Trim(dayField.Text))
                    lineData.Add("Description", Trim(lineDescField.Text))
                    lineData.Add("Position", typeSelector.SelectedItem.Value)

                    linesMap.Add(lineIndex, lineData)
                Next

                Dim rowIndex As Integer = CInt(Session("rowIndex"))
                If Not linesData.ContainsKey(rowIndex) Then
                    linesData.Add(rowIndex, linesMap)
                Else
                    linesData.Item(rowIndex) = linesMap
                End If

                lines.Visible = False
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'As linhas do itinerário para o vôo foram salvas com sucesso.');", True)
            Else
                Session("linePending") = "Y"

                lines.Visible = False
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Há ainda estão pendentes de preencher os dados. Por favor, insira as informações.');", True)
            End If
        Else
            Session("linePending") = "Y"

            lines.Visible = False
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Ainda não adicionou qualquer informação online. Por favor, insira pelo menos um para o vôo.');", True)
        End If
    End Sub

    ' GUILLERMO - Este procedimiento hay que corregirlo para que muestre las líneas de itin. asociadas según el vuelo clickado (esto es, también detectar si viene de la BD o es nuevo)
    Protected Sub showLines(sender As Object, e As EventArgs)
        Dim rowBtn As Button = CType(sender, Button)
        Dim gridRow As GridViewRow = CType(rowBtn.NamingContainer, GridViewRow)
        Dim rowIndex As Integer = gridRow.RowIndex

        If rowIsFilled(rowIndex) Then
            Session("rowIndex") = rowIndex

            Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
      If linesData.Item(rowIndex).Count = 0 Then
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Día"))
        dt.Columns.Add(New DataColumn("Descrição"))
        dt.Columns.Add(New DataColumn("Posição"))

        Dim flightId As HiddenField = CType(gridRow.Cells(0).FindControl("flightId"), HiddenField)
        If flightId.Value <> -1 Then
          Dim linesDt As DataTable = sqlAccess.getItineraryLines(flightId.Value)
          If (Not linesDt Is Nothing) And (linesDt.Rows.Count > 0) Then
            Dim linesMap As New Dictionary(Of Integer, Dictionary(Of String, String))
            Dim lineData As Dictionary(Of String, String)

            Dim i As Integer = 0
            For Each row In linesDt.Rows
              dr = dt.NewRow()
              dr("Día") = row("Day")
              dr("Descrição") = row("Description")
              dr("Posição") = row("BeforeAfter")
              dt.Rows.Add(dr)

              lineData = New Dictionary(Of String, String)
              lineData.Add("Day", row("Day"))
              lineData.Add("Description", row("Description"))
              lineData.Add("Position", row("BeforeAfter"))
              linesMap.Add(i, lineData)

              i += 1
            Next
            linesData.Item(rowIndex) = linesMap
            Session("linesData") = linesData
            Session("linesDt") = dt
            fillLinesGrid(dt)

            Dim dayField, lineDescField As TextBox
            Dim typeSelector As DropDownList

            Dim pos As String

            For lineIndex As Integer = 0 To dt.Rows.Count - 1
              dayField = CType(linesGrid.Rows(lineIndex).Cells(0).FindControl("dayField"), TextBox)
              lineDescField = CType(linesGrid.Rows(lineIndex).Cells(1).FindControl("lineDescField"), TextBox)
              typeSelector = CType(linesGrid.Rows(lineIndex).Cells(2).FindControl("typeSelector"), DropDownList)

              dayField.Text = dt.Rows(lineIndex)("Día")
              lineDescField.Text = dt.Rows(lineIndex)("Descrição")
              pos = dt.Rows(lineIndex)("Posição")
              typeSelector.ClearSelection()
              typeSelector.Items.FindByValue(pos).Selected = True
            Next
          Else
            dr = dt.NewRow()
            dt.Rows.Add(dr)

            Session("linesDt") = dt
            fillLinesGrid(dt)
          End If
        Else
          dr = dt.NewRow()
          dt.Rows.Add(dr)

          Session("linesDt") = dt
          fillLinesGrid(dt)
        End If
      Else
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Día"))
        dt.Columns.Add(New DataColumn("Descrição"))
        dt.Columns.Add(New DataColumn("Posição"))

        Dim howMany As Integer = linesData.Item(rowIndex).Count
        For itemIndex As Integer = 0 To howMany - 1
          dr = dt.NewRow()
          dr("Día") = linesData.Item(rowIndex).Item(itemIndex).Item("Day")
          dr("Descrição") = linesData.Item(rowIndex).Item(itemIndex).Item("Description")
          dr("Posição") = linesData.Item(rowIndex).Item(itemIndex).Item("Position")

          dt.Rows.Add(dr)
        Next
        Session("linesDt") = dt
        fillLinesGrid(dt)

        Dim dayField, descField As TextBox
        Dim typeSelector As DropDownList

        Dim pos As String

        For lineIndex As Integer = 0 To howMany - 1
          dayField = CType(linesGrid.Rows(lineIndex).Cells(0).FindControl("dayField"), TextBox)
          descField = CType(linesGrid.Rows(lineIndex).Cells(1).FindControl("lineDescField"), TextBox)
          typeSelector = CType(linesGrid.Rows(lineIndex).Cells(2).FindControl("typeSelector"), DropDownList)

          dayField.Text = linesData.Item(rowIndex).Item(lineIndex).Item("Day")
          descField.Text = linesData.Item(rowIndex).Item(lineIndex).Item("Description")
          pos = linesData.Item(rowIndex).Item(lineIndex).Item("Position")
          typeSelector.ClearSelection()
          typeSelector.Items.FindByValue(pos).Selected = True
        Next
      End If

            lines.Visible = True
        End If
    End Sub

    Protected Sub addRow(sender As Object, e As EventArgs) Handles flightBtn.Click
        Dim howMany As Integer = gridView.Rows.Count
        If rowIsFilled() Then
            If Not Session("currentDt") Is Nothing Then
                Dim currentDt As DataTable = CType(Session("currentDt"), DataTable)
                Dim dtRow As DataRow = Nothing

                If currentDt.Rows.Count > 0 Then
                    Dim id, comp, logo, city, desc, quota, status As String

                    Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                    Dim flightData As Dictionary(Of String, String)

                    For rowIndex As Integer = 0 To currentDt.Rows.Count - 1
                        Dim flightId As HiddenField = CType(gridView.Rows(rowIndex).Cells(0).FindControl("flightId"), HiddenField)
                        Dim compSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
                        Dim logoImg As HtmlImage = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
                        Dim citySelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
                        Dim descField As TextBox = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
                        Dim quotaField As TextBox = CType(gridView.Rows(rowIndex).Cells(4).FindControl("quotaField"), TextBox)
                        Dim statusSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(5).FindControl("statusSelector"), DropDownList)

                        dtRow = currentDt.NewRow()

                        id = flightId.Value
                        comp = compSelector.Items(compSelector.SelectedIndex).Text
                        logo = logoImg.Src
                        city = citySelector.Items(citySelector.SelectedIndex).Text
                        desc = Trim(descField.Text)
                        quota = Trim(quotaField.Text)
                        status = statusSelector.Items(statusSelector.SelectedIndex).Text

                        ' Recolección de valores en las filas existentes hasta el momento
                        currentDt.Rows(rowIndex)("Id") = id
                        currentDt.Rows(rowIndex)("Companhia") = comp
                        currentDt.Rows(rowIndex)("Logo") = logo
                        currentDt.Rows(rowIndex)("Cidade") = city
                        currentDt.Rows(rowIndex)("Descrição") = desc
                        currentDt.Rows(rowIndex)("Quota") = quota
                        currentDt.Rows(rowIndex)("Estado") = status

                        ' Guardamos por cada fila los datos en una variable de tipo Dictionary, para posteriormente añadirla a la variable global "flightsData"
                        flightData = New Dictionary(Of String, String)
                        flightData.Add("Id", id)
                        flightData.Add("Company", comp)
                        flightData.Add("Logo", logo)
                        flightData.Add("City", city)
                        flightData.Add("Description", desc)
                        flightData.Add("Quota", quota)
                        flightData.Add("Status", status)
                        If Not flightsData.ContainsKey(rowIndex) Then
                            flightsData.Add(rowIndex, flightData)
                        Else
                            flightsData.Item(rowIndex) = flightData
                        End If
                    Next

                    dtRow("Id") = -1
                    dtRow("Companhia") = "Tap Air Portugal"
                    dtRow("Logo") = "Images/Logos/tap_logo.png"
                    Dim newCitySel As DropDownList = createCustomSelector("citySelector")
                    dtRow("Cidade") = newCitySel.SelectedItem.Text
                    Dim newStatusSel As DropDownList = createCustomSelector("statusSelector")
                    dtRow("Estado") = newStatusSel.SelectedItem.Text
                    currentDt.Rows.Add(dtRow) ' Adición de la nueva fila vacía

                    flightData = New Dictionary(Of String, String)
                    flightData.Add("Id", "")
                    flightData.Add("Company", "")
                    flightData.Add("Logo", "")
                    flightData.Add("City", "")
                    flightData.Add("Description", "")
                    flightData.Add("Quota", "")
                    flightData.Add("Status", "")
                    If Not flightsData.ContainsKey(currentDt.Rows.Count - 1) Then
                        flightsData.Add(currentDt.Rows.Count - 1, flightData)
                    Else
                        flightsData.Item(currentDt.Rows.Count - 1) = flightData
                    End If


                    Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                    Dim detailData As New Dictionary(Of String, String)
                    detailData.Add("Code", "")
                    If Not detailsData.ContainsKey(howMany) Then
                        detailsData.Add(howMany, detailData)
                    Else
                        detailsData.Item(howMany) = detailData
                    End If
                    Session("detailsData") = detailsData
                    Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
                    If Not pendingData.ContainsKey(howMany) Then
                        pendingData.Add(howMany, "Y") ' Marcamos la nueva fila como pendiente de rellenar sus detalles
                    Else
                        pendingData.Item(howMany) = "Y"
                    End If
                    Session("pendingData") = pendingData

                    Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
                    Dim linesMap As New Dictionary(Of Integer, Dictionary(Of String, String))
                    Dim lineData As New Dictionary(Of String, String)
                    lineData.Add("Day", "")
                    linesMap.Add(0, lineData)
                    If Not linesData.ContainsKey(howMany) Then
                        linesData.Add(howMany, linesMap)
                    Else
                        linesData.Item(howMany) = linesMap
                    End If
                    Session("linesData") = linesData

                    Session("flightsData") = flightsData
                    Session("currentDt") = currentDt
                    fillGrid(currentDt)
                Else
                    initGrid(False)
                    scriptManager.RegisterStartupScript(Page, Me.GetType, "setCalendar", "setCalendars();", True)
                    Exit Sub
                End If
            Else
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Houve um erro na memória.');", True)
            End If

            setPreviousData()
        End If
    End Sub

    Protected Sub deleteRow(sender As Object, e As GridViewDeleteEventArgs)
        Dim rowIndex As Integer = e.RowIndex

        Dim currentDt As DataTable = CType(Session("currentDt"), DataTable)
        Dim flightsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
        Dim linesData As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))
        Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))

        currentDt.Rows.RemoveAt(rowIndex)
        flightsData.Item(rowIndex).Item("Company") = ""
        detailsData.Item(rowIndex).Item("Code") = ""
        linesData.Item(rowIndex).Item(0).Item("Day") = ""
        pendingData.Item(rowIndex) = ""

        Session("flightsData") = flightsData
        Session("detailsData") = detailsData
        Session("linesData") = linesData
        Session("pendingData") = pendingData
        Session("currentDt") = currentDt
        fillGrid(currentDt)

        setPreviousData()
    End Sub

    Private Sub setPreviousData()
        If Not Session("currentDt") Is Nothing Then
            Dim dt As DataTable = CType(Session("currentDt"), DataTable)

            Dim flightId As HiddenField
            Dim compSelector, citySelector, statusSelector As DropDownList
            Dim logoImg As HtmlImage
            Dim descField, quotaField As TextBox

            Dim comp, city, status As String

            If dt.Rows.Count > 0 Then
                For rowIndex As Integer = 0 To dt.Rows.Count - 1
                    flightId = CType(gridView.Rows(rowIndex).Cells(0).FindControl("flightId"), HiddenField)
                    compSelector = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
                    logoImg = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
                    citySelector = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
                    descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
                    quotaField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("quotaField"), TextBox)
                    statusSelector = CType(gridView.Rows(rowIndex).Cells(5).FindControl("statusSelector"), DropDownList)

                    flightId.Value = dt.Rows(rowIndex)("Id")
                    comp = dt.Rows(rowIndex)("Companhia").ToString()
                    compSelector.ClearSelection()
                    compSelector.Items.FindByText(comp).Selected = True
                    logoImg.Src = dt.Rows(rowIndex)("Logo").ToString()
                    city = dt.Rows(rowIndex)("Cidade").ToString()
                    citySelector.ClearSelection()
                    citySelector.Items.FindByText(city).Selected = True
                    descField.Text = dt.Rows(rowIndex)("Descrição").ToString()
                    quotaField.Text = dt.Rows(rowIndex)("Quota").ToString()
                    status = dt.Rows(rowIndex)("Estado").ToString()
                    statusSelector.ClearSelection()
                    statusSelector.Items.FindByText(status).Selected = True
                Next

                scriptManager.RegisterStartupScript(Page, Me.GetType, "setCalendar", "setCalendars();", True)
            End If
        End If
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

    Protected Sub showDetails(sender As Object, e As EventArgs)
        Dim showBtn As Button = CType(sender, Button)
        Dim btnRow As GridViewRow = CType(showBtn.NamingContainer, GridViewRow)
        Dim rowIndex As Integer = btnRow.RowIndex
        Session("rowIndex") = rowIndex

        If rowIsFilled(rowIndex) Then
            Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
            If detailsData.Item(rowIndex).Item("Code") <> "" Then
                flightCode.Text = detailsData.Item(rowIndex).Item("Code")
                flightDate.Text = detailsData.Item(rowIndex).Item("Date")
                flightHour.Text = detailsData.Item(rowIndex).Item("Hour")
                flightTaxes.Text = detailsData.Item(rowIndex).Item("Taxes")
                returnFlight.Text = detailsData.Item(rowIndex).Item("Return")
                returnFlightTime.Text = detailsData.Item(rowIndex).Item("ReturnFlightTime")
                flightTransfer.Text = detailsData.Item(rowIndex).Item("Transfer")
                flightHotel.Text = detailsData.Item(rowIndex).Item("Hotel")
                offerField.Text = detailsData.Item(rowIndex).Item("Offer")

                int1.Text = detailsData.Item(rowIndex).Item("IDouble")
                int2.Text = detailsData.Item(rowIndex).Item("ITriCua")
                int3.Text = detailsData.Item(rowIndex).Item("I34Ad")
                int4.Text = detailsData.Item(rowIndex).Item("I34Cr")
                int5.Text = detailsData.Item(rowIndex).Item("I34Bb")
                int6.Text = detailsData.Item(rowIndex).Item("IInd")
                int7.Text = detailsData.Item(rowIndex).Item("IOffer")
                ext1.Text = detailsData.Item(rowIndex).Item("ODouble")
                ext2.Text = detailsData.Item(rowIndex).Item("OTriCua")
                ext3.Text = detailsData.Item(rowIndex).Item("O34Ad")
                ext4.Text = detailsData.Item(rowIndex).Item("O34Cr")
                ext5.Text = detailsData.Item(rowIndex).Item("O34Bb")
                ext6.Text = detailsData.Item(rowIndex).Item("OInd")
                ext7.Text = detailsData.Item(rowIndex).Item("OOffer")
                bal1.Text = detailsData.Item(rowIndex).Item("BDouble")
                bal2.Text = detailsData.Item(rowIndex).Item("BTriCua")
                bal3.Text = detailsData.Item(rowIndex).Item("B34Ad")
                bal4.Text = detailsData.Item(rowIndex).Item("B34Cr")
                bal5.Text = detailsData.Item(rowIndex).Item("B34Bb")
                bal6.Text = detailsData.Item(rowIndex).Item("BInd")
                bal7.Text = detailsData.Item(rowIndex).Item("BOffer")
                su1.Text = detailsData.Item(rowIndex).Item("SDouble")
                su2.Text = detailsData.Item(rowIndex).Item("STriCua")
                su3.Text = detailsData.Item(rowIndex).Item("S34Ad")
                su4.Text = detailsData.Item(rowIndex).Item("S34Cr")
                su5.Text = detailsData.Item(rowIndex).Item("S34Bb")
                su6.Text = detailsData.Item(rowIndex).Item("SInd")
                su7.Text = detailsData.Item(rowIndex).Item("SOffer")
            Else
                flightCode.Text = ""
                flightDate.Text = ""
                flightHour.Text = ""
                flightTaxes.Text = ""
                returnFlight.Text = ""
                returnFlightTime.Text = ""
                flightTransfer.Text = ""
                flightHotel.Text = ""
                offerField.Text = ""

                int1.Text = ""
                int2.Text = ""
                int3.Text = ""
                int4.Text = ""
                int5.Text = ""
                int6.Text = ""
                int7.Text = ""
                ext1.Text = ""
                ext2.Text = ""
                ext3.Text = ""
                ext4.Text = ""
                ext5.Text = ""
                ext6.Text = ""
                ext7.Text = ""
                bal1.Text = ""
                bal2.Text = ""
                bal3.Text = ""
                bal4.Text = ""
                bal5.Text = ""
                bal6.Text = ""
                bal7.Text = ""
                su1.Text = ""
                su2.Text = ""
                su3.Text = ""
                su4.Text = ""
                su5.Text = ""
                su6.Text = ""
                su7.Text = ""
            End If

            'prices.Visible = True
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPrices", "showPrices();", True)
        End If
    End Sub

    Protected Sub exitPrices(sender As Object, e As EventArgs) Handles exitBtn.Click
        'prices.Visible = False
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePrices", "$('#prices').modal('hide');", True)

        Session("fromDetails") = "Y"
        confirmLbl.Text = "AVISO: Esta ação faz com que a perda de qualquer contribuição adicional para o pop-up ... certeza que você deixar essa janela?"
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showConfirm", "showConfirm();", True)
    End Sub

    Protected Sub savePrices(sender As Object, e As EventArgs) Handles saveBtn.Click
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
        Dim rowSelected As Integer = CInt(Session("rowIndex"))

        Dim code, day, hour, taxes, retFlight, retFlightTime, transfer, hotel, offerDesc As String
        Dim errorStr As String = ""

        code = Trim(flightCode.Text)
        day = flightDate.Text
        hour = Trim(flightHour.Text)
        taxes = Trim(flightTaxes.Text)
        retFlight = Trim(returnFlight.Text)
        retFlightTime = Trim(returnFlightTime.Text)
        transfer = Trim(flightTransfer.Text)
        hotel = Trim(flightHotel.Text)
        offerDesc = Trim(offerField.Text)

    If (code <> "") And (day <> "") And (hour <> "") And (taxes <> "") And (retFlight <> "") And (transfer <> "") Then
      detailsData.Item(rowSelected).Item("Code") = code
      detailsData.Item(rowSelected).Item("Date") = day
      detailsData.Item(rowSelected).Item("Hour") = hour
      detailsData.Item(rowSelected).Item("Taxes") = taxes
      detailsData.Item(rowSelected).Item("Return") = retFlight
      detailsData.Item(rowSelected).Item("ReturnFlightTime") = retFlightTime
      detailsData.Item(rowSelected).Item("Transfer") = transfer
      detailsData.Item(rowSelected).Item("Hotel") = hotel
      detailsData.Item(rowSelected).Item("Offer") = offerDesc

      detailsData.Item(rowSelected).Item("IDouble") = Trim(int1.Text)
      detailsData.Item(rowSelected).Item("ITriCua") = Trim(int2.Text)
      detailsData.Item(rowSelected).Item("I34Ad") = Trim(int3.Text)
      detailsData.Item(rowSelected).Item("I34Cr") = Trim(int4.Text)
      detailsData.Item(rowSelected).Item("I34Bb") = Trim(int5.Text)
      detailsData.Item(rowSelected).Item("IInd") = Trim(int6.Text)
      detailsData.Item(rowSelected).Item("IOffer") = Trim(int7.Text)
      detailsData.Item(rowSelected).Item("ODouble") = Trim(ext1.Text)
      detailsData.Item(rowSelected).Item("OTriCua") = Trim(ext2.Text)
      detailsData.Item(rowSelected).Item("O34Ad") = Trim(ext3.Text)
      detailsData.Item(rowSelected).Item("O34Cr") = Trim(ext4.Text)
      detailsData.Item(rowSelected).Item("O34Bb") = Trim(ext5.Text)
      detailsData.Item(rowSelected).Item("OInd") = Trim(ext6.Text)
      detailsData.Item(rowSelected).Item("OOffer") = Trim(ext7.Text)
      detailsData.Item(rowSelected).Item("BDouble") = Trim(bal1.Text)
      detailsData.Item(rowSelected).Item("BTriCua") = Trim(bal2.Text)
      detailsData.Item(rowSelected).Item("B34Ad") = Trim(bal3.Text)
      detailsData.Item(rowSelected).Item("B34Cr") = Trim(bal4.Text)
      detailsData.Item(rowSelected).Item("B34Bb") = Trim(bal5.Text)
      detailsData.Item(rowSelected).Item("BInd") = Trim(bal6.Text)
      detailsData.Item(rowSelected).Item("BOffer") = Trim(bal7.Text)
      detailsData.Item(rowSelected).Item("SDouble") = Trim(su1.Text)
      detailsData.Item(rowSelected).Item("STriCua") = Trim(su2.Text)
      detailsData.Item(rowSelected).Item("S34Ad") = Trim(su3.Text)
      detailsData.Item(rowSelected).Item("S34Cr") = Trim(su4.Text)
      detailsData.Item(rowSelected).Item("S34Bb") = Trim(su5.Text)
      detailsData.Item(rowSelected).Item("SInd") = Trim(su6.Text)
      detailsData.Item(rowSelected).Item("SOffer") = Trim(su7.Text)

      Session("detailsData") = detailsData

      Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))
      pendingData.Item(rowSelected) = "N"
      Session("pendingData") = pendingData
    Else
pendingData:
      Session("detailPending") = "Y"

      errorStr = "Há ainda estão pendentes de preencher os dados. Por favor, insira as informações."
    End If

        'prices.Visible = False
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hidePrices", "$('#prices').modal('hide');", True)
        If errorStr = "" Then
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Os detalhes do voo foram salvas com sucesso.');", True)
        Else
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Há ainda estão pendentes de preencher os dados. Por favor, insira as informações.');", True)
        End If
    End Sub

    Protected Sub goBack(sender As Object, e As EventArgs) Handles backBtn.Click
        Response.Redirect("Packages.aspx")
    End Sub

    Private Function rowIsFilled(Optional rowIndex As Integer = -1) As Boolean
        Dim result As Boolean = True

        Dim citySelector As DropDownList
        Dim descField, quotaField As TextBox

        If rowIndex <> -1 Then
            citySelector = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
            descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
            quotaField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("quotaField"), TextBox)

            If (citySelector.Items(citySelector.SelectedIndex).Value = "-1") Or (descField.Text = "") Or (quotaField.Text = "") Then
                result = False
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na linha " & (rowIndex + 1) & " antes de prosseguir.');", True)
            End If
        Else
            For i As Integer = 0 To gridView.Rows.Count - 1
                citySelector = CType(gridView.Rows(i).Cells(2).FindControl("citySelector"), DropDownList)
                descField = CType(gridView.Rows(i).Cells(3).FindControl("descField"), TextBox)
                quotaField = CType(gridView.Rows(i).Cells(4).FindControl("quotaField"), TextBox)

                If (citySelector.Items(citySelector.SelectedIndex).Value = "-1") Or (descField.Text = "") Or (quotaField.Text = "") Then
                    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na tabela antes de prosseguir.');", True)
                    Return False
                End If
            Next
        End If

        rowIsFilled = result
    End Function

    Private Function lineIsFilled(Optional lineIndex As Integer = -1) As Boolean
        Dim result As Boolean = True

        Dim dayField, descField As TextBox

        If lineIndex <> -1 Then
            dayField = CType(linesGrid.Rows(lineIndex).Cells(0).FindControl("dayField"), TextBox)
            descField = CType(linesGrid.Rows(lineIndex).Cells(1).FindControl("lineDescField"), TextBox)

            If (dayField.Text = "") Or (descField.Text = "") Then
                result = False
                lines.Visible = False
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na linha " & (lineIndex + 1) & " antes de prosseguir.');", True)
            End If
        Else
            For i As Integer = 0 To linesGrid.Rows.Count - 1
                dayField = CType(linesGrid.Rows(i).Cells(0).FindControl("dayField"), TextBox)
                descField = CType(linesGrid.Rows(i).Cells(1).FindControl("lineDescField"), TextBox)

                If (dayField.Text = "") Or (descField.Text = "") Then
                    lines.Visible = False
                    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Por favor, preencha as informações em falta na tabela antes de prosseguir.');", True)
                    Return False
                End If
            Next
        End If

        lineIsFilled = result
    End Function

    Private Function transformTime(time As String, Optional fromDB As Char = "N") As String
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

    Protected Sub companyChanged(sender As Object, e As EventArgs)
        Dim compSelector As DropDownList = CType(sender, DropDownList)
        Dim gridRow As GridViewRow = CType(compSelector.NamingContainer, GridViewRow)

        Dim compName As String = compSelector.SelectedItem.Text
        Dim fileName As String = sqlAccess.getCompanyLogo(compName)

        Dim logoImg As HtmlImage = CType(gridRow.Cells(1).FindControl("logoImg"), HtmlImage)
        logoImg.Src = "Images/Logos/" & fileName

        scriptManager.RegisterStartupScript(Page, Me.GetType, "setCalendars", "setCalendars();", True)
    End Sub

    Protected Sub registerPackageOld(sender As Object, e As EventArgs) 'Handles editRegisterBtn.Click
        loadPanel.Visible = True

        Dim errorStr As String = ""
        Dim currentData As New DataTable
        Dim newRow As DataRow

        Dim flightId As HiddenField
        Dim compSelector, citySelector, statusSelector As DropDownList
        Dim logoImg As HtmlImage
        Dim descField, cityDestField, quotaField As TextBox
        Dim logo, cityValue, description, cityDest, quota, status As String

        currentData.Columns.Add(New DataColumn("Id"))
        currentData.Columns.Add(New DataColumn("Companhia"))
        currentData.Columns.Add(New DataColumn("Logo"))
        currentData.Columns.Add(New DataColumn("Cidade"))
        currentData.Columns.Add(New DataColumn("CityDest"))
        currentData.Columns.Add(New DataColumn("Descrição"))
        currentData.Columns.Add(New DataColumn("Quota"))
        currentData.Columns.Add(New DataColumn("Estado"))

        Dim flightsData As New Dictionary(Of Integer, Dictionary(Of String, String))
        Dim flightData As Dictionary(Of String, String)
        Dim detailsData As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))

        Dim splitLogo() As String

        For rowIndex As Integer = 0 To gridView.Rows.Count - 1
            newRow = currentData.NewRow()
            flightData = New Dictionary(Of String, String)

            flightId = CType(gridView.Rows(rowIndex).Cells(0).FindControl("flightId"), HiddenField)
            compSelector = CType(gridView.Rows(rowIndex).Cells(0).FindControl("compSelector"), DropDownList)
            logoImg = CType(gridView.Rows(rowIndex).Cells(1).FindControl("logoImg"), HtmlImage)
            citySelector = CType(gridView.Rows(rowIndex).Cells(2).FindControl("citySelector"), DropDownList)
            cityDestField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("cityDestField"), TextBox)
            descField = CType(gridView.Rows(rowIndex).Cells(3).FindControl("descField"), TextBox)
            quotaField = CType(gridView.Rows(rowIndex).Cells(4).FindControl("quotaField"), TextBox)
            statusSelector = CType(gridView.Rows(rowIndex).Cells(5).FindControl("statusSelector"), DropDownList)
            splitLogo = logoImg.Src.Split("/")
            logo = splitLogo(splitLogo.Length - 1)
            cityValue = citySelector.SelectedItem.Value
            cityDest = Trim(cityDestField.Text)
            description = Trim(descField.Text)
            quota = Trim(quotaField.Text)
            status = statusSelector.SelectedItem.Text

            If (cityValue = "-1") Or (description = "") Or (quota = "") Then
                errorStr = "Eles foram detectadas preenchendo as mudanças " & (rowIndex + 1) & " pendentes linha da tabela de voos. Por favor, inclua esta informação."
                Exit For
            End If

            newRow("Id") = flightId.Value
            newRow("Companhia") = compSelector.SelectedItem.Text
            newRow("Logo") = logo
            newRow("Cidade") = citySelector.SelectedItem.Text
            newRow("CityDest") = cityDest
            newRow("Descrição") = description
            newRow("Quota") = quota
            newRow("Estado") = status
            currentData.Rows.Add(newRow)

            flightData.Add("Id", flightId.Value)
            flightData.Add("Company", compSelector.SelectedItem.Text)
            flightData.Add("Logo", logo)
            flightData.Add("City", citySelector.SelectedItem.Text)
            flightData.Add("CityDest", cityDest)
            flightData.Add("Description", description)
            flightData.Add("Quota", quota)
            flightData.Add("Status", status)
            flightsData.Add(rowIndex, flightData)
        Next

        Session("detailsData") = detailsData

        If errorStr = "" Then
            Session("currentDt") = currentData
            Session("flightsData") = flightsData

            Dim flightsNum As Integer = gridView.Rows.Count
            If flightsNum <> 0 Then
                Dim pendingData As Dictionary(Of Integer, Char) = CType(Session("pendingData"), Dictionary(Of Integer, Char))

                For Each item In pendingData
                    If item.Value = "Y" Then
                        errorStr = "Eles foram detectados por preencher os detalhes da linha " & (item.Key + 1) & " alterações pendentes. Por favor, reveja esta informação."
                        Exit For
                    End If
                Next

                If errorStr = "" Then
                    Dim packageData(3) As Object
                    Dim basicData As New Dictionary(Of String, String)

                    Dim packName, itinName, itinCode, destination, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes As String

                    packName = Trim(nameField.Text)
                    itinName = Trim(itinNameField.Text)
                    itinCode = Session("itinCode")
                    destination = Session("destination")
                    img1 = Session("img1")
                    img2 = Session("img2")
                    img3 = Session("img3")
                    img4 = Session("img4")
                    img5 = Session("img5")
                    dining = Trim(diningField.Text)
                    includedTxt = Trim(includedField.Text)
                    notIncludedTxt = Trim(notIncludedField.Text)
                    notes = Trim(notesField.Text)

                    If packName = "" Then
                        errorStr = "Você deve digitar o nome do pacote. Por favor, inclua esta informação."
                    ElseIf itinName = "" Then
                        errorStr = "É necessário introduzir o nome para o itinerário. Por favor, inclua esta informação."
                    ElseIf dining = "" Then
                        errorStr = "Você entra a refeição turno. Por favor, inclua esta informação."
                    ElseIf includedTxt = "" Then
                        errorStr = "Você deve especificar que é o que está incluído neste pacote. Por favor, adicione esta informação."
                    ElseIf notIncludedTxt = "" Then
                        errorStr = "É necessário especificar que não está incluído neste pacote. Por favor, adicione esta informação."
                    End If

                    If errorStr = "" Then
                        basicData.Add("PackageName", packName)
                        basicData.Add("SailingDate", Session("selSailDate"))
                        basicData.Add("ItineraryName", itinName)
                        basicData.Add("ItineraryCode", itinCode)
                        basicData.Add("Destination", destination)
                        basicData.Add("Img1", img1)
                        basicData.Add("Img2", img2)
                        basicData.Add("Img3", img3)
                        basicData.Add("Img4", img4)
                        basicData.Add("Img5", img5)
                        basicData.Add("Dining", dining)
                        basicData.Add("Included", includedTxt)
                        basicData.Add("NotIncluded", notIncludedTxt)
                        basicData.Add("Notes", notes)

                        Dim currentFlights As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("flightsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                        Dim currentDetails As Dictionary(Of Integer, Dictionary(Of String, String)) = CType(Session("detailsData"), Dictionary(Of Integer, Dictionary(Of String, String)))
                        Dim currentLines As Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))) = CType(Session("linesData"), Dictionary(Of Integer, Dictionary(Of Integer, Dictionary(Of String, String))))

                        packageData.SetValue(basicData, 0)
                        packageData.SetValue(currentFlights, 1)
                        packageData.SetValue(currentDetails, 2)
                        packageData.SetValue(currentLines, 3)

                        Dim someError As String = sqlAccess.editPackage(packageData)
                        If someError <> "" Then
                            Dim oldPackId As Integer
                            If Integer.TryParse(someError, oldPackId) Then
                                Dim newPackId As Integer = sqlAccess.getPackageIds(Session("selPackName"), Session("selSailDate"))
                                'Try
                                IO.Directory.Move(Server.MapPath("Images/Temp"), Server.MapPath("Images/Uploads/" & newPackId))
                                IO.Directory.Delete(Server.MapPath("Images/Uploads/" & oldPackId), True)
                                'Catch ex As Exception

                                'End Try

                                Session("packageModified") = "Y"
                            Else
                                errorStr = someError
                            End If
                        End If
                    End If
                End If
            Else
                errorStr = "Ainda não adicionou informações sobre qualquer voo. Por favor, insira pelo menos um para o pacote."
            End If
        End If

        If errorStr = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Pacote alterado com êxito.');", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & Replace(errorStr, Environment.NewLine, " ") & "');", True)
        End If
        loadPanel.Visible = False
    End Sub

    Protected Sub registerPackage(sender As Object, e As EventArgs) Handles editRegisterBtn.Click
        loadPanel.Visible = True

        Dim errorStr As String = ""

        If errorStr = "" Then
            Dim packageData(1) As Object
            Dim basicData As New Dictionary(Of String, String)

            Dim packName, itinName, itinCode, destination, img1, img2, img3, img4, img5, dining, includedTxt, notIncludedTxt, notes As String

            packName = Trim(nameField.Text)
            itinName = Trim(itinNameField.Text)
            itinCode = Session("itinCode")
            destination = Session("destination")
            img1 = Session("img1")
            img2 = Session("img2")
            img3 = Session("img3")
            img4 = Session("img4")
            img5 = Session("img5")
            dining = Trim(diningField.Text)
            includedTxt = Trim(includedField.Text)
            notIncludedTxt = Trim(notIncludedField.Text)
            notes = Trim(notesField.Text)

            If packName = "" Then
                errorStr = "Você deve digitar o nome do pacote. Por favor, inclua esta informação."
            ElseIf itinName = "" Then
                errorStr = "É necessário introduzir o nome para o itinerário. Por favor, inclua esta informação."
            ElseIf dining = "" Then
                errorStr = "Você entra a refeição turno. Por favor, inclua esta informação."
            ElseIf includedTxt = "" Then
                errorStr = "Você deve especificar que é o que está incluído neste pacote. Por favor, adicione esta informação."
            ElseIf notIncludedTxt = "" Then
                errorStr = "É necessário especificar que não está incluído neste pacote. Por favor, adicione esta informação."
            End If

            If errorStr = "" Then
                basicData.Add("PackageName", packName)
                basicData.Add("SailingDate", Session("selSailDate"))
                basicData.Add("ItineraryName", itinName)
                basicData.Add("ItineraryCode", itinCode)
                basicData.Add("Destination", destination)
                basicData.Add("Img1", img1)
                basicData.Add("Img2", img2)
                basicData.Add("Img3", img3)
                basicData.Add("Img4", img4)
                basicData.Add("Img5", img5)
                basicData.Add("Dining", dining)
                basicData.Add("Included", includedTxt)
                basicData.Add("NotIncluded", notIncludedTxt)
                basicData.Add("Notes", notes)

                Dim shipDescription = Trim(descripNavio.Text)

                basicData.Add("ShipDescription", shipDescription)

                basicData.Add("Flight", flightCheckBox.Checked)
                basicData.Add("Transfer", transferCheckBox.Checked)
                basicData.Add("Hotel", hotelCheckBox.Checked)

                packageData.SetValue(basicData, 0)
                Dim someError As String = sqlAccess.editPackage(packageData)
                If someError <> "" Then
                    Dim oldPackId As Integer
                    If Integer.TryParse(someError, oldPackId) Then
                        Dim newPackId As Integer = sqlAccess.getPackageIds(Session("selPackName"), Session("selSailDate"))

                        IO.Directory.Move(Server.MapPath("Images/Temp"), Server.MapPath("Images/Uploads/" & newPackId))
                        IO.Directory.Delete(Server.MapPath("Images/Uploads/" & oldPackId), True)

                        Session("packageModified") = "Y"
                    Else
                        errorStr = someError
                    End If
                End If
            End If
        End If

        If errorStr = "" Then
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'Pacote alterado com êxito.');", True)
        Else
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" & Replace(errorStr, Environment.NewLine, " ") & "');", True)
        End If
        loadPanel.Visible = False
    End Sub
End Class