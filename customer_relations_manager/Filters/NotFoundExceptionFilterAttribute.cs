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
    public class NotFoundExceptionFilterAttribute : ExceptionFilterAttribute
    {
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {
            if (actionExecutedContext.Exception is NotFoundException)
            {
                actionExecutedContext.Response = new HttpResponseMessage(HttpStatusCode.NotFound);
                if(!string.IsNullOrWhiteSpace(actionExecutedContext.Exception.Message))
                actionExecutedContext.Response.Content = new StringContent(actionExecutedContext.Exception.Message);
            }
        }
    }
}
