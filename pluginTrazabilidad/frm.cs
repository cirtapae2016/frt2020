using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Linq;
using System.Xml;

namespace pluginTrazabilidad
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

                    oForm.Mode = BoFormMode.fm_OK_MODE;

                    oForm.Items.Item(pluginForm.CBbuscar).AffectsFormMode = false;
                    oForm.Items.Item(pluginForm.TxtBuscar.Uid).AffectsFormMode = false;

                    ((ComboBox)oForm.Items.Item(pluginForm.CBbuscar).Specific).Select(0, BoSearchKey.psk_Index);

                    ((Folder)oForm.Items.Item(pluginForm.FolderPT).Specific).Item.Click();

                    oForm.Visible = true;
                }
                catch
                {
                    throw;
                }
            }
        }

        internal static void ItemEventHandler(string formUID, ref ItemEvent itemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bubbleEvent, string sessionId)
        {
            bubbleEvent = true;

            switch (itemEvent.ItemUID)
            {
                default:
                    break;
            }
        }
    }
}
