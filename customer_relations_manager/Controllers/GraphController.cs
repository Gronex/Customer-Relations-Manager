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

        [HttpGet, Route("Production")]
        public IDictionary<string, DataSet> GetProductionGraph()
        {
            var users = _userRepo.Get();

            return _graphService.GenerateProductionDataSets(users);
        }
    }
}
