namespace pluginPrdPT
{
    internal class valores

    {
        internal static string NroOF;
        internal static string NroOC;
        internal static string CardCode;
    }

    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.04.04";
        internal const string Caption = "EgresoPT";
    }

    internal static class pluginForm

    {
        internal const string FormType = "EgresoPT";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";
        internal const string ButtonPreviewTarja = "22";

        internal const string FolderPT = "17";
        internal const string FolderSubProd = "25";

        internal const string CBcalibre = "27";
        internal const string CBcaract = "28";


        internal static class TxtNroOrden
        {
            internal const string Uid = "3";
            internal const string Uds = "UD3";
        }

        internal static class TxtFolioInicio
        {
            internal const string Uid = "15";
            internal const string Uds = "UD15";
        }

        internal static class TxtFolioFin
        {
            internal const string Uid = "16";
            internal const string Uds = "UD16";
        }
        internal static class TxtCantidadPT
        {
            internal const string Uid = "26";
            internal const string Uds = "UD26";
        }


        internal static class TxtPesoPT
        {
            internal const string Uid = "23";
            internal const string Uds = "UD23";
        }

        internal static class TxtBodegaDestPT
        {
            internal const string Uid = "24";
            internal const string Uds = "UD24";
        }

        // internal const string StaticLote = "4";

        internal static class TxtNroPT
        {
            internal const string Uid = "4";
            internal const string Uds = "UD4";
        }

        internal const string ButtonAsignaLote = "6";
        internal const string ButtonAsigLoteSP = "20";

        internal const string CFLOrdenFab = "CFL3";

        internal static class GridConsumo
        {
            internal const string Uid = "5";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridSubProd
        {
            internal const string Uid = "18";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridConsumoSP
        {
            internal const string Uid = "21";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class TxtPesoSubProd
        {
            internal const string Uid = "19";
            internal const string Uds = "UD19";
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
    }
}