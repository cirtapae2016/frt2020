using CoreUtilities;
using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SAPWEB
{
    public partial class login : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            //if (!IsPostBack)
            //{
            drpList.Items.Clear();
            ListItem i;
            i = new ListItem("FRUTEXSA");
            drpList.Items.Add(i);
            i = new ListItem("PROCESADORA");
            drpList.Items.Add(i);
            i = new ListItem("PASERA");
            drpList.Items.Add(i);

            //}
        }

        protected void btnConectar_Click(object sender, EventArgs e)

        {
            cUsuario objEmpresa = new cUsuario(txtUsuario.Text, txtClave.Text, drpList.Text);
            //utiles objEmpresa = new utiles(drpList.Text, txtUsuario.Text, txtClave.Text);
            if (objEmpresa.Conectado)
            {
                Session["Usuario"] = objEmpresa;
                var cLogin = new CoreUtilities.Login { UserName = objEmpresa.Usuario, CompanyDB = objEmpresa.BaseDatos, Password = txtClave.Text };
                var Response = CommonFunctions.POST(ServiceLayer.Login, cLogin, null, out _);
                Session["ServiceLayer"] = Response;
                Server.Transfer("default.aspx?");
            }
            else
            {
                Label3.Text = "<h2> Error de Conexión: </h2>" + objEmpresa.Errorconexion;
            }
        }
    }
}