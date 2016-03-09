using System;
using System.Collections.Generic;
using Core.ApplicationServices.Graph.DataHolders;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;

namespace Core.ApplicationServices.ServiceInterfaces
{
    public interface IGraphService
    {
        IDictionary<string, IEnumerable<UserGraphData>> GenerateProductionDataTable(IEnumerable<Opportunity> opportunities, DateTime from, DateTime to);
        IDictionary<string, IEnumerable<UserGraphData>> GenerateGoalDataTable(IEnumerable<ProductionGoal> goals, DateTime startDate);
    }
}