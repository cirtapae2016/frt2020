using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Xml;

namespace pluginPrdPT

{
    internal static class frm

    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;
            UserDataSource Uds1 = null;
            UserDataSource Uds2 = null;
            UserDataSource Uds3 = null;
            UserDataSource Uds4 = null;
            UserDataSource Uds5 = null;
            UserDataSource Uds6 = null;
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

                    FormCreationPackage.UniqueID = "EgresoPT" + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    oForm.Mode = BoFormMode.fm_OK_MODE;

                    for (int i = 3; i < oForm.Items.Count; i++)
                    {
                        oForm.Items.Item(i).AffectsFormMode = false;
                    }
                    // oForm.DataBrowser.BrowseBy = pluginForm.TxtNroOrden.Uid;
                    //SAPbouiCOM.StaticText Lote = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticLote).Specific;
                    Uds1 = oForm.DataSources.UserDataSources.Add(pluginForm.TxtPesoSubProd.Uds, BoDataType.dt_QUANTITY, 19);
                    EditText PesoSubProd = (EditText)oForm.Items.Item(pluginForm.TxtPesoSubProd.Uid).Specific;
                    PesoSubProd.DataBind.SetBound(true, "", Uds1.UID);
                    PesoSubProd.Item.Top = 158;
                    PesoSubProd.Item.Left = 540;
                    PesoSubProd.Item.Height = 32;
                    PesoSubProd.Item.Width = 145;
                    PesoSubProd.Item.FontSize = 28;
                    PesoSubProd.Item.BackColor = Colores.White;
                    PesoSubProd.Item.RightJustified = true;

                    Uds2 = oForm.DataSources.UserDataSources.Add(pluginForm.TxtFolioInicio.Uds, BoDataType.dt_LONG_NUMBER, 32);
                    EditText FolioInicio = (EditText)oForm.Items.Item(pluginForm.TxtFolioInicio.Uid).Specific;
                    FolioInicio.DataBind.SetBound(true, "", Uds2.UID);
                    FolioInicio.Item.RightJustified = true;

                    Uds3 = oForm.DataSources.UserDataSources.Add(pluginForm.TxtFolioFin.Uds, BoDataType.dt_LONG_NUMBER, 32);
                    EditText FolioFin = (EditText)oForm.Items.Item(pluginForm.TxtFolioFin.Uid).Specific;
                    FolioFin.DataBind.SetBound(true, "", Uds3.UID);
                    FolioFin.Item.RightJustified = true;

                    Uds4 = oForm.DataSources.UserDataSources.Add(pluginForm.TxtPesoPT.Uds, BoDataType.dt_QUANTITY, 19);
                    EditText TxtPesoPT = (EditText)oForm.Items.Item(pluginForm.TxtPesoPT.Uid).Specific;
                    TxtPesoPT.DataBind.SetBound(true, "", Uds4.UID);
                    TxtPesoPT.Item.RightJustified = true;

                    Uds5 = oForm.DataSources.UserDataSources.Add(pluginForm.TxtCantidadPT.Uds, BoDataType.dt_LONG_NUMBER, 32);
                    EditText TxtCantidadPT = (EditText)oForm.Items.Item(pluginForm.TxtCantidadPT.Uid).Specific;
                    TxtCantidadPT.DataBind.SetBound(true, "", Uds5.UID);
                    TxtCantidadPT.Item.RightJustified = true;


                    EditText TxtBodegaDestPT = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific;
                    TxtBodegaDestPT.DataBind.SetBound(true, "", pluginForm.TxtBodegaDestPT.Uds);




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

                    DataTable oDT1 = oForm.DataSources.DataTables.Add(pluginForm.GridConsumo.Dt);
                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                    grid.DataTable = oDT1;

                    DataTable oDT2 = oForm.DataSources.DataTables.Add(pluginForm.GridSubProd.Dt);
                    Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;
                    grid2.DataTable = oDT2;

                    DataTable oDT3 = oForm.DataSources.DataTables.Add(pluginForm.GridConsumoSP.Dt);
                    Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;
                    grid3.DataTable = oDT3;

                    //oItem = oForm.Items.Add("40", SAPbouiCOM.BoFormItemTypes.it_RECTANGLE);
                    //oItem.Top = 85;
                    //oItem.Left = 20;
                    //oItem.Width = 666;
                    //oItem.Height = 110;
                    //oItem.BackColor = Colores.Green;





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
                    oCon.CondVal = "PT";
                    oCFL.SetConditions(oCons);

                    ((Folder)oForm.Items.Item(pluginForm.FolderPT).Specific).Item.Click();

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

                case pluginForm.TxtBodegaDestPT.Uid:
                    TxtBodegaDestPT(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.TxtNroPT.Uid:
                    TxtNroPT(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonAsignaLote:
                    ButtonAsignaLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonAsigLoteSP:
                    ButtonAsigLoteSP(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.ButtonPreviewTarja:
                    ButtonPreviewTarja(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.FolderPT:
                    FolderPT(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
                    break;

                case pluginForm.FolderSubProd:
                    FolderSubProd(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, SessionId);
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

        //if (DT != null)
        //{
        //    SAPbouiCOM.EditText Lote = (SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLote.Uid).Specific;
        //    Lote.Item.Click(SAPbouiCOM.BoCellClickType.ct_Regular);
        //}

        private static void TxtBodegaDestPT(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
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
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtBodegaDestPT.Uds).ValueEx = oDT.GetValue("WhsCode", 0).ToString();
                    }
                }
            }
        }

        private static void TxtNroOrden(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    ((Folder)oForm.Items.Item(pluginForm.FolderPT).Specific).Item.Click();
                }

            }

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

                            TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific;
                            TxtForm.Item.Visible = true;

                            StaticText Static = (StaticText)oForm.Items.Item("Item_14").Specific;
                            Static.Item.Visible = true;

                            valores.NroOC = oRS.Fields.Item("OriginAbs").Value.ToString();
                            valores.CardCode = oRS.Fields.Item("CardCode").Value.ToString();

                            EditText OF = (EditText)oForm.Items.Item("OF").Specific;
                            OF.Value = valores.NroOF;

                            EditText OV = (EditText)oForm.Items.Item("OV").Specific;
                            OV.Value = valores.NroOC;

                            //SAPbouiCOM.DataTable oDT1 = oForm.DataSources.DataTables.Add(pluginForm.GridConsumo.Dt);
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                            //grid.DataTable = oDT1;

                            //SAPbouiCOM.DataTable oDT2 = oForm.DataSources.DataTables.Add(pluginForm.GridSubProd.Dt);
                            Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;
                            //grid2.DataTable = oDT2;

                            //SAPbouiCOM.DataTable oDT3 = oForm.DataSources.DataTables.Add(pluginForm.GridConsumoSP.Dt);
                            Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;
                            //grid3.DataTable = oDT3;
                            //Linked = (SAPbouiCOM.LinkedButton)oForm.Items.Item(pluginForm.LinkedOV).Specific;
                            //Linked.Item.LinkTo = pluginForm.TxtNroOc.Uid;
                            //Linked.LinkedObject = BoLinkedObject.lf_Order;

                            //Linked = (SAPbouiCOM.LinkedButton)oForm.Items.Item(pluginForm.LinkedCardCode).Specific;
                            //Linked.Item.LinkTo = pluginForm.TxtCardCode.Uid; ;
                            //Linked.LinkedObject = BoLinkedObject.lf_BusinessPartner;


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
                            SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            string sSql1 = "SELECT  T0.\"U_DFO_Valor\",T0.\"U_DFO_Descrip\" " +
                                          "FROM \"@DFO_OPDFO\"  T0  " +
                                          "WHERE T0.\"U_DFO_Tipo\" = 'CALIBRE' and T0.\"U_DFO_Descrip\" = '" + it.ForeignName + "' ";
                            oRS1.DoQuery(sSql1);
                            if (oRS1.RecordCount != 0)
                            {
                                while (!oRS1.EoF)
                                {
                                    CBcalibre.ValidValues.Add(oRS1.Fields.Item("U_DFO_Valor").Value.ToString(), oRS1.Fields.Item("U_DFO_Descrip").Value.ToString());
                                    oRS1.MoveNext();
                                }
                            }

                            CBcaract = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                            //CBcaract.ValidValues.Add("-", "-");
                            oRS1 = null;
                            oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            sSql1 = "SELECT  T0.\"U_DFO_Valor\",T0.\"U_DFO_Descrip\" " +
                                          "FROM \"@DFO_OPDFO\"  T0  " +
                                          "WHERE T0.\"U_DFO_Tipo\" = 'CARACTERISTICA' and T0.\"U_DFO_Descrip\" = '" + it.ForeignName + "' ";
                            oRS1.DoQuery(sSql1);
                            if (oRS1.RecordCount != 0)
                            {
                                while (!oRS1.EoF)
                                {
                                    CBcaract.ValidValues.Add(oRS1.Fields.Item("U_DFO_Valor").Value.ToString(), oRS1.Fields.Item("U_DFO_Descrip").Value.ToString());
                                    oRS1.MoveNext();
                                }
                            }





                            if (it.InventoryUOM == "KG")
                            {
                                Static = (StaticText)oForm.Items.Item("Item_12").Specific;
                                Static.Item.Visible = false;
                                Static = (StaticText)oForm.Items.Item("Item_13").Specific;
                                Static.Item.Visible = false;
                                Static = (StaticText)oForm.Items.Item("Item_10").Specific;
                                Static.Item.Visible = false;

                                EditText FolioInicio = (EditText)oForm.Items.Item(pluginForm.TxtFolioInicio.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioInicio.Uds).ValueEx = "0";
                                FolioInicio.Item.RightJustified = true;
                                FolioInicio.Item.Visible = false;

                                EditText TxtFolioFin = (EditText)oForm.Items.Item(pluginForm.TxtFolioFin.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioFin.Uds).ValueEx = "0";
                                TxtFolioFin.Item.RightJustified = true;
                                TxtFolioFin.Item.Visible = false;

                                EditText TxtCantidadPT = (EditText)oForm.Items.Item(pluginForm.TxtCantidadPT.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtCantidadPT.Uds).ValueEx = "0";
                                TxtCantidadPT.Item.RightJustified = true;
                                TxtCantidadPT.Item.Visible = false;

                                Static = (StaticText)oForm.Items.Item("Item_11").Specific;
                                Static.Item.Visible = true;
                                Static = (StaticText)oForm.Items.Item("Item_15").Specific;
                                Static.Item.Visible = true;

                                EditText TxtPesoPT = (EditText)oForm.Items.Item(pluginForm.TxtPesoPT.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoPT.Uds).ValueEx = "0";
                                TxtPesoPT.Item.RightJustified = true;
                                TxtPesoPT.Item.Visible = true;
                            }
                            else
                            {
                                Static = (StaticText)oForm.Items.Item("Item_12").Specific;
                                Static.Item.Visible = true;
                                Static = (StaticText)oForm.Items.Item("Item_13").Specific;
                                Static.Item.Visible = true;
                                Static = (StaticText)oForm.Items.Item("Item_10").Specific;
                                Static.Item.Visible = true;

                                EditText FolioInicio = (EditText)oForm.Items.Item(pluginForm.TxtFolioInicio.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioInicio.Uds).ValueEx = "0";
                                FolioInicio.Item.RightJustified = true;
                                FolioInicio.Item.Visible = true;

                                EditText TxtFolioFin = (EditText)oForm.Items.Item(pluginForm.TxtFolioFin.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioFin.Uds).ValueEx = "0";
                                TxtFolioFin.Item.RightJustified = true;
                                TxtFolioFin.Item.Visible = true;

                                EditText TxtCantidadPT = (EditText)oForm.Items.Item(pluginForm.TxtCantidadPT.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtCantidadPT.Uds).ValueEx = "0";
                                TxtCantidadPT.Item.RightJustified = true;
                                TxtCantidadPT.Item.Visible = true;

                                Static = (StaticText)oForm.Items.Item("Item_11").Specific;
                                Static.Item.Visible = false;
                                Static = (StaticText)oForm.Items.Item("Item_15").Specific;
                                Static.Item.Visible = false;

                                EditText TxtPesoPT = (EditText)oForm.Items.Item(pluginForm.TxtPesoPT.Uid).Specific;
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoPT.Uds).ValueEx = "0";
                                TxtPesoPT.Item.RightJustified = true;
                                TxtPesoPT.Item.Visible = false;
                            }

                            Button Button = (Button)oForm.Items.Item(pluginForm.ButtonAsignaLote).Specific;
                            Button.Item.Visible = true;

                            grid.DataTable.Clear();
                            sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                                   " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                                   "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                                   "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                                   "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                                   " and T0.\"ItemCode\" = '" + oRS.Fields.Item("ItemCode").Value.ToString() + "' " +
                                   " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 ";
                            grid.DataTable.ExecuteQuery(sSql);
                            EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("DocEntry");
                            oColumns.Description = "N° Egreso";
                            oColumns.LinkedObjectType = "59";
                            oColumns = (EditTextColumn)grid.Columns.Item("AbsEntry");
                            oColumns.Description = "Interno Lote";
                            oColumns.LinkedObjectType = "10000044";
                            grid.SelectionMode = BoMatrixSelect.ms_Single;
                            for (int i = 0; i < grid.Columns.Count; i++)
                            {
                                try
                                {
                                    grid.Columns.Item(i).Editable = false;
                                }
                                catch
                                {
                                    throw;
                                }
                            }

                            grid2.DataTable.Clear();
                            sSql = "SELECT T0.\"DocEntry\", T0.\"ItemCode\", T0.\"BaseQty\", " +
                            "T0.\"PlannedQty\", T0.\"IssuedQty\",  T0.\"wareHouse\" FROM WOR1 T0 " +
                            "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"BaseQty\" < 0 order by 1 ";
                            grid2.DataTable.ExecuteQuery(sSql);
                            EditTextColumn oColumns2 = (EditTextColumn)grid2.Columns.Item("DocEntry");
                            oColumns2.Description = "N° OF";
                            oColumns2.LinkedObjectType = "202";
                            oColumns2 = (EditTextColumn)grid2.Columns.Item("ItemCode");
                            oColumns2.Description = "Artículo";
                            oColumns2.LinkedObjectType = "4";
                            grid2.SelectionMode = BoMatrixSelect.ms_Single;
                            for (int i = 0; i < grid2.Columns.Count; i++)
                            {
                                try
                                {
                                    grid2.Columns.Item(i).Editable = false;
                                }
                                catch
                                {
                                    throw;
                                }
                            }

                            grid3.DataTable.Clear();
                            sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                                   " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                                   "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                                   "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                                   "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                                   " and T0.\"ItemCode\" <> '" + oRS.Fields.Item("ItemCode").Value.ToString() + "' " +
                                   " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 order by 1 ";
                            grid3.DataTable.ExecuteQuery(sSql);
                            EditTextColumn oColumns3 = (EditTextColumn)grid3.Columns.Item("DocEntry");
                            oColumns3.Description = "N° Egreso";
                            oColumns3.LinkedObjectType = "59";
                            oColumns3 = (EditTextColumn)grid3.Columns.Item("AbsEntry");
                            oColumns3.Description = "Interno Lote";
                            oColumns3.LinkedObjectType = "10000044";
                            grid3.SelectionMode = BoMatrixSelect.ms_Single;
                            for (int i = 0; i < grid3.Columns.Count; i++)
                            {
                                try
                                {
                                    grid3.Columns.Item(i).Editable = false;
                                }
                                catch
                                {
                                    throw;
                                }
                            }

                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioInicio.Uds).ValueEx = "0";
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioFin.Uds).ValueEx = "0";
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtCantidadPT.Uds).ValueEx = "0";
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoPT.Uds).ValueEx = "0";
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoSubProd.Uds).ValueEx = "0";
                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtBodegaDestPT.Uds).ValueEx = "";


                            ComboBox Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                            Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);
                            Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;
                            Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);


                            //SAPbouiCOM.EditTextColumn oColumns = (SAPbouiCOM.EditTextColumn)grid.Columns.Item("DocEntry");
                            //oColumns.Description = "N° Consumo";
                            //oColumns.LinkedObjectType = "60";
                        }
                    }
                }
            }
        }

        private static void TxtNroPT(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                //if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_KEY_DOWN)
                //{
                //    if (oItemEvent.CharPressed == 13)
                //    {
                //        sbo_application.StatusBar.SetText(string.Format("ENTER"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                //        string DocEntryOF = valores.NroOF; //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroOrden.Uid).Specific).Value.Trim();
                //        string NumeroPT = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroPT.Uid).Specific).Value.Trim();

                //        string response = SAPFunctions.ConsumoProductoTerminado(NumeroPT, DocEntryOF, SessionId);

                //        sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                //        SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                //        SAPbouiCOM.DataTable oDT1 = grid.DataTable;
                //        oDT1.Clear();

                //        string sSql = "SELECT  T0.\"DocEntry\",T0.\"DocNum\", " +
                //            "T0.\"U_CodigoPT\", T0.\"U_LoteID\" " +
                //            "FROM \"@DFO_OLOPT\" T0 " +
                //            "WHERE T0.\"U_DocEntryOF\" = '" + DocEntryOF + "' ";

                //        oDT1.ExecuteQuery(sSql);

                //        SAPbouiCOM.EditText EditNumeroPT = (SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroPT.Uid).Specific;
                //        EditNumeroPT.Value = "";
                //    }
                //}
            }
        }

        private static void ButtonAsignaLote(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    // sbo_application.StatusBar.SetText(string.Format("ENTER"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    string DocNumOF = valores.NroOF; //((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroOrden.Uid).Specific).Value.Trim();
                    string FolioInicio = ((EditText)oForm.Items.Item(pluginForm.TxtFolioInicio.Uid).Specific).Value.Trim();
                    string FolioFin = ((EditText)oForm.Items.Item(pluginForm.TxtFolioFin.Uid).Specific).Value.Trim();
                    string PesoPT = ((EditText)oForm.Items.Item(pluginForm.TxtPesoPT.Uid).Specific).Value.Trim();

                    string ItemCode = ((EditText)oForm.Items.Item(pluginForm.TxtItemCode.Uid).Specific).Value.Trim();
                    string WhsCode = ((EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific).Value.Trim();
                    string CantidadPT = ((EditText)oForm.Items.Item(pluginForm.TxtCantidadPT.Uid).Specific).Value.Trim();

                    var it = CommonFunctions.GET(ServiceLayer.Items, ItemCode, null, SessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();
                    if (it.InventoryUOM == "KG")
                    {
                        if ((double.Parse(PesoPT) > 0))
                        {
                            if (WhsCode != "")
                            {
                                DateTime date = DateTime.Now;
                                string fecha = date.ToString("yyyyMMddHHmmssfff");

                                string responseCPT = CommonFunctions.ConsumoProductoTerminado(fecha, ItemCode, FolioInicio, FolioFin, DocNumOF, SessionId);

                                string responseRP = SAPFunctions.ReciboProduccion(WhsCode, fecha, "", PesoPT, "", "", DocNumOF,"","", sbo_company, SessionId);
                                //sbo_application.StatusBar.SetText(responseRP, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                            }
                            else
                            {
                                throw new Exception("Debe ingresar Bodega de destino");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe ingresar Peso");
                        }
                    }
                    else
                    {
                        if ((FolioInicio != "") && (FolioFin != ""))
                        {
                            if (int.Parse(FolioInicio) <= int.Parse(FolioFin))
                            {
                                if (WhsCode != "")
                                {
                                    if (int.Parse(CantidadPT) > 0)
                                    {
                                        DateTime date = DateTime.Now;
                                        string fecha = date.ToString("yyyyMMddHHmmssfff");

                                        string responseCPT = CommonFunctions.ConsumoProductoTerminado(fecha, ItemCode, FolioInicio, FolioFin, DocNumOF, SessionId);

                                        string responseRP = SAPFunctions.ReciboProduccion(WhsCode,fecha, "", CantidadPT, FolioInicio, FolioFin, DocNumOF,"","", sbo_company, SessionId);
                                        //sbo_application.StatusBar.SetText(responseRP, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioInicio.Uds).ValueEx = "0";
                                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioFin.Uds).ValueEx = "0";
                                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCantidadPT.Uds).ValueEx = "0";
                                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoPT.Uds).ValueEx = "0";
                                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoSubProd.Uds).ValueEx = "0";
                                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtBodegaDestPT.Uds).ValueEx = "";


                                        ComboBox Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                                        Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);

                                        Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;
                                        Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);


                                    }
                                    else
                                    {
                                        throw new Exception("Debe ingresar cantidad");
                                    }
                                }
                                else
                                {
                                    throw new Exception("Debe ingresar Bodega de destino");
                                }
                            }
                            else
                            {
                                throw new Exception("Ingrese valores correctos para folio");
                            }
                        }
                        else
                        {
                            throw new Exception("Debe ingresar folio inicial y final");
                        }
                    }

                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                    Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;
                    Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;

                    grid.DataTable.Clear();
                    string sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                           " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                           "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                           "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                           "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                           " and T0.\"ItemCode\" = '" + ItemCode + "' " +
                           " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 order by 1";
                    grid.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("DocEntry");
                    oColumns.Description = "N° Egreso";
                    oColumns.LinkedObjectType = "59";
                    oColumns = (EditTextColumn)grid.Columns.Item("AbsEntry");
                    oColumns.Description = "Interno Lote";
                    oColumns.LinkedObjectType = "10000044";
                    grid.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        try
                        {
                            grid.Columns.Item(i).Editable = false;
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    grid2.DataTable.Clear();
                    sSql = "SELECT T0.\"DocEntry\", T0.\"ItemCode\", T0.\"BaseQty\", " +
                    "T0.\"PlannedQty\", T0.\"IssuedQty\",  T0.\"wareHouse\" FROM WOR1 T0 " +
                    "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"BaseQty\" < 0 order by 1";
                    grid2.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns2 = (EditTextColumn)grid2.Columns.Item("DocEntry");
                    oColumns2.Description = "N° OF";
                    oColumns2.LinkedObjectType = "202";
                    oColumns2 = (EditTextColumn)grid2.Columns.Item("ItemCode");
                    oColumns2.Description = "Artículo";
                    oColumns2.LinkedObjectType = "4";
                    grid2.SelectionMode = BoMatrixSelect.ms_Single;
                    for (int i = 0; i < grid2.Columns.Count; i++)
                    {
                        try
                        {
                            grid2.Columns.Item(i).Editable = false;
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    grid3.DataTable.Clear();
                    sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                           " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                           "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                           "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                           "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                           " and T0.\"ItemCode\" <> '" + ItemCode + "' " +
                           " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 order by 1";
                    grid3.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns3 = (EditTextColumn)grid3.Columns.Item("DocEntry");
                    oColumns3.Description = "N° Egreso";
                    oColumns3.LinkedObjectType = "59";
                    oColumns3 = (EditTextColumn)grid3.Columns.Item("AbsEntry");
                    oColumns3.Description = "Interno Lote";
                    oColumns3.LinkedObjectType = "10000044";
                    grid3.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid3.Columns.Count; i++)
                    {
                        try
                        {
                            grid3.Columns.Item(i).Editable = false;
                        }
                        catch
                        {
                            throw;
                        }
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
                    string ItemCode1 = ((EditText)oForm.Items.Item(pluginForm.TxtItemCode.Uid).Specific).Value.Trim();
                    string WhsCode = ((EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific).Value.Trim();
                    string Caract = ((ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific).Selected.Value;
                    string Calibre = ((ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific).Selected.Value;
                    if (grid.Rows.SelectedRows.Count > 0)
                    {
                        if (double.Parse(Peso.Replace(".", ",")) != 0)
                        {
                            if (WhsCode != "")
                            {
                                if (Calibre != "-")
                                {
                                    int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                                    string ItemCode = grid.DataTable.GetValue("ItemCode", row).ToString();
                                    SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                    string sSql1 = "SELECT T0.\"ItemCode\",T0.\"LineNum\" " +
                                                   "FROM WOR1 T0  " +
                                                   "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"ItemCode\" = '" + ItemCode + "' ";
                                    oRS.DoQuery(sSql1);
                                    if (oRS.RecordCount != 0)
                                    {
                                        string LoteId = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                                        string LineNum = oRS.Fields.Item("LineNum").Value.ToString();
                                        DateTime date = DateTime.Now;
                                        string fecha = date.ToString("yyyyMMddHHmmssfff");
                                        string responseCPT = CommonFunctions.ConsumoProductoTerminado(fecha, ItemCode, "", "", DocNumOF, SessionId);

                                        //string responseRP = SAPFunctions.ReciboProduccion(fecha, LineNum, Peso, "", "", DocNumOF, sbo_company, SessionId);
                                        var responseRP = SAPFunctions.ReciboProduccion(WhsCode, LoteId, LineNum, Peso, "", "", DocNumOF, Calibre, Caract, sbo_company, SessionId).DeserializeJsonToDynamic();

                                        if (responseRP.DocEntry > 0)
                                        {
                                            sbo_application.StatusBar.SetText("OK", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioInicio.Uds).ValueEx = "0";
                                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtFolioFin.Uds).ValueEx = "0";
                                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtCantidadPT.Uds).ValueEx = "0";
                                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoPT.Uds).ValueEx = "0";
                                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtPesoSubProd.Uds).ValueEx = "0";
                                            oForm.DataSources.UserDataSources.Item(pluginForm.TxtBodegaDestPT.Uds).ValueEx = "";


                                            ComboBox Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcaract).Specific;
                                            Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);
                                            Combobox = (ComboBox)oForm.Items.Item(pluginForm.CBcalibre).Specific;
                                            Combobox.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);
                                        }

                                    }
                                }
                                else
                                {
                                    throw new Exception("Debe seleccionar calibre");
                                }
                            }
                            else
                            {
                                throw new Exception("Debe ingresar Bodega de destino");
                            }
                        }
                        else
                        {
                            throw new Exception("El Peso no puede ser igual a 0");
                        }
                    }
                    else
                    {
                        throw new Exception("Debe seleccionar el articulo");
                    }

                    Grid grid1 = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;

                    Grid grid2 = (Grid)oForm.Items.Item(pluginForm.GridSubProd.Uid).Specific;

                    Grid grid3 = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;

                    grid1.DataTable.Clear();
                    string sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                           " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                           "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                           "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                           "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                           " and T0.\"ItemCode\" = '" + ItemCode1 + "' " +
                           " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 order by 1";
                    grid1.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns = (EditTextColumn)grid1.Columns.Item("DocEntry");
                    oColumns.Description = "N° Egreso";
                    oColumns.LinkedObjectType = "59";
                    oColumns = (EditTextColumn)grid1.Columns.Item("AbsEntry");
                    oColumns.Description = "Interno Lote";
                    oColumns.LinkedObjectType = "10000044";
                    grid1.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid1.Columns.Count; i++)
                    {
                        try
                        {
                            grid1.Columns.Item(i).Editable = false;
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    grid2.DataTable.Clear();
                    sSql = "SELECT T0.\"DocEntry\", T0.\"ItemCode\", T0.\"BaseQty\", " +
                    "T0.\"PlannedQty\", T0.\"IssuedQty\",  T0.\"wareHouse\" FROM WOR1 T0 " +
                    "WHERE T0.\"DocEntry\" = '" + valores.NroOF + "' and T0.\"BaseQty\" < 0 order by 1";
                    grid2.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns2 = (EditTextColumn)grid2.Columns.Item("DocEntry");
                    oColumns2.Description = "N° OF";
                    oColumns2.LinkedObjectType = "202";
                    oColumns2 = (EditTextColumn)grid2.Columns.Item("ItemCode");
                    oColumns2.Description = "Artículo";
                    oColumns2.LinkedObjectType = "4";
                    grid2.SelectionMode = BoMatrixSelect.ms_Single;
                    for (int i = 0; i < grid2.Columns.Count; i++)
                    {
                        try
                        {
                            grid2.Columns.Item(i).Editable = false;
                        }
                        catch
                        {
                            throw;
                        }
                    }

                    grid3.DataTable.Clear();
                    sSql = "SELECT T0.\"DocEntry\",T2.\"AbsEntry\",T2.\"DistNumber\",T0.\"ItemCode\", T0.\"ItemName\", T0.\"DocQty\"," +
                           " T0.\"StockQty\", T1.\"Quantity\" FROM OITL T0  INNER JOIN ITL1 T1 ON T0.\"LogEntry\" = T1.\"LogEntry\" " +
                           "  inner join OBTN T2 ON T1.\"ItemCode\" = T2.\"ItemCode\" and T1.\"MdAbsEntry\" = T2.\"AbsEntry\" " +
                           "  and T1.\"SysNumber\" = T2.\"SysNumber\" " +
                           "  WHERE T0.\"BaseEntry\" = '" + valores.NroOF + "' " +
                           " and T0.\"ItemCode\" <> '" + ItemCode1 + "' " +
                           " and T0.\"BaseType\" = 202 and T0.\"ApplyType\"= 59 order by 1";
                    grid3.DataTable.ExecuteQuery(sSql);
                    EditTextColumn oColumns3 = (EditTextColumn)grid3.Columns.Item("DocEntry");
                    oColumns3.Description = "N° Egreso";
                    oColumns3.LinkedObjectType = "59";
                    oColumns3 = (EditTextColumn)grid3.Columns.Item("AbsEntry");
                    oColumns3.Description = "Interno Lote";
                    oColumns3.LinkedObjectType = "10000044";
                    grid3.SelectionMode = BoMatrixSelect.ms_None;
                    for (int i = 0; i < grid3.Columns.Count; i++)
                    {
                        try
                        {
                            grid3.Columns.Item(i).Editable = false;
                        }
                        catch
                        {
                            throw;
                        }
                    }
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
                    if (oForm.PaneLevel == 1)
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumo.Uid).Specific;
                        int gridrowcount = grid.Rows.Count;
                        for (var i = 0; i <= gridrowcount - 1; i++)
                        {
                            if (grid.Rows.IsSelected(i) == true)
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
                                sbo_application.MessageBox("El lote no se encuentra en existencia");
                            }
                        }
                        else
                        {
                            sbo_application.MessageBox("Debe seleccionar un registro");
                        }
                    }
                    if (oForm.PaneLevel == 2)
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridConsumoSP.Uid).Specific;
                        int gridrowcount = grid.Rows.Count;
                        for (var i = 0; i <= gridrowcount - 1; i++)
                        {
                            if (grid.Rows.IsSelected(i) == true)
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
                                sbo_application.MessageBox("El lote no se encuentra en existencia");
                            }
                        }
                        else
                        {
                            sbo_application.MessageBox("Debe seleccionar un registro");
                        }
                    }
                }
            }
        }

        private static void FolderPT(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    oForm.Freeze(true);
                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific;
                    TxtForm.Item.Left = 358;
                    TxtForm.Item.Top = 178;

                    StaticText StaticForm = (StaticText)oForm.Items.Item("Item_14").Specific;
                    StaticForm.Item.Left = 262;
                    StaticForm.Item.Top = 178;
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    //oForm.Freeze(true);
                    //EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific;
                    //TxtForm.Item.Left = 358;
                    //TxtForm.Item.Top = 178;

                    //StaticText StaticForm = (StaticText)oForm.Items.Item("Item_14").Specific;
                    //StaticForm.Item.Left = 262;
                    //StaticForm.Item.Top = 178;
                    oForm.Freeze(false);
                }
            }
        }

        private static void FolderSubProd(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string SessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    oForm.Freeze(true);
                    EditText TxtForm = (EditText)oForm.Items.Item(pluginForm.TxtBodegaDestPT.Uid).Specific;
                    TxtForm.Item.Left = 606;
                    TxtForm.Item.Top = 224;

                    StaticText StaticForm = (StaticText)oForm.Items.Item("Item_14").Specific;
                    StaticForm.Item.Left = 516;
                    StaticForm.Item.Top = 224;
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {

                    oForm.Freeze(false);
                }
            }
        }
    }
}