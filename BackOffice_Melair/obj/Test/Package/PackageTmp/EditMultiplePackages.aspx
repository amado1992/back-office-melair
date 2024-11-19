<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="EditMultiplePackages.aspx.vb"
    Inherits="BackOffice_Melair.EditMultiplePackages" %>

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
            <img src="royal_logo.png" alt="Royal Caribbean Logo" />
            <h1>
                Gerente de Pacotes</h1>
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
    <div class="modal fade" id="imagesPopup" tabindex="-1" role="dialog" aria-labelledby="myModalLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                        <span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title">
                        Subir imagens</h4>
                </div>
                <div class="modal-body">
                    <div class="imgStyle">
                        <div id="img1" class="img">
                            <label>
                                Imagem 1:</label>
                            <asp:FileUpload ID="uploadCtrl1" runat="server" />
                            <asp:Label ID="img1Lbl" runat="server" Text="" ForeColor="Green" />
                        </div>
                        <div id="img2" class="img">
                            <label>
                                Imagem 2:</label>
                            <asp:FileUpload ID="uploadCtrl2" runat="server" />
                            <asp:Label ID="img2Lbl" runat="server" Text="" ForeColor="Green" />
                        </div>
                        <div id="img3" class="img">
                            <label>
                                Imagem 3:</label>
                            <asp:FileUpload ID="uploadCtrl3" runat="server" />
                            <asp:Label ID="img3Lbl" runat="server" Text="" ForeColor="Green" />
                            <asp:LinkButton runat="server" Text="Remover" OnClick="deleteImage" CommandArgument="3"
                                ForeColor="Red" />
                        </div>
                        <div id="img4" class="img">
                            <label>
                                Imagem 4:</label>
                            <asp:FileUpload ID="uploadCtrl4" runat="server" />
                            <asp:Label ID="img4Lbl" runat="server" Text="" ForeColor="Green" />
                            <asp:LinkButton runat="server" Text="Remover" OnClick="deleteImage" CommandArgument="4"
                                ForeColor="Red" />
                        </div>
                        <div id="img5" class="img">
                            <label>
                                Imagem 5:</label>
                            <asp:FileUpload ID="uploadCtrl5" runat="server" />
                            <asp:Label ID="img5Lbl" runat="server" Text="" ForeColor="Green" />
                            <asp:LinkButton runat="server" Text="Remover" OnClick="deleteImage" CommandArgument="5"
                                ForeColor="Red" />
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
    <div class="titleDiv">
        <asp:Label ID="titleLbl" runat="server" Text="" />
    </div>
    <div id="registerDiv" class="registerDiv">
        <asp:UpdatePanel runat="server">
            <ContentTemplate>
                <div id="basicData">
                    <div id="imageDiv" class="entryDataClass">
                        <div style="height: 40px;">
                            <asp:CheckBox ID="selectImages" runat="server" CssClass="editMultiplePackagesCheckBox" />
                            <label>
                                Imagens do navio:</label>
                            <asp:Button ID="uploadBtn" runat="server" Text="Subir imagens" OnClick="showImages"
                                CssClass="btn btn-default" Style="width: 180px; float: none;" />
                        </div>
                        <div>
                            <asp:Label ID="imagesLbl" runat="server" Text="" ForeColor="Green" />
                        </div>
                    </div>
                    <div id="itinNameDiv" class="entryDataClass">
                        <asp:CheckBox ID="selectItineraryName" runat="server" CssClass="editMultiplePackagesCheckBox" />
                        <label>
                            Nome do itinerário:</label>
                        <asp:TextBox ID="itinNameField" runat="server" CssClass="form-control" />
                    </div>
                    <div id="nameDiv" class="entryDataClass">
                        <label>
                            Nome do pacote:</label>
                        <asp:TextBox ID="nameField" runat="server" CssClass="form-control" ReadOnly="true" />
                    </div>
                </div>
            </ContentTemplate>
        </asp:UpdatePanel>
        <div id="extraData">
            <div class="entryDataClass">
                <asp:CheckBox ID="selectDining" runat="server" CssClass="editMultiplePackagesCheckBox" />
                <label>
                    Turno de comida:</label>
                <asp:TextBox ID="diningField" runat="server" CssClass="form-control" />
            </div>
            <div class="entryDataClass">
                <asp:CheckBox ID="selectIncluded" runat="server" CssClass="editMultiplePackagesCheckBox" />
                <label>
                    Está incluído:</label>
                <asp:TextBox ID="includedField" runat="server" TextMode="MultiLine" CssClass="form-control" />
            </div>
            <div class="entryDataClass">
                <asp:CheckBox ID="selectNotIncluded" runat="server" CssClass="editMultiplePackagesCheckBox" />
                <label>
                    Não está incluído:</label>
                <asp:TextBox ID="notIncludedField" runat="server" TextMode="MultiLine" CssClass="form-control" />
            </div>
            <div class="entryDataClass">
                <asp:CheckBox ID="selectNotes" runat="server" CssClass="editMultiplePackagesCheckBox" />
                <label>
                    Notas importantes:</label>
                <asp:TextBox ID="notesField" runat="server" TextMode="MultiLine" CssClass="form-control" />
            </div>
        </div>
    </div>
    <div id="flightModal" class="modal fade" role="dialog">
        <div class="modal-dialog">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        &times;</button>
                    <h4 class="modal-title">
                        Vôo</h4>
                </div>
                <div class="modal-body">
                <style>
                    td
                    {
                        padding: 5px;
                    }
                </style>
                    <table style="width: 100%">
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightCompany" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Companhia:</label>
                                    <asp:DropDownList ID="listFlightCompany" runat="server" AutoPostBack="true" OnTextChanged="refreshLogo"
                                        CssClass="form-control">
                                        <asp:ListItem Text="" Value="-1" Selected="True" />
                                        <asp:ListItem Text="Emirates" Value="1" />
                                        <asp:ListItem Text="Tap Air Portugal" Value="2" />
                                    </asp:DropDownList>
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <label>
                                        Logo:</label>
                                    <asp:Image ID="imageFlightLogo" runat="server" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightCity" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Cidade:</label>
                                    <asp:DropDownList ID="listFlightCity" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="" Value="-1" Selected="True" />
                                        <asp:ListItem Text="Lisboa (Portugal)" Value="LIS" />
                                        <asp:ListItem Text="Porto (Portugal)" Value="OPO" />
                                        <asp:ListItem Text="Madrid (Espanha)" Value="MAD" />
                                    </asp:DropDownList>
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightDescription" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Descrição:</label>
                                    <asp:TextBox ID="textFlightDescription" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightCabinsPerCity" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Quota:</label>
                                    <asp:TextBox ID="textFlightCabinsPerCity" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightStatus" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Estado:</label>
                                    <asp:DropDownList ID="listFlightStatus" runat="server" CssClass="form-control">
                                        <asp:ListItem Text="" Value="-1" Selected="True" />
                                        <asp:ListItem Text="GTD" Value="1" />
                                        <asp:ListItem Text="RQ" Value="2" />
                                    </asp:DropDownList>
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightCode" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Código:</label>
                                    <asp:TextBox ID="textFlightCode" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightTime" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Hora:</label>
                                    <asp:TextBox ID="textFlightTime" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightTaxes" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Taxas p/pessoa:</label>
                                    <asp:TextBox ID="textFlightTaxes" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightReturn" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Vôo de retorno:</label>
                                    <asp:TextBox ID="textFlightReturn" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightReturnTime" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Horario vôo de retorno:</label>
                                    <asp:TextBox ID="textFlightReturnTime" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightTransfer" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Transfer:</label>
                                    <asp:TextBox ID="textFlightTransfer" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                        </tr>
                        <tr>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightHotel" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Hotel:</label>
                                    <asp:TextBox ID="textFlightHotel" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                            <td>
                                <div class="entryDataClass">
                                    <asp:CheckBox ID="selectFlightOfferDescription" runat="server" CssClass="editMultiplePackagesCheckBox" />
                                    <label>
                                        Descrição oferta:</label>
                                    <asp:TextBox ID="textFlightOfferDescription" runat="server" CssClass="form-control" />
                                </div>
                            </td>
                        </tr>
                    </table>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="saveFlightBtn" type="button" class="btn btn-default" OnClick="saveFlight"
                        runat="server" Text="Salvar Vôo" />
                    <button type="button" class="btn btn-default" data-dismiss="modal">
                        Cerrar</button>
                </div>
            </div>
        </div>
    </div>
    <div id="pricesModal" class="modal fade" role="dialog">
        <div class="modal-dialog modal-lg">
            <!-- Modal content-->
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal">
                        &times;</button>
                    <h4 class="modal-title">
                        Preços</h4>
                </div>
                <div class="modal-body">
                    <asp:Table ID="pricesTable" runat="server" CssClass="pricesTableStyle">
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
                                <asp:CheckBox ID="cInt1" runat="server" />
                                <asp:TextBox ID="tInt1" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cInt2" runat="server" />
                                <asp:TextBox ID="tInt2" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cInt3" runat="server" />
                                <asp:TextBox ID="tInt3" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cInt4" runat="server" />
                                <asp:TextBox ID="tInt4" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cInt5" runat="server" />
                                <asp:TextBox ID="tInt5" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cInt6" runat="server" />
                                <asp:TextBox ID="tInt6" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cInt7" runat="server" />
                                <asp:TextBox ID="tInt7" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableHeaderCell>Exterior</asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt1" runat="server" />
                                <asp:TextBox ID="tExt1" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt2" runat="server" />
                                <asp:TextBox ID="tExt2" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt3" runat="server" />
                                <asp:TextBox ID="tExt3" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt4" runat="server" />
                                <asp:TextBox ID="tExt4" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt5" runat="server" />
                                <asp:TextBox ID="tExt5" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt6" runat="server" />
                                <asp:TextBox ID="tExt6" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cExt7" runat="server" />
                                <asp:TextBox ID="tExt7" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableHeaderCell>Varanda</asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal1" runat="server" />
                                <asp:TextBox ID="tBal1" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal2" runat="server" />
                                <asp:TextBox ID="tBal2" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal3" runat="server" />
                                <asp:TextBox ID="tBal3" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal4" runat="server" />
                                <asp:TextBox ID="tBal4" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal5" runat="server" />
                                <asp:TextBox ID="tBal5" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal6" runat="server" />
                                <asp:TextBox ID="tBal6" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cBal7" runat="server" />
                                <asp:TextBox ID="tBal7" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                        <asp:TableRow>
                            <asp:TableHeaderCell>Suite</asp:TableHeaderCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui1" runat="server" />
                                <asp:TextBox ID="tSui1" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui2" runat="server" />
                                <asp:TextBox ID="tSui2" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui3" runat="server" />
                                <asp:TextBox ID="tSui3" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui4" runat="server" />
                                <asp:TextBox ID="tSui4" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui5" runat="server" />
                                <asp:TextBox ID="tSui5" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui6" runat="server" />
                                <asp:TextBox ID="tSui6" runat="server" />
                            </asp:TableCell>
                            <asp:TableCell>
                                <asp:CheckBox ID="cSui7" runat="server" />
                                <asp:TextBox ID="tSui7" runat="server" />
                            </asp:TableCell>
                        </asp:TableRow>
                    </asp:Table>
                </div>
                <div class="modal-footer">
                    <asp:Button ID="selectAllBtn" type="button" class="btn btn-default btn-xs" OnClick="selectAll"
                        runat="server" Text="Selecionar tudo" CommandArgument="True" />
                    <asp:Button ID="unselectAllBtn" type="button" class="btn btn-default btn-xs" OnClick="unselectAll"
                        runat="server" Text="Desmarque todos" CommandArgument="False" />
                    <asp:Button ID="selectIntBtn" type="button" class="btn btn-default btn-xs" OnClick="selectInt"
                        runat="server" Text="Selecionar Interior" CommandArgument="True" />
                    <asp:Button ID="selectExtBtn" type="button" class="btn btn-default btn-xs" OnClick="selectExt"
                        runat="server" Text="Selecionar Exterior" CommandArgument="True" />
                    <asp:Button ID="selectBalBtn" type="button" class="btn btn-default btn-xs" OnClick="selectBal"
                        runat="server" Text="Selecionar Varanda" CommandArgument="True" />
                    <asp:Button ID="selectSuiBtn" type="button" class="btn btn-default btn-xs" OnClick="selectSui"
                        runat="server" Text="Selecionar Suite" CommandArgument="True" />
                    <asp:Button ID="savePricesBtn" type="button" class="btn btn-default btn-xs" OnClick="savePrices"
                        runat="server" Text="Salvar Preços" />
                    <button type="button" class="btn btn-default btn-xs" data-dismiss="modal">
                        Cerrar</button>
                </div>
            </div>
        </div>
    </div>
    <div class="editButtonsDiv">
        <asp:Button ID="backBtn" runat="server" OnClick="goBack" Text="Retorno" class="btn btn-info btn-lg" />
        <asp:Button ID="editRegisterBtn" runat="server" OnClick="registerPackage" Text="Salvar pacote"
            class="btn btn-info btn-lg" />
        <asp:Button ID="editFlightBtn" type="button" class="btn btn-info btn-lg" data-toggle="modal"
            OnClick="showEditFlight" runat="server" Text="Vôo" />
        <asp:Button ID="editPricesBtn" type="button" class="btn btn-info btn-lg" data-toggle="modal"
            OnClick="showEditPrices" runat="server" Text="Preços" />
    </div>
    </form>
</body>
</html>
