<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="orden.aspx.cs" Inherits="SAPWEB.orden" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
         <script>
             function solonumeros(e) {

                 var key;

                 if (window.event) // IE
                 {
                     key = e.keyCode;
                 }
                 else if (e.which) // Netscape/Firefox/Opera
                 {
                     key = e.which;
                 }

                 if (key < 48 || key > 57) {
                     if (key != 44) {
                         return false;
                     }
                 }

                 return true;
             }
    </script>

  <style>
      .columna {
          background: #a9a0a0;
          margin-top: 1rem;
      }
  </style>
    <div>
         <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">
              <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
                <h2 class="h2" >N° Orden: <asp:Label ID="lblOrden" runat="server" Text=""></asp:Label><br /> <small><asp:Label ID="lblProceso" runat="server" Text="Label"></asp:Label> </small></h2>
                  <asp:Label ID="lblEmpresa" runat="server" Text="Label" Visible="False"></asp:Label>
              </div>
         </main>
    </div>

     <div class="d-flex  justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">

         <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">

            <div class="row">
                <div class="col-md-12">

                    <div class="row">
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">
                           <div class="">Fecha Planificada</div>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                            <asp:Label ID="lblFecha" runat="server"></asp:Label>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">

                            Estado
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                             <asp:Label ID="lblEstado" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">
                           <div class="">Descripción</div>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                             <asp:Label ID="lblDescrip" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">
                           <div class="">Artículo Planificado</div>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                            <asp:Label ID="lblArticulo" runat="server"></asp:Label>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">

                            Observaciones
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                             <asp:Label ID="lblObservaciones" runat="server" Text=""></asp:Label>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">
                           <div class="">Usuario</div>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                             <asp:Label ID="lblUsuario" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                    <div class="row">
                         <div class="col-md-2 p-3 mb-2 bg-info text-white">
                           <div class="">Consumos</div>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                            <asp:Label ID="lblConsumos" runat="server"></asp:Label>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">

                            Reportes
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                             <asp:Label ID="lblReportes" runat="server" Text="" ></asp:Label>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-info text-white">
                           <div class="">Redimiento Masa</div>
                        </div>
                        <div class="col-md-2 p-3 mb-2 bg-light text-dark">
                             <asp:Label ID="lblRend" runat="server" Text=""></asp:Label>
                        </div>
                    </div>
                </div>

                <div class="col-md-12">
                    <%-- -------- --%>
                  <hr class="mb-4">
         <%--    <div class="row">
                <div class="col-md-12">
    <h3 class="h3  bg-info text-white">COPAC</h3>
                    </div></div>
                 <div class="row" style="	box-shadow: 0px 0px 4px  black;">
                          <div class="col-md-3 mb-3" style=""> <label for="firstName">Horas Proceso</label>
                              <asp:TextBox ID="txtHorasProceso" class="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                          </div>
                          <div class="col-md-3 mb-3" style=""> <label for="lastName">Det.Programadas</label>
                               <asp:TextBox ID="txtDetProgramadas" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                          </div>
                          <div class="col-md-3" style=""><label for="lastName">Det.Operacional</label>
                               <asp:TextBox ID="txtDetOperacional" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                          </div>
                          <div class="col-md-3" style=""><label for="lastName">Det. Maquina</label>
                               <asp:TextBox ID="txtDetMAquina" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                          </div>
                 </div>
                 <div class="row" style="	box-shadow: 0px 0px 4px  black;">
                    <div class="col-md-3 mb-3" style=""> <label for="KiloRechazo1">Kilos Rechazo</label>
                        <asp:TextBox ID="txtKilosRechazados1" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                        <div class="invalid-feedback" style=""> Valid first name is required. </div>
                    </div>
                    <div class="col-md-9 mb-3" style=""> <label for="MotivoRechazo1">Motivo Rechazo</label>
                       <asp:TextBox ID="txtMotivoRechazo1" CssClass="form-control" placeholder=""  runat="server"></asp:TextBox>
                        <div class="invalid-feedback" style=""> Valid last name is required. </div>
                    </div>
                </div>
                 <div class="row" style="	box-shadow: 0px 0px 4px  black;">
              <div class="col-md-3 mb-3" style=""> <label for="KiloRechazo2">Kilos Rechazo 2</label>
                   <asp:TextBox ID="txtKilosRechazados2" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
              </div>
              <div class="col-md-9 mb-3" style=""> <label for="MotivoRechazo2">Motivo Rechazo 2</label>
                   <asp:TextBox ID="txtMotivoRechazo2" CssClass="form-control"  runat="server"></asp:TextBox>
              </div>
            </div>
                 <div class="row" style="	box-shadow: 0px 0px 4px  black;">
                      <div class="col-md-3 mb-3" style=""> <label for="AprobacionGerencia">Ap. Gerencia</label>
                           <asp:TextBox ID="txtAprobacionGerencia" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                        <div class="invalid-feedback" style=""> Valid first name is required. </div>
                      </div>
                      <div class="col-md-3 mb-3" style=""> <label for="Reproceso">Reproceso</label>
                           <asp:TextBox ID="txtReproceso" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                        <div class="invalid-feedback" style=""> Valid last name is required. </div>
                      </div>
                      <div class="col-md-3" style=""><label for="RepPNV">Rep. PNV</label>

                           <asp:TextBox ID="txtRepPNV" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                        <div class="invalid-feedback"> Valid last name is required. </div>
                      </div>
                      <div class="col-md-3" style=""><label for="Redestino">Redestino</label>
                           <asp:TextBox ID="txtRedestino" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                        <div class="invalid-feedback"> Valid last name is required. </div>
                      </div>
                </div>
                <div class="row" style="	box-shadow: 0px 0px 4px  black;">
                         <div class="col-md-3 mb-3" style=""> <label for="MermaFilm">Merma Film</label>
                           <asp:TextBox ID="txtMermaFilm" CssClass="form-control w-50 text-right"  onkeypress="javascript:return solonumeros(event)" runat="server"></asp:TextBox>
                      </div>
                      <div class="col-md-9 mb-3" style=""> <label for="Observaciones">Observaciones</label>
                           <asp:TextBox ID="txtObservaciones" class="form-control"   runat="server"></asp:TextBox>
                      </div>
                </div>--%>
            <hr class="mb-4">
<%--            <asp:Button ID="btnGrabar" runat="server" class="btn btn-info btn-lg btn-block" Text="Grabar Registro" OnClick="btnGrabar_Click" />    --%>

                    <%-- ///// --%>
                </div>
            </div>
         </main>
     </div>
     <div class="d-flex  justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
         <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">
            <div class="row">
                <div class="col-md-12">
                   <div class="row">
                     <div class="col-md-3 p-3 mb-2 bg-danger text-white align-content-center">
                          <div class="text-center col-md-12"> <h2 class="h2">Merma</h2></div>
                          <h4 class="h4 text-center"> <asp:Label ID="lblMerma" runat="server"></asp:Label></h4>
                         <asp:Label ID="lblBasuraPepa" runat="server"></asp:Label>
                     </div>

                     <div class="col-md-3 p-3 mb-2 bg-warning text-white">
                           <div class="text-center"><h2 class="h2">Descarte</h2></div>
                           <h4 class="h4 text-center"><asp:Label ID="lblDescarte" runat="server"></asp:Label></h4>
                     </div>

                     <div class="col-md-3 p-3 mb-2 bg-success text-white">
                           <div class="text-center col-md-12"><h2 class="h2">Fruta</h2></div>
                         <h4 class="h4 text-center"> <asp:Label ID="lblFruta" runat="server"></asp:Label></h4>
                     </div>

                       <div class="col-md-3 p-3 mb-2 bg-info text-white">
                           <div class="text-center col-md-12"><h2 class="h2">Kilos/Hora</h2></div>
                         <h4 class="h4 text-center"> <asp:Label ID="lblKilosHora" runat="server"></asp:Label></h4>
                     </div>
     </div>
                </div>
            </div>
         </main>
     </div>

<div>
    <div class="row alert-info col-md-12">

         <div class="row">
            <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
               <%--  <div class="col-md-6 form-inline text-center">--%>
              <div> <h3 class="h3">&nbsp;&nbsp;&nbsp; Consumos&nbsp;&nbsp;&nbsp;  </h3>       </div>
                <asp:Button ID="btnExpConsumos" runat="server" Text="Exportar" CssClass="btn btn-info"  Width="160" />
            </div>

            <div class="col-md-6">
                </div>
        </div>
    </div>
      <div class="d-flex  justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
         <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">
           <div class="row">
                <div class="col-md-12">

                    <asp:GridView ID="GridConsumos" runat="server" AllowPaging = "true" CssClass = "table table-responsive table-striped table-hover" AutoGenerateColumns="false" PageSize="10" ShowFooter="True">
                        <Columns>
                            <asp:BoundField DataField="sysdate" HeaderText="Fecha" SortExpression="sysdate" />
                            <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                            <asp:BoundField DataField="linea" HeaderText="Linea" SortExpression="linea" />
                            <asp:BoundField DataField="WhsCode" HeaderText="Bodega" SortExpression="WhsCode" />
                            <asp:BoundField DataField="turno" HeaderText="Turno" SortExpression="turno" />
                            <asp:BoundField DataField="Cosecha" HeaderText="Cosecha" SortExpression="Cosecha" />
                            <asp:BoundField DataField="ItemCode" HeaderText="Artículo" SortExpression="ItemCode" />
                            <asp:BoundField DataField="TIPOFRUTA" HeaderText="TIPO" SortExpression="ItemCode" />
                            <asp:BoundField DataField="kilos" HeaderText="Kilos" SortExpression="kilos" />
                            <asp:BoundField DataField="Calibre" HeaderText="Calibre" SortExpression="Calibre" />
                            <asp:BoundField DataField="MasterCalibre" HeaderText="M.Calibre" SortExpression="MasterCalibre" />
                            <asp:BoundField DataField="CardName" HeaderText="Productor" SortExpression="Productor" />
                        </Columns>
                    </asp:GridView>
                </div>
           </div>
        </main>
      </div>
</div>

<div>
    <div class="row alert-info col-md-12"">
         <div class="row">
              <div class="d-flex justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
               <%--  <div class="col-md-6 form-inline text-center">--%>

               <div> <h3 class="h3">&nbsp;&nbsp;&nbsp;Reportes&nbsp;&nbsp;&nbsp;  </h3>       </div>
                <asp:Button ID="Button1" runat="server" Text="Exportar" CssClass="btn btn-info"  Width="160" />
            </div>
            <div class="col-md-9">
                </div>
        </div>
    </div>
      <div class="d-flex  justify-content-between flex-wrap flex-md-nowrap align-items-center pt-3 pb-2 mb-3 border-bottom">
         <main role="main" class="col-md-12 ml-sm-auto col-lg-12 px-4">
           <div class="row">
                <div class="col-md-12">
                    <asp:GridView ID="GridReportes" runat="server" AllowPaging = "true" CssClass = "table table-responsive table-striped table-hover" AutoGenerateColumns="false" PageSize="10" ShowFooter="True">
                        <Columns>
                            <asp:BoundField DataField="sysdate" HeaderText="Fecha" SortExpression="sysdate" />
                            <asp:BoundField DataField="Lote" HeaderText="Lote" SortExpression="Lote" />
                            <asp:BoundField DataField="linea" HeaderText="Linea" SortExpression="linea" />
                            <asp:BoundField DataField="WhsCode" HeaderText="Bodega" SortExpression="WhsCode" />
                            <asp:BoundField DataField="turno" HeaderText="Turno" SortExpression="turno" />
                            <asp:BoundField DataField="Cosecha" HeaderText="Cosecha" SortExpression="Cosecha" />
                            <asp:BoundField DataField="ItemCode" HeaderText="Artículo" SortExpression="ItemCode" />
                             <asp:BoundField DataField="TIPOFRUTA" HeaderText="TIPO" SortExpression="ItemCode" />
                            <asp:BoundField DataField="kilos" HeaderText="Kilos" SortExpression="kilos" />
                            <asp:BoundField DataField="Calibre" HeaderText="Calibre" SortExpression="Calibre" />
                            <asp:BoundField DataField="MasterCalibre" HeaderText="M.Calibre" SortExpression="MasterCalibre" />
                            <asp:BoundField DataField="CardName" HeaderText="Productor" SortExpression="Productor" />
                        </Columns>
                    </asp:GridView>
                </div>
           </div>
        </main>
      </div>
</div>
</asp:Content>