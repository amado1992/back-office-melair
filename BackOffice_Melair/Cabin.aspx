<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="Cabin.aspx.vb" Inherits="BackOffice_Melair.Cabin" EnableEventValidation="false"%>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
    <style>
        .gridStyleQt {
            font-family: verdana,arial,sans-serif;
            font-size: 11px;
            color: black;
            border-width: 1px;
            border-color: white;
            border-collapse: collapse;
            margin-bottom: 15px;
        }

            .gridStyleQt th {
                background: #007bff;
                border: 1px solid white;
                padding: 0;
                text-align: center;
                width: 150px;
            }

            .gridStyleQt td {
                border-width: 1px;
                padding: 8px 20px 8px 20px;
                border-style: solid;
                border-color: white;
            }

        .displaycol {
            display: none;
        }

        .row-table {
            display: flex;
            flex-direction: row;
        }

        .row-table2 {
            display: flex;
            flex-direction: row;
            width: 93%;
            border: solid 1px gainsboro;
            padding: 10px;
            /*border-radius: 15px;*/
        }

        .column-table {
            display: flex;
            flex-direction: column;
        }

        .custom-label {
            font-weight: bold;
            font-size: 14px;
        }

        .gridStyleQuotas {
            font-family: verdana,arial,sans-serif;
            font-size: 11px;
            color: black;
            border-width: 1px;
            border-color: white;
            border-collapse: collapse;
            margin-bottom: 15px;
        }

            .gridStyleQuotas th {
                background: #007bff;
                border: 1px solid white;
                padding: 0;
                text-align: center;
                width: 150px;
            }

            .gridStyleQuotas td {
                border-width: 1px;
                padding: 8px 20px 8px 20px;
                border-style: solid;
                border-color: white;
            }
    </style>
    <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.min.css"/>
    <link rel="Stylesheet" href="Styles/style-cabin.css" />
    <!--<link rel="Stylesheet" href="Styles/bootstrap.min.css" />
        <script type="text/javascript" src="Scripts/jquery-3.1.0.js"></script>
        <script type="text/javascript" src="Scripts/bootstrap.min.js"></script>-->


    <link rel="stylesheet" href="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/css/bootstrap.min.css" integrity="sha384-xOolHFLEh07PJGoPkLv1IbcEPTNtaed2xpHsD9ESMhqIYd0nLMwNLD69Npy4HI+N" crossorigin="anonymous" />
    <script src="https://cdn.jsdelivr.net/npm/jquery@3.5.1/dist/jquery.slim.min.js" integrity="sha384-DfXdz2htPH0lsSSs5nCTpuj/zy4C+OGpamoFVy38MVBnE+IbbVYUew+OrCXaRkfj" crossorigin="anonymous"></script>
    <script src="https://cdn.jsdelivr.net/npm/bootstrap@4.6.2/dist/js/bootstrap.bundle.min.js" integrity="sha384-Fy6S3B9q64WdZWQUiU+q4/2Lc9npb8tCaSX9FK7E8HnRr0Jz8D6OP9dO5Vg3Q9ct" crossorigin="anonymous"></script>


    <!--<link rel="stylesheet" href="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/css/bootstrap.min.css"/>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/1.12.0/jquery.min.js"></script>
    <script src="http://maxcdn.bootstrapcdn.com/bootstrap/3.3.6/js/bootstrap.min.js"></script>-->

    <script type="text/javascript">
        function getConfirmation(sender) {

            $('#modalPopUp').modal('show');
            $('#btnConfirm').attr('onclick', "$('#modalPopUp').modal('hide');setTimeout(function(){" + $(sender).prop('href') + "}, 50);");
            return false;
        }

        function obtenerValor() {
            const n = 5;
            var stringJoin = ""
            var key = "txt"
            const quotas = [];

            for (let i = 1; i <= n; i++) {
                var _key = key + i
                var val = document.getElementById(_key.toString()).value
                quotas.push(val)

                document.getElementById(_key.toString()).value = ""
            }

            stringJoin = quotas.join('|')
            document.getElementById("myValueHiddenField").value = stringJoin
            console.log(`I love JavaScript.`, document.getElementById("lenghtHiddenField").value);
            return true;
        }

        function showConfirm() {
            $('#confirmModal').modal('show');
        }

        function showConfirmQt() {
            $('#confirmModalQt').modal('show');
        }
    </script>
    <script type="text/javascript"></script>
</head>
<body>

    <div class="header">
        <div id="logo">
            <img src="royal_logo.png" alt="Royal Caribbean International" /><h1>Gerente de Pacotes</h1>
        </div>
    </div>

    <h1 style="text-align: center; margin-top: -20px;">Camarote</h1>

    <form id="form1" runat="server" style="margin-left: 24px;">
        <asp:ScriptManager ID="ScriptManager1"
            runat="server" EnablePartialRendering="true" />
        <div>
            <!--<asp:UpdatePanel ID="UpdatePanel1"
                runat="server" UpdateMode="Conditional">
                <ContentTemplate>-->
            <asp:HiddenField ID="lenghtHiddenField" runat="server" Value="" />
            <asp:HiddenField ID="myValueHiddenField" runat="server" Value="" />

            <div id="modalPopUp" class="modal" tabindex="-1" style="display: none;">
                <div class="modal-dialog">
                    <div class="modal-content">
                        <div class="modal-header">
                            <h5 class="modal-title">Gestionar cuota</h5>
                            <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                                <span aria-hidden="true">&times;</span>
                            </button>
                        </div>
                        <div class="modal-body">
                            <div style="border: solid 1px gainsboro;padding: 20px;">
                                <h6><strong>Crear cuota</strong></h6>
                                <div class="row-table">
                                    <div>
                                        <asp:GridView ID="GridViewQt" runat="server" AutoGenerateColumns="false" CssClass="gridStyleQt">
                                            <Columns>
                                                <asp:BoundField DataField="name" HeaderText="Agencias" ItemStyle-Width="200" ItemStyle-Height="40"/>

                                            </Columns>
                                        </asp:GridView>
                                    </div>
                                    <div style="margin-top: 1px;">
                                        <div style="text-align: center; background: #007bff; height: 17px; margin-bottom: 3px; font-size: 12px; font-weight: bold;">
                                            <asp:Label ID="quotLabel" runat="server" Text='Cuota'></asp:Label>
                                        </div>
                                        <asp:PlaceHolder ID="phDinamicControls" runat="server"></asp:PlaceHolder>
                                    </div>
                                </div>
                            <div class="row-table">
                                <asp:Button CssClass="btn btn-primary" ID="btnEnviar" runat="server" Text="Guardar" OnClick="btnEnviar_Click" OnClientClick="return obtenerValor();" />
                                </div>
                            </div>

                            <div style="margin-top: 10px;">
                            <asp:GridView CssClass="gridStyleQuotas" ID="GridViewQuotas" runat="server" AutoGenerateColumns="False" DataKeyNames="idOrigin" OnRowEditing="OnRowEditingQuotas" OnRowCancelingEdit="OnRowCancelingEditQuotas"  OnRowUpdating="OnRowUpdatingQuotas" OnRowDeleting="OnRowDeletingQuotas">
                                <RowStyle BackColor=" #b0c6e2"
                                    ForeColor="Black"
                                    Font-Italic="true" />

                                <AlternatingRowStyle BackColor=" #dcdfef"
                                    ForeColor="Black"
                                    Font-Italic="true" />
                                <Columns>   
                                    <asp:TemplateField HeaderText="Agencia" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="nameListLabel" runat="server" Text='<%# Eval("name") %>'></asp:Label>
                                        </ItemTemplate>
                                    </asp:TemplateField>

                                    <asp:TemplateField HeaderText="Cuota" ItemStyle-HorizontalAlign="Left">
                                        <ItemTemplate>
                                            <asp:Label ID="quotaListLabel" runat="server" Text='<%# Eval("quota") %>'></asp:Label>
                                        </ItemTemplate>
                                        <EditItemTemplate>
                                            <asp:TextBox ID="quotaListTexBox" runat="server" Text='<%# Eval("quota") %>'></asp:TextBox>
                                        </EditItemTemplate>
                                    </asp:TemplateField>
                                    <asp:CommandField ItemStyle-HorizontalAlign="Center" HeaderText="Acciones" ButtonType="Image" UpdateImageUrl="/Images/Logos/edit.svg" CancelImageUrl="/Images/Logos/close.svg" EditImageUrl="/Images/Logos/edit.svg" DeleteImageUrl="/Images/Logos/remove.svg" ShowEditButton="true" CausesValidation="False" ShowDeleteButton="true" ItemStyle-Width="90" />

                                </Columns>
                            </asp:GridView>
                                </div>

                        </div>
                        <div class="modal-footer">
                            <button type="button" class="btn btn-secondary" data-dismiss="modal">Cerrar</button>
                            
                        </div>
                    </div>
                </div>
            </div>
            <!--</ContentTemplate>
            </asp:UpdatePanel>-->
        </div>

        
        <div class="modal fade" id="confirmModal" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabel">Confirmar</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="confirmLabel" runat="server" Text="" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>
                        <asp:Button ID="okBtn" runat="server" Text="Sí" OnClick="okClick" CssClass="btn btn-primary" />
                    </div>
                </div>
            </div>
        </div>

        <div class="modal fade" id="confirmModalQt" tabindex="-1" role="dialog" aria-labelledby="exampleModalLabel" aria-hidden="true">
            <div class="modal-dialog" role="document">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="exampleModalLabelQt">Confirmar</h5>
                        <button type="button" class="close" data-dismiss="modal" aria-label="Close">
                            <span aria-hidden="true">&times;</span>
                        </button>
                    </div>
                    <div class="modal-body">
                        <asp:Label ID="confirmLabelQt" runat="server" Text="" />
                    </div>
                    <div class="modal-footer">
                        <button type="button" class="btn btn-secondary" data-dismiss="modal">No</button>
                        <asp:Button ID="okBtnQt" runat="server" Text="Sí" OnClick="okClickQt" CssClass="btn btn-primary" />
                    </div>
                </div>
            </div>
        </div>


        <div>
            <!--<asp:UpdatePanel ID="UpdatePanel3"
                runat="server" UpdateMode="Conditional">
                <ContentTemplate>-->
            <div style="overflow-x: auto;">

            <asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" CssClass="gridStyle" DataKeyNames="idCabinLine" OnRowCommand="myGridView_RowCommand2" OnRowEditing="OnRowEditing" OnRowCancelingEdit="OnRowCancelingEdit" OnRowUpdating="OnRowUpdating" OnRowDeleting="OnRowDeleting" PageSize="5" AllowPaging="true" OnPageIndexChanging = "OnPaging" EmptyDataText="No records has been added." OnRowDataBound="GridView1_RowDataBound">
                <RowStyle BackColor="#edde9a"
                    ForeColor="Black"
                    Font-Italic="true" />

                <AlternatingRowStyle BackColor="#f9e8d3"
                    ForeColor="Black"
                    Font-Italic="true" />
                <Columns>

                    <asp:TemplateField HeaderText="Id" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="idCabinLineLabel" runat="server" Text='<%# Eval("idCabinLine") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Id" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="idPackLabel" runat="server" Text='<%# Eval("idPack") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Tipo de camarote editado" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="cabinTypeEditLabel" runat="server" Text='<%# Eval("cabinType") %>'></asp:Label>
                        </ItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Tipo de </br> camarote" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="cabinTypeName" runat="server" Text='<%# Eval("cabinType") %>'></asp:Label>
                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:DropDownList ID="cabinType" runat="server" Width="200" CssClass="margin-l">
                                <asp:ListItem Value="None" Selected="True">Seleccione un camarote
                                </asp:ListItem>
                                <asp:ListItem Text="Interior" Value="I"></asp:ListItem>
                                <asp:ListItem Text="Exterior" Value="O"></asp:ListItem>
                                <asp:ListItem Text="Balcón" Value="B"></asp:ListItem>
                                <asp:ListItem Text="Suite" Value="S"></asp:ListItem>

                            </asp:DropDownList>
                        </EditItemTemplate>

                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Categoría" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="categoryName" runat="server" Text='<%# Eval("category") %>'></asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="category" runat="server" Text='<%# Eval("category") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Precio por pessoa do pacote </br> Valor por pessoa 2 passageiros </br> Camarote duplo" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="priceName" runat="server" Text='<%# Eval("price") %>'></asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="price" runat="server" Text='<%# Eval("price") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Precio por pessoa do pacote </br> Valor por pessoa 1 passageiros </br> Camarote single" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="singleName" runat="server" Text='<%# Eval("single") %>'></asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="singleField" runat="server" Text='<%# Eval("single") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Precio por pessoa do pacote </br> Valor por pessoa 3 passageiros </br> Camarote triple" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="tripleName" runat="server" Text='<%# Eval("triple") %>'></asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="triple" runat="server" Text='<%# Eval("triple") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Precio por pessoa do pacote </br> Valor por pessoa 4 passageiros </br> Camarote cuadruple" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="cuadrupleName" runat="server" Text='<%# Eval("quadruple") %>'></asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="quadruple" runat="server" Text='<%# Eval("quadruple") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Promo" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="promoTitleName" runat="server" Text='<%# Eval("promoTitle") %>'></asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="promoTitle" runat="server" Text='<%# Eval("promoTitle") %>'></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:TemplateField HeaderText="Descripción de promo" ItemStyle-HorizontalAlign="Left">
                        <ItemTemplate>
                            <asp:Label ID="promoDescriptionName" runat="server" Text='<%# Eval("promoDescription") %>'>

                            </asp:Label>

                        </ItemTemplate>
                        <EditItemTemplate>
                            <asp:TextBox ID="promoDescription" runat="server" Text='<%# Eval("promoDescription") %>' TextMode="MultiLine"></asp:TextBox>
                        </EditItemTemplate>
                    </asp:TemplateField>

                    <asp:CommandField ItemStyle-HorizontalAlign="Center" HeaderText="Acciones" ButtonType="Image" UpdateImageUrl="/Images/Logos/edit.svg" CancelImageUrl="/Images/Logos/close.svg"   EditImageUrl="/Images/Logos/edit.svg"  DeleteImageUrl="/Images/Logos/remove.svg" ShowEditButton="true" ShowDeleteButton="true" ItemStyle-Width="90" />

                    <asp:TemplateField HeaderText="Cuota">
                        <ItemTemplate>
                            <asp:Button Font-Size="11" ID="editQt" runat="server" Text="Gestionar cuota" CommandName="myCommand" CommandArgument='<%# Container.DataItemIndex %>' CssClass="btn btn-light" />
                        </ItemTemplate>
                    </asp:TemplateField>

                </Columns>
            </asp:GridView>
                </div>
            <!--</ContentTemplate>
            </asp:UpdatePanel>-->

            <!--<asp:UpdatePanel ID="UpdatePanel2"
                runat="server" UpdateMode="Conditional">
                <ContentTemplate>-->
           
            <div class="row-table2">
                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="cabinTypeLabel" runat="server" Text='<strong>Tipo de camarote</strong> *'></asp:Label>
                    </div>
                    <div>
                        <asp:DropDownList ID="cabinType" runat="server" Width="220" CssClass="form-control">
                            <asp:ListItem Value="-1" Selected="True">Seleccione un camarote</asp:ListItem>
                        </asp:DropDownList>
                    </div>
                </div>
                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="categoryLabel" runat="server" Text='<strong>Categoría</strong> *'></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox placeholder="categoría" ID="category" runat="server" Text='<%# Eval("category") %>' Width="150" CssClass="form-control"></asp:TextBox>
                    </div>
                </div>
                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="priceLabel" runat="server" Text='<strong>Precio duplo</strong> *'></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox CssClass="form-control" placeholder="precio duplo" ID="price" runat="server" Text='<%# Eval("price") %>'></asp:TextBox>
                    </div>
                </div>
                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="singleFieldLabel" runat="server" Text='<strong>Precio single</strong>'></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox CssClass="form-control" placeholder="precio single" ID="singleField" runat="server" Text='<%# Eval("single") %>'></asp:TextBox>
                    </div>
                </div>
                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="tripleLabel" runat="server" Text='<strong>Precio triple</strong>'></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox CssClass="form-control" placeholder="precio triple" ID="triple" runat="server" Text='<%# Eval("triple") %>'></asp:TextBox>
                    </div>
                </div>
                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="quadrupleLabel" runat="server" Text='<strong>Precio cuadruple</strong>'></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox CssClass="form-control" placeholder="precio cuadruple" ID="quadruple" runat="server" Text='<%# Eval("quadruple") %>'></asp:TextBox>
                    </div>
                </div>

                <div>
                    <div style="background: #f9bc0a; font-size: 14px;">
                        <asp:Label CssClass="" ID="promoTitleLabel" runat="server" Text='<strong>Promo título</strong>'></asp:Label>
                    </div>
                    <div>
                        <asp:TextBox CssClass="form-control" placeholder="promo título" ID="promoTitle" runat="server" Text='<%# Eval("promoTitle") %>'></asp:TextBox>
                    </div>
                </div>

                <div>
                    <div>
                        <div style="background: #f9bc0a; font-size: 14px;">
                            <asp:Label CssClass="" ID="promoDescriptionLabel" runat="server" Text='<strong>Promo descripción</strong>'></asp:Label>
                        </div>
                        <div>
                            <asp:TextBox CssClass="form-control" placeholder="promo descripción" ID="promoDescription" runat="server" Text='<%# Eval("promoDescription") %>' TextMode="MultiLine"></asp:TextBox>
                        </div>
                    </div>
                </div>

                <div style="margin-left: 6px;">
                    <asp:Button ID="btnAdd" runat="server" Text="Guardar" OnClick="insert" CssClass="btn btn-primary" />
                </div>
            </div>

            <!--</ContentTemplate>
            </asp:UpdatePanel>-->

            <div class="d-flex flex-row justify-content-center">
                <div class="p-2">
                    <asp:Button ID="btBack" runat="server" OnClick="addPackage" Text="Novo pacote" CssClass="btn btn-primary" />
                </div>
                <div class="p-2">
                    <asp:Button ID="btNext" runat="server" OnClick="addLine" Text="Crear líneas (transfer, hotel o voos)" CssClass="btn btn-primary" />
                </div>
            </div>
        </div>
    </form>

</body>
</html>
