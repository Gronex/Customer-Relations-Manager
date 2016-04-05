using System;
using System.Collections.Generic;
using Core.ApplicationServices.Graph.DataHolders;
using Core.DomainModels.Activities;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;

namespace Core.ApplicationServices.ServiceInterfaces
{
    public interface IGraphService
    {
        IDictionary<string, IEnumerable<DateUserGraphData>> GenerateProductionDataTable(IEnumerable<Opportunity> opportunities, DateTime from, DateTime to, bool weighted);
        IDictionary<string, IEnumerable<DateUserGraphData>> GenerateGoalDataTable(IEnumerable<ProductionGoal> goals, DateTime startDate);
        IEnumerable<GraphData> GenerateActivityGraph(IEnumerable<Activity> activities);
    }
}
