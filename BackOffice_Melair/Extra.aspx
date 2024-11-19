<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Extra.aspx.vb" Inherits="BackOffice_Melair.Extra" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Extra | Back Office MELAIR</title>
    <link rel="Stylesheet" type="text/css" href="Styles/style.css" />
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
            <img src="royal_logo.png" alt="Royal Caribbean International" /><h1>Gerente de Pacotes</h1>
        </div>
    </div>
    
    <form id="form1" runat="server">
        <asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="true" />

        <div class="modal fade" id="popup" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Error</h4>
                  </div>
                  <div class="modal-body">
                    <asp:Label ID="msgLbl" runat="server" Text="" />
                  </div>
                  <div class="modal-footer">
                    <asp:Button ID="msgBtn" runat="server" Text="Aceitar" OnClick="closePopup" CssClass="btn btn-default" />
                  </div>
                </div>
              </div>
            </div>  

        <asp:Panel ID="loadPanel" runat="server" CssClass="fadeClass" Visible="false">
                <div class="popupClass">
                    <p>Cargando...</p>
                </div>
            </asp:Panel>
        <div class="linesDiv">
            <div id="linesGridDiv">
                <asp:GridView ID="gridView" runat="server" AutoGenerateColumns="false" OnRowDataBound="onRowDataBound" OnRowDeleting="deleteRow"
                    CssClass="flightsGrid" AlternatingRowStyle-BackColor="#CCE6FF">
                    <Columns>
                        <asp:TemplateField HeaderText="Código do vôo">
                            <ItemTemplate>
                                <asp:DropDownList ID="codeSelector" runat="server">
                                    
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Día">
                            <ItemTemplate>
                                <asp:TextBox ID="dayField" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:TextBox ID="descField" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Posição">
                            <ItemTemplate>
                                <asp:DropDownList ID="typeSelector" runat="server">
                                    <asp:ListItem Text="Antes" Value="B" Selected="True" />
                                    <asp:ListItem Text="Depois" Value="A" />
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowDeleteButton="true" ButtonType="Button" DeleteText="Remover" ControlStyle-BackColor="#B7CBCF" ControlStyle-CssClass="btn btn-default" />
                    </Columns>
                </asp:GridView>

                <asp:Button ID="lineBtn" runat="server" Text="Adicionar linha" OnClick="addRow" CssClass="btn btn-default" />
            </div>

            <div id="buttonsDiv">
                <asp:Button ID="skipBtn" runat="server" OnClick="skipStep" Text="Pular" CssClass="btn btn-default" />
                <asp:Button ID="finishBtn" runat="server" OnClick="finish" Text="Finalizar" CssClass="btn btn-default" />
            </div> 
        </div>
    </form>
</body>
</html>
