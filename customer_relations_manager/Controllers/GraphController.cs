using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Core.DomainModels.Graph;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Services;

namespace customer_relations_manager.Controllers
{
    public class GraphController : ApiController
    {
        private readonly IGenericRepository<User> _userRepo;
        private readonly IGraphService _graphService;
        
        public GraphController(IGenericRepository<User> userRepo, IGraphService graphService)
        {
            _userRepo = userRepo;
            _graphService = graphService;


        }

        [HttpGet]
        public IHttpActionResult Get(string id, [FromUri]DateTime? startDate = null, [FromUri]DateTime? endDate = null)
        {
            // Since i cant figure out how to make it do this automaticaly
            var users = _userRepo.Get();
            switch (id)
            {
                case "goal":
                    return Ok(_graphService.GenerateGoalDataTable(users, startDate, endDate));
                case "production":
                    return Ok(_graphService.GenerateProductionDataTable(users, startDate, endDate));
                default:
                    return NotFound();
            }
        }
        
    }
}
