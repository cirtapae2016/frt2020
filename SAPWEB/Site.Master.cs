using System;
using System.Web.UI;

namespace SAPWEB
{
    public partial class SiteMaster : MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Server.Transfer("login.aspx");
            }
            cUsuario Usuario = (cUsuario)Session["Usuario"];
            if (!Usuario.Empresa.Connected)
            {
                Usuario.Reconectar();
                if (!Usuario.Empresa.Connected)
                {
                    lblMensaje.Text = "<b>Error Conexion:</b> " + Usuario.Errorconexion;
                }
            }
            else
            {
                lblMensaje.Text = "<b>Sociedad conectada:</b> " + Usuario.NombreEmpresa + "<b>Usuario: </b>" + Usuario.NombreUsuario;
                Session["Usuario"] = Usuario;
            }
        }
    }
}