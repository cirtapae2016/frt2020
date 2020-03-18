using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Xml;

namespace pluginAsignaLote

{
    internal static class frmAsignaLote

    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;

            if (oMenuEvent.BeforeAction)
            {
                FormCreationPackage = (FormCreationParams)sbo_application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);

                try
                {
                    if (string.IsNullOrEmpty(sessionId))
                        sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                    string contenidoArchivo = Properties.Resources.ResourceManager.GetString(pluginForm.FormType);
                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;

                    FormCreationPackage.UniqueID = pluginForm.FormType + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    FormCreationPackage.UniqueID = "AsignaLote" + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    DBDataSource dbData = oForm.DataSources.DBDataSources.Add("OBTN");

                    Matrix oMatrix = (Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific;
                    oMatrix.Item.Enabled = false;

                    Column oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_ItemCode.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_ItemCode.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_itemName.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_itemName.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_SysNumber.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_SysNumber.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_DistNumber.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_DistNumber.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_MnfSerial.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_MnfSerial.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_InDate.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_InDate.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Status.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_Status.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Quantity.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_Quantity.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Balance.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_Balance.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Variedad.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Variedad.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Tipo.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Tipo.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Calibre.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Calibre.dbField);
                    oCol.Editable = false;

                    oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.Uid);
                    oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.dbField);

                    SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    string sSql = " SELECT T0.\"U_DFO_Valor\",T0.\"U_DFO_Descrip\"   FROM \"@DFO_OPDFO\"  T0 " +
                                "where T0.\"U_DFO_Tipo\" = 'DESTINOSUGERIDO' ";

                    oRS.DoQuery(sSql);
                    if (oRS.RecordCount != 0)
                    {
                        while (!oRS.EoF)
                        {
                            string code = oRS.Fields.Item("U_DFO_Valor").Value.ToString();
                            string name = oRS.Fields.Item("U_DFO_Descrip").Value.ToString();
                            oCol.ValidValues.Add(code, name);
                            oRS.MoveNext();
                        }
                    }
                    oCol.DisplayDesc = true;
                    oCol.Editable = true;

                    //oConditions = (SAPbouiCOM.Conditions)oApplication.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_Conditions);
                    //oCondition = oConditions.Add();
                    //oCondition.Alias = "your field that you need filtering";
                    //oCondition.Operation = SAPbouiCOM.BoConditionOperation.co_LESS_THAN; //Your condition
                    //oCondition.CondVal = "condition value";

                    ////run your query in your table with your conditions
                    //oDBDS.Query(oConditions);

                    dbData.Query(null);

                    oMatrix.LoadFromDataSourceEx(true);

                    oForm.Visible = true;
                }
                catch
                {
                    throw;
                }
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (oItemEvent.ItemUID)
            {
                case pluginForm.ButtonOK:
                    ButtonOk(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonCancel:
                    ButtonCacel(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.MatrixLote.Uid:
                    MatrixLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        private static void ButtonOk(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                    if (oForm.Mode == BoFormMode.fm_UPDATE_MODE)
                    {
                        DBDataSource dbData = oForm.DataSources.DBDataSources.Add("OBTN");

                        Matrix oMatrix = (Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific;

                        Column oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_DistNumber.Uid);
                        oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_DistNumber.dbField);

                        oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.Uid);
                        oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.dbField);

                        dbData.Clear();
                        oMatrix.FlushToDataSource();

                        for (int i = 0; i <= dbData.Size - 1; i++)
                        {
                            if (dbData.GetValue(pluginForm.MatrixLote.Colums.Col_DistNumber.dbField, i) != null)
                            {
                                if (dbData.GetValue(pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.dbField, i) != "")
                                {
                                    string BatchNum = dbData.GetValue(pluginForm.MatrixLote.Colums.Col_DistNumber.dbField, i);
                                    var batch = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{BatchNum}'", sessionId, out _).DeserializeJsonObject<BatchNumberDetails>();
                                    if (batch.DocEntry != 0)
                                    {
                                        batch.U_FRU_Destino = dbData.GetValue(pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.dbField, i);

                                        CommonFunctions.PATCH(ServiceLayer.BatchNumberDetails, batch, batch.DocEntry.ToString(), sessionId, out _);
                                    }
                                }
                            }
                        }
                        BindMatrixData(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    }
                    oForm.Mode = BoFormMode.fm_OK_MODE;
                }
            }
        }

        private static void ButtonCacel(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                }
            }
        }

        private static void MatrixLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    Matrix oMatrix = (Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific;
                    oMatrix.Item.Enabled = true;
                }
            }
        }

        private static void BindMatrixData(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            DBDataSource dbData = oForm.DataSources.DBDataSources.Add("OBTN");

            Matrix oMatrix = (Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific;

            Column oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_ItemCode.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_ItemCode.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_itemName.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_itemName.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_SysNumber.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_SysNumber.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_DistNumber.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_DistNumber.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_MnfSerial.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_MnfSerial.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_InDate.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_InDate.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Status.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_Status.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Quantity.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_Quantity.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Balance.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_Balance.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Variedad.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Variedad.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Tipo.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Tipo.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Calibre.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Calibre.dbField);

            oCol = oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.Uid);
            oCol.DataBind.SetBound(true, "OBTN", pluginForm.MatrixLote.Colums.Col_U_FRU_Destino.dbField);

            dbData.Query(null);

            oMatrix.LoadFromDataSourceEx(true);
        }
    }
}