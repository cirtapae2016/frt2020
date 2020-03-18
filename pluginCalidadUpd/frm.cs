using CoreSAPB1;
using CoreUtilities;
using SAPbouiCOM;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace pluginCalidadUpd
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
                    string date = DateTime.Now.ToString("yyyyMMdd");
                    string time = DateTime.Now.ToString("hh:mm");

                    XmlDocument xmlFormulario = new XmlDocument();
                    xmlFormulario.LoadXml(contenidoArchivo);

                    FormCreationPackage.XmlData = xmlFormulario.InnerXml;
                    FormCreationPackage.UniqueID = pluginForm.FormType + CommonFunctions.Random().ToString();
                    oForm = sbo_application.Forms.AddEx(FormCreationPackage);

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
                case pluginForm.ButtonFilter:
                    ButtonFilter(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;

                case pluginForm.ButtonReview:
                    ButtonReview(formUID, ref oItemEvent, sbo_company, ref sbo_application, out bBubbleEvent, sessionId);
                    break;
            }
        }

        internal static void FormDataEventHandler(ref BusinessObjectInfo businessObjectInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            switch (businessObjectInfo.EventType)
            {
                default:
                    break;
            }
        }

        internal static void RightClickEventHandler(ref ContextMenuInfo eventInfo, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent)
        {
            bBubbleEvent = true;
            if (!eventInfo.BeforeAction)
            {
                //functions.AddRightClickMenu(ref sbo_application, UserMenu.DeleteRow, "Borrar Fila", true, BoMenuType.mt_STRING, SAPMenu.RightClickMenu);
            }
        }

        internal static void MenuEventHandler(ref MenuEvent oMenuEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (!oMenuEvent.BeforeAction)
            {
                Form oForm;
                try { oForm = sbo_application.Forms.ActiveForm as Form; }
                catch { return; }

                if (oForm.TypeEx == pluginForm.FormType)
                {
                    switch (oMenuEvent.MenuUID)
                    {
                        case SAPMenu.New:
                            try
                            {
                                oForm.Freeze(true);
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;

                        case SAPMenu.Find:
                            try
                            {
                                oForm.Freeze(true);
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;

                        case UserMenu.DeleteRow:
                            try
                            {
                                oForm.Freeze(true);

                                Menus Menus = sbo_application.Menus;
                                if (Menus.Exists(UserMenu.DeleteRow))
                                    Menus.RemoveEx(UserMenu.DeleteRow);
                            }
                            catch (Exception e) { sbo_application.MessageBox(e.Message); }
                            finally { oForm.Freeze(false); }
                            break;
                    }
                }
            }
        }

        private static void ButtonFilter(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;
            if (oItemEvent.EventType == BoEventTypes.et_CLICK)
            {
                var oForm = sbo_application.Forms.Item(formUID);

                if (!oItemEvent.BeforeAction)
                {
                    try
                    {
                        oForm.Freeze(true);
                        var Where = "";

                        var Fecha = oForm.DataSources.UserDataSources.Item("UD_" + pluginForm.TxtFecha).ValueEx;
                        var Fruta = oForm.DataSources.UserDataSources.Item("UD_" + pluginForm.TxtFruta).ValueEx;
                        var Proceso = oForm.DataSources.UserDataSources.Item("UD_" + pluginForm.TxtProceso).ValueEx;
                        var Todos = oForm.DataSources.UserDataSources.Item("UD_" + pluginForm.ChkAll).ValueEx;

                        Where += "Where ifnull(T0.\"U_Revisado\",'N') = 'N'";

                        if (!string.IsNullOrEmpty(Fruta))
                        {
                            Where += $" and T0.\"U_PuntoControl\" like '{Fruta}%'";
                        }

                        var grid = oForm.Items.Item(pluginForm.GridCalidad).Specific as Grid;

                        var sql = @$"
select
	T0.""DocEntry"" ""Correlativo"",
    T0.""CreateDate"" ""Fecha"",
	T0.""CreateTime"" ""Hora"",
	T0.""Creator"" ""Analista"",
	T0.""U_PuntoControl"" ""Registro"",
	T0.""U_Version"" ""Version"",
	T0.""U_BaseType"" ""BaseType"",
	case T0.""U_BaseType"" when '4' then 'OT' when 'OTRUCK' then 'Recepcion' when '67' then 'Fumigado' when '59' then '' end ""Origen"",
	case T0.""U_BaseType"" when '4' then (select ""DocNum"" from OWOR where ""DocEntry""=T0.""U_BaseEntry"") when '67' then (select ""DocNum"" from OWTR where ""DocEntry""=T0.""U_BaseEntry"") else T0.""U_BaseEntry"" end ""Numero"",
    case ifnull(T0.""U_Revisado"",'N') when 'N' then 'No' when 'Y' then 'Si' end ""Aprobado"",
    T0.""U_RevisadoPor"" ""Aprobado Por"",
    string_agg(T1.""U_BatchNum"", ' | ') ""Lotes asignados""

from ""@DFO_ORQLTY"" T0
left join ""@DFO_RQLTY3"" T1 on T1.""DocEntry""=T0.""DocEntry""
{Where}
group by
	T0.""DocEntry"",
    T0.""CreateDate"",
	T0.""CreateTime"",
	T0.""Creator"",
	T0.""U_PuntoControl"",
	T0.""U_Version"",
	T0.""U_BaseType"",
	T0.""U_BaseEntry"",
	T0.""U_Revisado"",
	T0.""U_RevisadoPor""
order by 1
";

                        grid.DataTable.Clear();
                        grid.DataTable.ExecuteQuery(sql);

                        for (int i = 0; i < grid.DataTable.Columns.Count; i++)
                        {
                            grid.Columns.Item(i).Editable = false;
                        }

                        grid.Columns.Item("BaseType").Visible = false;
                        //grid.Columns.Item("BaseEntry").Visible = false;
                        grid.AutoResizeColumns();
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }

        private static void ButtonReview(string formUID, ref ItemEvent oItemEvent, SAPbobsCOM.Company sbo_company, ref Application sbo_application, out bool bBubbleEvent, string sessionId)
        {
            bBubbleEvent = true;

            if (oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    var grid = oForm.Items.Item(pluginForm.GridCalidad).Specific as Grid;
                    var DocEntry = SAPFunctions.GetFieldFromSelectedRow(grid, "Correlativo");

                    if (string.IsNullOrEmpty(DocEntry))
                    {
                        bBubbleEvent = false;
                        throw new Exception("Debe seleccionar un registro");
                    }
                }
            }

            if (!oItemEvent.BeforeAction)
            {
                var oForm = sbo_application.Forms.Item(formUID);
                if (oItemEvent.EventType == BoEventTypes.et_CLICK)
                {
                    try
                    {
                        oForm.Freeze(true);
                        var grid = oForm.Items.Item(pluginForm.GridCalidad).Specific as Grid;
                        var DocEntry = SAPFunctions.GetFieldFromSelectedRow(grid, "Correlativo");

                        SAPFunctions.LoadAproveCalidad(ref sbo_application, DocEntry.RemoveParents(), sessionId);
                    }
                    catch { throw; }
                    finally { oForm.Freeze(false); }
                }
            }
        }
    }
}
