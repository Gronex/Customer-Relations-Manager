using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Routing;

namespace customer_relations_manager
{
    public static class WebApiConfig
    {
        public static void Register(RouteCollection routes)
        {
            // Web API configuration and services
            
            // Web API routes
            //config.MapHttpAttributeRoutes();



            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "UserApi",
                routeTemplate: "api/users/{userId}/{controller}/{id}",
                defaults: new {id = RouteParameter.Optional }
            );
        }
    }
}
