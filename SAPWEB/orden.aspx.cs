using System;
using System.Web.UI;

namespace SAPWEB
{
    public partial class orden : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
            {
                Server.Transfer("login.aspx");
            }

            cUsuario Usuario = (cUsuario)Session["Usuario"];
            int vOrden;
            if (!IsPostBack)
            {
            }
            else
            {
                //vEmpresa = Request.Cookies["vEMPRESA"].Value;
                //vORDEN = int.Parse(Request.Cookies["vORDEN"].Value);
            }
        }
    }
}