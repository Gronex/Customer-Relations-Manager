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
        /// <summary>
        /// Updates the pagination request arguments if they are not at resonable values
        /// </summary>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        protected void CorrectPageInfo(ref int? page, ref int? pageSize)
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;
        }

        /// <summary>
        /// Gets the hostname of the request, the port is added if it is not the default one
        /// </summary>
        /// <returns></returns>
        protected string GetHostUri()
        {
            var scheme = Request.RequestUri.Scheme;
            var port = Request.RequestUri.IsDefaultPort ? string.Empty : ":" + Request.RequestUri.Port;
            return $"{scheme}://{Request.RequestUri.Host}{port}";
        }
    }
}
