using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;

namespace pluginComex
{
    public class mainComex : ISAPBusinessOne

    {
        public string CaptionMenuItem()
        {
            return MenuPlugin.Caption;
        }

        public void CreateUserFieldMD(SAPbobsCOM.Company sbo_company, ref Application sbo_application)
        {
        }

        public void CreateUserObjectMD(SAPbobsCOM.Company sbo_company, ref Application sbo_application)
        {
        }

        public void CreateUserTablesMD(SAPbobsCOM.Company sbo_company, ref Application sbo_application)
        {
        }

        public string FatherMenu()
        {
            return UserMenu.MenuComex;
        }

        public string MenuUID()
        {
            return MenuPlugin.MenuUID;
        }

        public void SBO_Application_FormDataEvent(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (businessObjectInfo.FormTypeEx)
            {
                case pluginForm.FormType:
                case CommonForms.FormComexDir.FormType:
                    frmComex.FormDataEventHandler(ref businessObjectInfo, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        public void SBO_Application_ItemEvent(string formUID, ref ItemEvent itemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (itemEvent.FormTypeEx)
            {
                case pluginForm.FormType:
                case CommonForms.FormComexDir.FormType:
                    frmComex.ItemEventHandler(formUID, ref itemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        public void SBO_Application_LayoutKeyEvent(ref LayoutKeyInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_MenuEvent(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            switch (oMenuEvent.MenuUID)
            {
                case MenuPlugin.MenuUID:
                    frmComex.FormLoad(ref oMenuEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case SAPMenu.New:
                case SAPMenu.Find:
                    frmComex.MenuEventHandler(ref oMenuEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        public void SBO_Application_PrintEvent(ref PrintEventInfo printeventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_ProgressBarEvent(ref ProgressBarEvent pVal, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_ReportDataEvent(ref ReportDataInfo reportDataInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_RightClickEvent(ref ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_ServerInvokeCompletedEvent(ref B1iEvent B1iEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_StatusBarEvent(string Text, SAPbobsCOM.Company sbo_company, ref Application sbo_application, BoStatusBarMessageType messageType)
        {
        }

        public void SBO_Application_UDOEvent(UDOEvent udoEventArgs, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }

        public void SBO_Application_WidgetEvent(WidgetData pWidgetData, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
        }
    }
}