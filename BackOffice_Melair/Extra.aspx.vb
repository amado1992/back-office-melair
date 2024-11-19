Public Class Extra
    Inherits System.Web.UI.Page

    Dim sqlAccess As New SQL

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        'If Session("login") <> "Y" Then
        '    Response.Redirect("Login.aspx")
        'End If

        Page.MaintainScrollPositionOnPostBack = True

        If Not IsPostBack Then
            initGrid()
        End If
    End Sub

    Private Sub initGrid()
        Dim dt As New DataTable
        Dim dr As DataRow

        dt.Columns.Add(New DataColumn("Código do vôo"))
        dt.Columns.Add(New DataColumn("Día"))
        dt.Columns.Add(New DataColumn("Descrição"))
        dt.Columns.Add(New DataColumn("Posição"))

        dr = dt.NewRow()
        dt.Rows.Add(dr)

        Session("currentDt") = dt
        fillGrid(dt)
    End Sub

    Private Sub fillGrid(dt As DataTable)
        gridView.DataSource = dt
        gridView.DataBind()
    End Sub

    Protected Sub addRow(sender As Object, e As EventArgs) Handles lineBtn.Click
        Dim currentDt As DataTable = CType(Session("currentDt"), DataTable)

        If rowIsFilled() Then
            If Not currentDt Is Nothing Then
                Dim dtRow As DataRow = Nothing

                If currentDt.Rows.Count > 0 Then
                    Dim code, day, desc, pos As String

                    For rowIndex As Integer = 0 To currentDt.Rows.Count - 1
                        Dim codeSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(0).FindControl("codeSelector"), DropDownList)
                        Dim dayField As TextBox = CType(gridView.Rows(rowIndex).Cells(1).FindControl("dayField"), TextBox)
                        Dim descField As TextBox = CType(gridView.Rows(rowIndex).Cells(2).FindControl("descField"), TextBox)
                        Dim typeSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(3).FindControl("typeSelector"), DropDownList)

                        dtRow = currentDt.NewRow()

                        code = codeSelector.SelectedItem.Text
                        day = Trim(dayField.Text)
                        desc = Trim(descField.Text)
                        pos = typeSelector.SelectedItem.Text

                        ' Recolección de valores en las filas existentes hasta el momento
                        currentDt.Rows(rowIndex)("Código do vôo") = code
                        currentDt.Rows(rowIndex)("Día") = day
                        currentDt.Rows(rowIndex)("Descrição") = desc
                        currentDt.Rows(rowIndex)("Posição") = pos
                    Next

                    ' Recreamos el objeto DropDownList que sirve de selector para la posición de la línea en el itinerario
                    Dim newTypeSel As DropDownList = createCustomSelector("typeSelector")
                    dtRow("Posição") = newTypeSel.SelectedItem.Text
                    currentDt.Rows.Add(dtRow) ' Adición de la nueva fila vacía

                    Session("currentDt") = currentDt
                    gridView.DataSource = currentDt
                    gridView.DataBind()
                Else
                    initGrid()
                    Exit Sub
                End If
            Else
                msgLbl.Text = "Houve um erro na memória."
                'popup.Visible = True
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup();", True)
            End If

            setPreviousData()
        End If
    End Sub

    Protected Sub deleteRow(sender As Object, e As GridViewDeleteEventArgs)
        Dim rowIndex As Integer = e.RowIndex

        Dim currentDt As DataTable = CType(Session("currentDt"), DataTable)
        currentDt.Rows.RemoveAt(rowIndex)

        Session("currentDt") = currentDt
        fillGrid(currentDt)

        setPreviousData()
    End Sub

    Private Sub setPreviousData()
        Dim dt As DataTable = CType(Session("currentDt"), DataTable)

        If Not dt Is Nothing Then
            If dt.Rows.Count > 0 Then
                Dim code, pos As String

                For rowIndex As Integer = 0 To dt.Rows.Count - 1
                    Dim codeSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(0).FindControl("codeSelector"), DropDownList)
                    Dim dayField As TextBox = CType(gridView.Rows(rowIndex).Cells(1).FindControl("dayField"), TextBox)
                    Dim descField As TextBox = CType(gridView.Rows(rowIndex).Cells(2).FindControl("descField"), TextBox)
                    Dim typeSelector As DropDownList = CType(gridView.Rows(rowIndex).Cells(3).FindControl("typeSelector"), DropDownList)

                    If rowIndex <> dt.Rows.Count - 1 Then
                        code = dt.Rows(rowIndex)("Código do vôo").ToString()
                        codeSelector.ClearSelection()
                        codeSelector.Items.FindByText(code).Selected = True
                    End If
                    dayField.Text = dt.Rows(rowIndex)("Día").ToString()
                    descField.Text = dt.Rows(rowIndex)("Descrição").ToString()
                    pos = dt.Rows(rowIndex)("Posição").ToString()
                    typeSelector.ClearSelection()
                    typeSelector.Items.FindByText(pos).Selected = True
                Next
            End If
        End If
    End Sub

    Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
        'popup.Visible = False
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "closePopup", "$('popup').remove();", True)

        If Session("packageRegistered") = "Y" Then
            Session("packageRegistered") = ""
            Response.Redirect("Packages.aspx")
        End If
    End Sub

    Protected Sub onRowDataBound(sender As Object, e As GridViewRowEventArgs)
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim num As Integer = e.Row.RowIndex
            Dim codeSelector As DropDownList = CType(e.Row.FindControl("codeSelector"), DropDownList)
            codeSelector.Items.Clear()

            codeSelector.DataSource = sqlAccess.getPackFlightCodes(Session("packName"), Session("sailDate"))
            'codeSelector.DataSource = sqlAccess.getPackFlightCodes("Norte de Europa maravilloso", "04/02/2017")
            codeSelector.DataTextField = "FlightCode"
            codeSelector.DataBind()
            'TODO Mirar el caso en el que codeSelector no tiene items.
            codeSelector.Items(0).Selected = True
        End If
    End Sub

    Private Function rowIsFilled(Optional rowIndex As Integer = -1) As Boolean
        Dim result As Boolean = True

        Dim dayField, descField As TextBox

        If rowIndex <> -1 Then
            dayField = CType(gridView.Rows(rowIndex).Cells(1).FindControl("dayField"), TextBox)
            descField = CType(gridView.Rows(rowIndex).Cells(2).FindControl("descField"), TextBox)

            If (Trim(dayField.Text) = "") Or (Trim(descField.Text) = "") Then
                result = False
                msgLbl.Text = "Por favor, preencha as informações em falta na linha " & (rowIndex + 1) & " antes de prosseguir."
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup();", True)
            End If
        Else
            For i As Integer = 0 To gridView.Rows.Count - 1
                dayField = CType(gridView.Rows(i).Cells(1).FindControl("dayField"), TextBox)
                descField = CType(gridView.Rows(i).Cells(2).FindControl("descField"), TextBox)

                If (Trim(dayField.Text) = "") Or (Trim(descField.Text) = "") Then
                    result = False
                    msgLbl.Text = "Por favor, preencha as informações em falta na tabela antes de prosseguir."
                    scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup();", True)
                End If
            Next
        End If

        rowIsFilled = result
    End Function

    Private Function createCustomSelector(name As String) As DropDownList
        Dim result As New DropDownList

        If name = "typeSelector" Then
            result.Items.Add(New ListItem("Antes", "A"))
            result.Items.Add(New ListItem("Depois", "D"))
        End If
        result.Items(0).Selected = True
        result.ID = name

        createCustomSelector = result
    End Function

    Protected Sub skipStep(sender As Object, e As EventArgs) Handles skipBtn.Click
        Response.Redirect("Packages.aspx")
    End Sub

    Protected Sub finish(sender As Object, e As EventArgs) Handles finishBtn.Click
        If rowIsFilled() Then
            loadPanel.Visible = True

            Dim linesData As New Dictionary(Of Integer, Dictionary(Of String, String))
            Dim lineData As Dictionary(Of String, String)

            Dim codeSelector, typeSelector As DropDownList
            Dim dayField, descField As TextBox

            For rowIndex As Integer = 0 To gridView.Rows.Count - 1
                codeSelector = CType(gridView.Rows(rowIndex).Cells(0).FindControl("codeSelector"), DropDownList)
                dayField = CType(gridView.Rows(rowIndex).Cells(1).FindControl("dayField"), TextBox)
                descField = CType(gridView.Rows(rowIndex).Cells(2).FindControl("descField"), TextBox)
                typeSelector = CType(gridView.Rows(rowIndex).Cells(3).FindControl("typeSelector"), DropDownList)

                lineData = New Dictionary(Of String, String)
                lineData.Add("Code", codeSelector.SelectedItem.Text)
                lineData.Add("Day", Trim(dayField.Text))
                lineData.Add("Description", Trim(descField.Text))
                lineData.Add("Position", typeSelector.SelectedItem.Value)
                linesData.Add(rowIndex, lineData)
            Next

            Dim someError As String = sqlAccess.insertItineraryLines(Session("packName"), linesData)
            If someError = "" Then
                Session("packageRegistered") = "Y"
                msgLbl.Text = "Todas as informações sobre o pacote foi registrado com êxito."
            Else
                msgLbl.Text = someError
            End If

            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup();", True)
            loadPanel.Visible = False
        End If
    End Sub

End Class