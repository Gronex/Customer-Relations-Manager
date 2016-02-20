using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using customer_relations_manager.ViewModels;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;

namespace customer_relations_manager.Controllers
{
    [Authorize(Roles = nameof(UserRole.Super))]
    public class UserGroupsController : CrmApiController
    {
        private readonly IGenericRepository<UserGroup> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UserGroupsController(
            IGenericRepository<UserGroup> repo,
            IUnitOfWork uow,
            IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<UserGroupViewModel> Get()
        {
            var inDb = _repo.Get();

            return inDb.Select(ug => _mapper.Map<UserGroupViewModel>(ug));
        }

        [HttpGet]
        [ResponseType(typeof(UserGroupViewModel))]
        public IHttpActionResult Get(int id)
        {
            var group = _repo.GetByKey(id);
            if(group == null) return NotFound();
            return Ok(_mapper.Map<UserGroupViewModel>(group));
        }

        [HttpPost]
        public IHttpActionResult Post(UserGroupViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = _repo.Insert(_mapper.Map<UserGroup>(model));
            try
            {
                _uow.Save();
            }
            catch (DuplicateKeyException)
            {
                return Duplicate(model);
            }
            return Created(result.Id.ToString(), _mapper.Map<UserGroupViewModel>(result));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, UserGroupViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var updated = _repo.Update(group => group.Name = model.Name, id);
            if(updated == null) return NotFound();

            try
            {
                _uow.Save();
            }
            catch (DuplicateKeyException)
            {
                return Duplicate(model);
            }
            return Ok(_mapper.Map<UserGroupViewModel>(updated));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
