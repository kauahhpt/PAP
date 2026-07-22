using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;

namespace AlunoGest
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            ScriptManager.ScriptResourceMapping.AddDefinition(
                "jquery",
                new ScriptResourceDefinition
                {
                    Path = "~/scripts/jquery-3.7.1.min.js",
                    DebugPath = "~/scripts/jquery-3.7.1.js",
                    CdnPath = "https://ajax.microsoft.com/ajax/jQuery/jquery-3.7.1.min.js",
                    CdnDebugPath = "https://ajax.microsoft.com/ajax/jQuery/jquery-3.7.1.js"
                });

            string[] rolesNecessarias =
            {
            "administrador",
            "aluno",
            "professor",
            "agrupamento",
            "encarregado"
        };

            foreach (string role in rolesNecessarias)
            {
                if (!Roles.RoleExists(role))
                {
                    Roles.CreateRole(role);
                }
            }



        }
        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}