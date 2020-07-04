using Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebApp.Controllers
{
    public class BaseController : Controller
    {
        private ILog iLog;

        public BaseController()
        {
            iLog = Log.GetInstance;
        }
        protected override void OnException(ExceptionContext filterContext)
        {
            var msg = iLog.CaughtExceptions(filterContext.Exception, filterContext.Controller.ToString());            

            Exception exception = filterContext.Exception;
            //Logging the Exception
            filterContext.ExceptionHandled = true;


            var Result = this.View("Error", new HandleErrorInfo(exception,
                filterContext.RouteData.Values["controller"].ToString(),
                filterContext.RouteData.Values["action"].ToString()));
            
            filterContext.Result = Result;
           

        }
    }
}