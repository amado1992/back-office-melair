Public Class Cabin
    Inherits System.Web.UI.Page
    Dim sqlAccess As New SQL
    Dim paramIdPack As String = 0
    Dim paramIdCabinLine As String = 0
    Dim listOrigins As New List(Of Integer)
    Dim showModal As Boolean = False

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If (Session IsNot Nothing) Then
            paramIdCabinLine = Session("IdCabinLine")
        End If

        If (Session IsNot Nothing) Then
            paramIdPack = Session("IdPack")
        End If

        origins()

        'Número de controles a crear
        Dim numControles As Integer = 5
        lenghtHiddenField.Value = 15

        'Crear y agregar los controles dinámicos
        For i As Integer = 0 To numControles - 1

            Dim txt As New TextBox()
            txt.Attributes.Add("style", "margin-bottom: 10px")
            txt.ID = "txt" + (i + 1).ToString()
            txt.Text = ""

            phDinamicControls.Controls.Add(txt)
            phDinamicControls.Controls.Add(New LiteralControl("<br/>"))
        Next

        If Not IsPostBack Then

            Dim idCabinLine = 0
            ' Add items to the DropDownList control
            cabinType.Items.Add(New ListItem("Interior", "I"))
            cabinType.Items.Add(New ListItem("Exterior", "O"))
            cabinType.Items.Add(New ListItem("Balcón", "B"))
            cabinType.Items.Add(New ListItem("Suite", "S"))


            Dim dbData As DataTable = sqlAccess.retrieveCabins(paramIdPack)
            If (Not dbData Is Nothing) And (dbData.Rows.Count > 0) Then
                GridView1.DataSource = dbData
                GridView1.DataBind()
                GridView1.Columns(0).Visible = False
                GridView1.Columns(1).Visible = False
                GridView1.Columns(2).Visible = False
            End If

        End If



    End Sub

    Protected Sub OnRowEditing(sender As Object, e As GridViewEditEventArgs)

        GridView1.EditIndex = e.NewEditIndex
        Me.BindGrid()
        'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hide", "$('#modalPopUp').modal('hide');", True)
    End Sub

    Private Sub BindGrid()
        Dim dbData As DataTable = sqlAccess.retrieveCabins(paramIdPack)
        If (Not dbData Is Nothing) And (dbData.Rows.Count > 0) Then
            GridView1.DataSource = dbData
            GridView1.DataBind()
            GridView1.Columns(0).Visible = False
            GridView1.Columns(1).Visible = False
            GridView1.Columns(2).Visible = False
        End If
    End Sub


    Protected Sub OnRowCancelingEdit(sender As Object, e As EventArgs)
        'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hide", "$('#modalPopUp').modal('hide');", True)
        GridView1.EditIndex = -1
        Me.BindGrid()
    End Sub


    Protected Sub OnRowDeleting(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)
        Dim idCabinLine As Integer = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Session("IdCabinLineDelete") = idCabinLine
        confirmLabel.Text = "¿Está seguro que desea elimanar este camarote?"

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showConfirm", "showConfirm();", True)

    End Sub


    Protected Sub OnRowUpdating(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)

        Dim packageData(0) As Object
        Dim basicData As New Dictionary(Of String, String)

        Dim row As GridViewRow = GridView1.Rows(e.RowIndex)

        Dim idCabinLine = Convert.ToInt32(GridView1.DataKeys(e.RowIndex).Values(0))
        Dim idPack = (TryCast(row.FindControl("idPackLabel"), Label)).Text
        Dim cabinTypeEdit = (TryCast(row.FindControl("cabinTypeEditLabel"), Label)).Text

        Dim cabinType = (TryCast(row.FindControl("cabinType"), DropDownList)).Text
        Dim category = (TryCast(row.FindControl("category"), TextBox)).Text
        Dim price = (TryCast(row.FindControl("price"), TextBox)).Text
        Dim singleField = (TryCast(row.FindControl("singleField"), TextBox)).Text
        Dim triple = (TryCast(row.FindControl("triple"), TextBox)).Text
        Dim quadruple = (TryCast(row.FindControl("quadruple"), TextBox)).Text
        Dim promoTitle = (TryCast(row.FindControl("promoTitle"), TextBox)).Text
        Dim promoDescription = (TryCast(row.FindControl("promoDescription"), TextBox)).Text

        If (cabinType = "None") Then
            cabinType = cabinTypeEdit
        End If

        basicData.Add("IdCabinLine", Integer.Parse(idCabinLine))
        basicData.Add("IdPack", Integer.Parse(idPack))
        basicData.Add("CabinType", cabinType)
        basicData.Add("Category", category)
        basicData.Add("Price", Single.Parse(price))
        basicData.Add("Singl", Single.Parse(singleField))
        basicData.Add("Triple", Single.Parse(triple))
        basicData.Add("Quadruple", Single.Parse(quadruple))
        basicData.Add("PromoTitle", promoTitle)
        basicData.Add("PromoDescription", promoDescription)

        packageData.SetValue(basicData, 0)

        sqlAccess.updateCabin(packageData)
        Me.BindGrid()
    End Sub

    Public Sub insert()
        Dim _idPack, _cabinType, _category, _price, _single, _triple, _quadruple, _promoTitle, _promoDescription
        Dim packageData(0) As Object
        Dim basicData As New Dictionary(Of String, String)

        _idPack = paramIdPack
        '_idPack = 983
        _cabinType = cabinType.Text
        _category = category.Text
        _price = price.Text
        _single = singleField.Text
        _triple = triple.Text
        _quadruple = quadruple.Text
        _promoTitle = promoTitle.Text
        _promoDescription = promoDescription.Text

        basicData.Add("IdPack", _idPack)
        basicData.Add("CabinType", _cabinType)
        basicData.Add("Category", _category)
        basicData.Add("Price", _price)
        basicData.Add("Singl", _single)
        basicData.Add("Triple", _triple)
        basicData.Add("Quadruple", _quadruple)
        basicData.Add("PromoTitle", _promoTitle)
        basicData.Add("PromoDescription", _promoDescription)

        packageData.SetValue(basicData, 0)

        sqlAccess.insertCabin(packageData)

        _idPack = String.Empty
        cabinType.Text = -1
        category.Text = String.Empty
        price.Text = String.Empty
        singleField.Text = String.Empty
        triple.Text = String.Empty
        quadruple.Text = String.Empty
        promoTitle.Text = String.Empty
        promoDescription.Text = String.Empty

        Me.BindGrid()
        'UpdatePanel3.Update()
    End Sub

    Protected Sub addPackage(sender As Object, e As EventArgs) Handles btBack.Click
        Response.Redirect("Register.aspx")
    End Sub
    Protected Sub addLine(sender As Object, e As EventArgs) Handles btNext.Click
        Response.Redirect("Lines.aspx")
    End Sub

    Public Sub insertQuotaOld()
        Dim idPack, idOrigin, idCabinLine, quota
        Dim packageData(0) As Object
        Dim basicData As New Dictionary(Of String, String)

        paramIdCabinLine = Session("IdCabinLine")
        idPack = paramIdPack
        'idPack = 983
        'idOrigin = DropDownOrigins.Text
        idCabinLine = paramIdCabinLine
        'quota = quotaTextBox.Text

        basicData.Add("IdPack", idPack)
        basicData.Add("IdOrigin", idOrigin)
        basicData.Add("IdCabinLine", idCabinLine)
        basicData.Add("Quota", quota)

        packageData.SetValue(basicData, 0)

        sqlAccess.insertQuota(packageData)

        idPack = String.Empty
        'DropDownOrigins.Text = -1
        'quotaTextBox.Text = String.Empty
    End Sub

    Public Sub insertQuota(idOrigin As Integer, quota As Integer)
        Dim idPack, idCabinLine
        Dim packageData(0) As Object
        Dim basicData As New Dictionary(Of String, String)

        paramIdCabinLine = Session("IdCabinLine")
        idPack = paramIdPack
        'idPack = 983
        idCabinLine = paramIdCabinLine
        basicData.Add("IdPack", idPack)
        basicData.Add("IdOrigin", idOrigin)
        basicData.Add("IdCabinLine", idCabinLine)
        basicData.Add("Quota", quota)

        packageData.SetValue(basicData, 0)

        sqlAccess.insertQuota(packageData)

        retrieveQuotas(paramIdPack, idCabinLine)
        idPack = String.Empty
    End Sub

    Private Sub origins()
        Dim dbData As DataTable = sqlAccess.retrieveOrigins()
        If (Not dbData Is Nothing) And (dbData.Rows.Count > 0) Then
            GridViewQt.DataSource = dbData
            GridViewQt.DataBind()
            'GridViewQt.Columns(0).Visible = False
            For Each row As DataRow In dbData.Rows
                'Dim listItem As New ListItem()
                'listItem.Value = row("idOrigin").ToString() ' Cambia "Id" por el nombre de la columna que deseas utilizar como valor
                'listItem.Text = row("name").ToString() ' Cambia "Nombre" por el nombre de la columna que deseas utilizar como texto
                'DropDownOrigins.Items.Add(listItem)

                'listOrigins = New List(Of Integer)
                listOrigins.Add(row("idOrigin"))
            Next
        End If
    End Sub

    Protected Sub myGridView_RowCommand2(sender As Object, e As GridViewCommandEventArgs) Handles GridView1.RowCommand

        showModal = True
        'ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
        If e.CommandName = "myCommand" Then
            ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)

            Dim rowIndex As Integer = Convert.ToInt32(e.CommandArgument)
            Dim row As GridViewRow = GridView1.Rows(rowIndex)
            Dim idCabinLine As String = (TryCast(row.FindControl("idCabinLineLabel"), Label)).Text
            Dim idPack As String = (TryCast(row.FindControl("idPackLabel"), Label)).Text

            Session("IdCabinLine") = idCabinLine
            Session("IdPack") = idPack
            paramIdPack = idPack

            retrieveQuotas(paramIdPack, idCabinLine)
            'retrieveQuotas(30989, 30005)
        End If
        UpdatePanel3.Update()
    End Sub

    'Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
    'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hide", "$('#modalPopUp').modal('hide');", True)
    'insertQuota()
    'End Sub


    Protected Sub OnPaging(ByVal sender As Object, ByVal e As GridViewPageEventArgs)
        'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hide", "$('#modalPopUp').modal('hide');", True)
        GridView1.PageIndex = e.NewPageIndex
        Me.BindGrid()
    End Sub

    Protected Sub btnEnviar_Click(ByVal sender As Object, ByVal e As EventArgs) Handles btnEnviar.Click
        Dim origins As New List(Of Integer)
        Dim stringJoin = myValueHiddenField.Value
        Dim arrayQuotas()
        Dim intQuota As Integer
        arrayQuotas = Split(stringJoin, "|")
        origins = listOrigins

        For i = 0 To origins.Count - 1
            Dim quota = arrayQuotas(i)
            Dim origin = origins(i)

            If Integer.TryParse(quota, intQuota) Then
                insertQuota(origin, intQuota)
            Else
                'La conversión falló, intNumero es igual a cero
                insertQuota(origin, intQuota)
            End If
        Next
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
    End Sub

    Protected Sub GridView1_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs)
        'If e.Row.RowType = DataControlRowType.DataRow Then
        'Dim bf As BoundField = DirectCast(GridView1.Columns(1), BoundField)
        'Dim bf As TemplateField = DirectCast(GridView1.Columns(1), TemplateField)
        'Dim cell As TableCell = e.Row.Cells(GridView1.Columns.IndexOf(bf))

        If e.Row.RowType = DataControlRowType.DataRow AndAlso GridView1.EditIndex <> e.Row.RowIndex Then
            Dim lbl As Label = DirectCast(e.Row.FindControl("cabinTypeName"), Label)

            Dim lblpriceName As Label = DirectCast(e.Row.FindControl("priceName"), Label)
            Dim lblsingleName As Label = DirectCast(e.Row.FindControl("singleName"), Label)
            Dim lbltripleName As Label = DirectCast(e.Row.FindControl("tripleName"), Label)
            Dim lblcuadrupleName As Label = DirectCast(e.Row.FindControl("cuadrupleName"), Label)
            ' Aquí puedes comprobar una condición y cambiar el valor de la etiqueta en consecuencia

            If (lblpriceName.Text = "" Or lblpriceName.Text = 0) Then
                lblpriceName.Text = ""
            Else
                lblpriceName.Text = lblpriceName.Text + " por pessoa"
            End If


            If (lblsingleName.Text = "" Or lblsingleName.Text = 0) Then
                lblsingleName.Text = ""
            Else
                lblsingleName.Text = lblsingleName.Text + " por pessoa"
            End If


            If (lbltripleName.Text = "" Or lbltripleName.Text = 0) Then
                lbltripleName.Text = ""
            Else
                lbltripleName.Text = lbltripleName.Text + " por pessoa"
            End If

            If (lblcuadrupleName.Text = "" Or lblcuadrupleName.Text = 0) Then
                lblcuadrupleName.Text = ""
            Else
                lblcuadrupleName.Text = lblcuadrupleName.Text + " por pessoa"
            End If


            If lbl.Text = "I" Then
                    lbl.Text = "Interior"
                End If

                If lbl.Text = "O" Then
                    lbl.Text = "Exterior"
                End If

                If lbl.Text = "B" Then
                    lbl.Text = "Balcón"
                End If

                If lbl.Text = "S" Then
                    lbl.Text = "Suite"
                End If
            End If

        ' Aquí puedes comprobar una condición y cambiar el contenido de la celda en consecuencia


        'End If
    End Sub

    Public Sub retrieveQuotas(idPack, idCabinLine)
        Dim dbData As DataTable = sqlAccess.retrieveQuotas(idPack, idCabinLine)
        If (Not dbData Is Nothing) And (dbData.Rows.Count > 0) Then
            GridViewQuotas.DataSource = dbData
            GridViewQuotas.DataBind()
        Else
            GridViewQuotas.DataSource = dbData
            GridViewQuotas.DataBind()
        End If
    End Sub

    Protected Sub OnRowEditingQuotas(sender As Object, e As GridViewEditEventArgs)
        GridViewQuotas.EditIndex = e.NewEditIndex
        Dim idCabinLine = Session("IdCabinLine")
        retrieveQuotas(paramIdPack, idCabinLine)
        'retrieveQuotas(30989, 30005)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
    End Sub

    Protected Sub OnRowCancelingEditQuotas(sender As Object, e As EventArgs)
        GridViewQuotas.EditIndex = -1
        Dim idCabinLine = Session("IdCabinLine")
        retrieveQuotas(paramIdPack, idCabinLine)
        'retrieveQuotas(30989, 30005)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
    End Sub

    Protected Sub OnRowUpdatingQuotas(ByVal sender As Object, ByVal e As GridViewUpdateEventArgs)

        Dim packageData(0) As Object
        Dim basicData As New Dictionary(Of String, String)

        Dim row As GridViewRow = GridViewQuotas.Rows(e.RowIndex)
        Dim idCabinLine = Session("IdCabinLine")
        Dim quota = (TryCast(row.FindControl("quotaListTexBox"), TextBox)).Text
        Dim idOrigin = Convert.ToInt32(GridViewQuotas.DataKeys(e.RowIndex).Values(0))

        paramIdCabinLine = Session("IdCabinLine")
        Dim idPack = paramIdPack
        basicData.Add("IdPack", idPack)
        basicData.Add("IdOrigin", idOrigin)
        basicData.Add("IdCabinLine", idCabinLine)
        basicData.Add("Quota", quota)

        packageData.SetValue(basicData, 0)

        sqlAccess.updateQuota(packageData)
        retrieveQuotas(paramIdPack, idCabinLine)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
    End Sub

    Protected Sub OnRowDeletingQuotas(ByVal sender As Object, ByVal e As GridViewDeleteEventArgs)

        Dim idOrgin As Integer = Convert.ToInt32(GridViewQuotas.DataKeys(e.RowIndex).Values(0))
        Session("IdOrigin") = idOrgin
        confirmLabelQt.Text = "¿Está seguro que desea elimanar este camarote?"
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showConfirmQt", "showConfirmQt();", True)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
        'Dim idCabinLine = Session("IdCabinLine")
        'retrieveQuotas(paramIdPack, idCabinLine)
        'retrieveQuotas(30989, 30005)
        'ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)
    End Sub

    Protected Sub okClick(sender As Object, e As EventArgs) Handles okBtn.Click
        Dim idCabinLine = Session("IdCabinLineDelete")
        sqlAccess.deleteCabin(idCabinLine)
        Me.BindGrid()
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirmModal').modal('hide');", True)

    End Sub

    Protected Sub okClickQt(sender As Object, e As EventArgs) Handles okBtnQt.Click
        Dim idCabinLine = Session("IdCabinLine")
        Dim idOrigin = Session("IdOrigin")
        Dim idPack = Session("IdPack")
        sqlAccess.deleteQuota(idCabinLine, idOrigin, idPack)
        retrieveQuotas(idPack, idCabinLine)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirmModal').modal('hide');", True)
        ScriptManager.RegisterStartupScript(Page, Page.GetType(), "myModal", "$(document).ready(function () {  $('#modalPopUp').modal('show');});", True)

    End Sub

End Class