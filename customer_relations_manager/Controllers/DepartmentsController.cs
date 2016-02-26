using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class DepartmentsController : CrmApiController
    {
        private readonly IGenericRepository<Department> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public DepartmentsController(IGenericRepository<Department> repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }
        
        public IEnumerable<GroupViewModel> Get()
        {
            return _repo.Get().Select(_mapper.Map<GroupViewModel>);
        }

        public IHttpActionResult Get(int id)
        {
            var model = _repo.GetByKey(id);
            if(model == null) return NotFound();
            return Ok(_mapper.Map<GroupViewModel>(model));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Post(GroupViewModel model)
        {
            var dbModel = _repo.Insert(_mapper.Map<Department>(model));
            try
            {
                _uow.Save();
            }
            catch (Exception)
            {
                return Conflict();
            }
            return Created(dbModel.Id.ToString(), _mapper.Map<GroupViewModel>(dbModel));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Put(int id, GroupViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(d => d.Name = model.Name, id);
            if(dbModel == null) return NotFound();
            try
            {
                _uow.Save();
            }
            catch (DuplicateException)
            {
                return Conflict();
            }
            return Ok(_mapper.Map<GoalViewModel>(dbModel));
        }

        [Authorize(Roles = nameof(UserRole.Super))]
        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
