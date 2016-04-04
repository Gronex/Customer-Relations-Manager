using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.Activity;
using customer_relations_manager.ViewModels.Company;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Filters;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers
{
    [RoutePrefix("api/persons")]
    [Authorize]
    public class PersonsController : CrmApiController
    {
        private readonly IPersonRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public PersonsController(IPersonRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        // GET: api/Persons
        public PaginationEnvelope<PersonViewModel> GetAll([FromUri]PagedSearchFilter filter)
        {
            filter = CorrectFilter(filter);
            filter.OrderBy = filter.OrderBy.Any() ? filter.OrderBy : new[] {"LastName"};
            return _repo
                .GetAll(filter)
                .MapData(_mapper.Map<PersonViewModel>);
        }

        // GET: api/Persons/5
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetById(id);
            return Ok(_mapper.Map<PersonViewModel>(data));
        }

        // POST: api/Persons
        public IHttpActionResult Post([FromBody]PersonViewModel model)
        {
            if(model == null || !ModelState.IsValid) return BadRequest(ModelState);
            var dbModel = _repo.Create(_mapper.Map<Person>(model));
            if(dbModel == null) return NotFound();

            _uow.Save();
            return Created(dbModel.Id.ToString(), _mapper.Map<PersonViewModel>(dbModel));
        }

        // PUT: api/Persons/5
        public IHttpActionResult Put(int id, [FromBody]PersonViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);
            var dbModel = _repo.Update(id, _mapper.Map<Person>(model));

            _uow.Save();
            return Ok(_mapper.Map<PersonViewModel>(dbModel));
        }

        [HttpPut, Route("{id}/add/{companyId}")]
        public IHttpActionResult AddToCompany(int id, int companyId)
        {
            var person = _repo.AddToCompany(id, companyId);
            if(person == null) return NotFound();
            _uow.Save();
            return Ok(_mapper.Map<PersonViewModel>(person));
        }

        // DELETE: api/Persons/5
        public void Delete(int id)
        {
            _repo.Unassign(id);
            _uow.Save();
        }

        [HttpGet, Route("{id}/activities")]
        public IEnumerable<ActivityOverviewViewModel> GetActivities(int id)
        {
            return _repo.GetById(id).Activities.Select(_mapper.Map<ActivityOverviewViewModel>);
        }
    }
}
