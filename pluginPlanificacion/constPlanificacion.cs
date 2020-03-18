namespace pluginPlanificacion
{
    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.04.01";
        internal const string Caption = "Planificacion";
    }

    internal static class pluginForm

    {
        internal const string FormType = "Planificacion";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";
        internal const string BtnBuscaPedido = "9";
        internal const string BtnPlanificar = "14";
        internal const string BtnCalibrar = "15";
        internal const string BtnFiltrar = "18";
        internal const string BtnSelect = "19";

        internal const string UDKilosPedido = "UD_16";
        internal const string UDKilosSelect = "UD_17";

        internal const string UDFechaOF = "UD_20";
        internal const string UDFechaSol = "UD_21";

        internal static class TxtFechaDesde
        {
            internal const string Uid = "3";
            internal const string Uds = "UD4";
        }

        internal static class TxtFechaHasta
        {
            internal const string Uid = "4";
            internal const string Uds = "UD4";
        }

        internal const string ChkMostrarTodo = "6";

        internal static class GridOV
        {
            internal const string Uid = "5";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridLote
        {
            internal const string Uid = "7";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridLoteMP
        {
            internal const string Uid = "8";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class RdCiruela
        {
            internal const string Uid = "10";
            internal const string Uds = "UD10";
        }

        internal static class RdNuez
        {
            internal const string Uid = "11";
        }

        internal static class RdPasa
        {
            internal const string Uid = "12";
        }

        internal static class RdAll
        {
            internal const string Uid = "13";
        }
    }
}