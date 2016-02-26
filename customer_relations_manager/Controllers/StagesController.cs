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
    [Authorize]
    public class StagesController : CrmApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Stage> _repo;

        public StagesController(IUnitOfWork uow, IMapper mapper, IGenericRepository<Stage> repo)
        {
            _uow = uow;
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public IEnumerable<StageViewModel> Get()
        {
            var stages = _repo.Get();
            return stages.Select(_mapper.Map<StageViewModel>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var stage = _repo.GetByKey(id);
            if (stage == null) return NotFound();

            return Ok(_mapper.Map<StageViewModel>(stage));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Post(StageViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Insert(_mapper.Map<Stage>(model));
            try
            {
                _uow.Save();
            }
            catch (DuplicateException)
            {
                return Duplicate(model);
            }

            return Created(dbModel.Id.ToString(), _mapper.Map<StageViewModel>(dbModel));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Put(int id, StageViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(s =>
            {
                s.Name = model.Name;
                s.Value = model.Value;
            }, id);

            try
            {
                _uow.Save();
            }
            catch (DuplicateException)
            {
                return Duplicate(model);
            }

            return Ok(_mapper.Map<StageViewModel>(dbModel));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
