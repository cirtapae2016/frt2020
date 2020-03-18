using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Xml;

namespace pluginPrdMP

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
                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;

                    FormCreationPackage.UniqueID = pluginForm.FormType + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    FormCreationPackage.UniqueID = "ConsumoMP" + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    oForm.Mode = BoFormMode.fm_OK_MODE;

                    for (int i = 3; i < oForm.Items.Count; i++)
                    {
                        oForm.Items.Item(i).AffectsFormMode = false;
                    }
                    // oForm.DataBrowser.BrowseBy = pluginForm.TxtNroOrden.Uid;
                    //SAPbouiCOM.StaticText Lote = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticLote).Specific;
                    EditText Lote = (EditText)oForm.Items.Item(pluginForm.TxtNroLote.Uid).Specific;
                    Lote.Item.Top = 90;
                    Lote.Item.Left = 25;
                    Lote.Item.Height = 100;
                    Lote.Item.Width = 656;
                    Lote.Item.FontSize = 60;
                    Lote.Item.BackColor = Colores.White;

                    //SAPbouiCOM.Item oLink;
                    //oLink = oForm.Items.Add("Link", BoFormItemTypes.it_LINKED_BUTTON);
                    //oLink.LinkTo = pluginForm.TxtNroOrden.Uid;
                    //SAPbouiCOM.LinkedButton Linked = (SAPbouiCOM.LinkedButton)oForm.Items.Item("Link").Specific;
                    //Linked.LinkedObject = BoLinkedObject.lf_ProductionOrder;

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

                    oItem = oForm.Items.Add("40", BoFormItemTypes.it_RECTANGLE);
                    oItem.Top = 85;
                    oItem.Left = 20;
                    oItem.Width = 666;
                    oItem.Height = 110;
                    oItem.BackColor = Colores.Green;

                    ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLOrdenFab);
                    Conditions oCons = oCFL.GetConditions();

                    Condition oCon = oCons.Add();
                    oCon.Alias = "Status";
                    oCon.Operation = BoConditionOperation.co_EQUAL;
                    oCon.CondVal = "R";

                    //oCon.Relationship = BoConditionRelationship.cr_AND;
                    //oCon = oCons.Add();
                    //oCon.Alias = "ItemCode";
                    //oCon.Operation = BoConditionOperation.co_CONTAIN;
                    //oCon.CondVal = "SE";

                    oCFL.SetConditions(oCons);

                    try
                    {
                        oForm.Freeze(true);

                        DataTable oDT = oForm.DataSources.DataTables.Add(pluginForm.GridConsumo.Dt);
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                        grid.DataTable = oDT;

                        Button ButtonAddConsumo = (Button)oForm.Items.Item(pluginForm.ButtonAddConsumo).Specific;
                        ButtonAddConsumo.Item.Enabled = true;
                    }
                    finally { oForm.Freeze(false); }

                    oForm.Visible = true;
                }
                catch (Exception e)
                {
                    throw new Exception(string.Format("FormLoad {0}", e.Message));
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

                case pluginForm.TxtNroLote.Uid:
                    TxtNroCaja(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtNroOrden.Uid:
                    TxtNroOrden(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
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
                    //sbo_application.StatusBar.SetText("Button OK", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
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
                    //sbo_application.StatusBar.SetText("Button Cancel", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }
            }
        }

        private static void TxtNroOrden(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);
            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    ((EditText)oForm.Items.Item(pluginForm.TxtNroLote.Uid).Specific).Item.Click();
                }
            }

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtNroOrden.Uds).ValueEx = oDT.GetValue("DocNum", 0).ToString();

                        valores.NroOF = oDT.GetValue("DocEntry", 0).ToString();

                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string sSql = "SELECT  T0.\"ItemCode\",T0.\"ProdName\", " +
                            "T0.\"PlannedQty\", T0.\"OriginNum\",  T0.\"OriginAbs\", T0.\"PostDate\", T0.\"DueDate\",T0.\"CardCode\" " +
                            "FROM OWOR T0  " +
                            "WHERE T0.\"DocEntry\" = '" + oDT.GetValue("DocEntry", 0).ToString() + "' ";
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

                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                            DataTable oDT1 = grid.DataTable;
                            oDT1.Clear();

                            sSql = "SELECT  T0.\"DocEntry\",T0.\"DocNum\", T1.\"DocDate\", T0.\"DocTime\", " +
                                "T1.\"ItemCode\", T1.\"Dscription\", T1.\"LineStatus\", T1.\"Quantity\",T1.\"WhsCode\" " +
                                "FROM OIGE T0 INNER JOIN IGE1 T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
                                "WHERE T1.\"BaseEntry\" = '" + oDT.GetValue("DocEntry", 0).ToString() + "' order by T0.\"DocEntry\" desc ";

                            oDT1.ExecuteQuery(sSql);
                            EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("DocEntry");
                            oColumns.Description = "N° Consumo";
                            oColumns.LinkedObjectType = "60";

                            for (int i = 0; i < grid.Columns.Count; i++)
                            {
                                try
                                {
                                    grid.Columns.Item(i).Editable = false;
                                }
                                catch (Exception e)
                                {
                                    throw new Exception(string.Format("FormLoad {0}", e.Message));
                                }
                            }

                            if (grid.Rows.Count > 0)
                            {
                                string DocEntryOIGE = grid.DataTable.GetValue("DocEntry", 0).ToString();
                                sSql = "select T0.\"DocEntry\",T0.\"DocNum\",T5.\"CardCode\" as CardCode,T1.\"Quantity\",T1.\"ItemCode\",T3.\"ALLOCQTY\" as BatchSerialQty, " +
                                "T4.\"DistNumber\" as BatchorSerialNum,T4.\"SysNumber\" from OIGE T0 inner join IGE1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" " +
                                "inner join OITM T2 on T1.\"ItemCode\" = T2.\"ItemCode\" " +
                                "inner join(select S0.\"DocEntry\", S0.\"DocLine\", S1.\"SysNumber\", -sum(S1.\"Quantity\") as AllocQty " +
                                "from OITL S0 inner join ITL1 S1 on S0.\"LogEntry\" = S1.\"LogEntry\" where S0.\"DocType\" = 60 group by S0.\"DocEntry\", " +
                                "S0.\"DocLine\", S1.\"SysNumber\") T3 on T1.\"DocEntry\" = T3.\"DocEntry\" and T1.\"LineNum\" = T3.\"DocLine\" " +
                                "inner join OBTN T4 on T3.\"SysNumber\" = T4.\"SysNumber\" and T1.\"ItemCode\" = T4.\"ItemCode\" " +
                                "inner join OWOR T5 on T1.\"BaseEntry\" = T5.\"DocEntry\" " +
                                "where T1.\"BaseType\" = 202 and T1.\"BaseEntry\" = '" + oDT.GetValue("DocEntry", 0).ToString() + "' and T0.\"DocEntry\" " +
                                "= '" + DocEntryOIGE + "'  ";

                                //SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                oRS.DoQuery(sSql);
                                if (oRS.RecordCount == 1)
                                {
                                    string lote = oRS.Fields.Item("BatchorSerialNum").Value.ToString();
                                    var batch = CommonFunctions.DeserializeJsonObject<BatchNumberDetails>(CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{lote}'", sessionId, out _));

                                    StaticText StaticLote = (StaticText)oForm.Items.Item(pluginForm.StaticLote).Specific;
                                    StaticLote.Caption = lote;
                                    StaticText StaticCons = (StaticText)oForm.Items.Item(pluginForm.StaticCons).Specific;
                                    StaticCons.Caption = batch.U_FRU_CantBinsVol.ToString();
                                    StaticText StaticDisp = (StaticText)oForm.Items.Item(pluginForm.StaticDisp).Specific;
                                    StaticDisp.Caption = batch.U_FRU_CantBinsDis.ToString();
                                }
                                if (oRS.RecordCount == 0)
                                {
                                    StaticText StaticLote = (StaticText)oForm.Items.Item(pluginForm.StaticLote).Specific;
                                    StaticLote.Caption = "";
                                    StaticText StaticCons = (StaticText)oForm.Items.Item(pluginForm.StaticCons).Specific;
                                    StaticCons.Caption = "";
                                    StaticText StaticDisp = (StaticText)oForm.Items.Item(pluginForm.StaticDisp).Specific;
                                    StaticDisp.Caption = "";
                                }
                            }

                            //SAPbouiCOM.DataTable oDTgrid = oForm.DataSources.DataTables.Add(pluginForm.GridConsumo.Dt);
                            //SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                            //grid.DataTable = oDTgrid;
                            //oDTgrid.Clear();

                            //SELECT "ItemCode", "Dscription", "Quantity" from IGE1 where "BaseType" = '202' and "BaseEntry" = 75

                            //sSql = "SELECT  T0.\"ItemCode\",T0.\"Dscription\",T0.\"Quantity\" " +
                            //    "FROM IGE1 T0 INNER JOIN IGE1 T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
                            //    "WHERE T1.\"BaseEntry\" = '" + oDT.GetValue("DocEntry", 0).ToString() + "' ";
                            //oDTgrid.ExecuteQuery(sSql);

                            int cantbins = 0;
                            cantbins = grid.DataTable.Rows.Count;
                            StaticText StaticTotBins = (StaticText)oForm.Items.Item(pluginForm.StaticTotBins).Specific;
                            StaticTotBins.Caption = cantbins.ToString();

                            double kgConsumido = 0;
                            for (int i = 0; i < grid.Rows.Count; i++)
                            {
                                kgConsumido = kgConsumido + double.Parse(grid.DataTable.GetValue("Quantity", i).ToString().Replace(".", ","));
                            }

                            StaticText StaticKgBins = (StaticText)oForm.Items.Item(pluginForm.StaticKgBins).Specific;
                            StaticKgBins.Caption = kgConsumido.ToString();

                            


                            grid.Item.AffectsFormMode = false;
                        }
                    }
                }
            }
        }

        private static void TxtNroCaja(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN)
                {
                    if (oItemEvent.CharPressed == 13)
                    {
                        string Lote = ((EditText)oForm.Items.Item(pluginForm.TxtNroLote.Uid).Specific).Value.Trim();
                        var batch = CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Lote}'", sessionId, out System.Net.HttpStatusCode httpStatus).DeserializeJsonObject<BatchNumberDetails>();
                        if (batch != null)
                        {
                            if (batch.DocEntry != 0)
                            {
                                //sbo_application.StatusBar.SetText(string.Format("ENTER"), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                                string DocEntryOF = valores.NroOF; //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroOrden.Uid).Specific).Value.Trim();

                                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                string sSql = "SELECT T0.\"ItemCode\",T0.\"LineNum\",T0.\"wareHouse\" " +
                                       "FROM WOR1 T0  " +
                                       "WHERE T0.\"DocEntry\" = '" + DocEntryOF + "' and T0.\"ItemCode\" = '" + batch.ItemCode + "' and T0.\"BaseQty\" > 0";
                                oRS.DoQuery(sSql);

                                if (oRS.RecordCount != 0)
                                {
                                    var bin = CommonFunctions.DeserializeJsonObject<ListadoBinsMP>(CommonFunctions.GET(ServiceLayer.ListadoBinsMP, null, $"?$filter=LOTE eq '{Lote}' ", sessionId, out _)); //and ALMACEN eq 'FRUTEXSA'
                                    if (bin != null)
                                    {

                                        if (bin.CODIGO != null)
                                        {

                                            //string Ubicacion = bin.ALMACEN;

                                            //string BodegaOF = oRS.Fields.Item("wareHouse").Value.ToString();
                                            string LineNum = oRS.Fields.Item("LineNum").Value.ToString();

                                            //if (Ubicacion == BodegaOF)
                                            //{
                                                string sSql1 = "SELECT t1.\"AllocQty\",t1.\"SysNumber\",t1.\"ItemCode\",t0.\"DocEntry\",T0.\"DocType\",T0.\"ApplyEntry\",T0.\"StockEff\" " +
                                                    "from OITL t0 inner join ITL1 t1 on t0.\"LogEntry\" = t1.\"LogEntry\" where t0.\"ApplyEntry\" = '" + DocEntryOF + "' " +
                                                    "and t1.\"ItemCode\" = '" + batch.ItemCode + "' and t1.\"SysNumber\" =  '" + batch.SystemNumber + "' " +
                                                    "and t0.\"DocType\" = 202 and T0.\"StockEff\" = 2 and T1.\"AllocQty\" > 0  ";
                                                SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                                oRS1.DoQuery(sSql1);
                                                if (oRS1.RecordCount != 0)
                                                {
                                                    var response = CommonFunctions.ConsumoLoteCalibrado(Lote, DocEntryOF, LineNum, sessionId).DeserializeJsonToDynamic();
                                                    if (response.DocEntry != null)
                                                    {
                                                        sbo_application.StatusBar.SetText("Lote consumido correctamente", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                                        batch = CommonFunctions.DeserializeJsonObject<BatchNumberDetails>(CommonFunctions.GET(ServiceLayer.BatchNumberDetails, null, $"?$filter=Batch eq '{Lote}'", sessionId, out _));

                                                    StaticText StaticLote = (StaticText)oForm.Items.Item(pluginForm.StaticLote).Specific;
                                                        StaticLote.Caption = Lote;
                                                    StaticText StaticCons = (StaticText)oForm.Items.Item(pluginForm.StaticCons).Specific;
                                                        StaticCons.Caption = batch.U_FRU_CantBinsVol.ToString();
                                                    StaticText StaticDisp = (StaticText)oForm.Items.Item(pluginForm.StaticDisp).Specific;
                                                        StaticDisp.Caption = batch.U_FRU_CantBinsDis.ToString();
                                                    }

                                                }
                                                else
                                                {
                                                    throw new Exception("El Lote no ha sido asignado a la OF");
                                                }
                                            //}
                                            //else
                                            //{
                                            //    throw new Exception("El Lote no se encuentra en la bodega asignada");
                                            //}
                                        }
                                        else
                                        {
                                            throw new Exception("El Lote no esta disponible o no existe");
                                        }

                                    }
                                    else
                                    {
                                        throw new Exception("El Lote no es válido");
                                    }
                                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                                    DataTable oDT1 = grid.DataTable;
                                    oDT1.Clear();

                                    sSql = "SELECT  T0.\"DocEntry\",T0.\"DocNum\",T1.\"DocDate\",T0.\"DocTime\", " +
                                        "T1.\"ItemCode\", T1.\"Dscription\", T1.\"LineStatus\", T1.\"Quantity\",T1.\"WhsCode\" " +
                                        "FROM OIGE T0 INNER JOIN IGE1 T1 ON T0.\"DocEntry\" = T1.\"DocEntry\" " +
                                        "WHERE T1.\"BaseType\" = '" + "202" + "' and  T1.\"BaseEntry\" = '" + DocEntryOF + "' order by T0.\"DocEntry\" desc ";

                                    oDT1.ExecuteQuery(sSql);
                                    EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("DocEntry");
                                    oColumns.Description = "N° Consumo";
                                    oColumns.LinkedObjectType = "60";

                                    for (int i = 0; i < grid.Columns.Count; i++)
                                    {
                                        try
                                        {
                                            grid.Columns.Item(i).Editable = false;
                                        }
                                        catch (Exception e)
                                        {
                                            throw new Exception(string.Format("FormLoad {0}", e.Message));
                                        }
                                    }
                                    ((EditText)oForm.Items.Item(pluginForm.TxtNroLote.Uid).Specific).Value = "";
                                    ((EditText)oForm.Items.Item(pluginForm.TxtNroLote.Uid).Specific).Item.Click();

                                    int cantbins = 0;
                                    cantbins = grid.DataTable.Rows.Count;
                                    StaticText StaticTotBins = (StaticText)oForm.Items.Item(pluginForm.StaticTotBins).Specific;
                                    StaticTotBins.Caption = cantbins.ToString();

                                    double kgConsumido = 0;
                                    for (int i = 0; i < grid.Rows.Count; i++)
                                    {
                                        kgConsumido = kgConsumido + double.Parse(grid.DataTable.GetValue("Quantity", i).ToString().Replace(".", ","));
                                    }

                                    StaticText StaticKgBins = (StaticText)oForm.Items.Item(pluginForm.StaticKgBins).Specific;
                                    StaticKgBins.Caption = kgConsumido.ToString();
                                }
                                else
                                {
                                    throw new Exception("El código del articulo asociado al lote no se encuentra en la orden de fabricación");
                                }
                            }
                            else
                            {
                                throw new Exception("El Lote ingresado no es válido");
                            }
                        }
                        else
                        {
                            throw new Exception("El Lote ingresado no existe");
                        }
                    }
                }
            }
        }
    }
}