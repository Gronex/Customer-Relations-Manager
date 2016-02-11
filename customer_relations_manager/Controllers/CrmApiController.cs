using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace customer_relations_manager.Controllers
{
    public abstract class CrmApiController : ApiController
    {
        protected IHttpActionResult GetModelErrorResponse()
        {
            var modelErrors = string.Join(",\n", ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage)));
            return BadRequest($"Invalid model\n {modelErrors}");
        }
    }
}
