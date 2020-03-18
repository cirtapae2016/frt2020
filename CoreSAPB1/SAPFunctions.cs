using CoreUtilities;
using OfficeOpenXml;
using Sap.Data.Hana;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace CoreSAPB1
{
    public static class SAPFunctions
    {
        public static void AssignUserQueries(string pName, string pCategory, string pForm, string pItem, string pCol, SAPbobsCOM.Company sbo_company)
        {
            SAPbobsCOM.Recordset oRecordSet = null;
            bool Transaction = false;

            string CategoryId;
            string QueryId;
            string IndexID = "1";

            try
            {
                oRecordSet = ((SAPbobsCOM.Recordset)(sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset)));

                oRecordSet.DoQuery("SELECT \"CategoryId\" FROM OQCN WHERE \"CatName\"='" + pCategory + "'");
                if (oRecordSet.RecordCount > 0)
                {
                    CategoryId = Convert.ToString(oRecordSet.Fields.Item("CategoryId").Value);

                    oRecordSet.DoQuery("SELECT \"IntrnalKey\" FROM OUQR WHERE \"QCategory\"=" + CategoryId + " AND \"QName\"='" + pName + "'");
                    if (oRecordSet.RecordCount > 0)
                    {
                        QueryId = Convert.ToString(oRecordSet.Fields.Item("IntrnalKey").Value);

                        oRecordSet.DoQuery("SELECT \"QueryId\" FROM CSHS WHERE \"FormID\"='" + pForm + "' AND \"ItemID\"='" + pItem + "' AND \"ColID\"='" + pCol + "'");
                        if (oRecordSet.RecordCount == 0)
                        {
                            Transaction = true;
                            sbo_company.StartTransaction();

                            oRecordSet.DoQuery("SELECT TOP 1 \"IndexID\" FROM CSHS ORDER BY \"IndexID\" DESC");
                            if (oRecordSet.RecordCount > 0)
                            {
                                IndexID = Convert.ToString(Convert.ToInt32(oRecordSet.Fields.Item(0).Value) + 1);
                            }
                            else
                            {
                                IndexID = "1";
                            }

                            oRecordSet.DoQuery("INSERT INTO CSHS (\"FormID\", \"ItemID\", \"ColID\", \"ActionT\", \"QueryId\", \"IndexID\", \"Refresh\", \"FieldID\",\"FrceRfrsh\", \"ByField\")" +
                                " VALUES ('" + pForm + "','" + pItem + "','" + pCol + "',2," + QueryId + "," + IndexID + ",'N',null,'N','N')");

                            sbo_company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                            Transaction = false;

                            // Assignment created
                        }
                        else
                        {
                            if (Convert.ToString(oRecordSet.Fields.Item("QueryId").Value) == QueryId)
                            {
                                // same Assignment exists
                            }
                            else
                            {
                                // ERROR: Another assignment exists. Can't create new one
                            }
                        }
                    }
                    else
                    {
                        // ERROR: Query not found
                    }
                }
                else
                {
                    // ERROR: Category not found
                }
            }
            catch (Exception e)
            {
                if (Transaction == true)
                {
                    sbo_company.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);
                }

                // other error
            }
            finally
            {
                oRecordSet = null;
                GC.Collect();
            }
        }

        public static string GetFieldFromSelectedRow(SAPbouiCOM.Grid grid, string columnName)
        {
            if (grid.Rows.SelectedRows.Count == 0) return string.Empty;

            int rowIndex = grid.Rows.SelectedRows.Item(0, SAPbouiCOM.BoOrderType.ot_RowOrder);

            return Convert.ToString(grid.DataTable.GetValue(columnName, rowIndex));
        }

        public static string GetDescriptionFromValidValue(string Table, string Alias, string Index, SAPbobsCOM.Company sbo_company)
        {
            try
            {
                var rs = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                var sql = $"select B.\"Descr\" from CUFD A inner join UFD1 B on A.\"FieldID\" = B.\"FieldID\" and A.\"TableID\" = B.\"TableID\" where A.\"TableID\" = '{Table}' and A.\"AliasID\" = replace('{Alias}', 'U_', '') and B.\"FldValue\" = '{Index}'";
                rs.DoQuery(sql);

                if (!rs.EoF)
                {
                    return rs.GetColumnValue(0).ToString();
                }
                else
                {
                    return "Descripcion no encontrada";
                }
            }
            catch { throw; }
        }

        public static object ChooseFromListEvent(SAPbouiCOM.ItemEvent oItemEvent)
        {
            try
            {
                SAPbouiCOM.IChooseFromListEvent oCFLEvent = (SAPbouiCOM.IChooseFromListEvent)(oItemEvent);
                return oCFLEvent.SelectedObjects;
            }
            catch
            {
                throw;
            }
        }

        public static object LoadFormCalidadOld(ref SAPbouiCOM.Application sbo_application, string Code, string sessionId, char Preview = 'N')
        {
            SAPbouiCOM.Form oFormCalidad = null;
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormCalidad.FormType);

                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);
                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormCalidad.FormType + CommonFunctions.Random().ToString();
                oFormCalidad = sbo_application.Forms.AddEx(FormCreationPackage);

                try
                {
                    SAPbouiCOM.Item Label_1 = null;
                    SAPbouiCOM.Item Label_2 = null;
                    SAPbouiCOM.Item Edit = null;
                    SAPbouiCOM.Item Check = null;
                    SAPbouiCOM.UserDataSource Uds = null;
                    SAPbouiCOM.StaticText St = null;
                    SAPbouiCOM.Button OK = (SAPbouiCOM.Button)oFormCalidad.Items.Item(CommonForms.FormCalidad.ButtonOK).Specific;
                    SAPbouiCOM.Button Cancel = (SAPbouiCOM.Button)oFormCalidad.Items.Item(CommonForms.FormCalidad.ButtonCancel).Specific;
                    SAPbouiCOM.Button Calc = (SAPbouiCOM.Button)oFormCalidad.Items.Item(CommonForms.FormCalidad.ButtonCalc).Specific;
                    SAPbouiCOM.EditText Obj = (SAPbouiCOM.EditText)oFormCalidad.Items.Item(CommonForms.FormCalidad.ExtObj).Specific;
                    SAPbouiCOM.EditText Version = (SAPbouiCOM.EditText)oFormCalidad.Items.Item(CommonForms.FormCalidad.TxtVersion).Specific;

                    int _top = CommonForms.FormCalidad.TopIni;
                    int _height = 0;

                    if (string.IsNullOrEmpty(sessionId))
                        sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                    var Locations = CommonFunctions.GET(ServiceLayer.ActivityLocations, null, null, sessionId, out _).DeserializeList<ActivityLocations>();

                    foreach (var item in Locations.Where(i => i.Code != "-2"))
                        ((SAPbouiCOM.ComboBox)oFormCalidad.Items.Item(CommonForms.FormCalidad.CmbLocation).Specific).ValidValues.Add(item.Code, item.Name);

                    string response = CommonFunctions.GET(ServiceLayer.ListadoAtributosCalidad, null, string.Format("?$filter = Code eq '{0}' and U_Activo eq 'Y'&$orderby = U_VisOrder", Code), sessionId, out _);

                    foreach (var item in response.DeserializeList<ListadoAtributosCalidad>())
                    {
                        if (!string.IsNullOrEmpty(item.U_Attr))
                        {
                            string _tolerancia = item.U_Tolerancia;
                            string _tope = item.U_Tope;

                            if (item.U_TipoFila == "0")
                            {
                                Label_1 = oFormCalidad.Items.Add("l" + item.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                                Label_1.Top = _top;
                                Label_1.Left = CommonForms.FormCalidad.LblLeft;
                                Label_1.Width = CommonForms.FormCalidad.LblWidth;
                                Label_1.TextStyle = 1;
                                Label_1.FontSize = 12;
                                St = (SAPbouiCOM.StaticText)Label_1.Specific;
                                St.Caption = item.U_Attr;
                            }
                            else
                            {
                                Label_1 = oFormCalidad.Items.Add("l" + item.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                                Label_1.Top = _top;
                                Label_1.Left = CommonForms.FormCalidad.LblLeft;
                                Label_1.Width = CommonForms.FormCalidad.LblWidth;
                                St = (SAPbouiCOM.StaticText)Label_1.Specific;
                                St.Caption = item.U_Attr;

                                Label_2 = oFormCalidad.Items.Add("ll" + item.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                                Label_2.Top = _top;
                                Label_2.Left = CommonForms.FormCalidad.LblTopeLeft;
                                Label_2.Width = CommonForms.FormCalidad.LblWidth;
                                St = (SAPbouiCOM.StaticText)Label_2.Specific;

                                if (item.U_Tolerancia == "O")
                                    _tolerancia = "=";

                                if (item.U_Tolerancia == "In")
                                    _tolerancia = "Entre";

                                if (item.U_Tolerancia == "O")
                                    _tope = "Obtenido";

                                St.Caption = string.Format("{0} {1}", _tolerancia, _tope);

                                if (item.U_TipoDato == "dt_BOOLEAN")
                                {
                                    Check = oFormCalidad.Items.Add("c" + item.LineId, SAPbouiCOM.BoFormItemTypes.it_CHECK_BOX);
                                    Check.Top = _top;
                                    Check.Left = CommonForms.FormCalidad.TxtLeft;
                                    Check.Width = CommonForms.FormCalidad.TxtWidth;

                                    Uds = oFormCalidad.DataSources.UserDataSources.Add("u" + item.LineId, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, Convert.ToInt32(item.U_Largo));

                                    SAPbouiCOM.CheckBox Ch = (SAPbouiCOM.CheckBox)Check.Specific;
                                    Ch.DataBind.SetBound(true, "", Uds.UID);
                                }
                                else
                                {
                                    SAPbouiCOM.BoDataType myenum = (SAPbouiCOM.BoDataType)Enum.Parse(typeof(SAPbouiCOM.BoDataType), item.U_TipoDato);

                                    Edit = (myenum == SAPbouiCOM.BoDataType.dt_LONG_TEXT) ? oFormCalidad.Items.Add("e" + item.LineId, SAPbouiCOM.BoFormItemTypes.it_EXTEDIT) : oFormCalidad.Items.Add("e" + item.LineId, SAPbouiCOM.BoFormItemTypes.it_EDIT);

                                    Edit.Top = _top;
                                    Edit.Left = CommonForms.FormCalidad.TxtLeft;
                                    Edit.Width = CommonForms.FormCalidad.TxtWidth;

                                    Edit.RightJustified = (myenum == SAPbouiCOM.BoDataType.dt_SHORT_NUMBER || myenum == SAPbouiCOM.BoDataType.dt_QUANTITY) ? true : false;
                                    Uds = oFormCalidad.DataSources.UserDataSources.Add("u" + item.LineId, myenum, Convert.ToInt32(item.U_Largo));

                                    SAPbouiCOM.EditText Ed = (SAPbouiCOM.EditText)Edit.Specific;
                                    Ed.DataBind.SetBound(true, "", Uds.UID);

                                    if (Uds.DataType == SAPbouiCOM.BoDataType.dt_SHORT_NUMBER)
                                        Uds.ValueEx = "0";
                                }

                                if (item.U_isTotal == "Y")
                                {
                                    Edit.Enabled = false;
                                    Edit.TextStyle = 1;
                                    Label_1.TextStyle = 4;
                                    Label_2.TextStyle = 4;
                                }
                            }

#if DEBUG
                            Console.WriteLine(item.LineId);
#endif

                            _height += Label_1.Height;
                            _top += Label_1.Height + 2;

                            oFormCalidad.Title = item.Code;
                            Version.Value = item.U_Version;
                        }
                    }

                    oFormCalidad.Height = oFormCalidad.Top + _top + 5;

                    OK.Item.Top = _top + (OK.Item.Height * 2);
                    Cancel.Item.Top = _top + (Cancel.Item.Height * 2);
                    Calc.Item.Top = _top + (Calc.Item.Height * 2);
                    Obj.Item.Top = _top;
                    Obj.Value = response;

                    Version.Item.Enabled = false;

                    if (Preview != 'N')
                    {
                        oFormCalidad.Title = string.Format("{0} {1}", "[Previsualizacion]", oFormCalidad.Title);
                        OK.Item.Enabled = false;
                    }
                }
                catch
                {
                    throw;
                }
                finally { oFormCalidad.Freeze(false); oFormCalidad.Visible = true; }

                return oFormCalidad;
            }
            catch { throw; }
        }

        public static object LoadFormCalidadOld2(ref SAPbouiCOM.Application sbo_application, string Code, string sessionId)
        {
            Code = "CIR-REP-RG5.5.1.2MP";
            SAPbouiCOM.Form oFormCalidad = null;
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormCalidad.FormType + "2");

                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);
                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormCalidad.FormType + CommonFunctions.Random().ToString();
                oFormCalidad = sbo_application.Forms.AddEx(FormCreationPackage);

                int _top = 51; ;
                int _height = 111;
                int _left = 24;
                int _width = 795;

                if (string.IsNullOrEmpty(sessionId))
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                string response = CommonFunctions.GET(ServiceLayer.ListadoAtributosCalidad, null, string.Format("?$filter = Code eq '{0}' and U_Activo eq 'Y'&$orderby = U_VisOrder", Code), sessionId, out _);
                var attrs = response.DeserializeList<ListadoAtributosCalidad>();

                SAPbouiCOM.Matrix matrix = null;
                SAPbouiCOM.StaticText staticText = null;
                SAPbouiCOM.Item item = null;
                SAPbouiCOM.Column column = null;
                SAPbouiCOM.UserDataSource userDataSource = null;
                SAPbouiCOM.Button button = null;

                foreach (var attr in attrs.Where(i => i.U_TipoFila == "0"))
                {
                    item = oFormCalidad.Items.Add("st" + attr.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = _left;
                    item.Width = 177;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = attr.U_Attr;

                    item = oFormCalidad.Items.Add("add" + attr.LineId, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 206;
                    item.Width = 17;
                    button = item.Specific as SAPbouiCOM.Button;
                    button.Caption = "...";

                    _top = _top + 14 + 1;

                    item = oFormCalidad.Items.Add("mx" + attr.LineId, SAPbouiCOM.BoFormItemTypes.it_MATRIX);
                    item.Top = _top;
                    item.Height = _height;
                    item.Left = _left;
                    item.Width = _width;
                    matrix = item.Specific as SAPbouiCOM.Matrix;

                    column = matrix.Columns.Add("#", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                    column.TitleObject.Caption = "#";
                    column.Width = 30;
                    column.Editable = false;

                    column = matrix.Columns.Add("0", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                    column.TitleObject.Caption = "Atributos";
                    column.Width = 100;
                    column.Editable = false;

                    userDataSource = oFormCalidad.DataSources.UserDataSources.Add("C" + matrix.Item.UniqueID + column.UniqueID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
                    column.DataBind.SetBound(true, "", userDataSource.UID);

                    //for (int i = 1; i < 4; i++)
                    //{
                    //    matrix.AddRow();
                    //    editText = matrix.Columns.Item("0").Cells.Item(i).Specific as SAPbouiCOM.EditText;
                    //    editText.Value = "Atrubito " + i.ToString();
                    //}

                    _top = _top + _height + 5;
                }

                item = oFormCalidad.Items.Add("1", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = _left;

                item = oFormCalidad.Items.Add("2", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = item.Left + item.Width + 5;

                _top += 5;

                oFormCalidad.Height = _top;
                oFormCalidad.Title = Code;

                oFormCalidad.Visible = true;
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }

            return oFormCalidad;
        }

        public static object LoadFormCalidadOld3(ref SAPbouiCOM.Application sbo_application, string Code, string sessionId)
        {
            SAPbouiCOM.Form oFormCalidad = null;
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormCalidad.FormType + "2");

                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);
                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormCalidad.FormType + CommonFunctions.Random().ToString();
                oFormCalidad = sbo_application.Forms.AddEx(FormCreationPackage);

                int _top = 51; ;
                int _height = 111;
                int _left = 24;
                int _width = 795;

                if (string.IsNullOrEmpty(sessionId))
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                string response = CommonFunctions.GET(ServiceLayer.ListadoAtributosCalidad, null, string.Format("?$filter = Code eq '{0}' and U_Activo eq 'Y'&$orderby = U_VisOrder", Code), sessionId, out _);
                var attrs = response.DeserializeList<ListadoAtributosCalidad>();

                SAPbouiCOM.Grid grid = null;
                SAPbouiCOM.StaticText staticText = null;
                SAPbouiCOM.Item item = null;
                SAPbouiCOM.DataTable dataTable = null;
                SAPbouiCOM.Button button = null;

                foreach (var title in attrs.Where(i => i.U_TipoFila == "0"))
                {
                    item = oFormCalidad.Items.Add("st" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = _left;
                    item.Width = 177;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = title.U_Attr;

                    item = oFormCalidad.Items.Add("add" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 206;
                    item.Width = 17;
                    button = item.Specific as SAPbouiCOM.Button;
                    button.Caption = "...";

                    _top = _top + 14 + 1;

                    item = oFormCalidad.Items.Add("gr" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_GRID);
                    item.Top = _top;
                    item.Height = _height;
                    item.Left = _left;
                    item.Width = _width;
                    grid = item.Specific as SAPbouiCOM.Grid;
                    grid.Item.Description = title.U_Attr;

                    dataTable = oFormCalidad.DataSources.DataTables.Add("dt" + title.LineId);

                    dataTable.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 254);
                    dataTable.Columns.Add("Atributos", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 254);

                    grid.DataTable = dataTable;

                    grid.Columns.Item(0).Editable = false;
                    grid.Columns.Item(0).Width = 100;
                    grid.Columns.Item(1).Editable = false;
                    grid.Columns.Item(1).Width = 100;

                    foreach (var attr in attrs.Where(i => i.U_Father == title.U_Attr))
                    {
                        dataTable.Rows.Add();
                        dataTable.Columns.Item(1).Cells.Item(dataTable.Rows.Count - 1).Value = attr.U_Attr;
                    }

                    _top = _top + _height + 5;
                    grid.AutoResizeColumns();
                }
                _top += 100;
                item = oFormCalidad.Items.Add("1", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = _left;

                item = oFormCalidad.Items.Add("2", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = item.Left + item.Width + 50;

                _top += 50;

                oFormCalidad.Height = _top;
                oFormCalidad.Title = Code;

                oFormCalidad.Visible = true;
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }

            return oFormCalidad;
        }

        public static object LoadFormCalidad(ref SAPbouiCOM.Application sbo_application, string Code, string sessionId, dynamic Cabecera = null, char Preview = 'N')
        {
            SAPbouiCOM.Form oFormCalidad = null;
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            var Version = string.Empty;
            var Desc = string.Empty;

            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormCalidad.FormType);

                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);
                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormCalidad.FormType + CommonFunctions.Random().ToString();
                oFormCalidad = sbo_application.Forms.AddEx(FormCreationPackage);

                int _top = 51;
                int _height = 111;
                int _left = 24;
                int _width = 795;

                if (string.IsNullOrEmpty(sessionId))
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                var attrs = CommonFunctions.GET(ServiceLayer.ListadoAtributosCalidad, null, $"?$filter = Code eq '{Code.RemoveParents()}' and U_Activo eq 'Y'&$orderby = U_VisOrder", sessionId, out _).DeserializeList<ListadoAtributosCalidad>();

                if (attrs.Count == 0)
                    throw new Exception("Error al cargar el registro");

                oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDAttrs).ValueEx = attrs.SerializeJson();

                Version = attrs[0].U_Version;
                Desc = attrs[0].U_Descripcion;

                SAPbouiCOM.Grid grid = null;
                SAPbouiCOM.StaticText staticText = null;
                SAPbouiCOM.EditText editText = null;
                SAPbouiCOM.Item item = null;
                SAPbouiCOM.DataTable dataTable = null;
                SAPbouiCOM.Button button = null;

                foreach (var title in attrs.Where(i => i.U_TipoFila == "0"))
                {
                    item = oFormCalidad.Items.Add("st" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = _left;
                    item.Width = 230;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = $"{title.U_Attr} ({title.U_Tope} {title.U_Unidad})";

                    item = oFormCalidad.Items.Add("add" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 300;
                    item.Width = 17;
                    button = item.Specific as SAPbouiCOM.Button;
                    button.Caption = "+";

                    item = oFormCalidad.Items.Add("bttman" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 320;
                    item.Width = 100;
                    button = item.Specific as SAPbouiCOM.Button;
                    button.Caption = "Muestra Manual";

                    item = oFormCalidad.Items.Add("pesoman" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_EDIT);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 430;
                    item.Width = 100;
                    editText = item.Specific as SAPbouiCOM.EditText;
                    editText.Value = "Ingrese peso";

                    _top = _top + 14 + 1;

                    item = oFormCalidad.Items.Add("gr" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_GRID);
                    item.Top = _top;
                    item.Height = _height;
                    item.Left = _left;
                    item.Width = _width;
                    grid = item.Specific as SAPbouiCOM.Grid;
                    grid.Item.Description = title.U_Attr;

                    dataTable = oFormCalidad.DataSources.DataTables.Add("dt" + title.LineId);
                    dataTable.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_Integer, 10);
                    dataTable.Columns.Add("Muestra[Editable]", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 254);
                    dataTable.Columns.Add("Fecha", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 10);
                    dataTable.Columns.Add("Hora", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 8);

                    foreach (var attr in attrs.Where(i => (i.U_Father == title.U_Attr && i.U_TipoFila == "1")))
                    {
                        if (attr.U_TipoDato == "dt_BOOLEAN")
                        {
                            dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 1);
                        }
                        else
                        {
                            SAPbouiCOM.BoDataType _enum = (SAPbouiCOM.BoDataType)Enum.Parse(typeof(SAPbouiCOM.BoDataType), attr.U_TipoDato);
                            SAPbouiCOM.BoFieldsType _fieldtype = SAPbouiCOM.BoFieldsType.ft_AlphaNumeric;

                            switch (_enum)
                            {
                                case SAPbouiCOM.BoDataType.dt_LONG_NUMBER:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Float;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_SHORT_NUMBER:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Integer;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_PERCENT:
                                case SAPbouiCOM.BoDataType.dt_SUM:
                                case SAPbouiCOM.BoDataType.dt_MEASURE:
                                case SAPbouiCOM.BoDataType.dt_RATE:
                                case SAPbouiCOM.BoDataType.dt_PRICE:
                                case SAPbouiCOM.BoDataType.dt_QUANTITY:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Quantity;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_LONG_TEXT:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Text;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_SHORT_TEXT:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_AlphaNumeric;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_DATE:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Date;
                                    break;
                            };

                            switch (_fieldtype)
                            {
                                case SAPbouiCOM.BoFieldsType.ft_Date:
                                case SAPbouiCOM.BoFieldsType.ft_Float:
                                case SAPbouiCOM.BoFieldsType.ft_Quantity:
                                case SAPbouiCOM.BoFieldsType.ft_Integer:
                                    dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 10);
                                    //try { dataTable.Columns.Add(attr.U_Attr, _fieldtype, 10); }
                                    //catch { dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 10); }
                                    break;

                                case SAPbouiCOM.BoFieldsType.ft_AlphaNumeric:
                                    dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 254);
                                    //try { dataTable.Columns.Add(attr.U_Attr, _fieldtype, 254); }
                                    //catch { dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 254); }

                                    break;

                                case SAPbouiCOM.BoFieldsType.ft_Text:
                                    dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype);
                                    //try { dataTable.Columns.Add(attr.U_Attr, _fieldtype); }
                                    //catch { dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype); }
                                    break;
                            }
                        }
                    }

                    grid.DataTable = dataTable;

                    grid.Columns.Item(0).Editable = false;
                    grid.Columns.Item(0).Width = 30;
                    grid.Columns.Item(1).Editable = true;
                    grid.Columns.Item(1).Width = 100;
                    grid.Columns.Item(2).Editable = false;
                    grid.Columns.Item(2).Width = 70;
                    grid.Columns.Item(3).Editable = false;
                    grid.Columns.Item(3).Width = 70;

                    _top = _top + _height + 5;

                    for (int i = 4; i < grid.Columns.Count; i++)
                    {
                        if (dataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && dataTable.Columns.Item(i).MaxLength == 254)
                        {
                            var _editCol = (SAPbouiCOM.EditTextColumn)grid.Columns.Item(i);
                            _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                            var _cbCol = (SAPbouiCOM.ComboBoxColumn)grid.Columns.Item(i);
                            _cbCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description;

                            var _tipTx = attrs.Where(i => i.U_Attr == _editCol.TitleObject.Caption.RemoveParents() && i.U_TipoFila == "1").Select(i => i.U_TipTxt).FirstOrDefault();
                            if (string.IsNullOrEmpty(_tipTx))
                            {
                                foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                {
                                    _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                }
                            }
                            else
                            {
                                foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}' and U_Tipo eq '{_tipTx}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                {
                                    _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                }
                            }
                        }
                        else if (dataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && dataTable.Columns.Item(i).MaxLength == 1)
                        {
                            var _editCol = (SAPbouiCOM.EditTextColumn)grid.Columns.Item(i);
                            _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                        }
                    }

                    grid.AutoResizeColumns();
                }

                item = oFormCalidad.Items.Add("edAccion", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
                item.Top = _top + 5;
                item.Height = 80;
                item.Left = _left;
                item.Width = _width - 15;
                item.Enabled = false;
                editText = item.Specific as SAPbouiCOM.EditText;
                editText.Value = $"{attrs[0].U_Accion}";

                _top += 100;
                item = oFormCalidad.Items.Add("1", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = _left;
                if (Preview == 'Y')
                    item.Enabled = false;

                item = oFormCalidad.Items.Add("2", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = item.Width + 50;
                if (Preview == 'Y')
                    item.Enabled = false;

                item = oFormCalidad.Items.Add("3", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = (item.Width) * 2 + 75;
                item.Width = 150;
                if (Preview == 'Y')
                    item.Enabled = false;

                button = item.Specific as SAPbouiCOM.Button;
                button.Caption = "Asignar a egreso produccion";

                _top += 70;

                oFormCalidad.Left = sbo_application.Desktop.Left;
                oFormCalidad.Width += 10;

                oFormCalidad.Height = _top;

                if (Cabecera != null)
                {
                    oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDCab).ValueEx = CommonFunctions.SerializeJson(Cabecera);
                    if (Cabecera.Lote != "Y")
                    {
                        item.Enabled = false;
                    }

                    item = oFormCalidad.Items.Add("stTipoReg", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = 5;
                    item.Left = 30;
                    item.Width = 115;
                    item.TextStyle = 1;
                    staticText = item.Specific as SAPbouiCOM.StaticText;

                    switch (Cabecera.Tipo)
                    {
                        case "1":
                            staticText.Caption = "Registro al día: ";
                            break;

                        case "2":
                            staticText.Caption = "Registro a la semana: ";
                            break;

                        case "3":
                            staticText.Caption = "Registro al mes: ";
                            break;

                        case "4":
                            staticText.Caption = "Orden de fabricacion: ";
                            break;

                        case "5":
                            staticText.Caption = "Despacho: ";
                            break;

                        case "OTRUCK":
                            staticText.Caption = "Recepcion: ";
                            break;

                        case "67":
                            staticText.Caption = "Fumigado: ";
                            break;
                    }

                    item = oFormCalidad.Items.Add("stFecha", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = 20;
                    item.Left = 30;
                    item.Width = 50;
                    item.TextStyle = 1;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = "Fecha";

                    item = oFormCalidad.Items.Add("stFechaV", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = 20;
                    item.Left = 140;
                    item.Width = 50;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = DateTime.Today.ToString("dd/MM/yyyy");

                    item = oFormCalidad.Items.Add("stDato", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = 5;
                    item.Left = 140;
                    item.Width = 50;
                    item.RightJustified = true;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = Cabecera.Valor;

                    if (Cabecera.Tipo == "4")
                    {
                        var _Of = CommonFunctions.GET(ServiceLayer.ListadoOrdenesFabricacion, null, $"?$filter=DocEntry eq {Cabecera.Valor}", sessionId, out _).DeserializeJsonObject<ListadoOrdenesFabricacion>();

                        staticText.Caption = _Of.DocNum.ToString();

                        item = oFormCalidad.Items.Add("stPedido", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 200;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Pedido: ";

                        item = oFormCalidad.Items.Add("stOrigin", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 260;
                        item.Width = 50;
                        item.RightJustified = true;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = _Of.OriginNum.ToString();

                        item = oFormCalidad.Items.Add("stCantidad", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 20;
                        item.Left = 200;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Cantidad: ";

                        item = oFormCalidad.Items.Add("stQuantity", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 20;
                        item.Left = 260;
                        item.Width = 50;
                        item.RightJustified = true;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = _Of.PlannedQty.GetStringFromDouble();

                        item = oFormCalidad.Items.Add("stVariedad", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 320;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Variedad: ";

                        item = oFormCalidad.Items.Add("stUVar", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 380;
                        item.Width = 50;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = _Of.U_FRU_Variedad;

                        item = oFormCalidad.Items.Add("stCalibre", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 20;
                        item.Left = 320;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Calibre: ";

                        item = oFormCalidad.Items.Add("stUCalibre", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 20;
                        item.Left = 380;
                        item.Width = 50;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = _Of.U_FRU_Calibre;

                        item = oFormCalidad.Items.Add("stTipo", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 35;
                        item.Left = 320;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Tipo: ";

                        item = oFormCalidad.Items.Add("stUTipo", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 35;
                        item.Left = 380;
                        item.Width = 200;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = _Of.U_FRU_Tipo;

                        item = oFormCalidad.Items.Add("stCliente", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 440;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Cliente: ";

                        item = oFormCalidad.Items.Add("stCardName", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 500;
                        item.Width = 350;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = $"{_Of.CardCode}:{_Of.CardName}";
                    }

                    if (Cabecera.Tipo == "OTRUCK")
                    {
                        var _Obj = CommonFunctions.GET(ServiceLayer.Recepcion, $"{Cabecera.Valor}", null, sessionId, out _).DeserializeJsonObject<Recepcion>();

                        item = oFormCalidad.Items.Add("stCantidad", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 20;
                        item.Left = 200;
                        item.Width = 50;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Cantidad: ";

                        item = oFormCalidad.Items.Add("stQuantity", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 20;
                        item.Left = 260;
                        item.Width = 50;
                        item.RightJustified = true;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = _Obj.DFO_TRUCK2Collection.Where(i => i.U_Lote == Cabecera.Lote).Select(i => i.U_PesoLote.GetStringFromDouble(2)).FirstOrDefault();

                        item = oFormCalidad.Items.Add("stCliente", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 440;
                        item.Width = 55;
                        item.TextStyle = 1;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = "Productor: ";

                        var _Guia = _Obj.DFO_TRUCK2Collection.Where(i => i.U_Lote == Cabecera.Lote).Select(i => i.U_FolioGuia).FirstOrDefault();
                        item = oFormCalidad.Items.Add("stCardName", SAPbouiCOM.BoFormItemTypes.it_STATIC);
                        item.Top = 5;
                        item.Left = 500;
                        item.Width = 350;
                        staticText = item.Specific as SAPbouiCOM.StaticText;
                        staticText.Caption = $"{_Obj.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == _Guia).Select(i => i.U_CardCode).FirstOrDefault()}:{_Obj.DFO_TRUCK1Collection.Where(i => i.U_FolioGuia == _Guia).Select(i => i.U_RznSoc).FirstOrDefault()}";

                        List<RegistroCalidad_Lotes> lotes = new List<RegistroCalidad_Lotes> {
                            new RegistroCalidad_Lotes
                            {
                                DocEntry = null,
                                LineId = null,
                                U_BatchNum = Cabecera.Lote,
                                U_Kg = _Obj.DFO_TRUCK2Collection.Where(i => i.U_Lote == Cabecera.Lote).Select(i => i.U_PesoLote).FirstOrDefault()
                            }
                        };

                        if (lotes != null)
                        {
                            oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDLotes).ValueEx = lotes.SerializeJson();
                        }

                        try
                        {
                            var url = new ServiceLayer.CalidadByLote(Cabecera.Lote, Code).url;
                            var CalidadKey = CommonFunctions.GET(url, null, null, sessionId, out _).DeserializeJsonToDynamic();
                            var Key = $"{CalidadKey.value[0].DocEntry.ToString()}";

                            var RegistroCalidad = CommonFunctions.GET(ServiceLayer.RegistroCalidad, Key, null, sessionId, out _).DeserializeJsonObject<RegistroCalidad>();

                            foreach (var dt in RegistroCalidad.U_FormXML.DeserializeList<RegCalidadDataTables>())
                            {
                                var dtXML = XDocument.Parse(dt.dtXML);
                                var dtUid = dtXML.Element("DataTable").Attribute("Uid").Value;
                                oFormCalidad.DataSources.DataTables.Item(dtUid).LoadFromXML(dtXML.ToString());

                                var _grid = oFormCalidad.Items.Item(dtUid.Replace("dt", "gr")).Specific as SAPbouiCOM.Grid;
                                _grid.DataTable = oFormCalidad.DataSources.DataTables.Item(dtUid);

                                for (int i = 4; i < _grid.Columns.Count; i++)
                                {
                                    if (_grid.DataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && _grid.DataTable.Columns.Item(i).MaxLength == 254)
                                    {
                                        var _editCol = (SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i);
                                        _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                                        var _cbCol = (SAPbouiCOM.ComboBoxColumn)_grid.Columns.Item(i);
                                        _cbCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description;

                                        var _tipTx = attrs.Where(i => i.U_Attr == _editCol.TitleObject.Caption.RemoveParents() && i.U_TipoFila == "1").Select(i => i.U_TipTxt).FirstOrDefault();
                                        if (string.IsNullOrEmpty(_tipTx))
                                        {
                                            foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                            {
                                                _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}' and U_Tipo eq '{_tipTx}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                            {
                                                _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                            }
                                        }
                                    }
                                    else if (_grid.DataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && _grid.DataTable.Columns.Item(i).MaxLength == 1)
                                    {
                                        var _editCol = (SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i);
                                        _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                                    }
                                }
                            }

                            oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx = Key;
                        }
                        catch { }
                    }

                    if (Cabecera.Tipo == "67")
                    {
                        oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDLotes).ValueEx = Cabecera.Lote;

                        try
                        {
                            string _lotesStr = Cabecera.Lote.ToString();
                            var _lotes = _lotesStr.DeserializeList<RegistroCalidad_Lotes>();
                            var url = new ServiceLayer.CalidadByLote(_lotes[0].U_BatchNum, Code).url;
                            var CalidadKey = CommonFunctions.GET(url, null, null, sessionId, out _).DeserializeJsonToDynamic();
                            var Key = $"{CalidadKey.value[0].DocEntry.ToString()}";

                            var RegistroCalidad = CommonFunctions.GET(ServiceLayer.RegistroCalidad, Key, null, sessionId, out _).DeserializeJsonObject<RegistroCalidad>();

                            foreach (var dt in RegistroCalidad.U_FormXML.DeserializeList<RegCalidadDataTables>())
                            {
                                var dtXML = XDocument.Parse(dt.dtXML);
                                var dtUid = dtXML.Element("DataTable").Attribute("Uid").Value;

                                oFormCalidad.DataSources.DataTables.Item(dtUid).LoadFromXML(dtXML.ToString());

                                var _grid = oFormCalidad.Items.Item(dtUid.Replace("dt", "gr")).Specific as SAPbouiCOM.Grid;

                                for (int i = 4; i < _grid.Columns.Count; i++)
                                {
                                    if (_grid.DataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && _grid.DataTable.Columns.Item(i).MaxLength == 254)
                                    {
                                        var _editCol = (SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i);
                                        _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                                        var _cbCol = (SAPbouiCOM.ComboBoxColumn)_grid.Columns.Item(i);
                                        _cbCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description;

                                        var _tipTx = attrs.Where(i => i.U_Attr == _editCol.TitleObject.Caption.RemoveParents() && i.U_TipoFila == "1").Select(i => i.U_TipTxt).FirstOrDefault();
                                        if (string.IsNullOrEmpty(_tipTx))
                                        {
                                            foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                            {
                                                _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                            }
                                        }
                                        else
                                        {
                                            foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}' and U_Tipo eq '{_tipTx}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                            {
                                                _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                            }
                                        }
                                    }
                                    else if (_grid.DataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && _grid.DataTable.Columns.Item(i).MaxLength == 1)
                                    {
                                        var _editCol = (SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i);
                                        _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                                    }
                                }
                            }

                            oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx = Key;
                        }
                        catch { }
                    }
                }

                if (string.IsNullOrEmpty(oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx))
                {
                    oFormCalidad.Title = $"{Code} ({Desc} Version:{Version} Analista:{sbo_application.Company.UserName})";
                }
                else
                {
                    oFormCalidad.Title = $"{Code} ({Desc} Version:{Version} Analista:{sbo_application.Company.UserName} Correlativo:{oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx})";
                }

                oFormCalidad.Visible = true;
            }
            catch
            {
                throw;
            }

            return oFormCalidad;
        }

        public static object LoadAproveCalidad(ref SAPbouiCOM.Application sbo_application, string DocEntry, string sessionId)
        {
            SAPbouiCOM.Form oFormCalidad = null;
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormCalidad.FormType);

                var RegistroCalidad = CommonFunctions.GET(ServiceLayer.RegistroCalidad, DocEntry, null, sessionId, out System.Net.HttpStatusCode statusCode).DeserializeJsonObject<RegistroCalidad>();
                if (statusCode == System.Net.HttpStatusCode.NotFound)
                    throw new Exception("Registro no encontrado");

                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);
                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormCalidad.FormType + CommonFunctions.Random().ToString();
                oFormCalidad = sbo_application.Forms.AddEx(FormCreationPackage);

                int _top = 51;
                int _height = 111;
                int _left = 24;
                int _width = 795;

                if (string.IsNullOrEmpty(sessionId))
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                var attrs = CommonFunctions.GET(ServiceLayer.ListadoAtributosCalidad, null, $"?$filter = Code eq '{RegistroCalidad.U_PuntoControl.RemoveParents()}' and U_Activo eq 'Y'&$orderby = U_VisOrder", sessionId, out _).DeserializeList<ListadoAtributosCalidad>();
                oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDAttrs).ValueEx = attrs.SerializeJson();

                SAPbouiCOM.Grid grid = null;
                SAPbouiCOM.StaticText staticText = null;
                SAPbouiCOM.EditText editText = null;
                SAPbouiCOM.Item item = null;
                SAPbouiCOM.DataTable dataTable = null;
                SAPbouiCOM.Button button = null;

                foreach (var title in attrs.Where(i => i.U_TipoFila == "0"))
                {
                    item = oFormCalidad.Items.Add("st" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_STATIC);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = _left;
                    item.Width = 230;
                    staticText = item.Specific as SAPbouiCOM.StaticText;
                    staticText.Caption = $"{title.U_Attr} ({title.U_Tope} {title.U_Unidad})";

                    item = oFormCalidad.Items.Add("add" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 300;
                    item.Width = 17;
                    button = item.Specific as SAPbouiCOM.Button;
                    button.Caption = "+";

                    item = oFormCalidad.Items.Add("bttman" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 320;
                    item.Width = 100;
                    button = item.Specific as SAPbouiCOM.Button;
                    button.Caption = "Muestra Manual";

                    item = oFormCalidad.Items.Add("pesoman" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_EDIT);
                    item.Top = _top;
                    item.Height = 14;
                    item.Left = 430;
                    item.Width = 100;
                    editText = item.Specific as SAPbouiCOM.EditText;
                    editText.Value = "Ingrese peso";

                    _top = _top + 14 + 1;

                    item = oFormCalidad.Items.Add("gr" + title.LineId, SAPbouiCOM.BoFormItemTypes.it_GRID);
                    item.Top = _top;
                    item.Height = _height;
                    item.Left = _left;
                    item.Width = _width;
                    grid = item.Specific as SAPbouiCOM.Grid;
                    grid.Item.Description = title.U_Attr;

                    dataTable = oFormCalidad.DataSources.DataTables.Add("dt" + title.LineId);
                    dataTable.Columns.Add("#", SAPbouiCOM.BoFieldsType.ft_Integer, 10);
                    dataTable.Columns.Add("Muestra[Editable]", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 254);
                    dataTable.Columns.Add("Fecha", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 10);
                    dataTable.Columns.Add("Hora", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 8);

                    foreach (var attr in attrs.Where(i => (i.U_Father == title.U_Attr && i.U_TipoFila == "1")))
                    {
                        if (attr.U_TipoDato == "dt_BOOLEAN")
                        {
                            dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", SAPbouiCOM.BoFieldsType.ft_AlphaNumeric, 1);
                        }
                        else
                        {
                            SAPbouiCOM.BoDataType _enum = (SAPbouiCOM.BoDataType)Enum.Parse(typeof(SAPbouiCOM.BoDataType), attr.U_TipoDato);
                            SAPbouiCOM.BoFieldsType _fieldtype = SAPbouiCOM.BoFieldsType.ft_AlphaNumeric;

                            switch (_enum)
                            {
                                case SAPbouiCOM.BoDataType.dt_LONG_NUMBER:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Float;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_SHORT_NUMBER:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Integer;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_PERCENT:
                                case SAPbouiCOM.BoDataType.dt_SUM:
                                case SAPbouiCOM.BoDataType.dt_MEASURE:
                                case SAPbouiCOM.BoDataType.dt_RATE:
                                case SAPbouiCOM.BoDataType.dt_PRICE:
                                case SAPbouiCOM.BoDataType.dt_QUANTITY:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Quantity;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_LONG_TEXT:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Text;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_SHORT_TEXT:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_AlphaNumeric;
                                    break;

                                case SAPbouiCOM.BoDataType.dt_DATE:
                                    _fieldtype = SAPbouiCOM.BoFieldsType.ft_Date;
                                    break;
                            };

                            switch (_fieldtype)
                            {
                                case SAPbouiCOM.BoFieldsType.ft_Date:
                                case SAPbouiCOM.BoFieldsType.ft_Float:
                                case SAPbouiCOM.BoFieldsType.ft_Quantity:
                                case SAPbouiCOM.BoFieldsType.ft_Integer:
                                    dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 10);
                                    //try { dataTable.Columns.Add(attr.U_Attr, _fieldtype, 10); }
                                    //catch { dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 10); }
                                    break;

                                case SAPbouiCOM.BoFieldsType.ft_AlphaNumeric:
                                    dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 254);
                                    //try { dataTable.Columns.Add(attr.U_Attr, _fieldtype, 254); }
                                    //catch { dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype, 254); }

                                    break;

                                case SAPbouiCOM.BoFieldsType.ft_Text:
                                    dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype);
                                    //try { dataTable.Columns.Add(attr.U_Attr, _fieldtype); }
                                    //catch { dataTable.Columns.Add($"{attr.U_Attr} ({attr.U_Unidad})", _fieldtype); }
                                    break;
                            }
                        }
                    }

                    grid.DataTable = dataTable;

                    grid.Columns.Item(0).Editable = false;
                    grid.Columns.Item(0).Width = 30;
                    grid.Columns.Item(1).Editable = true;
                    grid.Columns.Item(1).Width = 100;
                    grid.Columns.Item(2).Editable = false;
                    grid.Columns.Item(2).Width = 70;
                    grid.Columns.Item(3).Editable = false;
                    grid.Columns.Item(3).Width = 70;

                    _top = _top + _height + 5;

                    //for (int i = 4; i < grid.Columns.Count; i++)
                    //{
                    //    if (dataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && dataTable.Columns.Item(i).MaxLength == 254)
                    //    {
                    //        var _editCol = (SAPbouiCOM.EditTextColumn)grid.Columns.Item(i);
                    //        _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                    //        var _cbCol = (SAPbouiCOM.ComboBoxColumn)grid.Columns.Item(i);
                    //        _cbCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description;

                    //        foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                    //        {
                    //            _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                    //        }
                    //    }
                    //    else if (dataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && dataTable.Columns.Item(i).MaxLength == 1)
                    //    {
                    //        var _editCol = (SAPbouiCOM.EditTextColumn)grid.Columns.Item(i);
                    //        _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                    //    }
                    //}

                    grid.AutoResizeColumns();
                }

                item = oFormCalidad.Items.Add("edAccion", SAPbouiCOM.BoFormItemTypes.it_EXTEDIT);
                item.Top = _top + 5;
                item.Height = 80;
                item.Left = _left;
                item.Width = _width - 15;
                item.Enabled = false;
                editText = item.Specific as SAPbouiCOM.EditText;
                editText.Value = $"{attrs[0].U_Accion}";

                _top += 100;
                item = oFormCalidad.Items.Add("1", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = _left;
                item.Enabled = true;

                item = oFormCalidad.Items.Add("2", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = item.Width + 50;
                item.Enabled = false;

                item = oFormCalidad.Items.Add("3", SAPbouiCOM.BoFormItemTypes.it_BUTTON);
                item.Top = _top;
                item.Left = (item.Width) * 2 + 75;
                item.Width = 150;
                item.Enabled = true;

                button = item.Specific as SAPbouiCOM.Button;
                button.Caption = "Asignar a egreso produccion";

                _top += 70;

                oFormCalidad.Left = sbo_application.Desktop.Left;
                oFormCalidad.Width += 10;

                oFormCalidad.Height = _top;
                oFormCalidad.Title = $"{RegistroCalidad.U_PuntoControl} (Analista: {RegistroCalidad.Creator})";

                dynamic Cabecera = new ExpandoObject();

                Cabecera.Tipo = RegistroCalidad.U_BaseType;
                Cabecera.Valor = RegistroCalidad.U_BaseEntry;


                oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDLotes).ValueEx = RegistroCalidad.DFO_RQLTY3Collection.SerializeJson();
                oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDCab).ValueEx = CommonFunctions.SerializeJson(Cabecera);
                oFormCalidad.DataSources.UserDataSources.Item(CommonForms.FormCalidad.UDEntry).ValueEx = DocEntry;

                try
                {
                    foreach (var dt in RegistroCalidad.U_FormXML.DeserializeList<RegCalidadDataTables>())
                    {
                        var dtXML = XDocument.Parse(dt.dtXML);
                        var dtUid = dtXML.Element("DataTable").Attribute("Uid").Value;
                        oFormCalidad.DataSources.DataTables.Item(dtUid).LoadFromXML(dtXML.ToString());

                        var _grid = oFormCalidad.Items.Item(dtUid.Replace("dt", "gr")).Specific as SAPbouiCOM.Grid;
                        _grid.DataTable = oFormCalidad.DataSources.DataTables.Item(dtUid);

                        for (int i = 4; i < _grid.Columns.Count; i++)
                        {
                            if (_grid.DataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && _grid.DataTable.Columns.Item(i).MaxLength == 254)
                            {
                                var _editCol = (SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i);
                                _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_ComboBox;
                                var _cbCol = (SAPbouiCOM.ComboBoxColumn)_grid.Columns.Item(i);
                                _cbCol.DisplayType = SAPbouiCOM.BoComboDisplayType.cdt_Description;

                                var _tipTx = attrs.Where(i => i.U_Attr == _editCol.TitleObject.Caption.RemoveParents() && i.U_TipoFila == "1").Select(i => i.U_TipTxt).FirstOrDefault();
                                if (string.IsNullOrEmpty(_tipTx))
                                {
                                    foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                    {
                                        _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                    }
                                }
                                else
                                {
                                    foreach (var _txt in CommonFunctions.GET(ServiceLayer.MaestroTextosCortos, null, $"?$filter=Name eq '{attrs[1].U_Fruta.ToUpper()}' and U_Tipo eq '{_tipTx}'", sessionId, out _).DeserializeList<MaestroTextosCortos>())
                                    {
                                        _cbCol.ValidValues.Add(_txt.Code.ToString(), _txt.U_Texto);
                                    }
                                }
                                _editCol.Editable = true;
                            }
                            else if (_grid.DataTable.Columns.Item(i).Type == SAPbouiCOM.BoFieldsType.ft_AlphaNumeric && _grid.DataTable.Columns.Item(i).MaxLength == 1)
                            {
                                var _editCol = (SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i);
                                _editCol.Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                                _editCol.Editable = true;
                            }
                            else
                            {
                                ((SAPbouiCOM.EditTextColumn)_grid.Columns.Item(i)).Editable = true;
                            }
                        }
                    }
                }
                catch { }

                oFormCalidad.Visible = true;
            }
            catch
            {
                throw;
            }

            return oFormCalidad;
        }

        public static void AddNewMatrixColumn(ref SAPbouiCOM.Application sbo_application, SAPbouiCOM.ItemEvent oItemEvent)
        {
            try
            {
                var mxtuid = "mx" + oItemEvent.ItemUID.Replace("add", "");
                SAPbouiCOM.Matrix matrix = null;
                SAPbouiCOM.UserDataSource userDataSource = null;
                SAPbouiCOM.Column column = null;

                matrix = sbo_application.Forms.ActiveForm.Items.Item(mxtuid).Specific as SAPbouiCOM.Matrix;

                column = matrix.Columns.Add((matrix.Columns.Count + 1).ToString(), SAPbouiCOM.BoFormItemTypes.it_EDIT);
                column.TitleObject.Caption = "M" + (matrix.Columns.Count - 2).ToString();
                column.Width = 100;
                column.Editable = true;

                userDataSource = sbo_application.Forms.ActiveForm.DataSources.UserDataSources.Add("C" + matrix.Item.UniqueID + column.UniqueID, SAPbouiCOM.BoDataType.dt_SHORT_TEXT, 254);
                column.DataBind.SetBound(true, "", userDataSource.UID);
            }
            catch
            {
                throw;
            }
        }

        public static void AddNewGridColumn(ref SAPbouiCOM.Application sbo_application, SAPbouiCOM.ItemEvent oItemEvent)
        {
            try
            {
                var grduid = "gr" + oItemEvent.ItemUID.Replace("add", "");
                SAPbouiCOM.Grid grid = null;
                SAPbouiCOM.DataTable dataTable = null;

                grid = sbo_application.Forms.ActiveForm.Items.Item(grduid).Specific as SAPbouiCOM.Grid;
                dataTable = grid.DataTable;

                string xmldata = dataTable.SerializeAsXML(SAPbouiCOM.BoDataTableXmlSelect.dxs_All);
                string Uid = "M" + (dataTable.Columns.Count - 1).ToString();

                XElement newColumn = new XElement("Column",
                    new XAttribute("Uid", Uid),
                    new XAttribute("Type", "1"),
                    new XAttribute("MaxLength", "254"));

                XElement newCell = new XElement("Cell",
                    new XElement("ColumnUid", Uid),
                    new XElement("Value", ""));

                XDocument doc = XDocument.Parse(xmldata);
                doc.Root.Element("Columns").Add(newColumn);

                foreach (var item in doc.Descendants("Cells"))
                {
                    item.Add(newCell);
                }

                dataTable.LoadSerializedXML(SAPbouiCOM.BoDataTableXmlSelect.dxs_All, doc.ToString());
            }
            catch
            {
                throw;
            }
        }

        public static object LoadFormLote(ref SAPbouiCOM.Application sbo_application)
        {
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            SAPbouiCOM.Form oFormLote = null;
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormLoteTemp.FormType);
                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);

                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormLoteTemp.FormType + CommonFunctions.Random().ToString();
                oFormLote = sbo_application.Forms.AddEx(FormCreationPackage);

                SAPbouiCOM.ChooseFromList oCFL = oFormLote.ChooseFromLists.Item(CommonForms.FormLoteTemp.CFLEnvases);
                SAPbouiCOM.Conditions oCons = oCFL.GetConditions();

                SAPbouiCOM.Condition oCon = oCons.Add();
                oCon.Alias = "U_Subfamilia";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "BINS";

                oCFL.SetConditions(oCons);

                oCFL = null;

                oCFL = oFormLote.ChooseFromLists.Item(CommonForms.FormLoteTemp.CFLProductor);
                oCons = oCFL.GetConditions();

                oCon = oCons.Add();
                oCon.Alias = "GroupCode";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "103";

                oCFL.SetConditions(oCons);
            }
            catch
            {
                throw;
            }
            finally
            {
                oFormLote.Freeze(false);
                oFormLote.Visible = true;
            }

            return oFormLote;
        }

        public static object LoadFormEnvase(ref SAPbouiCOM.Application sbo_application)
        {
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            SAPbouiCOM.Form oFormEnvase = null;
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormEnvase.FormType);
                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);

                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormEnvase.FormType + CommonFunctions.Random().ToString();
                oFormEnvase = sbo_application.Forms.AddEx(FormCreationPackage);

                SAPbouiCOM.ChooseFromList oCFL = oFormEnvase.ChooseFromLists.Item(CommonForms.FormEnvase.CFLEnvases);
                SAPbouiCOM.Conditions oCons = oCFL.GetConditions();

                SAPbouiCOM.Condition oCon = oCons.Add();
                oCon.Alias = "U_Subfamilia";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "BINS";

                oCFL.SetConditions(oCons);
            }
            catch
            {
                throw;
            }
            finally
            {
                oFormEnvase.Freeze(false);
                oFormEnvase.Visible = true;
            }

            return oFormEnvase;
        }

        public static object LoadFormEnvLote(string Lote, ref SAPbouiCOM.Application sbo_application)
        {
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            SAPbouiCOM.Form oFormEnvLote = null;
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormEnvLote.FormType);
                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);

                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormEnvLote.FormType + CommonFunctions.Random().ToString();
                oFormEnvLote = sbo_application.Forms.AddEx(FormCreationPackage);

                SAPbouiCOM.StaticText StaticLote = (SAPbouiCOM.StaticText)oFormEnvLote.Items.Item(CommonForms.FormEnvLote.StaticLote).Specific;
                StaticLote.Caption = Lote;

                SAPbouiCOM.ChooseFromList oCFL = oFormEnvLote.ChooseFromLists.Item(CommonForms.FormEnvLote.CFLEnvases);
                SAPbouiCOM.Conditions oCons = oCFL.GetConditions();

                SAPbouiCOM.Condition oCon = oCons.Add();
                oCon.Alias = "U_Subfamilia";
                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                oCon.CondVal = "BINS";

                oCFL.SetConditions(oCons);
            }
            catch
            {
                throw;
            }
            finally
            {
                oFormEnvLote.Freeze(false);
                oFormEnvLote.Visible = true;
            }

            return oFormEnvLote;
        }

        public static object LoadFormLotesCalidad(ref SAPbouiCOM.Application sbo_application, string xmlDT)
        {
            SAPbouiCOM.FormCreationParams FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);
            SAPbouiCOM.Form oForm = null;
            try
            {
                string contenidoArchivo = Properties.Resources.ResourceManager.GetString(CommonForms.FormLotesCalidad.FormType);
                System.Xml.XmlDocument xmlFormulario = new System.Xml.XmlDocument();
                xmlFormulario.LoadXml(contenidoArchivo);

                FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                FormCreationPackage.UniqueID = CommonForms.FormLoteTemp.FormType + CommonFunctions.Random().ToString();
                oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                var dataTable = oForm.DataSources.DataTables.Item(CommonForms.FormLotesCalidad.GrdLotes.dt);
                XDocument doc = XDocument.Parse(xmlDT);

                XElement newColumn = new XElement("Column",
                    new XAttribute("Uid", "Asignar"),
                    new XAttribute("Type", "1"),
                    new XAttribute("MaxLength", "1"));

                XElement newCell = new XElement("Cell",
                    new XElement("ColumnUid", "Asignar"),
                    new XElement("Value", "N"));

                doc.Root.Element("Columns").Add(newColumn);

                foreach (var item in doc.Descendants("Cells"))
                {
                    item.Add(newCell);
                }

                newColumn = new XElement("Column",
                    new XAttribute("Uid", "Kilos"),
                    new XAttribute("Type", "7"));

                newCell = new XElement("Cell",
                    new XElement("ColumnUid", "Kilos"),
                    new XElement("Value", "0.00"));

                doc.Root.Element("Columns").Add(newColumn);

                foreach (var item in doc.Descendants("Cells"))
                {
                    item.Add(newCell);
                }

                dataTable.LoadSerializedXML(SAPbouiCOM.BoDataTableXmlSelect.dxs_All, doc.ToString());

                var oGrid = oForm.Items.Item(CommonForms.FormLotesCalidad.GrdLotes.uuid).Specific as SAPbouiCOM.Grid;
                oGrid.Columns.Item(0).Visible = false;
                oGrid.Columns.Item(1).Editable = false;
                oGrid.Columns.Item(2).Editable = false;
                oGrid.Columns.Item(3).Editable = false;
                oGrid.Columns.Item(4).Editable = false;
                oGrid.Columns.Item(5).Editable = false;
                oGrid.Columns.Item(6).Editable = false;
                oGrid.Columns.Item(7).Visible = false;
                oGrid.Columns.Item(8).Type = SAPbouiCOM.BoGridColumnType.gct_CheckBox;
                oGrid.AutoResizeColumns();
            }
            catch
            {
                throw;
            }
            finally
            {
                oForm.Freeze(false);
                oForm.Visible = true;
            }

            return oForm;
        }

        public static void AddRightClickMenu(ref SAPbouiCOM.Application sbo_application, string UniqueID, string Desc, bool Enable, SAPbouiCOM.BoMenuType Type, string MenuItem)
        {
            try
            {
                SAPbouiCOM.MenuItem oMenuItem = sbo_application.Menus.Item(MenuItem);
                SAPbouiCOM.Menus oMenus = oMenuItem.SubMenus;
                SAPbouiCOM.MenuCreationParams oCreationPackage = (SAPbouiCOM.MenuCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_MenuCreationParams);

                oCreationPackage.Type = Type;
                oCreationPackage.UniqueID = UniqueID;
                oCreationPackage.String = Desc;
                oCreationPackage.Enabled = Enable;

                oMenus.AddEx(oCreationPackage);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("66000-68"))
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        public static string ReciboProduccion(string Bodega, string LoteID, string LineNum, string Peso, string FolioInicio, string FolioFin, string DocEntryOF,string Calibre, string Caract, SAPbobsCOM.Company sbo_company, string SessionId)
        {
            var OF = CommonFunctions.DeserializeJsonToDynamic(CommonFunctions.GET(ServiceLayer.ProductionOrders, DocEntryOF, null, SessionId, out _));

            if (OF.AbsoluteEntry != null)
            {
                //var batch = DeserializeJsonObject<BatchNumberDetails>(GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Lote}'", SessionId));
                //var bin = DeserializeJsonObject<DFO_LOTESCALIBRADO>(GET(ServiceLayer.DFO_LOTESCALIBRADO, null, $"?$filter=LOTE eq '{Lote}'", SessionId));

                //Consultar cajas asociadas a OF sin lote asignado

                //SAPbobsCOM.Recordset oRS;
                //oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                //string sSql = "SELECT  count (T0.\"U_CodigoPT\") " +
                //    "as CantPT " +
                //    "FROM \"@DFO_OLOPT\" T0  " +
                //    "WHERE  T0.\"U_LoteID\" = '" + LoteID + "' and T0.\"U_DocEntryOF\" = '" + DocEntryOF + "' ";
                //oRS.DoQuery(sSql);

                double Qty = 0;
                if ((FolioFin != "") && (FolioInicio != ""))
                {
                    Qty = double.Parse(Peso.Replace(".", ",")); //(int.Parse(FolioFin) - int.Parse(FolioInicio)) + 1;
                }
                if ((FolioFin == "") && (FolioInicio == ""))
                {
                    if ((LineNum == "") && (Peso != ""))
                    {
                        Qty = double.Parse(Peso.Replace(".", ","));
                    }
                }

                string WhsCode = Bodega;

                DateTime date = DateTime.Now;
                string fecha = date.ToString("yyyyMMddHHmmssfff");
                var ListDocBatch = new List<BatchNumbers>();
                if (LineNum == "")
                {
                    BatchNumbers DocBatch = new BatchNumbers
                    {
                        BatchNumber = LoteID,
                        U_FRU_FolioCajaIni = FolioInicio,
                        U_FRU_FolioCajaFin = FolioFin,
                        U_FRU_CantBins = 1,
                        U_FRU_CantBinsDis = 1,
                        Quantity = Qty
                        //Status = "2"
                    };
                    ListDocBatch.Add(DocBatch);
                }
                else if (!string.IsNullOrEmpty(LineNum))
                {
                    BatchNumbers DocBatch = new BatchNumbers
                    {
                        BatchNumber = LoteID,
                        U_FRU_FolioCajaIni = FolioInicio,
                        U_FRU_FolioCajaFin = FolioFin,
                        U_FRU_CantBins = 1,
                        U_FRU_CantBinsDis = 1,
                        U_FRU_Calibre = Calibre,
                        U_FRU_Caracteristica = Caract,
                        Quantity = double.Parse(Peso.Replace(".", ",")),
                    };
                    ListDocBatch.Add(DocBatch);
                }

                var ListDocLines = new List<IDocument_Lines>();
                if (LineNum == "")
                {
                    IDocument_Lines DocLines = new IDocument_Lines
                    {
                        BaseEntry = OF.AbsoluteEntry,
                        //BaseLine = 0,
                        BaseType = "202",
                        Quantity = Qty,//double.Parse(oRS.Fields.Item("CantPT").Value.ToString()),
                        BatchNumbers = ListDocBatch,
                        WarehouseCode = WhsCode//,
                                               // ItemCode = OF.ItemNo
                    };
                    ListDocLines.Add(DocLines);
                }
                else if (!string.IsNullOrEmpty(LineNum))
                {
                    IDocument_Lines DocLines = new IDocument_Lines
                    {
                        BaseEntry = OF.AbsoluteEntry,
                        BaseLine = int.Parse(LineNum),
                        BaseType = "202",
                        Quantity = double.Parse(Peso.Replace(".", ",")),
                        BatchNumbers = ListDocBatch,
                        WarehouseCode = WhsCode//,
                                               // ItemCode = OF.ItemNo
                    };
                    ListDocLines.Add(DocLines);
                }

                IDocuments Documents = new IDocuments
                {
                    DocDate = DateTime.Now.ToString("yyyyMMdd"),
                    DocumentLines = ListDocLines,
                };

                var response = CommonFunctions.POST(ServiceLayer.InventoryGenEntries, Documents, SessionId, out System.Net.HttpStatusCode statusCode).DeserializeJsonToDynamic();

                if (response.DocEntry != null)
                {

                }
                else
                {
                    var objresponse = CommonFunctions.DeserializeJsonToDynamic(response);
                    return objresponse.error.message.value.ToString();
                }

                return response.ToString();
            }
            else
            {
                return "OF no encontrada";
            }
        }

        public async static void BatchStatus(string LoteID,string Status, string sessionId)
        {
            try
            {
                var batch = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{LoteID}'", sessionId, out _).DeserializeJsonObject<BatchNumberDetails>();
                if (batch.DocEntry != 0)
                {
                    if (Status == "0")
                    {
                        batch.Status = "dbs_Released";
                    }
                    if (Status == "1")
                    {
                        batch.Status = "dbs_NotAccessible";
                    }
                    if (Status == "2")
                    {
                        batch.Status = "dbs_Locked";
                    }
                    CommonFunctions.PATCH(ServiceLayer.BatchNumberDetails, batch, batch.DocEntry.ToString(), sessionId, out _);
                }
            }
            catch
            {
                { throw; }
            }
        }

        public static void PrintLayout(string LayoutCode, int DocKey, SAPbobsCOM.Company sbo_company)
        {
            try
            {
                SAPbobsCOM.CompanyService oCmpSrv = sbo_company.GetCompanyService();
                SAPbobsCOM.ReportLayoutsService oReportLayoutService = (SAPbobsCOM.ReportLayoutsService)oCmpSrv.GetBusinessService(SAPbobsCOM.ServiceTypes.ReportLayoutsService);
                SAPbobsCOM.ReportLayoutPrintParams oPrintParam = (SAPbobsCOM.ReportLayoutPrintParams)oReportLayoutService.GetDataInterface(SAPbobsCOM.ReportLayoutsServiceDataInterfaces.rlsdiReportLayoutPrintParams);
                oPrintParam.LayoutCode = LayoutCode;
                oPrintParam.DocEntry = DocKey;
                oReportLayoutService.Print(oPrintParam);
            }
            catch { throw; }
        }

        public static void PrintAduana(Embarque objComex, string sessionId, SAPbobsCOM.Company sbo_company)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                var Pedidos = new List<Order>();
                var PedidoEmb = string.Empty;
                var Contratos = string.Empty;
                var DescMercaCli = string.Empty;
                var DescMercaAd = string.Empty;
                var Ordenante = string.Empty;
                var Consig = string.Empty;
                var Kbrutos = 0.00;
                var Knetos = 0.00;
                var Comision = 0.00;
                var CondPago = string.Empty;
                var ClausulaVta = string.Empty;
                var ModoVta = string.Empty;
                var Contenedores = string.Empty;
                var Pais = string.Empty;
                var DirF = string.Empty;
                var DirC = string.Empty;


                if (objComex.U_QtyCont1 > 0 && !string.IsNullOrEmpty(objComex.U_TypCont1))
                {
                    Contenedores += $"{objComex.U_QtyCont1}x{objComex.U_TypCont1} ";
                }

                if (objComex.U_QtyCont2 > 0 && !string.IsNullOrEmpty(objComex.U_TypCont2))
                {
                    Contenedores += $"{objComex.U_QtyCont2}x{objComex.U_TypCont2}";
                }

                foreach (var det in objComex.DFO_EMB1Collection.GroupBy(i => i.U_DocEntry))
                {
                    var ov = CommonFunctions.GET(ServiceLayer.Orders, det.Key.ToString(), null, sessionId, out _).DeserializeJsonObject<Order>();
                    Pedidos.Add(ov);
                    PedidoEmb += $"{ov.NumAtCard};";
                    Contratos += $"{ov.JournalMemo};";

                    if (string.IsNullOrEmpty(Ordenante))
                        Ordenante = ov.PayToCode;

                    if (string.IsNullOrEmpty(DirF))
                        DirF = ov.Address;

                    if (string.IsNullOrEmpty(DirC))
                        DirC = ov.Address2;

                    if (string.IsNullOrEmpty(Consig))
                        Consig = ov.ShipToCode;

                    if (string.IsNullOrEmpty(CondPago))
                        CondPago = CommonFunctions.GET(ServiceLayer.PaymentTermsTypes, ov.PaymentGroupCode.ToString(), "?$select=PaymentTermsGroupName", sessionId, out _).DeserializeJsonToDynamic().PaymentTermsGroupName;

                    if (Comision == 0.00)
                        Comision = (double)ov.U_FRU_PorcentajeComision;

                    if (string.IsNullOrEmpty(ModoVta))
                        ModoVta = GetDescriptionFromValidValue("ORDR", "U_DTE_CodModVenta", ov.U_DTE_CodModVenta, sbo_company);

                    if (string.IsNullOrEmpty(ClausulaVta))
                        ClausulaVta = GetDescriptionFromValidValue("ORDR", "U_DTE_CodClauVenta", ov.U_DTE_CodClauVenta, sbo_company);

                    if (string.IsNullOrEmpty(Pais))
                        Pais = GetDescriptionFromValidValue("ORDR", "U_DTE_CodPaisDestin", ov.U_DTE_CodPaisDestin, sbo_company);

                    foreach (var lin in ov.DocumentLines)
                    {
                        DescMercaCli += $"{lin.Quantity.GetStringFromDouble()} {lin.U_FRU_DescripcionCliente} {lin.Currency}{lin.Price}\r\n";
                        DescMercaAd += $"{lin.Quantity.GetStringFromDouble()} {lin.U_FRU_DescripcionAduana} {lin.Currency}{lin.Price} \r\n";
                        Knetos += (double)lin.Weight1;

                        var PesoEnv = CommonFunctions.GET(ServiceLayer.Items, lin.U_FRU_CajaSaco, "?$select=InventoryWeight", sessionId, out _).DeserializeJsonObject<Items>().InventoryWeight;
                        if (!PesoEnv.HasValue)
                            PesoEnv = 0;

                        Kbrutos += (double)lin.Weight1 + ((double)PesoEnv * lin.Quantity);
                    }
                }

                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "FRUTEXSA";
                excelPackage.Workbook.Properties.Title = "Instrucciones de embarque";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Aduana");

                //Put Logo
                var image = Properties.Resources.Logo;
                var excelImage = worksheet.Drawings.AddPicture("Logo", image);
                excelImage.SetPosition(0, 0, 0, 0);

                //Add some text to cell
                worksheet.Column(1).Width = 39;
                worksheet.Column(2).Width = 106;
                worksheet.Cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                worksheet.Cells["A12"].Value = DateTime.Now.ToLongDateString();
                worksheet.Cells["A14"].Value = "A:"; worksheet.Cells["B14"].Value = objComex.U_CardNameAg;
                worksheet.Cells["A14:B14"].Style.Font.Bold = true;
                worksheet.Cells["A15"].Value = "CC:";
                worksheet.Cells["A15"].Style.Font.Bold = true;
                worksheet.Cells["A16"].Value = "CC:";
                worksheet.Cells["A16"].Style.Font.Bold = true;
                worksheet.Cells["A17"].Value = "DE:"; worksheet.Cells["B17"].Value = "DEPARTAMENTO DE COMEX - FRUTEXSA SpA";
                worksheet.Cells["A17:B17"].Style.Font.Bold = true;

                worksheet.Cells["A19"].Value = $"REF.: INSTRUCCIONES DE EMBARQUE PEDIDO {PedidoEmb}";
                worksheet.Cells["A19"].Style.Font.Bold = true;

                worksheet.Cells["A21"].Value = "ORDENANTE"; worksheet.Cells["B21"].Value = Ordenante;
                worksheet.Cells["A21:B21"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A21:B21"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47));

                worksheet.Cells["A22"].Value = "DIRECCION"; worksheet.Cells["B22"].Value = DirF;
                worksheet.Cells["A22:B22"].Style.WrapText = true;
                worksheet.Cells["A22:B22"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                worksheet.Cells["A23"].Value = "CONTRATO CLIENTE"; worksheet.Cells["B23"].Value = Contratos;
                worksheet.Cells["A24"].Value = "PAIS DESTINO"; worksheet.Cells["B24"].Value = Pais;
                worksheet.Cells["A25"].Value = "PUERTO EMBARQUE"; worksheet.Cells["B25"].Value = $"{objComex.U_PuertoEmb}, CHILE";
                worksheet.Cells["A26"].Value = "NAVE A EMBARCAR"; worksheet.Cells["B26"].Value = objComex.U_Nave;
                worksheet.Cells["A27"].Value = "NAVIERA"; worksheet.Cells["B27"].Value = objComex.U_CardNameNav;
                worksheet.Cells["A28"].Value = "VIAJE"; worksheet.Cells["B28"].Value = objComex.U_Viaje;
                worksheet.Cells["A29"].Value = "RESERVA/BOOKING"; worksheet.Cells["B29"].Value = objComex.U_Reserva;
                worksheet.Cells["A30"].Value = "PUERTO DESTINO"; worksheet.Cells["B30"].Value = $"{objComex.U_Destino}, {Pais}";
                worksheet.Cells["A31"].Value = "CANTIDAD DE CONTENEDORES"; worksheet.Cells["B31"].Value = Contenedores;
                worksheet.Cells["A32"].Value = "DESCRIPCION DE LA MERCADERIA"; worksheet.Cells["B32"].Value = DescMercaCli;
                worksheet.Cells["A33"].Value = "KILOS BRUTOS/KILOS NETOS"; worksheet.Cells["B33"].Value = $"{Kbrutos}/{Knetos}";
                worksheet.Cells["A34"].Value = "COMISION"; worksheet.Cells["B34"].Value = $"{Comision}";
                worksheet.Cells["A35"].Value = "FORMA DE PAGO"; worksheet.Cells["B35"].Value = $"{CondPago.RemoveParents()}";
                worksheet.Cells["A36"].Value = "CLAUSULA DE VENTA"; worksheet.Cells["B36"].Value = $"{ClausulaVta}";
                worksheet.Cells["A37"].Value = "MODO DE VENTA"; worksheet.Cells["B37"].Value = $"{ModoVta}";
                worksheet.Cells["A38"].Value = "CONSIGNAR"; worksheet.Cells["B38"].Value = Consig;
                worksheet.Cells["A38:B38"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A38:B38"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47));

                worksheet.Cells["A39"].Value = "DIRECCION"; worksheet.Cells["B39"].Value = DirC;
                worksheet.Cells["A39:B39"].Style.WrapText = true;
                worksheet.Cells["A39:B39"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                worksheet.Cells["A40"].Value = "NOTIFICAR"; worksheet.Cells["B40"].Value = objComex.U_Notif1;
                worksheet.Cells["A40:B40"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A40:B40"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47));

                worksheet.Cells["A41"].Value = "DIRECCION"; worksheet.Cells["B41"].Value = $"{objComex.U_NotD1} \r\n{objComex.U_NotD2} \r\n{objComex.U_NotD3} \r\n{objComex.U_NotD4} \r\n{objComex.U_NotD5} \r\n{objComex.U_NotD6} \r\n{objComex.U_NotD7}";
                worksheet.Cells["A41:B41"].Style.WrapText = true;
                worksheet.Cells["A41:B41"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;

                var linenum = 42;
                if (!string.IsNullOrEmpty(objComex.U_Notif2))
                {
                    worksheet.Cells[$"A{linenum}"].Value = "NOTIFICAR"; worksheet.Cells[$"B{linenum}"].Value = objComex.U_Notif2;
                    worksheet.Cells[$"A{linenum}:B{linenum}"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                    worksheet.Cells[$"A{linenum}:B{linenum}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47));
                    linenum++;

                    worksheet.Cells[$"A{linenum}"].Value = "DIRECCION"; worksheet.Cells[$"B{linenum}"].Value = $"{objComex.U_NotD1} \r\n{objComex.U_NotD2} \r\n{objComex.U_NotD3} \r\n{objComex.U_NotD4} \r\n{objComex.U_NotD5} \r\n{objComex.U_NotD6} \r\n{objComex.U_NotD7}";
                    worksheet.Cells[$"A{linenum}:B{linenum}"].Style.WrapText = true;
                    worksheet.Cells[$"A{linenum}:B{linenum}"].Style.VerticalAlignment = OfficeOpenXml.Style.ExcelVerticalAlignment.Top;
                    linenum++;
                }

                worksheet.Cells[$"A{linenum}"].Value = "OBSERVACIONES"; worksheet.Cells[$"B{linenum}"].Value = objComex.U_Comments;
                worksheet.Cells[$"A{linenum}"].Value = "MERCADERIA A DESPACHAR";
                linenum++;

                foreach (var line in objComex.DFO_EMB1Collection) { worksheet.Cells[$"B{linenum}"].Value = $"{line.U_ItemName} {line.U_Variedad} {line.U_Tipo} {line.U_Calibre} {line.U_Quantity}"; linenum++; }

                worksheet.Cells[$"A21:B{linenum}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                ////You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                //Save your file
                var pathWithEnv = $"%USERPROFILE%\\Documents\\Aduana-Embarque-{objComex.DocNum}.xlsx";
                var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

                FileInfo fi = new FileInfo(filePath);
                excelPackage.SaveAs(fi);
                try
                {
                    var process = new System.Diagnostics.ProcessStartInfo { FileName = "C:\\Program Files\\Microsoft Office\\root\\Office16\\EXCEL.EXE", Arguments = fi.FullName };
                    System.Diagnostics.Process.Start(process);
                }
                catch
                {
                    try
                    {
                        var process = new System.Diagnostics.ProcessStartInfo { FileName = "C:\\Program Files\\Microsoft Office\\Office14\\EXCEL.EXE", Arguments = fi.FullName };
                        System.Diagnostics.Process.Start(process);
                    }
                    catch { }
                }
            }
        }

        public static void PrintTransporte(Embarque objComex, string sessionId, SAPbobsCOM.Company sbo_company)
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                var Pedidos = new List<Order>();
                var PedidoEmb = string.Empty;
                var Contratos = string.Empty;
                var DescMercaCli = string.Empty;
                var DescMercaAd = string.Empty;
                var Ordenante = string.Empty;
                var Consig = string.Empty;
                var Contenedores = string.Empty;
                var Dir = string.Empty;
                var DiaPlanta = string.Empty;
                var DiaSag = string.Empty;
                var Stacking = new List<string>();

                if (objComex.U_QtyCont1 > 0 && !string.IsNullOrEmpty(objComex.U_TypCont1))
                {
                    Contenedores += $"{objComex.U_QtyCont1}x{objComex.U_TypCont1} ";
                }

                if (objComex.U_QtyCont2 > 0 && !string.IsNullOrEmpty(objComex.U_TypCont2))
                {
                    Contenedores += $"{objComex.U_QtyCont2}x{objComex.U_TypCont2}";
                }

                if (objComex.U_Mon == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_MonDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_MonFrom[0..^3]} A {objComex.U_MonTo[0..^3]} HRS");
                }

                if (objComex.U_Tue == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_TueDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_TueFrom[0..^3]} A {objComex.U_TueTo[0..^3]} HRS");
                }

                if (objComex.U_Wed == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_WedDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_WedFrom[0..^3]} A {objComex.U_WedTo[0..^3]} HRS");
                }

                if (objComex.U_Thu == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_ThuDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_ThuFrom[0..^3]} A {objComex.U_ThuTo[0..^3]} HRS");
                }

                if (objComex.U_Fri == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_FriDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_FriFrom[0..^3]} A {objComex.U_FriTo[0..^3]} HRS");
                }

                if (objComex.U_Sat == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_SatDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_SatFrom[0..^3]} A {objComex.U_SatTo[0..^3]} HRS");
                }

                if (objComex.U_Sun == "Y")
                {
                    Stacking.Add($"{DateTime.ParseExact(objComex.U_SunDt, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} - {objComex.U_SunFrom[0..^3]} A {objComex.U_SunTo[0..^3]} HRS");
                }

                if (objComex.DFO_EMB1Collection[0].U_Planta == "FRU-PAS")
                    Dir = "LA PALMA 400, LOS ANDES.";

                if (objComex.DFO_EMB1Collection[0].U_Planta == "FRU-PRO")
                    Dir = "BAJOS DE MATTE 3345, BUIN.";

                if (string.IsNullOrEmpty(DiaPlanta))
                {
                    try
                    {
                        DiaPlanta = DateTime.ParseExact(objComex.DFO_EMB1Collection[0].U_PlantaDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString();
                    }
                    catch { DiaPlanta = ""; }
                }

                if (string.IsNullOrEmpty(DiaSag))
                {
                    try
                    {
                        DiaSag = DateTime.ParseExact(objComex.DFO_EMB1Collection[0].U_SAGDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString();
                    }
                    catch { DiaSag = "No Aplica"; }
                }

                foreach (var det in objComex.DFO_EMB1Collection)
                {
                    var ov = CommonFunctions.GET(ServiceLayer.Orders, det.U_DocEntry.ToString(), null, sessionId, out _).DeserializeJsonObject<Order>();
                    Pedidos.Add(ov);
                    PedidoEmb += $"{ov.NumAtCard};";
                    Contratos += $"{ov.JournalMemo};";
                    if (string.IsNullOrEmpty(Ordenante))
                        Ordenante = ov.PayToCode;

                    if (string.IsNullOrEmpty(Consig))
                        Consig = ov.ShipToCode;

                    foreach (var lin in ov.DocumentLines)
                    {
                        DescMercaCli += $"{lin.U_FRU_DescripcionCliente} \r\n";
                        DescMercaAd += $"{lin.U_FRU_DescripcionAduana} {lin.Currency}{lin.Price} \r\n";
                    }
                }

                //Set some properties of the Excel document
                excelPackage.Workbook.Properties.Author = "FRUTEXSA";
                excelPackage.Workbook.Properties.Title = "Instrucciones de transporte";
                excelPackage.Workbook.Properties.Created = DateTime.Now;

                //Create the WorkSheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Transporte");

                //Put Logo
                var image = Properties.Resources.Logo;
                var excelImage = worksheet.Drawings.AddPicture("Logo", image);
                excelImage.SetPosition(0, 0, 0, 0);

                //Add some text to cell A1
                worksheet.Column(1).Width = 39;
                worksheet.Column(2).Width = 106;
                worksheet.Cells.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.White);

                worksheet.Cells["A12"].Value = DateTime.Now.ToLongDateString();
                worksheet.Cells["A14"].Value = "A:"; worksheet.Cells["B14"].Value = objComex.U_CardNameTransp;
                worksheet.Cells["A14:B14"].Style.Font.Bold = true;
                worksheet.Cells["A15"].Value = "CC:";
                worksheet.Cells["A15"].Style.Font.Bold = true;
                worksheet.Cells["A16"].Value = "CC:";
                worksheet.Cells["A16"].Style.Font.Bold = true;
                worksheet.Cells["A17"].Value = "DE:"; worksheet.Cells["B17"].Value = "DEPARTAMENTO DE COMEX - FRUTEXSA SpA";
                worksheet.Cells["A17:B17"].Style.Font.Bold = true;

                worksheet.Cells["A19"].Value = $"REF. : PROGRAMACION DE RETIRO, CARGA Y ENTREGA DE CONTENEDORES.";
                worksheet.Cells["A19"].Style.Font.Bold = true;

                worksheet.Cells["A21"].Value = "REFERENCIA CLIENTE"; worksheet.Cells["B21"].Value = $"{PedidoEmb}{objComex.DFO_EMB1Collection[0].U_CardName}";
                //worksheet.Cells["A21:B21"].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                worksheet.Cells["A21:B21"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47));

                worksheet.Cells["A22"].Value = "NAVE"; worksheet.Cells["B22"].Value = objComex.U_Nave;
                worksheet.Cells["A23"].Value = "VIAJE"; worksheet.Cells["B23"].Value = objComex.U_Viaje;
                worksheet.Cells["A24"].Value = "RESERVA/BOOKING"; worksheet.Cells["B24"].Value = objComex.U_Reserva;
                worksheet.Cells["A25"].Value = "NAVIERA"; worksheet.Cells["B25"].Value = objComex.U_CardNameNav;
                worksheet.Cells["A26"].Value = "CANTIDAD DE CONTENEDO"; worksheet.Cells["B26"].Value = Contenedores;
                worksheet.Cells["A27"].Value = "PUERTO EMBARQUE"; worksheet.Cells["B27"].Value = objComex.U_PuertoEmb;
                worksheet.Cells["A28"].Value = "PUERTO DESTINO"; worksheet.Cells["B28"].Value = objComex.U_Destino;
                worksheet.Cells["A29"].Value = "RETIRO DE CONTENEDORES"; worksheet.Cells["B29"].Value = objComex.U_Deposito;
                worksheet.Cells["A30"].Value = "PLANTA EN QUE CARGA"; worksheet.Cells["B30"].Value = Dir;
                worksheet.Cells["A31"].Value = "DIA PLANTA"; worksheet.Cells["B31"].Value = DiaPlanta;
                worksheet.Cells["A32"].Value = "DIA SAG"; worksheet.Cells["B32"].Value = DiaSag;
                worksheet.Cells["A33"].Value = "DESGLOSE DE CONTENEDORES POR HORARIO";
                worksheet.Cells["A33:B33"].Style.HorizontalAlignment = OfficeOpenXml.Style.ExcelHorizontalAlignment.Center;
                worksheet.Cells["A33:B33"].Merge = true;
                var linenum = 34;
                foreach (var cont in objComex.DFO_EMB1Collection.GroupBy(i => new { i.U_DocEntry, i.U_BaseLine }))
                {
                    worksheet.Cells[$"A{linenum}"].Value = $"CONTENEDOR {objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == cont.Key.U_DocEntry && i.U_BaseLine == cont.Key.U_BaseLine).Select(i=>i.U_Pedido).FirstOrDefault()}";
                    worksheet.Cells[$"B{linenum}"].Value = $"PLANTA " +
                        $"{DateTime.ParseExact(objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == cont.Key.U_DocEntry && i.U_BaseLine == cont.Key.U_BaseLine).Select(i=>i.U_PlantaDate).FirstOrDefault(), "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} " +
                        $"{objComex.DFO_EMB1Collection.Where(i => i.U_DocEntry == cont.Key.U_DocEntry && i.U_BaseLine == cont.Key.U_BaseLine).Select(i=>i.U_PlantaHour).FirstOrDefault()[0..^3]} HRS";
                    linenum++;
                }

                worksheet.Cells[$"A{linenum}"].Value = "ENTREGA PUERTO"; worksheet.Cells[$"B{linenum}"].Value = "STACKING OFICIAL";
                worksheet.Cells[$"A{linenum}:B{linenum}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47)); linenum++;
                foreach (var st in Stacking)
                {
                    worksheet.Cells[$"B{linenum}"].Value = st; linenum++;
                }
                worksheet.Cells[$"A{linenum}"].Value = "CORTE DOCUMENTAL"; worksheet.Cells[$"B{linenum}"].Value = $"{DateTime.ParseExact(objComex.U_DocCutDate, "yyyy-MM-dd", CultureInfo.InvariantCulture).ToLongDateString()} {objComex.U_DocCutHour[0..^3]} HRS";
                worksheet.Cells[$"A{linenum}:B{linenum}"].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(173, 255, 47)); linenum++;
                worksheet.Cells[$"A{linenum}"].Value = "AGENCIA DE ADUANAS"; worksheet.Cells[$"B{linenum}"].Value = objComex.U_CardNameAg; linenum++;
                worksheet.Cells[$"A{linenum}"].Value = "MERCADERIA A DESPACHAR"; linenum++;
                foreach (var line in objComex.DFO_EMB1Collection) { worksheet.Cells[$"B{linenum}"].Value = $"{line.U_ItemName} {line.U_Variedad} {line.U_Tipo} {line.U_Calibre} {line.U_Quantity}"; linenum++; }

                worksheet.Cells[$"A21:B{linenum}"].Style.Border.BorderAround(OfficeOpenXml.Style.ExcelBorderStyle.Thin);

                ////You could also use [line, column] notation:
                //worksheet.Cells[1, 2].Value = "This is cell B1!";

                //Save your file
                var pathWithEnv = $"%USERPROFILE%\\Documents\\Transporte-Embarque-{objComex.DocNum}.xlsx";
                var filePath = Environment.ExpandEnvironmentVariables(pathWithEnv);

                FileInfo fi = new FileInfo(filePath);
                excelPackage.SaveAs(fi);
                try
                {
                    var process = new System.Diagnostics.ProcessStartInfo { FileName = "C:\\Program Files\\Microsoft Office\\root\\Office16\\EXCEL.EXE", Arguments = fi.FullName };
                    System.Diagnostics.Process.Start(process);
                }
                catch
                {
                    try
                    {
                        var process = new System.Diagnostics.ProcessStartInfo { FileName = "C:\\Program Files\\Microsoft Office\\Office14\\EXCEL.EXE", Arguments = fi.FullName };
                        System.Diagnostics.Process.Start(process);
                    }
                    catch { }
                }
            }
        }

        public static DataTable SapDataTableToDotNetDataTable(string XmlDT)
        {
            var DT = new DataTable();
            //var XMLstream = new System.IO.FileStream(pathToXmlFile, FileMode.Open);
            var XDoc = System.Xml.Linq.XDocument.Parse(XmlDT);

            var Columns = XDoc.Element("DataTable").Element("Columns").Elements("Column");

            foreach (var Column in Columns)
            {
                DT.Columns.Add(Column.Attribute("Uid").Value);
            }

            var Rows = XDoc.Element("DataTable").Element("Rows").Elements("Row");

            var Names = new List<string>();
            foreach (var Row in Rows)
            {
                var DTRow = DT.NewRow();

                var Cells = Row.Element("Cells").Elements("Cell");
                foreach (var Cell in Cells)
                {
                    var ColName = Cell.Element("ColumnUid").Value;
                    var ColValue = Cell.Element("Value").Value;
                    DTRow[ColName] = ColValue;
                }

                DT.Rows.Add(DTRow);
            }

            return DT;
        }
    }

    public static class CapaNegocios
    {
        public static string LogFile;
        public static string dbFrutexsa;
        public static string dbProcesadora;
        public static string dbPasera;
        public static bool Test;

        public static class SEVT
        {
            public const string SequenceID = "SequenceID";
            public const string SourceDB = "SourceDB";
            public const string Timestamp = "Timestamp";
            public const string Status = "Status";
            public const string Retry = "Retry";
            public const string ObjectType = "ObjectType";
            public const string TransType = "TransType";
            public const string FieldsInKe = "FieldsInKe";
            public const string FieldNames = "FieldNames";
            public const string FieldValue = "FieldValue";
            public const string UserID = "UserID";
            public const string ProcessDate = "ProcessDate";
            public const string ProcessResult = "ProcessResult";
            public const string TargetDB = "TargetDB";
            public const string TargetKey = "TargetKey";
            public const string XML = "XML";
        }

        public static class SAPTransactions
        {
            public const string Add = "A";
            public const string Update = "U";
            public const string Close = "L";
            public const string Delete = "D";
            public const string Cancel = "C";
        }

        public static class SAPObjetos
        {
            public const string PurchaseOrder = "22";
            public const string ProductionOrder = "202";
            public const string PurchaseDeliveryNotes = "20";
            public const string DeliveryNotes = "15";
            public const string SalesOrder = "17";
            public const string IssueForProduction = "60";
            public const string ReceiptForProduction = "59";
            public const string SalesOpportunities = "97";
            public const string StockTransfer = "67";
            public const string ChartOfAccounts = "1";
            public const string BusinessPartners = "2";
            public const string Items = "4";
            public const string UserTablesMD = "153";
            public const string UserFieldsMD = "152";
            public const string UserObjectsMD = "206";
            public const string ProfitCenter = "61";
        }

        public class HanaCon
        {
            private HanaConnection cn;

            public SqlCommand cmd;
            public SqlDataReader dr;
            public string cSql;
            public HanaDataAdapter da;
            public DataTable dt;
            public string ErrorT;
            public int ErrorC;
            public int Registros;

            public HanaCon()
            {
                cn = new HanaConnection("Server=172.24.86.5:30015;UserID=SYSTEM;Password=SAPB1_Admin!!");
                cn.Open();
            }

            public DataTable PopulateDT()
            {
                DataTable Dt = new DataTable();

                ErrorC = 0;
                try
                {
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities
                    da = new HanaDataAdapter(cSql, cn);
#pragma warning restore CA2100 // Review SQL queries for security vulnerabilities
                    da.SelectCommand.CommandTimeout = 10000;
                    da.Fill(Dt);
                    Registros = Dt.Rows.Count;
                }
                catch (Exception ex)
                {
                    ErrorT = ex.ToString();
                    ErrorC = -1;
                    Registros = 0;
                }

                return Dt;
            }
        }

        public class BdConexion : HanaCon
        {
            public string COMPANYDB;
            public string SERVER;
            public string DBUSERNAME;
            public string DBPASSWORD;
            public string USERNAME;
            public string PASSWORD;
            public bool haydatos;

            public BdConexion()
            {
                cSql = "select * from FRUTEXSA_REPO.OADM";
                DataTable oAdm = new DataTable();
            }
        }

        public class InternalQ : HanaCon
        {
            public DataTable SevtDatos;
            public int lRetCode;
            public string ErrMsg;

            public InternalQ(string dbFrut, string dbProc, string dbPas, char isTest = 'N')
            {
                dbFrutexsa = dbFrut;
                dbProcesadora = dbProc;
                dbPasera = dbPas;
                Test = isTest == 'Y';

                string AddOn = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", "");
                string User = Environment.UserName + "." + Environment.UserDomainName;
                string PId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

                string Path = $"C:\\ProgramData\\SAP\\SAP Business One\\Log\\{AddOn}\\{User}";
                System.Threading.Tasks.Task.Run(() => CommonFunctions.DeleteOldLogFiles(Path));

                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                LogFile = $"{Path}\\{AddOn}.{DateTime.Now.ToString("yyyyMMdd_HH.mm.ss")}.pid{PId}.log.csv";

                if (Test)
                {
                    cSql = "select * from FRUTEXSA_REPO.\"SEVT_Test\" where \"Status\"!='OK' order by \"SequenceID\"";
                }
                else
                {
                    cSql = "select * from FRUTEXSA_REPO.SEVT where \"Status\" in ('New', 'Error') order by \"SequenceID\"";
                    //cSql = "select * from FRUTEXSA_REPO.SEVT where \"Status\" in ('New', 'Error') and \"SequenceID\" = 25812 order by \"SequenceID\"";
                }
                SevtDatos = PopulateDT();
                if (Registros > 0)
                {
                    CommonFunctions.LogFile(LogFile, "Procesar SEVT");
                    Procesar_Sevt();
                }
            }

            private SAPbobsCOM.Company connect(string db)

            {
                SAPbobsCOM.Company Company = new SAPbobsCOM.Company();
                Company.Server = "hana:30015";
                Company.LicenseServer = "sapb1:40000";
                Company.UseTrusted = false;
                Company.DbServerType = SAPbobsCOM.BoDataServerTypes.dst_HANADB;
                Company.DbUserName = "SYSTEM";
                Company.DbPassword = "SAPB1_Admin!!";
                Company.CompanyDB = db;
                Company.UserName = "Intercompany";
                Company.Password = "mngr";
                Company.language = SAPbobsCOM.BoSuppLangs.ln_Spanish_La;
                Company.AddonIdentifier = string.Empty;

                if (Company.Connect() != 0)
                {
                    CommonFunctions.LogFile(LogFile, $"Error conectando a: {db}:{Company.GetLastErrorDescription()}");
                    Company = null;
                    //ESCRIBIR LOG
                    //ENVIAR CORREO
                    //KILL APP
                }
                else
                {
                    Company.XMLAsString = true;
                    Company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                }

                return Company;
            }

            private void Actualiza_Sevt(int SequenceId, string update)
            {
                string dt = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss");

                if (Test)
                {
                    cSql = "update FRUTEXSA_REPO.\"SEVT_Test\" " +
                        $"set \"{SEVT.ProcessDate}\"='{dt}', {update} " +
                        $"where \"{SEVT.SequenceID}\"= '{SequenceId}'";
                }
                else
                {
                    cSql = "update FRUTEXSA_REPO.SEVT " +
                        $"set \"{SEVT.ProcessDate}\"='{dt}', {update} " +
                        $"where \"{SEVT.SequenceID}\"= '{SequenceId}'";
                }

                PopulateDT();
            }

            private void Procesar_Sevt()
            {
                SAPbobsCOM.Company Frutexsa;
                SAPbobsCOM.Company Procesadora;
                SAPbobsCOM.Company Pasera;

                try
                {
                    Frutexsa = new SAPbobsCOM.Company();
                    Procesadora = new SAPbobsCOM.Company();
                    Pasera = new SAPbobsCOM.Company();

                    CommonFunctions.LogFile(LogFile, "Connect to FRUTEXSA");
                    if ((Frutexsa = connect(dbFrutexsa)) == null)
                    {
                        return;
                    };

                    CommonFunctions.LogFile(LogFile, "Connect to PROCESADORA");
                    if ((Procesadora = connect(dbProcesadora)) == null)
                    {
                        return;
                    };

                    CommonFunctions.LogFile(LogFile, "Connect to PASERA");
                    if ((Pasera = connect(dbPasera)) == null)
                    {
                        return;
                    };

                    CommonFunctions.LogFile(LogFile, "For each record");
                    foreach (DataRow row in SevtDatos.Rows)
                    {
                        //add_59(96,11251,25942,Procesadora);
                        if (row[SEVT.Status].ToString() == "New" || row[SEVT.Status].ToString() == "Error")
                        {
                            CommonFunctions.LogFile(LogFile, row[SEVT.SequenceID].ToString() + " | " + row[SEVT.ObjectType].ToString() + " | " + row[SEVT.FieldValue].ToString());
#if DEBUG
                            Console.WriteLine(row[SEVT.SequenceID].ToString() + " | " + row[SEVT.ObjectType].ToString() + " | " + row[SEVT.FieldValue].ToString());
#endif
                            var _obj = row[SEVT.ObjectType].ToString();
                            switch (_obj)
                            {
                                case SAPObjetos.SalesOrder:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_17(row);
                                    break;

                                case SAPObjetos.DeliveryNotes:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_15(row);
                                    break;

                                case SAPObjetos.PurchaseOrder:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_22(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Update)
                                        update_22(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Close)
                                        close_22(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Cancel)
                                        cancel_22(row);
                                    break;

                                case SAPObjetos.PurchaseDeliveryNotes:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_20(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Update)
                                        update_20(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Close)
                                        close_20(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Cancel)
                                        cancel_20(row);
                                    break;

                                case SAPObjetos.ProductionOrder:
                                    add_202(row);
                                    break;

                                case SAPObjetos.SalesOpportunities:
                                    add_prj(row);
                                    break;

                                case SAPObjetos.StockTransfer:
                                    add_ic_transfer(row);
                                    break;

                                case SAPObjetos.ChartOfAccounts:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_plc(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Update)
                                        upd_plc(row);
                                    break;

                                case SAPObjetos.BusinessPartners:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_sn(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Update)
                                        upd_sn(row);
                                    break;

                                case SAPObjetos.Items:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_item(row);

                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Update)
                                        upd_item(row);
                                    break;

                                case SAPObjetos.UserTablesMD:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_udt(row);
                                    break;

                                case SAPObjetos.UserFieldsMD:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_udf(row);
                                    break;

                                case SAPObjetos.UserObjectsMD:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_udo(row);
                                    break;

                                case SAPObjetos.ProfitCenter:
                                    if (row[SEVT.TransType].ToString() == SAPTransactions.Add)
                                        add_cc(row);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    CommonFunctions.LogFile(LogFile, $"{e.Message}");
                }

                void add_17(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oOrders) as SAPbobsCOM.Documents;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e => e.Name == "DocEntry" ||
                            e.Name == "BaseType" ||
                            e.Name == "BaseEntry" ||
                            e.Name == "BaseLineNumber" ||
                            e.Name == "HandWritten" ||
                            e.Name == "Price" ||
                            e.Name == "WarehouseCode" ||
                            e.Name == "DocNum" ||
                            e.Name == "COGSAccountCode" ||
                            e.Name == "LineTotal" ||
                            e.Name == "UnitPrice" ||
                            e.Name == "PriceAfterVAT" ||
                            e.Name == "DocTotal" ||
                            e.Name == "TaxTotal" ||
                            e.Name == "LineTaxJurisdictions" ||
                            e.Name == "DocExpenseTaxJurisdictions" ||
                            e.Name == "LineExpenseTaxJurisdictions" ||
                            e.Name == "LineStatus" ||
                            e.Name == "ContactPersonCode" ||
                            e.Name == "ShipToCode" ||
                            e.Name == "PayToCode" ||
                            e.Name == "ShipFrom" ||
                            e.Name == "SupplierCatNum" ||
                            e.Name.LocalName.Contains("CostingCode") ||
                            e.Name.LocalName.Contains("COGSAccountCode") ||
                            e.Name == "BatchNumbers"
                            ).Remove();

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else if (seq[SEVT.TargetDB].ToString() == dbPasera)
                        {
                            Destino = Pasera;
                        }
                        else
                        {
                            return;
                        }

                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Documents;
                        DocDestino.UserFields.Fields.Item("U_IC_DocOrigen").Value = SourceKey.ToString();
                        DocDestino.DiscountPercent = 100;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_15(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oDeliveryNotes) as SAPbobsCOM.Documents;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e =>
                            e.Name != "BOM" &&
                            e.Name != "BO" &&
                            e.Name != "AdmInfo" &&
                            e.Name != "Version" &&
                            e.Name != "Object" &&
                            e.Name != "Documents" &&
                            e.Name != "row" &&
                            e.Name != "DocDate" &&
                            e.Name != "JournalMemo" && e.Name != "FolioPrefixString" && e.Name != "FolioNumber" && e.Name != "Comments" &&
                            e.Name != "Document_Lines" &&
                            e.Name != "LineNum" && e.Name != "Quantity" && e.Name != "ItemCode" &&
                            e.Name != "BatchNumbers" && e.Name != "BatchNumber" && e.Name != "BaseLineNumber" && e.Name != "Quantity" && e.Name != "ExpiryDate" && e.Name != "ManufacturerSerialNumber" && e.Name != "ManufacturingDate" && e.Name != "Notes" &&
                            //e.Name != "AddmisionDate" &&
                            !e.Name.LocalName.Contains("U_FRU_")
                            ).Remove();

                        xml_origen.Element("BOM").Element("BO").Element("AdmInfo").Element("Object").Value = "112";
                        xml_origen.Element("BOM").Element("BO").Element("Documents").Element("row").Add(new XElement("DocObjectCode") { Value = "60" });

                        XDocument xml_new_byLine = XDocument.Parse(xml_origen.ToString());
                        xml_new_byLine.Element("BOM").Element("BO").Element("Document_Lines").Descendants().Remove();
                        xml_new_byLine.Element("BOM").Element("BO").Element("BatchNumbers").Descendants().Remove();

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            if (DocOrigen.Lines.WarehouseCode == "FRU-PRO")
                            {
                                Destino = Procesadora;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OC no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else if (seq[SEVT.TargetDB].ToString() == dbPasera)
                        {
                            if (DocOrigen.Lines.WarehouseCode == "FRU-PAS")
                            {
                                Destino = Pasera;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OC no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }

                        int linenum = 0;
                        string whs = string.Empty;
                        string sql = string.Empty;
                        var rs = Destino.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                        foreach (var Item in xml_origen.Descendants().Where(i => i.Name == "Document_Lines").Elements())
                        {
                            foreach (var lote in xml_origen.Descendants().Where(i => i.Name == "BatchNumbers").Elements())
                            {
                                if (lote.Element("BaseLineNumber").Value == Item.Element("LineNum").Value)
                                {
                                    sql = $"select top 1 B.\"Lote\", A.\"Bodega\" from \"FRU_STOCK_LOTE\" A inner join \"FRU_MAESTRO_LOTE\" B on A.\"AbsEntry\"=B.\"AbsEntry\" where B.\"Lote\"='{lote.Element("BatchNumber").Value}'";
                                    rs.DoQuery(sql);
                                    whs = rs.Fields.Item(1).Value.ToString().Trim();

                                    xml_new_byLine.Element("BOM").Element("BO").Element("Document_Lines").Add(new XElement("row",
                                        new XElement("LineNum") { Value = linenum.ToString() },
                                        new XElement("ItemCode") { Value = Item.Element("ItemCode").Value },
                                        new XElement("Quantity") { Value = lote.Element("Quantity").Value },
                                        new XElement("WarehouseCode") { Value = whs }
                                        )
                                    );

                                    var xElements = new List<XElement>();

                                    foreach (var udf in lote.Elements())
                                    {
                                        if (udf.Name.LocalName.Contains("U_FRU"))
                                        {
                                            xElements.Add(new XElement(udf.Name.LocalName) { Value = udf.Value });
                                        }
                                    }

                                    xml_new_byLine.Element("BOM").Element("BO").Element("BatchNumbers").Add(new XElement("row",
                                        new XElement("BatchNumber") { Value = lote.Element("BatchNumber").Value },
                                        new XElement("Quantity") { Value = lote.Element("Quantity").Value },
                                        new XElement("BaseLineNumber") { Value = linenum.ToString() },
                                            xElements.Select(i => new XElement(i.Name) { Value = i.Value })
                                        )
                                    );

                                    linenum++;
                                }
                            }
                        }

                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_new_byLine.ToString(), 0) as SAPbobsCOM.Documents;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Destino.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_22(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e => e.Name == "DocEntry" ||
                            e.Name == "BaseType" ||
                            e.Name == "BaseEntry" ||
                            e.Name == "BaseLine" ||
                            e.Name == "HandWritten" ||
                            e.Name == "Price" ||
                            e.Name == "WarehouseCode" ||
                            e.Name == "DocNum" ||
                            e.Name == "COGSAccountCode" ||
                            e.Name == "LineTotal" ||
                            e.Name == "UnitPrice" ||
                            e.Name == "PriceAfterVAT" ||
                            e.Name == "DocTotal" ||
                            e.Name == "TaxTotal" ||
                            e.Name == "LineTaxJurisdictions" ||
                            e.Name == "DocExpenseTaxJurisdictions" ||
                            e.Name == "LineExpenseTaxJurisdictions" ||
                            e.Name == "LineStatus" ||
                            e.Name == "AttachmentEntry" ||
                            e.Name == "ContactPersonCode" ||
                            e.Name == "ShipToCode" ||
                            e.Name == "ShipFrom" ||
                            e.Name == "PayToCode").Remove();

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            if (DocOrigen.Lines.WarehouseCode == "FRU-PRO")
                            {
                                Destino = Procesadora;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OC no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else if (seq[SEVT.TargetDB].ToString() == dbPasera)
                        {
                            if (DocOrigen.Lines.WarehouseCode == "FRU-PAS")
                            {
                                Destino = Pasera;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OC no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }

                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Documents;
                        DocDestino.UserFields.Fields.Item("U_IC_DocOrigen").Value = SourceKey.ToString();
                        DocDestino.DiscountPercent = 100;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void update_22(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocOrigen.GetByKey(SourceKey);
                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            if (DocOrigen.Lines.WarehouseCode == "FRU-PRO")
                            {
                                Destino = Procesadora;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OC no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else if (seq[SEVT.TargetDB].ToString() == dbPasera)
                        {
                            if (DocOrigen.Lines.WarehouseCode == "FRU-PAS")
                            {
                                Destino = Pasera;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OC no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else
                        {
                            return;
                        }

                        xml_origen.Descendants().Where(
                            e => e.Name == "DocEntry" ||
                            e.Name == "Price" ||
                            e.Name == "BaseType" ||
                            e.Name == "BaseEntry" ||
                            e.Name == "BaseLine" ||
                            e.Name == "WarehouseCode" ||
                            e.Name == "DocNum" ||
                            e.Name == "COGSAccountCode" ||
                            e.Name == "LineTotal" ||
                            e.Name == "UnitPrice" ||
                            e.Name == "PriceAfterVAT" ||
                            e.Name == "DocTotal" ||
                            e.Name == "VatSum" ||
                            e.Name == "LineStatus" ||
                            e.Name == "ContactPersonCode" ||
                            e.Name == "ShipToCode" ||
                            e.Name == "ShipFrom" ||
                            e.Name == "PayToCode").Remove();

                        var _sql = $"Select \"DocEntry\" \"Key\" from \"{Destino.CompanyDB}\".\"OPOR\" where \"U_IC_DocOrigen\"={SourceKey}";
                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon() { cSql = _sql };
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            TargetKey = int.Parse(Row1["Key"].ToString());
                        }
                        temp.Dispose();

                        XElement queryParams = new XElement("QueryParams", "");
                        queryParams.Value = TargetKey.ToString();
                        xml_origen.Root.Element("BO").Add(queryParams);

                        var DocDestino = Destino.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocDestino.GetByKey(TargetKey);
                        DocDestino.DiscountPercent = 100;

                        if (DocDestino.UpdateFromXML(xml_origen.ToString()) != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{DocDestino.DocEntry.ToString()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void close_22(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());

                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon();
                        oTemp.cSql = $"Select \"DocEntry\" from \"{Destino.CompanyDB}\".\"OPOR\" where \"U_IC_DocOrigen\"={SourceKey}";
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            TargetKey = int.Parse(Row1["DocEntry"].ToString());
                        }
                        temp.Dispose();

                        var DocDestino = Destino.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocDestino.GetByKey(TargetKey);

                        if (DocDestino.Close() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='', \"{SEVT.TargetKey}\"='{DocDestino.DocEntry.ToString()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void cancel_22(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());

                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon();
                        oTemp.cSql = $"Select \"DocEntry\" from \"{Destino.CompanyDB}\".\"OPOR\" where \"U_IC_DocOrigen\"={SourceKey}";
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            TargetKey = int.Parse(Row1["DocEntry"].ToString());
                        }
                        temp.Dispose();

                        var DocDestino = Destino.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocDestino.GetByKey(TargetKey);

                        if (DocDestino.Cancel() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='', \"{SEVT.TargetKey}\"='{DocDestino.DocEntry.ToString()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_20(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Origen = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var DocOrigen = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseDeliveryNotes) as SAPbobsCOM.Documents;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e => e.Name == "DocEntry" ||
                            e.Name == "BaseType" ||
                            e.Name == "BaseEntry" ||
                            e.Name == "BaseLine" ||
                            e.Name == "LineStatus" ||
                            e.Name == "Price" ||
                            e.Name == "WarehouseCode" ||
                            e.Name == "DocNum" ||
                            e.Name == "COGSAccountCode" ||
                            e.Name == "LineTotal" ||
                            e.Name == "UnitPrice" ||
                            e.Name == "PriceAfterVAT" ||
                            e.Name == "DocTotal" ||
                            e.Name == "TaxTotal" ||
                            e.Name == "LineTaxJurisdictions" ||
                            e.Name == "DocExpenseTaxJurisdictions" ||
                            e.Name == "LineExpenseTaxJurisdictions").Remove();

                        if (seq[SEVT.SourceDB].ToString() == dbProcesadora)
                        {
                            if (DocOrigen.Lines.ItemCode.Substring(0,2)!="MP" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "SE" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "PT" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "RA" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "CA")
                            {
                                if (DocOrigen.Lines.WarehouseCode == "FRU-PAS")
                                    Origen = Procesadora;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='EM no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else if (seq[SEVT.SourceDB].ToString() == dbPasera)
                        {
                            if (DocOrigen.Lines.ItemCode.Substring(0, 2) != "MP" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "SE" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "PT" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "RA" || DocOrigen.Lines.ItemCode.Substring(0, 2) != "CA")
                            {
                                if (DocOrigen.Lines.WarehouseCode == "FRU-PAS")
                                    Origen = Pasera;
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='EM no relevante para bd', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                                return;
                            }
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='EM no relevante', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            return;
                        }


                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon();
                        oTemp.cSql = $"select \"U_IC_DocOrigen\" \"Key\" from \"{Origen.CompanyDB}\".OPOR where \"DocEntry\"=(select top 1 \"BaseEntry\" from \"{Origen.CompanyDB}\".PDN1 where \"DocEntry\"={SourceKey})";
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            string Key = Row1["Key"].ToString();
                            if (Key != "")
                            {
                                TargetKey = int.Parse(Row1["Key"].ToString());
                            }
                        }
                        temp.Dispose();

                        foreach (var item in xml_origen.Descendants().Where(i => i.Name == "BaseEntry"))
                        {
                            string Key = TargetKey.ToString();
                            if (string.IsNullOrEmpty(Key))
                            {
                                item.Value = TargetKey.ToString();
                            }
                        }

                        var DocDestino = Frutexsa.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Documents;
                        DocDestino.UserFields.Fields.Item("U_IC_DocOrigen").Value = SourceKey.ToString();
                        DocDestino.DiscountPercent = 100;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Frutexsa.GetLastErrorCode();
                            ErrMsg = Frutexsa.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Frutexsa.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void update_20(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Origen = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.SourceDB].ToString() == dbProcesadora)
                        {
                            Origen = Procesadora;
                        }
                        else
                        {
                            Origen = Pasera;
                        }

                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var DocOrigen = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocOrigen.GetByKey(SourceKey);
                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e => e.Name == "DocEntry" ||
                            e.Name == "BaseType" ||
                            e.Name == "BaseEntry" ||
                            e.Name == "BaseLine" ||
                            e.Name == "Price" ||
                            e.Name == "WarehouseCode" ||
                            e.Name == "DocNum" ||
                            e.Name == "COGSAccountCode" ||
                            e.Name == "LineTotal" ||
                            e.Name == "UnitPrice" ||
                            e.Name == "PriceAfterVAT" ||
                            e.Name == "DocTotal" ||
                            e.Name == "VatSum").Remove();

                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon();
                        oTemp.cSql = $"select \"U_IC_DocOrigen\" \"Key\" from \"{Origen.CompanyDB}\".OPOR where \"DocEntry\"=(select top 1 \"BaseEntry\" from \"{Origen.CompanyDB}\".PDN1 where \"DocEntry\"={SourceKey})";
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            TargetKey = int.Parse(Row1["DocEntry"].ToString());
                        }
                        temp.Dispose();

                        XElement queryParams = new XElement("QueryParams", "");
                        queryParams.Value = TargetKey.ToString();
                        xml_origen.Root.Element("BO").Add(queryParams);

                        var DocDestino = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocDestino.GetByKey(TargetKey);

                        if (DocDestino.UpdateFromXML(xml_origen.ToString()) != 0)
                        {
                            lRetCode = Frutexsa.GetLastErrorCode();
                            ErrMsg = Frutexsa.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{DocDestino.DocEntry.ToString()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void close_20(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Origen = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.SourceDB].ToString() == dbProcesadora)
                        {
                            Origen = Procesadora;
                        }
                        else
                        {
                            Origen = Pasera;
                        }

                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());

                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon();
                        oTemp.cSql = $"select \"U_IC_DocOrigen\" \"Key\" from \"{dbProcesadora}\".OPOR where \"DocEntry\"=(select top 1 \"BaseEntry\" from \"{dbProcesadora}\".PDN1 where \"DocEntry\"={SourceKey})";
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            TargetKey = int.Parse(Row1["DocEntry"].ToString());
                        }
                        temp.Dispose();

                        var DocDestino = Procesadora.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocDestino.GetByKey(TargetKey);

                        if (DocDestino.Close() != 0)
                        {
                            lRetCode = Procesadora.GetLastErrorCode();
                            ErrMsg = Procesadora.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='', \"{SEVT.TargetKey}\"='{DocDestino.DocEntry.ToString()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void cancel_20(DataRow seq)
                {
                    try
                    {
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());

                        var TargetKey = 0;
                        HanaCon oTemp = new HanaCon();
                        oTemp.cSql = $"select \"U_IC_DocOrigen\" \"Key\" from \"{dbProcesadora}\".OPOR where \"DocEntry\"=(select top 1 \"BaseEntry\" from \"{dbProcesadora}\".PDN1 where \"DocEntry\"={SourceKey})";
                        DataTable temp = new DataTable();
                        temp = oTemp.PopulateDT();
                        foreach (DataRow Row1 in temp.Rows)
                        {
                            TargetKey = int.Parse(Row1["DocEntry"].ToString());
                        }
                        temp.Dispose();

                        var DocDestino = Procesadora.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oPurchaseOrders) as SAPbobsCOM.Documents;
                        DocDestino.GetByKey(TargetKey);

                        if (DocDestino.Cancel() != 0)
                        {
                            lRetCode = Procesadora.GetLastErrorCode();
                            ErrMsg = Procesadora.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='', \"{SEVT.TargetKey}\"='{DocDestino.DocEntry.ToString()}',\"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_202(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Origen = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.SourceDB].ToString() == dbProcesadora)
                        {
                            Origen = Procesadora;
                        }
                        else
                        {
                            Origen = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var DocOrigen = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders) as SAPbobsCOM.ProductionOrders;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e => e.Name == "AbsoluteEntry" ||
                            e.Name == "DocumentAbsoluteEntry" ||
                            e.Name == "UserSignature" ||
                            e.Name == "Series" ||
                            e.Name == "Warehouse" ||
                            e.Name == "BaseQuantity" ||
                            //e.Name == "PlannedQuantity" ||
                            e.Name == "AdditionalQuantity" ||
                            e.Name == "ProductionOrderOrigin" ||
                            e.Name == "ProductionOrderOriginEntry" ||
                            e.Name == "ProductionOrderOriginNumber" ||
                            e.Name == "ProductionOrders_Stages" ||
                            e.Name == "ClosingDate" ||
                            e.Name == "ProductionOrderOrigin" ||
                            e.Name == "StageID").Remove();

                        var DocDestino = Frutexsa.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.ProductionOrders;
                        DocDestino.UserFields.Fields.Item("U_IC_DocOrigen").Value = SourceKey.ToString();
                        DocDestino.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposPlanned;
                        //var rowcount = DocDestino.Lines.Count;

                        for (int i = 0; i < DocDestino.Lines.Count; i++)
                        {
                            DocDestino.Lines.SetCurrentLine(i);
                            var it = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
                            it.GetByKey(DocDestino.Lines.ItemNo);
                            if (it.ItemCode.Contains("INS")
                                //&& it.Properties[1] == SAPbobsCOM.BoYesNoEnum.tNO
                                )
                            {
                                DocDestino.Lines.Delete();
                                i--;
                            }
                        }

                        Frutexsa.StartTransaction();
                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Frutexsa.GetLastErrorCode();
                            ErrMsg = Frutexsa.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            var DocEntry = Frutexsa.GetNewObjectKey();
                            DocDestino.GetByKey(int.Parse(DocEntry));
                            DocDestino.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposReleased;

                            if (DocDestino.Update() != 0)
                            {
                                lRetCode = Frutexsa.GetLastErrorCode();
                                ErrMsg = Frutexsa.GetLastErrorDescription().Replace("'", string.Empty);

                                string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);

                                if (Frutexsa.InTransaction)
                                    Frutexsa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                                return;
                            }
                            else
                            {
                                add_59(SourceKey, DocDestino.AbsoluteEntry, SequenceId, Origen);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void close_202(int TargetKey, int SequenceId)
                {
                    try
                    {
                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        var DocDestino = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oProductionOrders) as SAPbobsCOM.ProductionOrders;
                        DocDestino.GetByKey(TargetKey);

                        DocDestino.ProductionOrderStatus = SAPbobsCOM.BoProductionOrderStatusEnum.boposClosed;
                        if (DocDestino.Update() != 0)
                        {
                            lRetCode = Procesadora.GetLastErrorCode();
                            ErrMsg = Procesadora.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Manual', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"Retry\"=(\"Retry\"+1)";

                            //AsignarLotesPedido(int DocEntry);

                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_59(int SourceKey, int TargetKey, int SequenceId, SAPbobsCOM.Company Origen)
                {
                    try
                    {
                        var rs = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                        rs.DoQuery($"select distinct a.\"DocEntry\" from IGN1 a inner join OITM b on a.\"ItemCode\"=b.\"ItemCode\" where a.\"BaseType\"='202' and a.\"BaseEntry\"={SourceKey} and b.\"ItemCode\" not like 'INS%' order by 1");

                        if (rs.EoF)
                        {
                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='-999 - Consumos no encontrados', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(SequenceId, updSql);
                            rs = null;
                            GC.Collect();
                        }
                        else
                        {
                            while (!rs.EoF)
                            {
                                var DocOrigen = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenEntry) as SAPbobsCOM.Documents;
                                DocOrigen.GetByKey(int.Parse(rs.GetColumnValue(0).ToString()));

                                    XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                                    XDocument xml_origen_tmp = XDocument.Parse(DocOrigen.GetAsXML());
                                    xml_origen.Descendants().Where(
                                        e =>
                                        e.Name != "BOM" &&
                                        e.Name != "BO" &&
                                        e.Name != "AdmInfo" &&
                                        e.Name != "Version" &&
                                        e.Name != "Object" &&
                                        e.Name != "Documents" &&
                                        e.Name != "row" &&
                                        e.Name != "DocDate" &&
                                        e.Name != "JournalMemo" &&
                                        e.Name != "Document_Lines" &&
                                        e.Name != "LineNum" && e.Name != "Quantity" && e.Name != "BaseType" && e.Name != "BaseLine" && e.Name != "BaseEntry" &&
                                        e.Name != "BatchNumbers" && e.Name != "BatchNumber" && e.Name != "BaseLineNumber" && !e.Name.LocalName.Contains("U_FRU_")
                                        ).Remove();

                                foreach (var item in xml_origen.Descendants().Where(i => i.Name == "BaseEntry"))
                                {
                                    item.Value = TargetKey.ToString();
                                }

                                if (DocOrigen.Lines.BaseLine != 0)
                                {
                                    var sql = $"select \"LineNum\" from WOR1 where \"BaseQty\"<0 and \"DocEntry\"={TargetKey} and \"ItemCode\"='{xml_origen_tmp.Descendants().Where(i => i.Name == "ItemCode").Select(i => i.Value).FirstOrDefault()}'";
                                    var rs2 = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                                    rs2.DoQuery(sql);

                                    if (!rs2.EoF)
                                    {
                                        foreach (var item in xml_origen.Descendants().Where(i => i.Name == "BaseLine"))
                                        {
                                            item.Value = rs2.GetColumnValue(0).ToString();
                                        }
                                    }
                                }

                                    var DocDestino = Frutexsa.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Documents;
                                    DocDestino.UserFields.Fields.Item("U_IC_DocOrigen").Value = DocOrigen.DocEntry.ToString();

                                if (DocDestino.Add() != 0)
                                {
                                    lRetCode = Frutexsa.GetLastErrorCode();
                                    ErrMsg = Frutexsa.GetLastErrorDescription().Replace("'", string.Empty);

                                    string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                    Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);

                                    if (Frutexsa.InTransaction)
                                        Frutexsa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                                    return;
                                }
                                rs.MoveNext();
                            }
                            rs = null;
                            GC.Collect();
                            add_60(SourceKey, TargetKey, SequenceId, Origen);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_60(int SourceKey, int TargetKey, int SequenceId, SAPbobsCOM.Company Origen)
                {
                    try
                    {
                        var rs = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                        rs.DoQuery($"select distinct \"DocEntry\" from IGE1 where \"BaseType\"='202' and \"BaseEntry\"={SourceKey} and \"DocEntry\" not in (select \"U_IC_DocOrigen\" from FRUTEXSA.OIGE) and \"ItemCode\" not like 'INS%' order by 1");

                        if (rs.EoF)
                        {
                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='-999 - Egresos no encontrados', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(SequenceId, updSql);
                            rs = null;
                            GC.Collect();
                        }
                        else
                        {
                            while (!rs.EoF)
                            {
                                var DocOrigen = Origen.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oInventoryGenExit) as SAPbobsCOM.Documents;
                                DocOrigen.GetByKey(int.Parse(rs.GetColumnValue(0).ToString()));

                                XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                                XDocument xml_origen_tmp = XDocument.Parse(DocOrigen.GetAsXML());
                                xml_origen.Descendants().Where(
                                    e =>
                                    e.Name != "BOM" &&
                                    e.Name != "BO" &&
                                    e.Name != "AdmInfo" &&
                                    e.Name != "Version" &&
                                    e.Name != "Object" &&
                                    e.Name != "Documents" &&
                                    e.Name != "row" &&
                                    e.Name != "DocDate" &&
                                    e.Name != "JournalMemo" &&
                                    e.Name != "Document_Lines" &&
                                    e.Name != "LineNum" && e.Name != "Quantity" && e.Name != "BaseType" && e.Name != "BaseLine" && e.Name != "BaseEntry" &&
                                    e.Name != "BatchNumbers" && e.Name != "BatchNumber" && e.Name != "BaseLineNumber" && e.Name != "Quantity" && e.Name != "ExpiryDate" && e.Name != "ManufacturerSerialNumber" && e.Name != "ManufacturingDate" && e.Name != "Notes" && e.Name != "AddmisionDate" && !e.Name.LocalName.Contains("U_FRU_")
                                    ).Remove();

                                foreach (var item in xml_origen.Descendants().Where(i => i.Name == "BaseEntry"))
                                {
                                    item.Value = TargetKey.ToString();
                                }

                                if (DocOrigen.Lines.BaseLine != 0)
                                {
                                    var sql = $"select \"LineNum\" from WOR1 where \"BaseQty\">0 and \"DocEntry\"={TargetKey} and \"ItemCode\"='{xml_origen_tmp.Descendants().Where(i => i.Name == "ItemCode").Select(i => i.Value).FirstOrDefault()}'";
                                    var rs2 = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                                    rs2.DoQuery(sql);

                                    foreach (var item in xml_origen.Descendants().Where(i => i.Name == "BaseLine"))
                                    {
                                        item.Value = rs2.GetColumnValue(0).ToString();
                                    }
                                }

                                var DocDestino = Frutexsa.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Documents;
                                DocDestino.UserFields.Fields.Item("U_IC_DocOrigen").Value = DocOrigen.DocEntry.ToString();

                                if (DocDestino.Add() != 0)
                                {
                                    lRetCode = Frutexsa.GetLastErrorCode();
                                    ErrMsg = Frutexsa.GetLastErrorDescription().Replace("'", string.Empty);

                                    string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                                    Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);

                                    if (Frutexsa.InTransaction)
                                        Frutexsa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                                    return;
                                }

                                rs.MoveNext();
                            }
                            rs = null;
                            GC.Collect();
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"Retry\"=(\"Retry\"+1)");
                            if (Frutexsa.InTransaction)
                            {
                                Frutexsa.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);
                                close_202(TargetKey, SequenceId);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_prj(DataRow seq)
                {
                    try
                    {
                        int result1 = 0;
                        int result2 = 0;
                        int result3 = 0;

                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var CompanyService = Frutexsa.GetCompanyService();
                        var prjSrv = CompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ProjectsService) as SAPbobsCOM.IProjectsService;
                        var prj = prjSrv.GetDataInterface(SAPbobsCOM.ProjectsServiceDataInterfaces.psProject) as SAPbobsCOM.Project;

                        var rs = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                        rs.DoQuery($"SELECT case T0.\"OpprType\" when 'P' then 'PC' when 'R' then 'PV' end OpprType, T0.\"OpprId\", T0.\"Name\", T1.\"CardName\", T1.\"LicTradNum\" FROM OOPR T0  INNER JOIN OCRD T1 ON T0.\"CardCode\" = T1.\"CardCode\" WHERE T0.\"OpprId\" = {seq[SEVT.FieldValue]} ");

                        if (rs.EoF)
                        {
                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='-999 - Oportunidad no encontrada', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(seq[SEVT.SequenceID].ToString()), updSql);
                            return;
                        }

                        try
                        {
                            prj.Code = $"{rs.GetColumnValue(2)}";
                            prj.Name = $"{rs.GetColumnValue(0)}-{rs.GetColumnValue(1)}";
                            prj.UserFields.Item("U_FRU_Nombre").Value = $"{rs.GetColumnValue(3)}";
                            prj.UserFields.Item("U_FRU_Rut").Value = $"{rs.GetColumnValue(4)}";

                            prjSrv.AddProject(prj);

                            var oopr = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oSalesOpportunities) as SAPbobsCOM.SalesOpportunities;
                            oopr.GetByKey(int.Parse(seq[SEVT.FieldValue].ToString()));

                            oopr.ProjectCode = prj.Code;
                            oopr.Update();
                        }
                        catch (Exception e)
                        {
                            result1 = 99;
                            ErrMsg += $"{Frutexsa.CompanyDB}:{e.Message}";
                        }

                        CompanyService = Procesadora.GetCompanyService();
                        prjSrv = CompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ProjectsService) as SAPbobsCOM.IProjectsService;
                        prj = prjSrv.GetDataInterface(SAPbobsCOM.ProjectsServiceDataInterfaces.psProject) as SAPbobsCOM.Project;

                        try
                        {
                            prj.Code = $"{rs.GetColumnValue(2)}";
                            prj.Name = $"{rs.GetColumnValue(0)}-{rs.GetColumnValue(1)}";
                            prj.UserFields.Item("U_FRU_Nombre").Value = $"{rs.GetColumnValue(3)}";
                            prj.UserFields.Item("U_FRU_Rut").Value = $"{rs.GetColumnValue(4)}";

                            prjSrv.AddProject(prj);
                        }
                        catch (Exception e)
                        {
                            result2 = 99;
                            ErrMsg += $"|{Procesadora.CompanyDB}:{e.Message}";
                        }

                        CompanyService = Pasera.GetCompanyService();
                        prjSrv = CompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ProjectsService) as SAPbobsCOM.IProjectsService;
                        prj = prjSrv.GetDataInterface(SAPbobsCOM.ProjectsServiceDataInterfaces.psProject) as SAPbobsCOM.Project;

                        try
                        {
                            prj.Code = $"{rs.GetColumnValue(2)}";
                            prj.Name = $"{rs.GetColumnValue(0)}-{rs.GetColumnValue(1)}";
                            prj.UserFields.Item("U_FRU_Nombre").Value = $"{rs.GetColumnValue(3)}";
                            prj.UserFields.Item("U_FRU_Rut").Value = $"{rs.GetColumnValue(4)}";

                            prjSrv.AddProject(prj);
                        }
                        catch (Exception e)
                        {
                            result3 = 99;
                            ErrMsg += $"|{Pasera.CompanyDB}:{e.Message}";
                        }

                        if (result1 == 0 && result2 == 0 && result3 == 0)
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(seq[SEVT.SequenceID].ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{ErrMsg}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(seq[SEVT.SequenceID].ToString()), updSql);
                        }

                        rs = null;
                        GC.Collect();
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_ic_transfer(DataRow seq)
                {
                    try
                    {
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = int.Parse(seq[SEVT.FieldValue].ToString());
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oStockTransfer) as SAPbobsCOM.StockTransfer;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                            e =>
                            e.Name != "BOM" &&
                            e.Name != "BO" &&
                            e.Name != "AdmInfo" &&
                            e.Name != "Version" &&
                            e.Name != "Object" &&
                            e.Name != "StockTransfer" &&
                            e.Name != "row" &&
                            e.Name != "DocDate" &&
                            e.Name != "JournalMemo" && e.Name != "FolioPrefixString" && e.Name != "FolioNumber" && e.Name != "Comments" &&
                            e.Name != "StockTransfer_Lines" &&
                            e.Name != "LineNum" && e.Name != "Quantity" && e.Name != "ItemCode" &&
                            e.Name != "BatchNumbers" && e.Name != "BatchNumber" && e.Name != "BaseLineNumber" && e.Name != "Quantity" && e.Name != "ExpiryDate" && e.Name != "ManufacturerSerialNumber" && e.Name != "ManufacturingDate" && e.Name != "Notes" &&
                            //e.Name != "AddmisionDate" &&
                            !e.Name.LocalName.Contains("U_FRU_")
                            ).Remove();

                        SAPbobsCOM.Documents DocPasera;
                        SAPbobsCOM.Documents DocProcesadora;
                        XDocument xml_pasera = XDocument.Parse(xml_origen.ToString());
                        XDocument xml_proc = XDocument.Parse(xml_origen.ToString());
                        string updSql;

                        if (DocOrigen.FromWarehouse == "FRU-PAS" && DocOrigen.ToWarehouse == "FRU-PRO")
                        {
                            xml_pasera.Element("BOM").Element("BO").Element("AdmInfo").Element("Object").Value = "112";
                            xml_pasera.Element("BOM").Element("BO").Element("StockTransfer").Name = "Documents";
                            xml_pasera.Element("BOM").Element("BO").Element("Documents").Element("row").Add(new XElement("DocObjectCode") { Value = "60" });
                            xml_pasera.Element("BOM").Element("BO").Element("StockTransfer_Lines").Name = "Document_Lines";
                            xml_pasera.Element("BOM").Element("BO").Descendants().Where(e => e.Name.LocalName.Contains("U_FRU")).Remove();

                            xml_proc.Element("BOM").Element("BO").Element("AdmInfo").Element("Object").Value = "112";
                            xml_proc.Element("BOM").Element("BO").Element("StockTransfer").Name = "Documents";
                            xml_proc.Element("BOM").Element("BO").Element("Documents").Element("row").Add(new XElement("DocObjectCode") { Value = "59" });
                            xml_proc.Element("BOM").Element("BO").Element("StockTransfer_Lines").Name = "Document_Lines";
                        }
                        else if (DocOrigen.FromWarehouse == "FRU-PRO" && DocOrigen.ToWarehouse == "FRU-PAS")
                        {
                            xml_proc.Element("BOM").Element("BO").Element("AdmInfo").Element("Object").Value = "112";
                            xml_proc.Element("BOM").Element("BO").Element("StockTransfer").Name = "Documents";
                            xml_proc.Element("BOM").Element("BO").Element("Documents").Element("row").Add(new XElement("DocObjectCode") { Value = "60" });
                            xml_proc.Element("BOM").Element("BO").Element("StockTransfer_Lines").Name = "Document_Lines";
                            xml_proc.Element("BOM").Element("BO").Descendants().Where(e => e.Name.LocalName.Contains("U_FRU")).Remove();

                            xml_pasera.Element("BOM").Element("BO").Element("AdmInfo").Element("Object").Value = "112";
                            xml_pasera.Element("BOM").Element("BO").Element("StockTransfer").Name = "Documents";
                            xml_pasera.Element("BOM").Element("BO").Element("Documents").Element("row").Add(new XElement("DocObjectCode") { Value = "59" });
                            xml_pasera.Element("BOM").Element("BO").Element("StockTransfer_Lines").Name = "Document_Lines";
                        }
                        else
                        {
                            updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='TR no relevante para ic', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            return;
                        }

                        XDocument xml_pasera_byLine = XDocument.Parse(xml_pasera.ToString());
                        xml_pasera_byLine.Element("BOM").Element("BO").Element("Document_Lines").Descendants().Remove();
                        xml_pasera_byLine.Element("BOM").Element("BO").Element("BatchNumbers").Descendants().Remove();

                        int linenum = 0;
                        string whs = string.Empty;
                        string sql = string.Empty;
                        var pasera_rs = Pasera.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                        var proc_rs = Procesadora.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                        foreach (var Item in xml_pasera.Descendants().Where(i => i.Name == "Document_Lines").Elements())
                        {
                            foreach (var lote in xml_pasera.Descendants().Where(i => i.Name == "BatchNumbers").Elements())
                            {
                                if (lote.Element("BaseLineNumber").Value == Item.Element("LineNum").Value)
                                {
                                    if (xml_pasera.Element("BOM").Element("BO").Element("Documents").Element("row").Element("DocObjectCode").Value == "59")
                                    {
                                        whs = "IC";
                                    }
                                    else
                                    {
                                        sql = $"select top 1 B.\"Lote\", A.\"Bodega\" from \"FRU_STOCK_LOTE\" A inner join \"FRU_MAESTRO_LOTE\" B on A.\"AbsEntry\"=B.\"AbsEntry\" where B.\"Lote\"='{lote.Element("BatchNumber").Value}'";
                                        pasera_rs.DoQuery(sql);
                                        whs = pasera_rs.Fields.Item(1).Value.ToString().Trim();
                                    }

                                    xml_pasera_byLine.Element("BOM").Element("BO").Element("Document_Lines").Add(new XElement("row",
                                        new XElement("LineNum") { Value = linenum.ToString() },
                                        new XElement("ItemCode") { Value = Item.Element("ItemCode").Value },
                                        new XElement("Quantity") { Value = lote.Element("Quantity").Value },
                                        new XElement("WarehouseCode") { Value = whs }
                                        )
                                    );

                                    var xElements = new List<XElement>();
                                    if (xml_pasera.Element("BOM").Element("BO").Element("Documents").Element("row").Element("DocObjectCode").Value == "59")
                                    {
                                        foreach (var udf in lote.Elements())
                                        {
                                            if (udf.Name.LocalName.Contains("U_FRU"))
                                            {
                                                xElements.Add(new XElement(udf.Name.LocalName) { Value = udf.Value });
                                            }
                                        }
                                    }

                                    xml_pasera_byLine.Element("BOM").Element("BO").Element("BatchNumbers").Add(new XElement("row",
                                        new XElement("BatchNumber") { Value = lote.Element("BatchNumber").Value },
                                        new XElement("Quantity") { Value = lote.Element("Quantity").Value },
                                        new XElement("BaseLineNumber") { Value = linenum.ToString() },
                                            xElements.Select(i => new XElement(i.Name) { Value = i.Value })
                                        )
                                    );

                                    linenum++;
                                }
                            }
                        }

                        XDocument xml_proc_byLine = XDocument.Parse(xml_proc.ToString());
                        xml_proc_byLine.Element("BOM").Element("BO").Element("Document_Lines").Descendants().Remove();
                        xml_proc_byLine.Element("BOM").Element("BO").Element("BatchNumbers").Descendants().Remove();

                        linenum = 0;
                        foreach (var Item in xml_proc.Descendants().Where(i => i.Name == "Document_Lines").Elements())
                        {
                            foreach (var lote in xml_proc.Descendants().Where(i => i.Name == "BatchNumbers").Elements())
                            {
                                if (xml_proc.Element("BOM").Element("BO").Element("Documents").Element("row").Element("DocObjectCode").Value == "59")
                                {
                                    whs = "IC";
                                }
                                else
                                {
                                    sql = $"select top 1 B.\"Lote\", A.\"Bodega\" from \"FRU_STOCK_LOTE\" A inner join \"FRU_MAESTRO_LOTE\" B on A.\"AbsEntry\"=B.\"AbsEntry\" where B.\"Lote\"='{lote.Element("BatchNumber").Value}'";
                                    proc_rs.DoQuery(sql);
                                    whs = proc_rs.Fields.Item(1).Value.ToString().Trim();
                                }

                                if (lote.Element("BaseLineNumber").Value == Item.Element("LineNum").Value)
                                {
                                    xml_proc_byLine.Element("BOM").Element("BO").Element("Document_Lines").Add(new XElement("row",
                                        new XElement("LineNum") { Value = linenum.ToString() },
                                        new XElement("ItemCode") { Value = Item.Element("ItemCode").Value },
                                        new XElement("Quantity") { Value = lote.Element("Quantity").Value },
                                        new XElement("WarehouseCode") { Value = whs }
                                        )
                                    );

                                    var xElements = new List<XElement>();
                                    if (xml_proc.Element("BOM").Element("BO").Element("Documents").Element("row").Element("DocObjectCode").Value == "59")
                                    {
                                        foreach (var udf in lote.Elements())
                                        {
                                            if (udf.Name.LocalName.Contains("U_FRU"))
                                            {
                                                xElements.Add(new XElement(udf.Name.LocalName) { Value = udf.Value });
                                            }
                                        }
                                    }

                                    xml_proc_byLine.Element("BOM").Element("BO").Element("BatchNumbers").Add(new XElement("row",
                                        new XElement("BatchNumber") { Value = lote.Element("BatchNumber").Value },
                                        new XElement("Quantity") { Value = lote.Element("Quantity").Value },
                                        new XElement("BaseLineNumber") { Value = linenum.ToString() },
                                            xElements.Select(i => new XElement(i.Name) { Value = i.Value })
                                        )
                                    );

                                    linenum++;
                                }
                            }
                        }

                        DocPasera = Pasera.GetBusinessObjectFromXML(xml_pasera_byLine.ToString(), 0) as SAPbobsCOM.Documents;
                        DocProcesadora = Procesadora.GetBusinessObjectFromXML(xml_proc_byLine.ToString(), 0) as SAPbobsCOM.Documents;

                        Pasera.StartTransaction();
                        lRetCode = DocPasera.Add();
                        if (lRetCode != 0)
                        {
                            lRetCode = Pasera.GetLastErrorCode();
                            ErrMsg = Pasera.GetLastErrorDescription().Replace("'", string.Empty);

                            updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocPasera.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);

                            if (Pasera.InTransaction)
                                Pasera.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                            return;
                        }

                        Procesadora.StartTransaction();
                        lRetCode = DocProcesadora.Add();
                        if (lRetCode != 0)
                        {
                            lRetCode = Procesadora.GetLastErrorCode();
                            ErrMsg = Procesadora.GetLastErrorDescription().Replace("'", string.Empty);

                            updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocProcesadora.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            if (Pasera.InTransaction)
                                Pasera.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                            if (Procesadora.InTransaction)
                                Procesadora.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_RollBack);

                            return;
                        }

                        if (Pasera.InTransaction)
                            Pasera.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                        if (Procesadora.InTransaction)
                            Procesadora.EndTransaction(SAPbobsCOM.BoWfTransOpt.wf_Commit);

                        updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"Retry\"=(\"Retry\"+1)";
                        Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_plc(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oChartOfAccounts) as SAPbobsCOM.ChartOfAccounts;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.ChartOfAccounts;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void upd_plc(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oChartOfAccounts) as SAPbobsCOM.ChartOfAccounts;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.ChartOfAccounts;

                        if (DocDestino.Update() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_sn(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                        
                        if (DocOrigen.GetByKey(SourceKey))
                        {
                            XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                            xml_origen.Descendants().Where(i => i.Name == "Series"
                            || i.Name == "InternalKey"
                            || i.Name == "PeymentMethodCode"
                            || i.Name == "SalesPersonCode"
                            || i.Name == "BPPaymentMethods"
                            || i.Name.LocalName.Contains("Payment")).Remove();

                            var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.BusinessPartners;

                            if (DocDestino.Add() != 0)
                            {
                                lRetCode = Destino.GetLastErrorCode();
                                ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                                string updSql = "";
                                if (lRetCode == -10)
                                {
                                    updSql = $"\"{SEVT.Status}\"='Existe', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                                }
                                else
                                {
                                    updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                                }

                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            }
                        }
                        else
                        {
                            ErrMsg = $"El código de socio de negocios {SourceKey} no existe en el origen";

                            string updSql = $"\"{SEVT.Status}\"='Eliminado', \"{SEVT.ProcessResult}\"='{ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void upd_sn(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(i =>
                        i.Name == "SalesPersonCode" ||
                        i.Name == "PeymentMethodCode" ||
                        i.Name == "Series" ||
                        i.Name == "InternalKey" ||
                        i.Name == "LogInstance" ||
                        i.Name == "ContactPerson" ||
                        i.Name == "ContactEmployees" ||
                        i.Name == "BPAddresses" ||
                        i.Name == "BPPaymentMethods" ||
                        i.Name.LocalName.Contains("State") ||
                        i.Name.LocalName.Contains("Payment")).Remove();

                        //var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.BusinessPartners;
                        var DocDestino = Destino.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oBusinessPartners) as SAPbobsCOM.BusinessPartners;
                        DocDestino.GetByKey(SourceKey);

                        DocDestino.Browser.ReadXml(xml_origen.ToString(), 0);

                        //foreach (var prop in DocDestino.Addresses.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(i=>i.Name.Contains("State")))
                        //{
                        //    prop.SetValue(DocDestino.Addresses, "", null);
                        //}

                        for (int i = 0; i < DocDestino.Addresses.Count; i++)
                        {
                            DocDestino.Addresses.SetCurrentLine(i);
                            DocDestino.Addresses.State = "";
                        }

                        if (DocDestino.Update() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_item(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;

                        if (DocOrigen.GetByKey(SourceKey))
                        {
                            XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                            xml_origen.Descendants().Where(
                            e => e.Name == "Items_Prices" ||
                            e.Name == "DefaultWarehouse" ||
                            e.Name == "ItemWarehouseInfo" ||
                            e.Name == "AttachmentEntry").Remove();

                            var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Items;

                            if (DocDestino.Add() != 0)
                            {
                                lRetCode = Destino.GetLastErrorCode();
                                ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                                string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            }
                            else
                            {
                                string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                                Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                            }
                        }
                        else
                        {
                            ErrMsg = $"El código de artículo {SourceKey} no existe en el origen";

                            string updSql = $"\"{SEVT.Status}\"='Eliminado', \"{SEVT.ProcessResult}\"='{ErrMsg}', \"{SEVT.XML}\"='', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void upd_item(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        xml_origen.Descendants().Where(
                        e => e.Name == "Items_Prices" ||
                        e.Name == "DefaultWarehouse" ||
                        e.Name == "ItemWarehouseInfo" ||
                        e.Name == "AvgStdPrice" ||
                        e.Name == "AttachmentEntry").Remove();

                        //var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.Items;
                        var DocDestino = Destino.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oItems) as SAPbobsCOM.Items;
                        DocDestino.GetByKey(SourceKey);
                        DocDestino.Browser.ReadXml(xml_origen.ToString(), 0);

                        if (DocDestino.Update() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_udt(DataRow seq)
                {
                    try
                    {
                        GC.Collect();
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables) as SAPbobsCOM.UserTablesMD;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.UserTablesMD;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_udf(DataRow seq)
                {
                    try
                    {
                        GC.Collect();
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString().Split('\t');
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields) as SAPbobsCOM.UserFieldsMD;

                        DocOrigen.GetByKey(SourceKey[0], int.Parse(SourceKey[1]));

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.UserFieldsMD;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_udo(DataRow seq)
                {
                    try
                    {
                        GC.Collect();
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var SequenceId = int.Parse(seq[SEVT.SequenceID].ToString());
                        var SourceKey = seq[SEVT.FieldValue].ToString();
                        var DocOrigen = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD) as SAPbobsCOM.UserObjectsMD;
                        DocOrigen.GetByKey(SourceKey);

                        XDocument xml_origen = XDocument.Parse(DocOrigen.GetAsXML());
                        var DocDestino = Destino.GetBusinessObjectFromXML(xml_origen.ToString(), 0) as SAPbobsCOM.UserObjectsMD;

                        if (DocDestino.Add() != 0)
                        {
                            lRetCode = Destino.GetLastErrorCode();
                            ErrMsg = Destino.GetLastErrorDescription().Replace("'", string.Empty);

                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{lRetCode.ToString()} - {ErrMsg}', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                        else
                        {
                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"{SEVT.XML}\"='{DocDestino.GetAsXML().Trim()}', \"{SEVT.TargetKey}\"='{Procesadora.GetNewObjectKey()}',\"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(SequenceId.ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }

                void add_cc(DataRow seq)
                {
                    try
                    {
                        SAPbobsCOM.Company Destino = null;
                        if (!Procesadora.Connected)
                            Procesadora = connect(dbProcesadora);

                        if (!Frutexsa.Connected)
                            Frutexsa = connect(dbFrutexsa);

                        if (!Pasera.Connected)
                            Pasera = connect(dbPasera);

                        if (seq[SEVT.TargetDB].ToString() == dbProcesadora)
                        {
                            Destino = Procesadora;
                        }
                        else
                        {
                            Destino = Pasera;
                        }

                        var CompanyService = Destino.GetCompanyService();
                        var ccSrv = CompanyService.GetBusinessService(SAPbobsCOM.ServiceTypes.ProfitCentersService) as SAPbobsCOM.IProfitCentersService;
                        var cc = ccSrv.GetDataInterface(SAPbobsCOM.ProfitCentersServiceDataInterfaces.pcsProfitCenter) as SAPbobsCOM.ProfitCenter;
                        var ccParams = (SAPbobsCOM.IProfitCenterParams)ccSrv.GetDataInterface((SAPbobsCOM.ProfitCentersServiceDataInterfaces)SAPbobsCOM.DimensionsServiceDataInterfaces.dsDimensionParams);

                        var rs = Frutexsa.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                        rs.DoQuery($"SELECT T0.\"PrcCode\", T0.\"PrcName\", T0.\"GrpCode\", T0.\"DimCode\", T0.\"CCTypeCode\", to_varchar(T0.\"ValidFrom\",'YYYYMMDD') \"ValidFrom\", to_varchar(T0.\"ValidTo\",'YYYYMMDD') \"ValidTo\", T0.\"Active\", T0.\"CCOwner\", T0.\"U_FRU_Fruta\" FROM OPRC T0 WHERE T0.\"PrcCode\" = '{seq[SEVT.FieldValue]}'");

                        if (rs.EoF)
                        {
                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='-999 - CC no encontrado', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(seq[SEVT.SequenceID].ToString()), updSql);
                            return;
                        }

                        try
                        {
                            cc.CenterCode = rs.GetColumnValue(0).ToString();
                            cc.CenterName = rs.GetColumnValue(1).ToString();
                            cc.GroupCode = rs.GetColumnValue(2).ToString();
                            cc.InWhichDimension = int.Parse(rs.GetColumnValue(3).ToString());
                            cc.CostCenterType = rs.GetColumnValue(4).ToString();
                            cc.Effectivefrom = (rs.GetColumnValue(5).ToString().Length > 0) ? DateTime.ParseExact(rs.GetColumnValue(5).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.Today;
                            cc.EffectiveTo = (rs.GetColumnValue(6).ToString().Length > 0) ? DateTime.ParseExact(rs.GetColumnValue(6).ToString(), "yyyyMMdd", CultureInfo.InvariantCulture) : DateTime.ParseExact("20991231", "yyyyMMdd", CultureInfo.InvariantCulture);
                            cc.Active = (rs.GetColumnValue(7).ToString() == "Y") ? SAPbobsCOM.BoYesNoEnum.tYES : SAPbobsCOM.BoYesNoEnum.tNO;
                            cc.UserFields.Item("U_FRU_Fruta").Value = rs.GetColumnValue(9).ToString();

                            ccSrv.AddProfitCenter(cc);

                            string updSql = $"\"{SEVT.Status}\"='OK', \"{SEVT.ProcessResult}\"='OK', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(seq[SEVT.SequenceID].ToString()), updSql);
                        }
                        catch (Exception e)
                        {
                            string updSql = $"\"{SEVT.Status}\"='Error', \"{SEVT.ProcessResult}\"='{e.Message}', \"Retry\"=(\"Retry\"+1)";
                            Actualiza_Sevt(int.Parse(seq[SEVT.SequenceID].ToString()), updSql);
                        }
                    }
                    catch (Exception ex)
                    {
                        CommonFunctions.LogFile(LogFile, ex.Message);
                    }
                }
            }
        }
    }
}