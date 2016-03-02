using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DomainServices
{
    public class PaginationEnvelope<T>
    {
        public int PageSize { get; set; }
        public int PageNumber { get; set; }
        public int ItemCount { get; set; }
        public IEnumerable<T> Data { get; set; }
        
        public PaginationEnvelope<T2> MapData<T2>(Func<T, T2> mapper)
        {
            return new PaginationEnvelope<T2>
            {
                ItemCount = ItemCount,
                PageNumber = PageNumber,
                PageSize = PageSize,
                Data = Data.Select(mapper)
            };
        }
    }
}
