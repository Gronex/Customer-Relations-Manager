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
        public IHttpActionResult Get(string id)
        {
            // Since i cant figure out how to make it do this automaticaly
            switch (id)
            {
                case "goal":
                    //return Ok(GetGoalData());

                    var users = _userRepo.Get();

                    return Ok(_graphService.GenerateGoalDataTable(users));
                case "production":
                    return Ok(GetProductionData());
                default:
                    return NotFound();
            }
        }

        public IDictionary<string, DataSet> GetGoalData()
        {
            var users = _userRepo.Get();

            return _graphService.GenerateGoalDataSets(users);
        }

        public IDictionary<string, DataSet> GetProductionData()
        {
            var users = _userRepo.Get();

            return _graphService.GenerateProductionDataSets(users);
        }
    }
}
