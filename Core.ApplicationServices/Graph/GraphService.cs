using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;
using Core.DomainServices.Services;

namespace Core.ApplicationServices.Graph
{
    public class GraphService : IGraphService
    {
        public IDictionary<string, DataSet> GenerateProductionDataSets(IEnumerable<User> users)
        {
            throw new NotImplementedException();
        }
    }
}
