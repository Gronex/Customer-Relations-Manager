using System.Collections.Generic;

namespace Core.DomainServices.Filters
{
    public class PagedSearchFilter
    {
        public string Find { get; set; }
        public IEnumerable<string> OrderBy { get; set; }
        public int? Page { get; set; }
        public int? PageSize { get; set; }
    }
}
