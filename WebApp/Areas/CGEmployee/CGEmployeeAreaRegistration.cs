using System.Web.Mvc;

namespace WebApp.Areas.CGEmployee
{
    public class CGEmployeeAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "CGEmployee";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "CGEmployee_default",
                "CGEmployee/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}