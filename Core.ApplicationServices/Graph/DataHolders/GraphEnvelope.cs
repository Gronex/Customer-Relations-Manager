using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.Graph.DataHolders
{
    public class GraphEnvelope<T>
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public T Data { get; set; }
    }
}
