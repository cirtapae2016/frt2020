<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="resumen.aspx.cs" Inherits="SAPWEB.resumen" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div>
        <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">
            <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                <h2 class="h2">
                    <asp:Label ID="lblProceso" runat="server" Text="Label"></asp:Label><br />
                    <small>
                        <asp:Label ID="lblprocesoDesc" runat="server" Text="Label"></asp:Label>
                    </small></h2>
                <br />
                <div>
                    <asp:Button ID="btnExpConsumos" runat="server" Text="Exportar" CssClass="btn btn-info" Width="160" OnClick="btnExpConsumos_Click" />
                </div>
            </div>
        </main>
    </div>
    <div class="d-flex  justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
        <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">
            <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="GridResumen" runat="server" AutoGenerateColumns="False" AllowPaging="true" CssClass="table table-responsive table-striped table-hover" PageSize="20" ShowFooter="True">
                        <Columns>
                            <asp:HyperLinkField DataNavigateUrlFields="url" DataTextField="OF1" HeaderText="Orden" />
                            <asp:BoundField DataField="FechaProgramada" HeaderText="Fecha" />
                            <asp:BoundField DataField="DiasAbierta" HeaderText="Dias" />
                            <asp:BoundField DataField="Estado" HeaderText="Estado" />
                            <asp:BoundField DataField="Especie" HeaderText="Especie" />
                            <asp:BoundField DataField="Articulo" HeaderText="Articulo" />
                            <asp:BoundField DataField="Planificado" HeaderText="Planificado" />
                        </Columns>
                    </asp:GridView>
                </div>
            </div>
        </main>
    </div>
</asp:Content>