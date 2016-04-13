using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.Opportunity;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;

namespace customer_relations_manager.Controllers
{
    [Authorize(Roles = nameof(UserRole.Standard))]
    public class StagesController : CrmApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Stage> _repo;

        public StagesController(IGenericRepository<Stage> repo, IUnitOfWork uow, IMapper mapper)
        {
            _uow = uow;
            _mapper = mapper;
            _repo = repo;
        }
        
        [HttpGet]
        public IEnumerable<StageViewModel> GetAll()
        {
            var stages = _repo.Get(orderBy: s => s.OrderBy(st => st.Value));
            return stages.Select(_mapper.Map<StageViewModel>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var stage = _repo.GetByKeyThrows(id);
            return Ok(_mapper.Map<StageViewModel>(stage));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Post(StageViewModel model)
        {
            if(model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Insert(_mapper.Map<Stage>(model));
            _uow.Save();

            return Created(dbModel.Id.ToString(), _mapper.Map<StageViewModel>(dbModel));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Put(int id, StageViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(s =>
            {
                s.Name = model.Name;
                s.Value = model.Value;
            }, id);

            _uow.Save();

            return Ok(_mapper.Map<StageViewModel>(dbModel));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public void Delete(int id)
        {
            var toDelete = _repo.GetByKey(id);
            toDelete?.ProductionViewSettings.Clear();
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
