using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Linq;
using System.Xml;

namespace pluginRecepcion
{
    internal static class frmRecepcion

    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;
            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

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
                        oForm.DataBrowser.BrowseBy = "10";
                        ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value = date;
                        ((EditText)oForm.Items.Item(pluginForm.TxtLlegada).Specific).Value = time;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();

                        ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLActividades);
                        Conditions oCons = oCFL.GetConditions();

                        Condition oCon = oCons.Add();
                        oCon.Alias = "Recontact";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value.Trim();
                        oCon.Relationship = BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "Closed";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";
                        oCon.Relationship = BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "inactive";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "N";
                        oCon.Relationship = BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "Action";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "T";
                        //oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;

                        //oCon = oCons.Add();
                        //oCon.Alias = "CntctType";
                        //oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        //oCon.CondVal = "1";
                        oCFL.SetConditions(oCons);

                        ////DETALLE ENVASE
                        oCFL = null;
                        oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLEnvases);
                        oCons = oCFL.GetConditions();

                        oCon = oCons.Add();
                        oCon.Alias = "U_Subfamilia";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        //oCon.Relationship = BoConditionRelationship.cr_AND;
                        oCon.CondVal = "BINS";

                        oCFL.SetConditions(oCons);

                        //oCFL = null;
                        //oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLEnvGuia);
                        //oCons = oCFL.GetConditions();

                        //oCon = oCons.Add();
                        //oCon.Alias = "U_Subfamilia";
                        //oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                        ////oCon.Relationship = BoConditionRelationship.cr_AND;
                        //oCon.CondVal = "BINS";

                        //oCFL.SetConditions(oCons);

                        //descomentar
                        //oCFL = null;
                        //oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLRutTransp);
                        //oCons = oCFL.GetConditions();
                        //oCon = oCons.Add();
                        //oCon.Alias = "CardType";
                        //oCon.Operation = BoConditionOperation.co_EQUAL;
                        //oCon.CondVal = "S";
                        //oCon.Relationship = BoConditionRelationship.cr_AND;
                        //oCon = oCons.Add();
                        //oCon.Alias = "QryGroup2";
                        //oCon.Operation = BoConditionOperation.co_EQUAL;
                        //oCon.CondVal = "Y";
                        //oCFL.SetConditions(oCons);

                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddGuia).Specific).ChooseFromListUID = pluginForm.CFLActividades;

                        //((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_TipoEnv.Uid).Visible = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_CantEnv.Uid).Visible = false;
                    }
                    catch (Exception e)
                    {
                        sbo_application.MessageBox(e.Message);
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
                            try
                            {
                                oForm.Freeze(true);
                                string date = DateTime.Now.ToString("yyyyMMdd");
                                string time = DateTime.Now.ToString("hh:mm");

                                ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value = date;
                                ((EditText)oForm.Items.Item(pluginForm.TxtLlegada).Specific).Value = time;
                                ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();

                                ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = false;
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;

                        case SAPMenu.Find:
                            try
                            {
                                oForm.Freeze(true);
                                ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Item.Enabled = true;
                                ((ComboBox)oForm.Items.Item(pluginForm.CmbDocStatus).Specific).Item.Enabled = true;

                                ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = false;
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
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
                        case pluginForm.ButtonAddGuia:
                            ButtonAddGuia(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.MatrixGuia.Uid:
                            MatrixGuia(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.CmbTipoRecepcion:
                            CmbTipoRecepcion(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonRefresh:
                            ButtonRefresh(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddEnv:
                            ButtonAddEnv(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.TxtTransportista:
                            TxtTransportista(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.TxtRutTransp:
                            TxtRutTransp(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonFinish:
                            CreateSAPDoc(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;
                    }
                    break;

                case CommonForms.FormEnvase.FormType:
                    switch (oItemEvent.ItemUID)
                    {
                        case CommonForms.FormEnvase.ButtonOK:
                            ButtonConfirmEnv(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case CommonForms.FormEnvase.TxtTipoEnvase.Uid:
                            TxtTipoEnvase(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case "":
                            CloseFormEnvase(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;
                    }
                    break;
            }
        }

        private static void TxtTransportista(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string Cookies)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        //((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Value = oDT.GetValue("CardName", 0).ToString();
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Transportista", 0, oDT.GetValue("CardName", 0).ToString());
                        //((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Value = oDT.GetValue("LicTradNum", 0).ToString();
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_RUTTransp", 0, oDT.GetValue("LicTradNum", 0).ToString());
                    }

                    //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific).Item.Click();
                    //((SAPbouiCOM.Button)oForm.Items.Item(pluginForm.ButtonOC).Specific).Item.Click();
                }
            }
        }

        private static void TxtRutTransp(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string Cookies)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        //((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Value = oDT.GetValue("CardName", 0).ToString();
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Transportista", 0, oDT.GetValue("CardName", 0).ToString());
                        //((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Value = oDT.GetValue("LicTradNum", 0).ToString();
                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_RUTTransp", 0, oDT.GetValue("LicTradNum", 0).ToString());
                    }
                }
            }
        }

        private static void CmbTipoRecepcion(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_COMBO_SELECT)
                {
                    if (((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Value == "F")
                    {
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Item.Enabled = true;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddGuia).Specific).Item.Enabled = true;
                        //((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Item.Enabled = true;
                        ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Item.Enabled = true;
                    }
                    else
                    {
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddGuia).Specific).Item.Enabled = false;
                        ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Item.Enabled = false;
                        ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Item.Enabled = false;
                    }
                }
            }
        }

        private static void MatrixGuia(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && oItemEvent.ColUID == pluginForm.MatrixGuia.Colums.Col_TipoEnv.Uid)
                {
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).FlushToDataSource();
                    DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbGuias);
                    string Fruta = det.GetValue(pluginForm.MatrixGuia.Colums.Col_TipoEnv.dbField, oItemEvent.Row - 1);

                    ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLEnvGuia);
                    Conditions oCons = oCFL.GetConditions();
                    if (oCons.Count == 0)
                    {
                        Condition oCon = oCons.Add();
                        oCon.Alias = "U_Subfamilia";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        //oCon.Relationship = BoConditionRelationship.cr_AND;
                        oCon.CondVal = "BINS";
                        oCFL.SetConditions(oCons);
                    }
                    else
                    {
                        Condition oCon = oCons.Item(0);
                        oCon.Alias = "U_Subfamilia";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        //oCon.Relationship = BoConditionRelationship.cr_AND;
                        oCon.CondVal = "BINS";
                        oCFL.SetConditions(oCons);
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && oItemEvent.ColUID == pluginForm.MatrixGuia.Colums.Col_TipoEnv.Uid)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        try
                        {
                            ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).FlushToDataSource();
                            DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbGuias);
                            det.SetValue(pluginForm.MatrixGuia.Colums.Col_TipoEnv.dbField, oItemEvent.Row - 1, oDT.GetValue("ItemCode", 0).ToString());
                            ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).LoadFromDataSourceEx();
                            ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();
                        }
                        catch (Exception e)
                        {
                            sbo_application.StatusBar.SetText(e.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
        }

        private static void ButtonAddGuia(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        try
                        {
                            DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbGuias);
                            int respuesta = sbo_application.MessageBox("¿Desea actualizar los datos del trannsportista y chofer con los datos de la programacion?", 1, "Si", "No");
                            det.Clear();
                            for (int i = 0; i < oDT.Rows.Count; i++)
                            {
                                int offset = det.Size;
                                det.InsertRecord(offset);

                                string[] recepcionPreData = new string[]
                                {
                                    oDT.GetValue("U_DFO_Transportista", 0).ToString(),
                                    oDT.GetValue("U_DFO_RutTransp", 0).ToString(),
                                    oDT.GetValue("U_DFO_Chofer", 0).ToString(),
                                    oDT.GetValue("U_DFO_RutChofer", 0).ToString(),
                                    oDT.GetValue("U_DFO_Patente", 0).ToString(),
                                    oDT.GetValue("U_DFO_Acoplado", 0).ToString()
                                };

                                if (recepcionPreData != null || recepcionPreData.Length > 0)
                                {
                                    if (respuesta == 1)
                                    {
                                        //((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Value = (recepcionPreData[0].Length > 0) ? recepcionPreData[0] : ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Value;
                                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Transportista", 0, (recepcionPreData[0].Length > 0) ? recepcionPreData[0] : ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Value);
                                        //((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Value = (recepcionPreData[1].Length > 0) ? recepcionPreData[1] : ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Value;
                                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_RUTTransp", 0, (recepcionPreData[1].Length > 0) ? recepcionPreData[1] : ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Value);
                                        //((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Value = (recepcionPreData[2].Length > 0) ? recepcionPreData[2] : ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Value;
                                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Chofer", 0, (recepcionPreData[2].Length > 0) ? recepcionPreData[2] : ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Value);
                                        //((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value = (recepcionPreData[3].Length > 0) ? recepcionPreData[3] : ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_RutChofer", 0, (recepcionPreData[3].Length > 0) ? recepcionPreData[3] : ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value);
                                        //((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Value = (recepcionPreData[4].Length > 0) ? recepcionPreData[4] : ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Value;
                                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Patente", 0, (recepcionPreData[4].Length > 0) ? recepcionPreData[4] : ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Value);
                                        //((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Value = (recepcionPreData[5].Length > 0) ? recepcionPreData[5] : ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Value;
                                        oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Carro", 0, (recepcionPreData[5].Length > 0) ? recepcionPreData[5] : ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Value);
                                    }
                                }

                                var notes = oDT.GetValue("Notes", i).ToString().DeserializeJsonObject<Notes>();

                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_Productor.dbField, offset, oDT.GetValue("CardCode", i).ToString());
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_Oc.dbField, offset, oDT.GetValue("DocEntry", i).ToString());
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_Planificacion.dbField, offset, oDT.GetValue("ClgCode", i).ToString());
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_ItemCode.dbField, offset, notes.Codigo);
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_Fruta.dbField, offset, notes.Fruta);
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_Variedad.dbField, offset, notes.Variedad);
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_Tipo.dbField, offset, notes.Tipo);
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_CardName.dbField, offset, notes.RazonSocial);
                                det.SetValue(pluginForm.MatrixGuia.Colums.Col_BaseLine.dbField, offset, notes.LineNum);
                            }

                            ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).LoadFromDataSourceEx();
                            ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();
                        }
                        catch
                        {
                            throw;
                        }
                    }
                }
            }
        }

        private static void FormDataLoad(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);

            if (businessObjectInfo.BeforeAction)
            {
            }

            if (!businessObjectInfo.BeforeAction)
            {
                try
                {
                    oForm.Freeze(true);
                    ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Item.Enabled = false;
                    ((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Item.Enabled = false;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkEncarpado).Specific).Item.Enabled = false;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkEstivado).Specific).Item.Enabled = false;
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = false;
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();
                }
                catch (Exception e) { sbo_application.MessageBox(e.Message); }
                finally { oForm.Freeze(false); }
            }
        }

        private static void ButtonRefresh(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    RefreshBalance(sbo_application, sessionId);
                }
            }
        }

        private static double? RefreshBalance(Application sbo_application, string sessionId)
        {
            try
            {
                var oForm = sbo_application.Forms.ActiveForm;
                string DocKey = oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0);

                if (string.IsNullOrEmpty(sessionId))
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                string response = CommonFunctions.GET(ServiceLayer.Recepcion, DocKey, null, sessionId, out _);
                Recepcion recepcion = response.DeserializeJsonObject<Recepcion>();

                double _env = 0;
                foreach (var env in recepcion.DFO_TRUCK5Collection)
                {
                    var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                    if (env.U_Envases < 0 || env.U_Envases == null)
                        throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                    if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                        throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                    _env += ((double)env.U_Envases * (double)item.InventoryWeight);
                }

                double _sumaLotes = recepcion.DFO_TRUCK2Collection.Sum(item => item.U_PesoLote);
                //double _sumaLotes = recepcion.DFO_TRUCK2Collection.Where(i=>i.U_Castigo>0).Sum(i=>i.U_PesoEnvase.GetDoubleFromString(";"));
                double _tara = (double)recepcion.U_KilosIngreso - (double)recepcion.U_KilosSalida;
                double _dif = _tara - _sumaLotes - _env;
                double _porc = Math.Round((Math.Abs(_dif) * 100) / _tara, 2);

                ((StaticText)oForm.Items.Item(pluginForm.LbPesoEntrada).Specific).Caption = ((double)recepcion.U_KilosIngreso).GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbPesoSalida).Specific).Caption = ((double)recepcion.U_KilosSalida).GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbTara).Specific).Caption = _tara.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbPesoEnvases).Specific).Caption = _env.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbPesoLotes).Specific).Caption = _sumaLotes.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbDifPeso).Specific).Caption = _dif.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbDifPorc).Specific).Caption = _porc.GetStringFromDouble(2);

                if (_dif >= 0.4)
                {
                    oForm.Freeze(true);
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Image = Iconos.Error;
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.Visible = true;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Item.Enabled = true;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Checked = true;
                    oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Click();
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Item.Enabled = false;
                    oForm.Freeze(false);
                    oForm.Update();
                    return _dif;
                }
                else
                {
                    oForm.Freeze(true);
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Image = Iconos.Success;
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.Visible = true;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Checked = false;
                    oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Click();
                    oForm.Freeze(false);
                    oForm.Update();
                    return _dif;
                }
            }
            catch (Exception e)
            {
                sbo_application.MessageBox(e.Message);
                return null;
            }

            //try
            //{
            //    var oForm = sbo_application.Forms.ActiveForm;
            //    string DocKey = ((DBDataSource)oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera)).GetValue("DocEntry", 0);

            //    if (string.IsNullOrEmpty(sessionId))
            //        sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

            //    string response = CommonFunctions.GET(ServiceLayer.Recepcion, DocKey, null, sessionId, out _);
            //    Recepcion recepcion = response.DeserializeJsonObject<Recepcion>();

            //    double _sumaLotes = recepcion.DFO_TRUCK2Collection.Sum(item => item.U_PesoLote);
            //    double _tara = ((double)recepcion.U_KilosIngreso) - ((double)recepcion.U_KilosSalida);
            //    double _dif = _tara - _sumaLotes;
            //    double _porc = Math.Round((Math.Abs(_dif) * 100) / _tara, 2);

            //    ((StaticText)oForm.Items.Item(pluginForm.LbPesoEntrada).Specific).Caption = ((double)recepcion.U_KilosIngreso).GetStringFromDouble(2);
            //    ((StaticText)oForm.Items.Item(pluginForm.LbPesoSalida).Specific).Caption = ((double)recepcion.U_KilosSalida).GetStringFromDouble(2);
            //    //Kilos envases
            //    ((StaticText)oForm.Items.Item(pluginForm.LbTara).Specific).Caption = _tara.GetStringFromDouble(2);
            //    ((StaticText)oForm.Items.Item(pluginForm.LbPesoLotes).Specific).Caption = _sumaLotes.GetStringFromDouble(2);
            //    ((StaticText)oForm.Items.Item(pluginForm.LbDifPeso).Specific).Caption = _dif.GetStringFromDouble(2);
            //    ((StaticText)oForm.Items.Item(pluginForm.LbDifPorc).Specific).Caption = _porc.GetStringFromDouble(2);

            //    if (_dif >= 0.4)
            //    {
            //        oForm.Freeze(true);
            //        ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Image = "SB_ERROR";
            //        ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.Visible = true;
            //        ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Item.Enabled = true;
            //        ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Checked = true;
            //        oForm.Mode = BoFormMode.fm_UPDATE_MODE;
            //        ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Click();
            //        ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Item.Enabled = false;
            //        oForm.Freeze(false);
            //        oForm.Update();
            //    }
            //    else
            //    {
            //        oForm.Freeze(true);
            //        ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Image = "ARCHIVE_SUCCESS_ICON";
            //        ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.Visible = true;
            //        ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Checked = false;
            //        oForm.Mode = BoFormMode.fm_UPDATE_MODE;
            //        ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Click();
            //        oForm.Freeze(false);
            //        oForm.Update();
            //    }
            //}
            //catch (Exception e)
            //{
            //    sbo_application.MessageBox(e.Message);
            //}
        }

        private static void ButtonAddEnv(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    try
                    {
                        //var oMatrix = oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific as Matrix;
                        //int _row = ((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).GetNextSelectedRow(0, BoOrderType.ot_RowOrder);
                        //string _Item = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value;
                        //string _Cant = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value;
                        //string _Prop = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Value;

                        var oFormEnvase = SAPFunctions.LoadFormEnvase(ref sbo_application) as Form;

                        ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtFatherUID).Specific).Value = formUID;
                        //((SAPbouiCOM.EditText)oFormLote.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = _Item;
                        //((SAPbouiCOM.EditText)oFormLote.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = _Cant;
                        //((SAPbouiCOM.EditText)oFormLote.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Value = _Prop;
                    }
                    catch (Exception e) { sbo_application.MessageBox(e.Message); }
                    //oForm.Freeze(true);
                }
            }
        }

        private static void ButtonConfirmEnv(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oFormEnvase = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    Form oForm = null;

                    try
                    {
                        //string _FatherForm = ((SAPbouiCOM.EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtFatherUID).Specific).Value.Trim();
                        //oForm = sbo_application.Forms.Item(_FatherForm);
                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;

                        //SAPbouiCOM.Matrix matrixEnv = (SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific;
                        //SAPbouiCOM.DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbEnvase);

                        //if (matrixEnv.RowCount >= 2)
                        //{
                        //    matrixEnv.AddRow(1, matrixEnv.RowCount);
                        //}

                        //int _row = matrixEnv.RowCount;

                        //if (_row == 1)
                        //{
                        //    if (((SAPbouiCOM.EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value.Trim().Length > 0)
                        //    {
                        //        matrixEnv.AddRow(1, matrixEnv.RowCount);
                        //        _row = matrixEnv.RowCount;
                        //    }
                        //}

                        string _FatherForm = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtFatherUID).Specific).Value.Trim();
                        oForm = sbo_application.Forms.Item(_FatherForm);
                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;

                        Matrix matrixEnv = (Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific;
                        DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes);

                        if (matrixEnv.RowCount >= 2)
                        {
                            matrixEnv.AddRow(1, matrixEnv.RowCount);
                        }

                        int _row = matrixEnv.RowCount;

                        if (_row == 1)
                        {
                            if (((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value.Trim().Length > 0)
                            {
                                matrixEnv.AddRow(1, matrixEnv.RowCount);
                                _row = matrixEnv.RowCount;
                            }
                        }

                        string _uid = Guid.NewGuid().ToString();

                        string _Envase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value.Trim();
                        string _CantEnvases = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value.Trim();
                        string _Tipo = ((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Selected.Value;

                        if (_row == 0)
                        {
                            matrixEnv.AddRow(1, matrixEnv.RowCount);
                            _row = matrixEnv.RowCount;
                        }

                        ((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_LineId.Uid).Cells.Item(_row).Specific).Value = _row.ToString();
                        ((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value = _Envase;
                        ((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value = _CantEnvases;
                        ((ComboBox)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Select(_Tipo, BoSearchKey.psk_ByValue);

                        //((SAPbouiCOM.Folder)oForm.Items.Item(pluginForm.FdLote).Specific).Item.Enabled = true;

                        int respuesta = sbo_application.MessageBox("¿Desea añadir más Envases a la misma guía?", 1, "Si", "No");
                        if (respuesta == 1)
                        {
                            ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = string.Empty;
                            ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = string.Empty;
                            //((SAPbouiCOM.ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Select(2, BoSearchKey.psk_ByValue);
                        }
                        else
                        {
                            oFormEnvase.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        sbo_application.MessageBox(e.Message);
                    }
                    finally
                    {
                        oForm.Freeze(false);
                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    }
                }
            }
        }

        private static void TxtTipoEnvase(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oFormEnvase = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
                //Validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        try
                        {
                            oFormEnvase.DataSources.UserDataSources.Item(CommonForms.FormEnvase.TxtTipoEnvase.UDS).Value = oDT.GetValue("ItemCode", 0).ToString();
                        }
                        catch { }
                    }
                }
            }
        }

        private static void CloseFormEnvase(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_FORM_CLOSE)
                {
                    Form oFormEnvase = sbo_application.Forms.Item(formUID);
                    var _FatherForm = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtFatherUID).Specific).Value.Trim();
                    Form oForm = sbo_application.Forms.Item(_FatherForm);
                    oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    oForm.Freeze(false);
                }
            }
        }

        private static void CreateSAPDoc(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var oForm = sbo_application.Forms.Item(formUID);
                    var DocKey = oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0);
                    var response = CommonFunctions.GET(ServiceLayer.Recepcion, DocKey, null, sessionId, out _);
                    var Recepcion = response.DeserializeJsonObject<Recepcion>();

                    //Validación de Balance de masa
                    //double? Balance = RefreshBalance(sbo_application, sessionId);
                    //if (Balance == null)
                    //{
                    //    throw new Exception("Error al calcular balance de masa, contacte al administrador");
                    //}
                    if (((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Value == "F")
                    {
                        int seleccion = sbo_application.MessageBox("Esta acción es irreversible \r Asegurece de que los lotes, pesos y calidad están todos bien ingresados antes de continuar \r ¿Desea continuar?", 1, "Si", "Cancelar");
                        if (seleccion == 2)
                            return;
                        //Validación de Balance de masa
                        //if (Math.Abs((double)Balance) > 0.049)
                        //{
                        //    seleccion = sbo_application.MessageBox("El balance de masa difiere del 0,4%, ¿Desea recibir esta fruta de todas maneras?", 1, "Si", "Cancelar");
                        //    if (seleccion == 2)
                        //        return;
                        //}

                        try
                        {
                            foreach (var guia in Recepcion.DFO_TRUCK1Collection.GroupBy(i => i.U_FolioGuia))
                            {
                                string activity = Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key && i.U_LineStatus == 'O').First().U_ClgCode;

                                var resp = CommonFunctions.GET(ServiceLayer.Activities, activity, null, sessionId, out _).DeserializeJsonObject<Activities>();

                                if (resp.DocType == "22")
                                {
                                    string date = DateTime.Now.ToString("yyyyMMdd");
                                    string time = DateTime.Now.ToString("hh:mm");

                                    ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value = date;
                                    ((EditText)oForm.Items.Item(pluginForm.TxtSalida).Specific).Value = time;

                                    Recepcion.U_HoraSalida = time;
                                    response = CommonFunctions.PATCH(ServiceLayer.Recepcion, Recepcion, DocKey, sessionId, out System.Net.HttpStatusCode httpStatus1);
                                    //throw new Exception("Esta recepción solo se puede cerrar desde Calidad-Recepción ");
                                }
                                if ((resp.DocType == "112"))
                                {
                                    var respDraft = CommonFunctions.GET(ServiceLayer.Drafts, resp.DocEntry, null, sessionId, out _).DeserializeJsonObject<Drafts>();
                                    var count = 0;
                                    foreach (var item in respDraft.DocumentLines)
                                    {
                                        count += item.BatchNumbers.Count();
                                    }
                                    if (count > 0)
                                    {
                                        DraftsService_SaveDraftToDocument SaveDraft = new DraftsService_SaveDraftToDocument
                                        {
                                            Document = new IDocuments { DocEntry = respDraft.DocEntry }
                                        };

                                        var saveDrafts = CommonFunctions.POST(ServiceLayer.DraftsService_SaveDraftToDocument, SaveDraft, sessionId, out System.Net.HttpStatusCode httpStatus);
                                        if (httpStatus == System.Net.HttpStatusCode.NoContent)
                                        {
                                            sbo_application.MessageBox("Recepción ingresada con éxito");

                                            string date = DateTime.Now.ToString("yyyyMMdd");
                                            string time = DateTime.Now.ToString("hh:mm");

                                            ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value = date;
                                            ((EditText)oForm.Items.Item(pluginForm.TxtSalida).Specific).Value = time;

                                            Recepcion.U_HoraSalida = time;
                                            response = CommonFunctions.PATCH(ServiceLayer.Recepcion, Recepcion, DocKey, sessionId, out System.Net.HttpStatusCode httpStatus1);
                                            if (httpStatus1 == System.Net.HttpStatusCode.NoContent)
                                            {
                                                response = CommonFunctions.POST($"{ServiceLayer.Recepcion}({DocKey})/Close", null, sessionId, out _);
                                            }
                                        }
                                        else
                                        {
                                            sbo_application.MessageBox("error actualizando draft");
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("Esta recepción solo se puede cerrar desde Calidad-Recepción ");
                                    }
                                }
                            }
                        }
                        catch
                        {
                            throw;
                        }
                    }
                    else
                    {
                        int seleccion = sbo_application.MessageBox("Esta acción es irreversible \r ¿Desea continuar?", 1, "Si", "Cancelar");
                        if (seleccion == 2)
                            return;

                        string date = DateTime.Now.ToString("yyyyMMdd");
                        string time = DateTime.Now.ToString("hh:mm");

                        ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value = date;
                        ((EditText)oForm.Items.Item(pluginForm.TxtSalida).Specific).Value = time;

                        
                        Recepcion.U_HoraSalida = time;

                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, Recepcion, DocKey, sessionId, out System.Net.HttpStatusCode httpStatus);
                        if (httpStatus == System.Net.HttpStatusCode.NoContent)
                        {
                            response = CommonFunctions.POST($"{ServiceLayer.Recepcion}({DocKey})/Close", null, sessionId, out _);
                        }
                    }
                        

                    Recepcion = CommonFunctions.GET(ServiceLayer.Recepcion, DocKey, null, sessionId, out _).DeserializeJsonObject<Recepcion>();

                    sbo_application.Menus.Item(SAPMenu.Refresh).Activate();
                }
            }
        }
    }
}