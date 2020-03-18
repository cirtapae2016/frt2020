<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="consumos.aspx.cs" Inherits="SAPWEB.consumos" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <table class="nav-justified">
        <tr>
            <td>Orden de Fabricación</td>
            <td>Lote</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
        <tr>
            <td>
                <asp:DropDownList ID="DrpListOrden" runat="server" Height="16px" Width="278px">
                </asp:DropDownList>
            </td>
            <td>
                <asp:TextBox ID="txtConsumoLote" runat="server" Width="307px" Font-Bold="True" Font-Size="XX-Large" Height="54px" OnTextChanged="txtConsumoLote_TextChanged"></asp:TextBox>
            </td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
        </tr>
    </table>
</asp:Content>