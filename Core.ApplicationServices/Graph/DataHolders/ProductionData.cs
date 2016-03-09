using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices.Graph.DataHolders
{
    public class UserGraphData
    {
        public SimpleUser User { get; set; }
        public double Value { get; set; }
        public DateTime Period { get; set; }
    }
}
