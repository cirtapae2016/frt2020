using Sap.Data.Hana;
using System;
using System.Data;
using System.Data.SqlClient;

namespace SAPWEB
{
    public class utiles

    {
        public string Empresa;
        public string Usuario;
        public string Clave;
        public SAPbobsCOM.Company Empresa_SAP;
        public bool Conectado;
        public string EstadoConexion;

        public utiles(string EMPRESA, string USUARIO, string CLAVE)
        {
            this.Empresa = EMPRESA;
            this.Usuario = USUARIO;
            this.Clave = CLAVE;

            Connect_usuario();
        }

        private bool Connect_usuario()
        {
            SAPbobsCOM.Company Company = new SAPbobsCOM.Company();
            Company.Server = "hana:30015";
            Company.LicenseServer = "sapb1:40000";
            Company.UseTrusted = false;
            Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            Company.DbUserName = "SYSTEM";
            Company.DbPassword = "SAPB1_Admin!!";
            Company.CompanyDB = this.Empresa;
            Company.UserName = this.Usuario;
            Company.Password = this.Clave;
            Company.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;
            Company.AddonIdentifier = string.Empty;

            if (Company.Connect() != 0)
            {
                this.EstadoConexion = Company.GetLastErrorDescription();
                Company = null;
                this.Conectado = false;
            }
            else
            {
                Company.XMLAsString = true;
                Company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                this.Empresa_SAP = Company;
                this.Conectado = true;
            }

            return this.Conectado;
        }

        //public string Salida_Mercaderia_CO1(cldOrdenFabricacion parConsumo)
        //{
        //    string NewObjectKey;

        //    SAPbobsCOM.Company empresa = this.Empresa_SAP;

        //    ///// crear los lotes para las cajas
        //    /////

        //    SAPbobsCOM.Documents SalidaMercancia;
        //    SalidaMercancia = (SAPbobsCOM.Documents)empresa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit);

        //    SalidaMercancia.Series = 26;
        //    SalidaMercancia.DocDate = DateTime.ParseExact(parConsumo.DocDate, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        //    //SalidaMercancia.TaxDate = DateTime.ParseExact(fecha, "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
        //    SalidaMercancia.Comments = "CONSUMO PARA PRODUCCION";
        //    SalidaMercancia.JournalMemo = "CONSUMO PARA PRODUCCION";

        //    //SalidaMercancia.Lines.ItemCode = RS.Fields.Item("ItemCode").Value.ToString();
        //    SalidaMercancia.Lines.Quantity = parConsumo.Quantity;
        //    SalidaMercancia.Lines.BaseType = 202;
        //    SalidaMercancia.Lines.BaseEntry = parConsumo.DocEntry;
        //    SalidaMercancia.Lines.BaseLine = parConsumo.LineNum;
        //    SalidaMercancia.Lines.WarehouseCode = parConsumo.Warehouse;
        //    //SalidaMercancia.Lines.Price = 1.1;
        //    //SalidaMercancia.Lines.UnitPrice = 1.1;

        //    /////////////////////////////////////////////////
        //    /////////////////////////////////////////////////
        //    // iniciamos transaccion

        //    int errCode;
        //    string errMsg;

        //    errMsg = "";
        //    errCode = 0;
        //    NewObjectKey = "s";

        //    try
        //    {
        //        empresa.StartTransaction();

        //        if (SalidaMercancia.Add() == 0)
        //        {
        //            errCode = 0;

        //            NewObjectKey = empresa.GetNewObjectKey();

        //            if (empresa.InTransaction)
        //                empresa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

        //        }
        //        else
        //        {
        //            empresa.GetLastError(out errCode, out errMsg);

        //            if (empresa.InTransaction)
        //                empresa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

        //            NewObjectKey = errMsg;

        //        }

        //    }
        //    catch (Exception)
        //    {
        //        if (empresa.InTransaction)
        //            empresa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

        //        empresa.GetLastError(out errCode, out errMsg);

        //        NewObjectKey = errMsg;

        //    }

        //    return NewObjectKey;

        //}
    }

    public class clsResumenOF

    {
        #region Constructor

        public clsResumenOF(SAPbobsCOM.Company Empresa, string BD)
        {
            cRecordSet rs = new cRecordSet("select * from \"" + BD + "\".\"OWOR\" ", Empresa);
            if (rs.RS.RecordCount > 0)
            {
                this.Ofs = rs.RS.RecordCount;
            }

            cRecordSet rs1 = new cRecordSet("select * from \"" + BD + "\".\"OWOR\" where \"Status\"='L' ", Empresa);
            if (rs1.RS.RecordCount > 0)
            {
                this.Cerradas = rs1.RS.RecordCount;
            }

            cRecordSet rs2 = new cRecordSet("select * from \"" + BD + "\".\"OWOR\" where \"Status\"='R' ", Empresa);
            if (rs2.RS.RecordCount > 0)
            {
                this.Abiertas = rs2.RS.RecordCount;
            }

            cRecordSet rs3 = new cRecordSet("select * from \"" + BD + "\".\"OWOR\" where \"Status\"='P' ", Empresa);
            if (rs3.RS.RecordCount > 0)
            {
                this.SinHoras = rs3.RS.RecordCount;
            }
        }

        #endregion Constructor

        #region Propiedades

        public int Ofs;
        public int Cerradas;
        public int Abiertas;
        public int SinHoras;

        #endregion Propiedades
    }

    public class clsOrdenFabricacion : HanaCon

    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string PostDate { get; set; }
        public string StartDate { get; set; }
        public string DueDate { get; set; }
        public string DocDate { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string UM { get; set; }
        public int UserSign { get; set; }
        public string Warehouse { get; set; }
        public double PlannedQty { get; set; }
        public int OriginNum { get; set; }
        public string CardCode { get; set; }
        public string Status { get; set; }
        public string Project { get; set; }
        public string Comments { get; set; }
        public string Type { get; set; }
        public string U_Proceso { get; set; }
        public string U_OrdenAfecta { get; set; }
        public string U_TipoOrden { get; set; }
        public string U_TipoFruta { get; set; }
        public int LineNum { get; set; }
        public string ItemCode_D { get; set; }
        public string ItemName_D { get; set; }
        public double BaseQty { get; set; }
        public double PlannedQty_D { get; set; }
        public string Warehouse_D { get; set; }
        public double IssuedQty { get; set; }

        public string lote { get; set; }

        public double Quantity { get; set; }
        public string UsuarioSap { get; set; }
        public string ClaveSap { get; set; }

        public string[,] arrDetalle { get; set; }

        public string[,] arrDetalle1 { get; set; }

        public string CodigoNumeroPallet { get; set; }
        public int CantidadBins { get; set; }
        public string CodeProveedor { get; set; }
        public string NombreProveedor { get; set; }
        public string CodeCliente { get; set; }
        public string NombreCliente { get; set; }
        public string SalidaProduccion { get; set; }
        public string Conexion_SAP { get; set; }
        public int Lote { get; set; }
        public string Calibre { get; set; }
        public string Variedad { get; set; }
        public string Turno { get; set; }
        public string Area { get; set; }
        public int User_Autorizador { get; set; }
        public int DocEntryRef { get; set; }
        public DataTable OrdenesAbiertas;

        public clsOrdenFabricacion(string _EMPRESA)
        {
            this.cSql = "select \"DocEntry\", \"DueDate\"  from \"" + _EMPRESA + "\".\"OWOR\" where \"Status\"='R'";
            this.OrdenesAbiertas = this.PopulateDT();
        }

        public clsOrdenFabricacion(string _EMPRESA, int _DocEntry)
        {
        }
    }

    //public class clsOrdenFabricacion2 : cRecordSet
    //{
    //    public string OrdenFabricacion;
    //    public string Empresa;
    //   // public clsCliente oCliente;
    //    public int Oof;
    //    public string FechaProgramada;
    //    public string Descripcion;
    //    public string Observaciones;
    //    public string Item;
    //    public string Nombre;
    //    public string ItemNombre;
    //    public string Usuario;
    //    public double KilosPlanificados;
    //    public double Consumos;
    //    public double Reportes;
    //    public double Rendimiento;
    //    public double Merma;
    //    public double Pepa;
    //    public double Basura;
    //    public double Fruta;
    //    public double Descarte;
    //    public double SinIdentificar;
    //    public double RendimientoFruta;
    //    public string KilosHora;
    //    public string TiempoTranscurrido;
    //    public string Estado;
    //    public string P_Rendiminento_Masa;
    //    public string P_Rendimiento_Fruta;
    //    public string P_Rendimiento_Descarte;
    //    public string P_Redimiento_Merma;
    //    public string P_Rendimiento_Pepa;
    //    public string P_Rendimiento_Basura;
    //    public string Proceso_Productivo;
    //    public string CodigoCliente;
    //  //  public clsOrdenHana oHana;
    //    public DateTime FechaUltimoEgreso;
    //    public DateTime FechaEjecucion;
    //    public string[] TipoFrutas;
    //    public double[] KilosFrutas;
    //    public DataTable ConsumosDatos;
    //    public DataTable ReportesDatos;
    //    public DataTable RegistrosCompletos;
    //    private DataTable Cabecera;
    //    public DataTable ResumenOF;
    //    const string sqlCabecera = "select * from \"vista_OrdenesFabricacion\" ";
    //    const string sqlCabecera_2 = "select * from [dbo].[vista_produccion_SAP_FINAL]";
    //    const string sqlCondicionAbierta = " where Estado='Abierta'";
    //    const string sqlTodasLasOrdenes = "select ot1 as OF1,fechaProgramada, item as itemOF, nombre as NombreOF, usuario as usuarioOF, ot_descri, ot_observa, cantidad as CantidadOF, estado as EstadoOF, Especie, datediff ( dd, FechaProgramada, GETDATE()  ) as DiasOF, CodigoCliente from vista_OrdenesFabricacion ";

    //    public clsOrdenFabricacion2()
    //    {
    //        //this.cSql = "select OT from vista_OrdenesFabricacion";
    //        //this.RegistrosCompletos = this.PopulateDT();
    //    }

    //    public clsOrdenFabricacion2(int vOrden, SAPbobsCOM.Company Empresa)
    //    {
    //        this.Oof = vOrden;
    //        this.OrdenFabricacion = vOrden.ToString();
    //        //this.cSql = sqlCabecera_2 + " where OrdenFabricacion =" + this.OrdenFabricacion.ToString() + " and Empresa='"+this.Empresa+"'";
    //        this.CargaOrden();
    //    }

    //    private void CargaOrden()
    //    {
    //        this.cSql = sqlTodasLasOrdenes + "where  ot1 =" + this.OrdenFabricacion.ToString() + " and Empresa='" + this.Empresa + "'";
    //        this.Cabecera = this.PopulateDT();
    //        this.oHana = new clsOrdenHana(this.Oof, this.Empresa);

    //        if (this.Cabecera.Rows.Count > 0)
    //        {
    //            this.FechaProgramada = this.Cabecera.Rows[0]["fechaprogramada"].ToString();
    //            this.Item = this.Cabecera.Rows[0]["itemOF"].ToString();
    //            this.Nombre = this.Cabecera.Rows[0]["nombreOF"].ToString();
    //            this.ItemNombre = this.Cabecera.Rows[0]["itemOF"].ToString() + " - " + this.Cabecera.Rows[0]["nombreOF"].ToString();
    //            this.Usuario = this.Cabecera.Rows[0]["usuarioOF"].ToString();
    //            this.Descripcion = this.Cabecera.Rows[0]["ot_descri"].ToString();
    //            this.Observaciones = this.Cabecera.Rows[0]["ot_observa"].ToString();
    //            this.KilosPlanificados = double.Parse(this.Cabecera.Rows[0]["cantidadOF"].ToString());
    //            this.Estado = this.Cabecera.Rows[0]["EstadoOF"].ToString();
    //            this.FechaEjecucion = DateTime.Now;
    //            this.CodigoCliente = this.Cabecera.Rows[0]["CodigoCliente"].ToString();
    //            this.oCliente = new clsCliente(this.CodigoCliente);

    //            this.cSql = sqlCabecera_2 + " where OrdenFabricacion =" + this.OrdenFabricacion.ToString() + " and Empresa='" + this.Empresa + "'";
    //            this.Cabecera = this.PopulateDT();
    //            this.RegistrosCompletos = this.Cabecera;
    //            DataTable Cons = new DataTable();
    //            DataTable Rept = new DataTable();
    //            Cons = this.RegistrosCompletos.Clone();
    //            Rept = this.RegistrosCompletos.Clone();
    //            if (this.Cabecera.Rows.Count > 0)
    //            {
    //                this.Proceso_Productivo = this.Cabecera.Rows[0]["PROCESO_PRODUCTIVO"].ToString();

    //                DateTime vFecha;
    //                foreach (DataRow row in this.RegistrosCompletos.Rows)
    //                {
    //                    switch (row["TipoDocto"])
    //                    {
    //                        case "ConsumoProduccion":
    //                            this.Consumos += double.Parse(row["kilos"].ToString());
    //                            Cons.ImportRow(row);
    //                            break;
    //                        case "ReporteProduccion":
    //                            this.Reportes += double.Parse(row["kilos"].ToString());
    //                            Rept.ImportRow(row);
    //                            vFecha = Convert.ToDateTime(row["sysdate"].ToString());
    //                            if (this.FechaUltimoEgreso != Convert.ToDateTime("01-01-0001 0:00:00"))
    //                            {
    //                                int result = DateTime.Compare(this.FechaUltimoEgreso, vFecha);
    //                                if (result > 0)
    //                                {
    //                                    this.FechaUltimoEgreso = vFecha;
    //                                }
    //                            }
    //                            else
    //                            {
    //                                this.FechaUltimoEgreso = vFecha;
    //                            }

    //                            switch (row["TIPOFRUTA"].ToString())
    //                            {
    //                                case "FRUTA":
    //                                    this.Fruta += double.Parse(row["kilos"].ToString());
    //                                    break;
    //                                case "DESCARTE":
    //                                    this.Descarte += double.Parse(row["kilos"].ToString());
    //                                    break;
    //                                case "MERMA":
    //                                    this.Merma += double.Parse(row["kilos"].ToString());
    //                                    switch (row["ItemCode"].ToString())
    //                                    {
    //                                        case "PTC":
    //                                            this.Pepa += double.Parse(row["kilos"].ToString());
    //                                            break;
    //                                        case "BTC":
    //                                            this.Basura += double.Parse(row["kilos"].ToString());
    //                                            break;
    //                                        case "BASCAL":
    //                                            this.Basura += double.Parse(row["kilos"].ToString());
    //                                            break;
    //                                    }
    //                                    break;
    //                                default:
    //                                    this.SinIdentificar += double.Parse(row["kilos"].ToString());
    //                                    break;
    //                            }
    //                            break;
    //                    }
    //                }
    //                this.ConsumosDatos = Cons.Copy();
    //                this.ReportesDatos = Rept.Copy();
    //                this.Rendimiento = (this.Reportes / this.Consumos);
    //                this.RendimientoFruta = (this.Fruta / this.Consumos);
    //                this.P_Rendiminento_Masa = this.Rendimiento.ToString("p02");
    //                this.P_Rendimiento_Fruta = this.RendimientoFruta.ToString("p02");
    //                double Desc_Cons = (this.Descarte / this.Consumos);
    //                this.P_Rendimiento_Descarte = Desc_Cons.ToString("p02");
    //                double Merma_Cons = (this.Merma / this.Consumos);
    //                this.P_Redimiento_Merma = Merma_Cons.ToString("p02");
    //                double Pepa_Merma = (this.Pepa / this.Consumos);
    //                this.P_Rendimiento_Pepa = Pepa_Merma.ToString("p02");
    //                double Basura_Merma = (this.Basura / this.Consumos);
    //                this.P_Rendimiento_Basura = Basura_Merma.ToString("p02");
    //                TimeSpan span = (this.FechaEjecucion - this.FechaUltimoEgreso);
    //                this.TiempoTranscurrido = String.Format(" {0} dias, {1} horas, {2} minutos, {3} segundos", span.Days, span.Hours, span.Minutes, span.Seconds);

    //                if (this.oHana.HorasProceso > 0)
    //                {
    //                    double vKhora = this.Reportes / (this.oHana.HorasProceso - (this.oHana.DetencionesMaquina + this.oHana.DetencionesOperacionales + this.oHana.DetencionesProgramadas));
    //                    this.KilosHora = vKhora.ToString("f2");
    //                }

    //            }
    //        }
    //    }

    //    public clsOrdenFabricacion(string Orden)
    //    {
    //        this.Empresa = Orden.Substring(0, Orden.IndexOf("-") - 1);
    //        this.Oof = int.Parse(Orden.Substring(Orden.IndexOf("-") + 2, (Orden.IndexOf("/") - 2) - (Orden.IndexOf("-") + 2) + 1));
    //        this.OrdenFabricacion = Orden;
    //        this.cSql = sqlCabecera + " where OtEmpresa ='" + Orden.ToString() + "'";
    //        this.Cabecera = this.PopulateDT();
    //        foreach (DataRow Drow in this.Cabecera.Rows)
    //        {
    //            this.FechaProgramada = Drow["fechaprogramada"].ToString();
    //            this.Item = Drow["item"].ToString();
    //            this.Nombre = Drow["nombre"].ToString();
    //            this.ItemNombre = Drow["item"].ToString() + " - " + Drow["nombre"].ToString();
    //            this.Usuario = Drow["usuario"].ToString();
    //            this.Descripcion = Drow["ot_descri"].ToString();
    //            this.Observaciones = Drow["ot_observa"].ToString();
    //            this.KilosPlanificados = double.Parse(Drow["cantidad"].ToString());

    //        }

    //        string vista;
    //        if (this.Empresa == "Procesadora")
    //        {
    //            vista = "  [dbo].[vista_produccion_procesadora]";
    //        }
    //        else
    //        {
    //            vista = "  [dbo].[vista_produccion_pasera]";
    //        }

    //        this.cSql = "  select TipoDocto,sysdate, Lote, linea, WhsCode, turno, kilos, Cosecha,Nombre, Calibre, MasterCalibre, isnull(TIPOFRUTA, '') as TIPOFRUTA, CardName as Productor  from " + vista + "  where OrdenFabricacion=" + this.Oof.ToString();

    //        this.RegistrosCompletos = this.PopulateDT();

    //        DataTable Cons = new DataTable();
    //        DataTable Rept = new DataTable();
    //        Cons = this.RegistrosCompletos.Clone();
    //        Rept = this.RegistrosCompletos.Clone();
    //        foreach (DataRow row in this.RegistrosCompletos.Rows)
    //        {
    //            switch (row["TipoDocto"])
    //            {
    //                case "ConsumoProduccion":
    //                    this.Consumos += double.Parse(row["kilos"].ToString());
    //                    Cons.ImportRow(row);

    //                    break;
    //                case "ReporteProduccion":
    //                    this.Reportes += double.Parse(row["kilos"].ToString());
    //                    Rept.ImportRow(row);
    //                    switch (row["TIPOFRUTA"].ToString())
    //                    {
    //                        case "FRUTA":
    //                            this.Fruta += double.Parse(row["kilos"].ToString());
    //                            break;
    //                        case "DESCARTE":
    //                            this.Descarte += double.Parse(row["kilos"].ToString());
    //                            break;
    //                        case "MERMA":
    //                            this.Merma += double.Parse(row["kilos"].ToString());
    //                            break;
    //                        default:
    //                            this.SinIdentificar += double.Parse(row["kilos"].ToString());
    //                            break;
    //                    }

    //                    break;
    //            }

    //        }

    //        this.ConsumosDatos = Cons.Copy();
    //        this.ReportesDatos = Rept.Copy();

    //        //this.ConsumosDatos.Columns.Remove("TipoDocto");
    //        //this.ReportesDatos.Columns.Remove("TipoDocto");

    //        //foreach (DataRow Drow in this.ConsumosDatos.Rows)
    //        //{
    //        //    this.Consumos += double.Parse(Drow["kilos"].ToString());

    //        //}

    //        //this.cSql = "  select sysdate, Lote, linea, WhsCode, turno, kilos, Nombre, Calibre, TIPOFRUTA as [Tipo Fruta] from [dbo].[vista_produccion_SAP_FINAL] where Oorden='" + Orden + "'";
    //        //this.cSql = this.cSql + " and TipoDocto='ReporteProduccion' ";
    //        //this.ReportesDatos = this.PopulateDT();
    //        //foreach (DataRow Drow in this.ReportesDatos.Rows)
    //        //{
    //        //    this.Reportes += double.Parse(Drow["kilos"].ToString());

    //        //    switch (Drow[8].ToString())
    //        //    {
    //        //        case "FRUTA":
    //        //            this.Fruta += double.Parse(Drow["kilos"].ToString());
    //        //            break;
    //        //        case "DESCARTE":
    //        //            this.Descarte += double.Parse(Drow["kilos"].ToString());
    //        //            break;
    //        //        case "MERMA":
    //        //            this.Merma += double.Parse(Drow["kilos"].ToString());
    //        //            break;
    //        //        default:
    //        //            this.SinIdentificar += double.Parse(Drow["kilos"].ToString());
    //        //            break;
    //        //    }

    //        //}

    //        this.Rendimiento = (this.Reportes / this.Consumos) * 100;
    //        this.RendimientoFruta = (this.Fruta / this.Consumos) * 100;

    //        //this.cSql = "  select TipoDocto, isnull(TIPOFRUTA,'No Definido') as TIPOFRUTA, sum(kilos) as Kilos from [dbo].[vista_produccion_SAP_FINAL] where Oorden='" + Orden + "'";
    //        //this.cSql = this.cSql + "  and tipoDocto='ReporteProduccion' group by TipoDocto,TIPOFRUTA ";
    //        //this.ResumenOF = this.PopulateDT();

    //        //string[] fruta=new string[4];
    //        //double[] kilos = new double[4];
    //        //int i = 0;
    //        //foreach (DataRow data in this.ResumenOF.Rows)
    //        //{
    //        //     fruta[i]= data[1].ToString();
    //        //    kilos[i] = double.Parse(data[2].ToString());
    //        //    i++;
    //        //}
    //        //this.TipoFrutas = fruta;
    //        //this.KilosFrutas = kilos;
    //    }

    //}

    public class HanaCon
    {
        private HanaConnection cn;

        public SqlCommand cmd;
        public SqlDataReader dr;
        public string cSql;
        public HanaDataAdapter da;
        public DataTable dt;
        public string ErrorT;
        public int ErrorC;
        public int Registros;

        public HanaCon()
        {
            cn = new HanaConnection("Server=172.24.86.5:30015;UserID=SYSTEM;Password=SAPB1_Admin!!");
            cn.Open();
        }

        public DataTable PopulateDT()
        {
            DataTable Dt = new DataTable();
            ErrorC = 0;
            try
            {
                da = new HanaDataAdapter(cSql, cn);
                da.SelectCommand.CommandTimeout = 10000;
                da.Fill(Dt);
                this.Registros = Dt.Rows.Count;
            }
            catch (Exception ex)
            {
                this.ErrorT = ex.ToString();
                this.ErrorC = -1;
                this.Registros = 0;
            }

            return Dt;
        }
    }

    public class cUsuario

    {
        #region Constructor

        public cUsuario()
        {
        }

        public cUsuario(string usuario, string clave, string basedatos)
        {
            this.empresa = new SAPbobsCOM.Company();
            this.Empresa = this.empresa;
            this.usuario = usuario;
            this.clave = clave;
            this.basedatos = basedatos;
            this.empresa.Server = "hana:30015";
            this.empresa.LicenseServer = "sapb1:40000";
            this.empresa.UseTrusted = false;
            this.empresa.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
            this.empresa.DbUserName = "SYSTEM";
            this.empresa.DbPassword = "SAPB1_Admin!!";
            this.empresa.CompanyDB = this.basedatos;
            this.empresa.UserName = this.usuario;
            this.empresa.Password = this.clave;
            this.empresa.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;
            this.empresa.AddonIdentifier = string.Empty;
            Reconectar();
            //if (this.empresa.Connect() != 0)
            //{
            //    this.errorconexion = this.empresa.GetLastErrorDescription();
            //    this.empresa = null;
            //    this.conectado = false;

            //}
            //else
            //{
            //    this.empresa.XMLAsString = true;
            //    this.empresa.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
            //    this.errorconexion = "Conexion exitosa usuario: " + this.usuario ;
            //    this.conectado = true;
            //    this.Empresa = this.empresa;
            //    this.mRecordSet("select \"U_NAME\" from \"OUSR\" where \"USER_CODE\"='" + this.usuario + "'");
            //    if (this.RecordSet.RecordCount > 0)
            //    {
            //        this.NombreUsuario = this.RecordSet.Fields.Item(0).Value.ToString();
            //    }

            //}
            //this.Empresa = this.empresa;
            //this.Usuario = this.usuario;
            //this.Empresa = this.empresa;
            //this.Errorconexion = this.errorconexion;
            //this.Conectado = this.conectado;
            //this.BaseDatos = this.basedatos;
            //this.NombreEmpresa = this.Empresa.CompanyName;
        }

        #endregion Constructor

        #region Miembros

        private string usuario;
        private string clave;
        private string basedatos;
        private bool conectado;
        private string errorconexion;
        private SAPbobsCOM.Company empresa;

        #endregion Miembros

        #region Propiedades

        public string Usuario;
        public string NombreUsuario;
        public string Clave;
        public string BaseDatos;
        public string NombreEmpresa;
        public bool Conectado;
        public string Errorconexion;
        public SAPbobsCOM.Company Empresa;
        public SAPbobsCOM.Recordset RecordSet;

        public void mRecordSet(string sql)

        {
            //this.RecordSet = new SAPbobsCOM.Recordset();
            this.RecordSet = (SAPbobsCOM.Recordset)this.empresa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            //this.RS = empresa.GetBusinessObject(B)
            this.RecordSet.DoQuery(sql);
        }

        public void Reconectar()
        {
            if (this.empresa.Connect() != 0)
            {
                this.errorconexion = this.empresa.GetLastErrorDescription();
                this.empresa = null;
                this.conectado = false;
            }
            else
            {
                this.empresa.XMLAsString = true;
                this.empresa.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                this.errorconexion = "Conexion exitosa usuario: " + this.usuario;
                this.conectado = true;
                this.Empresa = this.empresa;
                this.NombreEmpresa = this.Empresa.CompanyName;
                this.mRecordSet("select \"U_NAME\" from \"OUSR\" where \"USER_CODE\"='" + this.usuario + "'");
                if (this.RecordSet.RecordCount > 0)
                {
                    this.NombreUsuario = this.RecordSet.Fields.Item(0).Value.ToString();
                }
            }
            this.Empresa = this.empresa;
            this.Usuario = this.usuario;
            this.Empresa = this.empresa;
            this.Errorconexion = this.errorconexion;
            this.Conectado = this.conectado;
            this.BaseDatos = this.basedatos;
        }

        #endregion Propiedades
    }

    public class cHana

    {
        #region Constructor

        public cHana()
        {
        }

        public cHana(string sql, string database)
        {
            this.Sql = sql;
            this.DataBase = database;
            this.PopulateDT();
        }

        #endregion Constructor

        #region Miembros

        private string sql;
        private string database;
        private const string HanaConn = "Server=172.24.86.5:30015;UserID=SYSTEM;Password=SAPB1_Admin!!";
        private HanaConnection cn;
        private SqlDataReader datareader;
        private HanaDataAdapter dataadapter;

        private void PopulateDT()
        {
            this.DataTable = new DataTable();
            this.CodigoError = 0;
            try
            {
                this.cn = new HanaConnection(HanaConn);
                cn.Open();
                dataadapter = new HanaDataAdapter(this.Sql, cn);
                dataadapter.SelectCommand.CommandTimeout = 10000;
                dataadapter.Fill(this.DataTable);
                this.Registros = this.DataTable.Rows.Count;
            }
            catch (Exception ex)
            {
                this.MensajeError = ex.ToString();
                this.CodigoError = -1;
                this.Registros = 0;
            }

            return;
        }

        #endregion Miembros

        #region Propiedades

        public string Sql;
        public string DataBase;
        public SqlCommand SqlCmd;
        public DataTable DataTable;
        public string MensajeError;
        public int CodigoError;
        public int Registros;

        #endregion Propiedades
    }

    public class cRecordSet

    {
        public SAPbobsCOM.Recordset RS;

        public cRecordSet(string sql, SAPbobsCOM.Company empresa)
        {
            //this.RS = new SAPbobsCOM.Recordset();
            this.RS = (SAPbobsCOM.Recordset)empresa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            //this.RS = empresa.GetBusinessObject(B)
            this.RS.DoQuery(sql);
        }

        public cRecordSet()
        {
        }

        public DataTable clsResumenOF(int _Opc, cUsuario Usuario)

        {
            string _sql = "select \"DocNum\" as OF1, \"StartDate\" as FechaProgramada, DAYS_BETWEEN(\"StartDate\", CURRENT_DATE) as DiasAbierta, CASE \"Status\" When 'C' Then 'Cancelada' When 'R' Then 'Liberada' When 'L' Then 'Cerrada' When 'P'  Then 'Planificada' END as Estado,\"ProdName\" as Especie, \"ProdName\" as Articulo, \"PlannedQty\" as Planificado,  'orden.aspx?' || TO_NVARCHAR( \"DocNum\") as url, \"DocEntry\" from  \"" + Usuario.BaseDatos + "\".\"OWOR\" ";
            string _Comandosql = "";
            switch (_Opc)
            {
                case 1:
                    _Comandosql = _sql;
                    break;

                case 2:
                    _Comandosql = _sql + " where \"Status\" = 'L'";
                    break;

                case 3:
                    _Comandosql = _sql + " where \"Status\" = 'R'";
                    break;

                case 4:
                    _Comandosql = _sql + " where \"Status\" = 'P'";
                    break;
            }

            this.RS = (SAPbobsCOM.Recordset)Usuario.Empresa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            this.RS.DoQuery(_Comandosql);
            DataTable Dt = new DataTable();

            return RsTODataTabla(ref this.RS);
        }

        public DataTable RsTODataTabla(ref SAPbobsCOM.Recordset _rs)
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < _rs.Fields.Count; i++)
                dt.Columns.Add(_rs.Fields.Item(i).Description);
            while (!_rs.EoF)
            {
                DataRow row = dt.NewRow();
                for (int i = 0; i < _rs.Fields.Count; i++)
                    row[i] = _rs.Fields.Item(i).Value;
                dt.Rows.Add(row.ItemArray);
                _rs.MoveNext();
            }
            return dt;
        }
    }
}