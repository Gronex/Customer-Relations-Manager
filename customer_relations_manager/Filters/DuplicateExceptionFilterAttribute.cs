using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Filters;
using Infrastructure.DataAccess.Exceptions;

namespace customer_relations_manager.Filters
{
    public class DuplicateExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is DuplicateException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.Conflict);
            }
        }
    }
}
