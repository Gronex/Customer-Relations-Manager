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
        public IEnumerable<ActivityCategoryViewModel> GetAll()
        {
            return _repo.Get(orderBy: ac => ac
                    .OrderBy(a => a.Name)
                    .ThenBy(a => a.Id))
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
            try { _uow.Save(); }
            catch (DuplicateException) { return Duplicate(model);}

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
            try { _uow.Save(); }
            catch (DuplicateException) { return Duplicate(model); }

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