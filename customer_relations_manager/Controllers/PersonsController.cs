using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.Company;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers
{
    [RoutePrefix("api/persons")]
    public class PersonsController : ApiController
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
        public IEnumerable<PersonViewModel> Get()
        {
            return _repo.GetAll().Select(_mapper.Map<PersonViewModel>);
        }

        // GET: api/Persons/5
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetById(id);
            if(data == null) return NotFound();
            return Ok(_mapper.Map<PersonViewModel>(data));
        }

        // POST: api/Persons?companyId=1
        public IHttpActionResult Post(
            [FromBody]PersonViewModel model
            )
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);
            var dbModel = _repo.Create(_mapper.Map<Person>(model));
            if(dbModel == null) return NotFound();

            _uow.Save();
            return Created(dbModel.Id.ToString(), _mapper.Map<PersonViewModel>(dbModel));
        }

        // PUT: api/Persons/5
        public IHttpActionResult Put(int id, [FromBody]PersonViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var dbModel = _repo.Update(id, _mapper.Map<Person>(model));
            if(dbModel == null) return NotFound();

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
        public void Delete(int id, [FromUri] int? companyId = null)
        {
            _repo.Unassign(id, companyId);
            _uow.Save();
        }
    }
}
