<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ConsOF.aspx.cs" Inherits="SAPWEB.ConsOF" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <%-- <link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css">--%>
    <%-- <h1 class="h1" >Procesadora</h1>
<div class="jumbotron">
</div>--%>
    <div class="jumbotron">
        <div class="row w-100">
            <div class="col-md-12">
                <div class="card border-info mx-sm-1 p-3">
                    <div class="card border-info shadow text-info p-3 my-card ">
                        <span class="fa fa-bell-o" aria-hidden="true"></span>
                        <div class="text-center">
                            <h3>RESUMEN ORDENES FABRICACIÓN</h3>
                        </div>
                    </div>
                    <br />
                    <div class="row w-100">

                        <div class="col-md-3">
                            <div class="card border-info mx-sm-1 p-3">
                                <div class="card border-info shadow text-info p-3 my-card ">
                                    <span class="fa fa-bell-o" aria-hidden="true"></span>
                                    <div class="text-center">
                                        <asp:Button ID="btnVerTotal" runat="server" CssClass="btn btn-info" Width="100" Text="Ver" OnClick="btnVerTotal_Click" />
                                    </div>
                                </div>
                                <div class="text-info text-center mt-3">
                                    <h4>OF's </h4>
                                </div>
                                <div class="text-info text-center mt-2">
                                    <h1>
                                        <asp:Label ID="lblOrdenes" runat="server" Text="Label"></asp:Label>
                                    </h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="card border-success mx-sm-1 p-3">
                                <div class="card border-success shadow text-success p-3 my-card">
                                    <span class="fa fa-bell-o" aria-hidden="true"></span>
                                    <div class="text-center">
                                        <asp:Button ID="btnVerCerradas" runat="server" CssClass="btn btn-success" Width="100" Text="Ver" OnClick="btnVerCerradas_Click" />
                                    </div>
                                </div>
                                <div class="text-success text-center mt-3">
                                    <h4>Cerradas</h4>
                                </div>
                                <div class="text-success text-center mt-2">
                                    <h1>
                                        <asp:Label ID="lblCerradas" runat="server" Text="Label"></asp:Label>
                                    </h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="card border-danger mx-sm-1 p-3">
                                <div class="card border-danger shadow text-danger p-3 my-card">
                                    <span class="fa fa-bell" aria-hidden="true"></span>
                                    <div class="text-center">
                                        <asp:Button ID="btnVerAbiertas" runat="server" CssClass="btn btn-danger" Width="100" Text="Ver" OnClick="btnVerAbiertas_Click" />
                                    </div>
                                </div>
                                <div class="text-danger text-center mt-3">
                                    <h4>Abiertas</h4>
                                </div>
                                <div class="text-danger text-center mt-2">
                                    <h1>
                                        <asp:Label ID="lblAbiertas" runat="server" Text="Label"></asp:Label></h1>
                                </div>
                            </div>
                        </div>
                        <div class="col-md-3">
                            <div class="card border-warning mx-sm-1 p-3">
                                <div class="card border-warning shadow text-warning p-3 my-card">
                                    <span class="fa fa-bell-slash" aria-hidden="true"></span>
                                    <div class="text-center">
                                        <asp:Button ID="btnVerShoras" runat="server" CssClass="btn btn-warning" Width="100" Text="Ver" OnClick="btnVerShoras_Click" />
                                    </div>
                                </div>
                                <div class="text-warning text-center mt-3">
                                    <h4>Planificadas</h4>
                                </div>
                                <div class="text-warning text-center mt-2">
                                    <h1>
                                        <asp:Label ID="lblSinHoras" runat="server" Text="Label"></asp:Label></h1>
                                </div>
                            </div>
                        </div>
                    </div>
                    <br />
                    <%--                         <div class="row w-100">
                             <div class="col-md-3">
                                <div class="card border-info mx-sm-1 p-3">
                                    <div class="card border-info shadow text-info p-3 my-card " ><span class="fa fa-bell-o" aria-hidden="true">    </span><div class="text-center"><asp:Button ID="btnVerOrdenesClientes_Pr" runat="server" CssClass="btn btn-info" Width="100" Text="Ver"  /></div></div>
                                    <div class="text-info text-center mt-3"><h4>OV's </h4></div>
                                    <div class="text-info text-center mt-2"><h1><asp:Label ID="lblOrdenesClientes" runat="server" Text="Label"></asp:Label> </h1></div>
                                </div>
                             </div>
                         </div>--%>
                </div>
            </div>
        </div>
    </div>

    <%-- <div class="jumbotron">
        <div class="row w-100">
                <div class="col-md-12">
                    <div class="card border-info mx-sm-1 p-3">
                        <div class="card border-info shadow text-info p-3 my-card " ><span class="fa fa-bell-o" aria-hidden="true">    </span><div class="text-center"><h3>PASERA</h3></div></div>
                            <br />
                            <div class="row w-100">
                                <div class="col-md-3">
                                    <div class="card border-info mx-sm-1 p-3">
                                        <div class="card border-info shadow text-info p-3 my-card " ><span class="fa fa-bell-o" aria-hidden="true">    </span><div class="text-center"><asp:Button ID="btnVerOrdenesPasera" runat="server" CssClass="btn btn-info" Width="100" Text="Ver" /></div></div>
                                        <div class="text-info text-center mt-3"><h4>Total </h4></div>
                                        <div class="text-info text-center mt-2"><h1><asp:Label ID="lblOrdenesPasera" runat="server" Text="Label"></asp:Label> </h1></div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="card border-success mx-sm-1 p-3">
                                        <div class="card border-success shadow text-success p-3 my-card"><span class="fa fa-bell-o" aria-hidden="true"></span><div class="text-center"><asp:Button ID="btnVerCerradasPsaera" runat="server" CssClass="btn btn-success" Width="100" Text="Ver" /></div></div>
                                        <div class="text-success text-center mt-3"><h4>Cerradas</h4></div>
                                        <div class="text-success text-center mt-2"><h1><asp:Label ID="lblCerradasPasera" runat="server" Text="Label"></asp:Label> </h1></div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="card border-danger mx-sm-1 p-3">
                                        <div class="card border-danger shadow text-danger p-3 my-card" ><span class="fa fa-bell" aria-hidden="true"></span><div class="text-center"><asp:Button ID="btnVerAbiertasPasera" runat="server" CssClass="btn btn-danger" Width="100" Text="Ver"  /></div></div>
                                        <div class="text-danger text-center mt-3"><h4>Abiertas</h4></div>
                                        <div class="text-danger text-center mt-2"><h1><asp:Label ID="lblAbiertasPasera" runat="server" Text="Label"></asp:Label></h1></div>
                                    </div>
                                </div>
                                <div class="col-md-3">
                                    <div class="card border-warning mx-sm-1 p-3">
                                        <div class="card border-warning shadow text-warning p-3 my-card" ><span class="fa fa-bell-slash" aria-hidden="true"></span><div class="text-center"><asp:Button ID="btnVerSinHorasPasera" runat="server" CssClass="btn btn-warning" Width="100" Text="Ver" /></div></div>
                                        <div class="text-warning text-center mt-3"><h4>Sin Horas</h4></div>
                                        <div class="text-warning text-center mt-2"><h1> <asp:Label ID="lblSinHorasPasera" runat="server" Text="Label"></asp:Label></h1></div>
                                    </div>
                                </div>
                             </div>
                             <br />
                             <div class="row w-100">

                                     <div class="col-md-3">
                                        <div class="card border-info mx-sm-1 p-3">
                                            <div class="card border-info shadow text-info p-3 my-card " ><span class="fa fa-bell-o" aria-hidden="true">    </span><div class="text-center"><asp:Button ID="btnVerOVPasera" runat="server" CssClass="btn btn-info" Width="100" Text="Ver" /></div></div>
                                            <div class="text-info text-center mt-3"><h4>OV's </h4></div>
                                            <div class="text-info text-center mt-2"><h1><asp:Label ID="lblOrdenesVenta_Pas" runat="server" Text="Label"></asp:Label> </h1></div>
                                        </div>
                                     </div>
                                 </div>
                     </div>
                </div>
         </div>
 </div>
    --%>
</asp:Content>