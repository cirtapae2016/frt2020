namespace pluginPrdSE
{
    internal class valores

    {
        internal static string NroOF;
        internal static string NroOC;
        internal static string CardCode;
    }

    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.04.05";
        internal const string Caption = "Semi-Elaborado";
    }

    internal static class pluginForm

    {
        internal const string FormType = "Calibra";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";
        internal const string ButtonAsigLoteSP = "31";
        internal const string ButtonCalidad = "22";
        internal const string ButtonPreviewTarja = "35";

        internal const string dbCalibrado = "@DFO_OCAOF";

        internal static class TxtNroOrden
        {
            internal const string Uid = "3";
            internal const string Uds = "UD3";
        }

        internal static class TxtEnvase
        {
            internal const string Uid = "33";
            internal const string Uds = "UD33";
        }

        internal static class TxtDescEnvase
        {
            internal const string Uid = "34";
            internal const string Uds = "UD34";
        }

        internal static class TxtPreTarja
        {
            internal const string Uid = "15";
            internal const string Uds = "UD15";
        }

        internal static class TxtTarjaApr
        {
            internal const string Uid = "27";
            internal const string Uds = "UD27";
        }

        internal static class TxtPesaje
        {
            internal const string Uid = "16";
            internal const string Uds = "UD16";
        }

        internal static class TxtMedicion1
        {
            internal const string Uid = "37";
            internal const string Uds = "UD37";
        }

        internal static class TxtMedicion2
        {
            internal const string Uid = "38";
            internal const string Uds = "UD38";
        }

        internal static class TxtMedicion3
        {
            internal const string Uid = "39";
            internal const string Uds = "UD39";
        }

        // internal const string StaticLote = "4";

        internal const string ButtonInsertCal = "6";
        internal const string ButtonConfCal = "4";
        internal const string ButtonAsignPeso = "17";
        internal const string ButtonAprueba = "25";
        internal const string ButtonRechazo = "26";
        internal const string ButtonAprReparo = "40";

        internal const string CFLOrdenFab = "CFL3";
        internal const string CFLEnvase = "CFL4";

        internal static class GridCalibrado
        {
            internal const string Uid = "5";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridCalibraPeso
        {
            internal const string Uid = "19";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridCalibraAprueba
        {
            internal const string Uid = "24";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridSubProd
        {
            internal const string Uid = "29";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridConsumoSP
        {
            internal const string Uid = "32";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class TxtPesoSubProd
        {
            internal const string Uid = "30";
            internal const string Uds = "UD30";
        }

        internal const string LinkedNroOrden = "LK3";
        internal const string LinkedItemCode = "LK8";
        internal const string LinkedOV = "LK11";
        internal const string LinkedCardCode = "LK14";

        internal const string CBcalibre = "42";
        internal const string CBcaract = "43";

        internal const string FolderBoca = "20";
        internal const string FolderPeso = "21";
        internal const string FolderAprueba = "23";
        internal const string FolderApruebaSP = "28";

        internal static class TxtBodegaDest
        {
            internal const string Uid = "41";
            internal const string Uds = "UD" + Uid;
        }

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

        internal static class TxtSumAprobado
        {
            internal const string Uid = "36";
            internal const string Uds = "UD" + Uid;
        }
    }
}