using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using customer_relations_manager.App_Start;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace customer_relations_manager
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            WebApiConfig.Register(RouteTable.Routes);
            SerializationSettings(GlobalConfiguration.Configuration);
        }

        private static void SerializationSettings(HttpConfiguration config)
        {
            var jsonSetting = new JsonSerializerSettings();
            jsonSetting.Converters.Add(new StringEnumConverter());
            jsonSetting.ContractResolver = new CamelCasePropertyNamesContractResolver();
            
            config.Formatters.JsonFormatter.SerializerSettings = jsonSetting;
        }
    }
}
