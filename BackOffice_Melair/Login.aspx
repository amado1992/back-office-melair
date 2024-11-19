<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Login.aspx.vb" Inherits="BackOffice_Melair.Login1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Inicio de sesión | Back Office MELAIR</title>
    <link rel="Stylesheet" href="Styles/style.css" />
    <link rel="Stylesheet" href="Styles/bootstrap.min.css" />
    <script type="text/javascript" src="Scripts/jquery-3.1.0.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript">
        function showPopup() {
            $('#popup').modal('show');
        }
    </script>
</head>
<body>
    <div class="header">
        <div id="logo">
            <img src="royal_logo.png" alt="Royal Caribbean Logo" /> <h1>Gerente de Pacotes</h1>
        </div>  
    </div>

    <div class="modal fade" id="popup" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
                      <div class="modal-dialog" role="document">
                        <div class="modal-content">
                          <div class="modal-header">
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                            <h4 class="modal-title" id="myModalLabel">Error</h4>
                          </div>
                          <div class="modal-body">
                            <asp:Label ID="msgLbl" runat="server" Text="" />
                          </div>
                          <div class="modal-footer">
                            <button type="button" class="btn btn-default" data-dismiss="modal">Aceitar</button>
                          </div>
                        </div>
                      </div>
    </div>

    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="true" />

        <div class="loginDiv">
            <div class="loginFields">
                <div class="row" style="margin-bottom: 15px;">
                    <asp:Label ID="Label1" runat="server">Usuário: </asp:Label> <asp:TextBox ID="userField" runat="server" class="form-control"></asp:TextBox>
                </div>
                <div class="row" style="margin-bottom: 35px;">
                    <asp:Label ID="Label2" runat="server">Senha: </asp:Label> <asp:TextBox ID="passField" TextMode="Password" runat="server" class="form-control"></asp:TextBox>
                </div>
            </div>

            <div class="buttonDiv">
                <asp:Button ID="loginBtn" runat="server" OnClick="login" Text="Iniciar sessão" CssClass="btn btn-default" />
            </div> 
        </div>
    </form>
</body>
</html>