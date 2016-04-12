using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using AutoMapper;
using customer_relations_manager.ViewModels.Activity;
using Core.DomainModels.Activities;
using Core.DomainServices;
using Infrastructure.DataAccess.Exceptions;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class ActivityCategoriesController : CrmApiController
    {
        private readonly IGenericRepository<ActivityCategory> _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public ActivityCategoriesController(IGenericRepository<ActivityCategory> repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [HttpGet]
        public IEnumerable<ActivityCategoryViewModel> GetAll([FromUri] string[] orderBy)
        {
            return _repo.GetOrderedByStrings(orderBy: orderBy != null && orderBy.Any() ? orderBy : new [] {"value"})
                .Select(_mapper.Map<ActivityCategoryViewModel>);
        }

        [HttpGet]
        public IHttpActionResult Get(int id)
        {
            var data = _repo.GetByKey(id);
            if (data == null) return NotFound();
            return Ok(_mapper.Map<ActivityCategoryViewModel>(data));
        }

        [HttpPost]
        public IHttpActionResult Post(ActivityCategoryViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Insert(_mapper.Map<ActivityCategory>(model));
            _uow.Save();

            return Created(dbModel.Id.ToString(), _mapper.Map<ActivityCategoryViewModel>(dbModel));
        }

        [HttpPut]
        public IHttpActionResult Put(int id, ActivityCategoryViewModel model)
        {
            if(!ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(ac =>
            {
                ac.Name = model.Name;
                // if null then Modelstate failed
                // ReSharper disable once PossibleInvalidOperationException
                ac.Value = model.Value.Value;
            }, id);
            _uow.Save();

            return Ok(_mapper.Map<ActivityCategoryViewModel>(dbModel));
        }

        [HttpDelete]
        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
