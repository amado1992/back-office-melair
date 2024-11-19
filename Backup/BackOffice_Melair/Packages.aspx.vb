Public Class Packages
    Inherits System.Web.UI.Page

    Dim sqlAccess As New SQL

    Protected Sub Page_Load(sender As Object, e As System.EventArgs) Handles Me.Load
    If Session("login") <> "Y" Then
      Response.Redirect("Login.aspx")
    End If

        Dim deDup As Byte
        deDup = Request.QueryString("Dup")
        If deDup = 1 And Session("DupMsg") = 1 Then
            Session("DupMsg") = 0
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "show", "showPopup('Informação', 'O pacote foi duplicado corretamente.');", True)
        End If


        If Not IsPostBack Then
            Session("packId") = ""
            Session("name") = ""
            Session("shipCode") = ""
            Session("sailDate") = ""
            Session("packageDeleted") = ""

            Dim dbData As DataTable = sqlAccess.retrievePackages()
            If (Not dbData Is Nothing) And (dbData.Rows.Count > 0) Then
                fillGrid(dbData)
                Session("dbData") = dbData
            End If
        End If
    End Sub

    Protected Sub addPackage(sender As Object, e As EventArgs) Handles openBtn.Click
        Response.Redirect("Register.aspx")
    End Sub

    Protected Sub logout(sender As Object, e As EventArgs) Handles logoutBtn.Click
        Session.Clear()

        Response.Redirect("Login.aspx")
    End Sub

    Private Sub fillGrid(dbData As DataTable)
        gridView.DataSource = dbData
        gridView.DataBind()
    End Sub

    Protected Sub onRowDataBound(sender As Object, e As GridViewRowEventArgs) Handles gridView.RowDataBound
        If e.Row.RowType = DataControlRowType.DataRow Then
            Dim nameLbl As Label = CType(e.Row.Cells(0).FindControl("nameLbl"), Label)
            Dim packName As String = nameLbl.Text

            Dim dateSelector As DropDownList = CType(e.Row.Cells(1).FindControl("dateSelector"), DropDownList)
            dateSelector.DataSource = sqlAccess.retrievePackageDates(packName)
            dateSelector.DataTextField = "SailingDate"
            dateSelector.DataValueField = "idPack"
            dateSelector.DataBind()
            dateSelector.Items.Insert(0, New ListItem("Tudo"))
            dateSelector.Items(0).Selected = True
        End If
    End Sub

    Protected Sub onPageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles gridView.PageIndexChanging
        gridView.PageIndex = e.NewPageIndex
        Me.fillGrid(Session("dbData"))
    End Sub

    Protected Sub editPackage(sender As Object, e As EventArgs)
        Dim packName As String = sender.CommandArgument.ToString()
        Dim editLink As LinkButton = CType(sender, LinkButton)
        Dim gridRow As GridViewRow = CType(editLink.NamingContainer, GridViewRow)
        Dim dateSelector As DropDownList = CType(gridRow.Cells(1).FindControl("dateSelector"), DropDownList)

        If dateSelector.SelectedIndex <> 0 Then
            Session("selPackName") = packName
            Session("selSailDate") = dateSelector.Items(dateSelector.SelectedIndex).Text

            Response.Redirect("EditPackage.aspx")
        Else
            Session("selPackName") = packName

            Response.Redirect("EditMultiplePackages.aspx")
            'scriptManager.RegisterStartupScript(Me, Me.GetType(), "show", "showPopup('Error', 'Você deve ter selecionado uma data específica para este pacote. Por favor, seleccione um e tente novamente.');", True)
        End If
    End Sub


    Protected Sub duplicarPackage(sender As Object, e As EventArgs)
        Dim packName As String = sender.CommandArgument.ToString()
        Dim editLink As LinkButton = CType(sender, LinkButton)
        Dim gridRow As GridViewRow = CType(editLink.NamingContainer, GridViewRow)
        Dim dateSelector As DropDownList = CType(gridRow.Cells(1).FindControl("dateSelector"), DropDownList)
        Dim idPack As Integer = 0

        If dateSelector.SelectedIndex <> 0 Then
            Session("selPackName") = packName
            Session("selSailDate") = dateSelector.Items(dateSelector.SelectedIndex).Text
            idPack = dateSelector.Items(dateSelector.SelectedIndex).Value
            Response.Redirect("DuplicarPackage.aspx?idPack=" & idPack)
        Else
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "show", "showPopup('Error', 'Você deve ter selecionado uma data específica para duplicar este pacote. Por favor, seleccione um e tente novamente.');", True)
        End If
    End Sub



    Protected Sub closePopup(sender As Object, e As EventArgs) Handles msgBtn.Click
        If Session("packageDeleted") = "Y" Then
            Response.Redirect("Packages.aspx")
        End If
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "hide", "$('#popup').modal('hide');", True)
    End Sub

    Protected Sub confirmDelete(sender As Object, e As EventArgs)
        Dim packName As String = sender.CommandArgument.ToString()
        Dim delLink As LinkButton = CType(sender, LinkButton)
        Dim gridRow As GridViewRow = CType(delLink.NamingContainer, GridViewRow)
        Dim dateSelector As DropDownList = CType(gridRow.Cells(1).FindControl("dateSelector"), DropDownList)

        Session("selPackName") = packName
        Session("selSailDate") = ""

        If dateSelector.SelectedIndex = 0 Then
            confirmLbl.Text = "Esta acção fará com que o apagamento completo do pacote para todas as datas associadas ... você continuar com a exclusão?"
        Else
            Session("selSailDate") = dateSelector.Items(dateSelector.SelectedIndex).Text
            confirmLbl.Text = "Esta acção irá causar a exclusão do pacote para a data selecionada ... você procederia?"
        End If
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showConfirm", "showConfirm();", True)
    End Sub

  Protected Sub yesClick(sender As Object, e As EventArgs) Handles yesBtn.Click
    Dim deletedIds As String = ""

    Try
      If Session("selSailDate") = "" Then ' Eliminación del paquete para todas sus fechas
        deletedIds = sqlAccess.deletePackage(Session("selPackName"))
      Else ' Eliminación del paquete para una fecha concreta
        deletedIds = sqlAccess.deletePackage(Session("selPackName"), Session("selSailDate"))
      End If
      Session("selPackName") = Nothing
      Session("selSailDate") = Nothing
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "hideConfirm", "$('#confirm').modal('hide');", True)
      If deletedIds <> "" Then
        Dim packIds() As String = deletedIds.Split("|")
        For Each packId In packIds
          If packId <> "" Then
            IO.File.Delete(Server.MapPath("Images/Uploads/" & packId))
          End If
        Next

        Session("packageDeleted") = "Y"
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Informação', 'O pacote foi removido com sucesso.');", True)
      Else
        scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', 'Houve um erro durante a remoção do pacote. Por favor, tente novamente.');", True)
      End If
    Catch ex As Exception
      scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup('Error', '" _
                                          & Replace(ex.Message, Environment.NewLine, " ") & "');", True)
    End Try
    
  End Sub

End Class