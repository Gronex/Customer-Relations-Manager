using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using customer_relations_manager.ViewModels.Goal;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers.Users
{
    [Authorize(Roles = nameof(UserRole.Super))]
    public class GoalsController : CrmApiController
    {
        private readonly IGoalRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public GoalsController(IGoalRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<GoalViewModel>))]
        public IHttpActionResult Get(string userId)
        {
            var dbData = _repo.GetAll(userId);
            if (dbData == null)
                return NotFound();
            
            return Ok(dbData.Select(_mapper.Map<GoalViewModel>));
        }

        [HttpGet]
        [ResponseType(typeof(GoalViewModel))]
        public IHttpActionResult Get(string userId, int id)
        {
            var data = _repo.GetById(userId, id);
            if (data == null)
                return NotFound();
            
            return Ok(_mapper.Map<GoalViewModel>(data));
        }

        [HttpPost]
        public IHttpActionResult Post(string userId, GoalViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var data = _mapper.Map<ProductionGoal>(model);

            var dbData = _repo.Create(userId, data);
            if(dbData == null)
                return NotFound();

            _uow.Save();
            return Created(dbData.Id.ToString(), _mapper.Map<GoalViewModel>(dbData));
        }

        [HttpPut]
        public IHttpActionResult Put(string userId, int id, GoalViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var data = _mapper.Map<ProductionGoal>(model);

            var dbData = _repo.Update(userId, id, data);
            if (dbData == null)
                return NotFound();

            _uow.Save();
            return Ok(_mapper.Map<GoalViewModel>(dbData));
        }
        
        [HttpDelete]
        public void Delete(string userId, int id)
        {
            _repo.Delete(userId, id);
            _uow.Save();
        }
    }
}
