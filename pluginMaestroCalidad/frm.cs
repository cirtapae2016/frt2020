using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace pluginCalidadMaestro
{
    internal static class frm

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
                    oForm.DataBrowser.BrowseBy = "3";

                    ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLProceso);
                    Conditions oCons = oCFL.GetConditions();

                    Condition oCon = oCons.Add();
                    oCon.Alias = "DimCode";
                    oCon.Operation = BoConditionOperation.co_EQUAL;
                    oCon.CondVal = "2";

                    oCFL.SetConditions(oCons);

                    var oMatrix = oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix;
                    oMatrix.AutoResizeColumns();
                    oMatrix.AddRow();
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

            switch (oItemEvent.ItemUID)
            {
                case pluginForm.TxtPuntoControl:
                case pluginForm.CmbFruta:
                case pluginForm.TxtProceso:
                    TxtPuntoControl(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.MatrixAttr.Uid:
                    MatrixAttr(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonPreview:
                    ButtonPreview(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonPreview2:
                    ButtonPreview2(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonPreview3:
                    ButtonPreview3(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
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
            }
        }

        internal static void RightClickEventHandler(ref ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
            if (!eventInfo.BeforeAction)
            {
                SAPFunctions.AddRightClickMenu(ref sbo_application, UserMenu.DeleteRow, "Borrar Fila", true, BoMenuType.mt_STRING, SAPMenu.RightClickMenu);
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
                                var oMatrix = oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix;
                                oMatrix.AutoResizeColumns();
                                oMatrix.AddRow();
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
                                var oMatrix = oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix;
                                var oCell = oMatrix.GetCellFocus();
                                oMatrix.SelectRow(oCell.rowIndex, true, false);
                                oMatrix.SetCellFocus(1, oCell.ColumnIndex);
                                oMatrix.DeleteRow(oCell.rowIndex);
                                oForm.Mode = BoFormMode.fm_UPDATE_MODE;

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

        private static void ButtonPreview(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            throw new Exception("Funcion no disponible");
        }

        private static void ButtonPreview2(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            throw new Exception("Funcion no disponible");
        }

        private static void ButtonPreview3(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    _ = SAPFunctions.LoadFormCalidad(ref sbo_application, oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).GetValue("Code", 0), sessionId, null, 'Y');
                }
            }
        }

        private static void MatrixAttr(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.ColUID == pluginForm.MatrixAttr.Colums.Col_Formula.Uid && oForm.Mode == BoFormMode.fm_ADD_MODE && (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS || (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN && oItemEvent.CharPressed == 9)))
                {
                    var oCheck = (oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix).Columns.Item(pluginForm.MatrixAttr.Colums.Col_Calc.Uid).Cells.Item(oItemEvent.Row).Specific as CheckBox;
                    if (oCheck.Checked)
                    {
                        var oCell = (oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix).Columns.Item(pluginForm.MatrixAttr.Colums.Col_Formula.Uid).Cells.Item(oItemEvent.Row);
                        var oEdit = oCell.Specific as EditText;
                        string _Formula = oEdit.Value;

                        try { _Formula = FormatString(_Formula); }
                        catch (Exception e) { sbo_application.MessageBox(e.Message); bBubbleEvent = false; oCell.Click(); }
                        finally { oEdit.Value = _Formula; }
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.ColUID == pluginForm.MatrixAttr.Colums.Col_Attr.Uid && oForm.Mode != BoFormMode.fm_FIND_MODE && (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS || (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN && oItemEvent.CharPressed == 9)))
                {
                    var oMatrix = oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix;

                    if (oItemEvent.Row >= oMatrix.RowCount)
                    {
                        if (oItemEvent.Row == 1)
                        {
                            var oEditText = (EditText)oMatrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Attr.Uid).Cells.Item(oItemEvent.Row).Specific;

                            string sr = oEditText.String;

                            if (oEditText.String != "")
                            {
                                //oMatrix.AddRow(1, oMatrix.RowCount);
                                //((EditText)oMatrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_LineId.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = (oItemEvent.Row + 1).ToString();
                                //((EditText)oMatrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Attr.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = string.Empty;

                                ((Matrix)oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific).FlushToDataSource();

                                DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbAttr);
                                //det.Clear();

                                int offset = det.Size;
                                det.InsertRecord(offset);

                                ((Matrix)oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific).LoadFromDataSourceEx();
                            }
                        }
                        else
                        {
                            var oEditText = (EditText)oMatrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Attr.Uid).Cells.Item(oItemEvent.Row).Specific;

                            string sr = oEditText.String;

                            if (oEditText.String != "")
                            {
                                //oMatrix.AddRow(1, oItemEvent.Row);
                                //((EditText)oMatrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_LineId.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = (oItemEvent.Row + 1).ToString();
                                //((EditText)oMatrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Attr.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = string.Empty;

                                ((Matrix)oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific).FlushToDataSource();

                                DBDataSource det = oForm.DataSources.DBDataSources.Item(pluginForm.dbAttr);
                                //det.Clear();

                                int offset = det.Size;
                                det.InsertRecord(offset);

                                ((Matrix)oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific).LoadFromDataSourceEx();
                            }
                        }
                    }
                }

                if (oItemEvent.ColUID == pluginForm.MatrixAttr.Colums.Col_Father.Uid && oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    try
                    {
                        oForm.Freeze(true);
                        DBDataSource Det = oForm.DataSources.DBDataSources.Item(pluginForm.dbAttr);

                        var matrix = oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix;
                        var combo = (ComboBox)matrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Father.Uid).Cells.Item(oItemEvent.Row).Specific;
                        var titulos = new Dictionary<int, string>();

                        try
                        {
                            while (combo.ValidValues.Count > 0)
                                combo.ValidValues.Remove(0, BoSearchKey.psk_Index);
                        }
                        catch { }

                        for (int i = 1; i < matrix.RowCount; i++)
                        {
                            var isnumeric = int.TryParse(((ComboBox)matrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_TipoFila.Uid).Cells.Item(i).Specific).Value.Trim(), out int n);
                            if (isnumeric)
                            {
                                int tipo = int.Parse(((ComboBox)matrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_TipoFila.Uid).Cells.Item(i).Specific).Value.Trim());
                                string title = ((EditText)matrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Attr.Uid).Cells.Item(i).Specific).Value.Trim();

                                if (tipo == 0)
                                {
                                    titulos.Add(i, title);
                                }
                            }
                        }

                        foreach (KeyValuePair<int, string> entry in titulos)
                        {
                            try { combo.ValidValues.Add(entry.Value, entry.Key.ToString()); }
                            catch { };
                        }
                    }
                    catch (Exception ex)
                    {
                        sbo_application.MessageBox(ex.Message);
                    }
                    finally
                    {
                        oForm.Freeze(false);
                    }
                }
            }
        }

        private static void FormDataLoad(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oForm = sbo_application.Forms.Item(businessObjectInfo.FormUID);

            if (businessObjectInfo.BeforeAction)
            {
            }

            if (!businessObjectInfo.BeforeAction)
            {
                try
                {
                    oForm.Freeze(true);
                    ((ComboBox)oForm.Items.Item(pluginForm.CmbFruta).Specific).Item.Enabled = false;
                    //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtProceso).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtPuntoControl).Specific).Item.Enabled = false;
                    ((EditText)oForm.Items.Item(pluginForm.TxtCliente).Specific).Item.Enabled = false;
                }
                catch (Exception e) { sbo_application.MessageBox(e.Message); }
                finally { oForm.Freeze(false); }

                if ((businessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_LOAD || businessObjectInfo.EventType == BoEventTypes.et_FORM_DATA_ADD) && businessObjectInfo.ActionSuccess)
                {
                    var matrix = oForm.Items.Item(pluginForm.MatrixAttr.Uid).Specific as Matrix;
                    if (matrix.RowCount > 0)
                    {
                        var combo = (ComboBox)matrix.Columns.Item(pluginForm.MatrixAttr.Colums.Col_Father.Uid).Cells.Item(1).Specific;

                        while (combo.ValidValues.Count > 0)
                            combo.ValidValues.Remove(0, BoSearchKey.psk_Index);
                    }
                }
            }
        }

        private static void TxtPuntoControl(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if ((oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS || (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN && oItemEvent.CharPressed == 9)) && oForm.Mode == BoFormMode.fm_ADD_MODE)
                {
                    string _Frut = ((ComboBox)oForm.Items.Item(pluginForm.CmbFruta).Specific).Value.Trim();
                    string _Proc = ((EditText)oForm.Items.Item(pluginForm.TxtProceso).Specific).Value.Trim();
                    string _CodCal = ((EditText)oForm.Items.Item(pluginForm.TxtPuntoControl).Specific).Value.Trim();

                    ((EditText)oForm.Items.Item(pluginForm.TxtCode).Specific).Value = string.Format("{0}-{1}-{2}", _Frut, _Proc, _CodCal);
                }

                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && oItemEvent.ItemUID == pluginForm.TxtProceso)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        try
                        {
                            ((EditText)oForm.Items.Item(pluginForm.TxtNombreProceso).Specific).Value = oDT.GetValue("PrcName", 0).ToString();
                            //EditText)oForm.Items.Item(pluginForm.TxtProceso).Specific).Value = oDT.GetValue("PrcCode", 0).ToString();
                            oForm.DataSources.DBDataSources.Item(pluginForm.dbCabecera).SetValue("U_Proceso", 0, oDT.GetValue("PrcCode", 0).ToString());
                        }
                        catch { }
                    }
                }
            }
        }

        private static string FormatString(string expression)
        {
            if (string.IsNullOrEmpty(expression))
            {
                throw new ArgumentNullException("Formula vacía");
            }

            StringBuilder formattedString = new StringBuilder();
            int balanceOfParenth = 0;

            for (int i = 0; i < expression.Length; i++)
            {
                char ch = expression[i];

                if (ch == '(')
                {
                    balanceOfParenth++;
                }
                else if (ch == ')')
                {
                    balanceOfParenth--;
                }

                if (char.IsWhiteSpace(ch))
                {
                    continue;
                }
                else if (char.IsUpper(ch))
                {
                    formattedString.Append(char.ToLower(ch));
                }
                else
                {
                    formattedString.Append(ch);
                }
            }

            if (balanceOfParenth != 0)
            {
                throw new FormatException("Revise la cantidad de parentesis");
            }

            return formattedString.ToString();
        }
    }
}