using System.Collections.Generic;

namespace Core.DomainModels.Graph
{
    public class DataSet
    {
        public string Label { get; set; }
        public IEnumerable<DataPoint> DataPoints { get; set; }
    }
}
