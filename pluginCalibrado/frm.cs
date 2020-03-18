using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Linq;
using System.Xml;

namespace pluginPrdSE
{
    internal static class frm
    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;

            if (oMenuEvent.BeforeAction)
            {
                FormCreationPackage = (FormCreationParams)sbo_application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);

                try
                {
                    if (string.IsNullOrEmpty(SessionId))
                        SessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                    string contenidoArchivo = Properties.Resources.ResourceManager.GetString(pluginForm.FormType);

                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                    FormCreationPackage.UniqueID = pluginForm.FormType + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    oForm.Mode = BoFormMode.fm_OK_MODE;

                    for (int i = 3; i < 100; i++)
                    {
                        try
                        {
                            oForm.Items.Item(i).AffectsFormMode = false;
                        }
                        catch
                        {
                        }
                    }

                    DataTable oDT = oForm.DataSources.DataTables.Add(pluginForm.GridCalibrado.Dt);
                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                    grid.DataTable = oDT;

                    DataTable oDT2 = oForm.DataSources.DataTables.Add(pluginForm.GridCalibraPeso.Dt);
                    Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                    grid2.DataTable = oDT2;

                    DataTable oDT3 = oForm.DataSources.DataTables.Add(pluginForm.GridCalibraAprueba.Dt);
                    Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                    grid3.DataTable = oDT3;

                    DataTable oDT4 = oForm.DataSources.DataTables.Add(pluginForm.GridSubProd.Dt);
                    Grid grid4 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;
                    grid4.DataTable = oDT4;

                    DataTable oDT5 = oForm.DataSources.DataTables.Add(pluginForm.GridConsumoSP.Dt);
                    Grid grid5 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;
                    grid5.DataTable = oDT5;

                    Item oItem;

                    oItem = oForm.Items.Add("OF", BoFormItemTypes.it_EDIT);
                    oItem.Left = 800;
                    EditText OF = (EditText)oForm.Items.Item("OF").Specific;
                    OF.Item.Enabled = false;

                    oItem = oForm.Items.Add("OV", BoFormItemTypes.it_EDIT);
                    oItem.Left = 800;
                    EditText OV = (EditText)oForm.Items.Item("OV").Specific;
                    OV.Item.Enabled = false;

                    LinkedButton Linked = (LinkedButton)oForm.Items.Item(pluginForm.LinkedNroOrden).Specific;
                    Linked.Item.LinkTo = "OF";
                    Linked.LinkedObject = BoLinkedObject.lf_ProductionOrder;

                    Linked = (LinkedButton)oForm.Items.Item(pluginForm.LinkedOV).Specific;
                    Linked.Item.LinkTo = "OV";
                    Linked.LinkedObject = BoLinkedObject.lf_Order;

                    Linked = (LinkedButton)oForm.Items.Item(pluginForm.LinkedCardCode).Specific;
                    Linked.Item.LinkTo = pluginForm.TxtCardCode.Uid;
                    Linked.LinkedObject = BoLinkedObject.lf_BusinessPartner;

                    Linked = (LinkedButton)oForm.Items.Item(pluginForm.LinkedItemCode).Specific;
                    Linked.Item.LinkTo = pluginForm.TxtItemCode.Uid;
                    Linked.LinkedObject = BoLinkedObject.lf_Items;

                    EditText NumPreTarja = (EditText)oForm.Items.Item(pluginForm.TxtPreTarja.Uid).Specific;
                    NumPreTarja.Item.Top = 140;
                    NumPreTarja.Item.Left = 35;
                    NumPreTarja.Item.Height = 50;
                    NumPreTarja.Item.Width = 300;
                    NumPreTarja.Item.FontSize = 30;
                    NumPreTarja.Item.BackColor = Colores.White;

                    EditText NumTarjaApr = (EditText)oForm.Items.Item(pluginForm.TxtTarjaApr.Uid).Specific;
                    NumTarjaApr.Item.Top = 140;
                    NumTarjaApr.Item.Left = 35;
                    NumTarjaApr.Item.Height = 50;
                    NumTarjaApr.Item.Width = 300;
                    NumTarjaApr.Item.FontSize = 30;
                    NumTarjaApr.Item.BackColor = Colores.White;

                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        grid.Columns.Item(i).Editable = true;
                    }

                    grid2.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid2.Columns.Count; i++)
                    {
                        grid2.Columns.Item(i).Editable = false;
                    }

                    grid3.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid3.Columns.Count; i++)
                    {
                        grid3.Columns.Item(i).Editable = false;
                    }

                    ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLOrdenFab);
                    Conditions oCons = oCFL.GetConditions();

                    Condition oCon = oCons.Add();
                    oCon.Alias = "Status";
                    oCon.Operation = BoConditionOperation.co_EQUAL;
                    oCon.CondVal = "R";
                    oCon.Relationship = BoConditionRelationship.cr_AND;
                    oCon = oCons.Add();
                    oCon.Alias = "ItemCode";
                    oCon.Operation = BoConditionOperation.co_CONTAIN;
                    oCon.CondVal = "CA";
                    oCon.Relationship = BoConditionRelationship.cr_OR;
                    oCon = oCons.Add();
                    oCon.Alias = "ItemCode";
                    oCon.Operation = BoConditionOperation.co_CONTAIN;
                    oCon.CondVal = "SE";
                    oCFL.SetConditions(oCons);

                    ChooseFromListCollection oCFLs = null;
                    Conditions oCons1 = null;
                    Condition oCon1 = null;
                    oCFLs = oForm.ChooseFromLists;
                    ChooseFromList oCFL1 = null;
                    ChooseFromListCreationParams oCFLCreationParams = null;
                    oCFLCreationParams = ((ChooseFromListCreationParams)(sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_ChooseFromListCreationParams)));
                    oCFLCreationParams.MultiSelection = false;
                    oCFLCreationParams.ObjectType = "4";
                    oCFLCreationParams.UniqueID = "CFL1";
                    oCFL1 = oCFLs.Add(oCFLCreationParams);

                    oCons1 = oCFL1.GetConditions();
                    oCon1 = oCons1.Add();
                    oCon1.Alias = "U_Subfamilia";
                    oCon1.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCon1.CondVal = "BINS";
                    oCFL1.SetConditions(oCons1);

                    ChooseFromList oCFL4 = oForm.ChooseFromLists.Item(pluginForm.CFLEnvase);
                    Conditions oCons4 = null;
                    Condition oCon4 = null;
                    oCons4 = oCFL4.GetConditions();
                    oCon4 = oCons4.Add();
                    oCon4.Alias = "U_Subfamilia";
                    oCon4.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                    oCon4.CondVal = "BINS";
                    oCFL4.SetConditions(oCons1);

                    ((Folder)oForm.Items.Item(pluginForm.FolderBoca).Specific).Item.Click();

                    grid.SelectionMode = BoMatrixSelect.ms_Single;
                    grid4.SelectionMode = BoMatrixSelect.ms_Single;
                    grid5.SelectionMode = BoMatrixSelect.ms_Single;

                    oForm.Visible = true;
                }
                catch
                {
                    throw;
                }
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            switch (oItemEvent.ItemUID)
            {
                case pluginForm.ButtonOK:
                    ButtonOk(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonCancel:
                    ButtonCacel(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.TxtNroOrden.Uid:
                    TxtNroOrden(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.TxtEnvase.Uid:
                    TxtEnvase(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.GridCalibrado.Uid:
                    MatrixCalibrado(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                //case pluginForm.TxtNroPT.Uid:
                //    TxtNroPT(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                //    break;

                case pluginForm.ButtonInsertCal:
                    ButtonInsertCal(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonConfCal:
                    ButtonConfCal(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.TxtPreTarja.Uid:
                    TxtPreTarja(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonAprueba:
                    ButtonAprueba(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonRechazo:
                    ButtonRechazo(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.TxtTarjaApr.Uid:
                    TxtTarjaApr(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.GridCalibraAprueba.Uid:
                    GridCalibraAprueba(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.GridCalibraPeso.Uid:
                    GridCalibraPeso(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonAsignPeso:
                    ButtonAsignPeso(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonAsigLoteSP:
                    ButtonAsigLoteSP(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonAprReparo:
                    ButtonAprReparo(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonPreviewTarja:
                    ButtonPreviewTarja(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonCalidad:
                    ButtonCalidad(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.FolderAprueba:
                    FolderAprueba(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.FolderApruebaSP:
                    FolderApruebaSP(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.TxtBodegaDest.Uid:
                    TxtBodegaDest(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;
            }
        }

        private static void ButtonOk(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                    //sbo_application.StatusBar.SetText("Button OK", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }
            }
        }

        private static void ButtonCacel(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                    //sbo_application.StatusBar.SetText("Button Cancel", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }
            }
        }

        private static void ButtonCalidad(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
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

                    var grid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;
                    var lote = SAPFunctions.GetFieldFromSelectedRow(grid, "U_IdTarja");
                    var item = ((EditText)oForm.Items.Item(pluginForm.TxtItemName.Uid).Specific).Value;

                    dynamic Cabecera = new System.Dynamic.ExpandoObject();

                    Cabecera.Tipo = "202";
                    Cabecera.Valor = oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx;
                    Cabecera.Lote = lote;

                    if (item.ToUpper().Contains("NUEZ"))
                    {
                        _ = SAPFunctions.LoadFormCalidad(ref sbo_application, "NUEZ-20017-RG-5.5.1.2CN", SessionId, Cabecera);
                    }

                    if (item.ToUpper().Contains("CIRU"))
                    {
                        _ = SAPFunctions.LoadFormCalidad(ref sbo_application, "CIRUELA-CAL-RG-5.5.1.2C", SessionId, Cabecera);
                    }

                    if (item.ToUpper().Contains("PASA"))
                    {
                        _ = SAPFunctions.LoadFormCalidad(ref sbo_application, "PASA-20017-RG-5.6.1CCPSE", SessionId, Cabecera);
                    }
                }
            }
        }

        private static void TxtEnvase(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT1 = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT1 != null)
                    {
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtEnvase.Uds).ValueEx = oDT1.GetValue("ItemCode", 0).ToString();
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtDescEnvase.Uds).ValueEx = oDT1.GetValue("ItemName", 0).ToString();
                    }
                }
            }
        }

        private static void TxtNroOrden(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT1 = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT1 != null)
                    {
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtNroOrden.Uds).ValueEx = oDT1.GetValue("DocNum", 0).ToString();

                        valores.NroOF = oDT1.GetValue("DocEntry", 0).ToString();
                        oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx = oDT1.GetValue("DocEntry", 0).ToString();

                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string sSql = "SELECT  T0.\"ItemCode\",T0.\"ProdName\", " +
                            "T0.\"PlannedQty\", T0.\"OriginNum\",  T0.\"OriginAbs\", T0.\"PostDate\", T0.\"DueDate\",T0.\"CardCode\" " +
                            "FROM OWOR T0  " +
                            "WHERE T0.\"DocEntry\" = '" + oDT1.GetValue("DocEntry", 0).ToString() + "' ";
                        oRS.DoQuery(sSql);
                        if (oRS.RecordCount != 0)
                        {
                            EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtItemCode.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("ItemCode").Value.ToString();

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtItemName.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("ProdName").Value.ToString();

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtCantPlan.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("PlannedQty").Value.ToString();

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtNroOc.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("OriginNum").Value.ToString();

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtFecCreac.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("PostDate").Value.ToString();

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtFecPlanf.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("DueDate").Value.ToString();

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                            TxtForm.Value = oRS.Fields.Item("CardCode").Value.ToString();

                            valores.NroOC = oRS.Fields.Item("OriginAbs").Value.ToString();
                            valores.CardCode = oRS.Fields.Item("CardCode").Value.ToString();

                            EditText OF = (EditText)oForm.Items.Item("OF").Specific;
                            OF.Value = valores.NroOF;

                            EditText OV = (EditText)oForm.Items.Item("OV").Specific;
                            OV.Value = valores.NroOC;
                            //Linked = (SAPbouiCOM.LinkedButton)oForm.Items.Item(pluginForm.LinkedOV).Specific;
                            //Linked.Item.LinkTo = pluginForm.TxtNroOc.Uid;
                            //Linked.LinkedObject = BoLinkedObject.lf_Order;

                            //Linked = (SAPbouiCOM.LinkedButton)oForm.Items.Item(pluginForm.LinkedCardCode).Specific;
                            //Linked.Item.LinkTo = pluginForm.TxtCardCode.Uid; ;
                            //Linked.LinkedObject = BoLinkedObject.lf_BusinessPartner;
                            //SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                            //SAPbouiCOM.DataTable oDT1 = grid.DataTable;
                            //oDT1.Clear();

                            //sSql = "SELECT  T0.\"DocEntry\",T0.\"DocNum\", " +
                            //    "T0.\"U_CodigoPT\", T0.\"U_LoteID\" " +
                            //    "FROM \"@DFO_OLOPT\" T0 " +
                            //    "WHERE T0.\"U_DocEntryOF\" = '" + oDT.GetValue("DocEntry", 0).ToString() + "' ";

                            //oDT1.ExecuteQuery(sSql);
                            //SAPbouiCOM.EditTextColumn oColumns = (SAPbouiCOM.EditTextColumn)grid.Columns.Item("DocEntry");
                            //oColumns.Description = "N° Consumo";
                            //oColumns.LinkedObjectType = "60";

                            try
                            {
                                SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                //SAPbouiCOM.DataTable oDT = grid.DataTable;
                                grid.DataTable.Clear();
                                sSql = "SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                "FROM \"@DFO_OCAOF\" T0 " +
                                "WHERE T0.\"U_BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' order by  T0.\"DocEntry\" ";
                                grid.DataTable.ExecuteQuery(sSql);
                                grid.SelectionMode = BoMatrixSelect.ms_Single;
                                grid.Columns.Item(2).Visible = false;
                                grid.Columns.Item(4).Editable = true;
                                grid.Columns.Item(6).Editable = true;

                                for (int i = 0; i <= grid.Columns.Count - 1; i++)
                                {
                                    grid.Columns.Item(i).Editable = false;
                                }

                                int k = 4;
                                grid.Columns.Item(k).Editable = true;
                                k = 6;
                                grid.Columns.Item(k).Editable = true;

                                int gridrowcount = grid.Rows.Count;
                                for (var i = 1; i <= gridrowcount; i++)
                                {
                                    grid.CommonSetting.SetRowBackColor(i, -1);
                                    if (grid.DataTable.GetValue("U_NBoca", i - 1).ToString() != "0")
                                    {
                                        grid.CommonSetting.SetRowEditable(i, false);
                                    }

                                    if (grid.DataTable.GetValue("U_CodEnvase", i - 1).ToString() != "")
                                    {
                                        grid.CommonSetting.SetRowEditable(i, false);
                                    }

                                    if (!string.IsNullOrEmpty(grid.DataTable.GetValue("Remark", i - 1).ToString()))
                                    {
                                        grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                    }

                                    if (grid.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                    {
                                        grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                    }
                                }

                                string sDocEntry = grid.DataTable.GetValue("DocEntry", grid.DataTable.Rows.Count - 1).ToString();
                                if (sDocEntry != "0")
                                {
                                    grid.DataTable.Rows.Add();
                                    grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                                }

                                k = 6;
                                grid.Columns.Item(6).Type = SAPbouiCOM.BoGridColumnType.gct_EditText;
                                EditTextColumn oEdit = (EditTextColumn)grid.Columns.Item(k);



                                ComboBox CBcalibre = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;

                                int CountCB = CBcalibre.ValidValues.Count;
                                if (CountCB > 0)
                                {
                                    for (int i = CountCB - 1; i >= 1; i--)
                                    {

                                        CBcalibre.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                                    }
                                }


                                ComboBox CBcaract = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;

                                CountCB = CBcaract.ValidValues.Count;
                                if (CountCB > 0)
                                {
                                    for (int i = CountCB - 1; i >= 1; i--)
                                    {
                                        CBcaract.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                                    }
                                }

                                var it = CommonFunctions.GET(ServiceLayer.Items, oRS.Fields.Item("ItemCode").Value.ToString(), null, SessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                                CBcalibre = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;
                                //CBcalibre.ValidValues.Add("-", "-");
                                SAPbobsCOM.Recordset oRS2 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                string sSql2 = "SELECT  T0.\"U_DFO_Valor\",T0.\"U_DFO_Descrip\" " +
                                              "FROM \"@DFO_OPDFO\"  T0  " +
                                              "WHERE T0.\"U_DFO_Tipo\" = 'CALIBRE' and T0.\"U_DFO_Descrip\" = '" + it.ForeignName + "' ";
                                oRS2.DoQuery(sSql2);
                                if (oRS2.RecordCount != 0)
                                {
                                    while (!oRS2.EoF)
                                    {
                                        CBcalibre.ValidValues.Add(oRS2.Fields.Item("U_DFO_Valor").Value.ToString(), oRS2.Fields.Item("U_DFO_Descrip").Value.ToString());
                                        oRS2.MoveNext();
                                    }
                                }

                                CBcaract = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                                //CBcaract.ValidValues.Add("-", "-");
                                oRS2 = null;
                                oRS2 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                sSql2 = "SELECT  T0.\"U_DFO_Valor\",T0.\"U_DFO_Descrip\" " +
                                              "FROM \"@DFO_OPDFO\"  T0  " +
                                              "WHERE T0.\"U_DFO_Tipo\" = 'CARACTERISTICA' and T0.\"U_DFO_Descrip\" = '" + it.ForeignName + "' ";
                                oRS2.DoQuery(sSql2);
                                if (oRS2.RecordCount != 0)
                                {
                                    while (!oRS2.EoF)
                                    {
                                        try
                                        {
                                            CBcaract.ValidValues.Add(oRS2.Fields.Item("U_DFO_Valor").Value.ToString(), oRS2.Fields.Item("U_DFO_Descrip").Value.ToString());
                                        }
                                        catch
                                        {

                                        }
                                        oRS2.MoveNext();
                                    }
                                }


                                oEdit.ChooseFromListUID = "CFL1";

                                Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                                grid2.DataTable.Clear();
                                sSql = "SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                "FROM \"@DFO_OCAOF\" T0 " +
                                "WHERE T0.\"U_BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' order by  T0.\"DocEntry\" ";
                                grid2.DataTable.ExecuteQuery(sSql);
                                grid2.SelectionMode = BoMatrixSelect.ms_Single;

                                for (int i = 0; i < grid2.Columns.Count; i++)
                                {
                                    grid2.Columns.Item(i).Editable = false;
                                }

                                //k = 7;
                                //grid2.Columns.Item(k).Editable = true;
                                //k = 8;
                                //grid2.Columns.Item(k).Editable = true;
                                //k = 9;
                                //grid2.Columns.Item(k).Editable = true;

                                gridrowcount = grid2.Rows.Count;
                                for (var i = 1; i <= gridrowcount; i++)
                                {
                                    grid2.CommonSetting.SetRowBackColor(i, -1);

                                    if (!string.IsNullOrEmpty(grid2.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid2.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                    {
                                        grid2.CommonSetting.SetRowEditable(i, false);
                                        grid2.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                    }

                                    if (grid2.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                    {
                                        grid2.CommonSetting.SetRowEditable(i, false);
                                        grid2.CommonSetting.SetRowBackColor(i, Colores.Red);
                                    }
                                }

                                Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                                grid3.DataTable.Clear();
                                grid3.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                "FROM \"@DFO_OCAOF\" T0 " +
                                "WHERE T0.\"U_BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' order by  T0.\"DocEntry\" ");
                                grid3.SelectionMode = BoMatrixSelect.ms_Single;

                                for (int i = 0; i < grid3.Columns.Count; i++)
                                {
                                    grid3.Columns.Item(i).Editable = false;
                                }
                                gridrowcount = grid3.Rows.Count;
                                for (var i = 1; i <= gridrowcount; i++)
                                {
                                    grid3.CommonSetting.SetRowBackColor(i, -1);
                                    if (!string.IsNullOrEmpty(grid3.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid3.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                    {
                                        grid3.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                    }

                                    if (grid3.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                    {
                                        grid3.CommonSetting.SetRowBackColor(i, Colores.Red);
                                    }
                                }

                                Grid grid4 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;

                                grid4.DataTable.Clear();
                                sSql = "SELECT T0.\"DocEntry\", T0.\"ItemCode\", T0.\"BaseQty\", " +
                                "T0.\"PlannedQty\", T0.\"IssuedQty\",  T0.\"wareHouse\" FROM WOR1 T0 " +
                                "WHERE T0.\"DocEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' and T0.\"BaseQty\" < 0";
                                grid4.DataTable.ExecuteQuery(sSql);
                                EditTextColumn oColumns4 = (EditTextColumn)grid4.Columns.Item("DocEntry");
                                oColumns4.Description = "N° OF";
                                oColumns4.LinkedObjectType = "202";
                                oColumns4 = (EditTextColumn)grid4.Columns.Item("ItemCode");
                                oColumns4.Description = "Artículo";
                                oColumns4.LinkedObjectType = "4";
                                grid4.SelectionMode = BoMatrixSelect.ms_Single;
                                for (int i = 0; i < grid4.Columns.Count; i++)
                                {
                                    try
                                    {
                                        grid4.Columns.Item(i).Editable = false;
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception(string.Format("FormLoad {0}", e.Message));
                                    }
                                }

                                Grid grid5 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;

                                grid5.DataTable.Clear();
                                sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                                       " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                                       "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                                       "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                                       "  WHERE T0.\"BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' " +
                                       " and T0.\"ItemCode\" <> '" + oRS.Fields.Item("ItemCode").Value.ToString() + "' " +
                                       " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 ";
                                grid5.DataTable.ExecuteQuery(sSql);
                                EditTextColumn oColumns5 = (EditTextColumn)grid5.Columns.Item("DocEntry");
                                oColumns5.Description = "N° Egreso";
                                oColumns5.LinkedObjectType = "59";
                                oColumns5 = (EditTextColumn)grid5.Columns.Item("AbsEntry");
                                oColumns5.Description = "Interno Lote";
                                oColumns5.LinkedObjectType = "10000044";
                                grid5.SelectionMode = BoMatrixSelect.ms_Single;
                                for (int i = 0; i < grid5.Columns.Count; i++)
                                {
                                    try
                                    {
                                        grid5.Columns.Item(i).Editable = false;
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception(string.Format("FormLoad {0}", e.Message));
                                    }
                                }

                                var response = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                                var calibrado = CommonFunctions.DeserializeList<Calibrado>(response);
                                //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                                TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                                TxtForm.Value = count.ToString();


                                ComboBox Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                                Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);
                                Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;
                                Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);


                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format("{ 0}", e.Message));
                            }
                        }
                    }
                }
            }
        }

        internal static void RightClickEventHandler(ref ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
            if (!eventInfo.BeforeAction)
            {
                //CommonFunctions.AddRightClickMenu(ref sbo_application, UserMenu.DeleteRow, "Borrar Fila", true, BoMenuType.mt_STRING, SAPMenu.RightClickMenu);
                //CommonFunctions.AddRightClickMenu(ref sbo_application, UserMenu.AddRow, "Añadir Fila", true, BoMenuType.mt_STRING, SAPMenu.RightClickMenu);
            }
        }

        internal static void MenuEventHandler(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;

            if (oMenuEvent.BeforeAction)
            {
                Form oForm;
                try { oForm = sbo_application.Forms.ActiveForm as Form; }
                catch { return; }

                if (oForm.TypeEx == pluginForm.FormType)
                {
                    switch (oMenuEvent.MenuUID)
                    {
                        //    case "1292":
                        //        try
                        //        {
                        //            oForm.Freeze(true);

                        //            bBubbleEvent = false;
                        //            var oGrid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;

                        //            int selRow = oGrid.Rows.Count;
                        //            int lastRowIndex = oGrid.DataTable.Rows.Count - 1;
                        //            SAPbouiCOM.DataTable oDT = oGrid.DataTable;
                        //            oDT.Clear();
                        //            oGrid.DataTable.Rows.Add(1);

                        //            //oGrid.DataTable.SetValue("U_BaseEntry", lastRowIndex, valores.NroOF);
                        //        }
                        //        catch (Exception e) { sbo_application.MessageBox(e.Message); }
                        //        finally { oForm.Freeze(false); }
                        //        break;

                        //    case "1293":
                        //        try
                        //        {
                        //            oForm.Freeze(true);
                        //            var oGrid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;
                        //            oGrid.Rows.SelectedRows.Clear();

                        //        }
                        //        catch (Exception e) { sbo_application.MessageBox(e.Message); }
                        //        finally { oForm.Freeze(false); }
                        //        break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void MatrixCalibrado(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK && oItemEvent.ItemUID == pluginForm.GridCalibrado.Uid)
                {
                    bBubbleEvent = true;
                    var oGrid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;
                    // oForm.EnableMenu("1292", true);
                    // oForm.EnableMenu("1293", true);
                }
                //if (oItemEvent.ColUID == pluginForm.MatrixCalibrado.Colums.Col_Formula.Uid && oForm.Mode == BoFormMode.fm_ADD_MODE && (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS || (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN && oItemEvent.CharPressed == 9)))
                //{
                //    var oCheck = (oForm.Items.Item(pluginForm.MatrixCalibrado.Uid).Specific as Matrix).Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Calc.Uid).Cells.Item(oItemEvent.Row).Specific as CheckBox;
                //    if (oCheck.Checked)
                //    {
                //        var oCell = (oForm.Items.Item(pluginForm.MatrixCalibrado.Uid).Specific as Matrix).Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Formula.Uid).Cells.Item(oItemEvent.Row);
                //        var oEdit = oCell.Specific as EditText;
                //        string _Formula = oEdit.Value;

                //        try { _Formula = FormatString(_Formula); }
                //        catch (Exception e) { sbo_application.MessageBox(e.Message); bBubbleEvent = false; oCell.Click(); }
                //        finally { oEdit.Value = _Formula; }
                //    }
                //}
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST && oItemEvent.ColUID == "U_CodEnvase")
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        try
                        {
                            var oGrid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;
                            int i = oItemEvent.Row;
                            oGrid.DataTable.SetValue(oItemEvent.ColUID, i, oDT.GetValue("ItemCode", 0).ToString());
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                }

                //if (oItemEvent.ColUID == pluginForm.MatrixCalibrado.Colums.Col_Attr.Uid && oForm.Mode != BoFormMode.fm_FIND_MODE && (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS || (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN && oItemEvent.CharPressed == 9)))
                //{
                //    var oMatrix = oForm.Items.Item(pluginForm.MatrixCalibrado.Uid).Specific as Matrix;

                //    if (oItemEvent.Row >= oMatrix.RowCount)
                //    {
                //        if (oItemEvent.Row == 1)
                //        {
                //            var oEditText = (SAPbouiCOM.EditText)oMatrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Attr.Uid).Cells.Item(oItemEvent.Row).Specific;

                //            string sr = oEditText.String;

                //            if (oEditText.String != "")
                //            {
                //                oMatrix.AddRow(1, oMatrix.RowCount);
                //                ((SAPbouiCOM.EditText)oMatrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_LineId.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = (oItemEvent.Row + 1).ToString();
                //                ((SAPbouiCOM.EditText)oMatrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Attr.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = string.Empty;
                //            }
                //        }
                //        else
                //        {
                //            var oEditText = (SAPbouiCOM.EditText)oMatrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Attr.Uid).Cells.Item(oItemEvent.Row).Specific;

                //            string sr = oEditText.String;

                //            if (oEditText.String != "")
                //            {
                //                oMatrix.AddRow(1, oItemEvent.Row);
                //                ((SAPbouiCOM.EditText)oMatrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_LineId.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = (oItemEvent.Row + 1).ToString();
                //                ((SAPbouiCOM.EditText)oMatrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Attr.Uid).Cells.Item(oMatrix.RowCount).Specific).Value = string.Empty;
                //            }
                //        }
                //    }
                //}

                //if (oItemEvent.ColUID == pluginForm.MatrixCalibrado.Colums.Col_Father.Uid && oItemEvent.EventType == BoEventTypes.et_CLICK)
                //{
                //    try
                //    {
                //        oForm.Freeze(true);
                //        DBDataSource Det = oForm.DataSources.DBDataSources.Item(pluginForm.dbAttr);

                //        var matrix = oForm.Items.Item(pluginForm.MatrixCalibrado.Uid).Specific as Matrix;
                //        var combo = (SAPbouiCOM.ComboBox)matrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Father.Uid).Cells.Item(oItemEvent.Row).Specific;
                //        var titulos = new Dictionary<int, string>();

                //        try
                //        {
                //            while (combo.ValidValues.Count > 0)
                //                combo.ValidValues.Remove(0, SAPbouiCOM.BoSearchKey.psk_Index);
                //        }
                //        catch { }

                //        for (int i = 1; i < matrix.RowCount; i++)
                //        {
                //            var isnumeric = int.TryParse(((SAPbouiCOM.ComboBox)matrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_TipoFila.Uid).Cells.Item(i).Specific).Value.Trim(), out int n);
                //            if (isnumeric)
                //            {
                //                int tipo = int.Parse(((SAPbouiCOM.ComboBox)matrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_TipoFila.Uid).Cells.Item(i).Specific).Value.Trim());
                //                string title = ((SAPbouiCOM.EditText)matrix.Columns.Item(pluginForm.MatrixCalibrado.Colums.Col_Attr.Uid).Cells.Item(i).Specific).Value.Trim();

                //                if (tipo == 0)
                //                {
                //                    titulos.Add(i, title);
                //                }
                //            }
                //        }

                //        foreach (KeyValuePair<int, string> entry in titulos)
                //        {
                //            try { combo.ValidValues.Add(entry.Value, entry.Key.ToString()); }
                //            catch { };
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        sbo_application.MessageBox(ex.Message);
                //    }
                //    finally
                //    {
                //        oForm.Freeze(false);
                //    }
                //}
            }
        }

        private static void ButtonInsertCal(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    int respuesta = sbo_application.MessageBox("¿Desea actualizar el registro?", 1, "Si", "No");
                    if (respuesta == 1)
                    {
                        var oGrid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;

                        try
                        {
                            int gridrowcount = oGrid.Rows.Count;
                            for (var i = 0; i <= gridrowcount - 1; i++)
                            {
                                string pretarja = "";
                                string fechahora = "";
                                if (string.IsNullOrEmpty(oGrid.DataTable.GetValue("U_IdTarja", i).ToString()))
                                {
                                    if (oGrid.DataTable.GetValue("U_NBoca", i).ToString() != "0")
                                    {

                                        if (!string.IsNullOrEmpty(oGrid.DataTable.GetValue("U_CodEnvase", i).ToString()))
                                        {
                                            DateTime oDate = DateTime.Now;

                                            fechahora = oDate.ToString("yyyyMMddHHmmssfff");
                                            pretarja = fechahora;
                                            var DocEntryCA = oGrid.DataTable.GetValue("DocEntry", i).ToString();
                                            if (DocEntryCA != "0")
                                            {
                                                Calibrado Calibrado = new Calibrado
                                                {
                                                    U_BaseEntry = valores.NroOF,
                                                    U_IdTarja = pretarja,
                                                    U_NBoca = oGrid.DataTable.GetValue("U_NBoca", i).ToString(),
                                                    U_Peso = oGrid.DataTable.GetValue("U_Peso", i).ToString(),
                                                    U_CodEnvase = oGrid.DataTable.GetValue("U_CodEnvase", i).ToString()
                                                };

                                                if (string.IsNullOrEmpty(SessionId))
                                                    SessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                                                var res = CommonFunctions.PATCH(ServiceLayer.Calibrado, Calibrado, DocEntryCA, SessionId, out _);
#if DEBUG
                                                sbo_application.StatusBar.SetText(res, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
#endif
                                            }
                                            else
                                            {
                                                Calibrado Calibrado = new Calibrado
                                                {
                                                    U_BaseEntry = valores.NroOF,
                                                    U_IdTarja = pretarja,
                                                    U_NBoca = oGrid.DataTable.GetValue("U_NBoca", i).ToString(),
                                                    U_Peso = "0",
                                                    U_CodEnvase = oGrid.DataTable.GetValue("U_CodEnvase", i).ToString()
                                                };

                                                if (string.IsNullOrEmpty(SessionId))
                                                    SessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                                                var res = CommonFunctions.POST(ServiceLayer.Calibrado, Calibrado, SessionId, out _);
#if DEBUG
                                                sbo_application.StatusBar.SetText(res, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
#endif
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("Debe ingresar envase utilizado");
                                        }

                                    }
                                    else
                                    {
                                        throw new Exception("Debe ingresar número de boca");
                                    }
                                }
                            }
                            try
                            {
                                Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                grid.DataTable.Clear();
                                grid.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                "FROM \"@DFO_OCAOF\" T0 " +
                                "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                grid.SelectionMode = BoMatrixSelect.ms_Single;

                                for (int j = 0; j < 4; j++)
                                {
                                    grid.Columns.Item(j).Editable = false;
                                }
                                grid.Columns.Item(2).Visible = false;
                                int growcount = grid.Rows.Count;
                                for (var i = 1; i <= growcount; i++)
                                {
                                    grid.CommonSetting.SetRowEditable(i, false);
                                    grid.CommonSetting.SetRowBackColor(i, -1);
                                    if (!string.IsNullOrEmpty(grid.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                    {
                                        grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                    }

                                    if (grid.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                    {
                                        grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                    }
                                }

                                Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                                grid2.DataTable.Clear();
                                grid2.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                "FROM \"@DFO_OCAOF\" T0 " +
                                "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\" ");
                                grid2.SelectionMode = BoMatrixSelect.ms_Single;
                                grid2.Columns.Item(2).Visible = false;
                                for (int i = 0; i < grid2.Columns.Count; i++)
                                {
                                    grid2.Columns.Item(i).Editable = false;
                                }
                                growcount = grid2.Rows.Count;
                                for (var i = 1; i <= growcount; i++)
                                {
                                    //grid.CommonSetting.SetRowEditable(i, false);
                                    grid2.CommonSetting.SetRowBackColor(i, -1);
                                    if (!string.IsNullOrEmpty(grid2.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid2.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                    {
                                        grid2.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                    }

                                    if (grid2.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                    {
                                        grid2.CommonSetting.SetRowBackColor(i, Colores.Red);
                                    }
                                }

                                Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                                grid3.DataTable.Clear();
                                grid3.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                "FROM \"@DFO_OCAOF\" T0 " +
                                "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\" ");
                                grid3.SelectionMode = BoMatrixSelect.ms_Single;
                                grid3.Columns.Item(2).Visible = false;
                                for (int i = 0; i < grid3.Columns.Count; i++)
                                {
                                    grid3.Columns.Item(i).Editable = false;
                                }

                                growcount = grid3.Rows.Count;
                                for (var i = 1; i <= growcount; i++)
                                {
                                    //grid.CommonSetting.SetRowEditable(i, false);
                                    grid3.CommonSetting.SetRowBackColor(i, -1);
                                    if (!string.IsNullOrEmpty(grid3.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid3.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                    {
                                        grid3.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                    }

                                    if (grid3.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                    {
                                        grid3.CommonSetting.SetRowBackColor(i, Colores.Red);
                                    }
                                }

                                grid.DataTable.Rows.Add();
                                grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                            }
                            catch (Exception e)
                            {
                                throw new Exception(string.Format("{ 0}", e.Message));
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(e.Message);
                        }
                    }
                }
                oForm.Mode = BoFormMode.fm_OK_MODE;
            }
        }

        private static void TxtPreTarja(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN)
                {
                    if (oItemEvent.CharPressed == 13)
                    {
                        //sbo_application.StatusBar.SetText(string.Format("ENTER"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);

                        string NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtPreTarja.Uid).Specific).Value.Trim();

                        try
                        {
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                            int gridrowcount = grid.Rows.Count;
                            for (var i = 1; i <= gridrowcount; i++)
                            {
                                if (grid.DataTable.GetValue("U_IdTarja", i - 1).ToString() != "")
                                {
                                    if (grid.DataTable.GetValue("U_IdTarja", i - 1).ToString() == NumeroPT)
                                    {
                                        grid.Rows.SelectedRows.Clear();
                                        grid.Rows.SelectedRows.Add(i - 1);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("{ 0}", e.Message));
                        }
                    }
                }
            }
        }

        private static void GridCalibraAprueba(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK && oItemEvent.ItemUID == pluginForm.GridCalibraAprueba.Uid)
                {
                    bBubbleEvent = true;
                    var oGrid = oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific as Grid;
                    int gridrowcount = oGrid.Rows.Count;
                    for (var i = 0; i <= gridrowcount - 1; i++)
                    {
                        if (oGrid.Rows.IsSelected(i) == true)
                        {
                            //oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                            EditText idTarjaApr = (EditText)oForm.Items.Item(pluginForm.TxtTarjaApr.Uid).Specific;
                            idTarjaApr.Value = oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                        }
                    }
                }
            }
        }

        private static void TxtTarjaApr(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN)
                {
                    if (oItemEvent.CharPressed == 13)
                    {
                       // sbo_application.StatusBar.SetText(string.Format("ENTER"), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

                        string NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtTarjaApr.Uid).Specific).Value.Trim();

                        try
                        {
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                            int gridrowcount = grid.Rows.Count;
                            for (var i = 1; i <= gridrowcount; i++)
                            {
                                if (grid.DataTable.GetValue("U_IdTarja", i - 1).ToString() != "")
                                {
                                    if (grid.DataTable.GetValue("U_IdTarja", i - 1).ToString() == NumeroPT)
                                    {
                                        grid.Rows.SelectedRows.Clear();
                                        grid.Rows.SelectedRows.Add(i - 1);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("{ 0}", e.Message));
                        }
                    }
                }
            }
        }

        private static void GridCalibraPeso(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            var oForm = sbo_application.Forms.Item(formUID);

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK && oItemEvent.ItemUID == pluginForm.GridCalibraPeso.Uid)
                {
                    bBubbleEvent = true;
                    var oGrid = oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific as Grid;
                    int gridrowcount = oGrid.Rows.Count;
                    oForm.Freeze(true);
                    for (var i = 0; i <= gridrowcount - 1; i++)
                    {
                        if (oGrid.Rows.IsSelected(i) == true)
                        {
                            //oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                            EditText idTarjaApr = (EditText)oForm.Items.Item(pluginForm.TxtPreTarja.Uid).Specific;
                            idTarjaApr.Value = oGrid.DataTable.GetValue("U_IdTarja", i).ToString();

                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtMedicion1.Uds).ValueEx = oGrid.DataTable.GetValue("U_Medicion1", i).ToString();
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtMedicion2.Uds).ValueEx = oGrid.DataTable.GetValue("U_Medicion2", i).ToString();
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtMedicion3.Uds).ValueEx = oGrid.DataTable.GetValue("U_Medicion3", i).ToString();

                            if (string.IsNullOrEmpty(oGrid.DataTable.GetValue("U_Estado", i).ToString()))
                            {
                                ((EditText)oForm.Items.Item(pluginForm.TxtPesaje.Uid).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtMedicion1.Uid).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtMedicion2.Uid).Specific).Item.Enabled = true;
                                ((EditText)oForm.Items.Item(pluginForm.TxtMedicion3.Uid).Specific).Item.Enabled = true;
                            }
                            else
                            {
                                ((EditText)oForm.Items.Item(pluginForm.TxtPesaje.Uid).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtMedicion1.Uid).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtMedicion2.Uid).Specific).Item.Enabled = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtMedicion3.Uid).Specific).Item.Enabled = false;
                            }
                        }
                    }
                    oForm.Freeze(false);
                }
            }
        }

        private static void TxtTarjaPeso(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN)
                {
                    if (oItemEvent.CharPressed == 13)
                    {
                       // sbo_application.StatusBar.SetText(string.Format("ENTER"), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);

                        string NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtPreTarja.Uid).Specific).Value.Trim();

                        try
                        {
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                            int gridrowcount = grid.Rows.Count;
                            for (var i = 1; i <= gridrowcount; i++)
                            {
                                if (grid.DataTable.GetValue("U_IdTarja", i - 1).ToString() != "")
                                {
                                    if (grid.DataTable.GetValue("U_IdTarja", i - 1).ToString() == NumeroPT)
                                    {
                                        grid.Rows.SelectedRows.Clear();
                                        grid.Rows.SelectedRows.Add(i - 1);
                                        break;
                                    }
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("{ 0}", e.Message));
                        }
                    }
                }
            }
        }

        private static void ButtonConfCal(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    int respuesta = sbo_application.MessageBox("¿Desea confirmar el recibo de producción?", 1, "Si", "No");
                    if (respuesta == 1)
                    {
                        SAPbobsCOM.UserObjectsMD oUDO = (SAPbobsCOM.UserObjectsMD)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);
                        //oUDO.GetByKey("OCAOF");
                        //oUDO.GetAsXML();
                        //oUDO.SaveXML(@"C:\WF\DFO_OCAOF.XML");
                        //oUDO..SaveXML(@"C:\WF\DFO_OCAOF.XML");
                        var oGrid = oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific as Grid;

                        int gridrowcount = oGrid.Rows.Count;

                        try
                        {
                            gridrowcount = oGrid.Rows.Count;
                            bool DocEntCal = true;
                            bool IdBocaPeso = true;
                            for (var i = 0; i <= gridrowcount - 1; i++)
                            {
                                var DocEntryCA = oGrid.DataTable.GetValue("DocEntry", i).ToString();
                                var IdTarja = oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                                if (DocEntryCA == "0" && IdTarja != "")
                                {
                                    DocEntCal = false;
                                }
                            }

                            if (DocEntCal == true)
                            {
                                for (var i = 0; i <= gridrowcount - 1; i++)
                                {
                                    var DocEntryCA = oGrid.DataTable.GetValue("DocEntry", i).ToString();
                                    if (DocEntryCA != "0")
                                    {
                                        if (oGrid.DataTable.GetValue("U_IdTarja", i).ToString() == "" |
                                            oGrid.DataTable.GetValue("U_NBoca", i).ToString() == "0" |
                                            oGrid.DataTable.GetValue("U_CodEnvase", i).ToString() == "" |
                                            oGrid.DataTable.GetValue("U_Peso", i).ToString() == "0")
                                        {
                                            IdBocaPeso = false;
                                        }
                                    }
                                }
                                if (DocEntCal == true && IdBocaPeso == true)
                                {
                                    for (var i = 0; i <= gridrowcount - 1; i++)
                                    {
                                        if (oGrid.DataTable.GetValue("U_IdTarja", i).ToString() != "" &&
                                            oGrid.DataTable.GetValue("U_NBoca", i).ToString() != "0" &&
                                            oGrid.DataTable.GetValue("U_CodEnvase", i).ToString() != "" &&
                                            oGrid.DataTable.GetValue("U_Peso", i).ToString() != "0")
                                        {
                                            var DocEntryCA = oGrid.DataTable.GetValue("DocEntry", i).ToString();
                                            var U_BaseEntry = oGrid.DataTable.GetValue("U_BaseEntry", i).ToString();
                                            var U_IdTarja = oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                                            var U_Peso = oGrid.DataTable.GetValue("U_Peso", i).ToString();
                                            var U_CodEnvase = oGrid.DataTable.GetValue("U_CodEnvase", i).ToString();
                                            string response = CommonFunctions.ReciboCalibrado("", DocEntryCA, U_BaseEntry, U_IdTarja, U_Peso, "", "", "", "", "", SessionId);
                                        }
                                    }
                                    try
                                    {
                                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                        grid.DataTable.Clear();
                                        grid.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\",T0.\"Remark\", T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                        "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                        "FROM \"@DFO_OCAOF\" T0 " +
                                        "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                        grid.SelectionMode = BoMatrixSelect.ms_Single;
                                        grid.Columns.Item(2).Visible = false;
                                        for (int j = 0; j < 2; j++)
                                        {
                                            grid.Columns.Item(j).Editable = false;
                                        }
                                        int growcount = grid.Rows.Count;
                                        for (var i = 1; i <= growcount; i++)
                                        {
                                            grid.CommonSetting.SetRowEditable(i, false);
                                            grid.CommonSetting.SetRowBackColor(i, -1);
                                            if (!string.IsNullOrEmpty(grid.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                            {
                                                grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                            }

                                            if (grid.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                            {
                                                grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                            }
                                        }
                                        grid.DataTable.Rows.Add();
                                        grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                                    }
                                    catch (Exception e)
                                    {
                                        throw new Exception(string.Format("{ 0}", e.Message));
                                    }

                                    var response1 = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                                    var calibrado = CommonFunctions.DeserializeList<Calibrado>(response1);
                                    //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                    var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                                    TxtForm.Value = count.ToString();
                                }
                                else
                                {
                                    throw new Exception("Verifique que todos los datos hayan sido ingresados (IdTarja, Boca, Peso)");
                                }
                            }
                            else
                            {
                                throw new Exception("Existen registros sin insertar");
                            }
                        }
                        catch (Exception e)
                        {
                            sbo_application.MessageBox(e.Message);
                        }
                    }
                }
            }
        }

        private static void ButtonAprueba(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    int respuesta = sbo_application.MessageBox("¿Desea confirmar el recibo de producción?", 1, "Si", "No");
                    if (respuesta == 1)
                    {
                        var oGrid = oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific as Grid;
                        var DocEntryCA = "";
                        var U_BaseEntry = "";
                        var U_IdTarja = "";
                        var U_Peso = "";
                        var U_CodEnvase = "";
                        var Remark = "";
                        var U_Estado = "";
                        var U_Medicion1 = "";
                        var U_Medicion2 = "";
                        var U_Medicion3 = "";
                        var Variedad = "";
                        var CodProductor = "";
                        var NomProductor = "";
                        double FRU_Conteo = 0;
                        bool IsSelect = false;
                        int gridrowcount = oGrid.Rows.Count;
                        int divConteo = 0;
                        string WhsCode = ((EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific).Value.Trim();

                        for (var i = 0; i <= gridrowcount - 1; i++)
                        {
                            if (oGrid.Rows.IsSelected(i) == true)
                            {
                                DocEntryCA = oGrid.DataTable.GetValue("DocEntry", i).ToString();
                                U_BaseEntry = oGrid.DataTable.GetValue("U_BaseEntry", i).ToString();
                                U_IdTarja = oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                                U_Peso = oGrid.DataTable.GetValue("U_Peso", i).ToString();
                                U_CodEnvase = oGrid.DataTable.GetValue("U_CodEnvase", i).ToString();
                                Remark = oGrid.DataTable.GetValue("Remark", i).ToString();
                                U_Estado = oGrid.DataTable.GetValue("U_Estado", i).ToString();
                                U_Medicion1 = oGrid.DataTable.GetValue("U_Medicion1", i).ToString();
                                U_Medicion2 = oGrid.DataTable.GetValue("U_Medicion2", i).ToString();
                                U_Medicion3 = oGrid.DataTable.GetValue("U_Medicion3", i).ToString();
                                IsSelect = true;
                                break;
                            }
                        }
                        if (IsSelect)
                        {
                            SAPbobsCOM.Recordset oRS6 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            string sSql1 = "Select \"U_BatchNum\" from \"@DFO_RQLTY3\" where (\"U_BatchNum\")   = '" + U_IdTarja + "' ";
                            oRS6.DoQuery(sSql1);
                            if (oRS6.RecordCount != 0)
                            {
                                if (U_Peso != "0")
                                {
                                    if (Remark == "")
                                    {
                                        if (U_Estado == "")
                                        {
                                            if (WhsCode != "")
                                            {
                                                try
                                                {
                                                    if ((U_Medicion1) != "0")
                                                    {
                                                        divConteo = divConteo + 1;
                                                    }
                                                    if ((U_Medicion2) != "0")
                                                    {
                                                        divConteo = divConteo + 1;
                                                    }
                                                    if ((U_Medicion3) != "0")
                                                    {
                                                        divConteo = divConteo + 1;
                                                    }

                                                    if (divConteo > 0)
                                                    {
                                                        FRU_Conteo = Math.Round((double.Parse(U_Medicion1.Replace(".", ",")) + double.Parse(U_Medicion2.Replace(".", ",")) + double.Parse(U_Medicion3.Replace(".", ","))) / divConteo);
                                                    }
                                                    else
                                                    {
                                                    }

                                                    string sSql = "select distinct T4.\"U_FRU_Variedad\",T4.\"U_FRU_Productor\",T4.\"U_FRU_NomProveedor\" from OIGE T0 " +
                                                       "inner join IGE1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" inner join OITM T2 on T1.\"ItemCode\" = T2.\"ItemCode\" " +
                                                       "inner join(select S0.\"DocEntry\", S0.\"DocLine\", S1.\"SysNumber\", -sum(S1.\"Quantity\") as AllocQty from OITL S0 " +
                                                       "inner join ITL1 S1 on S0.\"LogEntry\" = S1.\"LogEntry\" where S0.\"DocType\" = 60 group by S0.\"DocEntry\", S0.\"DocLine\", S1.\"SysNumber\") T3 " +
                                                       "on T1.\"DocEntry\" = T3.\"DocEntry\" and T1.\"LineNum\" = T3.\"DocLine\" inner join OBTN T4 on T3.\"SysNumber\" = T4.\"SysNumber\" " +
                                                       "and T1.\"ItemCode\" = T4.\"ItemCode\" inner join OWOR T5 on T1.\"BaseEntry\" = T5.\"DocEntry\" where T1.\"BaseType\" = 202 " +
                                                       "and T1.\"BaseEntry\" = '" + U_BaseEntry + "' group by T4.\"U_FRU_Variedad\",T4.\"U_FRU_Productor\",T4.\"U_FRU_NomProveedor\"  ";

                                                    var oRS = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                                                    oRS.DoQuery(sSql);
                                                    if (oRS.RecordCount == 1)
                                                    {
                                                        Variedad = oRS.Fields.Item("U_FRU_Variedad").Value.ToString();
                                                        CodProductor = oRS.Fields.Item("U_FRU_Productor").Value.ToString();
                                                        NomProductor = oRS.Fields.Item("U_FRU_NomProveedor").Value.ToString();
                                                    }

                                                    //ReciboCalibrado(string DocEntry, string DocEntryOF, string Tarja, string Peso, string Conteo, string reparo,string Variedad,string CodPro,string NomPro, string SessionId)
                                                    var response = CommonFunctions.ReciboCalibrado(WhsCode, DocEntryCA, U_BaseEntry, U_IdTarja, U_Peso, FRU_Conteo.ToString(), "", Variedad, CodProductor, NomProductor, SessionId).DeserializeJsonToDynamic();

                                                    if (response.DocEntry > 0)
                                                    {
                                                        var rs = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                                                        var rs2 = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                                                        string query = $"select ifnull(\"DocEntry\",0) \"DocEntry\" from \"@DFO_ORQLTY\" where \"U_BaseType\"='4' and \"U_BaseEntry\"={response.DocumentLines[0].BaseEntry.ToString()} order by 1";
                                                        rs.DoQuery(query);

                                                        while (!rs.EoF)
                                                        {
                                                            var query2 = $"delete from \"@DFO_RQLTY4\" where \"DocEntry\"={int.Parse(rs.Fields.Item("DocEntry").Value.ToString())}";
                                                            rs2.DoQuery(query2);
                                                            //CommonFunctions.ActualizarTarjasByMP(U_IdTarja);
                                                            CommonFunctions.ActualizarTotalesPorLote(int.Parse(rs.Fields.Item("DocEntry").Value.ToString()), SessionId);
                                                            //System.Threading.Thread.Sleep(1000);
                                                            rs.MoveNext();
                                                        }
                                                        rs = null;
                                                        rs2 = null;
                                                    }
                                                }
                                                catch
                                                (Exception e)
                                                {
                                                    bBubbleEvent = false;
                                                    throw new Exception(e.Message);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Debe ingresar Bodega de destino");
                                            }
                                        }
                                        else
                                        {
                                            sbo_application.MessageBox("El Lote ya fue rechazado");
                                        }

                                        try
                                        {
                                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                            grid.DataTable.Clear();
                                            grid.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\",T0.\"Remark\", T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                            "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                            "FROM \"@DFO_OCAOF\" T0 " +
                                            "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                            grid.SelectionMode = BoMatrixSelect.ms_Single;
                                            grid.Columns.Item(2).Visible = false;
                                            for (int j = 0; j < 2; j++)
                                            {
                                                grid.Columns.Item(j).Editable = false;
                                            }
                                            int growcount = grid.Rows.Count;
                                            for (var i = 1; i <= growcount; i++)
                                            {
                                                grid.CommonSetting.SetRowEditable(i, false);
                                                grid.CommonSetting.SetRowBackColor(i, -1);
                                                if (!string.IsNullOrEmpty(grid.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                                {
                                                    grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                                }

                                                if (grid.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                                {
                                                    grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                                }
                                            }
                                            string sDocEntry = grid.DataTable.GetValue("DocEntry", grid.DataTable.Rows.Count - 1).ToString();

                                            if (sDocEntry != "0")
                                            {
                                                grid.DataTable.Rows.Add();
                                                grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                                            }

                                            Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                                            grid2.DataTable.Clear();
                                            grid2.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                            "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                            "FROM \"@DFO_OCAOF\" T0 " +
                                            "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                            grid2.SelectionMode = BoMatrixSelect.ms_Single;
                                            grid2.Columns.Item(2).Visible = false;
                                            for (int i = 0; i < grid2.Columns.Count; i++)
                                            {
                                                grid2.Columns.Item(i).Editable = false;
                                            }
                                            growcount = grid2.Rows.Count;
                                            for (var i = 1; i <= growcount; i++)
                                            {
                                                //grid.CommonSetting.SetRowEditable(i, false);
                                                grid2.CommonSetting.SetRowBackColor(i, -1);
                                                if (!string.IsNullOrEmpty(grid2.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid2.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                                {
                                                    grid2.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                                }

                                                if (grid2.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                                {
                                                    grid2.CommonSetting.SetRowBackColor(i, Colores.Red);
                                                }
                                            }

                                            Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                                            grid3.DataTable.Clear();
                                            grid3.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                            "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                            "FROM \"@DFO_OCAOF\" T0 " +
                                            "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                            grid3.SelectionMode = BoMatrixSelect.ms_Single;
                                            grid3.Columns.Item(2).Visible = false;

                                            for (int i = 0; i < grid3.Columns.Count; i++)
                                            {
                                                grid3.Columns.Item(i).Editable = false;
                                            }
                                            growcount = grid3.Rows.Count;
                                            for (var i = 1; i <= growcount; i++)
                                            {
                                                //grid.CommonSetting.SetRowEditable(i, false);
                                                grid3.CommonSetting.SetRowBackColor(i, -1);
                                                if (!string.IsNullOrEmpty(grid3.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid3.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                                {
                                                    grid3.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                                }

                                                if (grid3.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                                {
                                                    grid3.CommonSetting.SetRowBackColor(i, Colores.Red);
                                                }
                                            }

                                            var response = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                                            var calibrado = CommonFunctions.DeserializeList<Calibrado>(response);
                                            //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                            var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                                            EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                                            TxtForm.Value = count.ToString();
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception(string.Format("{0}", e.Message));
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("El Lote ya fue aprobado");
                                    }
                                }
                                else
                                {
                                    throw new Exception("El Peso no puede ser igual a 0");
                                }
                            }
                            else
                            {
                                throw new Exception("El lote no cuenta con calidad asociada");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un lote");
                        }
                    }
                }
            }
        }

        private static void ButtonAprReparo(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    int respuesta = sbo_application.MessageBox("¿Desea confirmar el recibo de producción con reparos?", 1, "Si", "No");
                    if (respuesta == 1)
                    {
                        var oGrid = oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific as Grid;
                        var DocEntryCA = "";
                        var U_BaseEntry = "";
                        var U_IdTarja = "";
                        var U_Peso = "";
                        var U_CodEnvase = "";
                        var Remark = "";
                        var U_Estado = "";
                        int gridrowcount = oGrid.Rows.Count;
                        var U_Medicion1 = "";
                        var U_Medicion2 = "";
                        var U_Medicion3 = "";
                        var Variedad = "";
                        var CodProductor = "";
                        var NomProductor = "";
                        double FRU_Conteo = 0;
                        bool IsSelect = false;
                        int divConteo = 0;
                        string WhsCode = ((EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific).Value.Trim();

                        for (var i = 0; i <= gridrowcount - 1; i++)
                        {
                            if (oGrid.Rows.IsSelected(i) == true)
                            {
                                DocEntryCA = oGrid.DataTable.GetValue("DocEntry", i).ToString();
                                U_BaseEntry = oGrid.DataTable.GetValue("U_BaseEntry", i).ToString();
                                U_IdTarja = oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                                U_Peso = oGrid.DataTable.GetValue("U_Peso", i).ToString();
                                U_CodEnvase = oGrid.DataTable.GetValue("U_CodEnvase", i).ToString();
                                Remark = oGrid.DataTable.GetValue("Remark", i).ToString();
                                U_Estado = oGrid.DataTable.GetValue("U_Estado", i).ToString();
                                U_Medicion1 = oGrid.DataTable.GetValue("U_Medicion1", i).ToString();
                                U_Medicion2 = oGrid.DataTable.GetValue("U_Medicion2", i).ToString();
                                U_Medicion3 = oGrid.DataTable.GetValue("U_Medicion3", i).ToString();
                                IsSelect = true;

                                break;
                            }
                        }
                        if (IsSelect)
                        {
                            SAPbobsCOM.Recordset oRS6 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            string sSql1 = "Select \"U_BatchNum\" from \"@DFO_RQLTY3\" where (\"U_BatchNum\")   = '" + U_IdTarja + "' ";
                            oRS6.DoQuery(sSql1);
                            if (oRS6.RecordCount != 0)
                            {
                                if (U_Peso != "0")
                                {
                                    if (Remark == "")
                                    {
                                        if (U_Estado == "")
                                        {
                                            if (WhsCode != "")
                                            {
                                                try
                                                {
                                                    if ((U_Medicion1) != "0")
                                                    {
                                                        divConteo = divConteo + 1;
                                                    }
                                                    if ((U_Medicion2) != "0")
                                                    {
                                                        divConteo = divConteo + 1;
                                                    }
                                                    if ((U_Medicion3) != "0")
                                                    {
                                                        divConteo = divConteo + 1;
                                                    }

                                                    if (divConteo > 0)
                                                    {
                                                        FRU_Conteo = Math.Round((double.Parse(U_Medicion1.Replace(".", ",")) + double.Parse(U_Medicion2.Replace(".", ",")) + double.Parse(U_Medicion3.Replace(".", ","))) / divConteo);
                                                    }
                                                    else
                                                    {
                                                    }

                                                    string sSql = "select distinct T4.\"U_FRU_Variedad\",T4.\"U_FRU_Productor\",T4.\"U_FRU_NomProveedor\" from OIGE T0 " +
                                                       "inner join IGE1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" inner join OITM T2 on T1.\"ItemCode\" = T2.\"ItemCode\" " +
                                                       "inner join(select S0.\"DocEntry\", S0.\"DocLine\", S1.\"SysNumber\", -sum(S1.\"Quantity\") as AllocQty from OITL S0 " +
                                                       "inner join ITL1 S1 on S0.\"LogEntry\" = S1.\"LogEntry\" where S0.\"DocType\" = 60 group by S0.\"DocEntry\", S0.\"DocLine\", S1.\"SysNumber\") T3 " +
                                                       "on T1.\"DocEntry\" = T3.\"DocEntry\" and T1.\"LineNum\" = T3.\"DocLine\" inner join OBTN T4 on T3.\"SysNumber\" = T4.\"SysNumber\" " +
                                                       "and T1.\"ItemCode\" = T4.\"ItemCode\" inner join OWOR T5 on T1.\"BaseEntry\" = T5.\"DocEntry\" where T1.\"BaseType\" = 202 " +
                                                       "and T1.\"BaseEntry\" = '" + U_BaseEntry + "' group by T4.\"U_FRU_Variedad\",T4.\"U_FRU_Productor\",T4.\"U_FRU_NomProveedor\"  ";

                                                    var oRS = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                                                    oRS.DoQuery(sSql);
                                                    if (oRS.RecordCount == 1)
                                                    {
                                                        Variedad = oRS.Fields.Item("U_FRU_Variedad").Value.ToString();
                                                        CodProductor = oRS.Fields.Item("U_FRU_Productor").Value.ToString();
                                                        NomProductor = oRS.Fields.Item("U_FRU_NomProveedor").Value.ToString();
                                                    }

                                                    //ReciboCalibrado(string DocEntry, string DocEntryOF, string Tarja, string Peso, string Conteo, string reparo,string Variedad,string CodPro,string NomPro, string SessionId)
                                                    var response = CommonFunctions.ReciboCalibrado(WhsCode, DocEntryCA, U_BaseEntry, U_IdTarja, U_Peso, FRU_Conteo.ToString(), "", Variedad, CodProductor, NomProductor, SessionId).DeserializeJsonToDynamic();
                                                    if (response.DocEntry > 0)
                                                    {
                                                        var rs = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;
                                                        var rs2 = sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset) as SAPbobsCOM.Recordset;

                                                        string query = $"select ifnull(\"DocEntry\",0) \"DocEntry\" from \"@DFO_ORQLTY\" where \"U_BaseType\"='4' and \"U_BaseEntry\"={response.DocumentLines[0].BaseEntry.ToString()} order by 1";
                                                        rs.DoQuery(query);

                                                        while (!rs.EoF)
                                                        {
                                                            var query2 = $"delete from \"@DFO_RQLTY4\" where \"DocEntry\"={int.Parse(rs.Fields.Item("DocEntry").Value.ToString())}";
                                                            rs2.DoQuery(query2);

                                                            CommonFunctions.ActualizarTotalesPorLote(int.Parse(rs.Fields.Item("DocEntry").Value.ToString()), SessionId);
                                                            //System.Threading.Thread.Sleep(1000);
                                                            rs.MoveNext();
                                                        }
                                                        rs = null;
                                                        rs2 = null;
                                                    }
                                                }
                                                catch
                                                (Exception e)
                                                {
                                                    bBubbleEvent = false;
                                                    throw new Exception(e.Message);
                                                }
                                            }
                                            else
                                            {
                                                throw new Exception("Debe ingresar Bodega de destino");
                                            }
                                        }
                                        else
                                        {
                                            throw new Exception("El Lote ya fue rechazado");
                                        }

                                        try
                                        {
                                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                            grid.DataTable.Clear();
                                            grid.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\",T0.\"Remark\", T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                            "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                            "FROM \"@DFO_OCAOF\" T0 " +
                                            "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                            grid.SelectionMode = BoMatrixSelect.ms_Single;
                                            grid.Columns.Item(2).Visible = false;
                                            for (int j = 0; j < 2; j++)
                                            {
                                                grid.Columns.Item(j).Editable = false;
                                            }
                                            int growcount = grid.Rows.Count;
                                            for (var i = 1; i <= growcount; i++)
                                            {
                                                grid.CommonSetting.SetRowEditable(i, false);
                                                grid.CommonSetting.SetRowBackColor(i, -1);

                                                if (!string.IsNullOrEmpty(grid.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                                {
                                                    grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                                }

                                                if (grid.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                                {
                                                    grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                                }
                                            }
                                            string sDocEntry = grid.DataTable.GetValue("DocEntry", grid.DataTable.Rows.Count - 1).ToString();

                                            if (sDocEntry != "0")
                                            {
                                                grid.DataTable.Rows.Add();
                                                grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                                            }

                                            Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                                            grid2.DataTable.Clear();
                                            grid2.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                            "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                            "FROM \"@DFO_OCAOF\" T0 " +
                                            "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                            grid2.SelectionMode = BoMatrixSelect.ms_Single;
                                            grid2.Columns.Item(2).Visible = false;
                                            for (int i = 0; i < grid2.Columns.Count; i++)
                                            {
                                                grid2.Columns.Item(i).Editable = false;
                                            }
                                            growcount = grid2.Rows.Count;
                                            for (var i = 1; i <= growcount; i++)
                                            {
                                                //grid.CommonSetting.SetRowEditable(i, false);
                                                grid2.CommonSetting.SetRowBackColor(i, -1);
                                                if (!string.IsNullOrEmpty(grid2.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid2.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                                {
                                                    grid2.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                                }

                                                if (grid2.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                                {
                                                    grid2.CommonSetting.SetRowBackColor(i, Colores.Red);
                                                }
                                            }

                                            Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                                            grid3.DataTable.Clear();
                                            grid3.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                            "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                            "FROM \"@DFO_OCAOF\" T0 " +
                                            "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\"");
                                            grid3.SelectionMode = BoMatrixSelect.ms_Single;
                                            grid3.Columns.Item(2).Visible = false;

                                            for (int i = 0; i < grid3.Columns.Count; i++)
                                            {
                                                grid3.Columns.Item(i).Editable = false;
                                            }
                                            growcount = grid3.Rows.Count;
                                            for (var i = 1; i <= growcount; i++)
                                            {
                                                //grid.CommonSetting.SetRowEditable(i, false);
                                                grid3.CommonSetting.SetRowBackColor(i, -1);
                                                if (!string.IsNullOrEmpty(grid3.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid3.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                                {
                                                    grid3.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                                }

                                                if (grid3.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                                {
                                                    grid3.CommonSetting.SetRowBackColor(i, Colores.Red);
                                                }
                                            }

                                            var response = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                                            var calibrado = CommonFunctions.DeserializeList<Calibrado>(response);
                                            //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                            var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                                            EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                                            TxtForm.Value = count.ToString();
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception(string.Format("{ 0}", e.Message));
                                        }
                                    }
                                    else
                                    {
                                        throw new Exception("El Lote ya fue aprobado");
                                    }
                                }
                                else
                                {
                                    throw new Exception("El Peso no puede ser igual a 0");
                                }
                            }
                            else
                            {
                                throw new Exception("El lote no cuenta con calidad asociada");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un lote");
                        }
                    }
                }
            }
        }

        private static void ButtonAsignPeso(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    string NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtPreTarja.Uid).Specific).Value.Trim();
                    int respuesta = sbo_application.MessageBox("¿Desea actualizar el registro '" + NumeroPT + "' ?", 1, "Si", "No");
                    if (respuesta == 1)
                    {
                        NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtPreTarja.Uid).Specific).Value.Trim();
                        string Peso = ((EditText)oForm.Items.Item(pluginForm.TxtPesaje.Uid).Specific).Value.Trim();

                        var oGrid = oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific as Grid;
                        int gridrow = oGrid.Rows.Count;
                        double pesoEnv = 0;
                        double Medicion1 = 0;
                        double Medicion2 = 0;
                        double Medicion3 = 0;

                        bool IsSelected = false;

                        for (var i = 0; i <= gridrow - 1; i++)
                        {
                            if (oGrid.Rows.IsSelected(i) == true)
                            {
                                //oGrid.DataTable.GetValue("U_IdTarja", i).ToString();
                                string EnvItemCode = oGrid.DataTable.GetValue("U_CodEnvase", i).ToString();

                                string args = string.Format("?$filter=ItemCode eq '{0}'", EnvItemCode);// docentry corresponde a numerico, el argumento no va entre comillas
                                var response = CommonFunctions.GET(ServiceLayer.Items, null, args, SessionId, out _);
                                CoreUtilities.Items item = CommonFunctions.DeserializeJsonObject<CoreUtilities.Items>(response);
                                if (item != null)
                                {
                                    pesoEnv = double.Parse((item.SalesUnitWeight.Value.ToString()).Replace(".", ","));
                                }
                                Medicion1 = double.Parse((((EditText)oForm.Items.Item(pluginForm.TxtMedicion1.Uid).Specific).Value.Trim()).Replace(".", ","));
                                Medicion2 = double.Parse((((EditText)oForm.Items.Item(pluginForm.TxtMedicion2.Uid).Specific).Value.Trim()).Replace(".", ","));
                                Medicion3 = double.Parse((((EditText)oForm.Items.Item(pluginForm.TxtMedicion3.Uid).Specific).Value.Trim()).Replace(".", ","));
                                IsSelected = true;
                            }
                        }

                        if (IsSelected)
                        {
                            Peso = (double.Parse(Peso.Replace(".", ",")) - pesoEnv).ToString();

                            var OCAOF = CommonFunctions.DeserializeJsonObject<Calibrado>(CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_IdTarja eq '{NumeroPT}'", SessionId, out _));// and U_Peso eq 0", SessionId));

                            if (OCAOF != null)
                            {
                                if (OCAOF.U_Estado == null)
                                {
                                    OCAOF.U_Peso = Peso;
                                    OCAOF.U_Medicion1 = Medicion1;
                                    OCAOF.U_Medicion2 = Medicion2;
                                    OCAOF.U_Medicion3 = Medicion3;

                                    CommonFunctions.PATCH(ServiceLayer.Calibrado, OCAOF, OCAOF.DocEntry, SessionId, out _);
                                }
                                else
                                {
                                    throw new Exception("El Lote ya fue cerrado");
                                }

                                try
                                {
                                    SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                    //SAPbouiCOM.DataTable oDT = grid.DataTable;
                                    grid.DataTable.Clear();

                                    string sql = "SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                    "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                    "FROM \"@DFO_OCAOF\" T0 " +
                                    "WHERE T0.\"U_BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' order by  T0.\"DocEntry\" ";

                                    grid.DataTable.ExecuteQuery(sql);
                                    grid.SelectionMode = BoMatrixSelect.ms_Single;
                                    grid.Columns.Item(2).Visible = false;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        grid.Columns.Item(i).Editable = false;
                                    }

                                    int gridrowcount = grid.Rows.Count;
                                    for (var i = 1; i <= gridrowcount; i++)
                                    {
                                        grid.CommonSetting.SetRowEditable(i, false);

                                        grid.CommonSetting.SetRowBackColor(i, -1);

                                        if (!string.IsNullOrEmpty(grid.DataTable.GetValue("Remark", i - 1).ToString()))//if (grid.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                        {
                                            grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                        }

                                        if (grid.DataTable.GetValue("U_Estado", i - 1).ToString() == "R")
                                        {
                                            grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                        }
                                    }
                                    string sDocEntry = grid.DataTable.GetValue("DocEntry", grid.DataTable.Rows.Count - 1).ToString();

                                    if (sDocEntry != "0")
                                    {
                                        grid.DataTable.Rows.Add();
                                        grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                                    }

                                    Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                                    grid2.DataTable.Clear();
                                    grid2.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                    "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                    "FROM \"@DFO_OCAOF\" T0 " +
                                    "WHERE T0.\"U_BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' order by  T0.\"DocEntry\" ");
                                    grid2.SelectionMode = BoMatrixSelect.ms_Single;

                                    Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                                    grid3.DataTable.Clear();
                                    grid3.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                    "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                    "FROM \"@DFO_OCAOF\" T0 " +
                                    "WHERE T0.\"U_BaseEntry\" = '" + oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx + "' order by  T0.\"DocEntry\" ");
                                    grid3.SelectionMode = BoMatrixSelect.ms_Single;
                                    grid3.Columns.Item(2).Visible = false;

                                    var response = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                                    var calibrado = CommonFunctions.DeserializeList<Calibrado>(response);
                                    //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                    var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                                    TxtForm.Value = count.ToString();
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(string.Format("{ 0}", e.Message));
                                }
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un registro");
                        }
                    }
                }
            }
        }

        private static void ButtonRechazo(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    string NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtTarjaApr.Uid).Specific).Value.Trim();
                    if (NumeroPT != "")
                    {
                        int respuesta = sbo_application.MessageBox("¿Desea Rechazar el lote? '" + NumeroPT + "' ?", 1, "Si", "No");
                        if (respuesta == 1)
                        {
                            NumeroPT = ((EditText)oForm.Items.Item(pluginForm.TxtTarjaApr.Uid).Specific).Value.Trim();
                            string Peso = ((EditText)oForm.Items.Item(pluginForm.TxtPesaje.Uid).Specific).Value.Trim();
                            var OCAOF = CommonFunctions.DeserializeJsonObject<Calibrado>(CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_IdTarja eq '{NumeroPT}'", SessionId, out _));// and U_Peso eq 0", SessionId));

                            if (OCAOF != null)
                            {
                                var remark = OCAOF.Remark;
                                if (OCAOF.Remark == null)
                                {
                                    if (OCAOF.U_Estado == null)
                                    {
                                        OCAOF.U_Estado = "R";
                                        CommonFunctions.PATCH(ServiceLayer.Calibrado, OCAOF, OCAOF.DocEntry, SessionId, out _);
                                    }
                                    else
                                    {
                                        sbo_application.MessageBox("El Lote ya fue rechazado");
                                        bBubbleEvent = false;
                                    }
                                }
                                else
                                {
                                    sbo_application.MessageBox("El Lote ya fue aprobado");
                                    bBubbleEvent = false;
                                }

                                try
                                {
                                    SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                    Columns columns = (Columns)oForm.Items.Item(pluginForm.GridCalibrado.Uid).Specific;
                                    //SAPbouiCOM.DataTable oDT = grid.DataTable;
                                    grid.DataTable.Clear();
                                    grid.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                    "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                    "FROM \"@DFO_OCAOF\" T0 " +
                                    "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\" ");
                                    grid.SelectionMode = BoMatrixSelect.ms_Single;
                                    grid.Columns.Item(2).Visible = false;
                                    for (int i = 0; i < 4; i++)
                                    {
                                        grid.Columns.Item(i).Editable = false;
                                    }

                                    int gridrowcount = grid.Rows.Count;
                                    for (var i = 1; i <= gridrowcount; i++)
                                    {
                                        if (grid.DataTable.GetValue("Remark", i - 1).ToString() != "")
                                        {
                                            grid.CommonSetting.SetRowEditable(i, false);
                                        }
                                    }
                                    string sDocEntry = grid.DataTable.GetValue("DocEntry", grid.DataTable.Rows.Count - 1).ToString();

                                    if (sDocEntry != "0")
                                    {
                                        grid.DataTable.Rows.Add();
                                        grid.CommonSetting.SetRowBackColor(grid.Rows.Count, -1);
                                    }

                                    Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridCalibraPeso.Uid).Specific;
                                    grid2.DataTable.Clear();
                                    grid2.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                    "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                    "FROM \"@DFO_OCAOF\" T0 " +
                                    "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\" ");
                                    grid2.SelectionMode = BoMatrixSelect.ms_Single;
                                    grid2.Columns.Item(2).Visible = false;

                                    Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                                    grid3.DataTable.Clear();
                                    grid3.DataTable.ExecuteQuery("SELECT  T0.\"DocEntry\", T0.\"Remark\",T0.\"U_BaseEntry\",T0.\"U_IdTarja\",T0.\"U_NBoca\", " +
                                    "T0.\"U_Peso\",T0.\"U_CodEnvase\",T0.\"U_Medicion1\",T0.\"U_Medicion2\",T0.\"U_Medicion3\",T0.\"U_Estado\",T0.\"U_Comentario\" " +
                                    "FROM \"@DFO_OCAOF\" T0 " +
                                    "WHERE T0.\"U_BaseEntry\" = '" + valores.NroOF + "' order by  T0.\"DocEntry\" ");
                                    grid3.SelectionMode = BoMatrixSelect.ms_Single;
                                    grid3.Columns.Item(2).Visible = false;

                                    var response = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                                    var calibrado = CommonFunctions.DeserializeList<Calibrado>(response);
                                    //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                    var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                                    TxtForm.Value = count.ToString();
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(string.Format("{ 0}", e.Message));
                                }
                            }
                        }
                    }
                    else
                    {
                        sbo_application.MessageBox("Debe seleccionar un registro");
                    }
                }
            }
        }

        private static void ButtonAsigLoteSP(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    string DocNumOF = valores.NroOF; //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroOrden.Uid).Specific).Value.Trim();
                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;
                    DataTable oDT = oForm.DataSources.DataTables.Item(pluginForm.GridSubProd.Dt);
                    string Peso = ((EditText)oForm.Items.Item(pluginForm.TxtPesoSubProd.Uid).Specific).Value.Trim();
                    string ItemCode1 = ((EditText)oForm.Items.Item(pluginForm.TxtEnvase.Uid).Specific).Value.Trim();
                    string ItemCodeSE = ((EditText)oForm.Items.Item(pluginForm.TxtItemCode.Uid).Specific).Value.Trim();
                    string WhsCode = ((EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific).Value.Trim();
                    string Caract = ((ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific).Selected.Value;
                    string Calibre = ((ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific).Selected.Value;

                    var item = CommonFunctions.GET(ServiceLayer.Items, ItemCode1, null, SessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();
                    if (item.SalesUnitWeight > 0)
                    {
                        if (grid.Rows.SelectedRows.Count > 0)
                        {
                            if (double.Parse(Peso.Replace(".", ",")) != 0)
                            {
                                if (WhsCode != "")
                                {
                                    if (Caract == "-")
                                    {
                                        Caract = "";

                                    }

                                    if (Calibre == "-")
                                    {
                                        Calibre = "";
                                    }

                                    double peso = double.Parse(Peso.Replace(".", ",")) - (double)item.SalesUnitWeight;
                                    int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                                    string ItemCode = grid.DataTable.GetValue("ItemCode", row).ToString();
                                    SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    string sSql1 = "SELECT T0.\"ItemCode\",T0.\"LineNum\" " +
                                                    "FROM WOR1 T0  " +
                                                    "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"ItemCode\" = '" + ItemCode + "' ";
                                    oRS.DoQuery(sSql1);
                                    if (oRS.RecordCount != 0)
                                    {
                                        string LineNum = oRS.Fields.Item("LineNum").Value.ToString();
                                        DateTime date = DateTime.Now;
                                        string fecha = date.ToString("yyyyMMddHHmmssfff");
                                        string responseRP = SAPFunctions.ReciboProduccion(WhsCode, fecha, LineNum, peso.ToString(), "", "", DocNumOF, Calibre, Caract, sbo_company, SessionId);
                                        sbo_application.StatusBar.SetText(responseRP, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                        string responseCPT = CommonFunctions.ConsumoProductoTerminado(fecha, ItemCode, "", "", DocNumOF, SessionId);

                                        ComboBox Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                                        Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);
                                        Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;
                                        Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);
                                    }

                                }
                                else
                                {
                                    throw new Exception("Debe ingresar una bodega de destino");
                                }
                            }
                            else
                            {
                                throw new Exception("El Peso no puede ser igual a 0");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un artículo");
                        }
                    }
                    else
                    {
                    }
                    //if (grid.Rows.SelectedRows.Count > 0)
                    //{
                    //    if (double.Parse(Peso.Replace(".", ",")) != 0)
                    //    {
                    //        int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                    //        string ItemCode = grid.DataTable.GetValue("ItemCode", row).ToString();
                    //        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                    //        string sSql1 = "SELECT T0.\"ItemCode\",T0.\"LineNum\" " +
                    //                        "FROM WOR1 T0  " +
                    //                        "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"ItemCode\" = '" + ItemCode + "' ";
                    //        oRS.DoQuery(sSql1);
                    //        if (oRS.RecordCount != 0)
                    //        {
                    //            string LineNum = oRS.Fields.Item("LineNum").Value.ToString();
                    //            DateTime date = DateTime.Now;
                    //            string fecha = date.ToString("yyyyMMddHHmmssfff");
                    //            string responseCPT = CommonFunctions.ConsumoProductoTerminado(DocNumOF + fecha, ItemCode, "", "",  DocNumOF, SessionId);

                    //            string responseRP = SAPFunctions.ReciboProduccion("",DocNumOF + fecha, LineNum, Peso, "", "", DocNumOF, sbo_company, SessionId);
                    //            sbo_application.StatusBar.SetText(responseRP, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                    //        }
                    //    }
                    //    else
                    //    {
                    //        throw new Exception("El Peso no puede ser igual a 0");
                    //    }
                    //}
                    //else
                    //{
                    //    throw new Exception("Debe seleccionar un artículo");
                    //}

                    Grid grid4 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;

                    Grid grid5 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;

                    grid4.DataTable.Clear();
                    string sSql = "SELECT T0.\"DocEntry\", T0.\"ItemCode\", T0.\"BaseQty\", " +
                    "T0.\"PlannedQty\", T0.\"IssuedQty\",  T0.\"wareHouse\" FROM WOR1 T0 " +
                    "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"BaseQty\" < 0";
                    grid4.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns4 = (EditTextColumn)grid4.Columns.Item("DocEntry");
                    oColumns4.Description = "N° OF";
                    oColumns4.LinkedObjectType = "202";
                    oColumns4 = (EditTextColumn)grid4.Columns.Item("ItemCode");
                    oColumns4.Description = "Artículo";
                    oColumns4.LinkedObjectType = "4";
                    grid4.SelectionMode = BoMatrixSelect.ms_Single;
                    for (int i = 0; i < grid4.Columns.Count; i++)
                    {
                        try
                        {
                            grid4.Columns.Item(i).Editable = false;
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("FormLoad {0}", e.Message));
                        }
                    }

                    grid5.DataTable.Clear();
                    sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                           " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                           "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                           "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                           "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                           " and T0.\"ItemCode\" <> '" + ItemCodeSE + "' " +
                           " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 ";
                    grid5.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns5 = (EditTextColumn)grid5.Columns.Item("DocEntry");
                    oColumns5.Description = "N° Egreso";
                    oColumns5.LinkedObjectType = "59";
                    oColumns5 = (EditTextColumn)grid5.Columns.Item("AbsEntry");
                    oColumns5.Description = "Interno Lote";
                    oColumns5.LinkedObjectType = "10000044";
                    grid5.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid5.Columns.Count; i++)
                    {
                        try
                        {
                            grid5.Columns.Item(i).Editable = false;
                        }
                        catch (Exception e)
                        {
                            throw new Exception(string.Format("FormLoad {0}", e.Message));
                        }
                    }
                    var response = CommonFunctions.GET(ServiceLayer.Calibrado, null, $"?$filter=U_BaseEntry eq {oForm.DataSources.UserDataSources.Item("UDEntry").ValueEx}", SessionId, out _);
                    var calibrado = CommonFunctions.DeserializeList<Calibrado>(response);
                    //var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                    var count = calibrado.Where(i => i.U_Estado == "A").Sum(i => double.Parse(i.U_Peso.Replace(".", ",")));

                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtSumAprobado.Uid).Specific;
                    TxtForm.Value = count.ToString();
                }
            }
        }

        private static void ButtonPreviewTarja(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
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
                    string U_IdTarja = "";
                    if (oForm.PaneLevel == 3)
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridCalibraAprueba.Uid).Specific;
                        int gridrowcount = grid.Rows.Count;
                        for (var i = 0; i <= gridrowcount - 1; i++)
                        {
                            if (grid.Rows.IsSelected(i))
                            {
                                U_IdTarja = grid.DataTable.GetValue("U_IdTarja", i).ToString();
                                break;
                            }
                        }
                        if (U_IdTarja != "")
                        {
                            var LoteObj = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{U_IdTarja}'", SessionId, out _).DeserializeJsonObject<BatchNumberDetails>();
                            if (LoteObj != null)
                            {
                                SAPFunctions.PrintLayout("BTN10003", (int)LoteObj.DocEntry, sbo_company);
                            }
                            else
                            {
                                throw new Exception("El lote no se encuentra en existencia");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un registro");
                        }
                    }
                    if (oForm.PaneLevel == 4)
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;
                        int gridrowcount = grid.Rows.Count;
                        for (var i = 0; i <= gridrowcount - 1; i++)
                        {
                            if (grid.Rows.IsSelected(i))
                            {
                                U_IdTarja = grid.DataTable.GetValue("DistNumber", i).ToString();
                                break;
                            }
                        }
                        if (U_IdTarja != "")
                        {
                            var LoteObj = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{U_IdTarja}'", SessionId, out _).DeserializeJsonObject<BatchNumberDetails>();
                            if (LoteObj != null)
                            {
                                SAPFunctions.PrintLayout("BTN10003", (int)LoteObj.DocEntry, sbo_company);
                            }
                            else
                            {
                                throw new Exception("El lote no se encuentra en existencia");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe seleccionar un registro");
                        }
                    }
                }
            }
        }

        private static void TxtBodegaDest(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
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
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtBodegaDest.Uds).ValueEx = oDT.GetValue("WhsCode", 0).ToString();
                    }
                }
            }
        }

        private static void FolderAprueba(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    //EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific;
                    //TxtForm.Item.Visible = false;

                    //StaticText Static = (StaticText)oForm.Items.Item("Item_11").Specific;
                    //Static.Item.Visible = false;
                    oForm.Freeze(true);
                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific;
                    TxtForm.Item.Left = 593;
                    TxtForm.Item.Top = 179;

                    StaticText StaticForm = (StaticText)oForm.Items.Item("Item_11").Specific;
                    StaticForm.Item.Left = 505;
                    StaticForm.Item.Top = 179;
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific;
                    TxtForm.Item.Visible = true;

                    StaticText Static = (StaticText)oForm.Items.Item("Item_11").Specific;
                    Static.Item.Visible = true;

                    oForm.Freeze(false);
                }
            }
        }

        private static void FolderApruebaSP(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    //EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific;
                    //TxtForm.Item.Visible = false;

                    //StaticText Static = (StaticText)oForm.Items.Item("Item_11").Specific;
                    //Static.Item.Visible = false;
                    oForm.Freeze(true);
                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific;
                    TxtForm.Item.Left = 672;
                    TxtForm.Item.Top = 215;

                    StaticText StaticForm = (StaticText)oForm.Items.Item("Item_11").Specific;
                    StaticForm.Item.Left = 582;
                    StaticForm.Item.Top = 215;
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDest.Uid).Specific;
                    TxtForm.Item.Visible = true;

                    StaticText Static = (StaticText)oForm.Items.Item("Item_11").Specific;
                    Static.Item.Visible = true;

                    oForm.Freeze(false);
                }
            }
        }
    }
}