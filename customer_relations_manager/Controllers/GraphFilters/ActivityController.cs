using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.GraphFilter;
using customer_relations_manager.ViewModels.GraphFilter.ActivityGraph;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Microsoft.AspNet.Identity;

namespace customer_relations_manager.Controllers.GraphFilters
{
    [Route("api/graphfilters/activity")]
    [Authorize]
    public class ActivityController : CrmApiController
    {
        private readonly IActivityViewSettingsRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivityController(IActivityViewSettingsRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<GraphFilterOverviewViewModel> Get()
        {
            return _repo.GetAll(User.Identity.GetUserName()).Select(_mapper.Map<GraphFilterOverviewViewModel>);
        }

        [HttpGet]
        [Route("api/graphfilters/activity/{id}")]
        public IHttpActionResult Get(int id)
        {
            return Ok(_mapper.Map<ActivityGraphFilterViewModel>(_repo.GetById(id)));
        }

        [HttpPost]
        public IHttpActionResult Post(ActivityGraphFilterViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var inDb = _repo.Create(_mapper.Map<ActivityViewSettings>(model), User.Identity.Name);
            _uow.Save();
            return Created(inDb.Id.ToString(), _mapper.Map<ActivityGraphFilterViewModel>(inDb));
        }

        [HttpPut]
        [Route("api/graphfilters/activity/{id}")]
        public IHttpActionResult Put(int id, ActivityGraphFilterViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);
            var inDb = _repo.Update(id, _mapper.Map<ActivityViewSettings>(model), User.Identity.Name);
            _uow.Save();
            return Ok(_mapper.Map<ActivityGraphFilterViewModel>(inDb));
        }

        [HttpDelete]
        [Route("api/graphfilters/activity/{id}")]
        public void Delete(int id)
        {
            _repo.Delete(id, User.Identity.Name);
            _uow.Save();
        }
    }
}
