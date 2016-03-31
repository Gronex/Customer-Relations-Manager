using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Company;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using System.Data.Entity;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class CompaniesController : CrmApiController
    {
        private readonly IGenericRepository<Company> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CompaniesController(IGenericRepository<Company> repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public PaginationEnvelope<CompanyOverviewViewModel> GetAll([FromUri]string[] orderBy, int? page = null, int? pageSize = null)
        {
            CorrectPageInfo(ref page, ref pageSize);
            
            var data = _repo.GetPaged(orderBy == null || orderBy.Length < 1 
                ? new[] { "name" } 
                : orderBy, page, pageSize);
            return data.MapData(_mapper.Map<CompanyOverviewViewModel>);
        }

        [HttpGet]
        [ResponseType(typeof(CompanyViewModel))]
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetByKey(id);
            if(data == null) return NotFound();

            return Ok(_mapper.Map<CompanyViewModel>(data));
        }

        [HttpPost]
        public IHttpActionResult Post(CompanyViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Insert(_mapper.Map<Company>(model));
            _uow.Save();

            return Created(dbModel.Id.ToString(), _mapper.Map<CompanyViewModel>(dbModel));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, CompanyViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(company =>
            {
                var updated = _mapper.Map<Company>(model);

                company.Address = updated.Address;
                company.City = updated.City;
                company.Country = updated.Country;
                company.Name = updated.Name;
                company.PhoneNumber = updated.PhoneNumber;
                company.PostalCode = updated.PostalCode;
                company.WebSite = updated.WebSite;
            }, id);
            
            if(dbModel == null) return NotFound();
            _uow.Save();

            return Ok(_mapper.Map<CompanyViewModel>(dbModel));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }

        [HttpGet]
        [Route("api/companies/{id}/persons")]
        public IHttpActionResult Persons(int id)
        {
            var company = _repo.GetByKey(id);
            if(company == null) return NotFound();

            var employees = company.Employees;
            return Ok(employees.Select(_mapper.Map<PersonViewModel>));
        }
    }
}
