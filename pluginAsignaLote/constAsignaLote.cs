namespace pluginAsignaLote
{
    internal class valores

    {
        internal static string NroOF;
        internal static string NroOC;
        internal static string CardCode;
    }

    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.04.06";
        internal const string Caption = "Asigna Lote";
    }

    internal static class pluginForm

    {
        internal const string FormType = "AsignaLote";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";

        internal const string dbAsignaLote = "OBTN";

        internal static class MatrixLote
        {
            internal const string Uid = "3";

            internal static class Colums
            {
                internal static class Col_LineId
                {
                    internal const string Uid = "#";
                    internal const string dbField = "LineId";
                }

                internal static class Col_ItemCode
                {
                    internal const string Uid = "0";
                    internal const string dbField = "Itemcode";
                }

                internal static class Col_itemName
                {
                    internal const string Uid = "1";
                    internal const string dbField = "itemName";
                }

                internal static class Col_SysNumber
                {
                    internal const string Uid = "2";
                    internal const string dbField = "SysNumber";
                }

                internal static class Col_DistNumber
                {
                    internal const string Uid = "3";
                    internal const string dbField = "DistNumber";
                }

                internal static class Col_MnfSerial
                {
                    internal const string Uid = "4";
                    internal const string dbField = "MnfSerial";
                }

                internal static class Col_InDate
                {
                    internal const string Uid = "5";
                    internal const string dbField = "InDate";
                }

                internal static class Col_Status
                {
                    internal const string Uid = "6";
                    internal const string dbField = "Status";
                }

                internal static class Col_Quantity
                {
                    internal const string Uid = "7";
                    internal const string dbField = "Quantity";
                }

                internal static class Col_Balance
                {
                    internal const string Uid = "8";
                    internal const string dbField = "Balance";
                }

                internal static class Col_U_FRU_Variedad
                {
                    internal const string Uid = "9";
                    internal const string dbField = "U_FRU_Variedad";
                }

                internal static class Col_U_FRU_Tipo
                {
                    internal const string Uid = "10";
                    internal const string dbField = "U_FRU_Tipo";
                }

                internal static class Col_U_FRU_Calibre
                {
                    internal const string Uid = "11";
                    internal const string dbField = "U_FRU_Calibre";
                }

                internal static class Col_U_FRU_Destino
                {
                    internal const string Uid = "12";
                    internal const string dbField = "U_FRU_Destino";
                }
            }
        }
    }
}