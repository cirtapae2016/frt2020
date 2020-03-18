using System;
using System.Data;
using System.Web.UI;

namespace SAPWEB
{
    public partial class resumen : Page

    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Server.Transfer("login.aspx");
            }
            //const string _op3 = "select \"DocNum\" as OF1, \"StartDate\" as FechaProgramada, DAYS_BETWEEN(\"StartDate\", CURRENT_DATE) as DiasAbierta, CASE \"Status\" When 'C' Then 'Cancelada' When 'R' Then 'Liberada' When 'L' Then 'Cerrada' When 'P'  Then 'Planificada' END as Estado,\"ProdName\" as Especie, \"ProdName\" as Articulo, \"PlannedQty\" as Planificado,  'orden.aspx?' as url from  \"SBO04DEMOPROCESADORA\".\"OWOR\" where \"Status\" = 'R'";
            DataTable Dt = new DataTable();

            cUsuario Usuario = (cUsuario)Session["Usuario"];
            int _Opcion;
            _Opcion = int.Parse(Request.QueryString["vOp"]);

            GridResumen.Columns[0].ItemStyle.Width = 100;
            GridResumen.Columns[1].ItemStyle.Width = 170;
            GridResumen.Columns[2].ItemStyle.Width = 80;
            GridResumen.Columns[3].ItemStyle.Width = 100;
            GridResumen.Columns[4].ItemStyle.Width = 110;
            GridResumen.Columns[5].ItemStyle.Width = 400;
            GridResumen.Columns[6].ItemStyle.Width = 90;
            lblProceso.Text = "Ordenes de Fabricacion";

            switch (_Opcion)
            {
                case 1:
                    lblprocesoDesc.Text = "* todas *";
                    break;

                case 2:
                    lblprocesoDesc.Text = "* cerradas *";
                    break;

                case 3:
                    lblprocesoDesc.Text = "* abiertas *";
                    break;

                case 4:
                    lblprocesoDesc.Text = "* planificadas *";
                    break;
            }

            cRecordSet rs2 = new cRecordSet();
            GridResumen.DataSource = rs2.clsResumenOF(_Opcion, Usuario);
            GridResumen.DataBind();
        }

        protected void btnExpConsumos_Click(object sender, EventArgs e)

        {
        }
    }
}

//private void ExportToExcel(string nameReport, GridView wControl)
//{
//    wControl.AllowPaging = false;
//    wControl.DataBind();
//    HttpResponse response = Response;
//    StringWriter sw = new StringWriter();
//    HtmlTextWriter htw = new HtmlTextWriter(sw);
//    Page pageToRender = new Page();
//    HtmlForm form = new HtmlForm();
//    form.Controls.Add(wControl);
//    pageToRender.Controls.Add(form);
//    response.Clear();
//    response.Buffer = true;
//    response.ContentType = "application/vnd.ms-excel";
//    response.AddHeader("Content-Disposition", "attachment;filename=" + nameReport);
//    response.Charset = "UTF-8";
//    response.ContentEncoding = Encoding.Default;
//    pageToRender.RenderControl(htw);
//    response.Write(sw.ToString());
//    response.End();
//    wControl.AllowPaging = true;
//    wControl.DataBind();
//}