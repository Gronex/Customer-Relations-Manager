using System;
using System.Collections.Generic;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;

namespace Core.DomainServices.Services
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
    }
}
