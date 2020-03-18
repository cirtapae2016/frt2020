using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace pluginCalidadRecepcion
{
    internal static class frmCalidadRecepcion

    {
        private static Recepcion recepcion;
        private static string response;
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;
            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            if (oMenuEvent.BeforeAction)
            {
                if (string.IsNullOrEmpty(sessionId))
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                FormCreationPackage = (FormCreationParams)sbo_application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);
                try
                {
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
                        oForm.EnableMenu("1293", false);
                        oForm.EnableMenu("1284", false);
                        oForm.EnableMenu("1286", false);

                        oForm.DataBrowser.BrowseBy = "10";
                        ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Value = date;
                        ((EditText)oForm.Items.Item(pluginForm.TxtLlegada).Specific).Value = time;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).AutoResizeColumns();

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
                        oCon.Relationship = BoConditionRelationship.cr_AND;

                        oCon = oCons.Add();
                        oCon.Alias = "CntctType";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "1";
                        oCFL.SetConditions(oCons);

                        oCFL = null;

                        oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLEnvases);
                        oCons = oCFL.GetConditions();

                        oCon = oCons.Add();
                        oCon.Alias = "U_Subfamilia";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "BINS";

                        oCFL.SetConditions(oCons);

                        oCFL = null;

                        oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLProductor);
                        oCons = oCFL.GetConditions();

                        oCon = oCons.Add();
                        oCon.Alias = "GroupCode";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "103";

                        oCFL.SetConditions(oCons);

                        oForm.Mode = BoFormMode.fm_FIND_MODE;
                        ((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Select("F", BoSearchKey.psk_ByValue);
                        ((ComboBox)oForm.Items.Item(pluginForm.CmbDocStatus).Specific).Item.Enabled = true;
                        ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Item.Enabled = true;
                        ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Item.Enabled = true;

                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_TipoEnv.Uid).Visible = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_CantEnv.Uid).Visible = false;

                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_TipoEnv.Uid).Visible = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_CantEnv.Uid).Visible = false;
                    }
                    catch
                    {
                        throw;
                    }
                    finally { oForm.Freeze(false); oForm.Visible = true; }
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

            switch (oItemEvent.FormTypeEx)
            {
                case pluginForm.FormType:
                    switch (oItemEvent.ItemUID)
                    {
                        case pluginForm.ButtonAddGuia:
                            ButtonAddGuia(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddLote:
                            ButtonAddLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.MatrixGuia.Uid:
                            MatrixGuia(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.CmbTipoRecepcion:
                            CmbTipoRecepcion(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddQlty:
                            ButtonAddQlty(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonRefresh:
                            ButtonRefresh(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.MatrixLote.Uid:
                            MatrixLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.FdBalance:
                            FdBalance(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonFinish:
                            CreateSAPDoc(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddEnv:
                            ButtonAddEnv(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddEnvEnt:
                            ButtonAddEnvase("Entrada", formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonAddEnvSal:
                            ButtonAddEnvase("Salida", formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonPreviewTarja:
                            ButtonPreviewTarja(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonEliminaTarja:
                            ButtonEliminaTarja(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case pluginForm.ButtonOK:
                            var oForm = sbo_application.Forms.Item(formUID);
                            if (!oItemEvent.BeforeAction && oItemEvent.EventType == BoEventTypes.et_CLICK)
                            {
                                //string delR3 = $"delete from \"@DFO_RQLTY3\" where \"DocEntry\"=10";
                                //string delR4 = $"delete from \"@DFO_RQLTY4\" where \"DocEntry\"=10";

                                //var rs = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                                //rs.DoQuery(delR3);
                                //rs.DoQuery(delR4);

                                //CommonFunctions.ActualizarTotalesPorLote(10, sessionId);
                            }
                            break;
                    }
                    break;

                case CommonForms.FormLoteTemp.FormType:
                    switch (oItemEvent.ItemUID)
                    {
                        case CommonForms.FormLoteTemp.ButtonOK:
                            ButtonConfirmLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case CommonForms.FormLoteTemp.TxtTipoEnvase.Uid:
                            TxtTipoEnvase(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case CommonForms.FormLoteTemp.TxtProductor.Uid:
                            TxtProductor(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case "":
                            CloseFormRegLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;
                    }
                    break;

                case CommonForms.FormEnvLote.FormType:
                    switch (oItemEvent.ItemUID)
                    {
                        case CommonForms.FormEnvLote.ButtonOK:
                            ButtonConfirmEnv(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case CommonForms.FormEnvLote.TxtTipoEnvase.Uid:
                            TxtTipoEnvLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case "":
                            CloseFormEnvase(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;
                    }
                    break;

                case CommonForms.FormEnvase.FormType:
                    switch (oItemEvent.ItemUID)
                    {
                        case CommonForms.FormEnvase.ButtonOK:
                            ButtonConfirmEnvCam(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case CommonForms.FormEnvase.TxtTipoEnvase.Uid:
                            TxtTipoEnvaseCam(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;

                        case "":
                            CloseFormEnvaseCam(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            break;
                    }
                    break;
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
                    FormDataUpdate(ref businessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }
        //internal static void RightClickEventHandler(ref ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        //{
        //    bBubbleEvent = true;
        //    if (!eventInfo.BeforeAction)
        //    {
        //        SAPFunctions.AddRightClickMenu(ref sbo_application, UserMenu.DeleteRow, "Borrar Fila", true, BoMenuType.mt_STRING, SAPMenu.RightClickMenu);
        //    }
        //}

        internal static void MenuEventHandler(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oMenuEvent.BeforeAction)
            {
                //validaciones
            }

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

                                ((EditText)oForm.Items.Item(pluginForm.TxtFecha).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Item.Enabled = false;
                                ((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Item.Enabled = false;
                                ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Enabled = false;
                                ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Visible = false;
                                ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = true;
                            }
                            catch { throw; }
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
                                ((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Select("F", BoSearchKey.psk_ByValue);
                                ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Enabled = true;
                                ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Visible = false;
                                ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = true;
                            }
                            catch { throw; }
                            finally { oForm.Freeze(false); }
                            break;

                        //case UserMenu.DeleteRow:
                        //    try
                        //    {
                        //        oForm.Freeze(true);
                        //        var oMatrix = oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific as Matrix;
                        //        var oCell = oMatrix.GetCellFocus();

                        //        int _row = ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).GetNextSelectedRow() - 1;
                        //        var Lote = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes).GetValue("U_Lote", _row);

                        //        oMatrix.SelectRow(oCell.rowIndex, true, false);
                        //        oMatrix.SetCellFocus(1, oCell.ColumnIndex);
                        //        oMatrix.DeleteRow(oCell.rowIndex);


                        //        var oMatrix2 = oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific as Matrix;
                        //        var dbDataEnvLote = oForm.DataSources.DBDataSources.Item(pluginForm.dbEnvLote);
                        //        oMatrix2.FlushToDataSource();
                        //        for (var i = 0; i <= dbDataEnvLote.Size - 1; i++)
                        //        {
                        //            if (dbDataEnvLote.GetValue(pluginForm.MatrixEnvase.Colums.Col_Lote.dbField, i) == Lote)
                        //            {
                        //                oMatrix2.SelectRow(i, true, false);
                        //                oMatrix2.DeleteRow(i);
                        //            }
                        //        }



                        //        oForm.Mode = BoFormMode.fm_UPDATE_MODE;

                        //        Menus Menus = sbo_application.Menus;
                        //        if (Menus.Exists(UserMenu.DeleteRow))
                        //            Menus.RemoveEx(UserMenu.DeleteRow);
                        //    }
                        //    catch (Exception e) { sbo_application.MessageBox(e.Message); }
                        //    finally { oForm.Freeze(false); }
                        //    break;
                    }
                }
            }
        }

        private static void ButtonPreviewTarja(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    int _row = ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).GetNextSelectedRow() - 1;
                    var Lote = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes).GetValue("U_Lote", _row);
                    var LoteObj = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Lote}'", sessionId, out _).DeserializeJsonObject<BatchNumberDetails>();
                    if (LoteObj != null)
                    {
                        SAPFunctions.PrintLayout("BTN10003", (int)LoteObj.DocEntry, sbo_company);
                    }
                    else
                    {
                        sbo_application.MessageBox("El lote no se encuentra en existencia");
                    }
                }
            }
        }

        private static void ButtonEliminaTarja(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        int _row = ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).GetNextSelectedRow() - 1;

                        var Lote = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes).GetValue("U_Lote", _row);
                        if (Lote != null)
                        {
                            var Recepcion = CommonFunctions.GET(ServiceLayer.Recepcion, oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0), null, sessionId, out _).DeserializeJsonObject<Recepcion>();

                            var count = Recepcion.DFO_TRUCK2Collection.Count(i => i.U_Lote == Lote);
                            ///var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                            if (count == 1)
                            {
                                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                string sSql = "Select \"U_BatchNum\" from \"@DFO_RQLTY3\" where (\"U_BatchNum\")   = '" + Lote + "' ";
                                oRS.DoQuery(sSql);
                                if (oRS.RecordCount == 0)
                                {
                                    int respuesta = sbo_application.MessageBox("¿Desea confirmar La eliminación del registro '" + Lote + "' ?", 1, "Si", "No");
                                    if (respuesta == 1)
                                    {
                                        int respuesta1 = sbo_application.MessageBox("Esta acción es irreversible, esta seguro? ", 1, "Si", "No");
                                        if (respuesta1 == 1)
                                        {

                                            SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                            sSql = "Delete from \"@DFO_TRUCK5\" where (\"U_Lote\")   = '" + Lote + "' ";
                                            oRS1.DoQuery(sSql);

                                            sSql = "Delete from \"@DFO_TRUCK3\" where (\"U_Lote\")   = '" + Lote + "' ";
                                            oRS1.DoQuery(sSql);

                                            sSql = "Delete from \"@DFO_TRUCK2\" where (\"U_Lote\")   = '" + Lote + "' ";
                                            oRS1.DoQuery(sSql);

                                            sbo_application.Menus.Item(SAPMenu.Refresh).Activate();
                                        }
                                    }
                                }
                                else
                                {
                                    throw new Exception("El registro ya tiene calidad asociada, no puede ser eliminado");
                                }
                            }
                            else
                            {
                                throw new Exception("El registro no existe en la recepción");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un registro");
                        }
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }
            }
        }

        private static void FormDataUpdate(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);

            if (businessObjectInfo.BeforeAction)
            {
                //Validaciones
            }

            if (!businessObjectInfo.BeforeAction)
            {
                if (businessObjectInfo.ActionSuccess)
                {
                    //var matrix = oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific as SAPbouiCOM.Matrix;
                    //var dbDataLote = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes);
                    //var dbDataGuia = oForm.DataSources.DBDataSources.Item(pluginForm.dbGuias);
                    //var ocKey = dbDataGuia.GetValue(pluginForm.MatrixGuia.Colums.Col_Oc.dbField, 0);

                    //try
                    //{
                    //    matrix.FlushToDataSource();
                    //    for (var i = 0; i <= dbDataLote.Size - 1; i++)
                    //    {
                    //        DFO_OBTCH Lote = new DFO_OBTCH
                    //        {
                    //            Code = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_Code.dbField, i),
                    //            Name = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_Code.dbField, i),
                    //            U_LoteID = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_Lote.dbField, i),
                    //            U_PesoLote = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_Peso.dbField, i),
                    //            U_CodEnvase = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_TipoEnv.dbField, i),
                    //            U_Envases = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_CantEnv.dbField, i),
                    //            U_CardCode = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_Productor.dbField, i),
                    //            U_BaseEntry = dbDataLote.GetValue("DocEntry", i),
                    //            U_Muestra = dbDataLote.GetValue(pluginForm.MatrixLote.Colums.Col_Muestra.dbField, i)
                    //        };

                    //        if (string.IsNullOrEmpty(sessionId))
                    //            sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                    //    }
                    //}
                    //catch (Exception e)
                    //{
                    //    sbo_application.MessageBox(e.Message);
                    //}
                }
            }
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
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        try
                        {
                            int _row = ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).GetNextSelectedRow() - 1;
                            var _Lote = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes).GetValue("U_Lote", _row);
                            //var oMatrix = oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific as Matrix;
                            //int _row = ((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).GetNextSelectedRow(0, BoOrderType.ot_RowOrder);
                            //string _Item = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value;
                            //string _Cant = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value;
                            //string _Prop = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Value;

                            var oFormEnvLote = SAPFunctions.LoadFormEnvLote(_Lote, ref sbo_application) as Form;

                            ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtFatherUID).Specific).Value = formUID;
                        }
                        catch { throw; }
                        //oForm.Freeze(true);
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }
            }
        }

        private static void ButtonConfirmEnv(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oFormEnvLote = sbo_application.Forms.Item(formUID);
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


                        string _FatherForm = ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtFatherUID).Specific).Value.Trim();
                        oForm = sbo_application.Forms.Item(_FatherForm);
                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;

                        //ANTIGUO

                        //Matrix matrixEnv = (Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific;
                        //DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes);

                        //if (matrixEnv.RowCount >= 2)
                        //{
                        //    matrixEnv.AddRow(1, matrixEnv.RowCount);
                        //}

                        //int _row = matrixEnv.RowCount;

                        //if (_row == 1)
                        //{
                        //    if (((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value.Trim().Length > 0)
                        //    {
                        //        matrixEnv.AddRow(1, matrixEnv.RowCount);
                        //        _row = matrixEnv.RowCount;
                        //    }
                        //}

                        //ANTIGUO


                        string _uid = Guid.NewGuid().ToString();
                        string _Envase = ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtTipoEnvase.Uid).Specific).Value.Trim();
                        string _NomEnvase = ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtNomEnvase.Uid).Specific).Value.Trim();
                        string _Lote = ((StaticText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.StaticLote).Specific).Caption.Trim();
                        string _CantEnvases = ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtCantEnvase).Specific).Value.Trim();
                        string _Tipo = ((ComboBox)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtPropEnv).Specific).Selected.Value;

                        //NUEVO

                        string args = null;
                        string NroLlegada = ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Value;

                        if (NroLlegada.Length > 0)
                        {
                            args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                        }
                        //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                        var response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                        var recepcion = response.DeserializeJsonObject<Recepcion>();

                        int isnul = recepcion.DFO_TRUCK5Collection.Count(i => string.IsNullOrEmpty(i.U_Lote));
                        int coun = recepcion.DFO_TRUCK5Collection.Count;
                        if ((coun == 1) && (isnul == 1))
                        {
                            recepcion.DFO_TRUCK5Collection[0].U_Lote = _Lote;
                            recepcion.DFO_TRUCK5Collection[0].U_CodEnvase = _Envase;
                            recepcion.DFO_TRUCK5Collection[0].U_NomEnvase = _NomEnvase;
                            recepcion.DFO_TRUCK5Collection[0].U_Envases = int.Parse(_CantEnvases);
                            recepcion.DFO_TRUCK5Collection[0].U_PropEnv = _Tipo;
                        }
                        else
                        {
                            Recepcion_EnvLote EnvLotes;
                            EnvLotes = new Recepcion_EnvLote
                            {
                                U_Lote = _Lote,
                                U_CodEnvase = _Envase,
                                U_NomEnvase = _NomEnvase,
                                U_Envases = int.Parse(_CantEnvases),
                                U_PropEnv = _Tipo
                            };
                            recepcion.DFO_TRUCK5Collection.Add(EnvLotes);
                        }


                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out System.Net.HttpStatusCode httpStatus);
                        if (httpStatus == System.Net.HttpStatusCode.NoContent)
                        {
                            int respuesta = sbo_application.MessageBox("¿Desea añadir más Envases al lote '" + _Lote + "' ? ", 1, "Si", "No");
                            if (respuesta == 1)
                            {
                                ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtTipoEnvase.Uid).Specific).Value = string.Empty;
                                ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtNomEnvase.Uid).Specific).Value = string.Empty;
                                ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtCantEnvase).Specific).Value = string.Empty;
                                //((SAPbouiCOM.ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Select(2, BoSearchKey.psk_ByValue);
                            }
                            else
                            {
                                oFormEnvLote.Close();
                                sbo_application.Menus.Item(SAPMenu.Refresh).Activate();

                            }
                        }
                        else
                        {
                            throw new Exception("Error al ingresar lote, intente de nuevo");
                        }
                        //FINNUEVO

                        //ANTIGUO
                        //if (_row == 0)
                        //{
                        //    matrixEnv.AddRow(1, matrixEnv.RowCount);
                        //    _row = matrixEnv.RowCount;
                        //}

                        //((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_LineId.Uid).Cells.Item(_row).Specific).Value = _row.ToString();
                        //((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Lote.Uid).Cells.Item(_row).Specific).Value = _Lote;
                        //((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value = _Envase;
                        //((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_NomEnvase.Uid).Cells.Item(_row).Specific).Value = _NomEnvase;
                        //((EditText)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value = _CantEnvases;
                        //((ComboBox)matrixEnv.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Select(_Tipo, BoSearchKey.psk_ByValue);
                        //ANTIGUO
                        

                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        oForm.Freeze(false);

                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    }
                }
            }
        }

        private static void TxtTipoEnvLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oFormEnvLote = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
                //Validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        try
                        {
                            oFormEnvLote.DataSources.UserDataSources.Item(CommonForms.FormEnvLote.TxtTipoEnvase.UDS).Value = oDT.GetValue("ItemCode", 0).ToString();
                            oFormEnvLote.DataSources.UserDataSources.Item(CommonForms.FormEnvLote.TxtNomEnvase.UDS).Value = oDT.GetValue("ItemName", 0).ToString();
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
                    Form oFormEnvLote = sbo_application.Forms.Item(formUID);
                    var _FatherForm = ((EditText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.TxtFatherUID).Specific).Value.Trim();
                    Form oForm = sbo_application.Forms.Item(_FatherForm);

                    //oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    oForm.Freeze(false);
                }
            }
        }

        private static void ButtonAddEnvase(string tipo, string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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

                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        try
                        {
                            //var oMatrix = oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific as Matrix;
                            //int _row = ((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).GetNextSelectedRow(0, BoOrderType.ot_RowOrder);
                            //string _Item = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value;
                            //string _Cant = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value;
                            //string _Prop = ((SAPbouiCOM.EditText)((SAPbouiCOM.Matrix)oForm.Items.Item(pluginForm.MatrixEnvase.Uid).Specific).Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Value;

                            var oFormEnvase = SAPFunctions.LoadFormEnvase(ref sbo_application) as Form;

                            ((StaticText)oFormEnvase.Items.Item(CommonForms.FormEnvase.StaticTipoReg).Specific).Caption = tipo;
                            ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtFatherUID).Specific).Value = formUID;
                            //((SAPbouiCOM.EditText)oFormLote.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = _Item;
                            //((SAPbouiCOM.EditText)oFormLote.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = _Cant;
                            //((SAPbouiCOM.EditText)oFormLote.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Value = _Prop;
                        }
                        catch { throw; }
                        //oForm.Freeze(true);
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }
            }
        }

        private static void ButtonConfirmEnvCam(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    string tipo = ((StaticText)oFormEnvase.Items.Item(CommonForms.FormEnvase.StaticTipoReg).Specific).Caption;
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

                        if (tipo == "Entrada")
                        {
                            Matrix matrixEnvEnt = (Matrix)oForm.Items.Item(pluginForm.MatrixEnvaseEnt.Uid).Specific;
                            //DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbEnvEnt);

                            //NUEVO
                            string _uid = Guid.NewGuid().ToString();

                            string _Envase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value.Trim();
                            string _NomEnvase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtNomEnvase.Uid).Specific).Value.Trim();
                            string _CantEnvases = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value.Trim();
                            string _Tipo = ((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Selected.Value;


                            string args = null;
                            string NroLlegada = ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Value;

                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                            var response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            var recepcion = response.DeserializeJsonObject<Recepcion>();


                            int isnul = recepcion.DFO_TRUCK4Collection.Count(i => string.IsNullOrEmpty(i.U_CodEnvase));
                            int coun = recepcion.DFO_TRUCK4Collection.Count;
                            if ((coun == 1) && (isnul == 1))
                            {
                                recepcion.DFO_TRUCK4Collection[0].U_CodEnvase = _Envase;
                                recepcion.DFO_TRUCK4Collection[0].U_NomEnvase = _NomEnvase;
                                recepcion.DFO_TRUCK4Collection[0].U_Envases = int.Parse(_CantEnvases);
                                recepcion.DFO_TRUCK4Collection[0].U_PropEnv = _Tipo;
                            }
                            else
                            {
                                Recepcion_Envases EnvEntrada;
                                EnvEntrada = new Recepcion_Envases
                                {
                                    U_CodEnvase = _Envase,
                                    U_NomEnvase = _NomEnvase,
                                    U_Envases = int.Parse(_CantEnvases),
                                    U_PropEnv = _Tipo
                                };
                                recepcion.DFO_TRUCK4Collection.Add(EnvEntrada);
                            }
                            response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out System.Net.HttpStatusCode httpStatus);
                            if (httpStatus == System.Net.HttpStatusCode.NoContent)
                            {
                                int respuesta = sbo_application.MessageBox("¿Desea añadir más Envases a la recepción?", 1, "Si", "No");
                                if (respuesta == 1)
                                {
                                    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = string.Empty;
                                    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvLote.TxtNomEnvase.Uid).Specific).Value = string.Empty;
                                    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = string.Empty;
                                    //((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Select(2, BoSearchKey.psk_ByValue);
                                }
                                else
                                {
                                    oFormEnvase.Close();
                                    sbo_application.Menus.Item(SAPMenu.Refresh).Activate();
                                }
                            }
                            else
                            {
                                throw new Exception("Error al ingresar envase, intente de nuevo");
                            }
                            //FINNUEVO

                            //if (matrixEnvEnt.RowCount >= 2)
                            //{
                            //    matrixEnvEnt.AddRow(1, matrixEnvEnt.RowCount);
                            //}

                            //int _row = matrixEnvEnt.RowCount;

                            //if (_row == 1)
                            //{
                            //    if (((EditText)matrixEnvEnt.Columns.Item(pluginForm.MatrixEnvaseEnt.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value.Trim().Length > 0)
                            //    {
                            //        matrixEnvEnt.AddRow(1, matrixEnvEnt.RowCount);
                            //        _row = matrixEnvEnt.RowCount;
                            //    }
                            //}

                            //string _uid = Guid.NewGuid().ToString();

                            //string _Envase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value.Trim();
                            //string _NomEnvase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtNomEnvase.Uid).Specific).Value.Trim();
                            //string _CantEnvases = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value.Trim();
                            //string _Tipo = ((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Selected.Value;

                            //if (_row == 0)
                            //{
                            //    matrixEnvEnt.AddRow(1, matrixEnvEnt.RowCount);
                            //    _row = matrixEnvEnt.RowCount;
                            //}

                            //((EditText)matrixEnvEnt.Columns.Item(pluginForm.MatrixEnvaseEnt.Colums.Col_LineId.Uid).Cells.Item(_row).Specific).Value = _row.ToString();
                            //((EditText)matrixEnvEnt.Columns.Item(pluginForm.MatrixEnvaseEnt.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value = _Envase;
                            //((EditText)matrixEnvEnt.Columns.Item(pluginForm.MatrixEnvaseEnt.Colums.Col_NomEnvase.Uid).Cells.Item(_row).Specific).Value = _NomEnvase;
                            //((EditText)matrixEnvEnt.Columns.Item(pluginForm.MatrixEnvaseEnt.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value = _CantEnvases;
                            //((ComboBox)matrixEnvEnt.Columns.Item(pluginForm.MatrixEnvaseEnt.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Select(_Tipo, BoSearchKey.psk_ByValue);

                            //int respuesta = sbo_application.MessageBox("¿Desea añadir más Envases a la recepción?", 1, "Si", "No");
                            //if (respuesta == 1)
                            //{
                            //    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = string.Empty;
                            //    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvLote.TxtNomEnvase.Uid).Specific).Value = string.Empty;
                            //    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = string.Empty;
                            //    //((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Select(2, BoSearchKey.psk_ByValue);
                            //}
                            //else
                            //{
                            //    oFormEnvase.Close();
                            //}
                        }
                        else
                        {
                            Matrix matrixEnvSal = (Matrix)oForm.Items.Item(pluginForm.MatrixEnvaseSal.Uid).Specific;
                            DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbEnvSal);

                            //NUEVO
                            string _uid = Guid.NewGuid().ToString();

                            string _Envase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value.Trim();
                            string _NomEnvase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtNomEnvase.Uid).Specific).Value.Trim();
                            string _CantEnvases = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value.Trim();
                            string _Tipo = ((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Selected.Value;


                            string args = null;
                            string NroLlegada = ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Value;

                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                            var response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            var recepcion = response.DeserializeJsonObject<Recepcion>();

                            int isnul = recepcion.DFO_TRUCK6Collection.Count(i => string.IsNullOrEmpty(i.U_CodEnvase));
                            int coun = recepcion.DFO_TRUCK6Collection.Count;
                            if ((coun == 1) && (isnul == 1))
                            {
                                recepcion.DFO_TRUCK6Collection[0].U_CodEnvase = _Envase;
                                recepcion.DFO_TRUCK6Collection[0].U_NomEnvase = _NomEnvase;
                                recepcion.DFO_TRUCK6Collection[0].U_Envases = int.Parse(_CantEnvases);
                                recepcion.DFO_TRUCK6Collection[0].U_PropEnv = _Tipo;
                            }
                            else
                            {
                                Recepcion_Envases_Sal EnvSalida;
                                EnvSalida = new Recepcion_Envases_Sal
                                {
                                    U_CodEnvase = _Envase,
                                    U_NomEnvase = _NomEnvase,
                                    U_Envases = int.Parse(_CantEnvases),
                                    U_PropEnv = _Tipo
                                };
                                recepcion.DFO_TRUCK6Collection.Add(EnvSalida);
                            }
                            response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out System.Net.HttpStatusCode httpStatus);
                            if (httpStatus == System.Net.HttpStatusCode.NoContent)
                            {
                                int respuesta = sbo_application.MessageBox("¿Desea añadir más Envases a la recepción?", 1, "Si", "No");
                                if (respuesta == 1)
                                {
                                    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = string.Empty;
                                    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvLote.TxtNomEnvase.Uid).Specific).Value = string.Empty;
                                    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = string.Empty;
                                    //((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Select(2, BoSearchKey.psk_ByValue);
                                }
                                else
                                {
                                    oFormEnvase.Close();
                                    sbo_application.Menus.Item(SAPMenu.Refresh).Activate();
                                }
                            }
                            else
                            {
                                throw new Exception("Error al ingresar envase, intente de nuevo");
                            }
                            //FINNUEVO


                            //if (matrixEnvSal.RowCount >= 2)
                            //{
                            //    matrixEnvSal.AddRow(1, matrixEnvSal.RowCount);
                            //}

                            //int _row = matrixEnvSal.RowCount;

                            //if (_row == 1)
                            //{
                            //    if (((EditText)matrixEnvSal.Columns.Item(pluginForm.MatrixEnvaseSal.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value.Trim().Length > 0)
                            //    {
                            //        matrixEnvSal.AddRow(1, matrixEnvSal.RowCount);
                            //        _row = matrixEnvSal.RowCount;
                            //    }
                            //}

                            //string _uid = Guid.NewGuid().ToString();

                            //string _Envase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value.Trim();
                            //string _NomEnvase = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtNomEnvase.Uid).Specific).Value.Trim();
                            //string _CantEnvases = ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value.Trim();
                            //string _Tipo = ((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Selected.Value;

                            //if (_row == 0)
                            //{
                            //    matrixEnvSal.AddRow(1, matrixEnvSal.RowCount);
                            //    _row = matrixEnvSal.RowCount;
                            //}

                            //((EditText)matrixEnvSal.Columns.Item(pluginForm.MatrixEnvaseSal.Colums.Col_LineId.Uid).Cells.Item(_row).Specific).Value = _row.ToString();
                            //((EditText)matrixEnvSal.Columns.Item(pluginForm.MatrixEnvaseSal.Colums.Col_Envase.Uid).Cells.Item(_row).Specific).Value = _Envase;
                            //((EditText)matrixEnvSal.Columns.Item(pluginForm.MatrixEnvaseSal.Colums.Col_NomEnvase.Uid).Cells.Item(_row).Specific).Value = _NomEnvase;
                            //((EditText)matrixEnvSal.Columns.Item(pluginForm.MatrixEnvaseSal.Colums.Col_Cantidad.Uid).Cells.Item(_row).Specific).Value = _CantEnvases;
                            //((ComboBox)matrixEnvSal.Columns.Item(pluginForm.MatrixEnvaseSal.Colums.Col_Propiedad.Uid).Cells.Item(_row).Specific).Select(_Tipo, BoSearchKey.psk_ByValue);

                            //int respuesta = sbo_application.MessageBox("¿Desea añadir más Envases a la recepción?", 1, "Si", "No");
                            //if (respuesta == 1)
                            //{
                            //    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtTipoEnvase.Uid).Specific).Value = string.Empty;
                            //    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtNomEnvase.Uid).Specific).Value = string.Empty;
                            //    ((EditText)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtCantEnvase).Specific).Value = string.Empty;
                            //    //((ComboBox)oFormEnvase.Items.Item(CommonForms.FormEnvase.TxtPropEnv).Specific).Select(2, BoSearchKey.psk_ByValue);
                            //}
                            //else
                            //{
                            //    oFormEnvase.Close();
                            //}
                        }

                        
                        //((SAPbouiCOM.Folder)oForm.Items.Item(pluginForm.FdLote).Specific).Item.Enabled = true;
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        oForm.Freeze(false);
                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    }
                }
            }
        }

        private static void TxtTipoEnvaseCam(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                            oFormEnvase.DataSources.UserDataSources.Item(CommonForms.FormEnvase.TxtNomEnvase.UDS).Value = oDT.GetValue("ItemName", 0).ToString();
                        }
                        catch { }
                    }
                }
            }
        }

        private static void CloseFormEnvaseCam(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    //oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    oForm.Freeze(false);
                }
            }
        }

        private static void ButtonAddQlty(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                //Validaciones
            }

            if (!oItemEvent.BeforeAction)
            {

                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        int _row = ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).GetNextSelectedRow() - 1;
                        var Recepcion = CommonFunctions.GET(ServiceLayer.Recepcion, oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0), null, sessionId, out _).DeserializeJsonObject<Recepcion>();
                        var _guia = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes).GetValue("U_FolioGuia", _row);

                        dynamic Cabecera = new System.Dynamic.ExpandoObject();

                        Cabecera.Tipo = "OTRUCK";
                        Cabecera.Valor = oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("DocEntry", 0);
                        Cabecera.Lote = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes).GetValue("U_Lote", _row);

                        switch (Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == _guia).Select(i => i.U_Fruta).FirstOrDefault())
                        {
                            case "CIRUELA":
                                _ = SAPFunctions.LoadFormCalidad(ref sbo_application, "CIRUELA-20023-RG-5.5.1.2MP", sessionId, Cabecera) as Form;
                                break;

                            case "NUEZ":
                                _ = SAPFunctions.LoadFormCalidad(ref sbo_application, "NUEZ-20023-RG-5.5.1.2MPN", sessionId, Cabecera) as Form;
                                break;

                            case "PASA":
                                _ = SAPFunctions.LoadFormCalidad(ref sbo_application, "PASA-20023-RG-5.6.1.1MP", sessionId, Cabecera) as Form;
                                break;

                            case "UVA":
                                throw new Exception("No hay registro");

                            case "ALMENDRA":
                                throw new Exception("No hay registro");
                        }
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }

            }
        }

        private static void CmbTipoRecepcion(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_COMBO_SELECT)
                {
                    if (((ComboBox)oForm.Items.Item(pluginForm.CmbTipoRecepcion).Specific).Value == "F")
                    {
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Item.Enabled = true;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddGuia).Specific).Item.Enabled = true;
                        ((EditText)oForm.Items.Item(pluginForm.TxtTransportista).Specific).Item.Enabled = true;
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
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    if (oItemEvent.ColUID == pluginForm.MatrixGuia.Colums.Col_Planificacion.Uid)
                    {
                        if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                        {
                            var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                            if (oDT != null)
                            {
                                try
                                {
                                    ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(oItemEvent.ColUID).Cells.Item(oItemEvent.Row).Specific).Value = oDT.GetValue("ClgCode", 0).ToString();
                                }
                                catch { }
                            }
                        }
                        else
                        {
                            throw new Exception("La recepción ya fue cerrada");
                        }
                    }
                }

                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        if (oItemEvent.ColUID == "#" && oItemEvent.Row > 0)
                        {
                            string activity = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Planificacion.Uid).Cells.Item(oItemEvent.Row).Specific).Value;
                            var resp = CommonFunctions.GET(ServiceLayer.Activities, activity, null, sessionId, out _).DeserializeJsonObject<Activities>();

                            if (resp.DocType == "112")
                            {
                                var response = CommonFunctions.GET(ServiceLayer.Drafts, resp.DocEntry, null, sessionId, out _).DeserializeJsonObject<Drafts>();

                                var count = 0;
                                foreach (var item in response.DocumentLines)
                                {
                                    count += item.BatchNumbers.Count();
                                }
                                if (count > 0)
                                {
                                    ((Button)oForm.Items.Item(pluginForm.ButtonAddLote).Specific).Item.Enabled = false;
                                }
                                else
                                {
                                    ((Button)oForm.Items.Item(pluginForm.ButtonAddLote).Specific).Item.Enabled = true;
                                }
                            }
                            else
                            {
                                ((Button)oForm.Items.Item(pluginForm.ButtonAddLote).Specific).Item.Enabled = true;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }
            }
        }

        private static void MatrixLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_MATRIX_LINK_PRESSED)
                {
                    if (oItemEvent.ColUID == pluginForm.MatrixLote.Colums.Col_Lote.Uid)
                    {
                        var oMatrix = oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific as Matrix;
                        oForm.Freeze(true);

                        //oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Visible = true;
                        //oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Cells.Item(oItemEvent.Row).Click(SAPbouiCOM.BoCellClickType.ct_Linked, 0);
                        //oMatrix.Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Visible = false;

                        oForm.Freeze(false);
                        bBubbleEvent = false;
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK && oForm.Mode == BoFormMode.fm_OK_MODE)
                {
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        if (oItemEvent.ColUID == "#")
                        {

                            Matrix matrixLote = (Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific;
                            string Lote = ((EditText)matrixLote.Columns.Item(pluginForm.MatrixEnvase.Colums.Col_Lote.Uid).Cells.Item(oItemEvent.Row).Specific).Value;
                            if (!string.IsNullOrEmpty(Lote))
                            {
                                ((Button)oForm.Items.Item(pluginForm.ButtonAddQlty).Specific).Item.Enabled = true;
                                ((Button)oForm.Items.Item(pluginForm.ButtonAddEnv).Specific).Item.Enabled = true;
                                ((Button)oForm.Items.Item(pluginForm.ButtonEliminaTarja).Specific).Item.Enabled = true;

                                string NoCalidad = "";
                                ((StaticText)oForm.Items.Item(pluginForm.LbCalidad).Specific).Caption = NoCalidad;
                                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                string sSql = "Select \"DocEntry\" from \"@DFO_RQLTY3\" where (\"U_BatchNum\")   = '" + Lote + "' ";
                                oRS.DoQuery(sSql);
                                if (oRS.RecordCount > 0)
                                    ((StaticText)oForm.Items.Item(pluginForm.LbCalidad).Specific).Caption = oRS.Fields.Item("DocEntry").Value.ToString();
                            }
                            else
                            {
                                ((Button)oForm.Items.Item(pluginForm.ButtonAddQlty).Specific).Item.Enabled = false;
                                ((Button)oForm.Items.Item(pluginForm.ButtonAddEnv).Specific).Item.Enabled = false;
                                ((Button)oForm.Items.Item(pluginForm.ButtonEliminaTarja).Specific).Item.Enabled = false;
                                string NoCalidad = "";
                                ((StaticText)oForm.Items.Item(pluginForm.LbCalidad).Specific).Caption = NoCalidad;
                            }
                        }
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    if (oItemEvent.ColUID == pluginForm.MatrixLote.Colums.Col_Productor.Uid)
                    {
                        var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                        if (oDT != null)
                        {
                            try
                            {
                                DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes);
                                det.SetValue(pluginForm.MatrixLote.Colums.Col_Productor.dbField, oItemEvent.Row - 1, oDT.GetValue("CardCode", 0).ToString());
                            }
                            catch { }
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
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        try
                        {
                            Matrix oMatrix = (Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific;
                            oMatrix.AddRow();
                        }
                        finally { oForm.Freeze(false); }
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
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
                //validaciones
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

                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) == "C")
                    {
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Obs.Uid).Editable = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Castigo.Uid).Editable = false;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Aprob.Uid).Editable = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonFinish).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddEnv).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddEnvEnt).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddEnvSal).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddGuia).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddLote).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddQlty).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonEliminaTarja).Specific).Item.Enabled = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonFinish).Specific).Item.Enabled = false;
                    }
                    else
                    {
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Oc.Uid).Editable = true;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Castigo.Uid).Editable = true;
                        ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Aprob.Uid).Editable = true;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddEnv).Specific).Item.Enabled = true;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddEnvEnt).Specific).Item.Enabled = true;
                        ((Button)oForm.Items.Item(pluginForm.ButtonAddEnvSal).Specific).Item.Enabled = true;
                        ((Button)oForm.Items.Item(pluginForm.ButtonFinish).Specific).Item.Enabled = true;
                    }
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).AutoResizeColumns();
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific).Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Visible = false;
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixPesaje.Uid).Specific).AutoResizeColumns();
                    ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).AutoResizeColumns();
                }
                catch { throw; }
                finally { oForm.Freeze(false); }
            }
        }

        private static void ButtonRefresh(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                //validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    RefreshBalance(sbo_application, sessionId);
                }
            }
        }

        private static void FdBalance(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction && oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (oForm.Mode != BoFormMode.fm_OK_MODE)
                {
                    bBubbleEvent = false;
                }
            }

            if (!oItemEvent.BeforeAction && oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                //RefreshBalance(sbo_application);
            }
        }

        //Mover a su propio plugin
        private static void ButtonAddLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    if (oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Status", 0) != "C")
                    {
                        try
                        {
                            var oMatrix = oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific as Matrix;
                            int _row = ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).GetNextSelectedRow(0, BoOrderType.ot_RowOrder);
                            string _Guia = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Folio.Uid).Cells.Item(_row).Specific).Value;
                            string _Prod = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Productor.Uid).Cells.Item(_row).Specific).Value;
                            string _NomProd = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_CardName.Uid).Cells.Item(_row).Specific).Value;
                            string _Env = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_TipoEnv.Uid).Cells.Item(_row).Specific).Value;

                            var oFormLote = SAPFunctions.LoadFormLote(ref sbo_application) as Form;
                            oFormLote.Freeze(true);
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtFatherUID).Specific).Value = formUID;
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtLoteID).Specific).Value = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtFolioGuia).Specific).Value = _Guia;
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtProductor.Uid).Specific).Value = _Prod;
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtNomProd.Uid).Specific).Value = _NomProd;
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtTipoEnvase.Uid).Specific).Value = _Env;
                            ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtLoteCancha).Specific).Value = "0";
                            oFormLote.Freeze(false);
                        }
                        catch { throw; }
                    //oForm.Freeze(true);
                    }
                    else
                    {
                        throw new Exception("La recepción ya fue cerrada");
                    }
                }
            }
        }

        //Mover a su propio plugin
        private static void CloseFormRegLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    Form oFormLote = sbo_application.Forms.Item(formUID);
                    var _FatherForm = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtFatherUID).Specific).Value.Trim();
                    Form oForm = sbo_application.Forms.Item(_FatherForm);
                    oForm.Freeze(false);
                }
            }
        }

        //Mover a su propio plugin
        private static void ButtonConfirmLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oFormLote = sbo_application.Forms.Item(formUID);
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
                        string _FatherForm = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtFatherUID).Specific).Value.Trim();
                        oForm = sbo_application.Forms.Item(_FatherForm);

                        //oForm.Mode = BoFormMode.fm_UPDATE_MODE;

                        //ANTIGUO

                        int _rowguia = ((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).GetNextSelectedRow(0, BoOrderType.ot_RowOrder);
                        //Matrix matrixLote = (Matrix)oForm.Items.Item(pluginForm.MatrixLote.Uid).Specific;
                        //DBDataSource dBDataSource = oForm.DataSources.DBDataSources.Item(pluginForm.dbLotes);

                        //if (matrixLote.RowCount >= 2)
                        //{
                        //    matrixLote.AddRow(1, matrixLote.RowCount);
                        //}
                        //if (matrixLote.RowCount == 0)
                        //{
                        //    matrixLote.AddRow(1, 0);
                        //}

                        //int _row = matrixLote.RowCount;
                        //if (_row == 1)
                        //{
                        //    if (((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Cells.Item(_row).Specific).Value.Trim().Length > 0)
                        //    {
                        //        matrixLote.AddRow(1, matrixLote.RowCount);
                        //        _row = matrixLote.RowCount;
                        //    }
                        //}

                        //ANTIGUO


                        string _uid = Guid.NewGuid().ToString();
                        string _LoteId = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtLoteID).Specific).Value.Trim();
                        string _Guia = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtFolioGuia).Specific).Value.Trim();
                        string _Envases = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtCantEnvase).Specific).Value.Trim();
                        string _Tipo = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtTipoEnvase.Uid).Specific).Value.Trim();
                        string _Muestra = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtMuestra).Specific).Value.Trim();
                        string _Prod = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtProductor.Uid).Specific).Value.Trim();
                        string _NomProd = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtNomProd.Uid).Specific).Value.Trim();
                        string _LoteCancha = ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtLoteCancha).Specific).Value.Trim();
                        string _Variedad = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Variedad.Uid).Cells.Item(_rowguia).Specific).Value;
                        string _TipoF = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_Tipo.Uid).Cells.Item(_rowguia).Specific).Value;

                        string _BaseLine = ((EditText)((Matrix)oForm.Items.Item(pluginForm.MatrixGuia.Uid).Specific).Columns.Item(pluginForm.MatrixGuia.Colums.Col_BaseLine.Uid).Cells.Item(_rowguia).Specific).Value;
                        string _TipoSecado = ((ComboBox)oFormLote.Items.Item(CommonForms.FormLoteTemp.CmbTipoSecado).Specific).Selected.Description.Trim();


                        //NUEVO

                        string args = null;
                        string NroLlegada = ((EditText)oForm.Items.Item(pluginForm.TxtDocNum).Specific).Value;

                        if (NroLlegada.Length > 0)
                        {
                            args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                        }
                        //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                        var response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                        var recepcion = response.DeserializeJsonObject<Recepcion>();
                        int isnul = recepcion.DFO_TRUCK2Collection.Count(i => string.IsNullOrEmpty(i.U_Lote));
                        int coun = recepcion.DFO_TRUCK2Collection.Count;
                        if ((coun == 1) && (isnul == 1))
                        {
                            recepcion.DFO_TRUCK2Collection[0].U_Lote = _LoteId;
                            recepcion.DFO_TRUCK2Collection[0].U_FolioGuia = _Guia;
                            recepcion.DFO_TRUCK2Collection[0].U_CardCode = _Prod;
                            recepcion.DFO_TRUCK2Collection[0].U_CardName = _NomProd;
                            recepcion.DFO_TRUCK2Collection[0].U_Code = _uid;
                            recepcion.DFO_TRUCK2Collection[0].U_Muestra = _Muestra;
                            recepcion.DFO_TRUCK2Collection[0].U_PesoLote = 0;
                            recepcion.DFO_TRUCK2Collection[0].U_Tipo = _TipoF;
                            recepcion.DFO_TRUCK2Collection[0].U_TipoSecado = _TipoSecado;
                            recepcion.DFO_TRUCK2Collection[0].U_Variedad = _Variedad;
                            recepcion.DFO_TRUCK2Collection[0].U_Castigo = 0;
                            recepcion.DFO_TRUCK2Collection[0].U_LoteCancha = int.Parse(_LoteCancha);
                            recepcion.DFO_TRUCK2Collection[0].U_BaseLine = _BaseLine;
                        }
                        else
                        {
                            Recepcion_Lotes Lotes;
                            Lotes = new Recepcion_Lotes
                            {
                                U_Lote = _LoteId,
                                U_FolioGuia = _Guia,
                                U_CardCode = _Prod,
                                U_CardName = _NomProd,
                                U_Code = _uid,
                                U_Muestra = _Muestra,
                                U_PesoLote = 0,
                                U_Tipo = _TipoF,
                                U_TipoSecado = _TipoSecado,
                                U_Variedad = _Variedad,
                                U_Castigo = 0,
                                U_LoteCancha = int.Parse(_LoteCancha),
                                U_BaseLine = _BaseLine
                            };
                            recepcion.DFO_TRUCK2Collection.Add(Lotes);
                        }



                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out System.Net.HttpStatusCode httpStatus);
                        if (httpStatus == System.Net.HttpStatusCode.NoContent)
                        {
                            ((Folder)oForm.Items.Item(pluginForm.FdLote).Specific).Item.Enabled = true;

                            int respuesta = sbo_application.MessageBox("¿Desea añadir más lotes a la misma guía?", 1, "Si", "No");
                            if (respuesta == 1)
                            {
                                ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtLoteID).Specific).Value = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtCantEnvase).Specific).Value = string.Empty;
                                ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtMuestra).Specific).Value = string.Empty;
                                ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtLoteCancha).Specific).Value = "0";
                                ((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.TxtCantEnvase).Specific).Item.Click();
                                //((EditText)oFormLote.Items.Item(CommonForms.FormLoteTemp.CmbTipoSecado).Specific).Value = null;
                            }
                            else
                            {
                                oFormLote.Close();
                                sbo_application.Menus.Item(SAPMenu.Refresh).Activate();
                            }
                        }
                        else
                        {
                            throw new Exception("Error al ingresar lote, intente de nuevo");
                        }


                        //ANTIGUO

                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_LineId.Uid).Cells.Item(_row).Specific).Value = _row.ToString();
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Code.Uid).Cells.Item(_row).Specific).Value = _uid;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Lote.Uid).Cells.Item(_row).Specific).Value = _LoteId;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Folio.Uid).Cells.Item(_row).Specific).Value = _Guia;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Peso.Uid).Cells.Item(_row).Specific).Value = "0";
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_CantEnv.Uid).Cells.Item(_row).Specific).Value = _Envases;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_TipoEnv.Uid).Cells.Item(_row).Specific).Value = _Tipo;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Muestra.Uid).Cells.Item(_row).Specific).Value = _Muestra;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Productor.Uid).Cells.Item(_row).Specific).Value = _Prod;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_NomProd.Uid).Cells.Item(_row).Specific).Value = _NomProd;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Variedad.Uid).Cells.Item(_row).Specific).Value = _Variedad;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_LoteCancha.Uid).Cells.Item(_row).Specific).Value = _LoteCancha;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_Tipo.Uid).Cells.Item(_row).Specific).Value = _TipoF;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_BaseLine.Uid).Cells.Item(_row).Specific).Value = _BaseLine;
                        //((EditText)matrixLote.Columns.Item(pluginForm.MatrixLote.Colums.Col_TipoSecado.Uid).Cells.Item(_row).Specific).Value = _TipoSecado;

                        //ANTIGUO
                        

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

        //Mover a su propio plugin
        private static void TxtTipoEnvase(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oFormLote = sbo_application.Forms.Item(formUID);

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
                            oFormLote.DataSources.UserDataSources.Item(CommonForms.FormLoteTemp.TxtTipoEnvase.UDS).Value = oDT.GetValue("ItemCode", 0).ToString();
                        }
                        catch { }
                    }
                }
            }
        }

        private static void TxtProductor(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oFormLote = sbo_application.Forms.Item(formUID);

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
                            oFormLote.DataSources.UserDataSources.Item(CommonForms.FormLoteTemp.TxtProductor.UDS).Value = oDT.GetValue("CardCode", 0).ToString();
                            oFormLote.DataSources.UserDataSources.Item(CommonForms.FormLoteTemp.TxtNomProd.UDS).Value = oDT.GetValue("CardName", 0).ToString();
                        }
                        catch { }
                    }
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

                double _envEnt = 0;
                if (recepcion.DFO_TRUCK4Collection.Count(i => i.U_CodEnvase != null) > 0)
                {
                    foreach (var env in recepcion.DFO_TRUCK4Collection)
                    {
                        var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                        if (env.U_Envases < 0 || env.U_Envases == null)
                            throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                        if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                            throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                        _envEnt += ((double)env.U_Envases * (double)item.InventoryWeight);
                    }
                }

                double _envLote = 0;
                if (recepcion.DFO_TRUCK5Collection.Count(i => i.U_CodEnvase != null) > 0)
                {
                    foreach (var env in recepcion.DFO_TRUCK5Collection)
                    {
                        var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                        if (env.U_Envases < 0 || env.U_Envases == null)
                            throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                        if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                            throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                        _envLote += ((double)env.U_Envases * (double)item.InventoryWeight);
                    }
                }

                double _envSal = 0;
                if (recepcion.DFO_TRUCK6Collection.Count(i => i.U_CodEnvase != null) > 0)
                {
                    foreach (var env in recepcion.DFO_TRUCK6Collection)
                    {
                        var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                        if (env.U_Envases < 0 || env.U_Envases == null)
                            throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                        if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                            throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                        _envSal += ((double)env.U_Envases * (double)item.InventoryWeight);
                    }
                }

                double _sumaLotes = recepcion.DFO_TRUCK2Collection.Sum(item => item.U_PesoLote);
                //double _sumaLotes = recepcion.DFO_TRUCK2Collection.Where(i=>i.U_Castigo>0).Sum(i=>i.U_PesoEnvase.GetDoubleFromString(";"));
                double _tara = ((((double)recepcion.U_KilosIngreso + (double)recepcion.U_KilosIngAco) - _envEnt) - (((double)recepcion.U_KilosSalida + (double)recepcion.U_KilosSalAco) - _envSal));
                double _dif = _tara - _sumaLotes;
                double _porc = Math.Round((Math.Abs(_dif) * 100) / _tara, 2);

                ((StaticText)oForm.Items.Item(pluginForm.LbPesoEntrada).Specific).Caption = ((double)recepcion.U_KilosIngreso + (double)recepcion.U_KilosIngAco).GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbPesoSalida).Specific).Caption = ((double)recepcion.U_KilosSalida + (double)recepcion.U_KilosSalAco).GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbEnvEnt).Specific).Caption = _envEnt.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbEnvSal).Specific).Caption = _envSal.GetStringFromDouble(2);

                ((StaticText)oForm.Items.Item(pluginForm.LbTara).Specific).Caption = _tara.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbPesoEnvases).Specific).Caption = _envLote.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbPesoLotes).Specific).Caption = _sumaLotes.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbDifPeso).Specific).Caption = _dif.GetStringFromDouble(2);
                ((StaticText)oForm.Items.Item(pluginForm.LbDifPorc).Specific).Caption = _porc.GetStringFromDouble(2);

                if (_dif >= 0.4)
                {
                    oForm.Freeze(true);
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Image = Iconos.Error;
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.FromPane = 7;
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.ToPane = 7;
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
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.FromPane = 7;
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.ToPane = 7;
                    ((Button)oForm.Items.Item(pluginForm.ButtonImage).Specific).Item.Visible = true;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkRevision).Specific).Checked = false;
                    oForm.Mode = BoFormMode.fm_UPDATE_MODE;
                    ((Button)oForm.Items.Item(pluginForm.ButtonOK).Specific).Item.Click();
                    oForm.Freeze(false);
                    oForm.Update();
                    return _dif;
                }
            }
            catch
            {
                return null;
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
                    double? Balance = RefreshBalance(sbo_application, sessionId);
                    if (Balance == null)
                    {
                        throw new Exception("Error al calcular balance de masa, contacte al administrador");
                    }

                    int seleccion = sbo_application.MessageBox("Esta acción es irreversible \r Asegurece de que los lotes, pesos y calidad están todos bien ingresados antes de continuar \r ¿Desea continuar?", 1, "Si", "Cancelar");
                    if (seleccion == 2)
                        return;
                    //Validación de Balance de masa
                    if (Math.Abs((double)Balance) > 0.049)
                    {
                        seleccion = sbo_application.MessageBox("El balance de masa difiere del 0,4%, ¿Desea recibir esta fruta de todas maneras?", 1, "Si", "Cancelar");
                        if (seleccion == 2)
                            return;
                    }

                    try
                    {
                        foreach (var guia in Recepcion.DFO_TRUCK1Collection.GroupBy(i => i.U_FolioGuia))
                        {
                            string activity = Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key && i.U_LineStatus == 'O').First().U_ClgCode;

                            var resp = CommonFunctions.GET(ServiceLayer.Activities, activity, null, sessionId, out _).DeserializeJsonObject<Activities>();

                            if (resp.DocType == "22")
                            {
                                List<IDocument_Lines> document_Lines = new List<IDocument_Lines>();
                                foreach (var baseline in Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key && i.U_LineStatus == 'O'))
                                {
                                    List<BatchNumbers> batchNumbers = new List<BatchNumbers>();

                                    response = CommonFunctions.GET(ServiceLayer.PurchaseOrders, baseline.U_DocEntry, null, sessionId, out _);
                                    var Oc = response.DeserializeJsonObject<PurchaseOrder>();

                                    if (Recepcion.DFO_TRUCK2Collection.Count(i => i.U_Aprobado == "Y") <= 0)
                                        throw new Exception("No existen lotes aprobados");

                                    foreach (var batchline in Recepcion.DFO_TRUCK2Collection.OrderBy(i => i.U_PesoLote).Where(i => i.U_FolioGuia == baseline.U_FolioGuia && i.U_BaseLine == baseline.U_BaseLine && i.U_Aprobado == "Y"))
                                    {
                                        if (batchline.U_PesoLote <= 0)
                                            throw new Exception("Todos los lotes APROBADOS deben tener peso para poder Finalizar la recepcion");

                                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                        string sSql = "Select \"U_BatchNum\" from \"@DFO_RQLTY3\" where (\"U_BatchNum\")   = '" + batchline.U_Lote + "' ";
                                        oRS.DoQuery(sSql);
                                        if (oRS.RecordCount == 0)
                                            throw new Exception("Todos los lotes deben tener calidad asignada");


                                        BatchNumbers _batch = new BatchNumbers
                                        {
                                            BatchNumber = batchline.U_Lote,
                                            Quantity = batchline.U_PesoLote,
                                            U_FRU_Variedad = baseline.U_Variedad,
                                            U_FRU_Tipo = baseline.U_Tipo,
                                            U_FRU_CantBins = Recepcion.DFO_TRUCK5Collection.Where(i => i.U_Lote == batchline.U_Lote).Sum(i => (int)i.U_Envases),
                                            U_FRU_CantBinsDis = Recepcion.DFO_TRUCK5Collection.Where(i => i.U_Lote == batchline.U_Lote).Sum(i => (int)i.U_Envases),
                                            U_FRU_Productor = batchline.U_CardCode,
                                            U_FRU_NomProveedor = batchline.U_CardName,
                                            U_FRU_FolioCancha = batchline.U_LoteCancha,
                                            U_FRU_Castigo = batchline.U_Castigo
                                        };

                                        batchNumbers.Add(_batch);
                                    }

                                    IDocument_Lines _line = new IDocument_Lines
                                    {
                                        BaseEntry = int.Parse(baseline.U_DocEntry),
                                        BaseLine = int.Parse(baseline.U_BaseLine),
                                        BaseType = "22",
                                        BatchNumbers = batchNumbers,
                                        U_FRU_Variedad = baseline.U_Variedad,
                                        U_FRU_Tipo = baseline.U_Tipo,
                                        Quantity = batchNumbers.Sum(i => i.Quantity)
                                    };

                                    document_Lines.Add(_line);
                                }

                                PurchaseDeliveryNote purchaseDeliveryNote = new PurchaseDeliveryNote
                                {
                                    CardCode = Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key).First().U_CardCode,
                                    DocDate = DateTime.Now.ToString("yyyyMMdd"),
                                    FolioNumber = guia.Key,
                                    FolioPrefixString = "GD",
                                    Comments = Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key).First().U_Obs,
                                    //DiscountPercent = 100,
                                    DocumentLines = document_Lines
                                };

                                response = CommonFunctions.POST(ServiceLayer.PurchaseDeliveryNotes, purchaseDeliveryNote, sessionId, out System.Net.HttpStatusCode httpStatus);
                                if (httpStatus == System.Net.HttpStatusCode.Created)
                                {
                                    sbo_application.StatusBar.SetText("Recepcion cargada con exito", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                    foreach (var line in purchaseDeliveryNote.DocumentLines)
                                    {
                                        foreach (var _rline in Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == purchaseDeliveryNote.FolioNumber && i.U_BaseLine == line.BaseLine.ToString()).ToList()) { _rline.U_LineStatus = 'C'; }
                                    }

                                    response = CommonFunctions.PATCH(ServiceLayer.Recepcion, Recepcion, DocKey, sessionId, out _);

                                    //Se ingresa solicitud de transferencia por los envases pertenecientes a frutexsa

                                    //foreach (var Productor in Recepcion.DFO_TRUCK2Collection.GroupBy(i => i.U_CardCode))
                                    //{
                                    //    var InvTranReqLines = new List<StockTransferLines>();
                                    //    foreach (var lote in Recepcion.DFO_TRUCK2Collection.Where(i => i.U_CardCode == Productor.Key))
                                    //    {
                                    //        foreach (var env in Recepcion.DFO_TRUCK5Collection.Where(i => i.U_Lote == lote.U_Lote))
                                    //        {
                                    //            if (env.U_PropEnv == "2")
                                    //            {
                                    //                //BODEGA PROCESADORA
                                    //                //PATIO NUECES : N1-PAT
                                    //                //PATIO CIRUELA : BPAT
                                    //                //BODEGA PASERA
                                    //                //PATIO BPATIO
                                    //                StockTransferLines DocLines = new StockTransferLines
                                    //                {
                                    //                    ItemCode = env.U_CodEnvase,
                                    //                    Quantity = (double)env.U_Envases,
                                    //                    U_FRU_Caracteristica = env.U_Lote,
                                    //                    FromWarehouseCode = "BPROD",
                                    //                    WarehouseCode = "BPAT"

                                    //                };
                                    //                InvTranReqLines.Add(DocLines);
                                    //            }
                                    //        }
                                    //    }
                                    //    StockTransfer SolicitudEnv = new StockTransfer
                                    //    {
                                    //        CardCode = Productor.Key,
                                    //        StockTransferLines = InvTranReqLines,
                                    //        Comments = "Ingreso de envases Basado en Recepción '" + Recepcion.DocEntry + "' "
                                    //    };
                                    //    var Resp = CommonFunctions.POST(ServiceLayer.InventoryTransferRequests, SolicitudEnv, sessionId, out HttpStatusCode statusCode);

                                    //    if (statusCode == HttpStatusCode.Created)
                                    //    {
                                    //    }
                                    //    else
                                    //    {
                                    //        var objresponse = CommonFunctions.DeserializeJsonToDynamic(response);
                                    //        string errorMsg = objresponse.error.message.value.ToString();
                                    //        throw new Exception(errorMsg);

                                    //    }
                                    //}

                                    //FIN Se ingresa solicitud de transferencia por los envases pertenecientes a frutexsa
                                }
                                else
                                {
                                    var result = response.DeserializeJsonToDynamic();
                                    string errorMsg = result.error.message.value.ToString();
                                    throw new Exception(errorMsg);
                                }
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
                                        response = CommonFunctions.POST($"{ServiceLayer.Recepcion}({DocKey})/Close", null, sessionId, out _);
                                    }
                                    else
                                    {
                                        throw new Exception("error actualizando draft");
                                    }
                                }
                                else
                                {
                                    List<IDocument_Lines> document_Lines = new List<IDocument_Lines>();
                                    foreach (var baseline in Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key && i.U_LineStatus == 'O'))
                                    {

                                        List<BatchNumbers> batchNumbers = new List<BatchNumbers>();

                                        response = CommonFunctions.GET(ServiceLayer.PurchaseOrders, baseline.U_DocEntry, null, sessionId, out _);
                                        var Oc = response.DeserializeJsonObject<PurchaseOrder>();

                                        if (Recepcion.DFO_TRUCK2Collection.Count(i => i.U_Aprobado == "Y") <= 0)
                                            throw new Exception("No existen lotes aprobados");

                                        foreach (var batchline in Recepcion.DFO_TRUCK2Collection.OrderBy(i => i.U_PesoLote).Where(i => i.U_FolioGuia == baseline.U_FolioGuia && i.U_BaseLine == baseline.U_BaseLine && i.U_Aprobado == "Y"))
                                        {

                                            if (batchline.U_PesoLote <= 0)
                                                throw new Exception("Todos los lotes APROBADOS deben tener peso para poder Finalizar la recepcion");

                                            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                            string sSql = "Select \"U_BatchNum\" from \"@DFO_RQLTY3\" where (\"U_BatchNum\")   = '" + batchline.U_Lote + "' ";
                                            oRS.DoQuery(sSql);
                                            if (oRS.RecordCount == 0)
                                                throw new Exception("Todos los lotes deben tener calidad asignada");

                                            BatchNumbers _batch = new BatchNumbers
                                            {
                                                BatchNumber = batchline.U_Lote,
                                                Quantity = batchline.U_PesoLote,
                                                U_FRU_Variedad = baseline.U_Variedad,
                                                U_FRU_Tipo = baseline.U_Tipo,
                                                U_FRU_CantBins = Recepcion.DFO_TRUCK5Collection.Where(i => i.U_Lote == batchline.U_Lote).Sum(i => (int)i.U_Envases),
                                                U_FRU_CantBinsDis = Recepcion.DFO_TRUCK5Collection.Where(i => i.U_Lote == batchline.U_Lote).Sum(i => (int)i.U_Envases),
                                                U_FRU_Productor = batchline.U_CardCode,
                                                U_FRU_NomProveedor = batchline.U_CardName,
                                                U_FRU_FolioCancha = batchline.U_LoteCancha,
                                                U_FRU_Castigo = batchline.U_Castigo
                                            };

                                            batchNumbers.Add(_batch);
                                        }

                                        IDocument_Lines _line = new IDocument_Lines
                                        {
                                            //BaseEntry = int.Parse(baseline.U_DocEntry),
                                            //BaseLine = int.Parse(baseline.U_BaseLine),
                                            //BaseType = "22",
                                            ItemCode = baseline.U_ItemCode,
                                            U_FRU_Variedad = baseline.U_Variedad,
                                            U_FRU_Tipo = baseline.U_Tipo,
                                            BatchNumbers = batchNumbers,
                                            Quantity = batchNumbers.Sum(i => i.Quantity)
                                        };

                                        document_Lines.Add(_line);
                                    }

                                    InventoryGenEntries inventoryGenEntries = new InventoryGenEntries
                                    {
                                        //CardCode = Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key).First().U_CardCode,
                                        DocDate = DateTime.Now.ToString("yyyyMMdd"),
                                        FolioNumber = guia.Key,
                                        FolioPrefixString = "GD",
                                        Comments = Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == guia.Key).First().U_Obs,
                                        //DiscountPercent = 100,
                                        DocumentLines = document_Lines
                                    };

                                    response = CommonFunctions.POST(ServiceLayer.InventoryGenEntries, inventoryGenEntries, sessionId, out System.Net.HttpStatusCode httpStatus);
                                    if (httpStatus == System.Net.HttpStatusCode.Created)
                                    {
                                        sbo_application.StatusBar.SetText("Recepcion cargada con exito", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                        //var delDraft = CommonFunctions.DELETE(ServiceLayer.Drafts, resp.DocEntry, sessionId, out System.Net.HttpStatusCode statusCode);
                                        //if (statusCode == System.Net.HttpStatusCode.NoContent)
                                        //{
                                        //    sbo_application.StatusBar.SetText("Draft eliminado", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                                        //}

                                        foreach (var line in inventoryGenEntries.DocumentLines)
                                        {
                                            foreach (var _rline in Recepcion.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == inventoryGenEntries.FolioNumber)) { _rline.U_LineStatus = 'C'; }
                                        }

                                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, Recepcion, DocKey, sessionId, out _);

                                        //Se ingresa solicitud de transferencia por los envases pertenecientes a frutexsa

                                        //foreach (var Productor in Recepcion.DFO_TRUCK2Collection.GroupBy(i => i.U_CardCode))
                                        //{
                                        //    var InvTranReqLines = new List<StockTransferLines>();
                                        //    foreach (var lote in Recepcion.DFO_TRUCK2Collection.Where(i => i.U_CardCode == Productor.Key))
                                        //    {
                                        //        foreach (var env in Recepcion.DFO_TRUCK5Collection.Where(i => i.U_Lote == lote.U_Lote))
                                        //        {
                                        //            if (env.U_PropEnv == "2")
                                        //            {
                                        //                StockTransferLines DocLines = new StockTransferLines
                                        //                {
                                        //                    ItemCode = env.U_CodEnvase,
                                        //                    Quantity = (double)env.U_Envases,
                                        //                    U_FRU_Caracteristica = env.U_Lote,
                                        //                    FromWarehouseCode = "BPROD",
                                        //                    WarehouseCode = "BPATIO"

                                        //                };
                                        //                InvTranReqLines.Add(DocLines);
                                        //            }
                                        //        }
                                        //    }
                                        //    StockTransfer SolicitudEnv = new StockTransfer
                                        //    {
                                        //        CardCode = Productor.Key,
                                        //        StockTransferLines = InvTranReqLines,
                                        //        Comments = "Ingreso de envases basado en Recepción '" + Recepcion.DocEntry + "' "
                                        //    };
                                        //    var Resp = CommonFunctions.POST(ServiceLayer.InventoryTransferRequests, SolicitudEnv, sessionId, out HttpStatusCode statusCode);

                                        //    if (statusCode == HttpStatusCode.Created)
                                        //    {
                                        //    }
                                        //    else
                                        //    {
                                        //        var objresponse = CommonFunctions.DeserializeJsonToDynamic(response);
                                        //        string errorMsg = objresponse.error.message.value.ToString();
                                        //        throw new Exception(errorMsg);

                                        //    }
                                        //}

                                        //FIN Se ingresa solicitud de transferencia por los envases pertenecientes a frutexsa
                                    }
                                    else
                                    {
                                        var result = response.DeserializeJsonToDynamic();
                                        string errorMsg = result.error.message.value.ToString();
                                        throw new Exception(errorMsg);
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        throw;
                    }

                    Recepcion = CommonFunctions.GET(ServiceLayer.Recepcion, DocKey, null, sessionId, out _).DeserializeJsonObject<Recepcion>();

                    if (Recepcion.DFO_TRUCK1Collection.Count(i => i.U_LineStatus == 'O') == 0)
                    {
                        try
                        {
                            response = CommonFunctions.POST($"{ServiceLayer.Recepcion}({DocKey})/Close", null, sessionId, out _);
                        }
                        catch
                        {
                            throw;
                        }
                    }


                    var rs = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                    var rs2 = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                    string query = $"select ifnull(\"DocEntry\",0) \"DocEntry\" from \"@DFO_ORQLTY\" where \"U_BaseType\"='OTRUCK' and \"U_BaseEntry\"={Recepcion.DocEntry} order by 1";
                    rs.DoQuery(query);

                    while (!rs.EoF)
                    {
                        var query2 = $"delete from \"@DFO_RQLTY4\" where \"DocEntry\"={int.Parse(rs.Fields.Item("DocEntry").Value.ToString())}";
                        rs2.DoQuery(query2);

                        CommonFunctions.ActualizarTotalesPorLote(int.Parse(rs.Fields.Item("DocEntry").Value.ToString()), sessionId);
                        //System.Threading.Thread.Sleep(300);
                        rs.MoveNext();
                    }
                    try
                    {
                        sbo_application.Menus.Item(SAPMenu.Refresh).Activate();
                    }
                    catch
                    {
                    }
                }
            }
        }
    }
}