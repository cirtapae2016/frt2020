using CoreSAPB1;
using CoreUtilities;
using System;
using System.Linq;
using System.Xml;

using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

using System.Net;
using System.IO;

namespace pluginPesaje
{
    internal static class frmPesaje

    {
        private static System.Timers.Timer aTimer;
        private static Recepcion recepcion;
        private static string response;
        private static bool PesaIP;
        private static string IpPesa;

        public static void FormLoad(ref SAPbouiCOM.MenuEvent menuEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool BubbleEvent, string sessionId)
        {
            BubbleEvent = true;

            SAPbouiCOM.FormCreationParams FormCreationPackage;
            SAPbouiCOM.Form oForm = null;
            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            if (menuEvent.BeforeAction)
            {
                FormCreationPackage = (SAPbouiCOM.FormCreationParams)sbo_application.CreateObject(SAPbouiCOM.BoCreatableObjectType.cot_FormCreationParams);

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

                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;

                    string strHostName = "";
                    strHostName = System.Net.Dns.GetHostName();
                    IPHostEntry ipEntry = System.Net.Dns.GetHostEntry(strHostName);
                    IPAddress[] addr = ipEntry.AddressList;
                    string Ip = addr[1].ToString();

                    string sSql = "select T0.\"U_DFO_Valor\" ,T0.\"U_DFO_Descrip\" from \"@DFO_OPDFO\" T0 where \"U_DFO_Tipo\" = 'PESA' and T0.\"U_DFO_Descrip\" = '" + Ip + "' ";
                    oRS.DoQuery(sSql);
                    if (oRS.RecordCount == 1)
                    {
                        IpPesa = oRS.Fields.Item("U_DFO_Valor").Value.ToString();
                        PesaIP = true;
                    }






                    for (int i = 3; i < oForm.Items.Count; i++)
                    {
                        oForm.Items.Item(i).AffectsFormMode = false;
                    }

                    SAPbouiCOM.DataTable oDT = oForm.DataSources.DataTables.Add(pluginForm.GridLote.Dt);
                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                    grid.DataTable = oDT;

                    SAPbouiCOM.StaticText Peso = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPeso).Specific;
                    Peso.Item.Top = 10;
                    Peso.Item.Left = 435;
                    Peso.Item.Height = 65;
                    Peso.Item.Width = 200;
                    Peso.Item.FontSize = 60;
                    Peso.Item.BackColor = 7794517;

                    SAPbouiCOM.ComboBox CBguia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                    SAPbouiCOM.Item oItem;
                    oItem = oForm.Items.Add("40", SAPbouiCOM.BoFormItemTypes.it_RECTANGLE);
                    oItem.Top = 5;
                    oItem.Left = 430;
                    oItem.Width = 210;
                    oItem.Height = 75;
                    oItem.BackColor = 7794517;

                    SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                    Frontal.Item.Visible = false;
                    Frontal.Item.Left = 560;
                    SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                    Trasera.Item.Visible = false;
                    Trasera.Item.Left = 560;

                    //Eliminar después
                    oItem = oForm.Items.Add("101", SAPbouiCOM.BoFormItemTypes.it_EDIT);
                    oItem.Top = 5;
                    oItem.Left = 333;


                    


                    grid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single;

                    for (int i = 0; i < grid.Columns.Count; i++)
                    {
                        grid.Columns.Item(i).Editable = false;
                    }

                    oForm.PaneLevel = 0;
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                    oForm.Visible = true;
                }
                catch (Exception e)
                {
                    sbo_application.StatusBar.SetText(string.Format("FormLoad {0}", e.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
            }
        }

        //DESCOMENTAR!!!!
        private async static void HandleTimerElapsed(SAPbouiCOM.Form form)
        {
            try
            {
                if (PesaIP)
                {
                    SAPbouiCOM.StaticText Peso = (SAPbouiCOM.StaticText)form.Items.Item(pluginForm.StaticPeso).Specific;
                    string PesoReg = "";
                    PesoReg = Peso.Caption;
                    string getpeso = await System.Threading.Tasks.Task.Run(() => CommonFunctions.GetPeso(IpPesa, 5000, PesoReg));

                    Peso.Caption = int.Parse(getpeso).ToString();
                }                    
            }
            catch
            {
            }
        }
        //

        public static void ItemEventHandler(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            //DESCOMENTAR!!!
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_FORM_LOAD && !oItemEvent.BeforeAction)
            {
                SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);
                aTimer = new System.Timers.Timer();
                aTimer.Interval = 100;
                aTimer.Elapsed += (sender, e) => { HandleTimerElapsed(oForm); };
                aTimer.Enabled = true;
            }
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_FORM_CLOSE && !oItemEvent.BeforeAction)
            {
                aTimer.Enabled = false;
                aTimer.Stop();
                aTimer.Dispose();
            }
            //

            switch (oItemEvent.ItemUID)
            {
                case pluginForm.ButtonOK:
                    ButtonOk(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonCancel:
                    ButtonCacel(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.CBpesaje:
                    CBpesaje(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                //case pluginForm.ButtonGetPeso:
                //    ButtonGetPeso(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent);
                //    break;

                case pluginForm.TxtNroLlegada.Uid:
                    TxtNroLlegada(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                //case pluginForm.TxtEnvase.Uid:
                //    TxtEnvase(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                //    break;

                case pluginForm.CBguia.Uid:
                    CBguia(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonAddPeso:
                    ButtonAddPeso(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonPromedio:
                    ButtonPromedio(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.CBtransporte:
                    CBtransporte(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.CBsentido:
                    CBsentido(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.GridLote.Uid:
                    GridLote(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        private static void ButtonOk(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                }
            }
        }

        private static void ButtonCacel(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_CLICK)
            {
                if (!oItemEvent.BeforeAction)
                {
                }
            }
        }

        private static void CBpesaje(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                oForm.Freeze(true);
                if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT)
                {
                    bool CbOk = false;
                    while (CbOk == false)
                    {
                        try
                        {
                            SAPbouiCOM.ComboBox CB = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBpesaje).Specific;
                            string tPesaje = CB.Selected.Value;
                            CbOk = true;
                        }
                        catch { }
                    }

                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBpesaje).Specific;
                    string tipoPesaje = CbSelect.Selected.Value;

                    if (tipoPesaje != "")
                    {
                        if (tipoPesaje == "1")//Transporte
                        {
                            oForm.Width = 800;

                            SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                            Frontal.Item.Visible = true;
                            Frontal.Item.Left = 560;
                            Frontal.Picture = "";
                            SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                            Trasera.Item.Visible = true;
                            Trasera.Item.Left = 560;
                            Trasera.Picture = "";

                            SAPbouiCOM.StaticText Peso = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPeso).Specific;
                            Peso.Item.Top = 10;
                            Peso.Item.Left = 560;
                            Peso.Item.Height = 65;
                            Peso.Item.Width = 200;
                            Peso.Item.FontSize = 60;
                            Peso.Item.BackColor = 7794517;

                            
                            SAPbouiCOM.Item oItem;
                            oItem = oForm.Items.Item("40");
                            oItem.Top = 5;
                            oItem.Left = 555;
                            oItem.Width = 210;
                            oItem.Height = 75;
                            oItem.BackColor = 7794517;

                            SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            grid.Item.Width = 529;

                            SAPbouiCOM.Button ButtonPromedio = ((SAPbouiCOM.Button)oForm.Items.Item(pluginForm.ButtonPromedio).Specific);
                            ButtonPromedio.Item.Visible = false;

                            SAPbouiCOM.EditText TxtNroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific);
                            TxtNroLlegada.Value = "";
                            //TxtNroLlegada.Item.Enabled = true;

                            SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.TxtNroLlegada.CFL);
                            SAPbouiCOM.Conditions oCons = oCFL.GetConditions();
                            int conscount = oCons.Count;

                            if (oCons.Count == 0)
                            {
                                SAPbouiCOM.Condition oCon = oCons.Add();
                                oCon.Alias = "U_Tipo";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_NULL;
                                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                                oCon = oCons.Add();
                                oCon.Alias = "Status";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "O";
                                oCFL.SetConditions(oCons);
                            }
                            if (oCons.Count == 1)
                            {
                                SAPbouiCOM.Condition oCon = oCons.Item(0);
                                oCon.Alias = "U_Tipo";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_NOT_NULL;
                                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                                oCon = oCons.Add();
                                oCon.Alias = "Status";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "O";
                                oCFL.SetConditions(oCons);
                            }
                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();

                            SAPbouiCOM.StaticText StaticTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticTransp).Specific;
                            StaticTransp.Caption = "";

                            SAPbouiCOM.StaticText StaticRutTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutTransp).Specific;
                            StaticRutTransp.Caption = "";

                            SAPbouiCOM.StaticText StaticNombChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticNombChofer).Specific;
                            StaticNombChofer.Caption = "";

                            SAPbouiCOM.StaticText StaticRutChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutChofer).Specific;
                            StaticRutChofer.Caption = "";

                            SAPbouiCOM.StaticText StaticEnvase = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticEnvase).Specific;
                            StaticEnvase.Caption = "";

                            SAPbouiCOM.StaticText StaticPatente = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific;
                            StaticPatente.Caption = "";

                            grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            SAPbouiCOM.DataTable oDT = grid.DataTable;
                            oDT.Clear();

                            SAPbouiCOM.StaticText StaticCodEnv = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticCodEnv).Specific);
                            StaticCodEnv.Caption = "";

                            SAPbouiCOM.ComboBox CBguia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;

                            int CountCB = CBguia.ValidValues.Count;
                            if (CountCB > 0)
                            {
                                for (int i = CountCB - 1; i >= 0; i--)
                                {
                                    CBguia.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                                }
                            }
                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();

                            SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                            //CBtransporte.Item.Enabled = true;
                            CBtransporte.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);

                            SAPbouiCOM.ComboBox CBsentido = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBsentido).Specific;
                            //CBsentido.Item.Enabled = true;
                            CBsentido.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);

                            oForm.PaneLevel = 1;
                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();
                            //TxtEnvase.Item.Enabled = false;
                            //CBguia.Item.Enabled = false;
                        }
                        if (tipoPesaje == "2")//Lote
                        {

                            oForm.Width = 716;

                            SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                            Frontal.Item.Visible = false;
                            Frontal.Item.Left = 560;
                            Frontal.Picture = "";
                            SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                            Trasera.Item.Visible = false;
                            Trasera.Item.Left = 560;
                            Trasera.Picture = "";

                            SAPbouiCOM.StaticText Peso = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPeso).Specific;
                            Peso.Item.Top = 10;
                            Peso.Item.Left = 435;
                            Peso.Item.Height = 65;
                            Peso.Item.Width = 200;
                            Peso.Item.FontSize = 60;
                            Peso.Item.BackColor = 7794517;


                            SAPbouiCOM.Item oItem;
                            oItem = oForm.Items.Item("40");
                            oItem.Top = 5;
                            oItem.Left = 430;
                            oItem.Width = 210;
                            oItem.Height = 75;
                            oItem.BackColor = 7794517;

                            SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            grid.Item.Width = 529;

                            SAPbouiCOM.Button ButtonPromedio = ((SAPbouiCOM.Button)oForm.Items.Item(pluginForm.ButtonPromedio).Specific);
                            ButtonPromedio.Item.Visible = true;

                            SAPbouiCOM.EditText TxtNroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific);
                            TxtNroLlegada.Value = "";
                            //TxtNroLlegada.Item.Enabled = true;

                            SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.TxtNroLlegada.CFL);
                            SAPbouiCOM.Conditions oCons = oCFL.GetConditions();

                            int conscount = oCons.Count;

                            if (oCons.Count == 0)
                            {
                                SAPbouiCOM.Condition oCon = oCons.Add();
                                oCon.Alias = "U_Tipo";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "F";
                                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                                oCon = oCons.Add();
                                oCon.Alias = "Status";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "O";
                                oCFL.SetConditions(oCons);
                            }
                            if (oCons.Count == 1)
                            {
                                SAPbouiCOM.Condition oCon = oCons.Item(0);
                                oCon.Alias = "U_Tipo";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "F";
                                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                                oCon = oCons.Add();
                                oCon.Alias = "Status";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "O";
                                oCFL.SetConditions(oCons);
                            }
                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();

                            SAPbouiCOM.StaticText StaticTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticTransp).Specific;
                            StaticTransp.Caption = "";

                            SAPbouiCOM.StaticText StaticRutTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutTransp).Specific;
                            StaticRutTransp.Caption = "";

                            SAPbouiCOM.StaticText StaticNombChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticNombChofer).Specific;
                            StaticNombChofer.Caption = "";

                            SAPbouiCOM.StaticText StaticRutChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutChofer).Specific;
                            StaticRutChofer.Caption = "";

                            SAPbouiCOM.StaticText StaticEnvase = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticEnvase).Specific;
                            StaticEnvase.Caption = "";

                            SAPbouiCOM.StaticText StaticPatente = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific;
                            StaticPatente.Caption = "";

                            SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                            CBtransporte.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);

                            SAPbouiCOM.ComboBox CBsentido = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBsentido).Specific;
                            CBsentido.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);

                            SAPbouiCOM.StaticText StaticCodEnv = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticCodEnv).Specific);
                            StaticCodEnv.Caption = "";

                            grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            SAPbouiCOM.DataTable oDT = grid.DataTable;
                            oDT.Clear();

                            SAPbouiCOM.ComboBox CBguia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                            //CBguia.Item.Enabled = true;

                            int CountCB = CBguia.ValidValues.Count;
                            if (CountCB > 0)
                            {
                                for (int i = CountCB - 1; i >= 0; i--)
                                {
                                    CBguia.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                                }
                            }
                            oForm.PaneLevel = 2;
                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();

                            //CBtransporte.Item.Enabled = false;
                            //CBsentido.Item.Enabled = false;
                            //TxtEnvase.Item.Enabled = false;
                        }
                        if (tipoPesaje == "3")//Envase
                        {
                            SAPbouiCOM.Button ButtonPromedio = ((SAPbouiCOM.Button)oForm.Items.Item(pluginForm.ButtonPromedio).Specific);
                            ButtonPromedio.Item.Visible = false;

                            SAPbouiCOM.EditText TxtNroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific);
                            TxtNroLlegada.Value = "";
                            //TxtNroLlegada.Item.Enabled = true;

                            SAPbouiCOM.ChooseFromList oCFL = oForm.ChooseFromLists.Item(pluginForm.TxtNroLlegada.CFL);
                            SAPbouiCOM.Conditions oCons = oCFL.GetConditions();

                            int conscount = oCons.Count;

                            if (oCons.Count == 0)
                            {
                                SAPbouiCOM.Condition oCon = oCons.Add();
                                oCon.Alias = "U_Tipo";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "F";
                                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                                oCon = oCons.Add();
                                oCon.Alias = "Status";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "O";
                                oCFL.SetConditions(oCons);
                               
                            }

                            if (oCons.Count == 1)
                            {
                                SAPbouiCOM.Condition oCon = oCons.Item(0);
                                oCon.Alias = "U_Tipo";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "F";
                                oCon.Relationship = SAPbouiCOM.BoConditionRelationship.cr_AND;
                                oCon = oCons.Add();
                                oCon.Alias = "Status";
                                oCon.Operation = SAPbouiCOM.BoConditionOperation.co_EQUAL;
                                oCon.CondVal = "O";
                                oCFL.SetConditions(oCons);
                            }

                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();

                            SAPbouiCOM.StaticText StaticTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticTransp).Specific;
                            StaticTransp.Caption = "";

                            SAPbouiCOM.StaticText StaticRutTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutTransp).Specific;
                            StaticRutTransp.Caption = "";

                            SAPbouiCOM.StaticText StaticNombChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticNombChofer).Specific;
                            StaticNombChofer.Caption = "";

                            SAPbouiCOM.StaticText StaticRutChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutChofer).Specific;
                            StaticRutChofer.Caption = "";

                            SAPbouiCOM.StaticText StaticEnvase = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticEnvase).Specific;
                            StaticEnvase.Caption = "";

                            SAPbouiCOM.StaticText StaticPatente = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific;
                            StaticPatente.Caption = "";

                            SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                            CBtransporte.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);

                            SAPbouiCOM.ComboBox CBsentido = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBsentido).Specific;
                            CBsentido.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);

                            SAPbouiCOM.StaticText StaticCodEnv = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticCodEnv).Specific);
                            StaticCodEnv.Caption = "";

                            SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            SAPbouiCOM.DataTable oDT = grid.DataTable;
                            oDT.Clear();

                            SAPbouiCOM.ComboBox CBguia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                            //CBguia.Item.Enabled = true;
                            int CountCB = CBguia.ValidValues.Count;
                            if (CountCB > 0)
                            {
                                for (int i = CountCB - 1; i >= 0; i--)
                                {
                                    CBguia.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                                }
                            }
                            oForm.PaneLevel = 3;
                            ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Item.Click();

                            //CBtransporte.Item.Enabled = false;
                            //CBsentido.Item.Enabled = false;
                            //TxtEnvase.Item.Enabled = false;
                        }
                    }
                }
                oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                oForm.Freeze(false);
            }
        }

        private static void TxtNroLlegada(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                try
                {
                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBpesaje).Specific;
                    string tipoPesaje = CbSelect.Selected.Value;

                    if (tipoPesaje != "")
                    {
                        if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_CHOOSE_FROM_LIST)
                        {
                            var oDT = SAPFunctions.ChooseFromListEvent(oItemEvent) as SAPbouiCOM.DataTable;
                            if (oDT != null)
                            {
                                // oForm.Freeze(true);
                                oForm.DataSources.UserDataSources.Item(pluginForm.TxtNroLlegada.Uds).ValueEx = oDT.GetValue("DocEntry", 0).ToString();

                                string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;

                                string args = null;
                                if (NroLlegada.Length > 0)
                                {
                                    args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                                }

                                //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                                string response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                                Recepcion recepcion = response.DeserializeJsonObject<Recepcion>();

                                try
                                {
                                    if (tipoPesaje == "1")
                                    {
                                        SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                                        Frontal.Item.Visible = true;
                                        Frontal.Item.Left = 560;
                                        Frontal.Picture = "";
                                        SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                                        Trasera.Item.Visible = true;
                                        Trasera.Item.Left = 560;
                                        Trasera.Picture = "";
                                    }
                                    if (tipoPesaje == "2")
                                    {
                                        SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                                        Frontal.Item.Visible = false;
                                        Frontal.Item.Left = 560;
                                        Frontal.Picture = "";
                                        SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                                        Trasera.Item.Visible = false;
                                        Trasera.Item.Left = 560;
                                        Trasera.Picture = "";
                                    }
                                }
                                catch
                                {

                                }



                                SAPbouiCOM.StaticText StaticTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticTransp).Specific;
                                StaticTransp.Caption = recepcion.U_Transportista;

                                SAPbouiCOM.StaticText StaticRutTransp = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutTransp).Specific;
                                StaticRutTransp.Caption = recepcion.U_RUTTransp;

                                SAPbouiCOM.StaticText StaticNombChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticNombChofer).Specific;
                                StaticNombChofer.Caption = recepcion.U_Chofer;

                                SAPbouiCOM.StaticText StaticRutChofer = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticRutChofer).Specific;
                                StaticRutChofer.Caption = recepcion.U_RutChofer;

                                SAPbouiCOM.StaticText StaticEnvase = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticEnvase).Specific;
                                StaticEnvase.Caption = "";

                                SAPbouiCOM.StaticText StaticPatente = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific;
                                StaticPatente.Caption = "";

                                SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                                CBtransporte.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);

                                SAPbouiCOM.StaticText StaticWeight = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticWeight).Specific;
                                StaticWeight.Caption = "";
                                

                                SAPbouiCOM.ComboBox CBsentido = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBsentido).Specific;
                                CBsentido.Select("-", SAPbouiCOM.BoSearchKey.psk_ByValue);

                                SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                SAPbouiCOM.DataTable oDTG = grid.DataTable;
                                oDTG.Clear();

                                args = null;
                                if (NroLlegada.Length > 0)
                                {
                                    args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                                }

                                SAPbouiCOM.ComboBox CBguia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                                if (CBguia.Item.Enabled == true)
                                {
                                    //sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                                    response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                                    recepcion = response.DeserializeJsonObject<Recepcion>();

                                    //SAPbouiCOM.ComboBox CBguia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                                    int CountCB = CBguia.ValidValues.Count;
                                    if (CountCB > 0)
                                    {
                                        for (int i = CountCB - 1; i >= 0; i--)
                                        {
                                            CBguia.ValidValues.Remove(i, SAPbouiCOM.BoSearchKey.psk_Index);
                                        }
                                    }
                                    CBguia.ValidValues.Add("-", "-");
                                    foreach (var item in recepcion.DFO_TRUCK1Collection.GroupBy(i => i.U_FolioGuia))
                                    {
                                        if (item.Key != null)
                                        {
                                            CBguia.ValidValues.Add(item.Key, item.Key);
                                        }
                                    }
                                }


                                oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    //sbo_application.StatusBar.SetText(string.Format("Debe seleccionar un tipo de pesaje"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    sbo_application.StatusBar.SetText(string.Format("{0}", e.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    bBubbleEvent = false;
                }
            }
        }

        private static void CBguia(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT)
                {
                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                    string guia = CbSelect.Selected.Value;

                    if (guia != "-")
                    {
                        string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                        string args = null;

                        if (NroLlegada.Length > 0)
                        {
                            args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);
                        }

                        response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                        recepcion = response.DeserializeJsonObject<Recepcion>();

                        int cantEnvases = 0;
                        foreach (var item in recepcion.DFO_TRUCK2Collection)
                        {
                            if (item.U_FolioGuia == guia)
                            {
                                if (item.U_Lote != null)
                                {
                                    foreach (var envase in recepcion.DFO_TRUCK5Collection)
                                    {
                                        if (item.U_Lote == envase.U_Lote)
                                        {
                                            cantEnvases = cantEnvases + int.Parse(envase.U_Envases.ToString());
                                        }
                                    }
                                }
                            }
                        }
                        SAPbouiCOM.StaticText StaticEnvase = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticEnvase).Specific;
                        StaticEnvase.Caption = cantEnvases.ToString();

                        try
                        {
                            SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                            SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            SAPbouiCOM.DataTable oDT = grid.DataTable;
                            oDT.Clear();

                            string sSql = "SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",sum (T1.\"U_Envases\") as Envases,T0.\"U_PesoLote\",T0.\"U_Muestra\" " +
                            "FROM \"@DFO_TRUCK2\" T0 join  \"@DFO_TRUCK5\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" and T0.\"U_Lote\" = T1.\"U_Lote\" " +
                            "where T0.\"U_FolioGuia\" =  '" + guia + "' and T0.\"DocEntry\" = T1.\"DocEntry\" and T1.\"DocEntry\" = '" + NroLlegada + "' " +
                            "group by T0.\"LineId\",T0.\"U_Lote\",T0.\"U_FolioGuia\",T0.\"U_PesoLote\",T0.\"U_Muestra\" ";

                            oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",sum (T1.\"U_Envases\") as Envases,T0.\"U_PesoLote\",T0.\"U_Muestra\" " +
                            "FROM \"@DFO_TRUCK2\" T0 join  \"@DFO_TRUCK5\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" and T0.\"U_Lote\" = T1.\"U_Lote\" " +
                            "where T0.\"U_FolioGuia\" =  '" + guia + "' and T0.\"DocEntry\" =  T1.\"DocEntry\" and T1.\"DocEntry\" = '" + NroLlegada + "' " +
                            "group by T0.\"LineId\",T0.\"U_Lote\",T0.\"U_FolioGuia\",T0.\"U_PesoLote\",T0.\"U_Muestra\" ");

                            grid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single;

                            for (int i = 0; i < grid.Columns.Count; i++)
                            {
                                grid.Columns.Item(i).Editable = false;
                            }
                        }
                        catch (Exception e)
                        {
                            sbo_application.StatusBar.SetText(string.Format("{0}", e.Message), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                }
            }
        }

        private static void CBtransporte(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT)
                {
                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                    string transporte = CbSelect.Selected.Value;

                    if (transporte != "-")
                    {
                        string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                        string args = null;
                        if (NroLlegada.Length > 0)
                        {
                            args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                        }

                        response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                        recepcion = response.DeserializeJsonObject<Recepcion>();
                        bool CamAco = false;

                        var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "1").Count();
                        if (count > 0)
                        {
                            foreach (var Pesaje in recepcion.DFO_TRUCK3Collection)
                            {
                                if (Pesaje.U_TipoPesaje == "1")
                                {
                                    if (Pesaje.U_Patente.Contains("/"))
                                    {
                                        CamAco = true;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((transporte == "1") || (transporte == "2"))
                            {
                                CamAco = false;
                            }
                            if (transporte == "3")
                            {
                                CamAco = true;
                            }
                        }

                        if (transporte == "1")
                        {
                            if (!CamAco)
                            {
                                if (!string.IsNullOrEmpty(recepcion.U_Patente))
                                {
                                    ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = recepcion.U_Patente;
                                    SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                                }
                                else
                                {
                                    ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                    SAPbouiCOM.DataTable oDT = grid.DataTable;
                                    oDT.Clear();
                                    bBubbleEvent = false;
                                    throw new Exception("Camión no registrado para la recepción");
                                }
                            }
                            else
                            {
                                ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                SAPbouiCOM.DataTable oDT = grid.DataTable;
                                oDT.Clear();
                                bBubbleEvent = false;
                                throw new Exception("Existen pesajes asociados a Camión/Acoplado");
                            }
                        }
                        if (transporte == "2")
                        {
                            if (!CamAco)
                            {
                                if (!string.IsNullOrEmpty(recepcion.U_Carro))
                                {
                                    ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = (recepcion.U_Carro);
                                    SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                                }
                                else
                                {
                                    ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                    SAPbouiCOM.DataTable oDT = grid.DataTable;
                                    oDT.Clear();
                                    bBubbleEvent = false;
                                    throw new Exception("Acoplado no registrado para la recepción");
                                }
                            }
                            else
                            {
                                ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                SAPbouiCOM.DataTable oDT = grid.DataTable;
                                oDT.Clear();
                                bBubbleEvent = false;
                                throw new Exception("Existen pesajes asociados a Camión/Acoplado");
                            }
                        }

                        if (transporte == "3")
                        {
                            if (CamAco)
                            {
                                if (!string.IsNullOrEmpty(recepcion.U_Patente))
                                {
                                    if (!string.IsNullOrEmpty(recepcion.U_Carro))
                                    {
                                        ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = (recepcion.U_Patente + "/" + recepcion.U_Carro);
                                        SAPbouiCOM.ComboBox CBtransporte = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                                    }
                                    else
                                    {
                                        ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                        CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                        SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                        SAPbouiCOM.DataTable oDT = grid.DataTable;
                                        oDT.Clear();
                                        bBubbleEvent = false;
                                        throw new Exception("Acoplado no registrado para la recepción");
                                    }
                                }
                                else
                                {
                                    ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                    SAPbouiCOM.DataTable oDT = grid.DataTable;
                                    oDT.Clear();
                                    bBubbleEvent = false; throw new Exception("Camión no registrado para la recepción");
                                }
                            }
                            else
                            {
                                ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption = "";
                                CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                                SAPbouiCOM.DataTable oDT = grid.DataTable;
                                oDT.Clear();
                                bBubbleEvent = false;
                                throw new Exception("Existen pesajes individuales asociados a Camión o Acoplado");
                            }
                        }

                        string patente = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption;
                        if (patente != "")
                        {
                            SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                            SAPbouiCOM.DataTable oDT = grid.DataTable;
                            oDT.Clear();
                            oDT.ExecuteQuery("SELECT T0.\"U_Sentido\", T0.\"U_Patente\", T0.\"U_Kilos\", T0.\"U_Fecha\", T0.\"U_Hora\" FROM \"@DFO_TRUCK3\" T0 where T0.\"U_Patente\" = '" + patente + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                            grid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None;

                            for (int i = 0; i < grid.Columns.Count; i++)
                            {
                                grid.Columns.Item(i).Editable = false;
                            }
                        }
                    }
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                }
            }
        }

        private static void CBsentido(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (!oItemEvent.BeforeAction)
            {
                if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_COMBO_SELECT)
                {
                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBsentido).Specific;
                    string sentido = CbSelect.Selected.Value;

                    if (sentido != "-")
                    {
                        if (sentido == "E")
                        {
                            string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                            string args = "";
                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            recepcion = response.DeserializeJsonObject<Recepcion>();
                            SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                            Frontal.Item.Visible = true;
                            Frontal.Item.Left = 560;
                            Frontal.Picture = recepcion.U_CamFrontal;
                            SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                            Trasera.Item.Visible = true;
                            Trasera.Item.Left = 560;
                            Trasera.Picture = recepcion.U_CamTrasera;
                        }
                        if (sentido == "S")
                        {
                            string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                            string args = "";
                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            recepcion = response.DeserializeJsonObject<Recepcion>();
                            SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                            Frontal.Item.Visible = true;
                            Frontal.Item.Left = 560;
                            Frontal.Picture = recepcion.U_CamFrontalSal;
                            SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                            Trasera.Item.Visible = true;
                            Trasera.Item.Left = 560;
                            Trasera.Picture = recepcion.U_CamTraseraSal;
                        }
                    }
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                }
            }
        }

        private static void ButtonAddPeso(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
            {
                if (!oItemEvent.BeforeAction)
                {
                    string Lote = null;
                    string sentidoPesaje = null;
                    string NroLlegada = null;
                    string fecha = null;
                    string hora = null;
                    string Patente = null;
                    string args = null;
                    string ItemCode = null;

                    //DESCOMENTAR!!
                    //string _Peso = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPeso).Specific).Caption.ToString();
                    var _Peso = ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value.GetDoubleFromString(",");

                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;

                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBpesaje).Specific;
                    string tipoPesaje = CbSelect.Selected.Value;
                    string tipoPesajeDesc = CbSelect.Selected.Description;
                    if (tipoPesaje != "")
                    {
                        if ((tipoPesaje == "1") || (tipoPesaje == "2"))//Transporte o Fruta
                        {
                            CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBsentido).Specific;
                            sentidoPesaje = CbSelect.Selected.Value;
                            NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;

                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                            response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            recepcion = response.DeserializeJsonObject<Recepcion>();
                            DateTime date = DateTime.Now;
                            fecha = date.ToString("yyyyMMdd");
                            DateTime time = DateTime.Now;
                            hora = time.ToString("hh:mm:ss");
                            Patente = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption;
                        }
                        else if (tipoPesaje == "3") // envases
                        {
                            NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;

                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                            response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            recepcion = response.DeserializeJsonObject<Recepcion>();
                            DateTime date = DateTime.Now;
                            fecha = date.ToString("yyyyMMdd");
                            DateTime time = DateTime.Now;
                            hora = time.ToString("hh:mm:ss");
                            Patente = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption;
                        }
                    }

                    if (_Peso != 0.00)
                    {
                        if (tipoPesaje != "")
                        {
                            if (tipoPesaje == "1")//Transporte
                            {
                                SAPbouiCOM.ComboBox CbTransP = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBtransporte).Specific;
                                string tipotransp = CbTransP.Selected.Value;
                                string transdesc = CbTransP.Selected.Description;

                                string PatVeh = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPatente).Specific).Caption;
                                if (PatVeh == "")
                                {
                                    bBubbleEvent = false;
                                    throw new Exception("No se encuentra vehiculo registrado, no se puede asignar pesaje ");
                                }
                                else
                                {
                                    int Fiansw = sbo_application.MessageBox("Se asignará el peso '" + _Peso + "' al '" + transdesc + "' '" + Patente + "' ", 1, "Si", "", "Cancelar");
                                    if (Fiansw == 1)
                                    {
                                        if ((tipotransp == "1") || (tipotransp == "2"))
                                        {
                                            if (sentidoPesaje == "E")
                                            {
                                                if (tipotransp == "1")
                                                {
                                                    recepcion.U_KilosIngreso = _Peso;
                                                    recepcion.U_HoraEntrada = hora;
                                                }
                                                if (tipotransp == "2")
                                                {
                                                    recepcion.U_KilosIngAco = _Peso;
                                                    //recepcion.U_HoraEntrada = hora;
                                                }
                                            }
                                            else if (sentidoPesaje == "S")
                                            {
                                                if (tipotransp == "1")
                                                {
                                                    recepcion.U_KilosSalida = _Peso;
                                                    //recepcion.U_HoraSalida = hora;
                                                }
                                                if (tipotransp == "2")
                                                {
                                                    recepcion.U_KilosSalAco = _Peso;
                                                    recepcion.U_HoraSalida = hora;
                                                }
                                            }

                                            bool firstline = false;
                                            Recepcion_Pesaje pesaje;

                                            if (recepcion.DFO_TRUCK3Collection[0].U_Kilos == 0)
                                            {
                                                Recepcion_Pesaje Linea1 = recepcion.DFO_TRUCK3Collection[0];
                                                if (tipotransp == "1")
                                                {
                                                    Linea1.U_Sentido = "E";
                                                    Linea1.U_TipoPesaje = tipoPesaje;
                                                    Linea1.U_Patente = Patente;
                                                    Linea1.U_Lote = Lote;
                                                    Linea1.U_Kilos = _Peso;
                                                    Linea1.U_Fecha = fecha;
                                                    Linea1.U_Hora = hora;
                                                    recepcion.U_KilosIngreso = _Peso;
                                                    firstline = true;
                                                }
                                                if (tipotransp == "2")
                                                {
                                                    Linea1.U_Sentido = "E";
                                                    Linea1.U_TipoPesaje = tipoPesaje;
                                                    Linea1.U_Patente = Patente;
                                                    Linea1.U_Lote = Lote;
                                                    Linea1.U_Kilos = _Peso;
                                                    Linea1.U_Fecha = fecha;
                                                    Linea1.U_Hora = hora;
                                                    recepcion.U_KilosIngAco = _Peso;
                                                    firstline = true;
                                                }
                                            }
                                            else if (sentidoPesaje == "E")
                                            {
                                                //int count = recepcion.DFO_TRUCK3Collection.Count(item => item.U_Sentido != "E");
                                                //if (count == 0)
                                                //{
                                                pesaje = new Recepcion_Pesaje { U_Sentido = sentidoPesaje, U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                if (tipotransp == "1")
                                                {
                                                    recepcion.U_KilosIngreso = _Peso;
                                                }
                                                if (tipotransp == "2")
                                                {
                                                    recepcion.U_KilosIngAco = _Peso;
                                                }
                                                //}
                                                //else
                                                //{
                                                //    sbo_application.StatusBar.SetText("No se puede seleccionar 'Sentido Entrada' ya que existen pesajes adicionales", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                //    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                                //    bBubbleEvent = false;
                                                //}
                                            }
                                            else if (sentidoPesaje == "S")
                                            {
                                                //int count = recepcion.DFO_TRUCK3Collection.Count(item => item.U_Sentido == "E");
                                                //if (count > 0)
                                                //{
                                                pesaje = new Recepcion_Pesaje { U_Sentido = sentidoPesaje, U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);

                                                if (tipotransp == "1")
                                                {
                                                    recepcion.U_KilosSalida = _Peso;
                                                }
                                                if (tipotransp == "2")
                                                {
                                                    recepcion.U_KilosSalAco = _Peso;
                                                }

                                                //}
                                                //else if (count == 0)
                                                //{
                                                //    sbo_application.StatusBar.SetText("No se puede seleccionar 'Sentido salida' sin ingresar entrada", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                //    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                                //    bBubbleEvent = false;
                                                //}
                                            }
                                            else
                                            {
                                                //int count = recepcion.DFO_TRUCK3Collection.Count(item => item.U_Sentido == "S");
                                                //if (count == 0)
                                                //{
                                                pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                //}
                                                //else
                                                //{
                                                //    sbo_application.StatusBar.SetText("Existe un pesaje de salida, por lo que solo puede ingresar como salida ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                //    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                                //    bBubbleEvent = false;
                                                //}
                                            }
                                            if (bBubbleEvent == true)
                                            {
                                                response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);

                                                int gridcountbef = grid.Rows.Count;
                                                SAPbouiCOM.DataTable oDT = grid.DataTable;
                                                oDT.Clear();
                                                oDT.ExecuteQuery("SELECT T0.\"U_Sentido\", T0.\"U_Patente\", T0.\"U_Kilos\", T0.\"U_Fecha\", T0.\"U_Hora\" FROM \"@DFO_TRUCK3\" T0 where T0.\"U_Patente\" = '" + Patente + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                                grid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None;

                                                for (int i = 0; i < grid.Columns.Count; i++)
                                                {
                                                    grid.Columns.Item(i).Editable = false;
                                                }
                                                int gridcountaft = grid.Rows.Count;
                                                if (firstline == false)
                                                {
                                                    if (gridcountaft > gridcountbef)
                                                    {
                                                        sbo_application.StatusBar.SetText("Peso ingresado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                                        ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value = "";
                                                        string sentido = "";
                                                        sentido = sentidoPesaje;
                                                        oForm.Freeze(true);
                                                        try
                                                        {
                                                            getframe(sentido, formUID, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        oForm.Freeze(false);

                                                    }
                                                    else
                                                    {
                                                        bBubbleEvent = false;
                                                        throw new Exception("Se produjo un error al ingresar el peso, por favor intente nuevamente");
                                                    }
                                                }
                                                else if (firstline == true)
                                                {
                                                    if (grid.DataTable.GetValue("U_Sentido", 0).ToString() == "E")
                                                    {
                                                        sbo_application.StatusBar.SetText("Peso ingresado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                                        try
                                                        {
                                                            string sentido = "";
                                                            sentido = sentidoPesaje;
                                                            getframe(sentido, formUID, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        oForm.Freeze(false);
                                                    }
                                                    else
                                                    {
                                                        bBubbleEvent = false;
                                                        throw new Exception("Se produjo un error al ingresar el peso, por favor intente nuevamente");
                                                    }
                                                }
                                            }
                                        }
                                        if (tipotransp == "3")
                                        {
                                            if (sentidoPesaje == "E")
                                            {
                                                recepcion.U_KilosIngreso = _Peso;
                                                recepcion.U_HoraEntrada = hora;
                                            }
                                            else if (sentidoPesaje == "S")
                                            {
                                                recepcion.U_KilosSalida = _Peso;
                                                recepcion.U_HoraSalida = hora;
                                            }

                                            bool firstline = false;
                                            Recepcion_Pesaje pesaje;
                                            //if (recepcion.DFO_TRUCK3Collection.Count == 0)
                                            //{
                                            if (recepcion.DFO_TRUCK3Collection[0].U_Kilos == 0) //(string.IsNullOrEmpty((recepcion.DFO_TRUCK3Collection[0].U_Kilos).ToString()) )
                                            {
                                                //pesaje = new Recepcion_Pesaje { U_Sentido = sentidoPesaje, U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                //recepcion.DFO_TRUCK3Collection.Add(pesaje);

                                                //recepcion.U_KilosIngreso = _Peso;

                                                Recepcion_Pesaje Linea1 = recepcion.DFO_TRUCK3Collection[0];

                                                Linea1.U_Sentido = "E";
                                                Linea1.U_TipoPesaje = tipoPesaje;
                                                Linea1.U_Patente = Patente;
                                                Linea1.U_Lote = Lote;
                                                Linea1.U_Kilos = _Peso;
                                                Linea1.U_Fecha = fecha;
                                                Linea1.U_Hora = hora;
                                                recepcion.U_KilosIngreso = _Peso;
                                                firstline = true;
                                            }
                                            //}
                                            else if (sentidoPesaje == "E")
                                            {
                                                //int count = recepcion.DFO_TRUCK3Collection.Count(item => item.U_Sentido != "E");
                                                //if (count == 0)
                                                //{
                                                pesaje = new Recepcion_Pesaje { U_Sentido = sentidoPesaje, U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);

                                                recepcion.U_KilosIngreso = _Peso;

                                                //}
                                                //else
                                                //{
                                                //    sbo_application.StatusBar.SetText("No se puede seleccionar 'Sentido Entrada' ya que existen pesajes adicionales", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                //    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                                //    bBubbleEvent = false;
                                                //}
                                            }
                                            else if (sentidoPesaje == "S")
                                            {
                                                //int count = recepcion.DFO_TRUCK3Collection.Count(item => item.U_Sentido == "E");
                                                //if (count > 0)
                                                //{
                                                pesaje = new Recepcion_Pesaje { U_Sentido = sentidoPesaje, U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);

                                                recepcion.U_KilosSalida = _Peso;

                                                //}
                                                //else if (count == 0)
                                                //{
                                                //    sbo_application.StatusBar.SetText("No se puede seleccionar 'Sentido salida' sin ingresar entrada", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                //    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                                //    bBubbleEvent = false;
                                                //}
                                            }
                                            else
                                            {
                                                //int count = recepcion.DFO_TRUCK3Collection.Count(item => item.U_Sentido == "S");
                                                //if (count == 0)
                                                //{
                                                pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Patente = Patente, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                //}
                                                //else
                                                //{
                                                //    sbo_application.StatusBar.SetText("Existe un pesaje de salida, por lo que solo puede ingresar como salida ", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                                                //    CbSelect.Select("-", SAPbouiCOM.BoSearchKey.psk_ByDescription);
                                                //    bBubbleEvent = false;
                                                //}
                                            }
                                            if (bBubbleEvent == true)
                                            {
                                                response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out System.Net.HttpStatusCode httpStatus);
                                                if (httpStatus == System.Net.HttpStatusCode.NoContent)
                                                {
                                                }

                                                int gridcountbef = grid.Rows.Count;
                                                SAPbouiCOM.DataTable oDT = grid.DataTable;
                                                oDT.Clear();
                                                oDT.ExecuteQuery("SELECT T0.\"U_Sentido\", T0.\"U_Patente\", T0.\"U_Kilos\", T0.\"U_Fecha\", T0.\"U_Hora\" FROM \"@DFO_TRUCK3\" T0 where T0.\"U_Patente\" = '" + Patente + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                                grid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_None;

                                                for (int i = 0; i < grid.Columns.Count; i++)
                                                {
                                                    grid.Columns.Item(i).Editable = false;
                                                }
                                                int gridcountaft = grid.Rows.Count;
                                                if (firstline == false)
                                                {
                                                    if (gridcountaft > gridcountbef)
                                                    {
                                                        sbo_application.StatusBar.SetText("Peso ingresado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                                        ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value = "";
                                                        string sentido = "";
                                                        sentido=sentidoPesaje;
                                                        oForm.Freeze(true);
                                                        try
                                                        {
                                                            getframe(sentido, formUID, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        oForm.Freeze(false);
                                                    }
                                                    else
                                                    {
                                                        bBubbleEvent = false;
                                                        throw new Exception("Se produjo un error al ingresar el peso, por favor intente nuevamente");
                                                    }
                                                }
                                                else if (firstline == true)
                                                {
                                                    if (grid.DataTable.GetValue("U_Sentido", 0).ToString() == "E")
                                                    {
                                                        sbo_application.StatusBar.SetText("Peso ingresado con éxito", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                                        ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value = "";
                                                        try
                                                        {
                                                            string sentido = "";
                                                            sentido = sentidoPesaje;
                                                            getframe(sentido, formUID, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        oForm.Freeze(false);

                                                    }
                                                    else
                                                    {
                                                        bBubbleEvent = false;
                                                        throw new Exception("Se produjo un error al ingresar el peso, por favor intente nuevamente");
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        bBubbleEvent = false;
                                    }
                                    try
                                    {


                                        if (NroLlegada.Length > 0)
                                        {
                                            args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                                        }
                                        //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                                        response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                                        recepcion = response.DeserializeJsonObject<Recepcion>();

                                        if (!string.IsNullOrEmpty(recepcion.U_CamFrontal))
                                        {

                                        }
                                        if (!string.IsNullOrEmpty(recepcion.U_CamTrasera))
                                        {

                                        }
                                    }
                                    catch
                                    {

                                    }

                                }
                            }

                            if (tipoPesaje == "2")//fruta
                            {
                                if (recepcion.U_KilosIngreso > 0)
                                {
                                    if (grid.Rows.SelectedRows.Count > 0)
                                    {
                                        SAPbouiCOM.ComboBox CbGuia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                                        string guia = CbGuia.Selected.Value;
                                        int row = grid.Rows.SelectedRows.Item(0, SAPbouiCOM.BoOrderType.ot_RowOrder);
                                        int LineId = int.Parse(grid.DataTable.GetValue("LineId", row).ToString());
                                        Lote = grid.DataTable.GetValue("U_Lote", row).ToString();
                                        string Muestra = grid.DataTable.GetValue("U_Muestra", row).ToString();
                                        double PesoMuestra = (double.Parse(Muestra.Replace(".", ",")) / 1000);
                                        //StaticWeight.Caption
                                        SAPbouiCOM.StaticText StaticWeight = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticWeight).Specific;
                                        double pesoEnv = double.Parse(StaticWeight.Caption);
                                        //double pesoEnv = double.Parse(grid.DataTable.GetValue("U_PesoEnvase", row).ToString());
                                        //int cantEnv = int.Parse(grid.DataTable.GetValue("U_PesoEnvase", row).ToString());
                                        //string envase = grid.DataTable.GetValue("U_Envases", row).ToString();
                                        if (pesoEnv == 0)
                                        {
                                            int Fiansw1 = sbo_application.MessageBox("El envase no tiene peso asignado, desea actualizar con el peso estandar  ", 1, "Si", "", "Cancelar");
                                            if (Fiansw1 == 1)
                                            {
                                                //SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                                                //oRS.DoQuery("select T0.\"IWeight1\" from OITM T0 where T0.\"ItemCode\" = '" + envase + "' ");
                                                //double.Parse(oRS.Fields.Item("IWeight1").Value.ToString());

                                                //Recepcion_Pesaje pesaje = new Recepcion_Pesaje { U_TipoPesaje = "3", U_Lote = Lote, U_Kilos = double.Parse(oRS.Fields.Item("IWeight1").Value.ToString()), U_Fecha = fecha, U_Hora = hora };
                                                //recepcion.DFO_TRUCK3Collection.Add(pesaje);

                                                //recepcion.DFO_TRUCK2Collection[LineId - 1].U_PesoEnvase = CommonFunctions.GetStringFromDouble(pesaje.U_Kilos);
                                                //response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);

                                                //SAPbouiCOM.DataTable oDT = grid.DataTable;
                                                //oDT.Clear();
                                                //oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",T0.\"U_Envases\",T0.\"U_PesoLote\",T0.\"U_Muestra\",T0.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                                //args = string.Format("?$select=LineGuia,U_FolioGuia,U_Lote,U_Envases,U_PesoLote,U_Muestra,U_CodEnvase,U_PesoEnvase&$filter=DocEntry eq {0} and U_FolioGuia eq {1}", NroLlegada, guia);
                                                //response = CommonFunctions.GET(ServiceLayer.ListadoRecepciones, null, args, sessionId, out _);
                                                //string xml = CommonFunctions.json2xml(response, oDT.UniqueID);
                                            }
                                            else
                                            {
                                                bBubbleEvent = false;
                                            }
                                        }
                                        pesoEnv = double.Parse(StaticWeight.Caption);
                                        if (pesoEnv != 0)
                                        {
                                            _Peso = _Peso - (pesoEnv) - PesoMuestra;
                                            int Fiansw = sbo_application.MessageBox("Se asignará el peso '" + _Peso + "' al lote '" + Lote + "' ", 1, "Si", "", "Cancelar");
                                            if (Fiansw == 1)
                                            {
                                                Recepcion_Pesaje pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                recepcion.DFO_TRUCK2Collection.Where(i => i.U_Lote == Lote).Single().U_PesoLote = pesaje.U_Kilos;
                                                response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);

                                                
                                                   

                                                    //if (Lote.Length > 0)
                                                    //{
                                                    //    args = string.Format("?$filter=U_LoteID eq '{0}'", Lote);// docentry corresponde a numerico, el argumento no va entre comillas
                                                    //}
                                                    //response = CommonFunctions.GET(ServiceLayer.OBTCH, null, args, sessionId);
                                                    //DFO_OBTCH Lotes = CommonFunctions.DeserializeJsonObject<DFO_OBTCH>(response);
                                                    //Lotes.U_PesoLote = CommonFunctions.GetStringFromDouble(double.Parse(recepcion.U_KilosIngreso) - double.Parse(_Peso));
                                                    //Lotes.U_PesoLote = _Peso;
                                                    //response = CommonFunctions.PATCH(ServiceLayer.OBTCH, Lotes, Lotes.Code, sessionId);
                                                    //sbo_application.MessageBox(response);

                                                    SAPbouiCOM.DataTable oDT = grid.DataTable;
                                                oDT.Clear();
                                                //oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",T0.\"U_Envases\",T0.\"U_PesoLote\",T0.\"U_Muestra\",T0.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                                oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",sum (T1.\"U_Envases\") as Envases,T0.\"U_PesoLote\",T0.\"U_Muestra\" " +
                                                    "FROM \"@DFO_TRUCK2\" T0 join  \"@DFO_TRUCK5\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" and T0.\"U_Lote\" = T1.\"U_Lote\" " +
                                                    "where T0.\"U_FolioGuia\" =  '" + guia + "' and T0.\"DocEntry\" = T1.\"DocEntry\" and T1.\"DocEntry\" = '" + NroLlegada + "' " +
                                                    "group by T0.\"LineId\",T0.\"U_Lote\",T0.\"U_FolioGuia\",T0.\"U_PesoLote\",T0.\"U_Muestra\" ");
                                                args = string.Format("?$select=LineGuia,U_FolioGuia,U_Lote,U_Envases,U_PesoLote,U_Muestra,U_CodEnvase,U_PesoEnvase&$filter=DocEntry eq {0} and U_FolioGuia eq {1}", NroLlegada, guia);
                                                response = CommonFunctions.GET(ServiceLayer.ListadoRecepciones, null, args, sessionId, out _);
                                                string xml = response.json2xml(oDT.UniqueID);
#if DEBUG
                                                sbo_application.StatusBar.SetText(response, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                                sbo_application.StatusBar.SetText(xml, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                                                //oDT.LoadFromXML(xml);

                                                sbo_application.StatusBar.SetText("Peso actualizado correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                                ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value = "";
                                            }
                                        }
                                        else
                                        {
                                            bBubbleEvent = false;
                                            throw new Exception("No existen pesos asignados al envase, por favor verificar");
                                            //sbo_application.StatusBar.SetText("No existen pesos asignados al envase, por favor verificar", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        }
                                    }
                                    else
                                    {
                                        bBubbleEvent = false;
                                        throw new Exception("Debe Seleccionar un lote");
                                        //sbo_application.StatusBar.SetText("Debe Seleccionar un lote", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                    }
                                }
                                else
                                {
                                    bBubbleEvent = false;
                                    throw new Exception("Debe pesar el camion completo antes de pesar lotes");
                                }
                            }

                            if (tipoPesaje == "3")//Envases
                            {
                                ItemCode = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticCodEnv).Specific).Caption;
                                if (ItemCode.Length > 0)
                                {
                                    args = string.Format("?$filter=ItemCode eq '{0}'", ItemCode);// docentry corresponde a numerico, el argumento no va entre comillas

                                    SAPbouiCOM.StaticText StaticItemName = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticItemName).Specific;
                                    SAPbouiCOM.ComboBox CbGuia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                                    string guia = CbGuia.Selected.Value;
                                    int row = grid.Rows.SelectedRows.Item(0, SAPbouiCOM.BoOrderType.ot_RowOrder);
                                    int LineId = int.Parse(grid.DataTable.GetValue("LineId", row).ToString());
                                    int Fiansw = sbo_application.MessageBox("Se asignará el peso '" + _Peso + "' al envase '" + StaticItemName.Caption + "' ", 1, "Si", "", "Cancelar");

                                    if (Fiansw == 1)
                                    {
                                        Recepcion_Pesaje pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Lote = Lote, U_Kilos = _Peso, U_Fecha = fecha, U_Hora = hora };
                                        recepcion.DFO_TRUCK3Collection.Add(pesaje);

                                        recepcion.DFO_TRUCK2Collection[LineId - 1].U_PesoEnvase = pesaje.U_Kilos.GetStringFromDouble(2);
                                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);

                                        SAPbouiCOM.DataTable oDT = grid.DataTable;
                                        oDT.Clear();
                                        oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",T0.\"U_Envases\",T0.\"U_PesoLote\",T0.\"U_Muestra\",T0.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                        args = string.Format("?$select=LineGuia,U_FolioGuia,U_Lote,U_Envases,U_PesoLote,U_Muestra,U_CodEnvase,U_PesoEnvase&$filter=DocEntry eq {0} and U_FolioGuia eq {1}", NroLlegada, guia);
                                        response = CommonFunctions.GET(ServiceLayer.ListadoRecepciones, null, args, sessionId, out _);
                                        string xml = response.json2xml(oDT.UniqueID);
#if DEBUG
                                        sbo_application.StatusBar.SetText(response, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        sbo_application.StatusBar.SetText(xml, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                                        //oDT.LoadFromXML(xml);

                                        sbo_application.StatusBar.SetText("Peso actualizado correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                        ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value = "";

                                        //if (Lote.Length > 0)
                                        //{
                                        //    args = string.Format("?$filter=U_LoteID eq '{0}'", Lote);// docentry corresponde a numerico, el argumento no va entre comillas
                                        //}
                                        //response = CommonFunctions.GET(ServiceLayer.OBTCH, null, args, sessionId);
                                        //DFO_OBTCH Lotes = CommonFunctions.DeserializeJsonObject<DFO_OBTCH>(response);
                                        //Lotes.U_PesoLote = CommonFunctions.GetStringFromDouble(double.Parse(recepcion.U_KilosIngreso) - double.Parse(_Peso));
                                        //Lotes.U_PesoEnvase = CommonFunctions.GetStringFromDouble(_Peso);

                                        //response = CommonFunctions.PATCH(ServiceLayer.OBTCH, Lotes, Lotes.Code, sessionId);
                                        //response = CommonFunctions.GET(ServiceLayer.OBTCH, null, args, sessionId);

                                        //if (Lotes.U_PesoEnvase == CommonFunctions.GetStringFromDouble(_Peso))
                                        //{
                                        //    SAPbouiCOM.ComboBox CbGuia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                                        //    string guia = CbGuia.Selected.Value;

                                        //    SAPbouiCOM.StaticText StaticWeight = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticWeight).Specific;
                                        //    StaticWeight.Caption = Lotes.U_PesoEnvase;
                                        //    SAPbouiCOM.DataTable oDT = grid.DataTable;
                                        //    oDT.Clear();
                                        //    oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T1.\"U_LoteID\",T1.\"U_Envases\",T1.\"U_PesoLote\",T0.\"U_Muestra\",T1.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 join \"@DFO_OBTCH\" T1 On T0.\"U_Lote\" = T1.\"U_LoteID\" where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                        //    grid.SelectionMode = SAPbouiCOM.BoMatrixSelect.ms_Single;

                                        //    for (int i = 0; i < grid.Columns.Count; i++)
                                        //    {
                                        //        grid.Columns.Item(i).Editable = false;
                                        //    }

                                        //    sbo_application.StatusBar.SetText("Peso actualizado correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                        //}
                                        //else
                                        //{
                                        //    sbo_application.StatusBar.SetText("Se produjo un error al ingresar, por favor intente nuevamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        //    bBubbleEvent = false;
                                        //}
                                    }
                                    else
                                    {
                                        bBubbleEvent = false;
                                    }
                                }
                                else
                                {
                                    bBubbleEvent = false;
                                    throw new Exception("Debe Seleccionar un Envase");
                                }
                            }
                        }
                    }
                    else
                    {
                        bBubbleEvent = false;
                        throw new Exception("El peso no puede ser 0, por favor reintente");
                    }
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                }
            }
        }

        private static void ButtonPromedio(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);

            bBubbleEvent = true;
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
            {
                if (!oItemEvent.BeforeAction)
                {
                    string Lote = null;
                    string sentidoPesaje = null;
                    string NroLlegada = null;
                    string fecha = null;
                    string hora = null;
                    string Patente = null;
                    string args = null;
                    string ItemCode = null;

                    DateTime date = DateTime.Now;
                    fecha = date.ToString("yyyyMMdd");
                    DateTime time = DateTime.Now;
                    hora = time.ToString("hh:mm:ss");

                    //DESCOMENTAR!!
                    //string _Peso = ((SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticPeso).Specific).Caption.ToString();
                    var _Peso = ((SAPbouiCOM.EditText)oForm.Items.Item("101").Specific).Value.GetDoubleFromString(",");

                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                    SAPbouiCOM.ComboBox CbGuia = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBguia.Uid).Specific;
                    string guia = CbGuia.Selected.Value;
                    SAPbouiCOM.ComboBox CbSelect = (SAPbouiCOM.ComboBox)oForm.Items.Item(pluginForm.CBpesaje).Specific;
                    string tipoPesaje = CbSelect.Selected.Value;

                    if (tipoPesaje != "")
                    {
                        if (tipoPesaje == "2")//Solo lotes
                        {
                            NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;

                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                            response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            recepcion = response.DeserializeJsonObject<Recepcion>();
                        }
                    }

                    if (tipoPesaje != "")
                    {
                        if (tipoPesaje == "2")//fruta
                        {

                            double _envLote = 0;
                            double _envLoteRech = 0;

                            int LoteAprCount = recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y").Count(item => item.U_PesoLote > 0);
                            int LoteRechCount = recepcion.DFO_TRUCK2Collection.Where(i => string.IsNullOrEmpty(i.U_Aprobado)).Count(item => item.U_PesoLote == 0);
                            int LoteCount = recepcion.DFO_TRUCK2Collection.Count();

                            if ((LoteAprCount + LoteRechCount) == LoteCount)
                            {
                                
                                    double _sumaLotes = recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y").Sum(item => item.U_PesoLote);//recepcion.DFO_TRUCK2Collection.Sum(item => item.U_PesoLote);
                                    _sumaLotes = _sumaLotes + recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y").Sum(item => ((item.U_Muestra).GetDoubleFromString(",") / 1000));
                                    foreach (var env in recepcion.DFO_TRUCK5Collection)
                                    {
                                        if (recepcion.DFO_TRUCK2Collection.Where(i => i.U_Lote == env.U_Lote && i.U_Aprobado == "Y").Count() > 0)
                                        {
                                            var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                                            if (env.U_Envases < 0 || env.U_Envases == null)
                                                throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                                            if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                                                throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                                            _envLote += ((double)env.U_Envases);
                                        }
                                    }
                                    double PromedioLote = 0;
                                    PromedioLote = _sumaLotes / _envLote;
                                    sbo_application.MessageBox("Promedio Lotes " + (PromedioLote.ToString()));

                                    foreach (var lote in recepcion.DFO_TRUCK2Collection.Where(i => string.IsNullOrEmpty(i.U_Aprobado)))
                                    {
                                        if (lote.U_FolioGuia == guia)
                                        {
                                            if (lote.U_PesoLote == 0)
                                            {
                                                int envaselote = 0;
                                                foreach (var envase in recepcion.DFO_TRUCK5Collection)
                                                {
                                                    if (lote.U_Lote == envase.U_Lote)
                                                    {
                                                        envaselote = envaselote + int.Parse(envase.U_Envases.ToString());
                                                    }
                                                }
                                                double muestra = (lote.U_Muestra.GetDoubleFromString(",")) / 1000;
                                                double PesoLote = (PromedioLote * envaselote) - muestra;//
                                                Recepcion_Pesaje pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Lote = lote.U_Lote, U_Kilos = PesoLote, U_Fecha = fecha, U_Hora = hora };
                                                recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                lote.U_PesoLote = PesoLote;
                                                response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);
                                            }
                                        }
                                    }
                                    SAPbouiCOM.DataTable oDT = grid.DataTable;
                                    oDT.Clear();
                                    //oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",T0.\"U_Envases\",T0.\"U_PesoLote\",T0.\"U_Muestra\",T0.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                    oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",sum (T1.\"U_Envases\") as Envases,T0.\"U_PesoLote\",T0.\"U_Muestra\" " +
                                        "FROM \"@DFO_TRUCK2\" T0 join  \"@DFO_TRUCK5\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" and T0.\"U_Lote\" = T1.\"U_Lote\" " +
                                        "where T0.\"U_FolioGuia\" =  '" + guia + "' and T0.\"DocEntry\" = T1.\"DocEntry\" and T1.\"DocEntry\" = '" + NroLlegada + "' " +
                                        "group by T0.\"LineId\",T0.\"U_Lote\",T0.\"U_FolioGuia\",T0.\"U_PesoLote\",T0.\"U_Muestra\" ");
                                    args = string.Format("?$select=LineGuia,U_FolioGuia,U_Lote,U_Envases,U_PesoLote,U_Muestra,U_CodEnvase,U_PesoEnvase&$filter=DocEntry eq {0} and U_FolioGuia eq {1}", NroLlegada, guia);
                                    response = CommonFunctions.GET(ServiceLayer.ListadoRecepciones, null, args, sessionId, out _);
                                    string xml = response.json2xml(oDT.UniqueID);
#if DEBUG
                                    sbo_application.StatusBar.SetText(response, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                    sbo_application.StatusBar.SetText(xml, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                                    //oDT.LoadFromXML(xml);

                                    sbo_application.StatusBar.SetText("Peso actualizado correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                
                            }
                            else
                            {


                                if (recepcion.DFO_TRUCK5Collection.Count(i => i.U_CodEnvase != null) > 0)
                                {
                                    foreach (var env in recepcion.DFO_TRUCK5Collection)
                                    {
                                        if (recepcion.DFO_TRUCK2Collection.Where(i => i.U_Lote == env.U_Lote && i.U_Aprobado == "Y").Count() > 0)
                                        {
                                            var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                                            if (env.U_Envases < 0 || env.U_Envases == null)
                                                throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                                            if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                                                throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                                            _envLote += ((double)env.U_Envases * (double)item.InventoryWeight);
                                        }

                                        if (recepcion.DFO_TRUCK2Collection.Where(i => i.U_Lote == env.U_Lote && (string.IsNullOrEmpty(i.U_Aprobado)||(i.U_Aprobado == "N"))).Count() > 0)
                                        {
                                            var item = CommonFunctions.GET(ServiceLayer.Items, env.U_CodEnvase, null, sessionId, out _).DeserializeJsonObject<CoreUtilities.Items>();

                                            if (env.U_Envases < 0 || env.U_Envases == null)
                                                throw new Exception($"Envase {env.U_CodEnvase} con cantidad 0 o vacio");

                                            if (item.InventoryWeight < 0 || item.InventoryWeight == null)
                                                throw new Exception($"Envase {env.U_CodEnvase} con peso 0 o vacio");

                                            _envLoteRech += ((double)env.U_Envases * (double)item.InventoryWeight);
                                        }
                                    }
                                }


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

                                        _envEnt += (((double)env.U_Envases * (double)item.InventoryWeight) - _envLoteRech);
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

                                        _envSal += (((double)env.U_Envases * (double)item.InventoryWeight) - _envLoteRech);
                                    }
                                }

                                
                                double _sumaLotes = recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y").Sum(item => item.U_PesoLote);//recepcion.DFO_TRUCK2Collection.Sum(item => item.U_PesoLote);
                                _sumaLotes = _sumaLotes + recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y" && i.U_PesoLote >0).Sum(item => ((item.U_Muestra).GetDoubleFromString(",") / 1000));

                                double _tara = 0;
                                var count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();
                                if (count == 0)
                                {
                                    if ((!string.IsNullOrEmpty(recepcion.U_Patente)) && (string.IsNullOrEmpty(recepcion.U_Carro)))
                                    {
                                        if ((recepcion.U_KilosIngreso > 0) && ((recepcion.U_KilosSalida > 0) && (recepcion.U_KilosSalida < recepcion.U_KilosIngreso)))
                                        {
                                            _tara = ((double)recepcion.U_KilosIngreso - _envEnt) - ((double)recepcion.U_KilosSalida - _envSal);
                                        }
                                        else
                                        {
                                            bBubbleEvent = false;
                                            throw new Exception("Debe pesar el transporte completo antes de pesar lotes");
                                        }
                                    }
                                    if ((!string.IsNullOrEmpty(recepcion.U_Patente)) && (!string.IsNullOrEmpty(recepcion.U_Carro)))
                                    {
                                        if ((recepcion.U_KilosIngreso > 0) && ((recepcion.U_KilosSalida > 0) && (recepcion.U_KilosSalida < recepcion.U_KilosIngreso)))
                                        {
                                            if (!recepcion.DFO_TRUCK3Collection[0].U_Patente.Contains("/"))
                                            {
                                                if ((recepcion.U_KilosIngAco > 0) && ((recepcion.U_KilosSalAco > 0) && (recepcion.U_KilosSalAco < recepcion.U_KilosIngAco)))
                                                {
                                                    _tara = ((((double)recepcion.U_KilosIngreso + (double)recepcion.U_KilosIngAco) - _envEnt) - (((double)recepcion.U_KilosSalida + (double)recepcion.U_KilosSalAco)) - _envSal);
                                                }
                                                else
                                                {
                                                    bBubbleEvent = false;
                                                    throw new Exception("Debe pesar el transporte completo antes de pesar lotes");
                                                }
                                            }
                                            else
                                            {
                                                if ((recepcion.U_KilosIngreso > 0) && ((recepcion.U_KilosSalida > 0) && (recepcion.U_KilosSalida < recepcion.U_KilosIngreso)))
                                                {
                                                    _tara = ((double)recepcion.U_KilosIngreso - _envEnt) - ((double)recepcion.U_KilosSalida - _envSal);
                                                }
                                                else
                                                {
                                                    bBubbleEvent = false;
                                                    throw new Exception("Debe pesar el transporte completo antes de pesar lotes");
                                                }
                                            }
                                        }
                                        else
                                        {
                                            bBubbleEvent = false;
                                            throw new Exception("Debe pesar el transporte completo antes de pesar lotes");
                                        }
                                    }
                                }
                                count = recepcion.DFO_TRUCK3Collection.Where(i => i.U_TipoPesaje == "3").Count();

                                if (count > 0)
                                {
                                    if ((recepcion.U_KilosIngreso > 0) && ((recepcion.U_KilosSalida > 0) && (recepcion.U_KilosSalida < recepcion.U_KilosIngreso)))
                                    {
                                        _tara = ((double)recepcion.U_KilosIngreso - _envEnt) - ((double)recepcion.U_KilosSalida - _envSal);
                                    }
                                    else
                                    {
                                        bBubbleEvent = false;
                                        throw new Exception("Debe pesar el transporte completo antes de pesar lotes");
                                    }
                                }



                                string LoteMax = "";
                                double PesoLotes = _sumaLotes;
                                double PesoEnvases = _envLote;
                                double EnvSinPeso = 0;
                                int CantEnvase = 0;
                                bool LoteSinPeso = false;
                                double PromedioLote = 0;

                                int _LoteSinPeso = recepcion.DFO_TRUCK2Collection.Count(item => item.U_PesoLote == 0);
                                if (_LoteSinPeso > 0)
                                {
                                    LoteSinPeso = true;
                                }

                                if (PesoLotes > 0)
                                {
                                    if (LoteSinPeso == true)
                                    {
                                        int Fiansw1 = sbo_application.MessageBox("Se asignará a los lotes restantes el promedio, esta seguro? ", 1, "Si", "", "Cancelar");
                                        if (Fiansw1 == 1)
                                        {
                                            double PesoEnvase = 0;
                                            CantEnvase = 0;
                                            foreach (var lote in recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y"))
                                            {
                                                if (lote.U_PesoLote == 0)
                                                {
                                                    foreach (var envase in recepcion.DFO_TRUCK5Collection)
                                                    {
                                                        if (lote.U_Lote == envase.U_Lote)
                                                        {
                                                            args = string.Format("?$filter=ItemCode eq '{0}'", envase.U_CodEnvase);// docentry corresponde a numerico, el argumento no va entre comillas
                                                            response = CommonFunctions.GET(ServiceLayer.Items, null, args, sessionId, out _);
                                                            Items item = response.DeserializeJsonObject<Items>();
                                                            if (item != null)
                                                            {
                                                                PesoEnvase = PesoEnvase + ((int)envase.U_Envases * double.Parse((item.SalesUnitWeight.Value.ToString()).Replace(".", ",")));
                                                            }
                                                            CantEnvase = CantEnvase + int.Parse(envase.U_Envases.ToString());
                                                        }
                                                    }
                                                }
                                                EnvSinPeso = EnvSinPeso + PesoEnvase;
                                            }

                                            PromedioLote = _tara - PesoLotes;

                                            PromedioLote = PromedioLote / CantEnvase;

                                            sbo_application.MessageBox("_tara " + (_tara.ToString()));
                                            sbo_application.MessageBox("Peso lotes aprobados con peso " + (PesoLotes.ToString()));
                                            sbo_application.MessageBox("Peso Envases con peso " + ((PesoEnvases - EnvSinPeso).ToString()));
                                            sbo_application.MessageBox("Peso Envases Sin Peso " + (EnvSinPeso.ToString()));
                                            sbo_application.MessageBox("Cantidad envases sin peso " + (CantEnvase.ToString()));
                                            sbo_application.MessageBox("Promedio diferencia " + (PromedioLote.ToString()));

                                            foreach (var lote in recepcion.DFO_TRUCK2Collection)
                                            {
                                                if (lote.U_FolioGuia == guia)
                                                {
                                                    if (lote.U_PesoLote == 0)
                                                    {
                                                        int envaselote = 0;
                                                        foreach (var envase in recepcion.DFO_TRUCK5Collection)
                                                        {
                                                            if (lote.U_Lote == envase.U_Lote)
                                                            {
                                                                envaselote = envaselote + int.Parse(envase.U_Envases.ToString());
                                                            }
                                                        }
                                                        double muestra = (lote.U_Muestra.GetDoubleFromString(",")) / 1000;
                                                        double PesoLote = (PromedioLote * envaselote) - muestra;//
                                                        Recepcion_Pesaje pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Lote = lote.U_Lote, U_Kilos = PesoLote, U_Fecha = fecha, U_Hora = hora };
                                                        recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                        lote.U_PesoLote = PesoLote;
                                                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);
                                                    }
                                                }
                                            }
                                        }
                                        SAPbouiCOM.DataTable oDT = grid.DataTable;
                                        oDT.Clear();
                                        //oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",T0.\"U_Envases\",T0.\"U_PesoLote\",T0.\"U_Muestra\",T0.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                        oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",sum (T1.\"U_Envases\") as Envases,T0.\"U_PesoLote\",T0.\"U_Muestra\" " +
                                            "FROM \"@DFO_TRUCK2\" T0 join  \"@DFO_TRUCK5\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" and T0.\"U_Lote\" = T1.\"U_Lote\" " +
                                            "where T0.\"U_FolioGuia\" =  '" + guia + "' and T0.\"DocEntry\" = T1.\"DocEntry\" and T1.\"DocEntry\" = '" + NroLlegada + "' " +
                                            "group by T0.\"LineId\",T0.\"U_Lote\",T0.\"U_FolioGuia\",T0.\"U_PesoLote\",T0.\"U_Muestra\" ");
                                        args = string.Format("?$select=LineGuia,U_FolioGuia,U_Lote,U_Envases,U_PesoLote,U_Muestra,U_CodEnvase,U_PesoEnvase&$filter=DocEntry eq {0} and U_FolioGuia eq {1}", NroLlegada, guia);
                                        response = CommonFunctions.GET(ServiceLayer.ListadoRecepciones, null, args, sessionId, out _);
                                        string xml = response.json2xml(oDT.UniqueID);
#if DEBUG
                                        sbo_application.StatusBar.SetText(response, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        sbo_application.StatusBar.SetText(xml, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                                        //oDT.LoadFromXML(xml);

                                        sbo_application.StatusBar.SetText("Peso actualizado correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                    }
                                    else
                                    {
                                        sbo_application.StatusBar.SetText("Todos los lotes tienen peso asignado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        bBubbleEvent = false;
                                    }
                                }
                                else if (PesoLotes == 0)
                                {
                                    if (LoteSinPeso == true)
                                    {
                                        int Fiansw1 = sbo_application.MessageBox("Se asignará el promedio a los lotes, esta seguro? ", 1, "Si", "", "Cancelar");
                                        if (Fiansw1 == 1)
                                        {
                                            double PesoEnvase = 0;
                                            CantEnvase = 0;
                                            foreach (var lote in recepcion.DFO_TRUCK2Collection.Where(i => i.U_Aprobado == "Y"))
                                            {
                                                if (lote.U_PesoLote == 0)
                                                {
                                                    foreach (var envase in recepcion.DFO_TRUCK5Collection)
                                                    {
                                                        if (lote.U_Lote == envase.U_Lote)
                                                        {
                                                            args = string.Format("?$filter=ItemCode eq '{0}'", envase.U_CodEnvase);// docentry corresponde a numerico, el argumento no va entre comillas
                                                            response = CommonFunctions.GET(ServiceLayer.Items, null, args, sessionId, out _);
                                                            Items item = response.DeserializeJsonObject<Items>();
                                                            if (item != null)
                                                            {
                                                                PesoEnvase = PesoEnvase + ((int)envase.U_Envases * double.Parse((item.SalesUnitWeight.Value.ToString()).Replace(".", ",")));
                                                            }
                                                            CantEnvase = CantEnvase + int.Parse(envase.U_Envases.ToString());
                                                        }
                                                    }
                                                }
                                                EnvSinPeso = PesoEnvase;
                                            }

                                            PromedioLote = _tara;

                                            PromedioLote = PromedioLote / CantEnvase;

                                            sbo_application.MessageBox("_tara " + (_tara.ToString()));
                                            sbo_application.MessageBox("Peso lotes con peso " + (PesoLotes.ToString()));
                                            sbo_application.MessageBox("Peso Envases " + (PesoEnvases.ToString()));
                                            sbo_application.MessageBox("Peso Envases Sin Peso " + (EnvSinPeso.ToString()));
                                            sbo_application.MessageBox("Cantidad envases sin peso " + (CantEnvase.ToString()));
                                            sbo_application.MessageBox("Promedio " + (PromedioLote.ToString()));

                                            foreach (var lote in recepcion.DFO_TRUCK2Collection)
                                            {
                                                if (lote.U_FolioGuia == guia)
                                                {
                                                    if (lote.U_PesoLote == 0)
                                                    {
                                                        int envaselote = 0;
                                                        foreach (var envase in recepcion.DFO_TRUCK5Collection)
                                                        {
                                                            if (lote.U_Lote == envase.U_Lote)
                                                            {
                                                                envaselote = envaselote + int.Parse(envase.U_Envases.ToString());
                                                            }
                                                        }

                                                        double muestra = (lote.U_Muestra.GetDoubleFromString(",")) / 1000;
                                                        double PesoLote = (PromedioLote * envaselote) - muestra;//
                                                        Recepcion_Pesaje pesaje = new Recepcion_Pesaje { U_TipoPesaje = tipoPesaje, U_Lote = lote.U_Lote, U_Kilos = PesoLote, U_Fecha = fecha, U_Hora = hora };
                                                        recepcion.DFO_TRUCK3Collection.Add(pesaje);
                                                        lote.U_PesoLote = PesoLote;
                                                        response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);
                                                    }
                                                }
                                            }
                                        }
                                        SAPbouiCOM.DataTable oDT = grid.DataTable;
                                        oDT.Clear();
                                        //oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",T0.\"U_Envases\",T0.\"U_PesoLote\",T0.\"U_Muestra\",T0.\"U_CodEnvase\",T0.\"U_PesoEnvase\" FROM \"@DFO_TRUCK2\" T0 where T0.\"U_FolioGuia\" = '" + guia + "' and T0.\"DocEntry\" = '" + NroLlegada + "' ");
                                        oDT.ExecuteQuery("SELECT T0.\"LineId\",T0.\"U_FolioGuia\",T0.\"U_Lote\",sum (T1.\"U_Envases\") as Envases,T0.\"U_PesoLote\",T0.\"U_Muestra\" " +
                                            "FROM \"@DFO_TRUCK2\" T0 join  \"@DFO_TRUCK5\" T1 on T0.\"DocEntry\" = T1.\"DocEntry\" and T0.\"U_Lote\" = T1.\"U_Lote\" " +
                                            "where T0.\"U_FolioGuia\" =  '" + guia + "' and T0.\"DocEntry\" = T1.\"DocEntry\" and T1.\"DocEntry\" = '" + NroLlegada + "' " +
                                            "group by T0.\"LineId\",T0.\"U_Lote\",T0.\"U_FolioGuia\",T0.\"U_PesoLote\",T0.\"U_Muestra\" ");
                                        args = string.Format("?$select=LineGuia,U_FolioGuia,U_Lote,U_Envases,U_PesoLote,U_Muestra,U_CodEnvase,U_PesoEnvase&$filter=DocEntry eq {0} and U_FolioGuia eq {1}", NroLlegada, guia);
                                        response = CommonFunctions.GET(ServiceLayer.ListadoRecepciones, null, args, sessionId, out _);
                                        string xml = response.json2xml(oDT.UniqueID);
#if DEBUG
                                        sbo_application.StatusBar.SetText(response, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                        sbo_application.StatusBar.SetText(xml, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                                        //oDT.LoadFromXML(xml);

                                        sbo_application.StatusBar.SetText("Peso actualizado correctamente", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                                    }
                                    //sbo_application.StatusBar.SetText("No existen lotes con peso asignado, por favor verificar", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                                    //bBubbleEvent = false;
                                }
                            }
                        }
                    }

                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                }
            }
        }
        
        private static void getframe(string sentido,string formUID, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {

            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent  = true;           

            string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
            if (!(Directory.Exists(@"C:\FOTOS")))
            {
                Directory.CreateDirectory(@"C:\FOTOS");
            }
            SAPbobsCOM.Recordset oRS1 = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
            string sSql1 = "select T0.\"U_DFO_Valor\" ,T0.\"U_DFO_Descrip\" from \"@DFO_OPDFO\" T0 where \"U_DFO_Tipo\" = 'FOTOS' ";
            oRS1.DoQuery(sSql1);
            if (oRS1.RecordCount != 0)
            {
                string rutaFoto = oRS1.Fields.Item("U_DFO_Valor").Value.ToString();

                SAPbobsCOM.Recordset oRS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);
                string sSql = "select T0.\"U_DFO_Valor\" ,T0.\"U_DFO_Descrip\" from \"@DFO_OPDFO\" T0 where \"U_DFO_Tipo\" = 'CAMARA' ";
                oRS.DoQuery(sSql);
                if (oRS.RecordCount != 0)
                {
                    while (!oRS.EoF)
                    {
                        //string sourceURL = "http://192.168.3.201:80/cgi-bin/snapshot.cgi"; //pasera frontal
                        //string sourceURL = "http://192.168.3.127:80/cgi-bin/snapshot.cgi"; //pasera trasera
                        //string sourceURL = "http://192.168.1.67:80/cgi-bin/snapshot.cgi"; //procesadora trasera
                        //string sourceURL = "http://192.168.1.179:80/cgi-bin/snapshot.cgi"; //procesadora frontal

                        string sourceURL = oRS.Fields.Item("U_DFO_Valor").Value.ToString();

                        byte[] buffer = new byte[1920 * 1080];
                        int read, total = 0;
                        string username = "admin";
                        string Pass = "Nimda2019.";


                        HttpWebRequest req = (HttpWebRequest)WebRequest.Create(sourceURL);
                        req.Credentials = new System.Net.NetworkCredential(username, Pass);
                        WebResponse resp = null;

                        try
                        {
                            resp = req.GetResponse();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception(ex.Message);
                        }


                        Stream stream = resp.GetResponseStream();
                        while ((read = stream.Read(buffer, total, 10000)) != 0)
                        {
                            total += read;
                        }

                        Image img = System.Drawing.Image.FromStream(new MemoryStream(buffer, 0, total));
                        string dir = @"" + rutaFoto + NroLlegada + oRS.Fields.Item("U_DFO_Descrip").Value.ToString() + DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg";

                        var encoder = ImageCodecInfo.GetImageEncoders()
                                         .First(c => c.FormatID == ImageFormat.Jpeg.Guid);
                        var encParams = new EncoderParameters(1);
                        encParams.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 20L);

                        // Create an Encoder object based on the GUID  
                        // for the Quality parameter category.  
                        System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;


                        try
                        {
                            img.Save(dir, encoder, encParams);

                            NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                            string args = "";
                            if (NroLlegada.Length > 0)
                            {
                                args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                            }
                            //string sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                            response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                            recepcion = response.DeserializeJsonObject<Recepcion>();

                            if (sentido == "E")
                            {
                                if (oRS.Fields.Item("U_DFO_Descrip").Value.ToString() == "FRONTAL")
                                {
                                    recepcion.U_CamFrontal = dir;
                                }
                                else
                                {
                                    recepcion.U_CamTrasera = dir;
                                }
                            }
                            else if (sentido == "S")
                            {
                                if (oRS.Fields.Item("U_DFO_Descrip").Value.ToString() == "FRONTAL")
                                {
                                    recepcion.U_CamFrontalSal = dir;
                                }
                                else
                                {
                                    recepcion.U_CamTraseraSal = dir;
                                }
                            }




                            response = CommonFunctions.PATCH(ServiceLayer.Recepcion, recepcion, recepcion.DocEntry, sessionId, out _);
                        }
                        catch
                        {

                        }
                        oRS.MoveNext();
                    }
                    NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                    string args1 = "";
                    if (NroLlegada.Length > 0)
                    {
                        args1 = string.Format("?$filter=DocEntry eq {0}", NroLlegada);// docentry corresponde a numerico, el argumento no va entre comillas
                    }
                    try
                    {
                        response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args1, sessionId, out _);
                        recepcion = response.DeserializeJsonObject<Recepcion>();
                        if (sentido == "E")
                        {
                            SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                            Frontal.Item.Visible = true;
                            Frontal.Item.Left = 560;
                            Frontal.Picture = recepcion.U_CamFrontal;
                            SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                            Trasera.Item.Visible = true;
                            Trasera.Item.Left = 560;
                            Trasera.Picture = recepcion.U_CamTrasera;
                        }
                        if (sentido == "S")
                        {
                            SAPbouiCOM.PictureBox Frontal = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicFrontal).Specific;
                            Frontal.Item.Visible = true;
                            Frontal.Item.Left = 560;
                            Frontal.Picture = recepcion.U_CamFrontalSal;
                            SAPbouiCOM.PictureBox Trasera = (SAPbouiCOM.PictureBox)oForm.Items.Item(pluginForm.PicTrasera).Specific;
                            Trasera.Item.Visible = true;
                            Trasera.Item.Left = 560;
                            Trasera.Picture = recepcion.U_CamTraseraSal;
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }



        private static void GridLote(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            SAPbouiCOM.Form oForm = sbo_application.Forms.Item(formUID);
            bBubbleEvent = true;
            string ItemCode = null;
            string CantEnvLote = null;
            string PesoEnvase = null;
            string Lote = null;
            string args = null;
            if (oItemEvent.EventType == SAPbouiCOM.BoEventTypes.et_ITEM_PRESSED)
            {
                if (!oItemEvent.BeforeAction)
                {
                    SAPbouiCOM.Grid grid = (SAPbouiCOM.Grid)oForm.Items.Item(pluginForm.GridLote.Uid).Specific;
                    if (oItemEvent.Row >= 0)
                    {
                        int gridrow = oItemEvent.Row;
                        grid.Rows.SelectedRows.Clear();
                        grid.Rows.SelectedRows.Add(gridrow);

                        Lote = grid.DataTable.Columns.Item(2).Cells.Item(gridrow).Value.ToString();
                        //ItemCode = grid.DataTable.Columns.Item(6).Cells.Item(gridrow).Value.ToString();
                        CantEnvLote = grid.DataTable.Columns.Item(3).Cells.Item(gridrow).Value.ToString();
                        // PesoEnvase = grid.DataTable.Columns.Item(7).Cells.Item(gridrow).Value.ToString();

                        //SAPbouiCOM.StaticText StaticCodEnv = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticCodEnv).Specific;
                        //StaticCodEnv.Caption = ItemCode;
                        //SAPbouiCOM.StaticText StaticItemName = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticItemName).Specific;
                        //StaticItemName.Caption = "";
                        
                        //StaticWeight.Caption = PesoEnvase;
                        SAPbouiCOM.StaticText StaticCantEnv = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticCantEnv).Specific;
                        StaticCantEnv.Caption = CantEnvLote;

                        string NroLlegada = ((SAPbouiCOM.EditText)oForm.Items.Item(pluginForm.TxtNroLlegada.Uid).Specific).Value;
                        args = null;

                        if (NroLlegada.Length > 0)
                        {
                            args = string.Format("?$filter=DocEntry eq {0}", NroLlegada);
                        }

                        response = CommonFunctions.GET(ServiceLayer.Recepcion, null, args, sessionId, out _);
                        recepcion = response.DeserializeJsonObject<Recepcion>();

                        int cantEnvases = 0;
                        double PesoEnvases = 0;
                        foreach (var envase in recepcion.DFO_TRUCK5Collection)
                        {
                            if (Lote == envase.U_Lote)
                            {
                                args = string.Format("?$filter=ItemCode eq '{0}'", envase.U_CodEnvase);// docentry corresponde a numerico, el argumento no va entre comillas
                                response = CommonFunctions.GET(ServiceLayer.Items, null, args, sessionId, out _);
                                Items item = response.DeserializeJsonObject<Items>();
                                if (item != null)
                                {
                                    PesoEnvases = PesoEnvases + ((int)envase.U_Envases * (double)item.SalesUnitWeight);
                                }
                            }
                        }
                        SAPbouiCOM.StaticText StaticWeight = (SAPbouiCOM.StaticText)oForm.Items.Item(pluginForm.StaticWeight).Specific;
                        StaticWeight.Caption = PesoEnvases.ToString();
                        StaticWeight.Item.Visible = true;
                        SAPbouiCOM.StaticText labelWeight = (SAPbouiCOM.StaticText)oForm.Items.Item("Item_4").Specific;
                        labelWeight.Item.Visible = true;

                    }
                    oForm.Mode = SAPbouiCOM.BoFormMode.fm_OK_MODE;
                }
            }
        }
    }
}

//double _tara = (double)recepcion.U_KilosIngreso - (double)recepcion.U_KilosSalida;

//foreach (var item in recepcion.DFO_TRUCK1Collection.GroupBy(i=> i.U_FolioGuia))
//                                  {
//                                    if (item.Key != null)
//                                  {
//                                    CBguia.ValidValues.Add(item.Key, item.Key);
//                              }
//                        }