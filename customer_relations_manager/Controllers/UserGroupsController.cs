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
    [Authorize]
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
        public IEnumerable<GroupViewModel> Get()
        {
            var inDb = _repo.Get();

            return inDb.Select(ug => _mapper.Map<GroupViewModel>(ug));
        }

        [HttpGet]
        [ResponseType(typeof(GroupViewModel))]
        public IHttpActionResult Get(int id)
        {
            var group = _repo.GetByKeyThrows(id);
            return Ok(_mapper.Map<GroupViewModel>(group));
        }

        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Post(GroupViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var result = _repo.Insert(_mapper.Map<UserGroup>(model));
            _uow.Save();
            return Created(result.Id.ToString(), _mapper.Map<GroupViewModel>(result));
        }

        [HttpPut]
        [Authorize(Roles = nameof(UserRole.Super))]
        public IHttpActionResult Put(int id, GroupViewModel model)
        {
            if (model == null|| !ModelState.IsValid) return BadRequest(ModelState);
            var updated = _repo.Update(group => group.Name = model.Name, id);

            _uow.Save();
            return Ok(_mapper.Map<GroupViewModel>(updated));
        }

        [HttpDelete]
        [Authorize(Roles = nameof(UserRole.Super))]
        public void Delete(int id)
        {
            var toDelete = _repo.GetByKey(id);
            toDelete?.ActivityViewSettingses.Clear();
            toDelete?.ProductionViewSettings.Clear();
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
