using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AlunoGest
{
    public partial class lixo : System.Web.UI.MasterPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void logout_LoggedOut(object sender, EventArgs e)
        {
            Session.Abandon();
            Session.Clear();
        }

    }
}