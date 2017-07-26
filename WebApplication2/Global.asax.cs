using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using WebApplication2.Models;

namespace WebApplication2
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
        }

        public MvcApplication()
        {
            AuthorizeRequest += new EventHandler(SAuthorizeRequest);
        }

        void SAuthorizeRequest(object sender, EventArgs e)
        {
            IIdentity id = Context.User.Identity;
            if (id.IsAuthenticated)
            {
                var roles = new ApplicationDbContext().GetRoles(id.Name);

                List<string> roles_d = new List<string>(roles.Split(','));
                roles_d.Add(roles);

                Context.User = new GenericPrincipal(id, roles_d.ToArray());
            }
        }

    }
}
