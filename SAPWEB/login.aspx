<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="login.aspx.cs" Inherits="SAPWEB.login" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <br />
        <br />
        <asp:Label ID="Label1" runat="server" Text="Usuario"></asp:Label><br />
        <asp:TextBox ID="txtUsuario" runat="server"></asp:TextBox>
        <br />
        <asp:Label ID="Label2" runat="server" Text="Clave"></asp:Label><br />
        <asp:TextBox ID="txtClave" runat="server" TextMode="Password"></asp:TextBox>
        <br />
        <br />
        <asp:DropDownList ID="drpList" runat="server" Height="33px" Width="170px"></asp:DropDownList>
        <br />
        <asp:Button ID="btnConectar" runat="server" Text="Login SAP" OnClick="btnConectar_Click" />
        <br />
        <asp:Label ID="Label3" runat="server" Text="Label"></asp:Label>
    </form>
</body>
</html>