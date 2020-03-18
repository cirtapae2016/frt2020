namespace CoreSAPB1
{
    public interface ISAPBusinessOne
    {
        string FatherMenu();

        string MenuUID();

        string CaptionMenuItem();

        void CreateUserTablesMD(SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application);

        void CreateUserFieldMD(SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application);

        void CreateUserObjectMD(SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application);

        void SBO_Application_FormDataEvent(ref SAPbouiCOM.BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId);

        void SBO_Application_ItemEvent(string formUID, ref SAPbouiCOM.ItemEvent itemEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId);

        void SBO_Application_LayoutKeyEvent(ref SAPbouiCOM.LayoutKeyInfo eventInfo, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_MenuEvent(ref SAPbouiCOM.MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent, string sessionId);

        void SBO_Application_PrintEvent(ref SAPbouiCOM.PrintEventInfo printeventInfo, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_ProgressBarEvent(ref SAPbouiCOM.ProgressBarEvent pVal, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_ReportDataEvent(ref SAPbouiCOM.ReportDataInfo reportDataInfo, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_RightClickEvent(ref SAPbouiCOM.ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_ServerInvokeCompletedEvent(ref SAPbouiCOM.B1iEvent B1iEvent, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_StatusBarEvent(string Text, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, SAPbouiCOM.BoStatusBarMessageType messageType);

        void SBO_Application_UDOEvent(SAPbouiCOM.UDOEvent udoEventArgs, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);

        void SBO_Application_WidgetEvent(SAPbouiCOM.WidgetData pWidgetData, SAPbobsCOM.Company sbo_company, ref SAPbouiCOM.Application sbo_application, out bool bBubbleEvent);
    }
}