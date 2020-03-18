using CoreSAPB1;
using CoreUtilities;
using SAPbobsCOM;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace pluginCalidad
{
    internal static class frmCalidad
    {
        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.FormTypeEx == pluginForm.FormType)
            {
                switch (oItemEvent.ItemUID)
                {
                    case pluginForm.ButtonOK:
                        ButtonOK(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case pluginForm.ButtonAsignar:
                        ButtonAsignar(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;
                }

                if (oItemEvent.ItemUID.Contains("bttman"))
                {
                    MuestraManual(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                }

                if (oItemEvent.ItemUID.Contains("add"))
                {
                    AddNewGridRow(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                }

                if (oItemEvent.ItemUID.Contains("gr"))
                {
                    MuestrasCalculadas(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    Porcentajes(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    CamposCalculados(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                }

                if (oItemEvent.EventType == BoEventTypes.et_FORM_CLOSE)
                    FormClose(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
            }

            if (oItemEvent.FormTypeEx == CommonForms.FormLotesCalidad.FormType)
            {
                switch (oItemEvent.ItemUID)
                {
                    case CommonForms.FormLotesCalidad.ButtonOK:
                        ButtonConformarLotes(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;

                    case CommonForms.FormLotesCalidad.GrdLotes.uuid:
                        GridLotes(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                        break;
                }
            }
        }

        internal static void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
        }

        private static void CamposCalculados(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
                {
                    try
                    {
                        oForm.Freeze(true);

                        var attrs = oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDAttrs).ValueEx.DeserializeList<ListadoAtributosCalidad>();
                        var dt = ((Grid)oForm.Items.Item(oItemEvent.ItemUID).Specific).DataTable;
                        var doc = XDocument.Parse(dt.SerializeAsXML(BoDataTableXmlSelect.dxs_All));

                        foreach (var col in doc.Element("DataTable").Elements("Columns").Elements())
                        {
                            var Uid = col.Attribute("Uid").Value;
                            foreach (var attr in attrs.Where(i => i.U_isTotal == "Y" && $"{i.U_Attr} ({i.U_Unidad})" == Uid))
                            {
                                int countVariables = attr.U_Formula.Count(v => v == '$');
                                StringBuilder builder = new StringBuilder(attr.U_Formula);
                                List<Tolerancias> variables = new List<Tolerancias>();

                                for (int i = 1; i <= countVariables; i++)
                                {
                                    int _From = CommonFunctions.IndexOfNth(attr.U_Formula, "$", i);
                                    int _Pos = _From + 1;
                                    int _To = 1;

                                    char ch = attr.U_Formula[_Pos];
                                    if (char.IsDigit(ch))
                                    {
                                        _To++;
                                        _Pos++;
                                    }

                                    try
                                    {
                                        ch = attr.U_Formula[_Pos];
                                        if (char.IsDigit(ch))
                                        {
                                            _To++;
                                            _Pos++;
                                        }

                                        try
                                        {
                                            ch = attr.U_Formula[_Pos];
                                            if (char.IsDigit(ch))
                                            {
                                                _To++;
                                                _Pos++;
                                            }
                                        }
                                        catch { }
                                    }
                                    catch { }

                                    object _valor = null;
                                    var _fieldUid = attr.U_Formula.Substring(_From, _To);
                                    var _fieldVis = Regex.Replace(_fieldUid, @"[^\d]", "");

                                    var title = attrs.Where(i => i.U_VisOrder == _fieldVis).Select(i => i.U_TipoFila).FirstOrDefault();

                                    if (title == "0")
                                    {
                                        var label = (StaticText)oForm.Items.Item($"st{oItemEvent.ItemUID.Replace("gr", "")}").Specific;
                                        _valor = Regex.Replace(Regex.Match(label.Caption, @"\(([^)]*)\)").Groups[1].Value.Trim(), @"[^\d]", "");
                                    }
                                    else
                                    {
                                        var _lin = attrs.Single(i => i.U_VisOrder == _fieldVis);
                                        var _title = attrs.Single(i => i.U_Attr == _lin.U_Father & i.U_TipoFila == "0");
                                        var _gridUid = $"gr{_title.LineId}";

                                        try
                                        {
                                            _valor = ((Grid)oForm.Items.Item(_gridUid).Specific).DataTable.GetValue(attrs.Where(i => i.U_VisOrder == _fieldVis).
                                                Select(i => $"{i.U_Attr} ({i.U_Unidad})").
                                                FirstOrDefault(), oItemEvent.Row);
                                        }
                                        catch
                                        {
                                            _valor = 0.00;
                                        }
                                    }
                                    Tolerancias par = new Tolerancias
                                    {
                                        Uid = _fieldUid,
                                        From = _From,
                                        To = _To,
                                        Value = _valor.ToString().GetDoubleFromString(",")
                                    };

                                    variables.Add(par);
                                }

                                foreach (var item in variables)
                                {
                                    builder.Replace(item.Uid, item.Value.GetStringFromDouble(2));
                                }

                                var _result = CommonFunctions.ParseMath(builder.ToString().Replace('.', ',').Replace("/0", "/1"));

                                dt.SetValue($"{attr.U_Attr} ({attr.U_Unidad})", oItemEvent.Row, _result.GetStringFromDouble(2));
                            }
                        }
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void MuestraManual(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    try
                    {
                        oForm.Freeze(true);
                        var edit = (EditText)oForm.Items.Item($"{oItemEvent.ItemUID.Replace("bttman", "pesoman")}").Specific;

                        if (!double.TryParse(edit.Value, out _))
                            throw new Exception("Ingrese un valor numerico");

                        var st = (StaticText)oForm.Items.Item($"{oItemEvent.ItemUID.Replace("bttman", "st")}").Specific;
                        var oldValue = Regex.Replace(Regex.Match(st.Caption, @"\(([^)]*)\)").Groups[1].Value.Trim(), @"[^\d]", "");

                        if (oldValue.Trim().Length == 0)
                            throw new Exception("La muestra debe venir con un valor desde el maestro para poder ser reemplazada");

                        st.Caption = st.Caption.Replace(oldValue, edit.Value);
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void MuestrasCalculadas(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
                {
                    try
                    {
                        oForm.Freeze(true);

                        var attrs = oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDAttrs).ValueEx.DeserializeList<ListadoAtributosCalidad>();
                        var dt = ((Grid)oForm.Items.Item(oItemEvent.ItemUID).Specific).DataTable;
                        var label = (StaticText)oForm.Items.Item($"st{oItemEvent.ItemUID.Replace("gr", "")}").Specific;
                        var doc = XDocument.Parse(dt.SerializeAsXML(BoDataTableXmlSelect.dxs_All));

                        var oldValue = Regex.Replace(Regex.Match(label.Caption, @"\(([^)]*)\)").Groups[1].Value.Trim(), @"[^\d]", "");
                        var label_attr = Regex.Replace(label.Caption, @"\([^()]*\)", string.Empty);
                        ListadoAtributosCalidad attr = null;

                        try
                        {
                            attr = attrs.Single(i => $"{i.U_Attr}" == label_attr.Trim() && i.U_TipoFila == "0" && i.U_isTotal == "Y");
                        }
                        catch
                        {
                            return;
                        }

                        int countVariables = attr.U_Formula.Count(v => v == '$');
                        StringBuilder builder = new StringBuilder(attr.U_Formula);
                        List<Tolerancias> variables = new List<Tolerancias>();

                        for (int i = 1; i <= countVariables; i++)
                        {
                            int _From = CommonFunctions.IndexOfNth(attr.U_Formula, "$", i);
                            int _Pos = _From + 1;
                            int _To = 1;

                            char ch = attr.U_Formula[_Pos];
                            if (char.IsDigit(ch))
                            {
                                _To++;
                                _Pos++;
                            }

                            try
                            {
                                ch = attr.U_Formula[_Pos];
                                if (char.IsDigit(ch))
                                {
                                    _To++;
                                    _Pos++;
                                }

                                try
                                {
                                    ch = attr.U_Formula[_Pos];
                                    if (char.IsDigit(ch))
                                    {
                                        _To++;
                                        _Pos++;
                                    }
                                }
                                catch { }
                            }
                            catch { }

                            object _valor;
                            var _fieldUid = attr.U_Formula.Substring(_From, _To);
                            var _fieldVis = Regex.Replace(_fieldUid, @"[^\d]", "");

                            var _lin = attrs.Single(i => i.U_VisOrder == _fieldVis);
                            var _tilte = attrs.Single(i => i.U_Attr == _lin.U_Father);
                            var _gridUid = $"gr{_tilte.LineId}";

                            try
                            {
                                _valor = ((Grid)oForm.Items.Item(_gridUid).Specific).DataTable.GetValue(attrs.Where(i => i.U_VisOrder == _fieldVis).
                                    Select(i => $"{i.U_Attr} ({i.U_Unidad})").
                                    FirstOrDefault(), oItemEvent.Row);
                            }
                            catch
                            {
                                _valor = 0.00;
                            }

                            Tolerancias par = new Tolerancias
                            {
                                Uid = _fieldUid,
                                From = _From,
                                To = _To,
                                Value = _valor.ToString().GetDoubleFromString(",")
                            };

                            variables.Add(par);
                        }

                        foreach (var item in variables)
                        {
                            builder.Replace(item.Uid, item.Value.GetStringFromDouble(2));
                        }

                        var _result = CommonFunctions.ParseMath(builder.ToString().Replace('.', ','));
                        label.Caption = $"{attr.U_Attr} ({_result.GetStringFromDouble(2)} {attr.U_Unidad})";
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void Porcentajes(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
                {
                    if (Regex.Match(oItemEvent.ColUID, @"\(([^)]*)\)").Groups[1].Value.Trim() != "%" || oItemEvent.ColUID.ToUpper().Contains("HUM"))
                        return;

                    var label = ((StaticText)oForm.Items.Item($"st{oItemEvent.ItemUID.Replace("gr", "")}").Specific).Caption;
                    var _muestra = Regex.Replace(Regex.Match(label, @"\(([^)]*)\)").Groups[1].Value.Trim(), @"[^\d]", "");

                    var dt = ((Grid)oForm.Items.Item(oItemEvent.ItemUID).Specific).DataTable;
                    var _value = dt.GetValue(oItemEvent.ColUID, oItemEvent.Row).ToString();

                    if (!string.IsNullOrEmpty(_muestra) && !string.IsNullOrEmpty(_value))
                    {
                        var _result = (_value.GetDoubleFromString(",") / _muestra.GetDoubleFromString(",")) * 100;
                        dt.SetValue(oItemEvent.ColUID, oItemEvent.Row, _result.GetStringFromDouble(2));
                    }
                }
            }
        }

        private static void Tolerancia(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
                {
                    if (oItemEvent.ItemUID.Contains("e"))
                    {
                        try
                        {
                            var oForm = sbo_application.Forms.Item(formUID);
                            var formItem = oForm.Items.Item(oItemEvent.ItemUID);

                            if (formItem.Type == BoFormItemTypes.it_EDIT)
                            {
                                string[] _Tope = ((StaticText)oForm.Items.Item("ll" + formItem.UniqueID.Substring(1, formItem.UniqueID.Length - 1)).Specific).Caption.Split(' ');
                                string _Operador = _Tope[0];
                                string _Valor = _Tope[1];

                                string[] _Rango;
                                string _Min;
                                string _Max;

                                string _Dato = ((EditText)oForm.Items.Item(oItemEvent.ItemUID).Specific).Value.Replace('.', ',').Trim();

                                if (_Valor == "Obtenido")
                                    return;

                                if (_Valor.Contains("-") && _Operador == "Entre")
                                {
                                    _Rango = _Valor.Split('-');
                                    _Min = _Rango[0].Replace('.', ',').Trim();
                                    _Max = _Rango[1].Replace('.', ',').Trim();

                                    var isNumeric = double.TryParse(_Min, out double n);
                                    if (!isNumeric)
                                        throw new Exception("El rango minimo no es un numerico");

                                    isNumeric = double.TryParse(_Max, out n);
                                    if (!isNumeric)
                                        throw new Exception("El rango maximo no es un numerico");

                                    if (double.Parse(_Dato) < double.Parse(_Min))
                                    {
                                        ((EditText)oForm.Items.Item(oItemEvent.ItemUID).Specific).BackColor = Colores.Yellow;
                                        return;
                                    }
                                    else if (double.Parse(_Dato) > double.Parse(_Max))
                                    {
                                        ((EditText)oForm.Items.Item(oItemEvent.ItemUID).Specific).BackColor = Colores.Red;
                                        return;
                                    }
                                    else
                                    {
                                        ((EditText)oForm.Items.Item(oItemEvent.ItemUID).Specific).BackColor = Colores.GreenYellow;
                                        return;
                                    }
                                }
                                else
                                {
                                    var isNumeric = double.TryParse(_Valor, out double n);
                                    if (!isNumeric)
                                        throw new Exception("El rango maximo no es un numerico");

                                    var _Edit = oForm.Items.Item(oItemEvent.ItemUID).Specific as EditText;
                                    bool _res = CommonFunctions.Operator(_Operador, double.Parse(_Dato), double.Parse(_Valor));
                                    switch (_Operador)
                                    {
                                        case ">=": if (_res) { _Edit.BackColor = Colores.Red; } else { _Edit.BackColor = Colores.GreenYellow; } break;
                                        case "<=": if (_res) { _Edit.BackColor = Colores.GreenYellow; } else { _Edit.BackColor = Colores.Red; } break;
                                        case ">": if (_res) { _Edit.BackColor = Colores.Red; } else { _Edit.BackColor = Colores.GreenYellow; } break;
                                        case "<": if (_res) { _Edit.BackColor = Colores.GreenYellow; } else { _Edit.BackColor = Colores.Red; } break;
                                        case "=": if (_res) { _Edit.BackColor = Colores.GreenYellow; } else { _Edit.BackColor = Colores.Red; } break;
                                        case "!=": if (_res) { _Edit.BackColor = Colores.GreenYellow; } else { _Edit.BackColor = Colores.Red; } break;
                                    }
                                }
                            }
                        }
                        catch (Exception e) { sbo_application.StatusBar.SetText(e.Message, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error); }
                    }
                }
            }
        }

        private static void ButtonOK(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            Form oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
                //Validaciones
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    GrabarRegistroCalidad(ref sbo_application, sbo_company, oItemEvent, sessionId);
                }
            }
        }

        private static void ButtonAsignar(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    var _Cabecera = oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDCab).ValueEx.DeserializeJsonToDynamic();
                    var _vista = new ServiceLayer.ListadoLotesOrdenFabricacion(int.Parse(_Cabecera.Valor.ToString()), oForm.Title.RemoveParents().Trim());
                    var response = CommonFunctions.GET(_vista.url, null, "?$filter=Disponible gt 0", sessionId, out _);
                    var _xml = response.json2xml(CommonForms.FormLotesCalidad.GrdLotes.dt);
                    var oFormLotes = SAPFunctions.LoadFormLotesCalidad(ref sbo_application, _xml) as Form;
                    oFormLotes.DataSources.UserDataSources.Item(CommonForms.FormLotesCalidad.UDFather).ValueEx = formUID;
                }
            }
        }

        private static void ButtonConformarLotes(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    if (oForm.Mode == BoFormMode.fm_UPDATE_MODE)
                    {
                        List<RegistroCalidad_Lotes> lotes = new List<RegistroCalidad_Lotes>();

                        XDocument doc = XDocument.Parse(oForm.DataSources.DataTables.Item(CommonForms.FormLotesCalidad.GrdLotes.dt).SerializeAsXML(BoDataTableXmlSelect.dxs_DataOnly));

                        foreach (var Row in doc.Element("DataTable").Elements("Rows").Elements())
                        {
                            var value = Row
                                .Element("Cells")
                                .Elements("Cell")
                                .Single(x => x.Element("ColumnUid").Value == "Asignar")
                                .Element("Value")
                                .Value;

                            if (value == "Y")
                            {
                                var _lote = new RegistroCalidad_Lotes
                                {
                                    DocEntry = null,
                                    LineId = null,
                                    U_BatchNum = Row.Element("Cells").Elements("Cell").Single(x => x.Element("ColumnUid").Value == "Lote").Element("Value").Value,
                                    U_Kg = Row.Element("Cells").Elements("Cell").Single(x => x.Element("ColumnUid").Value == "Kilos").Element("Value").Value.GetDoubleFromString(",")
                                };

                                lotes.Add(_lote);
                            }
                        }

                        if (lotes != null)
                        {
                            var FatherFormUID = oForm.DataSources.UserDataSources.Item(CommonForms.FormLotesCalidad.UDFather).ValueEx;
                            var FatherForm = sbo_application.Forms.Item(FatherFormUID);
                            FatherForm.DataSources.UserDataSources.Item(pluginForm.UDLotes).ValueEx = lotes.SerializeJson();
                        }
                    }
                }
            }
        }

        private static void GridLotes(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    try
                    {
                        var oForm = sbo_application.Forms.Item(formUID);
                        var p = oForm.DataSources.DataTables.Item(CommonForms.FormLotesCalidad.GrdLotes.dt).GetValue("Disponible", oItemEvent.Row);
                        oForm.DataSources.DataTables.Item(CommonForms.FormLotesCalidad.GrdLotes.dt).SetValue("Kilos", oItemEvent.Row, p);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }

        private static void AddNewGridRow(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    string date = DateTime.Now.ToString("dd/MM/yyyy");
                    string time = DateTime.Now.ToString("HH:mm:ss");
                    Form oForm = null;
                    try
                    {
                        oForm = sbo_application.Forms.Item(oItemEvent.FormUID);
                        oForm.Freeze(true);
                        var grduid = "gr" + oItemEvent.ItemUID.Replace("add", "");
                        Grid grid = null;

                        grid = sbo_application.Forms.ActiveForm.Items.Item(grduid).Specific as Grid;
                        grid.DataTable.Rows.Add();

                        string Prefix = "M";

                        var _index = grid.DataTable.Rows.Count - 1;
                        grid.DataTable.SetValue(0, _index, (_index + 1).ToString());
                        grid.DataTable.SetValue(1, _index, Prefix + (_index + 1).ToString());
                        grid.DataTable.SetValue(2, _index, date);
                        grid.DataTable.SetValue(3, _index, time);
                        grid.AutoResizeColumns();
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

        private static void GrabarRegistroCalidad(ref Application sbo_application, SAPbobsCOM.Company sbo_company, ItemEvent oItemEvent, string sessionId)
        {
            Form oForm = null;
            DataTable dt = null;
            Grid grid = null;

            try
            {
                oForm = sbo_application.Forms.Item(oItemEvent.FormUID);
                var _atributos = new List<RegistroCalidad_Detalle>();
                var _totales = new List<RegistroCalidad_Totales>();
                var _lotes = new List<RegistroCalidad_Lotes>();
                var _DataTables = new List<RegCalidadDataTables>();
                var _TextosCortos = CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, null, sessionId, out _).DeserializeList<MaestroTextosCortos>();

                var titulosStdGrid = new List<string>
                {
                    "#",
                    "Muestra[Editable]",
                    "Fecha",
                    "Hora"
                };

                for (int i = 0; i < oForm.DataSources.DataTables.Count; i++)
                {
                    dt = oForm.DataSources.DataTables.Item(i);
                    var grduid = "gr" + dt.UniqueID.Replace("dt", "");
                    grid = oForm.Items.Item(grduid).Specific as Grid;
                    var dtXML = XDocument.Parse(dt.SerializeAsXML(BoDataTableXmlSelect.dxs_All));
                    var Columns = dtXML.Element("DataTable").Element("Columns").Elements("Column");
                    var Rows = dtXML.Element("DataTable").Element("Rows").Elements("Row");
                    int _LineNum = 1;

                    var _dtCal = new RegCalidadDataTables { dtXML = dt.SerializeAsXML(BoDataTableXmlSelect.dxs_All) };
                    _DataTables.Add(_dtCal);

                    foreach (var Row in Rows)
                    {
                        var Cells = Row.Element("Cells").Elements("Cell");
                        foreach (var Cell in Cells)
                        {
                            string ColValue = string.Empty;
                            var ColName = Cell.Element("ColumnUid").Value;
                            var MaxLength = dtXML.Element("DataTable").Element("Columns").Elements("Column").Where(i => i.Attribute("Uid").Value == ColName).Select(i => i.Attribute("MaxLength").Value).FirstOrDefault();

                            if (int.Parse(MaxLength) == 254 && !titulosStdGrid.Any(word => ColName.Contains(word)))
                            {
                                ColValue = Cell.Element("Value").Value;
                                if (!string.IsNullOrEmpty(ColValue))
                                    ColValue = CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Code eq {ColValue}", sessionId, out _).DeserializeList<MaestroTextosCortos>().First().U_Texto;
                            }
                            else
                            {
                                ColValue = Cell.Element("Value").Value;
                            }

                            var _attr = new RegistroCalidad_Detalle
                            {
                                DocEntry = null,
                                LineId = null,
                                U_LineNum = _LineNum,
                                U_Attr = ColName,
                                U_Value = (ColValue.Length > 254) ? "" : ColValue,
                                U_Text = (ColValue.Length > 254) ? ColValue : "",
                                U_Title = grid.Item.Description,
                            };

                            _atributos.Add(_attr);
                        }
                        _LineNum++;
                    }

                    foreach (var column in _atributos.Where(i => i.U_Title == grid.Item.Description).GroupBy(i => i.U_Attr))
                    {
                        if (!titulosStdGrid.Any(word => column.Key == word))
                        {
                            sbo_application.StatusBar.SetText($"Procesando {column.Key}", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);

                            if (double.TryParse(_atributos.Where(i => i.U_Title == grid.Item.Description && i.U_Attr == column.Key).Select(i => i.U_Value).FirstOrDefault(), out _))
                            {
                                var _sum = _atributos.Where(i => i.U_Title == grid.Item.Description && i.U_Attr == column.Key).Average(i => i.U_Value.GetDoubleFromString(","));
                                var _count = _atributos.Count(i => i.U_Title == grid.Item.Description && i.U_Attr == column.Key);

                                if (_sum > 0)
                                {
                                    var _total = new RegistroCalidad_Totales
                                    {
                                        DocEntry = null,
                                        LineId = null,
                                        U_LineNum = _count,
                                        U_Attr = column.Key,
                                        U_Value = _sum.GetStringFromDouble(2),
                                        U_Title = grid.Item.Description
                                    };

                                    _totales.Add(_total);
                                }
                            }
                            else
                            {
                                var _count = _atributos.Count(i => i.U_Title == grid.Item.Description && i.U_Attr == column.Key);
                                var _total = new RegistroCalidad_Totales
                                {
                                    DocEntry = null,
                                    LineId = null,
                                    U_LineNum = _count,
                                    U_Attr = column.Key,
                                    U_Value = _atributos.Where(i => i.U_Title == grid.Item.Description && i.U_Attr == column.Key).Select(i => i.U_Value).FirstOrDefault(),
                                    U_Title = grid.Item.Description
                                };

                                _totales.Add(_total);
                            }
                        }
                    }
                }

                var _lotesStr = oForm.DataSources.UserDataSources.Item("UDLotes").ValueEx;
                if (!string.IsNullOrEmpty(_lotesStr))
                {
                    _lotes = _lotesStr.DeserializeList<RegistroCalidad_Lotes>();
                }

                var Obj = oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDCab).ValueEx.DeserializeJsonToDynamic();

                if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx))
                {
                    var Registro = new RegistroCalidad
                    {
                        DocEntry = null,
                        DocNum = null,
                        U_PuntoControl = oForm.Title.RemoveParents(),
                        U_Version = 0.0,
                        U_FormXML = _DataTables.SerializeJson(),
                        U_TotalKg = _lotes.Sum(i => i.U_Kg),
                        U_BaseType = Obj.Tipo.ToString(),
                        U_BaseEntry = Obj.Valor.ToString(),
                        DFO_RQLTY1Collection = _atributos,
                        DFO_RQLTY2Collection = _totales,
                        DFO_RQLTY3Collection = _lotes,
                        DFO_RQLTY4Collection = null
                    };

                    var response = CommonFunctions.POST(ServiceLayer.RegistroCalidad, Registro, sessionId, out System.Net.HttpStatusCode httpStatus);
                    if (httpStatus == System.Net.HttpStatusCode.Created)
                    {
                        var RegOK = response.DeserializeJsonObject<RegistroCalidad>();

                        oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx = RegOK.DocEntry.ToString();
                        if (Registro.U_TotalKg > 0)
                        {
                            sbo_application.StatusBar.SetText("Calculando y actualizando calidad en tarjas", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                            CommonFunctions.ActualizarTotalesPorLote((int)RegOK.DocEntry, sessionId);
                        }
                        sbo_application.MessageBox($"Registro creado con éxito, número de correlativo {RegOK.DocEntry}");

                        oForm.Title = oForm.Title.TrimEnd(')') + $" Correlativo: {RegOK.DocEntry})";

                        foreach (var lot in RegOK.DFO_RQLTY3Collection)
                        {
                            string upd = $"update \"@DFO_OCAOF\" set \"U_RegCalidad\" = '{RegOK.DocEntry}' where \"U_IdTarja\"='{lot.U_BatchNum}'";
                            var rs = sbo_company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                            rs.DoQuery(upd);
                        }
                    }
                    else
                    {
                        var _Error = response.DeserializeJsonToDynamic();
                        throw new Exception($"Error en el registro : {_Error.error.message.value.ToString()}");
                    }
                }
                else
                {
                    string delR1 = $"delete from \"@DFO_RQLTY1\" where \"DocEntry\"={oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx}";
                    string delR2 = $"delete from \"@DFO_RQLTY2\" where \"DocEntry\"={oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx}";
                    string delR3 = $"delete from \"@DFO_RQLTY3\" where \"DocEntry\"={oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx}";
                    string delR4 = $"delete from \"@DFO_RQLTY4\" where \"DocEntry\"={oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx}";

                    var rs = sbo_company.GetBusinessObject(BoObjectTypes.BoRecordset) as Recordset;
                    rs.DoQuery(delR1);
                    rs.DoQuery(delR2);
                    rs.DoQuery(delR3);
                    rs.DoQuery(delR4);

                    var Registro = new RegistroCalidad
                    {
                        U_PuntoControl = oForm.Title.RemoveParents(),
                        U_Version = 0.0,
                        U_FormXML = _DataTables.SerializeJson(),
                        U_TotalKg = _lotes.Sum(i => i.U_Kg),
                        U_BaseType = Obj.Tipo.ToString(),
                        U_BaseEntry = Obj.Valor.ToString(),
                        DFO_RQLTY1Collection = _atributos,
                        DFO_RQLTY2Collection = _totales,
                        DFO_RQLTY3Collection = _lotes,
                        DFO_RQLTY4Collection = null
                    };

                    var response = CommonFunctions.PATCH(ServiceLayer.RegistroCalidad, Registro, oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx, sessionId, out System.Net.HttpStatusCode httpStatus);
                    if (httpStatus == System.Net.HttpStatusCode.NoContent)
                    {
                        if (Registro.U_TotalKg > 0)
                            CommonFunctions.ActualizarTotalesPorLote(int.Parse(oForm.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx), sessionId);

                        sbo_application.MessageBox($"Registro {Registro.DocEntry} actualizado con éxito");
                    }
                    else
                    {
                        var _Error = response.DeserializeJsonToDynamic();
                        throw new Exception($"Error en la actualiazacion : {_Error.error.message.value.ToString()}");
                    }
                }
            }
            catch
            {
                throw;
            }
        }

        private static void FormClose(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var HasChanges = false;
                var oForm = sbo_application.Forms.Item(oItemEvent.FormUID);

                if (string.IsNullOrEmpty(oForm.DataSources.UserDataSources.Item(pluginForm.UDLotes).ValueEx.Trim()))
                {
                    for (int i = 0; i < oForm.DataSources.DataTables.Count; i++)
                    {
                        if (!HasChanges)
                        {
                            var dt = oForm.DataSources.DataTables.Item(i);
                            if (dt.Rows.Count > 0)
                            {
                                HasChanges = true;
                            }
                        }
                    }
                    if (HasChanges)
                    {
                        if (sbo_application.MessageBox("No ha asignado lotes al registro, ¿Está seguro que desea salir?", 2, "Si", "No") == 2)
                        {
                            bBubbleEvent = false;
                            return;
                        }
                    }
                }
            }
        }
    }
}