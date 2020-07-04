using System.Web.Mvc;

namespace WebApp.Areas.ServiceContractor
{
    public class ServiceContractorAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "ServiceContractor";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "ServiceContractor_default",
                "ServiceContractor/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}