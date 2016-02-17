using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.Company;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class CompaniesController : CrmApiController
    {
        private readonly ICompanyRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CompaniesController(ICompanyRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<CompanyOverviewViewModel> Get()
        {
            var data = _repo.GetAll();
            return data.Select(_mapper.Map<CompanyOverviewViewModel>);
        }

        [HttpGet]
        [ResponseType(typeof(IEnumerable<CompanyViewModel>))]
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetById(id);
            if(data == null) return NotFound();

            return Ok(_mapper.Map<CompanyViewModel>(data));
        }

        [HttpPost]
        public IHttpActionResult Post(CompanyViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Create(_mapper.Map<Company>(model));
            _uow.Save();

            return Created(dbModel.Id.ToString(), _mapper.Map<CompanyViewModel>(dbModel));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, CompanyViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(id, _mapper.Map<Company>(model));
            if(dbModel == null) return NotFound();
            _uow.Save();

            return Ok(_mapper.Map<CompanyViewModel>(dbModel));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.Delete(id);
            _uow.Save();
        }
    }
}
