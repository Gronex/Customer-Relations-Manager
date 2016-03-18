using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.ApplicationServices.ExtentionMethods;
using Core.ApplicationServices.Graph.DataHolders;
using Core.ApplicationServices.ServiceInterfaces;
using Core.DomainModels.Graph;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;

namespace customer_relations_manager.Controllers
{
    public class GraphController : ApiController
    {
        private readonly IGenericRepository<ProductionGoal> _goalRepo;
        private readonly IGenericRepository<Opportunity> _opportunityRepo;
        private readonly IGraphService _graphService;
        
        public GraphController(IGenericRepository<ProductionGoal> goalRepo, IGenericRepository<Opportunity> opportunityRepo, IGraphService graphService)
        {
            _goalRepo = goalRepo;
            _opportunityRepo = opportunityRepo;
            _graphService = graphService;
        }

        [HttpGet]
        public IHttpActionResult Get(
            string id, 
            [FromUri]int[] departments,
            [FromUri]int[] stages,
            [FromUri]int[] userGroups,
            [FromUri]int[] categories,
            [FromUri]string[] users,
            [FromUri]DateTime? startDate = null,
            [FromUri]DateTime? endDate = null,
            [FromUri]bool weighted = false)
        {
            var year = DateTime.UtcNow.Year;
            if (!startDate.HasValue) startDate = new DateTime(year, 1, 1);
            if (!endDate.HasValue) endDate = startDate.Value.AddYears(1);

            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            // Since i cant figure out how to make it do this automaticaly
            switch (id)
            {
                case "goal":
                    var goals = _goalRepo.Get(g => 
                        (g.StartDate <= endDate.Value) && 
                        (!users.Any() || users.Contains(g.User.Email)) &&
                        (!userGroups.Any() || g.User.Groups.Select(ug => ug.UserGroupId).Except(userGroups).Any()));
                    return Ok(new GraphEnvelope<IDictionary<string, IEnumerable<UserGraphData>>>
                    {
                        From = startDate.Value,
                        To = endDate.Value,
                        Data = _graphService.GenerateGoalDataTable(goals, startDate.Value.Date),
                    });
                    //return Ok(_graphService.GenerateGoalDataTable(users, startDate.Value.Date, endDate.Value.Date));
                case "production":
                    
                    var opportunities = _opportunityRepo.Get(
                        Opportunity.InTimeRange(startDate.Value.Date, endDate.Value.Date)
                        .AndAlso(Opportunity.ListFilter(departments, stages, categories, userGroups, users)));
                     return Ok(new GraphEnvelope<IDictionary<string, IEnumerable<UserGraphData>>>
                    {
                        From = startDate.Value,
                        To = endDate.Value,
                        Data = _graphService.GenerateProductionDataTable(opportunities, startDate.Value.Date, endDate.Value.Date)
                    });
                default:
                    return NotFound();
            }
        }
    }
}
