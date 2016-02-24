using System.Collections.Generic;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;

namespace Core.DomainServices.Services
{
    public interface IGraphService
    {
        IDictionary<string, DataSet> GenerateProductionDataSets(IEnumerable<User> users);
    }
}
