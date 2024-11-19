<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Register.aspx.vb" Inherits="BackOffice_Melair.Register1" %>

<%--<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>--%>
<!DOCTYPE html>
<html>
<head id="Head1" runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8" />
    <title>Nuevo paquete | Back Office MELAIR</title>
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
            $(".flightDate").datepicker();
            $(".flightDate").datepicker("option", $.datepicker.regional["pt"]);
        });

        function setFormats() {

            $(".flightDate").datepicker();
            $(".flightDate").datepicker("option", $.datepicker.regional["pt"]);
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

        function showImagesPopup() {
            $('#imagesPopup').modal('show');
        }
    </script>
    <style type="text/css">
        #pricesTbl, #pricesTbl th, #pricesTbl td {
            padding: 5px;
        }

            #pricesTbl th, #pricesTbl td {
                text-align: center;
            }

        .wickedpicker {
            z-index: 999999999 !important;
        }

        .container {
            display: grid;
            grid-template-columns: 0em auto auto auto;
            grid-template-rows: 200px auto;
            width: 100% !important;
        }

        .c-table {
            display: flex;
        }

        .row-table {
            display: flex;
            flex-direction: row;
        }

        .column-table {
            display: flex;
            flex-direction: column;
        }
    </style>
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
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
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
        <div class="modal fade" id="confirm" data-backdrop="static" data-keyboard="false"
            tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
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
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span></button>
                        <h4 class="modal-title">Subir imagens</h4>
                    </div>
                    <div class="modal-body">
                        <div class="imgStyle">
                            <div id="img1" class="row">
                                <label>
                                    Imagem 1:</label>
                                <asp:FileUpload ID="uploadCtrl1" runat="server" />
                            </div>
                            <div id="img2" class="row">
                                <label>
                                    Imagem 2:</label>
                                <asp:FileUpload ID="uploadCtrl2" runat="server" />
                            </div>
                            <div id="img3" class="row">
                                <label>
                                    Imagem 3:</label>
                                <asp:FileUpload ID="uploadCtrl3" runat="server" />
                            </div>
                            <div id="img4" class="row">
                                <label>
                                    Imagem 4:</label>
                                <asp:FileUpload ID="uploadCtrl4" runat="server" />
                            </div>
                            <div id="img5" class="row">
                                <label>
                                    Imagem 5:</label>
                                <asp:FileUpload ID="uploadCtrl5" runat="server" />
                            </div>
                        </div>
                    </div>
                    <div class="modal-footer">
                        <asp:Button ID="saveImagesBtn" runat="server" Text="Salvar imagens" OnClick="saveImages"
                            CssClass="btn btn-default" />
                    </div>
                </div>
            </div>
        </div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Panel ID="prices" runat="server" Visible="false">
                    <asp:Panel ID="Panel1" runat="server" CssClass="fadeClass" />
                    <asp:Panel ID="pricesPanel" runat="server" CssClass="pricesClass">
                        <div class="modal-header">
                            <h4 class="modal-title">Detalhes dos voos</h4>
                        </div>
                        <div id="repDiv" class="modal-body" runat="server">
                            <asp:UpdatePanel ID="UpdatePanel2" runat="server">
                                <ContentTemplate>
                                    <asp:Repeater ID="rep" runat="server" OnItemCreated="onItemCreated">
                                        <ItemTemplate>
                                            <div id="flightDetailsDiv" runat="server">
                                                <div id="flightInfoDiv" runat="server">
                                                    <div id="flightBasicDiv" runat="server">
                                                        <label>
                                                            Código:</label>
                                                        <asp:TextBox ID="flightCode" runat="server" />
                                                        <label>
                                                            Data:</label>
                                                        <asp:TextBox ID="flightDate" runat="server" CssClass="flightDate" />
                                                        <label>
                                                            Hora:</label>
                                                        <asp:TextBox ID="flightHour" runat="server" CssClass="flightHour" />
                                                        <label>
                                                            Taxas p/pessoa:</label>
                                                        <asp:TextBox ID="flightTaxes" runat="server" />
                                                        <label>
                                                            Vôo de retorno:</label>
                                                        <asp:TextBox ID="returnFlight" runat="server" />
                                                        <label>
                                                            Horario vôo de retorno:</label>
                                                        <asp:TextBox ID="returnFlightTime" runat="server" />
                                                    </div>
                                                    <div id="flightExtraDiv" runat="server" class="flightExtra">
                                                        <label>
                                                            Transfer:</label>
                                                        <asp:TextBox ID="flightTransfer" runat="server" />
                                                        <label>
                                                            Hotel:</label>
                                                        <asp:TextBox ID="flightHotel" runat="server" />
                                                        <label>
                                                            Descrição oferta:</label>
                                                        <asp:TextBox ID="offerField" runat="server" />
                                                    </div>
                                                </div>
                                                <br />
                                                <div id="flightPricesDiv" runat="server">
                                                    <asp:Table ID="pricesTbl" runat="server" class="pricesTbl">
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
                                                            <asp:TableCell>
                                                                <asp:TextBox ID="int1" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="int2" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="int3" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="int4" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="int5" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="int6" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="int7" runat="server" />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <asp:TableHeaderCell>Exterior</asp:TableHeaderCell><asp:TableCell>
                                                                <asp:TextBox ID="ext1" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="ext2" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="ext3" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="ext4" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="ext5" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="ext6" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="ext7" runat="server" />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <asp:TableHeaderCell>Varanda</asp:TableHeaderCell><asp:TableCell>
                                                                <asp:TextBox ID="bal1" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="bal2" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="bal3" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="bal4" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="bal5" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="bal6" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="bal7" runat="server" />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                        <asp:TableRow>
                                                            <asp:TableHeaderCell>Suite</asp:TableHeaderCell><asp:TableCell>
                                                                <asp:TextBox ID="su1" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="su2" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="su3" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="su4" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="su5" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="su6" runat="server" />
                                                            </asp:TableCell><asp:TableCell>
                                                                <asp:TextBox ID="su7" runat="server" />
                                                            </asp:TableCell>
                                                        </asp:TableRow>
                                                    </asp:Table>
                                                </div>
                                                <br />
                                            </div>
                                        </ItemTemplate>
                                    </asp:Repeater>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                        <div class="modal-footer">
                            <asp:Button ID="exitBtn" runat="server" Text="Sair" OnClick="exitPrices" CssClass="btn btn-default" />
                            <asp:Button ID="saveBtn" runat="server" Text="Salvar preços" OnClick="savePrices"
                                CssClass="btn btn-default" />
                        </div>
                    </asp:Panel>
                </asp:Panel>
            </ContentTemplate>
        </asp:UpdatePanel>
        <asp:Panel ID="loadPanel" runat="server" CssClass="fadeClass" Visible="false">
            <div class="popupClass">
                <p>
                    Cargando...
                </p>
            </div>
        </asp:Panel>

        <div class="row-table">
            <div class="column-table" style="margin-left: 40px;">
                <div>
                    <div id="basicDiv" runat="server" style="width: 553px;">
                        <div id="shipDiv" class="entryDataClass">
                            <label>
                                Navio:</label>
                            <asp:DropDownList Width="441" ID="shipSelector" runat="server" CssClass="form-control">
                                <asp:ListItem Value="-1" Selected="True">Selecione um navio</asp:ListItem>
                            </asp:DropDownList>
                        </div>
                        <div id="sailDiv" class="entryDataClass">
                            <label>
                                Data de partida:</label>
                            <div id="sailData">
                                <asp:TextBox Width="441" ID="sailField" runat="server" CssClass="form-control" />
                            </div>
                        </div>
                        <div id="btnDiv">
                            <asp:Button ID="searchBtn" runat="server" OnClick="showItinerary" Text="Ver itinerário"
                                CssClass="btn btn-default" />
                        </div>
                    </div>
                </div>

                <div id="col1El2Hide" runat="server" style="display: none">
                    <div id="hideDiv" runat="server" style="border: solid 1px gainsboro;padding: 14px;">
                        <div id="itinDiv">
                            <asp:UpdatePanel ID="UpdatePanel3" runat="server">
                                <ContentTemplate>
                                    <label class="headerLbl">
                                        Itinerário</label>
                                    <div id="nightsDiv">
                                        <label>
                                            Noites:</label>
                                        <asp:Label ID="nightsText" runat="server" Text="" />
                                    </div>
                                    <div id="destinyDiv">
                                        <label>
                                            Destino:</label>
                                        <asp:Label ID="destinyText" runat="server" Text="" />
                                    </div>
                                    <asp:Table ID="itinTbl" runat="server" CssClass="table-striped" Width="524px">
                                        <asp:TableHeaderRow ID="itinHeader">
                                            <asp:TableHeaderCell>Dia</asp:TableHeaderCell>
                                            <asp:TableHeaderCell>Porto</asp:TableHeaderCell>
                                            <asp:TableHeaderCell>Hora da chegada</asp:TableHeaderCell>
                                            <asp:TableHeaderCell>Hora da saída</asp:TableHeaderCell>
                                        </asp:TableHeaderRow>

                                    </asp:Table>
                                </ContentTemplate>
                            </asp:UpdatePanel>
                        </div>
                    </div>
                </div>
            </div>

            <div id="col2Hide" class="column-table" style="margin-left: 26px; display: none" runat="server">
                <div>
                    <div id="idHide1" style="display: none;" runat="server">
                        <label class="headerLbl">
                            Informação básica</label>
                        <div id="basicData">
                            <div id="imageDiv" class="entryDataClass">
                                <div>
                                    <label>
                                        Imagens do navio:</label>
                                    <button id="uploadBtn" type="button" onclick="showImagesPopup()" class="btn btn-default">
                                        Subir imagens</button>
                                </div>
                                <div>
                                    <asp:Label ID="imagesLbl" runat="server" Text="" ForeColor="Green" />
                                </div>
                            </div>
                            <div id="itinNameDiv" class="entryDataClass">
                                <label>
                                    Nome do itinerário:</label>
                                <asp:TextBox ID="itinNameField" runat="server" CssClass="form-control" />
                            </div>
                            <div id="nameDiv" class="entryDataClass">
                                <label>
                                    Nome do pacote:</label>
                                <asp:TextBox ID="nameField" runat="server" CssClass="form-control" />
                            </div>
                        </div>

                    </div>
                </div>
                <div>
                    <div>
                        <div class="entryDataClass">
                            <div>
                                <label>
                                    Descrição navio</label>
                                <asp:TextBox Height="149"  Width="425px" ID="descripNavio" runat="server" TextMode="MultiLine" CssClass="form-control" />
                            </div>
                        </div>
                        <label class="headerLbl">
                            Seleccione los items que añadirá al paquete</label>
                        <div class="entryDataClass">
                            <label>Voo</label>
                            <asp:CheckBox ID="flightCheckBox"
                                AutoPostBack="False"
                                TextAlign="Left"
                                OnCheckedChanged="Check_Flight"
                                runat="server" CssClass="check-box1" />
                        </div>

                        <div class="entryDataClass">
                            <label>Hotel</label>
                            <asp:CheckBox ID="hotelCheckBox"
                                AutoPostBack="False"
                                TextAlign="Left"
                                OnCheckedChanged="Check_Hotel"
                                runat="server" CssClass="check-box2" />
                        </div>

                        <div class="entryDataClass">
                            <label>Transferes</label>
                            <asp:CheckBox ID="transferCheckBox"
                                AutoPostBack="False"
                                TextAlign="Left"
                                OnCheckedChanged="Check_Transfer"
                                runat="server" CssClass="check-box3" />
                        </div>
                    </div>
                </div>
            </div>
            <div id="col3Hide" class="column-table" style="margin-left: 32px; display: none" runat="server">
                <div>
                    <div id="extraData" runat="server" style="display: none; margin-right: 184px;">
                        <div class="entryDataClass">
                            <label>
                                Turno de comida:</label>
                            <asp:TextBox ID="diningField" runat="server" CssClass="form-control" />
                        </div>
                        <div class="entryDataClass">
                            <label>
                                Está incluído:</label>
                            <asp:TextBox ID="includedField" runat="server" TextMode="MultiLine" CssClass="form-control" />
                        </div>

                    </div>
                </div>
                <div>
                    <div>
                        <div class="entryDataClass">
                            <label>
                                Não está incluído:</label>
                            <asp:TextBox Width="424px" ID="notIncludedField" runat="server" TextMode="MultiLine" CssClass="form-control" />
                        </div>
                        <div class="entryDataClass">
                            <label>
                                Notas importantes:</label>
                            <asp:TextBox Width="424px" ID="notesField" runat="server" TextMode="MultiLine" CssClass="form-control" />
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!--<div id="flightsDiv">
                <label class="headerLbl" style="margin-top: 20px;">
                    Voos</label>
                <br />
                <br />
                <div class="row">
                    <div class="col-md-2">
                        <label>
                            Definir data do voo:</label>
                    </div>
                    <div class="col-md-2">
                        <asp:RadioButtonList ID="rblDiasVuelo" runat="server">
                            <asp:ListItem Text="O mesmo dia do cruzeiro" Value="0" Selected="True" />
                            <asp:ListItem Text="1 dia antes do cruzeiro" Value="1" />
                            <asp:ListItem Text="2 dias antes do cruzeiro" Value="2" />
                            <asp:ListItem Text="4 dias antes do cruzeiro" Value="4" />
                        </asp:RadioButtonList>
                    </div>
                    <div class="col-md-2">
                        <p>
                            &nbsp;</p>
                    </div>
                    <div class="col-md-2">
                        <label>
                            Definir tipo de vuelo:</label>
                    </div>
                    <div class="col-md-4">
                       <asp:RadioButtonList ID="tipoVuelo" AutoPostBack="true" OnChange="addRemoveRow"  runat="server">
                            <asp:ListItem Text="Ida/vuelta mismo origen y destino" Value="0"  />
                            <asp:ListItem Text="Ida/vuelta con origen y destino diferentes"   Value="1" />
                            
                        </asp:RadioButtonList>
                    </div>
                </div>
                <asp:GridView ID="gridView" runat="server" AutoGenerateColumns="false" OnRowDataBound="onRowDataBound"
                    OnRowDeleting="deleteRow" CssClass="flightsGrid">
                    <Columns>
                        <asp:TemplateField HeaderText="Companhia">
                            <ItemTemplate>
                                <asp:DropDownList ID="compSelector" runat="server" OnSelectedIndexChanged="companyChanged"
                                    AutoPostBack="true">
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Logo">
                            <ItemTemplate>
                                <img id="logoImg" runat="server" src="Images/Logos/tap_logo.png" alt="Logo da companhia"
                                    width="75" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cidade ORIGEM">
                            <ItemTemplate>
                                <asp:DropDownList ID="citySelector" runat="server">
                                    <asp:ListItem Text="Selecione uma cidade" Value="-1" Selected="True" />
                                    <asp:ListItem Text="Lisboa (Portugal)" Value="LIS" />
                                    <asp:ListItem Text="Porto (Portugal)" Value="OPO" />
                                    <asp:ListItem Text="Madrid (Espanha)" Value="MAD" />
                                    <asp:ListItem Text="Estocolmo (Suecia)" Value="STO" />                                    
                                </asp:DropDownList>
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Cidade DESTINO">
                            <ItemTemplate>
                                <asp:TextBox ID="destinoField" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Descrição">
                            <ItemTemplate>
                                <asp:TextBox ID="descField" runat="server" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="Número de voos">
                            <ItemTemplate>
                                <asp:TextBox ID="numField" runat="server" />
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
                                <asp:Button ID="detailsBtn" runat="server" Text="Adicionar detalhes" OnClick="showDetails"
                                    BackColor="#B7CBCF" CssClass="btn btn-default" />
                            </ItemTemplate>
                        </asp:TemplateField>
                        <asp:CommandField ShowDeleteButton="true" ButtonType="Button" DeleteText="Remover"
                            ControlStyle-BackColor="#B7CBCF" ControlStyle-CssClass="btn btn-default" />
                    </Columns>
                </asp:GridView>
                <asp:Button ID="flightBtn" runat="server" Text="Adicionar vôo" OnClick="addRow" CssClass="btn btn-default" />
            </div>-->

        <div id="buttonsDiv" style="margin-left: 40px;">
            <asp:Button ID="backBtn" runat="server" OnClick="goBack" Text="Retorno" CssClass="btn btn-default" />
            <asp:Button ID="registerBtn" runat="server" OnClick="registerPackage" Text="Registar pacote"
                Enabled="false" CssClass="btn btn-default" />
        </div>

    </form>
</body>
</html>
