namespace pluginCalidadMaestro
{
    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.03.OCal";
        internal const string Caption = "Maestro Calidad";
    }

    internal static class pluginForm

    {
        internal const string FormType = "OMQLTY";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";

        internal const string TxtCode = "3";
        internal const string CmbFruta = "4";
        internal const string TxtProceso = "5";
        internal const string TxtPuntoControl = "6";
        internal const string TxtCliente = "9";

        internal const string CFLProceso = "CFL_5";

        internal const string ButtonPreview = "11";
        internal const string ButtonPreview2 = "13";
        internal const string ButtonPreview3 = "14";
        internal const string TxtDescrip = "12";

        internal const string TxtAccion = "15";
        internal const string TxtNombreProceso = "16";

        internal const string dbCabecera = "@DFO_OMQLTY";
        internal const string dbAttr = "@DFO_MQLTY1";

        internal static class MatrixAttr
        {
            internal const string Uid = "10";

            internal static class Colums
            {
                internal static class Col_LineId
                {
                    internal const string Uid = "#";
                }

                internal static class Col_Attr
                {
                    internal const string Uid = "1";
                }

                internal static class Col_Tipo
                {
                    internal const string Uid = "2";
                }

                internal static class Col_Unidad
                {
                    internal const string Uid = "3";
                }

                internal static class Col_Largo
                {
                    internal const string Uid = "4";
                }

                internal static class Col_Tolerancia
                {
                    internal const string Uid = "5";
                }

                internal static class Col_Tope
                {
                    internal const string Uid = "6";
                }

                internal static class Col_Calc
                {
                    internal const string Uid = "7";
                }

                internal static class Col_Formula
                {
                    internal const string Uid = "8";
                }

                internal static class Col_Activo
                {
                    internal const string Uid = "9";
                }

                internal static class Col_Orden
                {
                    internal const string Uid = "10";
                }

                internal static class Col_TipoFila
                {
                    internal const string Uid = "11";
                }

                internal static class Col_Rechazo
                {
                    internal const string Uid = "12";
                }

                internal static class Col_Father
                {
                    internal const string Uid = "13";
                }

                internal static class Col_AttrCode
                {
                    internal const string Uid = "14";
                }
            }
        }
    }
}