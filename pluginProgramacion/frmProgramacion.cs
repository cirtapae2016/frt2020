using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

namespace pluginProgramacion
{
    internal static class frmProgramacion

    {
        internal static void FormLoad(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            FormCreationParams FormCreationPackage;
            Form oForm = null;
            UserDataSource Uds1 = null;

            if (oMenuEvent.BeforeAction)
            {
                FormCreationPackage = (FormCreationParams)sbo_application.CreateObject(BoCreatableObjectType.cot_FormCreationParams);

                try
                {
                    if (string.IsNullOrEmpty(sessionId))
                        sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);

                    string contenidoArchivo = Properties.Resources.ResourceManager.GetString("Programacion");
                    string date = DateTime.Now.ToString("yyyyMMdd");

                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;

                    FormCreationPackage.UniqueID = "Programacion" + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

                    ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value = date;
                    oForm.Mode = BoFormMode.fm_OK_MODE;

                    for (int i = 3; i < oForm.Items.Count; i++)
                    {
                        oForm.Items.Item(i).AffectsFormMode = false;
                    }

                    try
                    {
                        oForm.Freeze(true);
                        valores.sorting = "";
                        valores.order = "desc";

                        DataTable oDT = oForm.DataSources.DataTables.Add(pluginForm.GridAct.Dt);
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridAct.Uid).Specific;
                        grid.DataTable = oDT;
                        grid.Item.AffectsFormMode = false;
                        oDT = oForm.DataSources.DataTables.Add(pluginForm.GridOC.Dt);
                        grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                        grid.DataTable = oDT;

                        ComboBox CBbins = (ComboBox)oForm.Items.Item(pluginForm.CBpropBins).Specific;
                        CBbins.Item.AffectsFormMode = false;
                        CBbins.ValidValues.Add("1", "Productor");
                        CBbins.ValidValues.Add("2", "Frutexsa");
                        CBbins.ValidValues.Add("3", "Ambos");
                        CBbins.Item.Enabled = false;

                        Button BtnAddAct = (Button)oForm.Items.Item(pluginForm.ButtonAddActivity).Specific;
                        BtnAddAct.Item.Enabled = false;
                        BtnAddAct.Item.AffectsFormMode = false;

                        EditText CantBins = (EditText)oForm.Items.Item(pluginForm.TxtCantBins).Specific;
                        CantBins.Item.Enabled = false;
                        CantBins.Item.AffectsFormMode = false;
                        oForm.DataSources.UserDataSources.Add("CantBins", BoDataType.dt_LONG_NUMBER, 2);
                        CantBins.DataBind.SetBound(true, "", "CantBins");

                        EditText HoraPrograma = (EditText)oForm.Items.Item(pluginForm.TxtHoraProg.Uid).Specific;
                        DBDataSource dbData = oForm.DataSources.DBDataSources.Add("OCLG");
                        //HoraPrograma.DataBind.DataBound(true,)

                        EditText CardCode = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                        CardCode.Item.AffectsFormMode = false;
                        CardCode.Item.Enabled = false;

                        EditText CantGuias = (EditText)oForm.Items.Item(pluginForm.TxtCantGuias).Specific;
                        CantGuias.Item.AffectsFormMode = false;
                        oForm.DataSources.UserDataSources.Add("CantGuias", BoDataType.dt_SHORT_NUMBER, 2);
                        CantGuias.DataBind.SetBound(true, "", "CantGuias");
                        CantGuias.Value = "1";

                        EditText CardName = (EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific;
                        CardName.Item.AffectsFormMode = false;
                        CardName.Item.Enabled = false;
                        ComboBox CBdestinatario = (ComboBox)oForm.Items.Item(pluginForm.CBdestinatario).Specific;
                        CBdestinatario.Item.Enabled = false;
                        CBdestinatario.Item.AffectsFormMode = false;

                        //var Locations = CommonFunctions.DeserializeList<ActivityLocations>(CommonFunctions.GET(ServiceLayer.ActivityLocations, null, null, sessionId, out _));

                        //foreach (var item in Locations.Where(i => i.Code != "-2"))
                        //    ((ComboBox)oForm.Items.Item(pluginForm.CBlocalidad).Specific).ValidValues.Add(item.Code, item.Name);

                        //((ComboBox)oForm.Items.Item(pluginForm.CBlocalidad).Specific).Item.Enabled = false;

                        Uds1 = oForm.DataSources.UserDataSources.Add("ChkPers", BoDataType.dt_SHORT_TEXT, 1);
                        CheckBox ChkTrasvacije = ((CheckBox)oForm.Items.Item(pluginForm.ChkTrasvacije).Specific);
                        ChkTrasvacije.Item.AffectsFormMode = false;
                        ChkTrasvacije.DataBind.SetBound(true, "", Uds1.UID);
                        ChkTrasvacije.ValOn = "Y";
                        ChkTrasvacije.ValOff = "N";

                        //Descomentar
                        ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLProductores);
                        Conditions oCons = oCFL.GetConditions();

                        Condition oCon = oCons.Add();
                        oCon.Alias = "GroupCode";
                        oCon.Operation = BoConditionOperation.co_EQUAL;
                        oCon.CondVal = "103"; //oCon.CondVal = "103";
                        Condition oCon1 = oCons.Add();
                        oCon1.Alias = "CardType";
                        oCon1.Operation = BoConditionOperation.co_EQUAL;
                        oCon1.CondVal = "S";
                        oCFL.SetConditions(oCons);

                        ChooseFromList oCFL2 = oForm.ChooseFromLists.Item(pluginForm.CFLCodProduct);
                        Conditions oCons2 = oCFL2.GetConditions();

                        Condition oCon2 = oCons2.Add();
                        oCon2.Alias = "GroupCode";
                        oCon2.Operation = BoConditionOperation.co_EQUAL;
                        oCon2.CondVal = "103"; //oCon2.CondVal = "103";

                        oCFL2.SetConditions(oCons2);

                        ChooseFromList oCFL3 = oForm.ChooseFromLists.Item(pluginForm.CFLRutTransp);
                        Conditions oCons3 = oCFL3.GetConditions();
                        Condition oCon5 = oCons3.Add();
                        oCon5.Alias = "GroupCode";
                        oCon5.Operation = BoConditionOperation.co_EQUAL;
                        oCon5.CondVal = "103"; //oCon2.CondVal = "103";
                        oCon5.Relationship = BoConditionRelationship.cr_OR;
                        Condition oCon3 = oCons3.Add();
                        oCon3.Alias = "CardType";
                        oCon3.Operation = BoConditionOperation.co_EQUAL;
                        oCon3.CondVal = "S";
                        oCon3.Relationship = BoConditionRelationship.cr_AND;
                        Condition oCon4 = oCons3.Add();
                        oCon4.Alias = "QryGroup2";
                        oCon4.Operation = BoConditionOperation.co_EQUAL;
                        oCon4.CondVal = "Y";

                        oCFL3.SetConditions(oCons3);

                        EditText TxtRutTransp = (EditText)oForm.Items.Item(pluginForm.TxtRutTransp.Uid).Specific;

                        TxtRutTransp.Item.AffectsFormMode = false;
                        oForm.DataSources.UserDataSources.Add("TxtRutTra", BoDataType.dt_LONG_TEXT, 15);
                        TxtRutTransp.DataBind.SetBound(true, "", "TxtRutTra");

                        //((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.AffectsFormMode = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();

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
                        ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Item.AffectsFormMode = false;

                        ComboBox CBDocumento = (ComboBox)oForm.Items.Item(pluginForm.CBdocumento).Specific;
                        CBDocumento.Select("0", BoSearchKey.psk_ByValue);

                        ComboBox CBfruta = (ComboBox)oForm.Items.Item(pluginForm.CBfruta).Specific;
                        CBfruta.ValidValues.Add("-", "-");
                        SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                        string sSql = "SELECT  T0.\"U_DFO_Valor\",T0.\"U_DFO_Descrip\" " +
                                      "FROM \"@DFO_OPDFO\"  T0  " +
                                      "WHERE T0.\"U_DFO_Tipo\" = 'FRUTA' ";
                        oRS.DoQuery(sSql);
                        if (oRS.RecordCount != 0)
                        {
                            while (!oRS.EoF)
                            {
                                CBfruta.ValidValues.Add(oRS.Fields.Item("U_DFO_Valor").Value.ToString(), oRS.Fields.Item("U_DFO_Descrip").Value.ToString());
                                oRS.MoveNext();
                            }
                        }

                        ((ComboBox)oForm.Items.Item(pluginForm.CBfruta).Specific).Select("-", BoSearchKey.psk_ByValue);
                    }
                    finally { oForm.Freeze(false); }

                    oForm.Visible = true;
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
                case pluginForm.ButtonOK:
                    ButtonOk(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonCancel:
                    ButtonCancel(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtCardCode.Uid:
                    TxtCardCode(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtTransportista.Uid:
                    TxtTransportista(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonGO:
                    ButtonGO(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonToday:
                    ButtonToday(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtCardName.Uid:
                    TxtCardName(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtRutTransp.Uid:
                    TxtRutTransp(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonOC:
                    ButtonOC(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonAddActivity:
                    ButtonAddActivity(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.GridOC.Uid:
                    GridOC(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonActLeft:
                    ButtonActLeft(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonActRight:
                    ButtonActRight(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonInactivaCupo:
                    ButtonInactivaCupo(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.CBdocumento:
                    CBdocumento(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtHoraProg.Uid:
                    TxtHoraProg(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.TxtRutChofer:
                    TxtRutChofer(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        private static void ButtonActLeft(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)

            {
                oForm.Freeze(true);
                if (!oItemEvent.BeforeAction)
                {
                    string date = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value;
                    //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                    DateTime oDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.CurrentUICulture);
                    oDate = oDate.AddDays(-1);
                    date = oDate.ToString("yyyyMMdd");
                    ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value = date;
                    ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();
                }
                oForm.Freeze(false);
            }
        }

        private static void ButtonActRight(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
            {
                oForm.Freeze(true);
                if (!oItemEvent.BeforeAction)
                {
                    string date = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value;
                    DateTime oDate = DateTime.ParseExact(date, "yyyyMMdd", CultureInfo.CurrentUICulture);
                    oDate = oDate.AddDays(+1);
                    date = oDate.ToString("yyyyMMdd");
                    ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value = date;
                    ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();
                }
                oForm.Freeze(false);
            }
        }

        private static void CBdocumento(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_COMBO_SELECT)
            {
                oForm.Freeze(true);
                if (!oItemEvent.BeforeAction)
                {
                    string documento = ((ComboBox)oForm.Items.Item(pluginForm.CBdocumento).Specific).Selected.Value;

                    if (documento == "1") //Guia de despacho
                    {
                        EditText CardCode = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                        CardCode.Item.Enabled = false;

                        EditText CardName = (EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific;
                        CardName.Item.Enabled = false;
                    }
                    else if (documento == "0")//Contrato
                    {
                        EditText CardCode = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                        CardCode.Item.Enabled = true;

                        EditText CardName = (EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific;
                        CardName.Item.Enabled = true;
                    }
                    else if (documento == "2")//Contrato
                    {
                        EditText CardCode = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                        CardCode.Item.Enabled = false;

                        EditText CardName = (EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific;
                        CardName.Item.Enabled = false;

                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardCode.Uds).ValueEx = "";
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardName.Uds).ValueEx = "";
                    }
                }
                oForm.Freeze(false);
            }
        }

        private static void TxtHoraProg(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            string hora = "";
            string GetHora = "";

            if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
            {
                if (!oItemEvent.BeforeAction)
                {
                    oForm.Freeze(true);
                    hora = ((EditText)oForm.Items.Item(pluginForm.TxtHoraProg.Uid).Specific).Value;
                    if (!hora.Contains(":"))
                    {
                        //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                        EditText Hora = (EditText)oForm.Items.Item(pluginForm.TxtHoraProg.Uid).Specific;
                        GetHora = formatoHora(hora);
                        Hora.Value = GetHora;
                    }
                    oForm.Freeze(false);
                }
            }
        }

        private static void TxtRutChofer(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            string RutChofer = "";

            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_KEY_DOWN && oItemEvent.CharPressed == 9)
                {
                    //oForm.Freeze(true);
                    RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                    if (RutChofer != "")
                    {
                        if (RutChofer.Contains("-"))
                        {
                            RutChofer = RutChofer.Replace(".", "");
                            if (CommonFunctions.validarRut(RutChofer))
                            {
                                //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                                EditText Rut = (EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific;
                                Rut.Value = RutChofer;
                            }
                            else
                            {
                                sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                bBubbleEvent = false;
                                // ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                            }
                        }
                        else
                        {
                            sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                            bBubbleEvent = false;
                            //((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                        }
                        //oForm.Freeze(false);
                    }
                }
            }
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_LOST_FOCUS)
                {
                    //oForm.Freeze(true);
                    RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                    if (RutChofer != "")
                    {
                        if (RutChofer.Contains("-"))
                        {
                            RutChofer = RutChofer.Replace(".", "");
                            if (CommonFunctions.validarRut(RutChofer))
                            {
                                //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                                EditText Rut = (EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific;
                                Rut.Value = RutChofer;
                            }
                            else
                            {
                                sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                bBubbleEvent = false;
                                ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                            }
                        }
                        else
                        {
                            sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                            bBubbleEvent = false;
                            ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                        }
                        //oForm.Freeze(false);
                    }
                }
            }
        }

        private static string formatoHora(string imput)

        {
            string hora = string.Empty;
            try
            {
                //DateTime.Now.ToString("HH:mm")
                imput = imput.PadRight(4, '0');
                string trabajo = imput.Substring(0, 4);
                trabajo = trabajo.Insert(2, ":");
                Regex regex = new Regex(@"^(?:[01][0-9]|2[0-3]):[0-5][0-9]$");
                Match match = regex.Match(trabajo);
                if (match.Success)
                {
                    hora = trabajo;
                }
                else
                {
                    hora = string.Empty;
                }
            }
            catch (Exception)
            {
                hora = string.Empty;
            }
            return hora;
        }

        private static void GridOC(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
            }

            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_DOUBLE_CLICK)
                {
                    if (oItemEvent.Row == -1)
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                        if (grid.DataTable.Rows.Count > 0)
                        {
                            var columna = oItemEvent.ColUID;
                            valores.sorting = columna;
                            if (valores.order == "desc")
                            {
                                valores.order = "asc";
                            }
                            else
                            {
                                valores.order = "desc";
                            }
                            ((Button)oForm.Items.Item(pluginForm.ButtonOC).Specific).Item.Click();
                        }
                    }
                }

                if (oItemEvent.EventType == BoEventTypes.et_ITEM_PRESSED)
                {
                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                    if (oItemEvent.Row >= 0)
                    {
                        int gridrow = oItemEvent.Row;
                        grid.Rows.SelectedRows.Clear();
                        grid.Rows.SelectedRows.Add(gridrow);

                        EditText CantBins = (EditText)oForm.Items.Item(pluginForm.TxtCantBins).Specific;
                        CantBins.Item.Enabled = true;

                        ComboBox CBlocalidad = (ComboBox)oForm.Items.Item(pluginForm.CBlocalidad).Specific;
                        CBlocalidad.Item.Enabled = true;

                        ComboBox CBbins = (ComboBox)oForm.Items.Item(pluginForm.CBpropBins).Specific;
                        CBbins.Item.Enabled = true;

                        ComboBox CBdestinatario = (ComboBox)oForm.Items.Item(pluginForm.CBdestinatario).Specific;
                        CBdestinatario.Item.Enabled = true;

                        Button BtnAddAct = (Button)oForm.Items.Item(pluginForm.ButtonAddActivity).Specific;
                        BtnAddAct.Item.Enabled = true;

                        sbo_application.StatusBar.SetText(gridrow.ToString(), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);

                        string documento = ((ComboBox)oForm.Items.Item(pluginForm.CBdocumento).Specific).Selected.Value;
                        if (documento != "2")
                        {
                            EditText CardCode = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                            CardCode.Value = grid.DataTable.Columns.Item(4).Cells.Item(gridrow).Value.ToString();

                            EditText CardName = (EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific;
                            CardName.Value = grid.DataTable.Columns.Item(5).Cells.Item(gridrow).Value.ToString();
                        }

                        if (documento != "2")
                        {
                            StaticText ItemCode = (StaticText)oForm.Items.Item(pluginForm.LblItemCode).Specific;//DESCRIPCION
                            ItemCode.Caption = grid.DataTable.Columns.Item(8).Cells.Item(gridrow).Value.ToString();
                            ItemCode.Item.BackColor = Colores.GreenYellow;

                            StaticText Variedad = (StaticText)oForm.Items.Item(pluginForm.LblVariedad).Specific;
                            Variedad.Caption = grid.DataTable.Columns.Item(11).Cells.Item(gridrow).Value.ToString();
                            Variedad.Item.BackColor = Colores.GreenYellow;

                            StaticText CantKg = (StaticText)oForm.Items.Item(pluginForm.LblCantKg).Specific;
                            CantKg.Caption = grid.DataTable.Columns.Item(9).Cells.Item(gridrow).Value.ToString() + " Kg";
                            CantKg.Item.BackColor = Colores.GreenYellow;
                        }
                        else
                        {
                            StaticText ItemCode = (StaticText)oForm.Items.Item(pluginForm.LblItemCode).Specific;
                            ItemCode.Caption = grid.DataTable.Columns.Item(7).Cells.Item(gridrow).Value.ToString();
                            ItemCode.Item.BackColor = Colores.GreenYellow;

                            StaticText Variedad = (StaticText)oForm.Items.Item(pluginForm.LblVariedad).Specific;
                            Variedad.Caption = grid.DataTable.Columns.Item(9).Cells.Item(gridrow).Value.ToString();
                            Variedad.Item.BackColor = Colores.GreenYellow;

                            StaticText CantKg = (StaticText)oForm.Items.Item(pluginForm.LblCantKg).Specific;
                            CantKg.Caption = grid.DataTable.Columns.Item(8).Cells.Item(gridrow).Value.ToString() + " Kg";
                            CantKg.Item.BackColor = Colores.GreenYellow;
                        }

                        if (documento != "2")
                        {
                            if (grid.DataTable.Columns.Item(9).Cells.Item(gridrow).Value.ToString() == "PASA")
                            {
                                var oEdit = (StaticText)oForm.Items.Item(pluginForm.LblOrigen).Specific;
                                oEdit.Item.Visible = true;
                                var oEdit1 = (ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific;
                                oEdit1.Item.Visible = true;
                            }
                            else if (grid.DataTable.Columns.Item(9).Cells.Item(gridrow).Value.ToString() != "PASA")
                            {
                                var oEdit = (StaticText)oForm.Items.Item(pluginForm.LblOrigen).Specific;
                                oEdit.Item.Visible = false;
                                var oEdit1 = (ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific;
                                oEdit1.Item.Visible = false;
                            }
                        }
                        else
                        {
                            if (grid.DataTable.Columns.Item(10).Cells.Item(gridrow).Value.ToString() == "PASA")
                            {
                                var oEdit = (StaticText)oForm.Items.Item(pluginForm.LblOrigen).Specific;
                                oEdit.Item.Visible = true;
                                var oEdit1 = (ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific;
                                oEdit1.Item.Visible = true;
                            }
                            else if (grid.DataTable.Columns.Item(10).Cells.Item(gridrow).Value.ToString() != "PASA")
                            {
                                var oEdit = (StaticText)oForm.Items.Item(pluginForm.LblOrigen).Specific;
                                oEdit.Item.Visible = false;
                                var oEdit1 = (ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific;
                                oEdit1.Item.Visible = false;
                            }
                        }
                    }
                }

                if (oItemEvent.EventType == BoEventTypes.et_DOUBLE_CLICK)
                {
                    oForm.Freeze(true);
                    if (oItemEvent.ColUID == "Contrato")
                    {
                        oForm.Freeze(true);
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                        if (oItemEvent.Row > -1)
                        {
                            sbo_application.OpenForm(BoFormObjectEnum.fo_PurchaseOrder, "", grid.DataTable.GetValue("InternoSAP", oItemEvent.Row).ToString());
                        }
                        oForm.Freeze(false);
                    }
                    oForm.Freeze(false);
                }
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
                    sbo_application.StatusBar.SetText("Button OK", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }
            }
        }

        private static void ButtonCancel(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                    sbo_application.StatusBar.SetText("Button Cancel", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                }
            }
        }

        private static void ButtonOC(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (!oItemEvent.BeforeAction)
            {
                Form oForm = sbo_application.Forms.Item(formUID);

                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    string documento = ((ComboBox)oForm.Items.Item(pluginForm.CBdocumento).Specific).Selected.Value;
                    if (documento == "0")//Compra
                    {
                        string CardCode = oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardCode.Uds).ValueEx;
                        try
                        {
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                            DataTable oDT = grid.DataTable;

                            string fruta = ((ComboBox)oForm.Items.Item(pluginForm.CBfruta).Specific).Selected.Value;

                            string xml = null;
                            oForm.Freeze(true);

                            string args = null;

                            if (string.IsNullOrEmpty(valores.sorting))
                            {
                                if (CardCode.Length > 0)
                                {
                                    if (!string.IsNullOrEmpty(fruta) && (fruta != "-"))
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=CodProveedor eq '{CardCode}' and Fruta eq '{fruta}' and DocStatus eq 'O'&$orderby=InternoSAP desc");
                                    }
                                    else
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=CodProveedor eq '{CardCode}' and DocStatus eq 'O'&$orderby=InternoSAP desc");
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(fruta) && (fruta != "-"))
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=DocStatus eq 'O' and Fruta eq '{fruta}'&$orderby=InternoSAP desc");
                                    }
                                    else
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=DocStatus eq 'O'&$orderby=InternoSAP desc");
                                    }
                                }
                            }
                            else
                            {
                                if (CardCode.Length > 0)
                                {
                                    if (!string.IsNullOrEmpty(fruta) && (fruta != "-"))
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=CodProveedor eq '{CardCode}' and Fruta eq '{fruta}' and DocStatus eq 'O'&$orderby={valores.sorting} {valores.order}");
                                    }
                                    else
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=CodProveedor eq '{CardCode}' and DocStatus eq 'O'&$orderby={valores.sorting} {valores.order}");
                                    }
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(fruta) && (fruta != "-"))
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=DocStatus eq 'O' and Fruta eq '{fruta}'&$orderby={valores.sorting} {valores.order}");
                                    }
                                    else
                                    {
                                        args = string.Format($"?$select=InternoSAP,Contrato,Linea,Fecha,CodProveedor,RazonSocial,Comprador,CodItem,Descripcion,Cantidad,Fruta,Variedad,Tipo&$filter=DocStatus eq 'O'&$orderby={valores.sorting} {valores.order}");
                                    }
                                }
                            }

                            string response = CommonFunctions.GET(ServiceLayer.ListadoOrdenCompra, null, args, sessionId, out _);

                            try
                            {
                                grid.DataTable.Clear();
                                xml = response.json2xml(pluginForm.GridOC.Dt);
                                grid.DataTable.LoadFromXML(xml);
                                EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("CodProveedor");
                                oColumns.LinkedObjectType = "2";

                                oColumns = (EditTextColumn)grid.Columns.Item("InternoSAP");
                                oColumns.LinkedObjectType = "22";
                                //oColumns.Visible = false;

                                oColumns = (EditTextColumn)grid.Columns.Item("CodItem");
                                oColumns.LinkedObjectType = "4";

                                grid.SelectionMode = BoMatrixSelect.ms_Single;

                                for (int i = 0; i < grid.Columns.Count; i++)
                                {
                                    grid.Columns.Item(i).Editable = false;
                                }

                                grid.AutoResizeColumns();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        finally
                        {
                            oForm.Freeze(false);
                        }
                    }
                    if (documento == "1")//Guia Despacho
                    {
                        string CardCode = oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardCode.Uds).ValueEx;
                        try
                        {
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                            DataTable oDT = grid.DataTable;

                            string xml = null;
                            oForm.Freeze(true);

                            string args = null;

                            args = string.Format($"?$select=InternoSAP,Despacho,DocStatus,Fecha,Comentarios,LineStatus,CodItem,Descripcion,Cantidad,Variedad,Tipo,Calibre,Caracteristica,ObjType&$filter=DocStatus eq 'O'&$orderby=InternoSAP");

                            string response = CommonFunctions.GET(ServiceLayer.ListadoTrasladoEntrePlantas, null, args, sessionId, out _);

                            try
                            {
                                oDT.Clear();
                                xml = response.json2xml(pluginForm.GridOC.Dt);
                                oDT.LoadFromXML(xml);
                                //EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("CodProveedor");
                                //oColumns.LinkedObjectType = "2";

                                EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("InternoSAP");
                                oColumns.LinkedObjectType = "59";
                                //oColumns.Visible = false;

                                oColumns = (EditTextColumn)grid.Columns.Item("CodItem");
                                oColumns.LinkedObjectType = "4";

                                grid.SelectionMode = BoMatrixSelect.ms_Single;

                                for (int i = 0; i < grid.Columns.Count; i++)
                                {
                                    grid.Columns.Item(i).Editable = false;
                                }

                                grid.AutoResizeColumns();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        finally
                        {
                            oForm.Freeze(false);
                        }
                    }

                    if (documento == "2")//Traslado Cancha
                    {
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardCode.Uds).ValueEx = "";
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardName.Uds).ValueEx = "";
                        string CardCode = oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardName.Uds).ValueEx;

                        EditText CardCod = (EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific;
                        CardCod.Item.AffectsFormMode = false;
                        CardCod.Item.Enabled = false;

                        EditText CardName = (EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific;
                        CardName.Item.AffectsFormMode = false;
                        CardName.Item.Enabled = false;

                        try

                        {
                            Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                            DataTable oDT = grid.DataTable;

                            oForm.Freeze(true);

                            string sSql = null;

                            string fruta = ((ComboBox)oForm.Items.Item(pluginForm.CBfruta).Specific).Selected.Value;
                            if (!string.IsNullOrEmpty(fruta) && (fruta != "-"))
                            {

                                sSql = "SELECT T0.\"DocEntry\" as \"InternoSAP\",T1.\"LineNum\" as \"Linea\", T0.\"DocStatus\",T0.\"DocDate\" as \"Fecha\",T0.\"Comments\" as \"Comentarios\"," +
                                "T1.\"LineStatus\",T1.\"ItemCode\" as \"CodItem\", T1.\"Dscription\" as \"Descripcion\",T1.\"Quantity\" as \"Cantidad\"," +
                                //"T2.\"OcrName\" as \"Fruta\", T1.\"U_FRU_Variedad\" as \"Variedad\",T1.\"U_FRU_Tipo\" as \"Tipo\",T1.\"U_FRU_Calibre\" as \"Calibre\", " +
                                " T3.\"U_FRU_Fruta\"  as \"Fruta\", T1.\"U_FRU_Variedad\" as \"Variedad\",T1.\"U_FRU_Tipo\" as \"Tipo\",T1.\"U_FRU_Calibre\" as \"Calibre\", " +
                                "T1.\"U_FRU_Caracteristica\" as \"Caracteristica\" from ODRF T0 inner join DRF1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" " +
                                //"join OOCR T2 on T1.\"OcrCode\" = T2.\"OcrCode\" " +
                                "join OITM T2 on T1.\"ItemCode\" = T2.\"ItemCode\" join OITB T3 on T2.\"ItmsGrpCod\" = T3.\"ItmsGrpCod\" " +
                                "where T0.\"DocStatus\" = 'O' and T0.\"ObjType\" = 59 and  T3.\"U_FRU_Fruta\" = '" + fruta + "'  ";
                            }
                            else
                            {
                                sSql = "SELECT T0.\"DocEntry\" as \"InternoSAP\",T1.\"LineNum\" as \"Linea\", T0.\"DocStatus\",T0.\"DocDate\" as \"Fecha\",T0.\"Comments\" as \"Comentarios\"," +
                                "T1.\"LineStatus\",T1.\"ItemCode\" as \"CodItem\", T1.\"Dscription\" as \"Descripcion\",T1.\"Quantity\" as \"Cantidad\"," +
                                //"T2.\"OcrName\" as \"Fruta\", T1.\"U_FRU_Variedad\" as \"Variedad\",T1.\"U_FRU_Tipo\" as \"Tipo\",T1.\"U_FRU_Calibre\" as \"Calibre\", " +
                                " T3.\"U_FRU_Fruta\"  as \"Fruta\", T1.\"U_FRU_Variedad\" as \"Variedad\",T1.\"U_FRU_Tipo\" as \"Tipo\",T1.\"U_FRU_Calibre\" as \"Calibre\", " +
                                "T1.\"U_FRU_Caracteristica\" as \"Caracteristica\" from ODRF T0 inner join DRF1 T1 on T0.\"DocEntry\" = T1.\"DocEntry\" " +
                                //"join OOCR T2 on T1.\"OcrCode\" = T2.\"OcrCode\" " +
                                "join OITM T2 on T1.\"ItemCode\" = T2.\"ItemCode\" join OITB T3 on T2.\"ItmsGrpCod\" = T3.\"ItmsGrpCod\" " +
                                "where T0.\"DocStatus\" = 'O' and T0.\"ObjType\" = 59 ";
                            }

                            try
                            {
                                grid.DataTable.Clear();

                                grid.DataTable.ExecuteQuery(sSql);
                                //EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("CodProveedor");
                                //oColumns.LinkedObjectType = "2";

                                EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("InternoSAP");
                                oColumns.LinkedObjectType = "112";
                                //oColumns.Visible = false;

                                oColumns = (EditTextColumn)grid.Columns.Item("CodItem");
                                oColumns.LinkedObjectType = "4";

                                grid.SelectionMode = BoMatrixSelect.ms_Single;

                                for (int i = 0; i < grid.Columns.Count; i++)
                                {
                                    grid.Columns.Item(i).Editable = false;
                                }

                                grid.AutoResizeColumns();
                            }
                            catch
                            {
                                throw;
                            }
                        }
                        finally
                        {
                            oForm.Freeze(false);
                        }
                    }

                    StaticText ItemCode = (StaticText)oForm.Items.Item(pluginForm.LblItemCode).Specific;
                    ItemCode.Caption = "";
                    ItemCode.Item.BackColor = Colores.White;

                    StaticText Variedad = (StaticText)oForm.Items.Item(pluginForm.LblVariedad).Specific;
                    Variedad.Caption = "";
                    Variedad.Item.BackColor = Colores.White;

                    StaticText CantKg = (StaticText)oForm.Items.Item(pluginForm.LblCantKg).Specific;
                    CantKg.Caption = "";
                    CantKg.Item.BackColor = Colores.White;
                }
                valores.sorting = "";
            }
        }

        private static void ButtonAddActivity(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            var oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (oItemEvent.BeforeAction)
                {
                    string CardCode = ((EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific).Value.Trim();
                    string DocDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value.Trim();
                    string documento = ((ComboBox)oForm.Items.Item(pluginForm.CBdocumento).Specific).Selected.Value;

                    if (documento != "2")
                    {
                        if ((CardCode.Length == 0))
                        {
                            sbo_application.StatusBar.SetText("Debe ingresar el cliente");
                            bBubbleEvent = false;
                        }
                    }

                    if ((DocDate.Length == 0))
                    {
                        sbo_application.StatusBar.SetText("Debe ingresar una fecha");
                        bBubbleEvent = false;
                    }

                    if (DocDate.Length != 0)
                    {
                        DateTime oDate = DateTime.ParseExact(DocDate, "yyyyMMdd", CultureInfo.CurrentUICulture);
                        if (oDate < DateTime.Today)
                        {
                            sbo_application.StatusBar.SetText("Fecha no válida, solo puede asignar cupos para hoy y fechas posteriores");
                            bBubbleEvent = false;
                        }
                    }
                }

                if (!oItemEvent.BeforeAction)
                {
                    string documento = ((ComboBox)oForm.Items.Item(pluginForm.CBdocumento).Specific).Selected.Value;
                    if (documento == "0")//Solicitud guia de despacho
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                        DataTable oDT = oForm.DataSources.DataTables.Item(pluginForm.GridOC.Dt);
                        string _DocEntry = null;
                        string _Obj = null;
                        string _Fruta = null;
                        string _Variedad = null;
                        string _Tipo = null;
                        string _ItemCode = null;
                        string _LineNum = null;

                        if (grid.Rows.SelectedRows.Count > 0)
                        {
                            int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                            _DocEntry = grid.DataTable.GetValue("InternoSAP", row).ToString();
                            _Obj = "22";
                            _ItemCode = grid.DataTable.GetValue("CodItem", row).ToString();
                            _Fruta = grid.DataTable.GetValue("Fruta", row).ToString();
                            _Tipo = grid.DataTable.GetValue("Tipo", row).ToString();
                            _Variedad = grid.DataTable.GetValue("Variedad", row).ToString();
                            _LineNum = grid.DataTable.GetValue("Linea", row).ToString();
                        }

                        string _Description = $"Recepcion {_Fruta} {_Variedad}";
                        CheckBox ChkTrasvacije = ((CheckBox)oForm.Items.Item(pluginForm.ChkTrasvacije).Specific);

                        Notes notes = new Notes
                        {
                            Codigo = _ItemCode,
                            Variedad = _Variedad,
                            Fruta = _Fruta,
                            Tipo = _Tipo,
                            RazonSocial = ((EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific).Value.Trim(),
                            LineNum = _LineNum
                        };

                        string _notes = notes.SerializeJson();

                        Activities Planificacion = new Activities
                        {
                            CardCode = ((EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific).Value.Trim(),
                            ActivityProperty = "cn_Task",
                            Details = _Description,
                            StartDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value,
                            EndDueDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value,
                            DocEntry = _DocEntry,
                            DocTypeEx = _Obj,
                            Notes = _notes,
                            Subject = "2",
                            U_DFO_CodFruta = _Fruta,
                            //HandledByRecipientList = "1",
                            //ActivityType = "1",
                            U_DFO_Trasv = (ChkTrasvacije.Checked) ? "Y" : "N",
                            U_DFO_Transportista = ((EditText)oForm.Items.Item(pluginForm.TxtTransportista.Uid).Specific).Value,
                            U_DFO_RutTransp = ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp.Uid).Specific).Value,
                            U_DFO_Chofer = ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Value,
                            U_DFO_RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value,
                            U_DFO_Patente = ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Value,
                            U_DFO_Acoplado = ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Value,
                            U_DFO_CantEnv = ((EditText)oForm.Items.Item(pluginForm.TxtCantBins).Specific).Value,
                            U_DFO_PropEnv = ((ComboBox)oForm.Items.Item(pluginForm.CBpropBins).Specific).Value//,
                                                                                                              // U_DFO_Origen = ((ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific).Value
                        };

                        int respuesta = sbo_application.MessageBox("¿Desea asignar el cupo?", 1, "Si", "No");
                        if (respuesta == 1)
                        {
                            if (Planificacion.U_DFO_Transportista == ""
                                || Planificacion.U_DFO_RutTransp == ""
                                || Planificacion.U_DFO_Chofer == ""
                                || Planificacion.U_DFO_RutChofer == ""
                                || Planificacion.U_DFO_Patente == ""
                                || Planificacion.U_DFO_CantEnv == ""
                                || Planificacion.U_DFO_PropEnv == ""
                                //|| Planificacion.U_DFO_Origen == ""
                                )
                            {
                                respuesta = sbo_application.MessageBox("¿Existen datos opcionales sin ingresar, desea continuar?", 1, "Si", "No");
                                if (respuesta == 1)
                                {
                                    var RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                                    if (RutChofer != "")
                                    {
                                        if (RutChofer.Contains("-"))
                                        {
                                            RutChofer = RutChofer.Replace(".", "");
                                            if (CommonFunctions.validarRut(RutChofer))
                                            {
                                                //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                                                EditText Rut = (EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific;
                                                Rut.Value = RutChofer;
                                            }
                                            else
                                            {
                                                sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                                ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                                                bBubbleEvent = false;
                                            }
                                        }
                                        else
                                        {
                                            sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                            ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                                            bBubbleEvent = false;
                                        }
                                        //oForm.Freeze(false);
                                    }
                                    if (bBubbleEvent)
                                    {
                                        var cantguia = ((EditText)oForm.Items.Item(pluginForm.TxtCantGuias).Specific).Value;
                                        if (int.Parse(cantguia) > 0)
                                        {
                                            for (int i = 0; i < int.Parse(cantguia); i++)
                                            {
                                                string response = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, sessionId, out _);
                                                sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var cantguia = ((EditText)oForm.Items.Item(pluginForm.TxtCantGuias).Specific).Value;
                                if (int.Parse(cantguia) > 0)
                                {
                                    for (int i = 0; i < int.Parse(cantguia); i++)
                                    {
                                        string response = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, sessionId, out _);
                                        sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                                    }
                                }
                                else
                                {
                                }
                            }
                        }
                        ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();
                    }
                    if (documento == "1")//Pedido
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                        DataTable oDT = oForm.DataSources.DataTables.Item(pluginForm.GridOC.Dt);
                        string _DocEntry = null;
                        string _Obj = null;
                        string _Fruta = null;
                        string _Variedad = null;
                        string _ItemCode = null;

                        if (grid.Rows.SelectedRows.Count > 0)
                        {
                            int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                            _DocEntry = grid.DataTable.GetValue("InternoSAP", row).ToString();
                            _Obj = "60";
                            _ItemCode = grid.DataTable.GetValue("CodItem", row).ToString();
                            _Fruta = grid.DataTable.GetValue("Fruta", row).ToString();
                            _Variedad = grid.DataTable.GetValue("Variedad", row).ToString();
                        }

                        string _Description = $"Recepcion {_Fruta} {_Variedad}";
                        CheckBox ChkTrasvacije = ((CheckBox)oForm.Items.Item(pluginForm.ChkTrasvacije).Specific);

                        Notes notes = new Notes
                        {
                            Codigo = _ItemCode,
                            Variedad = _Variedad,
                            Fruta = _Fruta,
                            RazonSocial = ((EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific).Value.Trim()
                        };

                        string _notes = notes.SerializeJson();

                        Activities Planificacion = new Activities
                        {
                            CardCode = ((EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific).Value.Trim(),
                            ActivityProperty = "cn_Task",
                            Details = _Description,
                            StartDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value,
                            EndDueDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value,
                            DocEntry = _DocEntry,
                            DocTypeEx = _Obj,
                            Notes = _notes,
                            U_DFO_CodFruta = _Fruta,
                            //HandledByRecipientList = "1",
                            //ActivityType = "1",
                            U_DFO_Trasv = (ChkTrasvacije.Checked) ? "Y" : "N",
                            U_DFO_Transportista = ((EditText)oForm.Items.Item(pluginForm.TxtTransportista.Uid).Specific).Value,
                            U_DFO_RutTransp = ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp.Uid).Specific).Value,
                            U_DFO_Chofer = ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Value,
                            U_DFO_RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value,
                            U_DFO_Patente = ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Value,
                            U_DFO_Acoplado = ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Value,
                            U_DFO_CantEnv = ((EditText)oForm.Items.Item(pluginForm.TxtCantBins).Specific).Value,
                            U_DFO_PropEnv = ((ComboBox)oForm.Items.Item(pluginForm.CBpropBins).Specific).Value//,
                            //U_DFO_Origen = ((ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific).Value
                        };

                        int respuesta = sbo_application.MessageBox("¿Desea asignar el cupo?", 1, "Si", "No");
                        if (respuesta == 1)
                        {
                            if (Planificacion.U_DFO_Transportista == ""
                                || Planificacion.U_DFO_RutTransp == ""
                                || Planificacion.U_DFO_Chofer == ""
                                || Planificacion.U_DFO_RutChofer == ""
                                || Planificacion.U_DFO_Patente == ""
                                || Planificacion.U_DFO_CantEnv == ""
                                || Planificacion.U_DFO_PropEnv == ""
                                //|| Planificacion.U_DFO_Origen == ""
                                )
                            {
                                respuesta = sbo_application.MessageBox("¿Existen datos opcionales sin ingresar, desea continuar?", 1, "Si", "No");
                                if (respuesta == 1)
                                {
                                    var RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                                    if (RutChofer != "")
                                    {
                                        if (RutChofer.Contains("-"))
                                        {
                                            RutChofer = RutChofer.Replace(".", "");
                                            if (CommonFunctions.validarRut(RutChofer))
                                            {
                                                //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                                                EditText Rut = (EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific;
                                                Rut.Value = RutChofer;
                                            }
                                            else
                                            {
                                                sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                                bBubbleEvent = false;
                                                ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                                            }
                                        }
                                        else
                                        {
                                            sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                            bBubbleEvent = false;
                                            ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                                        }
                                        //oForm.Freeze(false);
                                    }

                                    if (bBubbleEvent)
                                    {
                                        string response = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, sessionId, out System.Net.HttpStatusCode httpStatus);
                                        sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);

                                        if (httpStatus == System.Net.HttpStatusCode.Created)
                                        {
                                            sbo_application.StatusBar.SetText("Programación ingresada con exito", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);

                                            //Activities Planificacion = new Activities { Closed = "tYES" };
                                            //response = CommonFunctions.PATCH(ServiceLayer.Activities, Planificacion, guia.U_ClgCode, sessionId, out _);
                                        }
                                        else
                                        {
                                            var result = response.DeserializeJsonToDynamic();
                                            string errorMsg = result.error.message.value.ToString();
                                            sbo_application.StatusBar.SetText(errorMsg, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                                if (RutChofer != "")
                                {
                                    if (RutChofer.Contains("-"))
                                    {
                                        RutChofer = RutChofer.Replace(".", "");
                                        if (CommonFunctions.validarRut(RutChofer))
                                        {
                                            //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                                            EditText Rut = (EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific;
                                            Rut.Value = RutChofer;
                                        }
                                        else
                                        {
                                            sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                            bBubbleEvent = false;
                                        }
                                    }
                                    else
                                    {
                                        sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                        bBubbleEvent = false;
                                    }
                                }
                                if (bBubbleEvent)
                                {
                                    string response = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, sessionId, out _);
                                    sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
                                }
                            }
                        }
                        ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();
                    }
                    if (documento == "2")//Traslado Cancha
                    {
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridOC.Uid).Specific;
                        DataTable oDT = oForm.DataSources.DataTables.Item(pluginForm.GridOC.Dt);
                        string _DocEntry = null;
                        string _Obj = null;
                        string _Fruta = null;
                        string _Variedad = null;
                        string _Tipo = null;
                        string _ItemCode = null;
                        string _LineNum = null;
                        string _Descripcion = null;

                        if (grid.Rows.SelectedRows.Count > 0)
                        {
                            int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                            _DocEntry = grid.DataTable.GetValue("InternoSAP", row).ToString();
                            _Obj = "112";
                            _ItemCode = grid.DataTable.GetValue("CodItem", row).ToString();
                            _Fruta = grid.DataTable.GetValue("Fruta", row).ToString();
                            _Variedad = grid.DataTable.GetValue("Variedad", row).ToString();
                            _Tipo = grid.DataTable.GetValue("Tipo", row).ToString();
                            _LineNum = grid.DataTable.GetValue("Linea", row).ToString();
                            _Descripcion = grid.DataTable.GetValue("Descripcion", row).ToString();
                        }

                        string _Description = $"Recepcion {_Descripcion} {_Fruta} {_Variedad}";
                        CheckBox ChkTrasvacije = ((CheckBox)oForm.Items.Item(pluginForm.ChkTrasvacije).Specific);

                        Notes notes = new Notes
                        {
                            Codigo = _ItemCode,
                            Variedad = _Variedad,
                            Fruta = _Fruta,
                            Tipo = _Tipo,
                            RazonSocial = ((EditText)oForm.Items.Item(pluginForm.TxtCardName.Uid).Specific).Value.Trim(),
                            LineNum = _LineNum
                        };

                        string _notes = notes.SerializeJson();

                        Activities Planificacion = new Activities
                        {
                            CardCode = ((EditText)oForm.Items.Item(pluginForm.TxtCardCode.Uid).Specific).Value.Trim(),
                            ActivityProperty = "cn_Task",
                            Details = _Description,
                            StartDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value,
                            EndDueDate = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value,
                            DocEntry = _DocEntry,
                            DocTypeEx = _Obj,
                            Notes = _notes,
                            Subject = "2",
                            U_DFO_CodFruta = _Fruta,
                            //HandledByRecipientList = "1",
                            //ActivityType = "1",
                            U_DFO_Trasv = (ChkTrasvacije.Checked) ? "Y" : "N",
                            U_DFO_Transportista = ((EditText)oForm.Items.Item(pluginForm.TxtTransportista.Uid).Specific).Value,
                            U_DFO_RutTransp = ((EditText)oForm.Items.Item(pluginForm.TxtRutTransp.Uid).Specific).Value,
                            U_DFO_Chofer = ((EditText)oForm.Items.Item(pluginForm.TxtChofer).Specific).Value,
                            U_DFO_RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value,
                            U_DFO_Patente = ((EditText)oForm.Items.Item(pluginForm.TxtPatente).Specific).Value,
                            U_DFO_Acoplado = ((EditText)oForm.Items.Item(pluginForm.TxtAcoplado).Specific).Value,
                            U_DFO_CantEnv = ((EditText)oForm.Items.Item(pluginForm.TxtCantBins).Specific).Value,
                            U_DFO_PropEnv = ((ComboBox)oForm.Items.Item(pluginForm.CBpropBins).Specific).Value//,
                                                                                                              // U_DFO_Origen = ((ComboBox)oForm.Items.Item(pluginForm.CBorigen).Specific).Value
                        };

                        int respuesta = sbo_application.MessageBox("¿Desea asignar el cupo?", 1, "Si", "No");
                        if (respuesta == 1)
                        {
                            if (Planificacion.U_DFO_Transportista == ""
                                || Planificacion.U_DFO_RutTransp == ""
                                || Planificacion.U_DFO_Chofer == ""
                                || Planificacion.U_DFO_RutChofer == ""
                                || Planificacion.U_DFO_Patente == ""
                                || Planificacion.U_DFO_CantEnv == ""
                                || Planificacion.U_DFO_PropEnv == ""
                                //|| Planificacion.U_DFO_Origen == ""
                                )
                            {
                                respuesta = sbo_application.MessageBox("¿Existen datos opcionales sin ingresar, desea continuar?", 1, "Si", "No");
                                if (respuesta == 1)
                                {
                                    var RutChofer = ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Value;
                                    if (RutChofer != "")
                                    {
                                        if (RutChofer.Contains("-"))
                                        {
                                            RutChofer = RutChofer.Replace(".", "");
                                            if (CommonFunctions.validarRut(RutChofer))
                                            {
                                                //sbo_application.StatusBar.SetText(date, BoMessageTime.bmt_Long, BoStatusBarMessageType.smt_Warning);
                                                EditText Rut = (EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific;
                                                Rut.Value = RutChofer;
                                            }
                                            else
                                            {
                                                sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                                ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                                                bBubbleEvent = false;
                                            }
                                        }
                                        else
                                        {
                                            sbo_application.StatusBar.SetText("Ingrese RUT válido,con guión y dígito verificador", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                            ((EditText)oForm.Items.Item(pluginForm.TxtRutChofer).Specific).Item.Click();
                                            bBubbleEvent = false;
                                        }
                                        //oForm.Freeze(false);
                                    }
                                    if (bBubbleEvent)
                                    {
                                        var cantguia = ((EditText)oForm.Items.Item(pluginForm.TxtCantGuias).Specific).Value;
                                        if (int.Parse(cantguia) > 0)
                                        {
                                            for (int i = 0; i < int.Parse(cantguia); i++)
                                            {
                                                string response = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, sessionId, out _);
                                                sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                                            }
                                        }
                                        else
                                        {
                                        }
                                    }
                                }
                            }
                            else
                            {
                                var cantguia = ((EditText)oForm.Items.Item(pluginForm.TxtCantGuias).Specific).Value;
                                if (int.Parse(cantguia) > 0)
                                {
                                    for (int i = 0; i < int.Parse(cantguia); i++)
                                    {
                                        string response = CommonFunctions.POST(ServiceLayer.Activities, Planificacion, sessionId, out _);
                                        sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
                                    }
                                }
                                else
                                {
                                }
                            }
                        }
                        ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();
                    }

#if DEBUG

#endif
                }
            }
        }

        private static void ButtonInactivaCupo(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;

            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (oItemEvent.BeforeAction)
                {
                }

                if (!oItemEvent.BeforeAction)
                {
                    Grid grid = (Grid)oForm.Items.Item(pluginForm.GridAct.Uid).Specific;
                    DataTable oDT = oForm.DataSources.DataTables.Item(pluginForm.GridAct.Dt);
                    string _DocEntry = null;

                    if (grid.Rows.SelectedRows.Count > 0)
                    {
                        int respuesta = sbo_application.MessageBox("¿Desea inactivar el registro?", 1, "Si", "No");
                        if (respuesta == 1)
                        {
                            int row = grid.Rows.SelectedRows.Item(0, BoOrderType.ot_RowOrder);
                            _DocEntry = grid.DataTable.GetValue("Numero", row).ToString();

                            Activities Planificacion = new Activities
                            {
                                Closed = "tYES"
                            };

                            string response = CommonFunctions.PATCH(ServiceLayer.Activities, Planificacion, _DocEntry, sessionId, out _);

                            ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();

#if DEBUG
                            sbo_application.StatusBar.SetText(response, BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
#endif
                        }
                    }
                    else
                    {
                        sbo_application.StatusBar.SetText("Debe seleccionar un registro de actividad", BoMessageTime.bmt_Medium, BoStatusBarMessageType.smt_Warning);
                    }
                }
            }
        }

        private static void TxtCardCode(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardCode.Uds).ValueEx = oDT.GetValue("CardCode", 0).ToString();
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardName.Uds).ValueEx = oDT.GetValue("CardName", 0).ToString();

                        StaticText ItemCode = (StaticText)oForm.Items.Item(pluginForm.LblItemCode).Specific;
                        ItemCode.Caption = "";
                        ItemCode.Item.BackColor = Colores.White;

                        StaticText Variedad = (StaticText)oForm.Items.Item(pluginForm.LblVariedad).Specific;
                        Variedad.Caption = "";
                        Variedad.Item.BackColor = Colores.White;

                        StaticText CantKg = (StaticText)oForm.Items.Item(pluginForm.LblCantKg).Specific;
                        CantKg.Caption = "";
                        CantKg.Item.BackColor = Colores.White;
                    }
                }
            }
        }

        private static void TxtTransportista(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string Cookies)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    //Descomentar
                    //ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.CFLTransp);
                    //Conditions oCons = oCFL.GetConditions();
                    //Condition oCon = oCons.Add();
                    //oCon.Alias = "CardType";
                    //oCon.Operation = BoConditionOperation.co_EQUAL;
                    //oCon.CondVal = "S";
                    //oCon.Relationship = BoConditionRelationship.cr_AND;
                    //Condition oCon1 = oCons.Add();
                    //oCon1.Alias = "QryGroup2";
                    //oCon1.Operation = BoConditionOperation.co_EQUAL;
                    //oCon1.CondVal = "Y";
                    //oCFL.SetConditions(oCons);
                }
            }
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtTransportista.Uds).ValueEx = oDT.GetValue("CardName", 0).ToString();
                        oForm.DataSources.UserDataSources.Item("TxtRutTra").ValueEx = oDT.GetValue("LicTradNum", 0).ToString();
                    }
                }
            }
        }

        private static void TxtRutTransp(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string Cookies)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                }
            }
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == BoEventTypes.et_CHOOSE_FROM_LIST)
                {
                    var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as DataTable;
                    if (oDT != null)
                    {
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtTransportista.Uds).ValueEx = oDT.GetValue("CardName", 0).ToString();

                        oForm.DataSources.UserDataSources.Item("TxtRutTra").ValueEx = oDT.GetValue("LicTradNum", 0).ToString();
                    }
                }
            }
        }

        private static void ButtonGO(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                    try
                    {
                        oForm.Freeze(true);
                        string date = ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value.Trim();
                        Grid grid = (Grid)oForm.Items.Item(pluginForm.GridAct.Uid).Specific;
                        grid.Item.AffectsFormMode = false;
                        DataTable oDT = grid.DataTable;
                        EditText CantGuias = (EditText)oForm.Items.Item(pluginForm.TxtCantGuias).Specific;
                        CantGuias.Value = "1";

                        string xml = null;

                        string args = null;

                        if (date.Length > 0)
                        {
                            args = string.Format("?$select=Numero,RazonSocial,Detalle,Fecha,Contrato,Trasvasije,CantEnvases,Transportista,Patente&$filter=Recontact eq '{0}' and Asunto eq 2 &$orderby=Numero", date);
                        }

                        var response = CommonFunctions.GET(ServiceLayer.ListadoActividadesPlanificacion, null, args, sessionId, out System.Net.HttpStatusCode httpStatus);
                        if (httpStatus == System.Net.HttpStatusCode.OK)
                        {
                            try
                            {
                                oDT.Clear();
                                xml = response.json2xml(pluginForm.GridOC.Dt);
                                oDT.LoadFromXML(xml);
                                EditTextColumn oColumns = (EditTextColumn)grid.Columns.Item("Numero");
                                oColumns.LinkedObjectType = "33";
                            }
                            catch
                            {
                                sbo_application.StatusBar.SetText(string.Format("No existen recepciones programadas para el día"), BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                            }

                            for (int i = 0; i < grid.Columns.Count; i++)
                            {
                                grid.Columns.Item(i).Editable = false;
                            }
                            for (int i = 1; i <= grid.Rows.Count; i++)
                            {
                                string Actividad = oDT.GetValue("Numero", i - 1).ToString();
                                var resp = CommonFunctions.GET(ServiceLayer.Activities, Actividad, null, sessionId, out _).DeserializeJsonObject<Activities>();

                                string closed = resp.Closed;

                                if (closed == "tNO")
                                {
                                    grid.CommonSetting.SetRowBackColor(i, Colores.GreenYellow);
                                }
                                else if (closed == "tYES")
                                {
                                    grid.CommonSetting.SetRowBackColor(i, Colores.Red);
                                }
                            }
                        }
                    }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void ButtonToday(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                    try
                    {
                        oForm.Freeze(true);
                        ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Value = DateTime.Now.ToString("yyyyMMdd");
                        ((EditText)oForm.Items.Item(pluginForm.TxtDocDate.Uid).Specific).Item.AffectsFormMode = false;
                        ((Button)oForm.Items.Item(pluginForm.ButtonGO).Specific).Item.Click();
                    }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void TxtCardName(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
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
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardCode.Uds).ValueEx = oDT.GetValue("CardCode", 0).ToString();
                        oForm.DataSources.UserDataSources.Item(pluginForm.TxtCardName.Uds).ValueEx = oDT.GetValue("CardName", 0).ToString();
                    }
                }
            }
        }
    }
}