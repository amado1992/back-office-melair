Public Class test
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
    ScriptManager.RegisterStartupScript(Me, Me.GetType(), "showImages", "showImagesPopup('Hello World! ');", True)

    End Sub

End Class