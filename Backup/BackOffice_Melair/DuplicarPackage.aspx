<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="DuplicarPackage.aspx.vb" Inherits="BackOffice_Melair.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Paquetes | Back Office MELAIR</title>
    <link rel="Stylesheet" href="Styles/bootstrap.min.css" />
        <link rel="Stylesheet" href="Styles/style.css" />
        <link rel="Stylesheet" href="https://code.jquery.com/ui/1.12.0/themes/base/jquery-ui.css" />       
        <script type="text/javascript" src="Scripts/jquery-3.1.0.js"></script>
        <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
        <script type="text/javascript" src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"></script>
        <script type="text/javascript" src="Scripts/jquery.ui.datepicker-pt.js"></script>

     <script type="text/javascript">
            $(function () {
                $("#sailField").datepicker();
                $("#sailField").datepicker("option", $.datepicker.regional["pt"]);
                $("#dateField").datepicker();
                $("#dateField").datepicker("option", $.datepicker.regional["pt"]);

            });

                            
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

    <style type="text/css">
                       
            .wickedpicker 
            {
                z-index: 999999999 !important;
            }
     </style>



</head>
<body>
     <div class="header">
            <div id="logo">
                <img src="royal_logo.png" alt="Royal Caribbean International" /><h1>Gerente de Pacotes</h1>
            </div>
        </div>

    <form id="frDuplicar" runat="server">

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



    <asp:Panel ID="loadPanel" runat="server" CssClass="fadeClass" Visible="false">
                <div class="popupClass">
                    <p>Cargando...</p>
                </div>
    </asp:Panel>

    
    <div class="mainDiv">

    <div id="packsDiv">


    <div id="registerDiv" runat="server" class="registerDiv">
    
                <div id="basicDiv" runat="server">
                    <div id="shipDiv" class="entryDataClass">
                         <label>Navio:</label>
                         <asp:DropDownList ID="shipSelector" runat="server" CssClass="form-control">
                             <asp:ListItem Value="-1" Selected="True">Selecione um navio</asp:ListItem>
                         </asp:DropDownList>
                   </div>
                   <div id="sailDiv" class="entryDataClass">
                         <label>Data de partida:</label>
                         <div id="sailData">
                            <asp:TextBox ID="sailField" runat="server" CssClass="form-control" />
                         </div>
                    </div>
                    <div id="btnDiv">
                        <asp:Button ID="searchBtn" runat="server" OnClick="showItinerary" Text="Ver itinerário" CssClass="btn btn-default" />
                    </div>
                </div>
                
                <div id="hideDiv" runat="server" style="display: none;">
                    <div id="itinDiv">
                        <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                            <ContentTemplate>
                                <label class="headerLbl">Itinerário</label> 
                                
                                <div id="nightsDiv">
                                    <label>Noites:</label> <asp:Label ID="nightsText" runat="server" Text="" />
                                </div>
                                <div id="destinyDiv">
                                    <label>Destino:</label> <asp:Label ID="destinyText" runat="server" Text="" />
                                </div>
                                <asp:Table ID="itinTbl" runat="server" CssClass="table table-striped">
                                    <asp:TableHeaderRow ID="itinHeader">
                                        <asp:TableHeaderCell>Dia</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Porto</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Hora da chegada</asp:TableHeaderCell>
                                        <asp:TableHeaderCell>Hora da saída</asp:TableHeaderCell>
                                    </asp:TableHeaderRow>
                                    <%-- Las filas del itinerario se rellenan dinámicamente --%>
                                </asp:Table>                                
                            </ContentTemplate>
                        </asp:UpdatePanel>
                    </div>
                                                           
                </div>

                <div id="nameDiv" class="entryDataClass">
                          <label>Nome do novo pacote:</label> <asp:TextBox ID="nuevoNombre" runat="server" CssClass="form-control" style="width:400px;" />
                </div>


                <div class="row">
                            <div class="col-md-2" style="min-width:170px;"><label>Definir data do voo:</label> </div>
                            <div class="col-md-4"  style="min-width:220px;">
                                <asp:RadioButtonList ID="rblDiasVuelo" runat="server">
                                    <asp:ListItem Text="O mesmo dia do cruzeiro" Value="0" Selected="True" />
                                    <asp:ListItem Text="1 dia antes do cruzeiro" Value="1" />
                                    <asp:ListItem Text="2 dias antes do cruzeiro" Value="2" />
                                </asp:RadioButtonList>
                            </div>
                            <div class="col-md-6"><p>&nbsp;</p>
                            </div>                                               
               </div>

                <div id="buttonsDiv">
                    <asp:Button ID="backBtn" runat="server" OnClick="goBack" Text="Retorno" CssClass="btn btn-default" />
                    <asp:Button ID="registerBtn" runat="server" OnClick="registerPackage" Text="Duplicar pacote" Enabled="false" CssClass="btn btn-default" />
                </div>
            </div>


    </div>
    </div>


    
    </form>
</body>
</html>
