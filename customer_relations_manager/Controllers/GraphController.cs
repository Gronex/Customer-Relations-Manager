using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
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
        public IHttpActionResult Get(string id, [FromUri]DateTime? startDate = null, [FromUri]DateTime? endDate = null)
        {
            var year = DateTime.UtcNow.Year;
            if (!startDate.HasValue) startDate = new DateTime(year, 1, 1);
            if (!endDate.HasValue) endDate = new DateTime(year+1, 1, 1);

            // Since i cant figure out how to make it do this automaticaly
            switch (id)
            {
                case "goal":
                    var goals = _goalRepo.Get(g => g.StartDate <= endDate.Value);
                    return Ok(_graphService.GenerateGoalDataTable(goals, startDate.Value.Date));
                    //return Ok(_graphService.GenerateGoalDataTable(users, startDate.Value.Date, endDate.Value.Date));
                case "production":
                    var opportunities = _opportunityRepo.Get(Opportunity.InTimeRange(startDate.Value.Date, endDate.Value.Date));
                    return Ok(_graphService.GenerateProductionDataTable(opportunities, startDate.Value.Date, endDate.Value.Date));
                default:
                    return NotFound();
            }
        }
        
    }
}
