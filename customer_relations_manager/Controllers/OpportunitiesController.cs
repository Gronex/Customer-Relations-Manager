using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.Opportunity;
using Core.DomainModels.Opportunity;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers
{
    public class OpportunitiesController : CrmApiController
    {
        private readonly IOpportunityRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OpportunitiesController(IOpportunityRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<OpportunityOverviewViewMode> Get()
        {
            var data = _repo.GetAll();
            return data.Select(_mapper.Map<OpportunityOverviewViewMode>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetById(id);
            if (data == null) return NotFound();

            return Ok(_mapper.Map<OpportunityViewModel>(data));
        }

        [HttpPost]
        public IHttpActionResult Post(OpportunityViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var data = _mapper.Map<Opportunity>(model);

            var dbModel = _repo.Create(data, User.Identity.Name);

            if (dbModel == null)
                return NotFound();

            _uow.Save();
            return Created(dbModel.Id.ToString(), _mapper.Map<OpportunityViewModel>(dbModel));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, OpportunityViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var data = _mapper.Map<Opportunity>(model);
            var dbModel = _repo.Update(id, data);
            _uow.Save();
            return Ok(_mapper.Map<OpportunityViewModel>(dbModel));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.Delete(id);
            _uow.Save();
        }
    }
}
