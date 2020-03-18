using System;
using System.Web.UI;

namespace SAPWEB
{
    public partial class ConsOF : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Server.Transfer("login.aspx");
            }
            cUsuario Usuario = (cUsuario)Session["Usuario"];
            //lblMensaje.Text = Usuario.BaseDatos;
            clsResumenOF oResumen = new clsResumenOF(Usuario.Empresa, Usuario.BaseDatos);
            lblOrdenes.Text = oResumen.Ofs.ToString();
            lblAbiertas.Text = oResumen.Abiertas.ToString();
            lblCerradas.Text = oResumen.Cerradas.ToString();
            lblSinHoras.Text = oResumen.SinHoras.ToString();
        }

        protected void btnVerTotal_Click(object sender, EventArgs e)

        {
            Server.Transfer("resumen.aspx?vOp=1");
        }

        protected void btnVerAbiertas_Click(object sender, EventArgs e)

        {
            Server.Transfer("resumen.aspx?vOp=3");
        }

        protected void btnVerCerradas_Click(object sender, EventArgs e)

        {
            Server.Transfer("resumen.aspx?vOp=2");
        }

        protected void btnVerShoras_Click(object sender, EventArgs e)

        {
            Server.Transfer("resumen.aspx?vOp=4");
        }
    }
}