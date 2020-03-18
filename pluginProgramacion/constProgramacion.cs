namespace pluginProgramacion
{
    internal class valores
    {
        internal static string order;
        internal static string sorting;
    }

    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.02.01";
        internal const string Caption = "Programacion de camiones";
    }

    internal static class pluginForm

    {
        internal const string FormType = "Programacion";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";
        internal const string ButtonInactivaCupo = "27";

        internal static class TxtCardCode
        {
            internal const string Uid = "14";
            internal const string Uds = "UD14";
        }

        internal static class TxtCardName
        {
            internal const string Uid = "16";
            internal const string Uds = "UD16";
        }

        internal static class TxtDocDate
        {
            internal const string Uid = "4";
            internal const string Uds = "UD4";
        }

        internal static class TxtHoraProg
        {
            internal const string Uid = "39";
            internal const string Dds = "OCLG";
        }

        internal const string ButtonOC = "19";
        internal const string ButtonGO = "5";
        internal const string ButtonToday = "6";
        internal const string ButtonAddActivity = "25";
        internal const string ButtonActLeft = "7";
        internal const string ButtonActRight = "8";

        internal const string OptnSemana = "9";
        internal const string OptnDia = "10";
        internal const string OptnMes = "12";

        internal const string TxtCantBins = "22";

        internal static class TxtTransportista
        {
            internal const string Uid = "33";
            internal const string Uds = "UD33";
        }

        internal static class TxtRutTransp
        {
            internal const string Uid = "41";
            internal const string Uds = "UD41";
        }

        internal const string TxtChofer = "35";
        internal const string TxtRutChofer = "36";
        internal const string TxtPatente = "37";
        internal const string TxtAcoplado = "38";
        internal const string TxtCantGuias = "45";

        internal const string CBpropBins = "24";
        internal const string CBlocalidad = "30";
        internal const string CBdestinatario = "29";
        internal const string CBdocumento = "40";
        internal const string CBorigen = "42";
        internal const string CBfruta = "46";
        internal const string LblOrigen = "34";

        internal const string LblItemCode = "31";
        internal const string LblVariedad = "43";
        internal const string LblCantKg = "44";

        internal const string ChkTrasvacije = "32";

        internal const string CFLProductores = "CFL_2";
        internal const string CFLCodProduct = "CFL_3";
        internal const string CFLRutTransp = "CFL_7";
        internal const string CFLTransp = "CFL_6";

        internal static class GridOC
        {
            internal const string Uid = "20";
            internal const string Dt = "UDT_" + Uid;
        }

        internal static class GridAct
        {
            internal const string Uid = "11";
            internal const string Dt = "UDT_" + Uid;
        }
    }
}