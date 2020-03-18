namespace pluginPrdMP
{
    internal class valores

    {
        internal static string NroOF;
        internal static string NroOC;
        internal static string CardCode;
    }

    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.04.03";
        internal const string Caption = "Consumo";
    }

    internal static class pluginForm

    {
        internal const string FormType = "ConsumoMP";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";

        // internal const string StaticLote = "4";

        internal static class TxtNroLote
        {
            internal const string Uid = "4";
            internal const string Uds = "UD4";
        }

        internal const string ButtonAddConsumo = "6";

        internal const string CFLOrdenFab = "CFL_15";

        internal const string StaticLote = "16";
        internal const string StaticCons = "17";
        internal const string StaticDisp = "18";
        internal const string StaticTotBins = "19";
        internal const string StaticKgBins = "20";

        internal static class GridConsumo
        {
            internal const string Uid = "5";
            internal const string Dt = "UDT_" + Uid;
        }

        internal const string LinkedNroOrden = "LK3";
        internal const string LinkedItemCode = "LK8";
        internal const string LinkedOV = "LK11";
        internal const string LinkedCardCode = "LK14";

        internal static class TxtItemCode
        {
            internal const string Uid = "8";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtItemName
        {
            internal const string Uid = "9";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtCantPlan
        {
            internal const string Uid = "10";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtNroOc
        {
            internal const string Uid = "11";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtFecCreac
        {
            internal const string Uid = "12";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtFecPlanf
        {
            internal const string Uid = "13";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtCardCode
        {
            internal const string Uid = "14";
            internal const string Uds = "UD" + Uid;
        }

        internal static class TxtNroOrden
        {
            internal const string Uid = "15";
            internal const string Uds = "UD15";
        }
    }
}