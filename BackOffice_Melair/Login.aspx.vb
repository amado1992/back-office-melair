Public Class Login1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

    End Sub

    Protected Sub login(ByVal sender As Object, ByVal e As EventArgs) Handles loginBtn.Click
        Dim user As String = userField.Text
        Dim pass As String = passField.Text

        If user <> "" And pass <> "" Then
            If user = "Melair" And pass = "MEL001" Then
                Session("login") = "Y"

                Response.Redirect("Packages.aspx")
            Else
                msgLbl.Text = "Os dados inserida está incorreta. Por favor, tente novamente."
                scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup();", True)
            End If
        Else
            msgLbl.Text = "Por favor, preencha ambos os campos antes de pressionar o botão."
            scriptManager.RegisterStartupScript(Me, Me.GetType(), "showPopup", "showPopup();", True)
        End If
    End Sub
End Class