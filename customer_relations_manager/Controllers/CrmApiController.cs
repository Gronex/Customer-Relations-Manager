using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using Core.DomainServices.Filters;

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
        
        protected PagedSearchFilter CorrectFilter(PagedSearchFilter filter)
        {
            filter = filter ?? new PagedSearchFilter();
            if (filter.Page < 1 || filter.Page == null)
                filter.Page = 1;
            if (filter.PageSize < 1)
                filter.PageSize = 10;
            if(filter.OrderBy == null)
                filter.OrderBy = new List<string>();

            return filter;
        }
    }
}
