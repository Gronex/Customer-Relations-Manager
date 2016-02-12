using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;

namespace customer_relations_manager.Controllers
{
    public abstract class CrmApiController : ApiController
    {

        protected IHttpActionResult Duplicate<T>(T model)
        {
            var result = new NegotiatedContentResult<T>(HttpStatusCode.Conflict, model, this);
            return result;
        }
    }
}
