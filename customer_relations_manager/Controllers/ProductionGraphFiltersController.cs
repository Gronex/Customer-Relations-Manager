using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.GraphFilter.ProductionGraph;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Microsoft.AspNet.Identity;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class ProductionGraphFiltersController : CrmApiController
    {
        private readonly IProductionViewSettingsRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ProductionGraphFiltersController(IProductionViewSettingsRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<ProductionGraphFilterOverviewViewModel> Get()
        {
            return _repo.GetAll(User.Identity.GetUserName()).Select(_mapper.Map<ProductionGraphFilterOverviewViewModel>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            return Ok(_mapper.Map<ProductionGraphFilterViewModel>(_repo.GetById(id)));
        }

        [HttpPost]
        public IHttpActionResult Post(ProductionGraphFilterViewModel model)
        {
            if(model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var inDb = _repo.Create(_mapper.Map<ProductionViewSettings>(model), User.Identity.Name);
            _uow.Save();
            return Created(inDb.Id.ToString(), _mapper.Map<ProductionGraphFilterViewModel>(inDb));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, ProductionGraphFilterViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);
            var inDb = _repo.Update(id, _mapper.Map<ProductionViewSettings>(model), User.Identity.Name);
            _uow.Save();
            return Ok(_mapper.Map<ProductionGraphFilterViewModel>(inDb));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.Delete(id, User.Identity.Name);
            _uow.Save();
        }
    }
}
