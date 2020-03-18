using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace pluginPlanificacion
{
    internal static class frmPlanificacion
    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;

            if (oMenuEvent.BeforeAction)
            {
                FormCreationPackage = (FormCreationParams)sbo_application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);

                try
                {
                    if (string.IsNullOrEmpty(sessionId))
                        sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                    string contenidoArchivo = Properties.Resources.ResourceManager.GetString("Planificacion");
                    string date = DateTime.Now.ToString("yyyyMMdd");

                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                    FormCreationPackage.BorderStyle = BoFormBorderStyle.fbs_Fixed;

                    FormCreationPackage.UniqueID = "Planificacion" + CommonFunctions.Random().ToString();
                    var oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    ((EditText)oForm.Items.Item(pluginForm.TxtFechaDesde.Uid).Specific).Value = date;
                    ((EditText)oForm.Items.Item(pluginForm.TxtFechaHasta.Uid).Specific).Value = date;
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkMostrarTodo).Specific).Item.Enabled = false;

                    try
                    {
                        oForm.Freeze(true);

                        DataTable oDT = oForm.DataSources.DataTables.Add(pluginForm.GridOV.Dt);
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOV.Uid).Specific;
                        grid.DataTable = oDT;

                        oDT = oForm.DataSources.DataTables.Add(pluginForm.GridLote.Dt);
                        grid = (Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                        grid.DataTable = oDT;

                        oDT = oForm.DataSources.DataTables.Add(pluginForm.GridLoteMP.Dt);
                        grid = (Grid)oForm.Items.Item(pluginForm.GridLoteMP.Uid).Specific;
                        grid.DataTable = oDT;

                        Button BtnBuscaPedido = (Button)oForm.Items.Item(pluginForm.BtnBuscaPedido).Specific;
                        BtnBuscaPedido.Item.Enabled = true;

                        var Uds1 = oForm.DataSources.UserDataSources.Add("ChkTodo", BoDataType.dt_SHORT_TEXT, 1);
                        CheckBox ChkMostrarTodo = ((CheckBox)oForm.Items.Item(pluginForm.ChkMostrarTodo).Specific);
                        ChkMostrarTodo.DataBind.SetBound(true, "", Uds1.UID);
                        ChkMostrarTodo.ValOn = "Y";
                        ChkMostrarTodo.ValOff = "N";

                        ((OptionBtn)oForm.Items.Item(pluginForm.RdNuez.Uid).Specific).GroupWith(pluginForm.RdCiruela.Uid);
                        ((OptionBtn)oForm.Items.Item(pluginForm.RdPasa.Uid).Specific).GroupWith(pluginForm.RdCiruela.Uid);
                        ((OptionBtn)oForm.Items.Item(pluginForm.RdAll.Uid).Specific).GroupWith(pluginForm.RdCiruela.Uid);

                        oForm.DataSources.UserDataSources.Item(pluginForm.RdCiruela.Uds).ValueEx = "1";

                        oForm.EnableMenu("4870", true);

                        oForm.Width = sbo_application.Desktop.Width - 10;
                        oForm.Top = sbo_application.Desktop.Top - 10;
                        //oForm.Height = sbo_application.Desktop.Height - 150;

                        oForm.Items.Item(pluginForm.GridOV.Uid).Width = oForm.Width - 80;
                        oForm.Items.Item(pluginForm.GridLoteMP.Uid).Width = oForm.Width - 80;
                        oForm.Items.Item(pluginForm.GridLote.Uid).Width = oForm.Width - 80;

                        //oForm.Items.Item(pluginForm.BtnPlanificar).Height = oForm.Height - 25;


                        oForm.Visible = true;

                    }
                    finally { oForm.Freeze(false); }
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
                case pluginForm.BtnPlanificar:
                    ButtonPlanificar(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.BtnCalibrar:
                    break;

                case pluginForm.ButtonCancel:
                    ButtonCacel(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.GridOV.Uid:
                    GridOV(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.GridLote.Uid:
                    GridSE(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.GridLoteMP.Uid:
                    GridLoteMP(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.BtnBuscaPedido:
                    BtnBuscaPedido(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ChkMostrarTodo:
                    ChkMostrarTodo(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.BtnFiltrar:
                    BtnFiltrar(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.BtnSelect:
                    BtnSelect(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }
        private static void BtnSelect(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                        var gridSE = oForm.Items.Item(pluginForm.GridLote.Uid).Specific as Grid;
                        gridSE.Columns.Item(0).Editable = true;
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }
        private static void BtnFiltrar(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                        var gridSE = oForm.Items.Item(pluginForm.GridLote.Uid).Specific as Grid;
                        gridSE.Columns.Item(0).Editable = false;

                        sbo_application.Menus.Item("4870").Activate();
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }
        private static void GridOV(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            /*
             *CIRUELA
             **** Preguntar fecha de OF
             **** n° cubo | lote | productor | tendencia calibre | fecha inicio fumigacion
             ******** Sol trasl cubo >> bodcal
             */

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED && oItemEvent.ColUID == "RowsHeader" && oItemEvent.Row != -1)
                {
                    try
                    {
                        sbo_application.StatusBar.SetText("CARGANDO LOTES, ESPERE...", BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);

                        ((CheckBox)oForm.Items.Item(pluginForm.ChkMostrarTodo).Specific).Item.Enabled = true;

                        var grid = oForm.Items.Item(pluginForm.GridOV.Uid).Specific as Grid;
                        int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);

                        grid.DataTable.Columns.Item(0).Cells.Item(row).Value = "Y";

                        var Fruta = SAPFunctions.GetFieldFromSelectedRow(grid, "Fruta");
                        var Calibre = SAPFunctions.GetFieldFromSelectedRow(grid, "Calibre");
                        var Tipo = SAPFunctions.GetFieldFromSelectedRow(grid, "Tipo");
                        var KilosOv = SAPFunctions.GetFieldFromSelectedRow(grid, "Kilos");
                        oForm.DataSources.UserDataSources.Item(pluginForm.UDKilosPedido).ValueEx = KilosOv;
                        var Filtro = string.Empty;
                        var Clasif = string.Empty;

                        oForm.Freeze(true);

                        for (var i = 0; i < grid.Rows.Count; i++)
                        {
                            if (i != row)
                                grid.DataTable.Columns.Item(0).Cells.Item(i).Value = "N";
                        }

                        if (Fruta.Equals("CIRUELA"))
                        {
                            if (Tipo.Contains("ASHLOCK"))
                            {
                                Clasif = "BUENA o REGULAR";
                                Filtro += $" and A.\"Calibre\" in (select to_varchar(\"U_CalProd\") from \"@DFO_CALCIR\" where \"U_CalSc\"=''{Calibre}'') and (A.\"Info.Detallada\" like ''%BUENA%'' or A.\"Info.Detallada\" like ''%REGULAR%'')";
                            }

                            else if (Tipo.Contains("ELLIOT"))
                            {
                                Filtro += $" and A.\"Calibre\" in (select to_varchar(\"U_CalProd\") from \"@DFO_CALCIR\" where \"U_CalEll\"=''X'')";
                            }

                            else if (Tipo.Contains("CON CAROZO"))
                            {
                                Clasif = "TIERNIZADO o REGULAR";
                                Filtro += $" and A.\"Calibre\" in (select to_varchar(\"U_CalProd\") from \"@DFO_CALCIR\" where \"U_CalCc\"=''{Calibre}'') and (A.\"Info.Detallada\" like ''%TIERNIZADO%'' or A.\"Info.Detallada\" like ''%REGULAR%'')";
                            }

                            else
                            {
                                Clasif = "TIERNIZADO o Regular";
                                Filtro += $" and A.\"Calibre\" in (select to_varchar(\"U_CalProd\") from \"@DFO_CALCIR\" where \"U_CalCn\"=''{Calibre}'') and (A.\"Info.Detallada\" like ''%TIERNIZADO%'' or A.\"Info.Detallada\" like ''%REGULAR%'')";
                            }

                            var TipoSecado = string.Empty;
                            var Prod = string.Empty;
                        }

                        if (Fruta.Equals("PASA"))
                        {
                            var Tipoo = string.Empty;
                        }

                        string sql = $"call calidad_pivot ('{Fruta}','SE', '{Filtro}');";
                        var gridSE = oForm.Items.Item(pluginForm.GridLote.Uid).Specific as Grid;
                        gridSE.DataTable.ExecuteQuery(sql);

                        XDocument doc = XDocument.Parse(gridSE.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All));

                        XElement newColumn = new XElement("Column",
                            new XAttribute("Uid", "Select"),
                            new XAttribute("Type", "1"),
                            new XAttribute("MaxLength", "1"));

                        XElement newCell = new XElement("Cell",
                            new XElement("ColumnUid", "Select"),
                            new XElement("Value", "N"));

                        doc.Root.Element("Columns").Element("Column").AddBeforeSelf(newColumn);

                        foreach (var item in doc.Descendants("Cells"))
                        {
                            item.Add(newCell);
                        }

                        gridSE.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_All, doc.ToString());

                        for (int i = 0; i < gridSE.Columns.Count; i++)
                        {
                            gridSE.Columns.Item(i).Editable = false;
                            gridSE.Columns.Item(i).TitleObject.Sortable = true;
                        }

                        ((EditTextColumn)gridSE.Columns.Item(0)).Type = BoGridColumnType.gct_CheckBox;
                        gridSE.Columns.Item(0).Editable = true;

                        for (var i = 1; i <= gridSE.Rows.Count; i++)
                        {
                            var status = gridSE.DataTable.GetValue("Status", i - 1).ToString();
                            var asig = gridSE.DataTable.GetValue("Asignada", i - 1);

                            if (status != "0")
                            {
                                gridSE.CommonSetting.SetRowBackColor(i, Colores.Red);
                            }
                            else if ((double)asig > 0)
                            {
                                gridSE.CommonSetting.SetRowBackColor(i, Colores.Yellow);
                            }
                            else
                            {
                                gridSE.CommonSetting.SetRowBackColor(i, -1);
                            }
                        }

                        //gridSE.Item.Enabled = false;

                        if (Fruta.Equals("CIRUELA"))
                        {
                            ((EditTextColumn)gridSE.Columns.Item(1)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(2)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(3)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(4)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(6)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(8)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(9)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(10)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(11)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(13)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(16)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(18)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(20)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(21)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(22)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(23)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(24)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(26)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(28)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(29)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(30)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(31)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(32)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(33)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(34)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(35)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(36)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(39)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(40)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(41)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(44)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(46)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(47)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(50)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(51)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(52)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(53)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(56)).Visible = false;
                        }

                        if (Fruta.Equals("PASA"))
                        {
                            ((EditTextColumn)gridSE.Columns.Item(1)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(2)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(3)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(4)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(6)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(8)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(9)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(19)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(21)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(22)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(23)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(26)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(27)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(28)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(29)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(31)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(32)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(33)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(34)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(35)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(36)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(39)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(41)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(42)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(48)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(49)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(50)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(51)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(52)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(56)).Visible = false;
                        }

                        sbo_application.StatusBar.SetText("LOTES CARGADOS", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                        //sql = $"call calidad_pivot ('{Fruta}','MP');";
                        //var gridMP = oForm.Items.Item(pluginForm.GridLoteMP.Uid).Specific as Grid;
                        //gridMP.DataTable.ExecuteQuery(sql);

                        //doc = XDocument.Parse(gridMP.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All));

                        //newColumn = new XElement("Column",
                        //    new XAttribute("Uid", "Select"),
                        //    new XAttribute("Type", "1"),
                        //    new XAttribute("MaxLength", "1"));

                        //newCell = new XElement("Cell",
                        //    new XElement("ColumnUid", "Select"),
                        //    new XElement("Value", "N"));

                        //doc.Root.Element("Columns").Element("Column").AddBeforeSelf(newColumn);

                        //foreach (var item in doc.Descendants("Cells"))
                        //{
                        //    item.Add(newCell);
                        //}

                        //gridMP.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_All, doc.ToString());

                        //for (int i = 0; i < gridMP.Columns.Count; i++)
                        //{
                        //    gridMP.Columns.Item(i).Editable = false;
                        //}

                        //((EditTextColumn)gridMP.Columns.Item(0)).Type = BoGridColumnType.gct_CheckBox;
                        //gridMP.Columns.Item(0).Editable = true;
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
                else
                {
                    ((CheckBox)oForm.Items.Item(pluginForm.ChkMostrarTodo).Specific).Item.Enabled = false;
                }
            }
        }
        private static void GridSE(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {

            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    var oForm = sbo_application.Forms.Item(formUID);
                    var grid = oForm.Items.Item(pluginForm.GridLote.Uid).Specific as Grid;
                    var xmlDT = grid.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All);
                    var xDoc = XDocument.Parse(xmlDT);
                    var kilos = 0.00;

                    foreach (var row in xDoc.Root.Element("Rows").Elements())
                    {
                        var sel = row.Element("Cells").Elements().Where(i => i.Element("ColumnUid").Value == "Select").Select(i => i.Element("Value").Value).FirstOrDefault();
                        if (sel == "Y")
                        {
                            kilos += row.Element("Cells").Elements().Where(i => i.Element("ColumnUid").Value == "EnStock").Select(i => i.Element("Value").Value).FirstOrDefault().GetDoubleFromString(",");
                        }
                    }
                    oForm.DataSources.UserDataSources.Item(pluginForm.UDKilosSelect).ValueEx = kilos.GetStringFromDouble(2);
                }
            }
        }
        private static void ChkMostrarTodo(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    try
                    {
                        oForm.Freeze(true);
                        var grid = oForm.Items.Item(pluginForm.GridOV.Uid).Specific as Grid;
                        int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);

                        var Fruta = SAPFunctions.GetFieldFromSelectedRow(grid, "Fruta");

                        string sql = $"call calidad_pivot ('{Fruta}','SE');";
                        var gridSE = oForm.Items.Item(pluginForm.GridLote.Uid).Specific as Grid;
                        gridSE.DataTable.ExecuteQuery(sql);

                        XDocument doc = XDocument.Parse(gridSE.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All));

                        XElement newColumn = new XElement("Column",
                            new XAttribute("Uid", "Select"),
                            new XAttribute("Type", "1"),
                            new XAttribute("MaxLength", "1"));

                        XElement newCell = new XElement("Cell",
                            new XElement("ColumnUid", "Select"),
                            new XElement("Value", "N"));

                        doc.Root.Element("Columns").Element("Column").AddBeforeSelf(newColumn);

                        foreach (var item in doc.Descendants("Cells"))
                        {
                            item.Add(newCell);
                        }

                        gridSE.DataTable.LoadSerializedXML(BoDataTableXmlSelect.dxs_All, doc.ToString());

                        for (int i = 0; i < gridSE.Columns.Count; i++)
                        {
                            gridSE.Columns.Item(i).Editable = false;
                            gridSE.Columns.Item(i).TitleObject.Sortable = true;
                        }

                        ((EditTextColumn)gridSE.Columns.Item(0)).Type = BoGridColumnType.gct_CheckBox;
                        gridSE.Columns.Item(0).Editable = true;

                        for (var i = 1; i <= gridSE.Rows.Count; i++)
                        {
                            var status = gridSE.DataTable.GetValue("Status", i - 1).ToString();
                            var asig = gridSE.DataTable.GetValue("Asignada", i - 1);

                            if (status != "0")
                            {
                                gridSE.CommonSetting.SetRowBackColor(i, Colores.Red);
                            }
                            else if ((double)asig > 0)
                            {
                                gridSE.CommonSetting.SetRowBackColor(i, Colores.Yellow);
                            }
                            else
                            {
                                gridSE.CommonSetting.SetRowBackColor(i, -1);
                            }
                        }

                        if (Fruta.Equals("CIRUELA"))
                        {
                            ((EditTextColumn)gridSE.Columns.Item(1)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(2)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(3)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(4)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(6)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(8)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(9)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(10)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(11)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(13)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(16)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(18)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(20)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(21)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(22)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(23)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(24)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(26)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(28)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(29)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(30)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(31)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(32)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(33)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(34)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(35)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(36)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(39)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(40)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(41)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(44)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(46)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(47)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(50)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(51)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(52)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(53)).Visible = false;
                            ((EditTextColumn)gridSE.Columns.Item(56)).Visible = false;
                        }
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }
        private static void GridLoteMP(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
        }
        private static void BtnBuscaPedido(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                    string Fruta = "''";

                    switch (oForm.DataSources.UserDataSources.Item(pluginForm.RdCiruela.Uds).ValueEx)
                    {
                        case "1":
                            Fruta = "'CIRUELA'";
                            break;

                        case "2":
                            Fruta = "'NUEZ'";
                            break;

                        case "3":
                            Fruta = "'PASA'";
                            break;

                        case "4":
                            Fruta = "'CIRUELA','NUEZ','PASA'";
                            break;
                    }

                    try
                    {
                        oForm.Freeze(true);
                        string dateDesde = ((EditText)oForm.Items.Item(pluginForm.TxtFechaDesde.Uid).Specific).Value.Trim();
                        string dateHasta = ((EditText)oForm.Items.Item(pluginForm.TxtFechaHasta.Uid).Specific).Value.Trim();

                        DateTime oDate = DateTime.ParseExact(dateDesde, "yyyyMMdd", CultureInfo.CurrentUICulture);
                        dateDesde = oDate.ToString("yyyyMMdd");

                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOV.Uid).Specific;
                        DataTable oDT = grid.DataTable;
                        oDT.Clear();

                        string sSql = "" +
                        "select 'N' \"Select\", " +
                            "a.\"DocEntry\", " +
                            "a.\"DocNum\" \"Nro SAP\", " +
                            "a.\"NumAtCard\" \"Pedido\", " +
                            "a.\"CardCode\" \"Cod Cliente\", " +
                            "a.\"CardName\" \"Razon Social\", " +
                            "a.\"DocDueDate\" \"Fecha Carga\", " +
                            "week(a.\"DocDueDate\") \"Semana\", " +
                            "case a.\"Confirmed\" when 'Y' then 'Si' else 'No' end \"Autorizado\", " +
                            "b.\"LineNum\"+1 \"Linea\", " +
                            "b.\"ItemCode\" \"Cod Item\", " +
                            "b.\"Dscription\" \"Descripcion\", " +
                            "b.\"Quantity\" \"Cantidad\", " +
                            "b.\"U_FRU_Variedad\" \"Variedad\", " +
                            "b.\"U_FRU_Tipo\" \"Tipo\", " +
                            "b.\"U_FRU_Calibre\" \"Calibre\", " +
                            "b.\"U_FRU_Color\" \"Color\", " +
                            "b.\"U_FRU_Conteo\" \"Conteo\", " +
                            "b.\"U_FRU_Caracteristica\" \"Caracteristica\", " +
                            "b.\"U_FRU_FichaTecnica\" \"Ficha Tecnica\", " +
                            "e.\"U_FRU_Tolerancia\" \"Humedad\", " +
                            "b.\"U_FRU_CajaSaco\" \"Caja\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_CajaSaco\") \"Descripcion Caja\", " +
                            "b.\"U_FRU_Bolsa\" \"Bolsa\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Bolsa\") \"Descripcion Bolsa\", " +
                            "b.\"U_FRU_Pallet\" \"Pallet\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Pallet\") \"Descripcion Pallet\", " +
                            "b.\"U_FRU_EtqCliente\" \"Etiqueta Cli\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_EtqCliente\") \"Descripcion Etiqueta Cliente\", " +
                            "b.\"U_FRU_EtqSAG\" \"Etiqueta SAG\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_EtqSAG\") \"Descripcion Etiqueta SAG\", " +
                            "b.\"U_FRU_Zuncho\" \"Zuncho\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Zuncho\") \"Descripcion Zuncho\", " +
                            "b.\"U_FRU_Liner\" \"Liner\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Liner\") \"Descripcion Liner\", " +
                            "b.\"U_FRU_CintaEmb\" \"Cinta emb\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_CintaEmb\") \"Descripcion Cinta\", " +
                            "b.\"U_FRU_Pegamento\" \"Pegamento\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Pegamento\") \"Descripcion Pegamento\", " +
                            "b.\"U_FRU_Esquinero\" \"Esquinero\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Esquinero\") \"Descripcion Esquinero\", " +
                            "b.\"U_FRU_Perimetral\" \"Perimetral\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_Perimetral\") \"Descripcion Perimetral\", " +
                            "b.\"U_FRU_FilmRetail\" \"Film\", " +
                            "(select \"ItemName\" from OITM where \"ItemCode\"=b.\"U_FRU_FilmRetail\") \"Descripcion Film\", " +
                            "d.\"U_FRU_Fruta\" \"Fruta\", " +
                            "b.\"Weight1\" \"Kilos\", " +
                            "g.\"DocEntry\" \"DocEntryPlanta\" " +

                        "from FRUTEXSA.ORDR a " +
                        "inner join FRUTEXSA.RDR1 b on a.\"DocEntry\"=b.\"DocEntry\" " +
                        "inner join FRUTEXSA.OITM c on c.\"ItemCode\"=b.\"ItemCode\" " +
                        "inner join FRUTEXSA.OITB d on d.\"ItmsGrpCod\"=c.\"ItmsGrpCod\" " +
                        "left join FRUTEXSA.ITT1 e on e.\"Father\" = b.\"U_FRU_FichaTecnica\" " +
                        "left join FRUTEXSA.OITM f on f.\"ItemCode\" = e.\"Code\" " +
                        "inner join ORDR g on g.\"U_IC_DocOrigen\"=a.\"DocEntry\" " +

                        "where a.\"DocStatus\"='O' " +
                        "and a.\"CANCELED\"='N' " +
                        "and b.\"LineStatus\"='O'" +
                        "and f.\"ItemCode\" not in ('P-0190','P-0191') " +
                        "and f.\"ItemName\" like 'HUMEDAD%' " +
                        "and " +
                            "(" +
                            "g.\"DocEntry\"||b.\"ItemCode\"||ifnull(b.\"U_FRU_Variedad\",'')||ifnull(b.\"U_FRU_Tipo\",'')||ifnull(b.\"U_FRU_Calibre\",'')||ifnull(b.\"U_FRU_Color\",'') " +
                            "not in (select distinct \"OriginAbs\"||\"ItemCode\"||ifnull(\"U_FRU_Variedad\",'')||ifnull(\"U_FRU_Tipo\",'')||ifnull(\"U_FRU_Calibre\",'')||ifnull(\"U_FRU_Color\",'') from OWOR where \"OriginAbs\" is not null) " +
                            ") " +
                        $"and d.\"U_FRU_Fruta\" in ({Fruta}) " +
                        "order by 9 desc, 8 asc, 7 asc, 1 asc;";

                        oDT.ExecuteQuery(sSql);
                        grid.SelectionMode = BoMatrixSelect.ms_Single;

                        for (int i = 0; i < grid.Columns.Count; i++)
                        {
                            grid.Columns.Item(i).Editable = false;
                        }

                        grid.Columns.Item(1).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(4)).LinkedObjectType = "2";
                        ((EditTextColumn)grid.Columns.Item(10)).LinkedObjectType = "4";
                        ((EditTextColumn)grid.Columns.Item(21)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(23)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(25)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(27)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(29)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(31)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(33)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(35)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(37)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(39)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(41)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(43)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(45)).Visible = false;
                        ((EditTextColumn)grid.Columns.Item(46)).Visible = false;

                        ((EditTextColumn)grid.Columns.Item(0)).Type = BoGridColumnType.gct_CheckBox;
                        grid.Columns.Item(0).Editable = true;

                        grid.AutoResizeColumns();

                        for (var i = 1; i <= grid.Rows.Count; i++)
                        {
                            if (grid.DataTable.GetValue("Autorizado", i - 1).ToString() == "No")
                            {
                                grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                            }
                            else
                            {
                                grid.CommonSetting.SetRowBackColor(i, -1);
                            }
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
        private static void ButtonPlanificar(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var FechaOF = oForm.DataSources.UserDataSources.Item(pluginForm.UDFechaOF).ValueEx;
                    var FechaSol = oForm.DataSources.UserDataSources.Item(pluginForm.UDFechaSol).ValueEx;

                    if (string.IsNullOrEmpty(FechaOF))
                    {
                        bBubbleEvent = false;
                        sbo_application.MessageBox("Debe ingresar fecha de OF");
                        return;
                    }
                    if (string.IsNullOrEmpty(FechaSol))
                    {
                        bBubbleEvent = false;
                        sbo_application.MessageBox("Debe ingresar fecha de Solicitud");
                        return;
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var grid = (Grid)oForm.Items.Item(pluginForm.GridOV.Uid).Specific;
                    //var DocEntry = SAPFunctions.GetFieldFromSelectedRow(grid, "DocEntry");
                    var LineNum = SAPFunctions.GetFieldFromSelectedRow(grid, "Linea");
                    var Fruta = SAPFunctions.GetFieldFromSelectedRow(grid, "Fruta");
                    var rowIndex = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                    var DocEntryPlanta = SAPFunctions.GetFieldFromSelectedRow(grid, "DocEntryPlanta");

                    var Variedad = SAPFunctions.GetFieldFromSelectedRow(grid, "Variedad");
                    var Tipo = SAPFunctions.GetFieldFromSelectedRow(grid, "Tipo");
                    var Calibre = SAPFunctions.GetFieldFromSelectedRow(grid, "Calibre");
                    var Color = SAPFunctions.GetFieldFromSelectedRow(grid, "Color");
                    var Conteo = SAPFunctions.GetFieldFromSelectedRow(grid, "Conteo");
                    var Caracteristica = SAPFunctions.GetFieldFromSelectedRow(grid, "Caracteristica");

                    var gridSE = (Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                    var dtXML = XDocument.Parse(gridSE.DataTable.SerializeAsXML(BoDataTableXmlSelect.dxs_All));
                    var Columns = dtXML.Element("DataTable").Element("Columns").Elements("Column");
                    var Rows = dtXML.Element("DataTable").Element("Rows").Elements("Row");
                    var PT = grid.DataTable.GetValue("Cod Item", rowIndex).ToString();
                    var QTY = grid.DataTable.GetValue("Cantidad", rowIndex).ToString().GetDoubleFromString(",");

                    var ListaMat = CommonFunctions.GET(ServiceLayer.ProductTrees, PT, null, sessionId, out System.Net.HttpStatusCode httpStatus).DeserializeJsonObject<ProductTrees>();
                    if (httpStatus == System.Net.HttpStatusCode.NotFound)
                        throw new Exception($"No existe lista de materiales para el batch de produccion del codigo: {PT}, favor contactar a Hector Pincheira, anexo 9143");

                    var prod_lines = new List<ProductionOrderLine>();
                    var ins_tr_lines = new List<StockTransferLines>();
                    var se_tr_lines = new List<StockTransferLines>();

                    string SE = string.Empty;
                    double SEqty = 0;
                    var toWhs = string.Empty;

                    if (Fruta.Equals("CIRUELA"))
                        toWhs = "BACOP";

                    if (Fruta.Equals("NUEZ"))
                        toWhs = "N3-ACO";

                    if (Fruta.Equals("PASA"))
                        toWhs = "BSE";


                    foreach (var row in Rows.Where(i => i.Element("Cells").Elements("Cell").Where(i => i.Element("ColumnUid").Value == "Select").Select(i => i.Element("Value").Value).FirstOrDefault() == "Y"))
                    {
                        var Cells = row.Element("Cells").Elements("Cell");
                        var _qtyLine = Cells.Where(i => i.Element("ColumnUid").Value == "EnStock").Select(i => i.Element("Value").Value).FirstOrDefault().GetDoubleFromString(",");
                        var fromWhs = Cells.Where(i => i.Element("ColumnUid").Value == "Bodega").Select(i => i.Element("Value").Value).FirstOrDefault();

                        SE = Cells.Where(i => i.Element("ColumnUid").Value == "NumeroArticulo").Select(i => i.Element("Value").Value).FirstOrDefault();
                        SEqty += _qtyLine;

                        var bt = Cells.Where(i => i.Element("ColumnUid").Value == "Lote").Select(i => i.Element("Value").Value).FirstOrDefault();
                        var rs = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordsetEx) as SAPbobsCOM.RecordsetEx;
                        var sql = $"select top 1 A.\"Bodega\" from \"FRU_STOCK_LOTE\" A inner join \"FRU_MAESTRO_LOTE\" B on A.\"AbsEntry\"=B.\"AbsEntry\" where B.\"Lote\"='{bt}'";
                        rs.DoQuery(sql);

                        se_tr_lines.Add(
                            new StockTransferLines
                            {
                                ItemCode = SE,
                                Quantity = _qtyLine,
                                FromWarehouseCode = fromWhs,
                                WarehouseCode = rs.GetColumnValue(0).ToString(),
                                BatchNumbers = new List<BatchNumbers>
                                {
                                    new BatchNumbers
                                    {
                                        BatchNumber = bt,
                                        Quantity = _qtyLine
                                    }
                                }
                            }
                        );
                    }

                    prod_lines.Add(
                        new ProductionOrderLine
                        {
                            ItemNo = SE,
                            PlannedQuantity = SEqty,
                            Warehouse = toWhs
                        }
                    );

                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        if (grid.DataTable.Columns.Item(i).Name != "Cod Item" && grid.DataTable.Columns.Item(i).Name != "Ficha Tecnica")
                        {
                            try
                            {
                                var _value = grid.DataTable.GetValue(i, rowIndex).ToString();
                                var it = CommonFunctions.GET(ServiceLayer.Items, _value, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();
                                toWhs = string.Empty;

                                if (Fruta.Equals("CIRUELA"))
                                    toWhs = "BPROC";

                                if (Fruta.Equals("NUEZ"))
                                    toWhs = "N8-BPT";

                                if (Fruta.Equals("PASA"))
                                    toWhs = "BSE";
                                    //BPT

                                if (it.ItemCode != null)
                                {
                                    var line = new ProductionOrderLine
                                    {
                                        ItemNo = it.ItemCode,
                                        PlannedQuantity = (ListaMat.ProductTreeLines.Where(i => i.ItemCode == it.ItemCode).Select(i => i.Quantity).FirstOrDefault() / ListaMat.Quantity) * QTY,
                                        Warehouse = toWhs
                                    };

                                    if (line.PlannedQuantity != null)
                                    {
                                        prod_lines.Add(line);

                                        ins_tr_lines.Add(
                                            new StockTransferLines
                                            {
                                                ItemCode = line.ItemNo,
                                                Quantity = (double)line.PlannedQuantity,
                                                FromWarehouseCode = (sbo_application.Company.DatabaseName.Contains("PASERA")) ? "BPANOL" : "BMAT",
                                                WarehouseCode = toWhs
                                            }
                                        );
                                    }
                                }
                            }
                            catch { }
                        }
                    }
                    var FechaOF = DateTime.ParseExact(oForm.DataSources.UserDataSources.Item(pluginForm.UDFechaOF).ValueEx, "yyyyMMdd", CultureInfo.InvariantCulture);
                    var FechaSol = DateTime.ParseExact(oForm.DataSources.UserDataSources.Item(pluginForm.UDFechaSol).ValueEx, "yyyyMMdd", CultureInfo.InvariantCulture);

                    var prod = new ProductionOrder
                    {
                        ItemNo = PT,
                        StartDate = FechaOF.ToString("yyyyMMdd"),
                        PlannedQuantity = QTY,
                        ProductionOrderLines = prod_lines,
                        ProductionOrderType = "bopotSpecial",
                        Warehouse = toWhs,
                        ProductionOrderOriginEntry = int.Parse(DocEntryPlanta),
                        U_FRU_Calibre = Calibre,
                        U_FRU_Caracteristica = Caracteristica,
                        U_FRU_Color = Color,
                        U_FRU_Conteo = Conteo,
                        U_FRU_Tipo = Tipo,
                        U_FRU_Variedad = Variedad
                    };

                    var response = CommonFunctions.POST(ServiceLayer.ProductionOrders, prod, sessionId, out System.Net.HttpStatusCode statusCode);
                    if (statusCode == System.Net.HttpStatusCode.Created)
                    {
                        var of = response.DeserializeJsonObject<ProductionOrder>();
                        sbo_application.MessageBox($"OF : {of.DocumentNumber} creada con exito");

                        var se_tr = new StockTransfer
                        {
                            DueDate = FechaSol.ToString("yyyyMMdd"),
                            TaxDate = FechaSol.ToString("yyyyMMdd"),
                            U_DTE_FolioRef = of.DocumentNumber.ToString(),
                            StockTransferLines = se_tr_lines
                        };

                        var ins_tr = new StockTransfer
                        {
                            DueDate = FechaSol.ToString("yyyyMMdd"),
                            TaxDate = FechaSol.ToString("yyyyMMdd"),
                            U_DTE_FolioRef = of.DocumentNumber.ToString(),
                            StockTransferLines = ins_tr_lines
                        };

                        response = CommonFunctions.POST(ServiceLayer.InventoryTransferRequests, se_tr, sessionId, out statusCode);
                        response = CommonFunctions.POST(ServiceLayer.InventoryTransferRequests, ins_tr, sessionId, out statusCode);
                    }
                    else
                    {
                        var _Error = response.DeserializeJsonToDynamic();
                        throw new Exception($"Error en el registro : {_Error.error.message.value.ToString()}");
                    }

                    oForm.Items.Item(pluginForm.BtnBuscaPedido).Click(BoCellClickType.ct_Regular);
                    oForm.DataSources.UserDataSources.Item(pluginForm.UDKilosPedido).ValueEx = "0.00";
                    oForm.DataSources.UserDataSources.Item(pluginForm.UDKilosSelect).ValueEx = "0.00";
                    oForm.DataSources.UserDataSources.Item(pluginForm.UDFechaOF).ValueEx = "";
                    oForm.DataSources.UserDataSources.Item(pluginForm.UDFechaSol).ValueEx = "";
                    gridSE.DataTable.Clear();
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
    }
}