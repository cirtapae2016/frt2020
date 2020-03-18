using CoreSAPB1;
using CoreUtilities;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Windows.Forms;

namespace DFOne_Test
{
    internal static class Program
    {
        #region VariablesGlobales

        public static System.Globalization.NumberFormatInfo oNumberFormatInfo = new System.Globalization.NumberFormatInfo();
        public static string RightClickColumn = null;
        public static int RightClickRow = -1;
        public static string RightClickItemUID = null;
        public static bool ModalFormIsOpen = false;
        public static SAPbobsCOM.Company sbo_company;
        public static SAPbouiCOM.Application sbo_application;
        public static string sessionId;

        public const string AddonNamespace = "DFO";
        public const string AddOnName = "DFOne_Test";
        public const string UserTablesFile = "UserTables.xml";
        public const string UserFieldsFile = "UserFields.xml";
        public const string UserObjectsFile = "UserObjects.xml";
        public static string LogFile;
        private static string UsrPass;

        #endregion VariablesGlobales

        [STAThread]
        private static void Main(string[] args)
        {

            string AddOn = AppDomain.CurrentDomain.FriendlyName.Replace(".exe", "");
            string User = Environment.UserName + "." + Environment.UserDomainName;
            string PId = System.Diagnostics.Process.GetCurrentProcess().Id.ToString();

            string Path = $"C:\\ProgramData\\SAP\\SAP Business One\\Log\\{AddOn}\\{User}";
            LogFile = $"{Path}\\Addon.{DateTime.Now.ToString("yyyyMMdd_HH.mm.ss")}.pid{PId}.log.csv";

            try
            {
                if (!Directory.Exists(Path))
                    Directory.CreateDirectory(Path);

                System.Threading.Tasks.Task.Run(() => CommonFunctions.DeleteOldLogFiles(Path));

                SAPbouiCOM.Framework.Application oApp = null;
                if (args.Length < 1)
                {
                    oApp = new SAPbouiCOM.Framework.Application();
                }
                else
                {
                    oApp = new SAPbouiCOM.Framework.Application(args[0]);
                }

                sbo_application = SAPbouiCOM.Framework.Application.SBO_Application;
                sbo_company = (SAPbobsCOM.Company)sbo_application.Company.GetDICompany();

                oNumberFormatInfo.NumberDecimalSeparator = sbo_company.GetCompanyService().GetAdminInfo().DecimalSeparator;
                oNumberFormatInfo.NumberGroupSeparator = sbo_company.GetCompanyService().GetAdminInfo().ThousandsSeparator;

                RemoveMenus();
                AddMenuItems();

                sbo_application.MenuEvent += new SAPbouiCOM._IApplicationEvents_MenuEventEventHandler(SBO_Application_MenuEvent);

                sbo_application.AppEvent += new SAPbouiCOM._IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);

                sbo_application.ItemEvent += new SAPbouiCOM._IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);

                sbo_application.FormDataEvent += new SAPbouiCOM._IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEvent);

                sbo_application.ReportDataEvent += new SAPbouiCOM._IApplicationEvents_ReportDataEventEventHandler(SBO_Application_ReportDataEvent);

                sbo_application.PrintEvent += new SAPbouiCOM._IApplicationEvents_PrintEventEventHandler(SBO_Application_PrintEvent);

                sbo_application.RightClickEvent += new SAPbouiCOM._IApplicationEvents_RightClickEventEventHandler(SBO_Application_RightClickEvent);

                sbo_application.ProgressBarEvent += new SAPbouiCOM._IApplicationEvents_ProgressBarEventEventHandler(SBO_Application_ProgressBarEvent);

                sbo_application.StatusBar.SetText(string.Format("Revisando la estructura de la base de datos"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
                //System.Threading.Tasks.Task.Run(() => LoadUserTablesMDToXmlFile(AddonNamespace));
                //System.Threading.Tasks.Task.Run(() => LoadUserFieldMDToXmlFile(AddonNamespace));
                //System.Threading.Tasks.Task.Run(() => LoadUserObjectsMDToXmlFile(AddonNamespace));

                //try { LoadUserTablesMDFromXmlFile(UserTablesFile); } catch (Exception e) { sbo_application.StatusBar.SetText(e.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning); }
                //try { LoadUserFieldMDFromXmlFile(UserFieldsFile); } catch (Exception e) { sbo_application.StatusBar.SetText(e.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning); }
                //try { LoadUserObjectMDFromXmlFile(UserObjectsFile); } catch (Exception e) { sbo_application.StatusBar.SetText(e.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Warning); }
                sbo_application.StatusBar.SetText(string.Format("Revision completada"), SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);

                sbo_application.StatusBar.SetText($"Addon {AddOnName} conectado", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
#if DEBUG
                sbo_application.StatusBar.SetText(string.Format("{0} = {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "Conectando al SL"), SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                try
                {
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                }
                catch
                {
                    ShowInputDialog(ref UsrPass);
                    if (string.IsNullOrEmpty(UsrPass))
                    {
                        sbo_application.MessageBox("Debe ingresar su contraseña SAP, salga y vuelva a entrar");
                        Environment.Exit(0);
                    }

                    var log = new Login { CompanyDB = sbo_company.CompanyDB, UserName = sbo_company.UserName, Password = UsrPass };
                    sessionId = CommonFunctions.POST(ServiceLayer.Login, log, null, out System.Net.HttpStatusCode statusCode);
                    if (statusCode != System.Net.HttpStatusCode.OK)
                    {
                        sbo_application.MessageBox("Clave incorrecta, salga y vuelva a entrar");
                        Environment.Exit(0);
                    }
                }
#if DEBUG
                sbo_application.StatusBar.SetText(string.Format("{0} = {1}", DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"), "SL conectado"), SAPbouiCOM.BoMessageTime.bmt_Medium, SAPbouiCOM.BoStatusBarMessageType.smt_Warning);
#endif
                oApp.Run();
            }
            catch (Exception ex)
            {
                CommonFunctions.LogFile(LogFile, ex.Message);
                MessageBox.Show(ex.Message);
            }
        }

        private static DialogResult ShowInputDialog(ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(210, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = FormBorderStyle.FixedToolWindow;
            inputBox.ClientSize = size;
            inputBox.Text = "Ingrese su clave de acceso a SAP";
            inputBox.StartPosition = FormStartPosition.CenterParent;

            TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            textBox.PasswordChar = '*';
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(okButton);

            inputBox.AcceptButton = okButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        private static void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent oMenuEvent, out bool bBubbleEvent)
        {
            bBubbleEvent = true;

            try
            {
                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type type in Assembly.LoadFrom(assembly.FullName).GetTypes().Where(i => i.GetInterface("ISAPBusinessOne") != null))
                    {
                        try
                        {
                            ISAPBusinessOne pluginclass = (ISAPBusinessOne)Activator.CreateInstance(type);
                            //ReconnectSL();
                            pluginclass.SBO_Application_MenuEvent(ref oMenuEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            if (!bBubbleEvent)
                                return;
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.LogFile(LogFile, ex.ToString());
                            sbo_application.MessageBox(ex.Message);
                            bBubbleEvent = false;
                            //sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }
        }

        private static void SBO_Application_ItemEvent(string formUID, ref SAPbouiCOM.ItemEvent oItemEvent, out bool bBubbleEvent)
        {
            bBubbleEvent = true;

            try
            {
                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type type in Assembly.LoadFrom(assembly.FullName).GetTypes().Where(i => i.GetInterface("ISAPBusinessOne") != null))
                    {
                        try
                        {
                            ISAPBusinessOne pluginclass = (ISAPBusinessOne)Activator.CreateInstance(type);
                            //ReconnectSL();
                            pluginclass.SBO_Application_ItemEvent(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            if (!bBubbleEvent)
                                return;
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.LogFile(LogFile, ex.ToString());
                            sbo_application.MessageBox(ex.Message);
                            bBubbleEvent = false;
                            //sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }
        }

        private static void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo oBusinessObjectInfo, out bool bBubbleEvent)
        {
            bBubbleEvent = true;

            try
            {
                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type type in Assembly.LoadFrom(assembly.FullName).GetTypes().Where(i => i.GetInterface("ISAPBusinessOne") != null))
                    {
                        try
                        {
                            ISAPBusinessOne pluginclass = (ISAPBusinessOne)Activator.CreateInstance(type);
                            //ReconnectSL();
                            pluginclass.SBO_Application_FormDataEvent(ref oBusinessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                            if (!bBubbleEvent)
                                return;
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.LogFile(LogFile, ex.ToString());
                            sbo_application.MessageBox(ex.Message);
                            bBubbleEvent = false;
                            //sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }
        }

        private static void SBO_Application_ReportDataEvent(ref SAPbouiCOM.ReportDataInfo oReportDataInfo, out bool bBubbleEvent)
        {
            bBubbleEvent = true;

            try
            {
                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type type in Assembly.LoadFrom(assembly.FullName).GetTypes().Where(i => i.GetInterface("ISAPBusinessOne") != null))
                    {
                        try
                        {
                            ISAPBusinessOne pluginclass = (ISAPBusinessOne)Activator.CreateInstance(type);
                            //ReconnectSL();
                            pluginclass.SBO_Application_ReportDataEvent(ref oReportDataInfo, sbo_company, ref sbo_application, out bBubbleEvent);
                            if (!bBubbleEvent)
                                return;
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.LogFile(LogFile, ex.ToString());
                            sbo_application.MessageBox(ex.Message);
                            bBubbleEvent = false;
                            //sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }
        }

        private static void SBO_Application_PrintEvent(ref SAPbouiCOM.PrintEventInfo oPrinteventInfo, out bool bBubbleEvent)
        {
            bBubbleEvent = true;

            try
            {
                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type type in Assembly.LoadFrom(assembly.FullName).GetTypes().Where(i => i.GetInterface("ISAPBusinessOne") != null))
                    {
                        try
                        {
                            ISAPBusinessOne pluginclass = (ISAPBusinessOne)Activator.CreateInstance(type);
                            //ReconnectSL();
                            pluginclass.SBO_Application_PrintEvent(ref oPrinteventInfo, sbo_company, ref sbo_application, out bBubbleEvent);
                            if (!bBubbleEvent)
                                return;
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.LogFile(LogFile, ex.ToString());
                            sbo_application.MessageBox(ex.Message);
                            bBubbleEvent = false;
                            //sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }
        }

        private static void SBO_Application_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo oContextMenuInfo, out bool bBubbleEvent)
        {
            bBubbleEvent = true;

            try
            {
                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type type in Assembly.LoadFrom(assembly.FullName).GetTypes().Where(i => i.GetInterface("ISAPBusinessOne") != null))
                    {
                        try
                        {
                            ISAPBusinessOne pluginclass = (ISAPBusinessOne)Activator.CreateInstance(type);
                            //ReconnectSL();
                            pluginclass.SBO_Application_RightClickEvent(ref oContextMenuInfo, sbo_company, ref sbo_application, out bBubbleEvent);
                            if (!bBubbleEvent)
                                return;
                        }
                        catch (Exception ex)
                        {
                            CommonFunctions.LogFile(LogFile, ex.ToString());
                            sbo_application.MessageBox(ex.Message);
                            bBubbleEvent = false;
                            //sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.MessageBox(ex.Message);
            }
        }

        private static void SBO_Application_ProgressBarEvent(ref SAPbouiCOM.ProgressBarEvent pVal, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        private static void SBO_Application_AppEvent(SAPbouiCOM.BoAppEventTypes oAppEvent)
        {
            switch (oAppEvent)
            {
                case SAPbouiCOM.BoAppEventTypes.aet_ShutDown:
                case SAPbouiCOM.BoAppEventTypes.aet_CompanyChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_ServerTerminition:
                    CommonFunctions.POST(ServiceLayer.Logout, null, sessionId, out _);
                    Environment.Exit(0);
                    break;

                case SAPbouiCOM.BoAppEventTypes.aet_LanguageChanged:
                case SAPbouiCOM.BoAppEventTypes.aet_FontChanged:
                    RemoveMenus();
                    AddMenuItems();
                    break;
            }
        }

        private static void AddMenuItems()
        {
            try
            {
                sbo_application.Forms.GetFormByTypeAndCount(169, 1).Freeze(true);

                SAPbouiCOM.MenuItem MenuItemModules = sbo_application.Menus.Item(SAPMenu.MenuModules);

                int position = MenuItemModules.SubMenus.Count;

                SAPbouiCOM.MenuItem menuItem = MenuItemModules.SubMenus.Add(UserMenu.MenuPrincipal, $"{AddOnName} {Assembly.GetEntryAssembly().GetName().Version.ToString().Replace(".0.0", "")}", SAPbouiCOM.BoMenuType.mt_POPUP, position);

                menuItem.SubMenus.Add(UserMenu.Configuracion, "Configuracion", SAPbouiCOM.BoMenuType.mt_STRING, 0);
                menuItem.SubMenus.Add(UserMenu.MenuRecepcion, "Recepcion", SAPbouiCOM.BoMenuType.mt_POPUP, ++position);
                menuItem.SubMenus.Add(UserMenu.MenuCalidad, "Calidad", SAPbouiCOM.BoMenuType.mt_POPUP, ++position);
                menuItem.SubMenus.Add(UserMenu.MenuProduccion, "Produccion", SAPbouiCOM.BoMenuType.mt_POPUP, ++position);
                menuItem.SubMenus.Add(UserMenu.MenuComex, "Comex", SAPbouiCOM.BoMenuType.mt_POPUP, ++position);

                foreach (var assembly in CommonFunctions.GetAssemblies(AppDomain.CurrentDomain.BaseDirectory))
                {
                    foreach (Type t in Assembly.LoadFrom(assembly.FullName).GetTypes())
                    {
                        if (t.GetInterface("ISAPBusinessOne") != null)
                        {
                            try
                            {
                                ISAPBusinessOne pluginclass = Activator.CreateInstance(t) as ISAPBusinessOne;

                                string fatherMenu = pluginclass.FatherMenu();
                                string menuUID = pluginclass.MenuUID();
                                string captionMenuItem = pluginclass.CaptionMenuItem();

                                if (!string.IsNullOrEmpty(menuUID))
                                {
                                    if (MenuItemModules.SubMenus.Exists(fatherMenu))
                                    {
                                        SAPbouiCOM.MenuItem fatherMenuItem = MenuItemModules.SubMenus.Item(fatherMenu);
                                        position = fatherMenuItem.SubMenus.Count;
                                        fatherMenuItem.SubMenus.Add(menuUID, captionMenuItem, SAPbouiCOM.BoMenuType.mt_STRING, position);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbo_application.StatusBar.SetText(ex.Message, SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
            }
            finally
            {
                sbo_application.Forms.GetFormByTypeAndCount(169, 1).Freeze(false);
                sbo_application.Forms.GetFormByTypeAndCount(169, 1).Update();
            }
        }

        private static void RemoveMenus()
        {
            SAPbouiCOM.Menus Menus = sbo_application.Menus;

            if (Menus.Exists(UserMenu.MenuPrincipal))
                Menus.RemoveEx(UserMenu.MenuPrincipal);
            if (Menus.Exists(UserMenu.Configuracion))
                Menus.RemoveEx(UserMenu.Configuracion);
            if (Menus.Exists(UserMenu.MenuCalidad))
                Menus.RemoveEx(UserMenu.MenuCalidad);
            if (Menus.Exists(UserMenu.MenuComex))
                Menus.RemoveEx(UserMenu.MenuComex);
            if (Menus.Exists(UserMenu.MenuProduccion))
                Menus.RemoveEx(UserMenu.MenuProduccion);
            if (Menus.Exists(UserMenu.MenuRecepcion))
                Menus.RemoveEx(UserMenu.MenuRecepcion);
            if (Menus.Exists(UserMenu.DeleteRow))
                Menus.RemoveEx(UserMenu.DeleteRow);
        }

        private static void ReconnectSL()
        {
            CommonFunctions.GET(ServiceLayer.Items, null, $"?$select=ItemCode&$top=1", sessionId, out System.Net.HttpStatusCode httpStatus);
            if (httpStatus == System.Net.HttpStatusCode.Unauthorized)
            {
                try
                {
                    sessionId = sbo_application.Company.GetServiceLayerConnectionContext(ServiceLayer.Address);
                }
                catch
                {
                    var log = new Login { CompanyDB = sbo_company.CompanyDB, UserName = "Intercompany", Password = "mngr" };
                    sessionId = CommonFunctions.POST(ServiceLayer.Login, log, null, out _);
                }
            }
        }
        private static void LoadUserFieldMDToXmlFile(string prefix)
        {
            SAPbobsCOM.UserFieldsMD userFieldsMD = null;
            XmlDocument documentoFinal = null;

            SAPbobsCOM.Recordset RS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            if (sbo_company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                RS.DoQuery(string.Format(@"select ""TableID"", ""FieldID"" from CUFD where (CUFD.""TableID"" like '@{0}%' or CUFD.""AliasID"" like '{0}%') and CUFD.""TableID"" not like 'A%'", prefix));

            if (RS.RecordCount > 0)
            {
                while (!RS.EoF)
                {
                    int fieldID = int.Parse(RS.Fields.Item("FieldID").Value.ToString());
                    string tableID = RS.Fields.Item("TableID").Value.ToString();

                    userFieldsMD = (SAPbobsCOM.UserFieldsMD)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserFields);

                    if (userFieldsMD.GetByKey(tableID, fieldID))
                    {
                        XmlDocument documento = new XmlDocument();
                        sbo_company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                        documento.LoadXml(userFieldsMD.GetAsXML());

                        if (documentoFinal == null)
                        {
                            documentoFinal = new XmlDocument();
                            documentoFinal.LoadXml(userFieldsMD.GetAsXML());
                        }
                        else
                        {
                            XmlNode nodeBO = documento.DocumentElement.FirstChild;
                            string stringContenidoNodeBO = nodeBO.InnerXml;
                            try
                            {
                                XmlNode nuevoNodeBO = documentoFinal.CreateElement("BO");
                                nuevoNodeBO.InnerXml = stringContenidoNodeBO;

                                documentoFinal.DocumentElement.AppendChild(nuevoNodeBO);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                    RS.MoveNext();
                }

                if (documentoFinal != null)
                    documentoFinal.Save(UserFieldsFile);

                sbo_application.StatusBar.SetText("UDF export completed", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        private static void LoadUserTablesMDToXmlFile(string prefix)
        {
            SAPbobsCOM.UserTablesMD userTablesMD = null;
            XmlDocument documentoFinal = null;

            SAPbobsCOM.Recordset RS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            if (sbo_company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                RS.DoQuery(string.Format(@"SELECT ""TableName"" FROM OUTB WHERE ""TableName"" LIKE '{0}%'", prefix));

            if (RS.RecordCount > 0)
            {
                while (!RS.EoF)
                {
                    string table = RS.Fields.Item("TableName").Value.ToString();

                    userTablesMD = (SAPbobsCOM.UserTablesMD)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserTables);

                    if (userTablesMD.GetByKey(table))
                    {
                        XmlDocument documento = new XmlDocument();
                        sbo_company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                        documento.LoadXml(userTablesMD.GetAsXML());

                        if (documentoFinal == null)
                        {
                            documentoFinal = new XmlDocument();
                            documentoFinal.LoadXml(userTablesMD.GetAsXML());
                        }
                        else
                        {
                            XmlNode nodeBO = documento.DocumentElement.FirstChild;
                            string stringContenidoNodeBO = nodeBO.InnerXml;
                            try
                            {
                                XmlNode nuevoNodeBO = documentoFinal.CreateElement("BO");
                                nuevoNodeBO.InnerXml = stringContenidoNodeBO;

                                documentoFinal.DocumentElement.AppendChild(nuevoNodeBO);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(userTablesMD);
                    userTablesMD = null;
                    GC.Collect();

                    RS.MoveNext();
                }
            }

            if (documentoFinal != null)
                documentoFinal.Save(UserTablesFile);

            sbo_application.StatusBar.SetText("UDT export completed", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
        }

        private static void LoadUserObjectsMDToXmlFile(string prefix)
        {
            SAPbobsCOM.UserObjectsMD userObjectsMD = null;
            XmlDocument documentoFinal = null;

            SAPbobsCOM.Recordset RS = (SAPbobsCOM.Recordset)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.BoRecordset);

            if (sbo_company.DbServerType == SAPbobsCOM.BoDataServerTypes.dst_HANADB)
                RS.DoQuery(string.Format(@"select ""Code"" from OUDO where ""TableName"" like '{0}%'", prefix));

            if (RS.RecordCount > 0)
            {
                while (!RS.EoF)
                {
                    string code = RS.Fields.Item("Code").Value.ToString();

                    userObjectsMD = (SAPbobsCOM.UserObjectsMD)sbo_company.GetBusinessObject(SAPbobsCOM.BoObjectTypes.oUserObjectsMD);

                    if (userObjectsMD.GetByKey(code))
                    {
                        XmlDocument documento = new XmlDocument();
                        sbo_company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;
                        documento.LoadXml(userObjectsMD.GetAsXML());

                        if (documentoFinal == null)
                        {
                            documentoFinal = new XmlDocument();
                            documentoFinal.LoadXml(userObjectsMD.GetAsXML());
                        }
                        else
                        {
                            XmlNode nodeBO = documento.DocumentElement.FirstChild;
                            string stringContenidoNodeBO = nodeBO.InnerXml;
                            try
                            {
                                XmlNode nuevoNodeBO = documentoFinal.CreateElement("BO");
                                nuevoNodeBO.InnerXml = stringContenidoNodeBO;

                                documentoFinal.DocumentElement.AppendChild(nuevoNodeBO);
                            }
                            catch (ArgumentException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (InvalidOperationException ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }
                        }
                    }

                    RS.MoveNext();
                }

                if (documentoFinal != null)
                    documentoFinal.Save(UserObjectsFile);

                sbo_application.StatusBar.SetText("UDO export completed", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
            }
        }

        private static void LoadUserFieldMDFromXmlFile(string xmlFileName)
        {
            SAPbobsCOM.UserFieldsMD userFieldsMD = null;
            GC.Collect();

            xmlFileName = AppDomain.CurrentDomain.BaseDirectory + xmlFileName;

            int lErrCode = 0;
            int recordCount = sbo_company.GetXMLelementCount(xmlFileName);

            for (int iCounter = 0; iCounter <= recordCount - 1; iCounter++)
            {
                sbo_company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;

                userFieldsMD = ((SAPbobsCOM.UserFieldsMD)(sbo_company.GetBusinessObjectFromXML(xmlFileName, Convert.ToInt32(iCounter))));
                lErrCode = userFieldsMD.Add();

                if (lErrCode != 0 && lErrCode != -1120 && lErrCode != -5002)
                {
                    if (lErrCode == -2035)
                    {
                        lErrCode = userFieldsMD.Update();
                        if (lErrCode == 0)
                        {
                            sbo_application.StatusBar.SetText($"UDF {userFieldsMD.Name} updated", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                        }
                    }
                    else
                    {
                        sbo_application.StatusBar.SetText($"UDF {userFieldsMD.Name} not created | {sbo_company.GetLastErrorDescription()}", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                    }
                }
                if (lErrCode == 0)
                {
                    sbo_application.StatusBar.SetText($"UDF {userFieldsMD.Name} created", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }

                GC.Collect();
            }
        }

        private static void LoadUserTablesMDFromXmlFile(string xmlFileName)
        {
            GC.Collect();

            SAPbobsCOM.UserTablesMD userTablesMD = null;

            xmlFileName = AppDomain.CurrentDomain.BaseDirectory + xmlFileName;

            int lErrCode = 0;
            int recordCount = sbo_company.GetXMLelementCount(xmlFileName);

            for (int iCounter = 0; iCounter <= recordCount - 1; iCounter++)
            {
                sbo_company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;

                userTablesMD = ((SAPbobsCOM.UserTablesMD)(sbo_company.GetBusinessObjectFromXML(xmlFileName, Convert.ToInt32(iCounter))));
                lErrCode = userTablesMD.Add();

                if (lErrCode != 0 && lErrCode != -2035 && lErrCode != -1120)
                {
                    sbo_application.StatusBar.SetText($"UDT {userTablesMD.TableName} not created | {sbo_company.GetLastErrorDescription()}", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                if (lErrCode == 0)
                {
                    sbo_application.StatusBar.SetText($"UDT {userTablesMD.TableName} created", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }

                GC.Collect();
            }
        }

        private static void LoadUserObjectMDFromXmlFile(string xmlFileName)
        {
            SAPbobsCOM.UserObjectsMD userObjectsMD = null;
            GC.Collect();

            xmlFileName = AppDomain.CurrentDomain.BaseDirectory + xmlFileName;

            int lErrCode = 0;
            int recordCount = sbo_company.GetXMLelementCount(xmlFileName);

            for (int iCounter = 0; iCounter <= recordCount - 1; iCounter++)
            {
                sbo_company.XmlExportType = SAPbobsCOM.BoXmlExportTypes.xet_ExportImportMode;

                userObjectsMD = ((SAPbobsCOM.UserObjectsMD)(sbo_company.GetBusinessObjectFromXML(xmlFileName, Convert.ToInt32(iCounter))));
                lErrCode = userObjectsMD.Add();

                if (lErrCode != 0 && lErrCode != -2035 && lErrCode != -1120 && lErrCode != -5002)
                {
                    sbo_application.StatusBar.SetText($"UDO {userObjectsMD.TableName} not created | {sbo_company.GetLastErrorDescription()}", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Error);
                }
                if (lErrCode == 0)
                {
                    sbo_application.StatusBar.SetText($"UDO {userObjectsMD.Name} created", SAPbouiCOM.BoMessageTime.bmt_Short, SAPbouiCOM.BoStatusBarMessageType.smt_Success);
                }

                GC.Collect();
            }
        }
    }
}