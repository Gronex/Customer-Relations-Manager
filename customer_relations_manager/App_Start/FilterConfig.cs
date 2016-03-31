using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Filters;
using System.Web.Mvc;
using customer_relations_manager.Filters;

namespace customer_relations_manager.App_Start
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterGlobalHttpFilters(HttpFilterCollection filters)
        {
            filters.Add(new NotFoundExceptionFilterAttribute());
            filters.Add(new DuplicateExceptionFilterAttribute());
            filters.Add(new NotAllowedExceptionFilterAttribute());
            filters.Add(new ParseExceptionFilterAttribute());
        }
    }
}
