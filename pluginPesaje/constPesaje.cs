namespace pluginPesaje
{
    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.02.03";
        internal const string Caption = "Pesaje romana";
    }

    internal static class pluginForm

    {
        internal const string FormType = "Pesaje";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";

        internal const string CBpesaje = "4";
        internal const string CFLnrollegada = "CFL6";

        internal const string PicFrontal = "50";
        internal const string PicTrasera = "51";

        internal static class TxtNroLlegada
        {
            internal const string Uid = "6";
            internal const string Uds = "UD6";
            internal const string CFL = "CFL6";
        }

        internal const string StaticCodEnv = "45";

        internal const string StaticTransp = "8";
        internal const string StaticRutTransp = "10";
        internal const string StaticNombChofer = "12";
        internal const string StaticRutChofer = "14";
        internal const string CBtransporte = "16";
        internal const string StaticPatente = "18";

        internal static class CBguia
        {
            internal const string Uid = "20";
            internal const string Uds = "UD20";
        }

        internal const string StaticEnvase = "22";
        internal const string ButtonAddPeso = "24";

        internal const string StaticPeso = "26";
        internal const string CBsentido = "27";
        internal const string LbCamAco = "29";
        internal const string LbPatente = "30";
        internal const string LbDocumento = "31";
        internal const string LbEnvase = "32";
        internal const string LbSentido = "33";

        internal const string ButtonPromedio = "35";

        internal static class GridLote
        {
            internal const string Uid = "44";
            internal const string Dt = "UDT_" + Uid;
        }

        internal const string StaticItemName = "46";
        internal const string StaticWeight = "47";
        internal const string StaticCantEnv = "48";
    }
}