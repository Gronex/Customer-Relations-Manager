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
using Core.DomainModels.Activities;
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
        private readonly IGenericRepository<Activity> _activityRepo;
        private readonly IGraphService _graphService;
        
        public GraphController(
            IGenericRepository<ProductionGoal> goalRepo, 
            IGenericRepository<Opportunity> opportunityRepo, 
            IGenericRepository<Activity> activityRepo,
            IGraphService graphService)
        {
            _goalRepo = goalRepo;
            _opportunityRepo = opportunityRepo;
            _activityRepo = activityRepo;
            _graphService = graphService;
        }

        [HttpGet]
        [Route("api/graph/goal")]
        public IHttpActionResult Goal(
            [FromUri]int[] userGroups,
            [FromUri]string[] users,
            [FromUri]DateTime? startDate = null,
            [FromUri]DateTime? endDate = null)
        {
            userGroups = userGroups ?? new int[0];
            users = users ?? new string[0];

            var year = DateTime.UtcNow.Year;
            if (!startDate.HasValue) startDate = new DateTime(year, 1, 1);
            if (!endDate.HasValue) endDate = startDate.Value.AddYears(1);

            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }


            var goals = _goalRepo.Get(g => 
                (g.StartDate <= endDate.Value) && 
                (!users.Any() || users.Contains(g.User.Email)) &&
                (!userGroups.Any() || g.User.Groups.Any(gr => userGroups.Contains(gr.UserGroupId))));
            return Ok(new GraphEnvelope<IDictionary<string, IEnumerable<DateUserGraphData>>>
            {
                From = startDate.Value,
                To = endDate.Value,
                Data = _graphService.GenerateGoalDataTable(goals, startDate.Value.Date),
            });
        }

        [HttpGet]
        [Route("api/graph/production")]
        public IHttpActionResult Production(
            [FromUri]int[] departments,
            [FromUri]int[] stages,
            [FromUri]int[] userGroups,
            [FromUri]int[] categories,
            [FromUri]string[] users,
            [FromUri]DateTime? startDate = null,
            [FromUri]DateTime? endDate = null,
            [FromUri]bool weighted = false)
        {
            userGroups = userGroups ?? new int[0];
            departments = departments ?? new int[0];
            stages = stages ?? new int[0];
            categories = categories ?? new int[0];
            users = users ?? new string[0];

            var year = DateTime.UtcNow.Year;
            if (!startDate.HasValue) startDate = new DateTime(year, 1, 1);
            if (!endDate.HasValue) endDate = startDate.Value.AddYears(1);

            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var opportunities = _opportunityRepo.Get(
                Opportunity.InTimeRange(startDate.Value.Date, endDate.Value.Date)
                .AndAlso(Opportunity.ListFilter(departments, stages, categories, userGroups, users)));
            return Ok(new GraphEnvelope<IDictionary<string, IEnumerable<DateUserGraphData>>>
            {
                From = startDate.Value,
                To = endDate.Value,
                Data = _graphService.GenerateProductionDataTable(opportunities, startDate.Value.Date, endDate.Value.Date, weighted)
            });
        }

        [HttpGet]
        [Route("api/graph/activities")]
        public IHttpActionResult Activities(
            [FromUri]int[] departments,
            [FromUri]int[] userGroups,
            [FromUri]string[] users,
            [FromUri]DateTime? startDate = null,
            [FromUri]DateTime? endDate = null)
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

            var activities = _activityRepo.Get(g =>
                g.DueDate <= endDate.Value && g.DueDate >= startDate.Value);

            return Ok(new GraphEnvelope<IEnumerable<GraphData>>
            {
                From = startDate.Value,
                To = endDate.Value,
                Data = _graphService.GenerateActivityGraph(activities)
            });
        }
    }
}
