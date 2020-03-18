using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Dynamic;
using System.Xml;

namespace pluginCalidadToma
{
    internal static class frm

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
                    string date = DateTime.Now.ToString("yyyyMMdd");
                    string time = DateTime.Now.ToString("hh:mm");

                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                    FormCreationPackage.UniqueID = pluginForm.FormType + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    ((OptionBtn)oForm.Items.Item(pluginForm.RdSemana.uuid).Specific).GroupWith(pluginForm.RdDia.uuid);
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdMes.uuid).Specific).GroupWith(pluginForm.RdDia.uuid);
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdOF.uuid).Specific).GroupWith(pluginForm.RdDia.uuid);
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdDespacho.uuid).Specific).GroupWith(pluginForm.RdDia.uuid);

                    ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLOf);
                    Conditions oCons = oCFL.GetConditions();
                    Condition oCon = oCon = oCons.Add();
                    oCon.Alias = "Status";
                    oCon.Operation = BoConditionOperation.co_EQUAL;
                    oCon.CondVal = "R";
                    oCFL.SetConditions(oCons);

                    //oCFL = null;
                    //oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLTr);
                    //oCons = oCFL.GetConditions();
                    //oCon = oCon = oCons.Add();
                    //oCon.Alias = "DocStatus";
                    //oCon.Operation = BoConditionOperation.co_EQUAL;
                    //oCon.CondVal = "O";
                    //oCFL.SetConditions(oCons);

                    oForm.Visible = true;
                }
                catch (Exception e)
                {
                    sbo_application.MessageBox(e.Message);
                }
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.FormTypeEx == pluginForm.FormType)
            {
                switch (oItemEvent.ItemUID)
                {
                    case pluginForm.RdDia.uuid:
                    case pluginForm.RdSemana.uuid:
                    case pluginForm.RdMes.uuid:
                    case pluginForm.RdOF.uuid:
                    case pluginForm.RdDespacho.uuid:
                        RadioButtons(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case pluginForm.BttRef.uuid:
                        BttRef(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case pluginForm.TxtOF.uuid:
                        TxtOF(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case pluginForm.TxtDespacho.uuid:
                        TxtDespacho(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case pluginForm.GrdCalidad.uuid:
                        GrdCalidad(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case pluginForm.BttGen.uuid:
                        BttGen(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;
                }
            }
        }

        internal static void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            switch (businessObjectInfo.EventType)
            {
                default:
                    break;
            }
        }

        internal static void RightClickEventHandler(ref ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
            if (!eventInfo.BeforeAction)
            {
                //functions.AddRightClickMenu(ref sbo_application, UserMenu.DeleteRow, "Borrar Fila", true, BoMenuType.mt_STRING, SAPMenu.RightClickMenu);
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
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;

                        case SAPMenu.Find:
                            try
                            {
                                oForm.Freeze(true);
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;

                        case UserMenu.DeleteRow:
                            try
                            {
                                oForm.Freeze(true);

                                Menus Menus = sbo_application.Menus;
                                if (Menus.Exists(UserMenu.DeleteRow))
                                    Menus.RemoveEx(UserMenu.DeleteRow);
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;
                    }
                }
            }
        }

        private static void RadioButtons(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    try
                    {
                        oForm.Freeze(true);
                        if (oItemEvent.ItemUID == pluginForm.RdDia.uuid)
                        {
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtSemana.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtMes.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtOF.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDespacho.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.CheckBox)oForm.Items.Item(pluginForm.ChkLote.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDia.uuid).Specific).Item.Enabled = true;
                        }

                        if (oItemEvent.ItemUID == pluginForm.RdSemana.uuid)
                        {
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtMes.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtOF.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDespacho.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.CheckBox)oForm.Items.Item(pluginForm.ChkLote.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDia.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtSemana.uuid).Specific).Item.Enabled = true;
                        }

                        if (oItemEvent.ItemUID == pluginForm.RdMes.uuid)
                        {
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDia.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtSemana.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtOF.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDespacho.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.CheckBox)oForm.Items.Item(pluginForm.ChkLote.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtMes.uuid).Specific).Item.Enabled = true;
                        }

                        if (oItemEvent.ItemUID == pluginForm.RdOF.uuid)
                        {
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDia.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtSemana.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtMes.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDespacho.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.CheckBox)oForm.Items.Item(pluginForm.ChkLote.uuid).Specific).Item.Enabled = true;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtOF.uuid).Specific).Item.Enabled = true;
                        }

                        if (oItemEvent.ItemUID == pluginForm.RdDespacho.uuid)
                        {
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDia.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtSemana.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtMes.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtOF.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.CheckBox)oForm.Items.Item(pluginForm.ChkLote.uuid).Specific).Item.Enabled = false;
                            //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtDespacho.uuid).Specific).Item.Enabled = true;
                        }
                    }
                    catch
                    {
                        //sbo_application.StatusBar.SetText(e.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                    }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void BttRef(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.RdDia.uds).ValueEx))
                        {
                            sbo_application.MessageBox("Debe seleccionar un concepto a que asociar la calidad");
                            bBubbleEvent = false;
                            return;
                        }
                        else
                        {
                            switch (int.Parse(oForm.DataSources.UserDataSources.Item(pluginForm.RdDia.uds).ValueEx))
                            {
                                case 1:
                                    if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.TxtDia.uds).ValueEx))
                                    {
                                        sbo_application.MessageBox("Debe especificar el día");
                                        bBubbleEvent = false;
                                        return;
                                    }
                                    break;

                                case 2:
                                    if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.TxtSemana.uds).ValueEx))
                                    {
                                        sbo_application.MessageBox("Debe especificar la semana");
                                        bBubbleEvent = false;
                                        return;
                                    }
                                    break;

                                case 3:
                                    if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.TxtMes.uds).ValueEx))
                                    {
                                        sbo_application.MessageBox("Debe especificar la semana");
                                        bBubbleEvent = false;
                                        return;
                                    }
                                    break;

                                case 4:
                                    if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.TxtOF.uds).ValueEx))
                                    {
                                        sbo_application.MessageBox("Debe especificar la orden de fabricacion");
                                        bBubbleEvent = false;
                                        return;
                                    }
                                    break;

                                case 5:
                                    if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.TxtOF.uds).ValueEx))
                                    {
                                        sbo_application.MessageBox("Debe especificar el despacho");
                                        bBubbleEvent = false;
                                        return;
                                    }
                                    break;
                            }
                        }
                    }
                    catch
                    {
                        bBubbleEvent = false;
                        throw;
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    var _ListCalidad = string.Empty;
                    try
                    {
                        oForm.Freeze(true);
                        switch (int.Parse(oForm.DataSources.UserDataSources.Item(pluginForm.RdDia.uds).ValueEx))
                        {
                            case 1:
                                _ListCalidad = CommonFunctions.GET(ServiceLayer.ListadoRegistrosCalidad, null, $"?$select=CodSAP,Fruta,PuntoControl,CodProceso,Proceso,Version,Descripcion&$filter=Activo eq 'Y'", sessionId, out _).json2xml(pluginForm.GrdCalidad.udt);
                                oForm.DataSources.DataTables.Item(pluginForm.GrdCalidad.udt).LoadFromXML(_ListCalidad);
                                break;

                            case 2:
                                _ListCalidad = CommonFunctions.GET(ServiceLayer.ListadoRegistrosCalidad, null, $"?$select=CodSAP,Fruta,PuntoControl,CodProceso,Proceso,Version,Descripcion&$filter=Activo eq 'Y'", sessionId, out _).json2xml(pluginForm.GrdCalidad.udt);
                                oForm.DataSources.DataTables.Item(pluginForm.GrdCalidad.udt).LoadFromXML(_ListCalidad);
                                break;

                            case 3:
                                _ListCalidad = CommonFunctions.GET(ServiceLayer.ListadoRegistrosCalidad, null, $"?$select=CodSAP,Fruta,PuntoControl,CodProceso,Proceso,Version,Descripcion&$filter=Activo eq 'Y'", sessionId, out _).json2xml(pluginForm.GrdCalidad.udt);
                                oForm.DataSources.DataTables.Item(pluginForm.GrdCalidad.udt).LoadFromXML(_ListCalidad);
                                break;

                            case 4:
                                var _DocKey = oForm.DataSources.UserDataSources.Item(pluginForm.TxtOF.uds).ValueEx;
                                var _Of = CommonFunctions.GET(ServiceLayer.ListadoOrdenesFabricacion, null, $"?$filter=DocEntry eq {_DocKey}", sessionId, out _).DeserializeJsonObject<ListadoOrdenesFabricacion>();
                                _ListCalidad = CommonFunctions.GET(ServiceLayer.ListadoRegistrosCalidad, null, $"?$select=CodSAP,Fruta,PuntoControl,CodProceso,Proceso,Version,Descripcion&$filter=Fruta eq '{_Of.U_FRU_Fruta}' and Activo eq 'Y'", sessionId, out _).json2xml(pluginForm.GrdCalidad.udt);
                                oForm.DataSources.DataTables.Item(pluginForm.GrdCalidad.udt).LoadFromXML(_ListCalidad);
                                break;

                            case 5:
                                throw new NotImplementedException();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        oForm.Freeze(false);
                    }
                }
            }
        }

        private static void TxtOF(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtOF.uds).ValueEx = oDT.GetValue("DocEntry", 0).ToString();
                    }
                }
            }
        }

        private static void TxtDespacho(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtDespacho.uds).ValueEx = oDT.GetValue("DocEntry", 0).ToString();
                    }
                }
            }
        }

        private static void GrdCalidad(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.Before_Action)
            {
            }

            if (!oItemEvent.Before_Action)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED && oItemEvent.ColUID == "RowsHeader" && oItemEvent.Row != -1)
                {
                    ((Button)oForm.Items.Item(pluginForm.BttGen.uuid).Specific).Item.Enabled = true;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdDia.uuid).Specific).Item.Enabled = false;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdSemana.uuid).Specific).Item.Enabled = false;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdMes.uuid).Specific).Item.Enabled = false;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdOF.uuid).Specific).Item.Enabled = false;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdDespacho.uuid).Specific).Item.Enabled = false;
                }
                else
                {
                    ((Button)oForm.Items.Item(pluginForm.BttGen.uuid).Specific).Item.Enabled = false;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdDia.uuid).Specific).Item.Enabled = true;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdSemana.uuid).Specific).Item.Enabled = true;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdMes.uuid).Specific).Item.Enabled = true;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdOF.uuid).Specific).Item.Enabled = true;
                    ((OptionBtn)oForm.Items.Item(pluginForm.RdDespacho.uuid).Specific).Item.Enabled = true;
                }
            }
        }

        private static void BttGen(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    var grid = oForm.Items.Item(pluginForm.GrdCalidad.uuid).Specific as Grid;
                    if (grid.Rows.SelectedRows.Count == 0)
                    {
                        sbo_application.MessageBox("Debe seleccionar una fila");
                        bBubbleEvent = false;
                        return;
                    }

                    var _code = SAPFunctions.GetFieldFromSelectedRow(grid, "CodSAP");
                    if (string.IsNullOrEmpty(_code))
                    {
                        sbo_application.MessageBox("El codigo del registro de calidad es invalido");
                        bBubbleEvent = false;
                        return;
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    string _dato = "";
                    switch (int.Parse(oForm.DataSources.UserDataSources.Item(pluginForm.RdDia.uds).ValueEx))
                    {
                        case 1:
                            _dato = oForm.DataSources.UserDataSources.Item(pluginForm.TxtDia.uds).ValueEx;
                            break;

                        case 2:
                            _dato = oForm.DataSources.UserDataSources.Item(pluginForm.TxtSemana.uds).ValueEx;
                            break;

                        case 3:
                            _dato = oForm.DataSources.UserDataSources.Item(pluginForm.TxtMes.uds).ValueEx;
                            break;

                        case 4:
                            _dato = oForm.DataSources.UserDataSources.Item(pluginForm.TxtOF.uds).ValueEx;
                            break;

                        case 5:
                            _dato = oForm.DataSources.UserDataSources.Item(pluginForm.TxtDespacho.uds).ValueEx;
                            break;
                    }

                    dynamic Cabecera = new ExpandoObject();

                    Cabecera.Tipo = oForm.DataSources.UserDataSources.Item(pluginForm.RdDia.uds).ValueEx;
                    Cabecera.Valor = _dato;
                    Cabecera.Lote = oForm.DataSources.UserDataSources.Item(pluginForm.ChkLote.uds).ValueEx;

                    var grid = oForm.Items.Item(pluginForm.GrdCalidad.uuid).Specific as Grid;
                    var _code = SAPFunctions.GetFieldFromSelectedRow(grid, "CodSAP");
                    SAPFunctions.LoadFormCalidad(ref sbo_application, _code, sessionId, Cabecera);
                }
            }
        }
    }
}