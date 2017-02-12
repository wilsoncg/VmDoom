using Akka.Actor;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using VmDoom.IRCd;

namespace VmDoom.WebApp
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected static ActorSystem actorSystem;
        protected static IActorRef IrcServer;

        protected void Application_Start()
        {
            actorSystem = ActorSystem.Create("app");
            IrcServer = actorSystem.ActorOf<IrcServer>();

            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
