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
using Core.DomainServices.Filters;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers
{
    [Authorize]
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
        public PaginationEnvelope<OpportunityOverviewViewMode> GetAll([FromUri]PagedSearchFilter filter)
        {
            filter = CorrectFilter(filter);
            
            filter.OrderBy = (filter.OrderBy.Any() ? filter.OrderBy : new[] { "name" })
                .Select(o => o.ToLower()
                    .Replace("ownername", "owner.firstName")
                    .Replace("companyname", "company.name")).ToArray();

            var data = _repo.GetAll(filter);
            return data.MapData(_mapper.Map<OpportunityOverviewViewMode>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetById(id);
            return Ok(_mapper.Map<OpportunityViewModel>(data));
        }

        [HttpPost]
        public IHttpActionResult Post(OpportunityViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var data = _mapper.Map<Opportunity>(model);

            var dbModel = _repo.Create(data, User.Identity.Name);

            _uow.Save();
            return Created(dbModel.Id.ToString(), _mapper.Map<OpportunityViewModel>(dbModel));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, OpportunityViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

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
