<%@ Page Language="vb"  AutoEventWireup="false" CodeBehind="Packages.aspx.vb" Inherits="BackOffice_Melair.Packages" %>

<!DOCTYPE html>
 <html>
    <head id="Head1" runat="server">
        <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
        <title>Paquetes | Back Office MELAIR</title>
        <link rel="Stylesheet" href="Styles/style.css" />
        <link rel="Stylesheet" href="Styles/bootstrap.min.css" />
        <script type="text/javascript" src="Scripts/jquery-3.1.0.js"></script>
        <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
        <script type="text/javascript">
            function showPopup(title, message) {
                $('#popup').modal('show');
                if (typeof title != "undefined" && typeof message != "undefined") {
                    document.getElementById('<%= msgHdr.ClientID %>').innerHTML = title;
                    document.getElementById('<%= msgLbl.ClientID %>').innerHTML = message;
                }
            }

            function showConfirm() {
                $('#confirm').modal('show');
            }
        </script>
    </head>
    <body>
        <div class="header">
            <div id="logo">
                <img src="royal_logo.png" alt="Royal Caribbean International" /><h1>Gerente de Pacotes</h1>
            </div>
        </div>

        <form id="tblForm" runat="server">
            <asp:ScriptManager ID="scriptManager" runat="server" EnablePartialRendering="true" />

            <div class="modal fade" id="popup" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">
                        <asp:Label ID="msgHdr" runat="server" Text="" />
                    </h4>
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

            <div class="modal fade" id="confirm" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <h4 class="modal-title">Confirmar</h4>
                  </div>
                  <div class="modal-body">
                    <asp:Label ID="confirmLbl" runat="server" Text="" />
                  </div>
                  <div class="modal-footer">
                    <asp:Button ID="yesBtn" runat="server" Text="Sim" OnClick="yesClick" CssClass="btn btn-default" />
                    <button type="button" data-dismiss="modal" class="btn btn-default">Não</button>
                  </div>
                </div>
              </div>
            </div>

            <div class="mainDiv">

                <div id="packsDiv">
                    <asp:GridView ID="gridView" runat="server" Width="100%"
                AutoGenerateColumns = "false" Font-Names = "Arial" 
                Font-Size = "11pt" AlternatingRowStyle-BackColor = "#99CCFF"  
                AllowPaging ="true" OnPageIndexChanging = "onPageIndexChanging" OnRowDataBound="onRowDataBound"
                PageSize = "8" CssClass="gridStyle">
                    <Columns>
                        <asp:TemplateField HeaderText="Nome" ItemStyle-HorizontalAlign="Center">                    
                            <ItemTemplate>
                                <asp:Label ID="nameLbl" runat="server" Text='<%# Eval("PackageName") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Data de partida" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:DropDownList ID="dateSelector" runat="server" CssClass="dateSelector">
                                    <asp:ListItem Text="Tudo" Selected="True" />
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Navio" ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:Label ID="shipLbl" runat="server" Text='<%# Eval("ShipCode") %>' />
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="editLink" runat="server" 
                                    CommandArgument = '<%# Eval("PackageName") %>' 
                                    Text = "Editar" OnClick="editPackage"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="deleteLink" runat="server" 
                                    CommandArgument = '<%# Eval("PackageName") %>'
                                    Text = "Remover" OnClick="confirmDelete"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                        <asp:TemplateField ItemStyle-HorizontalAlign="Center">
                            <ItemTemplate>
                                <asp:LinkButton ID="duplicarLink" runat="server" 
                                    CommandArgument = '<%# Eval("PackageName") %>'
                                    Text = "Duplicar" OnClick="duplicarPackage"></asp:LinkButton>
                            </ItemTemplate>
                        </asp:TemplateField>

                    </Columns>
                </asp:GridView>
                </div>
                <div class="buttonsDiv">
                        <asp:Button ID="openBtn" runat="server" OnClick="addPackage" Text="Novo pacote" CssClass="btn btn-default" />
                        <asp:Button ID="logoutBtn" runat="server" OnClick="logout" Text="Fechar sessão" CssClass="btn btn-default" />
                </div>

            </div>
        </form>
    </body>
</html>