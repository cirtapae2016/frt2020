using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;

namespace pluginFumigado
{
    internal static class frmFumigado
    {
        internal static void FormLoad(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                try
                {
                    oForm.Freeze(true);
                    var itemRef = oForm.Items.Item(pluginForm.ButtonCopy);
                    var btnFumigar = oForm.Items.Add(pluginForm.ButtonFumigar, BoFormItemTypes.it_BUTTON);
                    btnFumigar.Top = itemRef.Top;
                    btnFumigar.Width = itemRef.Width;
                    btnFumigar.Left = itemRef.Left - itemRef.Width;
                    btnFumigar.Height = itemRef.Height;
                    btnFumigar.Enabled = true;
                    ((Button)btnFumigar.Specific).Caption = "Fumigar";
                }
                finally
                {
                    oForm.Freeze(false);
                }
            }
        }

        internal static void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (businessObjectInfo.EventType)
            {
                case BoEventTypes.et_FORM_DATA_ADD:
                    FormDataAdd(ref businessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (oItemEvent.ItemUID)
            {
                case pluginForm.ButtonFumigar:
                    ButtonFumigar(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        private static void ButtonFumigar(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            //throw new NotImplementedException();
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var oForm = sbo_application.Forms.Item(formUID);
                    if (oForm.Mode != BoFormMode.fm_OK_MODE)
                    {
                        bBubbleEvent = false;
                        if (oForm.Mode == BoFormMode.fm_ADD_MODE)
                            throw new Exception("Debe crear el documento antes");

                        if (oForm.Mode == BoFormMode.fm_UPDATE_MODE)
                            throw new Exception("Debe grabar los cambios antes");

                        if (oForm.Mode == BoFormMode.fm_FIND_MODE)
                            throw new Exception("Debe buscar un registro antes");
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var oForm = sbo_application.Forms.Item(formUID);
                    var Doc = CommonFunctions.GET(ServiceLayer.StockTransfers, oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0), null, sessionId, out _).DeserializeJsonObject<StockTransfer>();

                    var lotes = new List<RegistroCalidad_Lotes>();

                    foreach (var line in Doc.StockTransferLines)
                    {
                        foreach (var lote in line.BatchNumbers)
                        {
                            var loteCalidad = new RegistroCalidad_Lotes
                            {
                                DocEntry = null,
                                LineId = null,
                                U_BatchNum = lote.BatchNumber,
                                U_Kg = lote.Quantity
                            };

                            lotes.Add(loteCalidad);
                        }
                    }

                    dynamic Cabecera = new System.Dynamic.ExpandoObject();

                    Cabecera.Tipo = "67";
                    Cabecera.Valor = Doc.DocNum;
                    Cabecera.Lote = lotes.SerializeJson();

                    var Fruta = oForm.DataSources.DBDataSources.Item("WTR1").GetValue("Dscription", 0);

                    if (Fruta.Contains("Pasa"))
                    {
                        var oFormCalidad = SAPFunctions.LoadFormCalidad(ref sbo_application, "PASA-20016-RG-6.1.1", sessionId, Cabecera);
                    }
                    if (Fruta.Contains("Cir"))
                    {
                        var oFormCalidad = SAPFunctions.LoadFormCalidad(ref sbo_application, "CIRUELA-GRAL-RG-6.1.1", sessionId, Cabecera);
                    }
                    if (Fruta.Contains("Nue"))
                    {
                        var oFormCalidad = SAPFunctions.LoadFormCalidad(ref sbo_application, "CIRUELA-GRAL-RG-6.1.1", sessionId, Cabecera);
                    }
                }
            }
        }
        private static void FormDataAdd(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (businessObjectInfo.BeforeAction)
            {

            }

            if (!businessObjectInfo.BeforeAction)
            {
                if (businessObjectInfo.ActionSuccess)
                {
                    var oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);
                    var DocEntry = oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0);
                    var Transf = CommonFunctions.GET(ServiceLayer.StockTransfers, DocEntry, null, sessionId, out System.Net.HttpStatusCode statusCode).DeserializeJsonObject<StockTransfer>();

                    if (!string.IsNullOrEmpty(Transf.U_DTE_FolioRef))
                    {
                        var Of = CommonFunctions.GET(ServiceLayer.ProductionOrders, null, $"?$filter=DocumentNumber eq {Transf.U_DTE_FolioRef}", sessionId, out statusCode).DeserializeJsonObject<ProductionOrder>();

                        foreach (var line in Transf.StockTransferLines)
                        {
                            foreach (var lot in line.BatchNumbers)
                            {
                                var ofLine = Of.ProductionOrderLines.Where(i => i.ItemNo == line.ItemCode).Single();

                                ofLine.BatchNumbers.Add(new BatchNumbers
                                {
                                    BatchNumber = lot.BatchNumber,
                                    Quantity = lot.Quantity,
                                    BaseLineNumber = (int)ofLine.LineNumber
                                });
                            }
                        }

                        var response = CommonFunctions.PATCH(ServiceLayer.ProductionOrders, Of, Of.AbsoluteEntry.ToString(), sessionId, out statusCode);
                        if (statusCode != System.Net.HttpStatusCode.NoContent)
                        {
                            var _Error = response.DeserializeJsonToDynamic();
                            throw new Exception($"Error en el registro : {_Error.error.message.value.ToString()}");
                        }
                        else
                        {
                            sbo_application.MessageBox($"Lotes asignados correctamente a la OF {Of.DocumentNumber}");
                        }
                    }
                }
            }
        }
    }
}