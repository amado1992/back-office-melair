<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Lines.aspx.vb" Inherits="BackOffice_Melair.Lines" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>

    <link rel="Stylesheet" href="Styles/style-cabin.css" />

    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" integrity="sha384-xOolHFLEh07PJGoPkLv1IbcEPTNtaed2xpHsD9ESMhqIYd0nLMwNLD69Npy4HI+N" crossorigin="anonymous" />
    <script src="https://cdn.jsdelivr.net/npm/jquery@3.5.1/dist/jquery.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-Fy6S3B9q64WdZWQUiU+q4/2Lc9npb8tCaSX9FK7E8HnRr0Jz8D6OP9dO5Vg3Q9ct" crossorigin="anonymous"></script>

    <style>
        .gridStyleT {
            font-family: verdana,arial,sans-serif;
            font-size: 11px;
            color: black;
            border-width: 1px;
            border-color: white;
            border-collapse: collapse;
            margin-bottom: 15px;
        }

            .gridStyleT th {
                background: #6cab44;
                border: 1px solid white;
                padding: 0;
                text-align: center;
                width: 150px;
            }

            .gridStyleT td {
                border-width: 1px;
                padding: 8px 20px 8px 20px;
                border-style: solid;
                border-color: white;
            }

        .gridStyleHV {
            font-family: verdana,arial,sans-serif;
            font-size: 11px;
            color: black;
            border-width: 1px;
            border-color: white;
            border-collapse: collapse;
            margin-bottom: 15px;
        }

            .gridStyleHV th {
                background: #4473c3;
                border: 1px solid white;
                padding: 0;
                text-align: center;
                width: 150px;
            }

            .gridStyleHV td {
                border-width: 1px;
                padding: 8px 20px 8px 20px;
                border-style: solid;
                border-color: white;
            }
    </style>
</head>
<body>
    <div class="header">
        <div id="logo">
            <img src="royal_logo.png" alt="Royal Caribbean International" /><h1>Gerente de Pacotes</h1>
        </div>
    </div>
    <h1 style="text-align: center;">Líneas vuelo, hotel y transfer</h1>
    <form id="form1" runat="server">
        <div class="container form-group" style="margin-top: 24px; background: #e4e4e4; height: 175px;">
            <div class="row" style="padding-top: 6px;">
                <div class="col-3">
                    <asp:Label runat="server" ID="LabelLineType" Text="Tipo de linha">
                    </asp:Label>
                    <asp:DropDownList ID="lineType" runat="server" Width="235" CssClass="form-control">
                        <asp:ListItem Value="-1" Selected="True">Seleccionar tipo de linha</asp:ListItem>
                    </asp:DropDownList>
                </div>

                <div class="col-3">
                    <asp:Label runat="server" ID="descriptionLabel" Text="Descrição">
                    </asp:Label>
                    <asp:TextBox CssClass="form-control" runat="server" ID="descripTextBox" TextMode="MultiLine"></asp:TextBox>
                </div>

                <div class="col-3">
                    <asp:Label runat="server" ID="LabelProgramLine" Text="Linha programa">
                    </asp:Label>
                    <asp:DropDownList ID="programLine" runat="server" Width="235" CssClass="form-control">
                        <asp:ListItem Value="-1" Selected="True">Seleccionar linha programa</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-3">
                    <asp:Label runat="server" ID="LabelAfterBefore" Text="Antes / depois itinerário">
                    </asp:Label>
                    <asp:DropDownList ID="afterBefore" runat="server" Width="205" CssClass="form-control">
                        <asp:ListItem Value="-1" Selected="True">Seleccione un elemento</asp:ListItem>
                    </asp:DropDownList>
                </div>
                <div class="col-3">
                    <div style="padding-top: 25px;">
                        <asp:Button runat="server" Text="Guardar" CssClass="btn btn-primary" OnClick="insert"></asp:Button>
                    </div>
                </div>
            </div>
        </div>

        <div class="container form-group" style="margin-top: 20px;">
            <div id="idCheckTransferes" class="row" runat="server" style="display: none !important;">

                <h4>Transferes</h4>
                <asp:GridView Width="1142" ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="gridStyleT" EmptyDataText="No records has been added." OnRowDataBound="GridView1_RowDataBound">

                    <RowStyle BackColor="#d4e4cc"
                        ForeColor="Black"
                        Font-Italic="true" />

                    <AlternatingRowStyle BackColor="#edf4ed"
                        ForeColor="Black"
                        Font-Italic="true" />

                    <Columns>
                        <asp:BoundField DataField="description" HeaderText="Descrição" />
                        <asp:BoundField DataField="num" HeaderText="Linha programa" />
                        <asp:BoundField DataField="before" HeaderText="Antes / depois itinerário" />
                    </Columns>
                </asp:GridView>
            </div>

            <div id="idCheckHotel" class="row" runat="server" style="display: none !important;">
                <h4>Hotel</h4>
                <asp:GridView Width="1142" ID="GridView2" runat="server" AutoGenerateColumns="False" CssClass="gridStyleHV" EmptyDataText="No records has been added." OnRowDataBound="GridView2_RowDataBound">

                    <RowStyle BackColor=" #b0c6e2"
                        ForeColor="Black"
                        Font-Italic="true" />

                    <AlternatingRowStyle BackColor=" #dcdfef"
                        ForeColor="Black"
                        Font-Italic="true" />

                    <Columns>
                        <asp:BoundField DataField="description" HeaderText="Descrição" />
                        <asp:BoundField DataField="num" HeaderText="Linha programa" />
                        <asp:BoundField DataField="before" HeaderText="Antes / depois itinerário" />
                    </Columns>
                </asp:GridView>
            </div>


            <div id="idCheckVoos" class="row" runat="server" style="display: none !important;">
                <h4>Voos</h4>
                <asp:GridView Width="1142" ID="GridView3" runat="server" AutoGenerateColumns="False" CssClass="gridStyleHV" EmptyDataText="No records has been added." OnRowDataBound="GridView3_RowDataBound">

                    <RowStyle BackColor=" #b0c6e2"
                        ForeColor="Black"
                        Font-Italic="true" />

                    <AlternatingRowStyle BackColor=" #dcdfef"
                        ForeColor="Black"
                        Font-Italic="true" />

                    <Columns>
                        <asp:BoundField DataField="description" HeaderText="Descrição" />
                        <asp:BoundField DataField="num" HeaderText="Linha programa" />
                        <asp:BoundField DataField="before" HeaderText="Antes / depois itinerário" />
                    </Columns>
                </asp:GridView>
            </div>
        </div>

        <div class="d-flex flex-row justify-content-center">

            <div class="p-2">
                <asp:Button ID="btPackages" runat="server" OnClick="packages" Text="Retorno a inicio" CssClass="btn btn-primary" />
            </div>
            <div class="p-2">
                <asp:Button ID="btCabins" runat="server" OnClick="cabins" Text="Camarotes" CssClass="btn btn-primary" />
            </div>
        </div>

    </form>

</body>
</html>
