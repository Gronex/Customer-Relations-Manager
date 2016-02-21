using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels;
using Core.DomainModels.Opportunity;
using Core.DomainModels.Users;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;

namespace customer_relations_manager.Controllers
{
    [Authorize(Roles = nameof(UserRole.Super))]
    public class OpportunityCategoriesController : CrmApiController
    {
        private readonly IGenericRepository<OpportunityCategory> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public OpportunityCategoriesController(IGenericRepository<OpportunityCategory> repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize]
        public IEnumerable<CategoryViewModel> Get()
        {
            return _repo.Get().Select(_mapper.Map<CategoryViewModel>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var model = _repo.GetByKey(id);

            if(model == null) return NotFound();

            return Ok(_mapper.Map<CategoryViewModel>(model));
        }


        [HttpPost]
        public IHttpActionResult Post(CategoryViewModel model)
        {
            var dbModel = _repo.Insert(_mapper.Map<OpportunityCategory>(model));
            try
            {
                _uow.Save();
            }
            catch (DuplicateException)
            {
                return Conflict();
            }

            return Created(dbModel.Id.ToString(), _mapper.Map<CategoryViewModel>(dbModel));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, CategoryViewModel model)
        {
            var dbModel = _repo.Update(oc =>
            {
                oc.Name = model.Name;
                oc.Value = model.Value;
            }, id);

            if (dbModel == null) return NotFound();

            try
            {
                _uow.Save();
            }
            catch (DuplicateException)
            {
                return Conflict();
            }

            return Ok(_mapper.Map<CategoryViewModel>(dbModel));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
