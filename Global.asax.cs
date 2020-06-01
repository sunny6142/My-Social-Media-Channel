using My_Social_Media_Channel.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace My_Social_Media_Channel
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        void Application_Error(object sender, EventArgs e)
        {
            Response<String> response = new Response<String>();
            Exception exception = Server.GetLastError();
            Response.Clear();

            Server.ClearError();
            string message = exception.Message;
            HttpException httpException = exception as HttpException;
            RouteData routeData = new RouteData();


            if (httpException != null)
            {
                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "NotFound");
                        routeData.Values.Add("statusCode", 404);
                        break;
                    case 403:
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "UnauthorizedAccess");
                        routeData.Values.Add("statusCode", 403);
                        break;
                    case 500:
                        // server error
                        routeData.Values.Add("controller", "Error");
                        routeData.Values.Add("action", "BadRequest");
                        routeData.Values.Add("statusCode", 500);
                        break;
                    default:
                        routeData.Values.Add("Controller", "Error");
                        routeData.Values.Add("action", "Type");
                        routeData.Values.Add("statusCode", httpException.GetHttpCode());
                        break;
                }
            }
          

            //Response.RedirectToRoute(routeData.Values);

          //  return Request.CreateResponse(HttpStatusCode.OK, response, Configuration.Formatters.JsonFormatter);
        }

    }
}
