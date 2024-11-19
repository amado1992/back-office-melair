<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditPackage.aspx.vb" Inherits="BackOffice_Melair.EditPackage" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Editar pacote | Back Office MELAIR</title>
    <link rel="Stylesheet" href="Styles/bootstrap.min.css" />
    <link rel="Stylesheet" href="Styles/style.css" />
    <link rel="Stylesheet" href="https://code.jquery.com/ui/1.12.0/themes/base/jquery-ui.css" />
    <!-- <link rel="Stylesheet" href="Styles/wickedpicker.css" /> -->
    <script type="text/javascript" src="Scripts/jquery-3.1.0.js"></script>
    <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>
    <script type="text/javascript" src="https://code.jquery.com/ui/1.12.0/jquery-ui.js"></script>
    <script type="text/javascript" src="Scripts/jquery.ui.datepicker-pt.js"></script>
    <!-- <script type="text/javascript" src="Scripts/wickedpicker.js"></script> -->
    <script type="text/javascript">
        $(function () {
            $("#sailField").datepicker();
            $("#sailField").datepicker("option", $.datepicker.regional["pt"]);
            $("#dateField").datepicker();
            $("#dateField").datepicker("option", $.datepicker.regional["pt"]);
            $(".flightDate").datepicker();
            $(".flightDate").datepicker("option", $.datepicker.regional["pt"]);

//            $(".flightHour").wickedpicker({
//                twentyFour: true,
//                upArrow: 'arrowUp',
//                downArrow: 'arrowDown',
//                close: 'wickedpicker__close',
//                hoverState: 'hover-state',
//                title: 'Hora'
//            });
        });

        function setFormats() {
            $(".flightDate").datepicker();
            $(".flightDate").datepicker("option", $.datepicker.regional["pt"]);

//            $(".flightHour").wickedpicker({
//                twentyFour: true,
//                upArrow: 'arrowUp',
//                downArrow: 'arrowDown',
//                close: 'wickedpicker__close',
//                hoverState: 'hover-state',
//                title: 'Hora'
//            });
        }

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

        function showImagesPopup(img1, img2, img3, img4, img5) {
            $('#imagesPopup').modal('show');
            if (img1 != "") {
                document.getElementById('<%= img1Lbl.ClientID %>').innerHTML = img1;
            }
            if (img2 != "") {
                document.getElementById('<%= img2Lbl.ClientID %>').innerHTML = img2;
            }
            if (img3 != "") {
                document.getElementById('<%= img3Lbl.ClientID %>').innerHTML = img3;
            }
            if (img4 != "") {
                document.getElementById('<%= img4Lbl.ClientID %>').innerHTML = img4;
            }
            if (img5 != "") {
                document.getElementById('<%= img5Lbl.ClientID %>').innerHTML = img5;
            }
        }

        function showPrices() {
            $('#prices').modal('show');
        }
    </script>

    <style type="text/css">
        #pricesTbl, #pricesTbl th, #pricesTbl td
        {
            padding: 5px;
        }

        #pricesTbl th, #pricesTbl td
        {
            text-align: center;
        }
            
        .wickedpicker 
        {
            z-index: 999999999 !important;
        }
    </style>
</head>
<body>
    <div class="header">
        <div id="logo">
            <img src="royal_logo.png" alt="Royal Caribbean Logo" /> <h1>Gerente de Pacotes</h1>
        </div>  
    </div>

    <form id="form1" runat="server">
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
                    <asp:Label ID="confirmLbl" runat="server" Text="AVISO: Esta ação faz com que a perda de qualquer contribuição adicional para o pop-up ... certeza que você deixar essa janela?" />
                  </div>
                  <div class="modal-footer">
                    <asp:Button ID="yesBtn" runat="server" Text="Sim" OnClick="closeConfirm" CssClass="btn btn-default" />
                    <asp:Button ID="noBtn" runat="server" Text="Não sair" OnClick="recoverPopup" CssClass="btn btn-default" />
                  </div>
                </div>
              </div>
            </div>

        <div class="modal fade" id="imagesPopup" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
              <div class="modal-dialog" role="document">
                <div class="modal-content">
                  <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">Subir imagens</h4>
                  </div>
                  <div class="modal-body">
                    <div class="imgStyle">
                       <div id="img1" class="img">
                          <label>Imagem 1:</label> <asp:FileUpload ID="uploadCtrl1" runat="server" />
                          <asp:Label ID="img1Lbl" runat="server" Text="" ForeColor="Green" />
                       </div>
                       <div id="img2" class="img">
                          <label>Imagem 2:</label> <asp:FileUpload ID="uploadCtrl2" runat="server" />
                          <asp:Label ID="img2Lbl" runat="server" Text="" ForeColor="Green" />
                       </div>
                       <div id="img3" class="img">
                          <label>Imagem 3:</label> <asp:FileUpload ID="uploadCtrl3" runat="server" />
                          <asp:Label ID="img3Lbl" runat="server" Text="" ForeColor="Green" />
                          <asp:LinkButton runat="server" Text="Remover" OnClick="deleteImage" CommandArgument="3" ForeColor="Red" />
                       </div>
                       <div id="img4" class="img">
                          <label>Imagem 4:</label> <asp:FileUpload ID="uploadCtrl4" runat="server" />
                          <asp:Label ID="img4Lbl" runat="server" Text="" ForeColor="Green" />
                          <asp:LinkButton runat="server" Text="Remover" OnClick="deleteImage" CommandArgument="4" ForeColor="Red" />
                       </div>
                       <div id="img5" class="img">
                          <label>Imagem 5:</label> <asp:FileUpload ID="uploadCtrl5" runat="server" />
                          <asp:Label ID="img5Lbl" runat="server" Text="" ForeColor="Green" />
                          <asp:LinkButton runat="server" Text="Remover" OnClick="deleteImage" CommandArgument="5" ForeColor="Red" />
                       </div>
                    </div>
                  </div>
                  <div class="modal-footer">
                    <asp:Button ID="saveImagesBtn" runat="server" Text="Salvar imagens" OnClick="saveImages" CssClass="btn btn-default" />
                  </div>
                </div>
              </div>
            </div>

        <div class="modal fade" id="prices" data-backdrop="static" data-keyboard="false" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
            <div id="pricesDlg" class="modal-dialog" role="document">
               <div class="modal-content">
                  <div class="modal-header">
                    <h4 class="modal-title">Detalhes do vôo</h4>
                  </div>
                  <div class="modal-body">
                    <div id="flightDetailsDiv" runat="server">
                       <div id="flightInfoDiv" runat="server">
                                                            <div id="flightBasicDiv" runat="server">
                                                                <label>Código:</label>
                                                                <asp:TextBox ID="flightCode" runat="server" />
                                                                <label>Data:</label>
                                                                <asp:TextBox ID="flightDate" runat="server" CssClass="flightDate" />
                                                                <label>Hora:</label>
                                                                <asp:TextBox ID="flightHour" runat="server" CssClass="flightHour" />
                                                                <label>Taxas p/pessoa:</label>
                                                                <asp:TextBox ID="flightTaxes" runat="server" />
                                                                <label>Vôo de retorno:</label>
                                                                <asp:TextBox ID="returnFlight" runat="server" />
                                                                <label>Horario vôo de retorno:</label>
                                                                <asp:TextBox ID="returnFlightTime" runat="server" />
                                                            </div>
                                                            <div id="flightExtraDiv" runat="server" class="flightExtra">
                                                                <label>Transfer:</label>
                                                                <asp:TextBox ID="flightTransfer" runat="server" />
                                                                <label>Hotel:</label>
                                                                <asp:TextBox ID="flightHotel" runat="server" />
                                                                <label>Descrição oferta:</label>
                                                                <asp:TextBox ID="offerField" runat="server" />
                                                            </div>
                                                        </div>

                                                        <br />

                                                        <div id="flightPricesDiv" runat="server">
                                                            <asp:Table ID="pricesTbl" runat="server">
                                                                <asp:TableHeaderRow>
                                                                    <asp:TableHeaderCell></asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>Ocupación dupla 1ª e 2ª cama</asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>Ocupación tripla y cuádrupla 1ª e 2ª cama</asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>3ª e 4ª cama adultos</asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>3ª e 4ª cama crianças</asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>3ª e 4ª cama bebés</asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>Suplem. Individual para Ocupaçao em Single</asp:TableHeaderCell>
                                                                    <asp:TableHeaderCell>Oferta</asp:TableHeaderCell>
                                                                </asp:TableHeaderRow>
                                                                <asp:TableRow>
                                                                    <asp:TableHeaderCell>Interior</asp:TableHeaderCell>
                                                                    <asp:TableCell><asp:TextBox ID="int1" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="int2" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="int3" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="int4" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="int5" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="int6" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="int7" runat="server" /></asp:TableCell></asp:TableRow><asp:TableRow>
                                                                    <asp:TableHeaderCell>Exterior</asp:TableHeaderCell><asp:TableCell><asp:TextBox ID="ext1" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="ext2" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="ext3" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="ext4" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="ext5" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="ext6" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="ext7" runat="server" /></asp:TableCell></asp:TableRow><asp:TableRow>
                                                                    <asp:TableHeaderCell>Varanda</asp:TableHeaderCell><asp:TableCell><asp:TextBox ID="bal1" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="bal2" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="bal3" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="bal4" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="bal5" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="bal6" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="bal7" runat="server" /></asp:TableCell></asp:TableRow><asp:TableRow>
                                                                    <asp:TableHeaderCell>Suite</asp:TableHeaderCell><asp:TableCell><asp:TextBox ID="su1" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="su2" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="su3" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="su4" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="su5" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="su6" runat="server" /></asp:TableCell><asp:TableCell><asp:TextBox ID="su7" runat="server" /></asp:TableCell></asp:TableRow></asp:Table></div></div></div><div class="modal-footer">
                    <asp:Button ID="exitBtn" runat="server" Text="Sair" OnClick="exitPrices" CssClass="btn btn-default" />
                    <asp:Button ID="saveBtn" runat="server" Text="Salvar preços" OnClick="savePrices" CssClass="btn btn-default" />
                  </div>
               </div>
            </div>
        </div>

        <%-- Panel correspondiente al popup de las líneas del itinerario --%>
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <asp:Panel ID="lines" runat="server" Visible="false">
                    <asp:Panel runat="server" CssClass="fadeClass" />
                    <asp:Panel ID="linesPanel" runat="server" CssClass="linesClass">
                        <div class="modal-header">
                            <h4 class="modal-title">Linhas do vôo</h4>
                        </div>
                        <div id="linesDiv" runat="server" class="modal-body">
                            <asp:UpdatePanel runat="server" style="margin-bottom: 35px; height: 180px; width: 640px; overflow: auto;">
                                <ContentTemplate>
                                    <asp:GridView ID="linesGrid" runat="server" AutoGenerateColumns="false" OnRowDeleting="deleteLine"
                                    CssClass="flightsGrid" AlternatingRowStyle-BackColor="#CCE6FF" style="margin-bottom: 10px;">
                                        <Columns>
                                            <asp:TemplateField HeaderText="Día">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="dayField" runat="server" />
                                                </ItemTemplate>
                                            </asp:TemplateField>
                                            <asp:TemplateField HeaderText="Descrição">
                                                <ItemTemplate>
                                                    <asp:TextBox ID="lineDescField" runat="server" />
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

                                    <div>
                                        <asp:Button ID="lineBtn" runat="server" Text="Adicionar linha" OnClick="addLine" CssClass="btn btn-default" />
                                    </div>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="exitLinesBtn" runat="server" Text="Sair" OnClick="exitLines" CssClass="btn btn-default" />
                            <asp:Button ID="saveLinesBtn" runat="server" Text="Salvar linhas" OnClick="saveLines" CssClass="btn btn-default" />
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>

        <asp:Panel ID="loadPanel" runat="server" CssClass="fadeClass" Visible="false">
                <div class="popupClass">
                    <p>Cargando...</p>
                </div>
        </asp:Panel>

        <div class="titleDiv">
            <asp:Label ID="titleLbl" runat="server" Text="" />
        </div>
        <div id="registerDiv" class="registerDiv">
           <asp:UpdatePanel runat="server">
                <ContentTemplate>
                    <div id="basicData">
                                    <div id="imageDiv" class="entryDataClass">
                                        <div style="height: 40px;">
                                            <label>Imagens do navio:</label>
                                            <asp:Button ID="uploadBtn" runat="server" Text="Subir imagens" OnClick="showImages" CssClass="btn btn-default" style="width: 180px; float: none;" />
                                        </div>
                                        <div>
                                            <asp:Label ID="imagesLbl" runat="server" Text="" ForeColor="Green" />
                                        </div>
                                    </div>
                                    <div id="itinNameDiv" class="entryDataClass">
                                        <label>Nome do itinerário:</label> <asp:TextBox ID="itinNameField" runat="server" CssClass="form-control" />
                                    </div>
                                    <div id="nameDiv" class="entryDataClass">
                                        <label>Nome do pacote:</label> <asp:TextBox ID="nameField" runat="server" CssClass="form-control" ReadOnly=true />
                                    </div>
                                </div>
                </ContentTemplate>
            </asp:UpdatePanel>

                        <div id="extraData">                     
                            <div class="entryDataClass">
                                <label>Turno de comida:</label> <asp:TextBox ID="diningField" runat="server" CssClass="form-control" />
                            </div>
                            <div class="entryDataClass">
                                <label>Está incluído:</label> <asp:TextBox ID="includedField" runat="server" TextMode="MultiLine" CssClass="form-control" />
                            </div>
                            <div class="entryDataClass">
                                <label>Não está incluído:</label> <asp:TextBox ID="notIncludedField" runat="server" TextMode="MultiLine" CssClass="form-control" />
                            </div>
                            <div class="entryDataClass">
                                <label>Notas importantes:</label> <asp:TextBox ID="notesField" runat="server" TextMode="MultiLine" CssClass="form-control" />
                            </div>
                        </div>
                    </div>

            <div style="margin-top: 30px; margin-bottom: 35px; padding-left: 25px;">
                <asp:GridView ID="gridView" runat="server" AutoGenerateColumns="false" OnRowDataBound="onRowDataBound" OnRowDeleting="deleteRow"
                CssClass="flightsGrid">
                            <Columns>
                                <asp:TemplateField HeaderText="Companhia">
                                    <ItemTemplate>
                                        <asp:HiddenField ID="flightId" runat="server" Value="" />
                                        <asp:DropDownList ID="compSelector" runat="server" OnSelectedIndexChanged="companyChanged" AutoPostBack="true">
                                            
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Logo">
                                    <ItemTemplate>
                                        <img id="logoImg" runat="server" src="Images/Logos/tap_logo.png" alt="Logo da companhia" width="60" height="30" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Cidade">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="citySelector" runat="server">
                                            <asp:ListItem Text="Selecione uma cidade" Value="-1" Selected="True" />
                                            
                                            <asp:ListItem Text="Lisboa (Portugal)" Value="LIS" />
                                            <asp:ListItem Text="Porto (Portugal)" Value="OPO" />
                                            <asp:ListItem Text="Madrid (Espanha)" Value="MAD" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Descrição">
                                    <ItemTemplate>
                                        <asp:TextBox ID="descField" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Quota">
                                    <ItemTemplate>
                                        <asp:TextBox ID="quotaField" runat="server" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField HeaderText="Estado">
                                    <ItemTemplate>
                                        <asp:DropDownList ID="statusSelector" runat="server">
                                            <asp:ListItem Text="GTD" Value="GTD" Selected="True" />
                                            <asp:ListItem Text="RQ" Value="RQ" />
                                        </asp:DropDownList>
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="detailsBtn" runat="server" Text="Editar detalhes" OnClick="showDetails" BackColor="#B7CBCF" CssClass="btn btn-default" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:TemplateField>
                                    <ItemTemplate>
                                        <asp:Button ID="linesBtn" runat="server" Text="Editar linhas" OnClick="showLines" BackColor="#B7CBCF" CssClass="btn btn-default" />
                                    </ItemTemplate>
                                </asp:TemplateField>
                                <asp:CommandField ShowDeleteButton="true" ButtonType="Button" DeleteText="Remover" ControlStyle-BackColor="#B7CBCF" ControlStyle-CssClass="btn btn-default" />
                            </Columns>
                        </asp:GridView>

                <asp:Button ID="flightBtn" runat="server" Text="Adicionar vôo" OnClick="addRow" CssClass="btn btn-default" />
            </div>

            <div class="editButtonsDiv">
                <asp:Button ID="backBtn" runat="server" OnClick="goBack" Text="Retorno" CssClass="btn btn-default" />
                <asp:Button ID="editRegisterBtn" runat="server" OnClick="registerPackage" Text="Salvar pacote" CssClass="btn btn-default" />
            </div>
        </div>
    </form>
</body>
</html>
