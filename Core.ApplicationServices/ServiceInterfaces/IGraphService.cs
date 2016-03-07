using System;
using System.Collections.Generic;
using Core.ApplicationServices.Graph.DataHolders;
using Core.DomainModels.Users;

namespace Core.ApplicationServices.ServiceInterfaces
{
    public interface IGraphService
    {
        IDictionary<object, List<object>> GenerateGoalDataTable(
            IEnumerable<User> users,
            DateTime? startDate = null,
            DateTime? endDate = null
        );

        IDictionary<object, List<object>> GenerateProductionDataTable(
            IEnumerable<User> users,
            DateTime? startDate = null,
            DateTime? endDate = null
        );

        IDictionary<string, IEnumerable<ProductionData>> Test(DateTime? from, DateTime? to);
    }
}
