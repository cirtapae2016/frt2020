using CoreUtilities;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAPWEB
{
    public partial class consumos : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Server.Transfer("login.aspx");
            }
            //const string _op3 = "select \"DocNum\" as OF1, \"StartDate\" as FechaProgramada, DAYS_BETWEEN(\"StartDate\", CURRENT_DATE) as DiasAbierta, CASE \"Status\" When 'C' Then 'Cancelada' When 'R' Then 'Liberada' When 'L' Then 'Cerrada' When 'P'  Then 'Planificada' END as Estado,\"ProdName\" as Especie, \"ProdName\" as Articulo, \"PlannedQty\" as Planificado,  'orden.aspx?' as url from  \"SBO04DEMOPROCESADORA\".\"OWOR\" where \"Status\" = 'R'";

            if (!IsPostBack)
            {
                DrpListOrden.Items.Clear();
                cUsuario Usuario = (cUsuario)Session["Usuario"];

                DataTable Ordenes = new DataTable();
                cRecordSet cOrd = new cRecordSet();

                Ordenes = cOrd.clsResumenOF(3, Usuario);

                if (Ordenes.Rows.Count > 0)
                {
                    foreach (DataRow row in Ordenes.Rows)
                    {
                        ListItem i;
                        i = new ListItem(row["OF1"].ToString() + " / " + row["ESPECIE"].ToString(), row["DocEntry"].ToString());
                        DrpListOrden.Items.Add(i);
                    }
                }
            }
            txtConsumoLote.Focus();
        }

        protected void btnConectar_Click(object sender, EventArgs e)

        {
        }

        protected void txtConsumoLote_TextChanged(object sender, EventArgs e)

        {
            var Response = Session["ServiceLayer"];
            string respuesta = CommonFunctions.ConsumoLoteCalibrado("19387-1", "138", "0", Response.ToString());
        }
    }
}