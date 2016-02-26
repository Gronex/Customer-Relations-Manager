using System.Collections.Generic;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;

namespace Core.DomainServices.Services
{
    public interface IGraphService
    {

        // Testing...
        IDictionary<object, List<object>> GenerateGoalDataTable(IEnumerable<User> users);

        IDictionary<object, List<object>> GenerateProductionDataTable(IEnumerable<User> users);
    }
}
