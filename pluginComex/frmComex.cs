using CoreSAPB1;
using CoreUtilities;
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Linq;
using System.Xml;

namespace pluginComex
{
    internal static class frmComex

    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;
            Recordset rs = (Recordset)sbo_company.GetBusinessObject(BoObjectTypes.BoRecordset);

            if (oMenuEvent.BeforeAction)
            {
                FormCreationPackage = (FormCreationParams)sbo_application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
                try
                {
                    if (string.IsNullOrEmpty(sessionId))
                        sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                    string contenidoArchivo = Properties.Resources.ResourceManager.GetString(pluginForm.FormType);
                    string date = DateTime.Now.ToString("yyyyMMdd");
                    string time = DateTime.Now.ToString("hh:mm");

                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                    FormCreationPackage.UniqueID = pluginForm.FormType + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    try
                    {
                        oForm.DataBrowser.BrowseBy = pluginForm.TxtDocEntry.Uid;
                        ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).Columns.Item(pluginForm.MtxOV.Columns.Col_OvCardCode.Uid).Visible = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).Columns.Item(pluginForm.MtxOV.Columns.Col_OvDocEntry.Uid).Visible = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).Columns.Item(pluginForm.MtxOV.Columns.Col_ItemCode.Uid).Visible = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).AutoResizeColumns();

                        var oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLNav);
                        Conditions oCons = oCFL.GetConditions();
                        Condition oCon = oCon = oCons.Add();
                        oCon.Alias = "CardType";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "S";
                        oCFL.SetConditions(oCons);

                        oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLAg);
                        oCons = oCFL.GetConditions();
                        oCon = oCon = oCons.Add();
                        oCon.Alias = "CardType";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "S";
                        oCFL.SetConditions(oCons);

                        oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLTr);
                        oCons = oCFL.GetConditions();
                        oCon = oCons.Add();
                        oCon.Alias = "CardType";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "S";
                        oCon.Relationship = BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "QryGroup2";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "Y";
                        oCFL.SetConditions(oCons);

                        var sql = "select \"DocEntry\" from ORDR where \"DocStatus\"='O' and \"CANCELED\"='N' and \"Confirmed\"='Y' and \"DocEntry\" not in (select \"U_DocEntry\" from \"@DFO_EMB1\")";
                        rs.DoQuery(sql);

                        oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLOv);
                        oCons = oCFL.GetConditions();
                        oCon = oCon = oCons.Add();
                        oCon.Alias = "DocEntry";
                        oCon.Operation = BoConditionOperation.co_EQUAL;

                        while (!rs.EoF)
                        {
                            oCon.CondVal = rs.Fields.Item(0).Value.ToString();
                            rs.MoveNext();

                            if (!rs.EoF)
                            {
                                oCon.Relationship = BoConditionRelationship.cr_OR;
                                oCon = oCons.Add();
                                oCon.Alias = "DocEntry";
                                oCon.Operation = BoConditionOperation.co_EQUAL;
                            }
                        }

                        oCFL.SetConditions(oCons);

                        ((Button)oForm.Items.Item(pluginForm.ButtonAddOVs).Specific).ChooseFromListUID = pluginForm.CFLOv;

                        ((ButtonCombo)oForm.Items.Item(pluginForm.ButtonPrint).Specific).ValidValues.Add("1", "Aduana");
                        ((ButtonCombo)oForm.Items.Item(pluginForm.ButtonPrint).Specific).ValidValues.Add("2", "Transportista");
                        ((ButtonCombo)oForm.Items.Item(pluginForm.ButtonPrint).Specific).ValidValues.Add("3", "Planta");

                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtDocDate.dbField, 0, date);
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbDetalle).Clear();
                    }
                    catch (Exception ex)
                    {
                        sbo_application.StatusBar.SetText(ex.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    }
                    finally { oForm.Freeze(false); oForm.Visible = true; }
                }
                catch (Exception e)
                {
                    sbo_application.MessageBox(e.Message);
                }
            }
        }

        internal static void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            switch (businessObjectInfo.EventType)
            {
                case BoEventTypes.et_FORM_DATA_LOAD:
                    FormDataLoad(ref businessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case BoEventTypes.et_FORM_DATA_UPDATE:
                case BoEventTypes.et_FORM_DATA_ADD:
                    FormDataOVupdate(ref businessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        internal static void MenuEventHandler(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (!oMenuEvent.BeforeAction)
            {
                Form oForm;
                try { oForm = sbo_application.Forms.ActiveForm as Form; }
                catch { return; }

                if (oForm.TypeEx == pluginForm.FormType)
                {
                    switch (oMenuEvent.MenuUID)
                    {
                        case SAPMenu.New:
                            break;

                        case SAPMenu.Find:
                            break;
                    }
                }
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (oItemEvent.FormTypeEx)
            {
                case pluginForm.FormType:
                    switch (oItemEvent.ItemUID)
                    {
                        case pluginForm.TxtCardCodeNav.Uid:
                            TxtCardCodeNav(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.TxtCardCodeAg.Uid:
                            TxtCardCodeAg(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.TxtCardCodeTransp.Uid:
                            TxtCardCodeTransp(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.MtxOV.Uid:
                            MtxOV(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddOVs:
                            ButtonAddOVs(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonRefresh:
                            ButtonRefresh(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonPrint:
                            ButtonPrint(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ChkMonday.Uid:
                        case pluginForm.ChkTuesday.Uid:
                        case pluginForm.ChkWednesday.Uid:
                        case pluginForm.ChkThursday.Uid:
                        case pluginForm.ChkFriday.Uid:
                        case pluginForm.ChkSaturday.Uid:
                        case pluginForm.ChkSunday.Uid:
                            ChkStacking(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;
                    }
                    break;
            }
        }

        private static void ButtonPrint(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_COMBO_SELECT)
                {
                    var objComex = CommonFunctions.GET(ServiceLayer.Embarque, oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0), null, sessionId, out System.Net.HttpStatusCode statusCode).DeserializeJsonObject<Embarque>();

                    if (statusCode == System.Net.HttpStatusCode.OK)
                    {
                        switch (int.Parse(((ButtonCombo)(oForm.Items.Item(pluginForm.ButtonPrint).Specific)).Caption))
                        {
                            case 1:
                                SAPFunctions.PrintAduana(objComex, sessionId, sbo_company);
                                break;

                            case 2:
                                SAPFunctions.PrintTransporte(objComex, sessionId, sbo_company);
                                break;

                            case 3:
                                throw new Exception("No habilitado");
                        }
                    }
                    else
                    {
                        throw new Exception("Documento no encontrado");
                    }
                }
                //((ButtonCombo)(oForm.Items.Item(pluginForm.ButtonPrint).Specific)).Caption = "Imprimir";
            }
        }

        private static void FormDataLoad(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (businessObjectInfo.BeforeAction)
            {
            }

            if (!businessObjectInfo.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);
                ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).AutoResizeColumns();
                if (oForm.Mode == BoFormMode.fm_OK_MODE || oForm.Mode == BoFormMode.fm_UPDATE_MODE) ((Button)oForm.Items.Item(pluginForm.ButtonRefresh).Specific).Item.Enabled = true;
            }
        }

        private static void ButtonRefresh(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbDetalle);

                    for (int i = 0; i < det.Size; i++)
                    {
                    }

                    ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).LoadFromDataSourceEx();
                    ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).AutoResizeColumns();
                }

                //if (oForm.Mode == BoFormMode.fm_OK_MODE) oForm.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void ChkStacking(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    ((EditText)oForm.Items.Item(pluginForm.TxtMondayFrom.Uid).Specific).Item.Click();
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    string editDate = "";
                    bool check = false;
                    switch (oItemEvent.ItemUID)
                    {
                        case pluginForm.ChkMonday.Uid:
                            editDate = pluginForm.TxtMonDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkMonday.Uid).Specific).Checked;
                            break;

                        case pluginForm.ChkTuesday.Uid:
                            editDate = pluginForm.TxtTueDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkTuesday.Uid).Specific).Checked;
                            break;

                        case pluginForm.ChkWednesday.Uid:
                            editDate = pluginForm.TxtWedDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkWednesday.Uid).Specific).Checked;
                            break;

                        case pluginForm.ChkThursday.Uid:
                            editDate = pluginForm.TxtThuDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkThursday.Uid).Specific).Checked;
                            break;

                        case pluginForm.ChkFriday.Uid:
                            editDate = pluginForm.TxtFriDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkFriday.Uid).Specific).Checked;
                            break;

                        case pluginForm.ChkSaturday.Uid:
                            editDate = pluginForm.TxtSatDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkSaturday.Uid).Specific).Checked;
                            break;

                        case pluginForm.ChkSunday.Uid:
                            editDate = pluginForm.TxtSunDt.Uid;
                            check = ((CheckBox)oForm.Items.Item(pluginForm.ChkSunday.Uid).Specific).Checked;
                            break;

                        default:
                            break;
                    }

                    var oEdit = oForm.Items.Item(editDate).Specific as EditText;
                    if (check)
                    {
                        oEdit.Item.Visible = true;
                    }
                    else
                    {
                        oEdit.Item.Visible = false;
                    }
                }
            }
        }

        private static void ButtonAddOVs(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    try
                    {
                        var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                        if (oDT != null)
                        {
                            //((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).Clear();
                            DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbDetalle);
                            var rs = sbo_company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                            string CardCode = string.Empty;
                            string sql;

                            for (int i = 0; i < oDT.Rows.Count; i++)
                            {
                                var DocEntry = oDT.GetValue("DocEntry", i).ToString();
                                Order oNv = CommonFunctions.GET(ServiceLayer.Orders, DocEntry, null, sessionId, out _).DeserializeJsonObject<Order>();

                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_DirFact", 0, $"{oNv.PayToCode} \r\n{oDT.GetValue("Address", i)}");
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_DirDesp", 0, $"{oNv.ShipToCode} \r\n{oDT.GetValue("Address2", i)}");
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Destino", 0, $"{oNv.U_DTE_IdAdicPtoDesemb}");

                                foreach (var item in oNv.DocumentLines)
                                {
                                    int offset = det.Size;
                                    det.InsertRecord(offset);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_OvDocEntry.dbField, offset, oDT.GetValue("DocEntry", i).ToString());
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_OvDocNum.dbField, offset, oDT.GetValue("DocNum", i).ToString());
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_OvCardCode.dbField, offset, oDT.GetValue("CardCode", i).ToString());
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_OvCardName.dbField, offset, oDT.GetValue("CardName", i).ToString());
                                    //det.SetValue(pluginForm.MtxOV.Columns.Col_OvDestino.dbField, offset, (string.IsNullOrEmpty(oDT.GetValue("U_DTE_IdAdicPtoDesemb", i).ToString())) ? "" : oDT.GetValue("U_DTE_IdAdicPtoDesemb", i).ToString()); //"");
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_OvATA.dbField, offset, "");
                                    //det.SetValue(pluginForm.MtxOV.Columns.Col_Planta.dbField, offset, string.IsNullOrEmpty(oDT.GetValue("U_DFO_Planta", i).ToString()) ? "": oDT.GetValue("U_DFO_Planta", i).ToString());
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Planta.dbField, offset, item.WarehouseCode);
                                    //det.SetValue(pluginForm.MtxOV.Columns.Col_FechaPlanta.dbField, offset, (string.IsNullOrEmpty(oDT.GetValue("U_DFO_PlantaDate", i).ToString()))? "": DateTime.Parse(oDT.GetValue("U_DFO_PlantaDate", i).ToString()).ToString("yyyyMMdd"));
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_HoraPlanta.dbField, offset, oDT.GetValue("U_DFO_PlantaHour", i).ToString());
                                    //det.SetValue(pluginForm.MtxOV.Columns.Col_FechaSAG.dbField, offset, (string.IsNullOrEmpty(oDT.GetValue("U_DFO_SAGDate", i).ToString())) ? "" : DateTime.Parse(oDT.GetValue("U_DFO_SAGDate", i).ToString()).ToString("yyyyMMdd"));
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_HoraSAG.dbField, offset, oDT.GetValue("U_DFO_SAGHour", i).ToString());
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_ItemCode.dbField, offset, item.ItemCode);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_ItemName.dbField, offset, item.ItemDescription);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Qty.dbField, offset, item.Quantity.GetStringFromDouble(2));
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Price.dbField, offset, item.Price);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Variedad.dbField, offset, item.U_FRU_Variedad);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Tipo.dbField, offset, item.U_FRU_Tipo);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Calibre.dbField, offset, item.U_FRU_Calibre);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Color.dbField, offset, item.U_FRU_Color);
                                    //det.SetValue(pluginForm.MtxOV.Columns.Col_Conteo.dbField, offset, item.U_FRU_Conteo);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Caracteristica.dbField, offset, item.U_FRU_Caracteristica);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_DescCli.dbField, offset, item.U_FRU_DescripcionCliente);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_DescPlanta.dbField, offset, item.U_FRU_DescripcionAduana);
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_BaseLine.dbField, offset, item.LineNum.ToString());
                                    det.SetValue(pluginForm.MtxOV.Columns.Col_Pedido.dbField, offset, oDT.GetValue("NumAtCard", i).ToString());
                                    CardCode = oDT.GetValue("CardCode", i).ToString();
                                }
                            }

                            sql = $"select top 1 \"U_NotD1\", \"U_NotD2\", \"U_NotD3\", \"U_NotD4\", \"U_NotD5\", \"U_NotD6\", \"U_NotD7\", \"U_Not2D1\", \"U_Not2D2\", \"U_Not2D3\", \"U_Not2D4\", \"U_Not2D5\", \"U_Not2D6\", \"U_Not2D7\", \"U_Notif1\", \"U_Notif2\" from \"@DFO_OEMB\" where \"DocEntry\" = (select max(\"DocEntry\") from \"@DFO_EMB1\" where \"U_CardCode\"='{CardCode}') ";
                            rs.DoQuery(sql);

                            if (rs.RecordCount != 0)
                            {
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD1", 0, rs.Fields.Item(0).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD2", 0, rs.Fields.Item(1).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD3", 0, rs.Fields.Item(2).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD4", 0, rs.Fields.Item(3).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD5", 0, rs.Fields.Item(4).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD6", 0, rs.Fields.Item(5).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_NotD7", 0, rs.Fields.Item(6).Value.ToString());

                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D1", 0, rs.Fields.Item(7).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D2", 0, rs.Fields.Item(8).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D3", 0, rs.Fields.Item(9).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D4", 0, rs.Fields.Item(10).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D5", 0, rs.Fields.Item(11).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D6", 0, rs.Fields.Item(12).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Not2D7", 0, rs.Fields.Item(13).Value.ToString());

                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Notif1", 0, rs.Fields.Item(14).Value.ToString());
                                oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Notif2", 0, rs.Fields.Item(15).Value.ToString());
                            }
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).LoadFromDataSourceEx();
                        ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).AutoResizeColumns();
                    }
                }

                if (oForm.Mode == BoFormMode.fm_OK_MODE)
                    oForm.Mode = BoFormMode.fm_UPDATE_MODE;
            }
        }

        private static void MtxOV(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_MATRIX_LINK_PRESSED)
                {
                    var oForm = sbo_application.Forms.Item(formUID);
                    var oMatrix = oForm.Items.Item(pluginForm.MtxOV.Uid).Specific as Matrix;

                    if (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_OvDocNum.Uid)
                    {
                        oForm.Freeze(true);
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_OvDocEntry.Uid).Visible = true;
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_OvDocEntry.Uid).Cells.Item(oItemEvent.Row).Click(BoCellClickType.ct_Linked, 0);
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_OvDocEntry.Uid).Visible = false;

                        oForm.Freeze(false);
                        bBubbleEvent = false;
                    }

                    if (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_OvCardName.Uid)
                    {
                        oForm.Freeze(true);
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_OvCardCode.Uid).Visible = true;
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_OvCardCode.Uid).Cells.Item(oItemEvent.Row).Click(BoCellClickType.ct_Linked, 0);
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_OvCardCode.Uid).Visible = false;

                        oForm.Freeze(false);
                        bBubbleEvent = false;
                    }

                    if (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_ItemName.Uid)
                    {
                        oForm.Freeze(true);
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_ItemCode.Uid).Visible = true;
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_ItemCode.Uid).Cells.Item(oItemEvent.Row).Click(BoCellClickType.ct_Linked, 0);
                        oMatrix.Columns.Item(pluginForm.MtxOV.Columns.Col_ItemCode.Uid).Visible = false;

                        oForm.Freeze(false);
                        bBubbleEvent = false;
                    }
                }
            }
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
                {
                    try
                    {
                        var oForm = sbo_application.Forms.Item(formUID);
                        var oMatrix = oForm.Items.Item(pluginForm.MtxOV.Uid).Specific as Matrix;
                        if ((oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_Planta.Uid) ||
                           (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_FechaPlanta.Uid) ||
                           (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_HoraPlanta.Uid) ||
                           (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_FechaSAG.Uid) ||
                           (oItemEvent.ColUID == pluginForm.MtxOV.Columns.Col_HoraSAG.Uid))
                        {
                            var row = oItemEvent.Row - 1;

                            var dbDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbDetalle);
                            oMatrix.FlushToDataSource();
                            var DocEntryOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_OvDocEntry.dbField, row);
                            var PlantaOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_Planta.dbField, row);
                            var FechaPlantaOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_FechaPlanta.dbField, row);
                            var HoraPlantaOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_HoraPlanta.dbField, row);
                            var FechaSAGOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_FechaSAG.dbField, row);
                            var HoraOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_HoraSAG.dbField, row);

                            for (var i = 0; i <= dbDataSource.Size - 1; i++)
                            {
                                if (dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_OvDocEntry.dbField, i) == DocEntryOV)
                                {
                                    dbDataSource.SetValue(pluginForm.MtxOV.Columns.Col_Planta.dbField, i, PlantaOV);
                                    dbDataSource.SetValue(pluginForm.MtxOV.Columns.Col_FechaPlanta.dbField, i, FechaPlantaOV);
                                    dbDataSource.SetValue(pluginForm.MtxOV.Columns.Col_HoraPlanta.dbField, i, HoraPlantaOV);
                                    dbDataSource.SetValue(pluginForm.MtxOV.Columns.Col_FechaSAG.dbField, i, FechaSAGOV);
                                    dbDataSource.SetValue(pluginForm.MtxOV.Columns.Col_HoraSAG.dbField, i, HoraOV);
                                }
                            }
                            ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).LoadFromDataSourceEx();
                            ((Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific).AutoResizeColumns();
                        }
                    }
                    catch (Exception e) { sbo_application.MessageBox(e.Message); }
                }
                /*
                if (oItemEvent.EventType == BoEventTypes.et_DOUBLE_CLICK)
                {
                    var row = oItemEvent.Row - 1;
                    var oForm = sbo_application.Forms.Item(formUID);
                    var oMatrix = oForm.Items.Item(pluginForm.MtxOV.Uid).Specific as Matrix;
                    var dbDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbDetalle);
                    oMatrix.FlushToDataSource();
                    var DocEntryOV = dbDataSource.GetValue(pluginForm.MtxOV.Columns.Col_OvDocEntry.dbField, row);

                    var oFormComexDir = SAPFunctions.LoadFormComexDir(ref sbo_application, DocEntryOV) as Form;

                    ((EditText)oFormComexDir.Items.Item(CommonForms.FormComexDir.TxtFatherUID).Specific).Value = formUID;
                }
                */
            }
        }

        private static void TxtCardCodeTransp(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtCardCodeTransp.dbField, 0, oDT.GetValue("CardCode", 0).ToString());
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtCardNameTransp.dbField, 0, oDT.GetValue("CardName", 0).ToString());
                    }
                }
            }
        }

        private static void TxtCardCodeAg(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtCardCodeAg.dbField, 0, oDT.GetValue("CardCode", 0).ToString());
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtCardNameAg.dbField, 0, oDT.GetValue("CardName", 0).ToString());
                    }
                }
            }
        }

        private static void TxtCardCodeNav(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtCardCodeNav.dbField, 0, oDT.GetValue("CardCode", 0).ToString());
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue(pluginForm.TxtCardNameNav.dbField, 0, oDT.GetValue("CardName", 0).ToString());
                    }
                }
            }
        }

        private static void FormDataOVupdate(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (businessObjectInfo.BeforeAction)
            {
            }

            if (!businessObjectInfo.BeforeAction)
            {
                if (businessObjectInfo.ActionSuccess)
                {
                    try
                    {
                        var oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);
                        Matrix oMatrix = (Matrix)oForm.Items.Item(pluginForm.MtxOV.Uid).Specific;
                        var dbDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbDetalle);
                        oMatrix.FlushToDataSource();

                        var objComex = CommonFunctions.GET(ServiceLayer.Embarque, oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0), null, sessionId, out System.Net.HttpStatusCode statusCode).DeserializeJsonObject<Embarque>();

                        foreach (var det in objComex.DFO_EMB1Collection.GroupBy(i => i.U_DocEntry))
                        {
                            var ov = CommonFunctions.GET(ServiceLayer.Orders, det.Key.ToString(), null, sessionId, out _).DeserializeJsonObject<Order>();

                            ov.U_DFO_PlantaDate = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key).First().U_PlantaDate;
                            ov.U_DFO_PlantaHour = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key).First().U_PlantaHour;
                            ov.U_DFO_SAGDate = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key).First().U_SAGDate;
                            ov.U_DFO_SAGHour = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key).First().U_SAGHour;
                            ov.DocDueDate = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key).First().U_PlantaDate;

                            if (string.IsNullOrEmpty(sessionId))
                                sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                            foreach (var lines in ov.DocumentLines)
                            {
                                lines.U_FRU_DescripcionAduana = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key && i.U_BaseLine == lines.LineNum).First().U_DescAd;
                                lines.U_FRU_DescripcionCliente = objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == det.Key && i.U_BaseLine == lines.LineNum).First().U_DescCl;
                            }

                            CommonFunctions.PATCH(ServiceLayer.Orders, ov, ov.DocEntry.ToString(), sessionId, out statusCode);
                            /*
                            if (statusCode == System.Net.HttpStatusCode.NoContent)
                            {
                                var upd = CommonFunctions.GET(ServiceLayer.Orders, det.Key.ToString(), null, sessionId, out _).DeserializeJsonObject<Order>();
                                if (!string.IsNullOrEmpty(upd.U_DFO_PlantaDate))
                                {
                                    foreach (var lin in upd.DocumentLines)
                                    {
                                        string _Fruta = string.Empty;
                                        Notes notes = new Notes
                                        {
                                            Codigo = lin.ItemCode,
                                            Variedad = lin.U_FRU_Variedad,
                                            Fruta = _Fruta,
                                            RazonSocial = upd.CardName,
                                            LineNum = lin.LineNum.ToString()
                                        };

                                        string _notes = notes.SerializeJson();

                                        var Login = new Login { UserName = "Intercompany", Password = "mngr" };
                                        var SessionIc = string.Empty;

                                        int? DocEntryPlanta;
                                        switch (lin.WarehouseCode)
                                        {
                                            case "FRU-PAS":
                                                Login.CompanyDB = "TESTPASERA";
                                                SessionIc = CommonFunctions.POST(ServiceLayer.Login, Login, null, out _);
                                                break;

                                            case "FRU-PRO":
                                                Login.CompanyDB = "TESTPROCESADORA";
                                                SessionIc = CommonFunctions.POST(ServiceLayer.Login, Login, null, out _);
                                                break;
                                        }

                                        DocEntryPlanta = (int)CommonFunctions.GET(ServiceLayer.Orders, null, $"?$filter=U_IC_DocOrigen eq {ov.DocEntry}&$select=DocEntry", SessionIc, out statusCode).DeserializeJsonObject<Order>().DocEntry;
                                        if (DocEntryPlanta == 0 || DocEntryPlanta == null)
                                            throw new Exception("No se pudo agendar el cupo en la planta, consulte con depto TI");

                                        Activities Planificacion = new Activities
                                        {
                                            CardCode = upd.CardCode,
                                            ActivityProperty = "cn_Task",
                                            Details = $"Recepcion {_Fruta} {lin.U_FRU_Variedad}",
                                            StartDate = upd.DocDueDate,
                                            EndDueDate = upd.DocDueDate,
                                            DocEntry = DocEntryPlanta.ToString(),
                                            DocTypeEx = "17",
                                            Notes = _notes,
                                            U_DFO_CodFruta = _Fruta,
                                            U_DFO_Transportista = objComex.U_CardNameTransp,
                                            U_DFO_RutTransp = objComex.U_CardCodeTransp
                                        };

                                        //var Cupo = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, SessionIc, out statusCode);
                                        //if (statusCode != System.Net.HttpStatusCode.Created)
                                        //{
                                        //    var _Error = Cupo.DeserializeJsonToDynamic();
                                        //    throw new Exception($"Error en el registro : {_Error.error.message.value.ToString()}");
                                        //}
                                        //else
                                        //{
                                        //    sbo_application.StatusBar.SetText("Cupo agendado en la planta", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                                        //}
                                    }
                                }
                            }
                            */
                        }
                    }
                    catch { throw; }
                }
            }
        }
    }
}