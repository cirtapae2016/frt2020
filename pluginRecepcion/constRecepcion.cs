﻿namespace pluginRecepcion
{
    internal static class MenuPlugin
    {
        internal const string MenuUID = "mnuFRU.02.02";
        internal const string Caption = "Recepcion";
    }

    internal static class pluginForm

    {
        internal const string FormType = "Recepcion";
        internal const string ButtonOK = "1";
        internal const string ButtonCancel = "2";
        internal const string ButtonAddGuia = "26";
        internal const string ButtonAddLote = "38";
        internal const string ButtonAddQlty = "37";
        internal const string ButtonAddEnv = "65";
        internal const string CmbTipoRecepcion = "8";
        internal const string TxtChofer = "16";
        internal const string TxtRutChofer = "18";
        internal const string TxtPatente = "20";
        internal const string TxtAcoplado = "22";
        internal const string TxtLlegada = "14";
        internal const string TxtEntrada = "30";
        internal const string TxtSalida = "32";
        internal const string TxtTransportista = "4";
        internal const string TxtRutTransp = "6";
        internal const string TxtFree = "33";
        internal const string TxtDocNum = "10";
        internal const string TxtFecha = "12";
        internal const string TxtInstrucciones = "54";
        internal const string ButtonRefresh = "55";
        internal const string ChkRevision = "58";
        internal const string ButtonImage = "59";
        internal const string ChkEncarpado = "60";
        internal const string ChkEstivado = "61";
        internal const string ButtonFinish = "62";
        internal const string CmbDocStatus = "63";

        internal const string dbCabecera = "@DFO_OTRUCK";
        internal const string dbGuias = "@DFO_TRUCK1";
        internal const string dbLotes = "@DFO_TRUCK2";
        internal const string dbEnvase = "@DFO_TRUCK4";

        internal const string LbPesoEntrada = "46";
        internal const string LbPesoLotes = "47";
        internal const string LbPesoEnvases = "48";
        internal const string LbPesoSalida = "49";
        internal const string LbDifPeso = "50";
        internal const string LbDifPorc = "51";
        internal const string LbTara = "57";

        internal const string FdLote = "35";
        internal const string FdBalance = "52";

        internal const string CFLActividades = "CFL_1";
        internal const string CFLOc = "CFL_2";
        internal const string CFLEnvases = "CFL_4";
        internal const string CFLEnvGuia = "CFL_6";
        internal const string CFLRutTransp = "CFL_8";

        internal static class MatrixGuia
        {
            internal const string Uid = "25";

            internal static class Colums
            {
                internal static class Col_Planificacion
                {
                    internal const string Uid = "0";
                    internal const string dbField = "U_ClgCode";
                }

                internal static class Col_Folio
                {
                    internal const string Uid = "1";
                    internal const string dbField = "U_FolioGuia";
                }

                internal static class Col_Oc
                {
                    internal const string Uid = "2";
                    internal const string dbField = "U_DocEntry";
                }

                internal static class Col_Productor
                {
                    internal const string Uid = "3";
                    internal const string dbField = "U_CardCode";
                }

                internal static class Col_CardName
                {
                    internal const string Uid = "4";
                    internal const string dbField = "U_RznSoc";
                }

                internal static class Col_ItemCode
                {
                    internal const string Uid = "5";
                    internal const string dbField = "U_ItemCode";
                }

                internal static class Col_Fruta
                {
                    internal const string Uid = "6";
                    internal const string dbField = "U_Fruta";
                }

                internal static class Col_Variedad
                {
                    internal const string Uid = "7";
                    internal const string dbField = "U_Variedad";
                }

                internal static class Col_KilosGuia
                {
                    internal const string Uid = "8";
                    internal const string dbField = "U_KilosGuia";
                }

                internal static class Col_TipoEnv
                {
                    internal const string Uid = "9";
                    internal const string dbField = "U_CodEnvase";
                }

                internal static class Col_CantEnv
                {
                    internal const string Uid = "10";
                    internal const string dbField = "U_Envases";
                }

                internal static class Col_Obs
                {
                    internal const string Uid = "11";
                    internal const string dbField = "U_Obs";
                }

                internal static class Col_BaseLine
                {
                    internal const string Uid = "12";
                    internal const string dbField = "U_BaseLine";
                }
                internal static class Col_Tipo
                {
                    internal const string Uid = "13";
                    internal const string dbField = "U_Tipo";
                }
            }
        }

        internal static class MatrixEnvase
        {
            internal const string Uid = "64";

            internal static class Colums
            {
                internal static class Col_LineId
                {
                    internal const string Uid = "#";
                    internal const string dbField = "LineId";
                }

                internal static class Col_Envase
                {
                    internal const string Uid = "0";
                    internal const string dbField = "U_CodEnvase";
                }

                internal static class Col_Cantidad
                {
                    internal const string Uid = "1";
                    internal const string dbField = "U_Envases";
                }

                internal static class Col_Propiedad
                {
                    internal const string Uid = "2";
                    internal const string dbField = "U_PropEnv";
                }
            }
        }
    }
}