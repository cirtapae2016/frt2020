using System.Collections.Generic;
using System.Xml.Serialization;

namespace CoreUtilities
{
    #region Menu SAP

    public static class SAPMenu
    {
        public const string RightClickMenu = "1280";
        public const string New = "1282";
        public const string Find = "1281";
        public const string Refresh = "1304";
        public const string MenuModules = "43520";
        public const string SalesMenu = "2048";
    }

    #endregion Menu SAP

    #region ServiceLayer

    public static class ServiceLayer
    {
        //Url
        public const string Address = "https://172.24.86.5:50000/b1s/v1";

        #region Objetos Nativos

        //Login
        public const string Login = "/Login";

        //Logout
        public const string Logout = "/Logout";

        //Oferta de venta
        public const string PurchaseQuotations = "/PurchaseQuotations";

        //Orden de compra
        public const string PurchaseOrders = "/PurchaseOrders";

        //Entrada Mercaderia por compra
        public const string PurchaseDeliveryNotes = "/PurchaseDeliveryNotes";

        //Factura de compra
        public const string PurchaseInvoices = "/PurchaseInvoices";

        //Nota de credito compra
        public const string PurchaseCreditNotes = "/PurchaseCreditNotes";

        //Nota de credito compra
        public const string PurchaseReturns = "/PurchaseReturns";

        //Oferta de venta
        public const string Quotations = "/Quotations";

        //Nota de venta
        public const string Orders = "/Orders";

        //Factura de venta
        public const string Invoices = "/Invoices";

        //Entrega
        public const string DeliveryNotes = "/DeliveryNotes";

        //Nota de credito
        public const string CreditNotes = "/CreditNotes";

        //Devolucion
        public const string Returns = "/Returns";

        //Transferencia de inventario
        public const string StockTransfers = "/StockTransfers";

        //Socios de negocios
        public const string BusinessPartners = "/BusinessPartners";

        //Actividades
        public const string Activities = "/Activities";

        //Locations
        public const string ActivityLocations = "/ActivityLocations";

        //Articulos
        public const string Items = "/Items";

        //Salida de mercaderia
        public const string InventoryGenExits = "/InventoryGenExits";

        //Entrada de mercaderia
        public const string InventoryGenEntries = "/InventoryGenEntries";

        //Solicitud de traslado
        public const string InventoryTransferRequests = "/InventoryTransferRequests";

        //LoteSAP
        public const string BatchNumberDetails = "/BatchNumberDetails";

        //Orden de Fabricacion
        public const string ProductionOrders = "/ProductionOrders";

        //Draft
        public const string Drafts = "/Drafts";

        //Save Draft Document

        public const string DraftsService_SaveDraftToDocument = "/DraftsService_SaveDraftToDocument";

        //Lista de Materiales
        public const string ProductTrees = "/ProductTrees";

        //Condiciones de pago
        public const string PaymentTermsTypes = "/PaymentTermsTypes";

        #endregion Objetos Nativos

        #region UDOs

        //Recepcion
        public const string Recepcion = "/OTRUCK";

        //Comex
        public const string Embarque = "/OEMB";

        //Maestro Calidad
        public const string MaestroCalidad = "/OMQLTY";

        //Registro Calidad
        public const string RegistroCalidad = "/ORQLTY";

        //Registro Producto terminado
        public const string ProductoTerminado = "/OLOPT";

        //Registro Calibrado
        public const string Calibrado = "/OCAOF";

        //Textos cortos calidad
        public const string MaestroTextosCortos = "/U_DFO_OTXT";

        //Maestro Atributos calidad
        public const string MaestroAtributosCalidad = "/U_DFO_OATTR";

        //Maestro Calibres Ciruela
        public const string CalibresCiruela = "/U_DFO_CALCIR";

        #endregion UDOs

        #region Vistas Analiticas

        //Listado de actividades
        public const string ListadoActividadesPlanificacion = "/sml.svc/DFO_ActivityList_Query";

        //Listado de ordenes de compra
        public const string ListadoOrdenCompra = "/sml.svc/DFO_OcList_Query";

        //Vista Recepciones
        public const string ListadoRecepciones = "/sml.svc/DFO_Recepciones_Query";

        //Vista Atributos Calidad
        public const string ListadoAtributosCalidad = "/sml.svc/DFO_CalidadAttrList_Query";

        //Vista Cabecera Calidad
        public const string ListadoRegistrosCalidad = "/sml.svc/DFO_CalidadList_Query";

        //Lista de traslados
        public const string ListadoTrasladoEntrePlantas = "/sml.svc/DFO_GdList_Query";

        //Lista de OTs
        public const string ListadoOrdenesFabricacion = "/sml.svc/DFO_OfList_Query";

        //Vista lotes calibrado
        public const string ListadoBinsMP = "/sml.svc/DFO_BinsCalibrado_Query";

        //Vista lotes por OF
        public class ListadoLotesOrdenFabricacion
        {
            public string url { get; }

            public ListadoLotesOrdenFabricacion(int DocKey, string Punto)
            {
                url = $"/sml.svc/DFO_OfLoteList_QueryParameters(DocKey={DocKey}, Punto='{Punto}')/DFO_OfLoteList_Query";
            }
        }

        public class CalidadByLote
        {
            public string url { get; }

            public CalidadByLote(string Lote, string IdCalidad)
            {
                url = $"/sml.svc/DFO_CalidadByLote_QueryParameters(Lote='{Lote}', IdCalidad='{IdCalidad}')/DFO_CalidadByLote_Query";
            }
        }

        public class RegQltyByLote
        {
            public string url { get; }

            public RegQltyByLote(string Lote)
            {
                url = $"/sml.svc/DFO_RegQaByLote_QueryParameters(Lote='{Lote}')/DFO_RegQaByLote_Query";
            }
        }

        public class LoteMPtoCA
        {
            public string url { get; }

            public LoteMPtoCA(string Lote)
            {
                url = $"/sml.svc/DFO_LoteMP_QueryParameters(Batch='{Lote}')/DFO_LoteMP_Query";
            }
        }

        #endregion Vistas Analiticas
    }

    #endregion ServiceLayer

    #region Menu Usuario

    public static class UserMenu
    {
        public const string MenuPrincipal = "mnuFRU.00.00";
        public const string Configuracion = "mnuFRU.01.00";
        public const string MenuRecepcion = "mnuFRU.02.00";
        public const string MenuCalidad = "mnuFRU.03.00";
        public const string MenuProduccion = "mnuFRU.04.00";
        public const string MenuComex = "mnuFRU.05.00";
        public const string DeleteRow = "mnuFRU.Del.Row";
        public const string AddRow = "mnuFRU.Add.Row";
    }

    #endregion Menu Usuario

    #region Codigos de Documento Electronico

    public static class CodigosDocumentoElectronico
    {
        public const string FacturaElectronica = "33";
        public const string FacturaNoAfectaElectronica = "34";
        public const string BoletaElectronica = "39";
        public const string BoletaNoAfectaElectronica = "41";
        public const string LiquidacionFacturaElectronica = "43";
        public const string FacturaDeCompraElectronica = "46";
        public const string GuiaDespachoElectronica = "52";
        public const string NotaDebitoElectronica = "56";
        public const string NotaCreditoElectronica = "61";
        public const string FacturaExportacionElectronica = "110";
        public const string NotaDebitoExportacionElectronica = "111";
        public const string NotaCreditoExportacionElectronica = "112";

        public static string[] All = { FacturaElectronica, FacturaNoAfectaElectronica, BoletaElectronica, BoletaNoAfectaElectronica,
                                    LiquidacionFacturaElectronica, FacturaDeCompraElectronica, GuiaDespachoElectronica, NotaDebitoElectronica,
                                    NotaCreditoElectronica, FacturaExportacionElectronica, NotaDebitoExportacionElectronica,
                                    NotaCreditoExportacionElectronica };
    }

    #endregion Codigos de Documento Electronico

    #region Formularios SAP

    public static class SAPFormType
    {
        public const string OfertaVentas = "149";
        public const string PedidoCliente = "139";
        public const string Entrega = "140";
        public const string Devolucion = "180";
        public const string SolicitudAnticipoCliente = "65308";
        public const string FacturaAnticipoCliente = "65300";
        public const string FacturaDeudores = "133";
        public const string FacturaExentaDeudores = "65302";
        public const string NotaDebitoCliente = "65303";
        public const string Boleta = "65304";
        public const string BoletaExenta = "65305";
        public const string FacturaExportacion = "65307";
        public const string NotaCreditoCliente = "179";
        public const string FacturaReservaCliente = "60091";
        public const string FacturaDeudorPago = "60090";

        public const string SolicitudCompra = "1470000200";
        public const string OfertaCompra = "540000988";
        public const string Pedido = "142";
        public const string EntradaMercancias = "143";
        public const string DevolucionMercancias = "182";
        public const string SolicitudAnticipoProveedor = "65309";
        public const string FacturaAnticipoProveedor = "65301";
        public const string FacturaProveedor = "141";
        public const string NotaDebitoProveedor = "65306";
        public const string NotaCreditoProveedor = "181";
        public const string FacturaReservaProveedor = "60092";

        public const string EntradaMercanciasInventario = "721";
        public const string SalidaMercanciasInventario = "720";
        public const string SolicitudTraslado = "1250000940";
        public const string TransferenciaStock = "940";
        public const string Lote = "65053";
        public const string OrdenFabricacion = "65211";

        public static string[] All = { OfertaVentas, PedidoCliente, Entrega, Devolucion, SolicitudAnticipoCliente, FacturaAnticipoCliente, FacturaDeudores, NotaDebitoCliente, Boleta,
                                    FacturaExportacion, NotaCreditoCliente, FacturaReservaCliente,
                                    SolicitudCompra, OfertaCompra, Pedido, EntradaMercancias, DevolucionMercancias, SolicitudAnticipoProveedor, FacturaAnticipoProveedor, FacturaProveedor,
                                    NotaDebitoProveedor, NotaCreditoProveedor,FacturaReservaProveedor,
                                    EntradaMercanciasInventario, SalidaMercanciasInventario, SolicitudTraslado, TransferenciaStock, OrdenFabricacion};

        public static string[] WithFolio = {Entrega, FacturaAnticipoCliente, FacturaDeudores, NotaDebitoCliente, Boleta, BoletaExenta, FacturaExportacion, NotaCreditoCliente,
                                    FacturaReservaCliente, FacturaDeudorPago, FacturaExentaDeudores,
                                    SalidaMercanciasInventario, TransferenciaStock,
                                    DevolucionMercancias, FacturaProveedor};

        public static string[] VentasFolio = {Entrega, FacturaAnticipoCliente, FacturaDeudores, NotaDebitoCliente, Boleta, BoletaExenta, FacturaExportacion, NotaCreditoCliente,
                                    FacturaReservaCliente, FacturaDeudorPago, FacturaExentaDeudores};

        public static string[] InventarioFolio = { SalidaMercanciasInventario, TransferenciaStock };

        public static string[] ComprasFolio = { DevolucionMercancias, FacturaProveedor };
    }

    public static class CommonForms
    {
        public static class FormCalidad
        {
            public const string FormType = "Calidad";
            public const string ButtonOK = "1";
            public const string ButtonCancel = "2";
            public const string ButtonAsignar = "3";
            public const string CmbObj = "5";
            public const string TxtID = "6";
            public const string ButtonCalc = "7";
            public const string TxtVersion = "8";
            public const string TxtMuestra = "11";
            public const string CmbLocation = "14";
            public const string UDLotes = "UDLotes";
            public const string UDCab = "UDCab";
            public const string UDEntry = "UDEntry";
            public const string UDAttrs = "UDAttrs";

            public const string ExtObj = "99";
            public const int TopIni = 57;
            public const int LblLeft = 35;
            public const int LblWidth = 152;
            public const int TxtLeft = 204;
            public const int TxtWidth = 152;
            public const int LblTopeLeft = 365;
        }

        public static class FormLotesCalidad
        {
            public const string FormType = "LotesCalidad";
            public const string ButtonOK = "1";
            public const string ButtonCancel = "2";
            public const string UDFather = "UDFather";

            public static class GrdLotes
            {
                public const string uuid = "3";
                public const string dt = "DT_3";
            }
        }

        public static class FormLoteTemp
        {
            public const string CFLEnvases = "CFL_0";
            public const string CFLProductor = "CFL_1";

            public const string FormType = "RegLote";
            public const string ButtonOK = "1";
            public const string ButtonCancel = "2";
            public const string TxtLoteID = "4";
            public const string TxtFolioGuia = "6";
            public const string TxtCantEnvase = "8";
            public const string TxtLoteCancha = "18";
            public const string CmbTipoSecado = "19";

            public static class TxtTipoEnvase
            {
                public const string Uid = "10";
                public const string UDS = "UD_10";
            };


            public static class TxtProductor
            {
                public const string Uid = "16";
                public const string UDS = "UD_16";
            };
            public static class TxtNomProd
            {
                public const string Uid = "20";
                public const string UDS = "UD_20";
            };

            public const string TxtFatherUID = "11";
            public const string TxtMuestra = "13";
        }

        public static class FormEnvase
        {
            public const string CFLEnvases = "CFL_0";

            public const string FormType = "RegEnvase";
            public const string ButtonOK = "1";
            public const string ButtonCancel = "2";
            public const string TxtPropEnv = "6";
            public const string StaticTipoReg = "12";
            public const string TxtCantEnvase = "8";

            public static class TxtTipoEnvase
            {
                public const string Uid = "10";
                public const string UDS = "UD_10";
            };
            public static class TxtNomEnvase
            {
                public const string Uid = "13";
                public const string UDS = "UD_13";
            };

            public const string TxtFatherUID = "11";
        }

        public static class FormEnvLote
        {
            public const string CFLEnvases = "CFL_0";

            public const string FormType = "RegEnvLote";
            public const string ButtonOK = "1";
            public const string ButtonCancel = "2";

            public const string TxtPropEnv = "6";
            public const string StaticLote = "7";
            public const string TxtCantEnvase = "8";

            public static class TxtTipoEnvase
            {
                public const string Uid = "10";
                public const string UDS = "UD_10";
            };
            public static class TxtNomEnvase
            {
                public const string Uid = "12";
                public const string UDS = "UD_12";
            };

            public const string TxtFatherUID = "11";
        }

        public static class FormComexDir
        {
            public const string FormType = "ComexDir";
            public const string ButtonOK = "1";
            //public const string ButtonCancel = "2";

            public static class GridDireccion
            {
                public const string Uid = "3";
                public const string Dt = "UDT_" + Uid;
            }

            public const string TxtFatherUID = "11";
        }
    }

    #endregion Formularios SAP

    #region Colores

    public static class Colores
    {
        public static int White = (System.Drawing.Color.White.R) | (System.Drawing.Color.White.G << 8) | (System.Drawing.Color.White.B << 16);
        public static int Black = (System.Drawing.Color.Black.R) | (System.Drawing.Color.Black.G << 8) | (System.Drawing.Color.Black.B << 16);
        public static int Blue = (System.Drawing.Color.Blue.R) | (System.Drawing.Color.Blue.G << 8) | (System.Drawing.Color.Blue.B << 16);
        public static int Yellow = (System.Drawing.Color.Yellow.R) | (System.Drawing.Color.Yellow.G << 8) | (System.Drawing.Color.Yellow.B << 16);
        public static int Red = (System.Drawing.Color.Red.R) | (System.Drawing.Color.Red.G << 8) | (System.Drawing.Color.Red.B << 16);
        public static int Green = (System.Drawing.Color.Green.R) | (System.Drawing.Color.Green.G << 8) | (System.Drawing.Color.Green.B << 16);
        public static int Orange = (System.Drawing.Color.Orange.R) | (System.Drawing.Color.Orange.G << 8) | (System.Drawing.Color.Orange.B << 16);
        public static int LightGray = (System.Drawing.Color.LightGray.R) | (System.Drawing.Color.LightGray.G << 8) | (System.Drawing.Color.LightGray.B << 16);
        public static int GreenYellow = (System.Drawing.Color.GreenYellow.R) | (System.Drawing.Color.GreenYellow.G << 8) | (System.Drawing.Color.GreenYellow.B << 16);
    }

    #endregion Colores

    #region Iconos

    public static class Iconos
    {
        public const string Success = "ARCHIVE_SUCCESS_ICON";
        public const string Error = "SB_ERROR";
        public const string Refresh = "1304_MENU";
    }

    #endregion Iconos

    #region Objetos

    #region Document Object

    public class IDocument_Lines
    {
        public int? DocEntry { get; set; }
        public int? LineNum { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string BaseType { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public string Price { get; set; }
        public string Currency { get; set; }
        public string WarehouseCode { get; set; }
        public string U_FRU_Variedad { get; set; }
        public string U_FRU_Tipo { get; set; }
        public string U_FRU_Color { get; set; }
        public string U_FRU_Calibre { get; set; }
        public string U_FRU_Conteo { get; set; }
        public string U_FRU_Caracteristica { get; set; }
        public string U_FRU_DescripcionCliente { get; set; }
        public string U_FRU_DescripcionAduana { get; set; }
        public string U_FRU_CajaSaco { get; set; }
        public double? Weight1 { get; set; }
        public double? Weight2 { get; set; }
        public List<BatchNumbers> BatchNumbers { get; set; }
    }

    public class IDocuments
    {
        public int? DocEntry { get; set; }
        public int? DocNum { get; set; }
        public string DocDate { get; set; }
        public string DocDueDate { get; set; }
        public string RefDate { get; set; }
        public string CardCode { get; set; }
        public string CardName { get; set; }
        public string NumAtCard { get; set; }
        public string FolioNumber { get; set; }
        public string FolioPrefixString { get; set; }
        public string Indicator { get; set; }
        public string Comments { get; set; }
        public string Address { get; set; }
        public string Address2 { get; set; }
        public string SalesPersonCode { get; set; }
        public string DocumentsOwner { get; set; }
        public string DocumentStatus { get; set; }
        public string DocumentSubType { get; set; }
        public string DocType { get; set; }
        public string DocRate { get; set; }
        public string DocCurrency { get; set; }
        public string Cancelled { get; set; }
        public string FederalTaxID { get; set; }
        public string GroupNumber { get; set; }
        public double? DiscountPercent { get; set; }
        public string JournalMemo { get; set; }
        public string ShipToCode { get; set; }
        public string ShipFrom { get; set; }
        public string PayToCode { get; set; }
        public int? PaymentGroupCode { get; set; }
        public string U_DFO_Planta { get; set; }
        public string U_DFO_PlantaDate { get; set; }
        public string U_DFO_PlantaHour { get; set; }
        public string U_DFO_SAGDate { get; set; }
        public string U_DFO_SAGHour { get; set; }
        public double? U_FRU_PorcentajeComision { get; set; }
        public string U_DTE_CodModVenta { get; set; }
        public string U_DTE_CodClauVenta { get; set; }
        public string U_DTE_IdAdicPtoDesemb { get; set; }
        public string U_DTE_CodPaisDestin { get; set; }
        public List<IDocument_Lines> DocumentLines { get; set; }
    }

    public class Quotation : IDocuments { }

    public class Order : IDocuments { }

    public class DeliveryNote : IDocuments { }

    public class Invoice : IDocuments { }

    public class CreditNote : IDocuments { }

    public class Return : IDocuments { }

    public class PurchaseOrder : IDocuments { }

    public class PurchaseDeliveryNote : IDocuments { }

    public class PurchaseInvoice : IDocuments { }

    public class PurchaseCreditNote : IDocuments { }

    public class PurchaseReturn : IDocuments { }

    public class InventoryGenExit : IDocuments { }

    public class InventoryGenEntries : IDocuments { }

    public class Drafts : IDocuments { }

    public class DraftsService_SaveDraftToDocument
    {
        public IDocuments Document { get; set; }
    }

    public class InventoryTransferRequests : StockTransfer { }

    public class StockTransferLines
    {
        public int? DocEntry { get; set; }
        public int? LineNum { get; set; }
        public int? BaseEntry { get; set; }
        public int? BaseLine { get; set; }
        public string BaseType { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public double Quantity { get; set; }
        public string FromWarehouseCode { get; set; }
        public string WarehouseCode { get; set; }
        public string SerialNumber { get; set; }
        public string U_FRU_Variedad { get; set; }
        public string U_FRU_Tipo { get; set; }
        public string U_FRU_Color { get; set; }
        public string U_FRU_Calibre { get; set; }
        public string U_FRU_Conteo { get; set; }
        public string U_FRU_Caracteristica { get; set; }
        public List<BatchNumbers> BatchNumbers { get; set; }
    }

    public class StockTransfer
    {
        public string DocEntry { get; set; }
        public string DocNum { get; set; }
        public string DocDate { get; set; }
        public string DueDate { get; set; }
        public string TaxDate { get; set; }
        public string CardCode { get; set; }
        public string FolioNumber { get; set; }
        public string FolioPrefixString { get; set; }
        public string FromWarehouse { get; set; }
        public string ToWarehouse { get; set; }
        public string Comments { get; set; }
        public string U_DTE_FolioRef { get; set; }
        public List<StockTransferLines> StockTransferLines { get; set; }
    }

    #endregion Document Object

    #region Planificacion

    public class Activities
    {
        public string ActivityCode { get; set; }
        public string Closed { get; set; }
        public string CardCode { get; set; }
        public string ActivityProperty { get; set; }
        public string Notes { get; set; }
        public string Details { get; set; }
        public string StartDate { get; set; }
        public string EndDueDate { get; set; }
        public string ActivityDate { get; }
        public string ActivityTime { get; }
        public string DocEntry { get; set; }
        public string DocTypeEx { get; set; }
        public string DocType { get; set; }
        public string DocNum { get; set; }
        public string HandledByRecipientList { get; set; }
        public string Subject { get; set; }
        public string ActivityType { get; set; }
        public string U_DFO_Trasv { get; set; }
        public string U_DFO_Transportista { get; set; }
        public string U_DFO_RutTransp { get; set; }
        public string U_DFO_Chofer { get; set; }
        public string U_DFO_RutChofer { get; set; }
        public string U_DFO_Patente { get; set; }
        public string U_DFO_Acoplado { get; set; }
        public string U_DFO_CantEnv { get; set; }
        public string U_DFO_PropEnv { get; set; }
        public string U_DFO_CodFruta { get; set; }
        public string U_DFO_Origen { get; set; }
    }

    public class ActivityLocations
    {
        public string Code { get; set; }
        public string Name { get; set; }
    }

    public class Notes
    {
        public string Codigo { get; set; }
        public string Fruta { get; set; }
        public string Variedad { get; set; }
        public string Tipo { get; set; }
        public string RazonSocial { get; set; }
        public string LineNum { get; set; }
    }

    #endregion Planificacion

    #region Pesaje

    public class Items
    {
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string InventoryUOM { get; set; }
        public string ForeignName { get; set; }
        public double? InventoryWeight { get; set; }
        public double? PurchaseUnitWeight { get; set; }
        public double? SalesUnitWeight { get; set; }

    }

    #endregion Pesaje

    #region Recepcion

    public class Recepcion_Guias
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public string U_ClgCode { get; set; }
        public string U_FolioGuia { get; set; }
        public double U_KilosGuia { get; set; }
        public int? U_Envases { get; set; }
        public string U_CodEnvase { get; set; }
        public string U_Obs { get; set; }
        public string U_Fruta { get; set; }
        public string U_RznSoc { get; set; }
        public string U_ItemCode { get; set; }
        public string U_Variedad { get; set; }
        public string U_Tipo { get; set; }
        public string U_BaseLine { get; set; }
        public string U_CardCode { get; set; }
        public string U_CardName { get; set; }
        public string U_DocEntry { get; set; }
        public char U_LineStatus { get; set; }
    }

    public class Recepcion_Lotes
    {
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string U_Lote { get; set; }
        public string U_FolioGuia { get; set; }
        public string U_Code { get; set; }
        public string U_CardCode { get; set; }
        public string U_CardName { get; set; }
        public int? U_Envases { get; set; }
        public string U_CodEnvase { get; set; }
        public string U_Muestra { get; set; }
        public double U_PesoLote { get; set; }
        public string U_PesoEnvase { get; set; }
        public double? U_Castigo { get; set; }
        public string U_Aprobado { get; set; }
        public string U_Variedad { get; set; }
        public string U_BaseLine { get; set; }
        public string U_Tipo { get; set; }
        public int? U_LoteCancha { get; set; }
        public string U_TipoSecado { get; set; }
    }

    public class Recepcion_Pesaje
    {
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string U_TipoPesaje { get; set; }
        public string U_Patente { get; set; }
        public string U_Lote { get; set; }
        public double U_Kilos { get; set; }
        public string U_Fecha { get; set; }
        public string U_Hora { get; set; }
        public string U_Sentido { get; set; }
        public string U_Motivo { get; set; }
    }

    public class Recepcion_Envases
    {
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string U_CodEnvase { get; set; }
        public string U_NomEnvase { get; set; }
        public int? U_Envases { get; set; }
        public string U_PropEnv { get; set; }
    }

    public class Recepcion_EnvLote
    {
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string U_Lote { get; set; }
        public string U_CodEnvase { get; set; }
        public string U_NomEnvase { get; set; }
        public int? U_Envases { get; set; }
        public string U_PropEnv { get; set; }
    }

    public class Recepcion_Envases_Sal
    {
        public string DocEntry { get; set; }
        public string LineId { get; set; }
        public string U_CodEnvase { get; set; }
        public string U_NomEnvase { get; set; }
        public int? U_Envases { get; set; }
        public string U_PropEnv { get; set; }
    }

    public class Recepcion
    {
        public string DocEntry { get; set; }
        public string DocNum { get; set; }
        public string Status { get; set; }
        public string Creator { get; set; }
        public string Canceled { get; set; }
        public string UserSign { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string U_Tipo { get; set; }
        public string U_Chofer { get; set; }
        public string U_RutChofer { get; set; }
        public string U_Transportista { get; set; }
        public string U_RUTTransp { get; set; }
        public string U_FreeTxt { get; set; }
        public string U_Patente { get; set; }
        public string U_Carro { get; set; }
        public string U_Fecha { get; set; }
        public string U_HoraLLegada { get; set; }
        public string U_HoraEntrada { get; set; }
        public string U_HoraSalida { get; set; }
        public string U_CamFrontal { get; set; }
        public string U_CamTrasera { get; set; }
        public string U_CamFrontalSal { get; set; }
        public string U_CamTraseraSal { get; set; }
        public double? U_KilosIngreso { get; set; }
        public double? U_KilosSalida { get; set; }
        public double? U_KilosIngAco { get; set; }
        public double? U_KilosSalAco { get; set; }
        public string U_Inst { get; set; }
        public string U_Revision { get; set; }
        public string Encarpado { get; set; }
        public string Estivado { get; set; }
        public List<Recepcion_Guias> DFO_TRUCK1Collection { get; set; }
        public List<Recepcion_Lotes> DFO_TRUCK2Collection { get; set; }
        public List<Recepcion_Pesaje> DFO_TRUCK3Collection { get; set; }
        public List<Recepcion_Envases> DFO_TRUCK4Collection { get; set; }
        public List<Recepcion_EnvLote> DFO_TRUCK5Collection { get; set; }
        public List<Recepcion_Envases_Sal> DFO_TRUCK6Collection { get; set; }
    }

    #endregion Recepcion

    #region Comex

    public class Embaque_Detalle
    {
        public int DocEntry { get; set; }
        public int LineId { get; set; }
        public double U_Quantity { get; set; }
        public string U_ItemCode { get; set; }
        public string U_ItemName { get; set; }
        public string U_Variedad { get; set; }
        public string U_Tipo { get; set; }
        public int U_DocEntry { get; set; }
        public int U_DocNum { get; set; }
        public string U_CardCode { get; set; }
        public string U_CardName { get; set; }
        public string U_ATA { get; set; }
        public string U_Calibre { get; set; }
        public string U_Color { get; set; }
        public int? U_Conteo { get; set; }
        public string U_Caracteristica { get; set; }
        public double U_Price { get; set; }
        public string U_Planta { get; set; }
        public string U_PlantaDate { get; set; }
        public string U_PlantaHour { get; set; }
        public string U_SAGDate { get; set; }
        public string U_SAGHour { get; set; }
        public int? U_BaseLine { get; set; }
        public string U_DescAd { get; set; }
        public string U_DescCl { get; set; }
        public string U_Pedido { get; set; }
    }

    public class Embarque
    {
        public int DocNum { get; set; }
        public string Status { get; set; }
        public string RequestStatus { get; set; }
        public string Creator { get; set; }
        public int DocEntry { get; set; }
        public string Canceled { get; set; }
        public int UserSign { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string U_CardCodeNav { get; set; }
        public string U_CardCodeAg { get; set; }
        public string U_CardNameNav { get; set; }
        public string U_CardNameAg { get; set; }
        public string U_PuertoEmb { get; set; }
        public string U_Destino { get; set; }
        public string U_Nave { get; set; }
        public string U_Viaje { get; set; }
        public string U_CardCodeTransp { get; set; }
        public string U_ETA { get; set; }
        public string U_ETD { get; set; }
        public string U_DocCutDate { get; set; }
        public string U_DocCutHour { get; set; }
        public string U_Mon { get; set; }
        public string U_Tue { get; set; }
        public string U_Wed { get; set; }
        public string U_Thu { get; set; }
        public string U_Fri { get; set; }
        public string U_Sat { get; set; }
        public string U_Sun { get; set; }
        public string U_MonFrom { get; set; }
        public string U_MonTo { get; set; }
        public string U_TueFrom { get; set; }
        public string U_TueTo { get; set; }
        public string U_WedFrom { get; set; }
        public string U_WedTo { get; set; }
        public string U_ThuFrom { get; set; }
        public string U_ThuTo { get; set; }
        public string U_FriFrom { get; set; }
        public string U_FriTo { get; set; }
        public string U_SatFrom { get; set; }
        public string U_SatTo { get; set; }
        public string U_SunFrom { get; set; }
        public string U_SunTo { get; set; }
        public string U_AduanaRmks { get; set; }
        public string U_PlantaRmks { get; set; }
        public string U_DocDate { get; set; }
        public string U_Reserva { get; set; }
        public string U_MonDt { get; set; }
        public string U_TueDt { get; set; }
        public string U_WedDt { get; set; }
        public string U_ThuDt { get; set; }
        public string U_FriDt { get; set; }
        public string U_SatDt { get; set; }
        public string U_SunDt { get; set; }
        public string U_Comments { get; set; }
        public string U_CardNameTransp { get; set; }
        public string U_DirFact { get; set; }
        public string U_DirDesp { get; set; }
        public string U_Notif1 { get; set; }
        public string U_NotD1 { get; set; }
        public string U_NotD2 { get; set; }
        public string U_NotD3 { get; set; }
        public string U_NotD4 { get; set; }
        public string U_NotD5 { get; set; }
        public string U_NotD6 { get; set; }
        public string U_NotD7 { get; set; }
        public string U_Notif2 { get; set; }
        public string U_Not2D1 { get; set; }
        public string U_Not2D2 { get; set; }
        public string U_Not2D3 { get; set; }
        public string U_Not2D4 { get; set; }
        public string U_Not2D5 { get; set; }
        public string U_Not2D6 { get; set; }
        public string U_Not2D7 { get; set; }
        public string U_Deposito { get; set; }
        public int? U_QtyCont1 { get; set; }
        public int? U_QtyCont2 { get; set; }
        public string U_TypCont1 { get; set; }
        public string U_TypCont2 { get; set; }
        public List<Embaque_Detalle> DFO_EMB1Collection { get; set; }
    }

    #endregion Comex

    #region Producto terminado

    public class ProductoTerminado
    {
        public string DocEntry { get; set; }
        public string DocNum { get; set; }
        public string Status { get; set; }
        public string Creator { get; set; }
        public string Canceled { get; set; }
        public string UserSign { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string U_DocEntryOF { get; set; }
        public string U_CodigoPT { get; set; }
        public string U_LoteID { get; set; }
    }

    #endregion Producto terminado

    #region Calibrado

    public class Calibrado
    {
        public string DocEntry { get; set; }
        public string DocNum { get; set; }
        public string Status { get; set; }
        public string Creator { get; set; }
        public string Canceled { get; set; }
        public string UserSign { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string Remark { get; set; }
        public string U_BaseEntry { get; set; }
        public string U_IdTarja { get; set; }
        public string U_NBoca { get; set; }
        public string U_Peso { get; set; }
        public string U_Estado { get; set; }
        public string U_CodEnvase { get; set; }
        public double? U_Medicion1 { get; set; }
        public double? U_Medicion2 { get; set; }
        public double? U_Medicion3 { get; set; }
        public int? U_RegCalidad { get; set; }
    }

    #endregion Calibrado

    #region Production Order

    public class ProductionOrderLine
    {
        public int? DocumentAbsoluteEntry { get; set; }
        public int? LineNumber { get; set; }
        public string ItemNo { get; set; }
        public double? BaseQuantity { get; set; }
        public double? PlannedQuantity { get; set; }
        public double? IssuedQuantity { get; set; }
        public string ProductionOrderIssueType { get; set; }
        public string Warehouse { get; set; }
        public string DistributionRule { get; set; }
        public string LocationCode { get; set; }
        public string Project { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public string WipAccount { get; set; }
        public string ItemType { get; set; }
        public string LineText { get; set; }
        public double? AdditionalQuantity { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<BatchNumbers> BatchNumbers { get; set; }
    }

    public class ProductionOrder
    {
        public int? AbsoluteEntry { get; set; }
        public int? DocumentNumber { get; set; }
        public string ItemNo { get; set; }
        public string ProductionOrderStatus { get; set; }
        public string ProductionOrderType { get; set; }
        public double PlannedQuantity { get; set; }
        public double? CompletedQuantity { get; set; }
        public double? RejectedQuantity { get; set; }
        public string PostingDate { get; set; }
        public string DueDate { get; set; }
        public int? ProductionOrderOriginEntry { get; set; }
        public int? ProductionOrderOriginNumber { get; set; }
        public string ProductionOrderOrigin { get; set; }
        public string Remarks { get; set; }
        public string ClosingDate { get; set; }
        public string ReleaseDate { get; set; }
        public string CustomerCode { get; set; }
        public string Warehouse { get; set; }
        public string InventoryUOM { get; set; }
        public string JournalRemarks { get; set; }
        public string CreationDate { get; set; }
        public string Printed { get; set; }
        public string DistributionRule { get; set; }
        public string Project { get; set; }
        public string DistributionRule2 { get; set; }
        public string DistributionRule3 { get; set; }
        public string DistributionRule4 { get; set; }
        public string DistributionRule5 { get; set; }
        public string StartDate { get; set; }
        public string ProductDescription { get; set; }
        public string U_FRU_CodigoServicio { get; set; }
        public double? U_FRU_CostoUnitario { get; set; }
        public int? U_IC_DocOrigen { get; set; }
        public string U_FRU_Variedad { get; set; }
        public string U_FRU_Tipo { get; set; }
        public string U_FRU_Calibre { get; set; }
        public string U_FRU_Color { get; set; }
        public string U_FRU_Conteo { get; set; }
        public string U_FRU_Caracteristica { get; set; }
        public List<ProductionOrderLine> ProductionOrderLines { get; set; }
    }

    #endregion Production Order

    #region Product Tree

    public class ProductTreeLines
    {
        public string ItemCode { get; set; }
        public double? Quantity { get; set; }
        public string IssueMethod { get; set; }
    }

    public class ProductTrees
    {
        public string TreeCode { get; set; }
        public double? Quantity { get; set; }
        public List<ProductTreeLines> ProductTreeLines { get; set; }
    }

    #endregion Product Tree

    #region Maestro Calidad

    public class MaestroCalidad_Detalle
    {
        public string Code { get; set; }
        public int LineId { get; set; }
        public string Object { get; set; }
        public object LogInst { get; set; }
        public string U_Attr { get; set; }
        public string U_TipoDato { get; set; }
        public string U_Largo { get; set; }
        public string U_Tolerancia { get; set; }
        public string U_Tope { get; set; }
        public string U_isTotal { get; set; }
        public string U_Formula { get; set; }
        public string U_Activo { get; set; }
        public string U_Unidad { get; set; }
        public string U_VisOrder { get; set; }
        public string U_TipoFila { get; set; }
    }

    public class MaestroCalidad
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string DocEntry { get; set; }
        public string Canceled { get; set; }
        public string Object { get; set; }
        public string LogInst { get; set; }
        public string UserSign { get; set; }
        public string Transfered { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public string UpdateDate { get; set; }
        public string UpdateTime { get; set; }
        public string DataSource { get; set; }
        public string U_Fruta { get; set; }
        public string U_Proceso { get; set; }
        public string U_PuntoControl { get; set; }
        public string U_Activo { get; set; }
        public string U_Version { get; set; }
        public string U_CardCode { get; set; }
        public string U_Descripcion { get; set; }
        public List<MaestroCalidad_Detalle> DFO_MQLTY1Collection { get; set; }
    }

    #endregion Maestro Calidad

    #region Registro Calidad

    public class RegistroCalidad_Detalle
    {
        public int? DocEntry { get; set; }
        public int? LineId { get; set; }
        public string U_Title { get; set; }
        public int? U_LineNum { get; set; }
        public string U_Attr { get; set; }
        public string U_FieldType { get; set; }
        public string U_Value { get; set; }
        public string U_Text { get; set; }
    }

    public class RegistroCalidad_Totales
    {
        public int? DocEntry { get; set; }
        public int? LineId { get; set; }
        public string U_Title { get; set; }
        public int? U_LineNum { get; set; }
        public string U_Attr { get; set; }
        public string U_FieldType { get; set; }
        public string U_Value { get; set; }
    }

    public class RegistroCalidad_Lotes
    {
        public int? DocEntry { get; set; }
        public int? LineId { get; set; }
        public string U_BatchNum { get; set; }
        public double U_Kg { get; set; }
    }

    public class RegistroCalidad_Totales_Lote
    {
        public int? DocEntry { get; set; }
        public int? LineId { get; set; }
        public string U_Title { get; set; }
        public string U_Attr { get; set; }
        public string U_Value { get; set; }
        public string U_BatchNum { get; set; }
    }

    public class RegistroCalidad
    {
        public int? DocEntry { get; set; }
        public int? DocNum { get; set; }
        public string CreateDate { get; set; }
        public string CreateTime { get; set; }
        public int UserSign { get; set; }
        public string Creator { get; set; }
        public double U_Version { get; set; }
        public string U_PuntoControl { get; set; }
        public double U_TotalKg { get; set; }
        public string U_Revisado { get; set; }
        public string U_RevisadoPor { get; set; }
        public string U_FormXML { get; set; }
        public string U_BaseType { get; set; }
        public string U_BaseEntry { get; set; }
        public List<RegistroCalidad_Detalle> DFO_RQLTY1Collection { get; set; }
        public List<RegistroCalidad_Totales> DFO_RQLTY2Collection { get; set; }
        public List<RegistroCalidad_Lotes> DFO_RQLTY3Collection { get; set; }
        public List<RegistroCalidad_Totales_Lote> DFO_RQLTY4Collection { get; set; }
    }

    public class RegCalidadDataTables
    {
        public string dtXML { get; set; }
    }

    #endregion Registro Calidad

    #region LoteSAP

    public class BatchNumberDetails
    {
        public int? DocEntry { get; set; }
        public string ItemCode { get; set; }
        public string ItemDescription { get; set; }
        public string Status { get; set; }
        public string Batch { get; set; }
        public string BatchAttribute1 { get; set; }
        public string BatchAttribute2 { get; set; }
        public string AdmissionDate { get; set; }
        public string ManufacturingDate { get; set; }
        public string ExpirationDate { get; set; }
        public string Details { get; set; }
        public int? SystemNumber { get; set; }
        public string U_FRU_Variedad { get; set; }
        public string U_FRU_Tipo { get; set; }
        public string U_FRU_Calibre { get; set; }
        public string U_FRU_Color { get; set; }
        public string U_FRU_Conteo { get; set; }
        public string U_FRU_Caracteristica { get; set; }
        public int? U_FRU_CantBins { get; set; }
        public int? U_FRU_CantBinsDis { get; set; }
        public int? U_FRU_CantBinsVol { get; set; }
        public string U_FRU_Productor { get; set; }
        public string U_FRU_EstadoCalid { get; set; }
        public string U_FRU_Humedad { get; set; }
        public string U_FRU_TendenciaCalib { get; set; }
        public double? U_FRU_Sorbato { get; set; }
        public string U_FRU_MastCalibre { get; set; }
        public string U_FRU_Moneda { get; set; }
        public string U_FRU_Destino { get; set; }
        public double? U_FRU_CostoML { get; set; }
        public double? U_FRU_CostoME { get; set; }
        public string U_FRU_TipoCosto { get; set; }
        public string U_FRU_FechaCosto { get; set; }
        public string U_FRU_FolioCajaIni { get; set; }
        public string U_FRU_FolioCajaFin { get; set; }
        public string U_FRU_FolioExtraido { get; set; }
        public string U_FRU_FolioAsig { get; set; }
        public string U_FRU_Fumigado { get; set; }
        public string U_FRU_Clasificacion { get; set; }
        public double? U_FRU_Carozo { get; set; }
        public int? U_FRU_FolioCancha { get; set; }
        public string U_FRU_Forma { get; set; }
        public string U_FRU_MateriasExtranas { get; set; }
        public string U_FRU_TotalDanos { get; set; }
        public string U_FRU_Cliente { get; set; }
        public string U_FRU_NomCliente { get; set; }
        public string U_FRU_NomProveedor { get; set; }
        public double? U_FRU_Castigo { get; set; }
        public string U_FRU_Pesticidas { get; set; }
        public string U_FRU_TipoSecado { get; set; }
        public string U_FRU_Envase { get; set; }
        public string U_FRU_ClasifHumedad { get; set; }
    }

    public class BatchNumbers
    {
        public int? BaseLineNumber { get; set; }
        public string BatchNumber { get; set; }
        public string AdmissionDate { get; set; }
        public string ManufacturingDate { get; set; }
        public string ExpiryDate { get; set; }
        public double Quantity { get; set; }
        public string Notes { get; set; }
        public int? SystemNumber { get; set; } //revisar
        public string U_FRU_Variedad { get; set; }
        public string U_FRU_Tipo { get; set; }
        public string U_FRU_Calibre { get; set; }
        public string U_FRU_Color { get; set; }
        public string U_FRU_Conteo { get; set; }
        public string U_FRU_Caracteristica { get; set; }
        public int? U_FRU_CantBins { get; set; }
        public int? U_FRU_CantBinsDis { get; set; }
        public int? U_FRU_CantBinsVol { get; set; }
        public string U_FRU_Productor { get; set; }
        public string U_FRU_EstadoCalid { get; set; }
        public string U_FRU_Humedad { get; set; }
        public string U_FRU_TendenciaCalib { get; set; }
        public double? U_FRU_Sorbato { get; set; }
        public string U_FRU_MastCalibre { get; set; }
        public string U_FRU_Moneda { get; set; }
        public string U_FRU_Destino { get; set; }
        public double? U_FRU_CostoML { get; set; }
        public double? U_FRU_CostoME { get; set; }
        public string U_FRU_TipoCosto { get; set; }
        public string U_FRU_FechaCosto { get; set; }
        public string U_FRU_FolioCajaIni { get; set; }
        public string U_FRU_FolioCajaFin { get; set; }
        public string U_FRU_FolioExtraido { get; set; }
        public string U_FRU_FolioAsig { get; set; }
        public string U_FRU_Fumigado { get; set; }
        public string U_FRU_Clasificacion { get; set; }
        public double? U_FRU_Carozo { get; set; }
        public int? U_FRU_FolioCancha { get; set; }
        public string U_FRU_Forma { get; set; }
        public string U_FRU_MateriasExtranas { get; set; }
        public string U_FRU_TotalDanos { get; set; }
        public string U_FRU_Cliente { get; set; }
        public string U_FRU_NomCliente { get; set; }
        public string U_FRU_NomProveedor { get; set; }
        public double? U_FRU_Castigo { get; set; }
        public string U_FRU_Pesticidas { get; set; }
        public string U_FRU_TipoSecado { get; set; }
        public string U_FRU_Envase { get; set; }
        public string U_FRU_ClasifHumedad { get; set; }
    }

    #endregion LoteSAP

    #region MaestroTextosCortos

    public class MaestroTextosCortos
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string U_Tipo { get; set; }
        public string U_Texto { get; set; }
    }

    #endregion MaestroTextosCortos

    #region MaestroAtributosCalidad

    public class MaestroAtributosCalidad
    {
        public int Code { get; set; }
        public string Name { get; set; }
        public string U_AttrName { get; set; }
        public string U_AttrLote { get; set; }
        public string U_Fruta { get; set; }
    }

    #endregion MaestroAtributosCalidad

    #region Login

    public class Login
    {
        public string Uri { get; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string CompanyDB { get; set; }

        public Login()
        {
            Uri = "/Login";
        }
    }

    #endregion Login

    #region Vista de EM Preliminar

    public class ListadoTrasladoEntrePlantas
    {
        public string DocEntry { get; set; }
        public string LineNum { get; set; }
        public string DocNum { get; set; }
        public string DocStatus { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string DocDueDate { get; set; }
        public string NumAtCard { get; set; }
        public string DocCur { get; set; }
        public string DocRate { get; set; }
        public string DocTotal { get; set; }
        public string Comments { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string LineTotal { get; set; }
        public string WhsCode { get; set; }
        public string OpenQty { get; set; }
        public string CardName { get; set; }
        public string PymntGroup { get; set; }
        public string SlpName { get; set; }

        public string id__ { get; set; }
    }

    #endregion Vista de EM Preliminar

    #region Vista de OCs

    public class ListadoOrdenCompra
    {
        public string DocEntry { get; set; }
        public string LineNum { get; set; }
        public string DocNum { get; set; }
        public string DocStatus { get; set; }
        public string DocDate { get; set; }
        public string CardCode { get; set; }
        public string DocDueDate { get; set; }
        public string NumAtCard { get; set; }
        public string DocCur { get; set; }
        public string DocRate { get; set; }
        public string DocTotal { get; set; }
        public string Comments { get; set; }
        public string LineStatus { get; set; }
        public string ItemCode { get; set; }
        public string Dscription { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }
        public string LineTotal { get; set; }
        public string WhsCode { get; set; }
        public string OpenQty { get; set; }
        public string CardName { get; set; }
        public string PymntGroup { get; set; }
        public string SlpName { get; set; }

        public string id__ { get; set; }
    }

    #endregion Vista de OCs

    #region Vista Calidad

    public class ListadoAtributosCalidad
    {
        public string Code { get; set; }
        public string U_Fruta { get; set; }
        public string U_Proceso { get; set; }
        public string U_Activo { get; set; }
        public string U_PuntoControl { get; set; }
        public string U_Version { get; set; }
        public string U_CardCode { get; set; }
        public int LineId { get; set; }
        public string U_Attr { get; set; }
        public string U_TipoDato { get; set; }
        public string U_Largo { get; set; }
        public string U_Tolerancia { get; set; }
        public string U_Tope { get; set; }
        public string U_isTotal { get; set; }
        public string U_Formula { get; set; }
        public string U_Unidad { get; set; }
        public string U_VisOrder { get; set; }
        public string U_TipoFila { get; set; }
        public string U_Father { get; set; }
        public string U_Rechazo { get; set; }
        public string U_AttrCode { get; set; }
        public string U_Descripcion { get; set; }
        public string U_Accion { get; set; }
        public string U_CodRegistro { get; set; }
        public string U_TipTxt { get; set; }
        public string id__ { get; set; }
    }

    #endregion Vista Calidad

    #region Vista Lotes Calibrado

    public class ListadoBinsMP
    {
        public string CODIGO { get; set; }
        public string DESCRIPCION { get; set; }
        public string LOTE { get; set; }
        public string ALMACEN { get; set; }
        public string ESTADO { get; set; }
        public double? STOCK { get; set; }
        public int? BINSTOTAL { get; set; }
        public int? BINSRESTANTES { get; set; }
        public int? BINSCONSUMIDOS { get; set; }
        public double? CANTIDADBINS { get; set; }
    }

    #endregion Vista Lotes Calibrado

    #region Vista Listado Calidad

    public class ListadoRegistrosCalidad
    {
        public string CodSAP { get; set; }
        public string Fruta { get; set; }
        public string PuntoControl { get; set; }
        public string CodProceso { get; set; }
        public double Version { get; set; }
        public string Cliente { get; set; }
        public string Descripcion { get; set; }
        public string CodUsuario { get; set; }
        public string Usuario { get; set; }
        public char Activo { get; set; }
    }

    #endregion Vista Listado Calidad

    #region Vista OFs

    public class ListadoOrdenesFabricacion
    {
        public int DocEntry { get; set; }
        public int DocNum { get; set; }
        public string Status { get; set; }
        public double PlannedQty { get; set; }
        public double CmpltQty { get; set; }
        public double RjctQty { get; set; }
        public string PostDate { get; set; }
        public string DueDate { get; set; }
        public string Comments { get; set; }
        public string CloseDate { get; set; }
        public string RlsDate { get; set; }
        public string CardCode { get; set; }
        public string Warehouse { get; set; }
        public string JrnlMemo { get; set; }
        public int? TransId { get; set; }
        public string CreateDate { get; set; }
        public string OcrCode { get; set; }
        public string OcrCode2 { get; set; }
        public string OcrCode3 { get; set; }
        public string OcrCode4 { get; set; }
        public string OcrCode5 { get; set; }
        public string Project { get; set; }
        public int? OriginAbs { get; set; }
        public int? OriginNum { get; set; }
        public string U_FRU_CodigoServicio { get; set; }
        public double U_FRU_CostoUnitario { get; set; }
        public string U_IC_DocOrigen { get; set; }
        public string U_FRU_Variedad { get; set; }
        public string U_FRU_Tipo { get; set; }
        public string U_FRU_Calibre { get; set; }
        public string U_FRU_Color { get; set; }
        public string U_FRU_Conteo { get; set; }
        public string U_FRU_Caracteristica { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public int USERID { get; set; }
        public string USER_CODE { get; set; }
        public string U_NAME { get; set; }
        public int ItmsGrpCod { get; set; }
        public string ItmsGrpNam { get; set; }
        public string U_FRU_Fruta { get; set; }
        public string CardName { get; set; }
        public int id__ { get; set; }
    }

    #endregion Vista OFs

    #endregion Objetos

    #region XMLGrid

    public static class XMLGrid
    {
        [XmlRoot(ElementName = "Column")]
        public class Column
        {
            [XmlAttribute(AttributeName = "Uid")]
            public string Uid { get; set; }

            [XmlAttribute(AttributeName = "Type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "MaxLength")]
            public string MaxLength { get; set; }
        }

        [XmlRoot(ElementName = "Columns")]
        public class Columns
        {
            [XmlElement(ElementName = "Column")]
            public List<Column> Column { get; set; }
        }

        [XmlRoot(ElementName = "Cell")]
        public class Cell
        {
            [XmlElement(ElementName = "ColumnUid")]
            public string ColumnUid { get; set; }

            [XmlElement(ElementName = "Value")]
            public string Value { get; set; }
        }

        [XmlRoot(ElementName = "Cells")]
        public class Cells
        {
            [XmlElement(ElementName = "Cell")]
            public List<Cell> Cell { get; set; }
        }

        [XmlRoot(ElementName = "Row")]
        public class Row
        {
            [XmlElement(ElementName = "Cells")]
            public Cells Cells { get; set; }
        }

        [XmlRoot(ElementName = "Rows")]
        public class Rows
        {
            [XmlElement(ElementName = "Row")]
            public List<Row> Row { get; set; }
        }

        [XmlRoot(ElementName = "DataTable")]
        public class DataTable
        {
            [XmlElement(ElementName = "Columns")]
            public Columns Columns { get; set; }

            [XmlElement(ElementName = "Rows")]
            public Rows Rows { get; set; }

            [XmlAttribute(AttributeName = "Uid")]
            public string Uid { get; set; }
        }
    }

    #endregion XMLGrid

    #region Math

    public class Tolerancias
    {
        public string Uid { get; set; }
        public double Value { get; set; }
        public int From { get; set; }
        public int To { get; set; }
    }

    #endregion Math

    public class ListAttrsToProm
    {
        public string Attr { get; set; }
        public double Value { get; set; }
        public double Weigth { get; set; }
    }
}
