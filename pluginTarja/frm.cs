using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;

namespace pluginTarja
{
    internal static class frm
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
                var itemRef = oForm.Items.Item(pluginForm.ButtonQty);
                var btnFumigar = oForm.Items.Add(pluginForm.ButtonImprimir, BoFormItemTypes.it_BUTTON);
                btnFumigar.Top = itemRef.Top;
                btnFumigar.Width = itemRef.Width;
                btnFumigar.Left = itemRef.Left - itemRef.Width;
                btnFumigar.Height = itemRef.Height;
                btnFumigar.Enabled = true;
                ((Button)btnFumigar.Specific).Caption = "Imprimir";
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (oItemEvent.ItemUID)
            {
                case pluginForm.ButtonImprimir:
                    ButtonPrint(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        internal static void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (businessObjectInfo.EventType)
            {
                case BoEventTypes.et_FORM_DATA_UPDATE:
                    FormDataUpdate(ref businessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        private static void FormDataUpdate(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (businessObjectInfo.BeforeAction)
            {

            }

            if (!businessObjectInfo.BeforeAction)
            {
                if (businessObjectInfo.ActionSuccess)
                {
                    if (!sbo_company.CompanyDB.Contains("FRUTEXSA"))
                    {
                        var oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);
                        var LoteId = oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DistNumber", 0);
                        var DocEntry = oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("AbsEntry", 0);

                        var _Log = new Login { UserName = "Intercompany", Password = "mngr", CompanyDB = "FRUTEXSA" };
                        var sessionIc = CommonFunctions.POST(ServiceLayer.Login, _Log, null, out _);
                        var response = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{LoteId}'", sessionIc, out System.Net.HttpStatusCode statusCode);

                        if (statusCode == System.Net.HttpStatusCode.OK)
                        {
                            var batchFrutexsa = response.DeserializeJsonObject<BatchNumberDetails>();
                            var AbsEntry = batchFrutexsa.DocEntry;

                            var batchPlanta = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, DocEntry, null, sessionId, out statusCode).DeserializeJsonObject<BatchNumberDetails>();
                            batchPlanta.CopyProperties(batchFrutexsa);
                            batchFrutexsa.DocEntry = AbsEntry;

                            response = CommonFunctions.PATCH(ServiceLayer.BatchNumberDetails, batchFrutexsa, AbsEntry.ToString(), sessionIc, out statusCode);

                            if (statusCode != System.Net.HttpStatusCode.NoContent)
                                throw new Exception($"Error actualizando el lote en Frutexsa : {response.DeserializeJsonToDynamic().error.message.value.ToString()}");

                            CommonFunctions.POST(ServiceLayer.Logout, null, sessionIc, out _);
                        }
                    }
                }
            }
        }

        private static void ButtonPrint(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
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
                            throw new Exception("Debe crear el lote antes");

                        if (oForm.Mode == BoFormMode.fm_UPDATE_MODE)
                            throw new Exception("Debe grabar los cambios antes");

                        if (oForm.Mode == BoFormMode.fm_FIND_MODE)
                            throw new Exception("Debe buscar un lote antes");
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var oForm = sbo_application.Forms.Item(formUID);
                    SAPFunctions.PrintLayout("BTN10003", int.Parse(oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("AbsEntry", 0)), sbo_company);
                }
            }
        }
    }
}