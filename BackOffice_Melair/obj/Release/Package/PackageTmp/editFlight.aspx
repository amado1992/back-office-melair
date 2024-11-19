<%@ Page Language="vb" AutoEventWireup="false" CodeBehind="editFlight.aspx.vb" Inherits="BackOffice_Melair.editFlight" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form runat="server">
    <input type="button" value="New Window!" onclick="window.open('http://stackoverflow.com','mywindow','width=400,height=200,toolbar=yes, location=yes,directories=yes,status=yes,menubar=yes,scrollbars=yes,copyhistory=yes, resizable=yes')">
    <asp:Panel ID="flights" runat="server">
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightCompany" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Companhia:</label>
            <asp:TextBox ID="textCompany" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightLogo" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Logo:</label>
            <asp:TextBox ID="textFlightLogo" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightCity" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Cidade:</label>
            <asp:TextBox ID="textFlightCity" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightDescription" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Descrição:</label>
            <asp:TextBox ID="textFlightDescription" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightCabinsPerCity" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Quota:</label>
            <asp:TextBox ID="textFlightCabinsPerCity" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightStatus" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Estado:</label>
            <asp:TextBox ID="textFlightStatus" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightCode" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Código:</label>
            <asp:TextBox ID="textFlightCode" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightTime" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Hora:</label>
            <asp:TextBox ID="textFlightTime" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightTaxes" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Taxas p/pessoa:</label>
            <asp:TextBox ID="textFlightTaxes" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightReturn" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Vôo de retorno:</label>
            <asp:TextBox ID="textFlightReturn" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightReturnTime" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Horario vôo de retorno:</label>
            <asp:TextBox ID="textFlightReturnTime" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightTransfer" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Transfer:</label>
            <asp:TextBox ID="textFlightTransfer" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightHotel" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Hotel:</label>
            <asp:TextBox ID="textFlightHotel" runat="server" CssClass="form-control" />
        </div>
        <div class="entryDataClass">
            <asp:CheckBox ID="selectFlightOfferDescription" runat="server" CssClass="editMultiplePackagesCheckBox" />
            <label>
                Descrição oferta:</label>
            <asp:TextBox ID="textFlightOfferDescription" runat="server" CssClass="form-control" />
        </div>
    </asp:Panel>
    </form>
</body>
</html>
