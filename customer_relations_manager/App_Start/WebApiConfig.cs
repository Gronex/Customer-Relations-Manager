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
                name: "AccountApi",
                routeTemplate: "api/{controller}/{action}"
            );

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

            routes.MapHttpRoute(
                name: "ActivityCommentApi",
                routeTemplate: "api/activities/{activityId}/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            routes.MapHttpRoute(
                name: "OneLevelNested",
                routeTemplate: "api/{controller}/{mainId}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
