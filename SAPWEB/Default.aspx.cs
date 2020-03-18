using System;
using System.Web.UI;

namespace SAPWEB
{
    public partial class _Default : Page

    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Server.Transfer("login.aspx");
            }
            cUsuario Usuario = (cUsuario)Session["Usuario"];
            //lblMensaje.Text = Usuario.BaseDatos;
        }
    }
}