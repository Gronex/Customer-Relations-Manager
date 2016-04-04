using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using customer_relations_manager.ViewModels.Activity;
using Core.DomainModels.Activities;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Filters;
using Core.DomainServices.Repositories;
using Microsoft.AspNet.Identity;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class ActivitiesController : CrmApiController
    {
        private readonly IUnitOfWork _uow;
        private readonly IActivityRepository _repo;
        private readonly IMapper _mapper;

        public ActivitiesController(IUnitOfWork uow, IActivityRepository repo, IMapper mapper)
        {
            _uow = uow;
            _repo = repo;
            _mapper = mapper;
        }

        // GET: api/Activities
        public PaginationEnvelope<ActivityOverviewViewModel> GetActivities(
            [FromUri]PagedSearchFilter filter,
            bool own = true)
        {
            filter = CorrectFilter(filter);
            //CorrectPageInfo(ref filter.Page, ref pageSize);
            var defaultOrder = new[] { "DueDate,DueTimeStart,DueTimeEnd" };
            filter.OrderBy = (filter.OrderBy ?? defaultOrder)
                .Select(o => o.ToLower()
                    .Replace("primarycontactname", "PrimaryContact.firstName")
                    .Replace("primaryresponsiblename", "PrimaryResponsible.firstName")
                    .Replace("companyname", "company.name")).ToArray();

            return _repo
                .GetAll(own ? User.Identity.Name : null, filter)
                .MapData(_mapper.Map<ActivityOverviewViewModel>);
        }


        // GET: api/Activities/5
        [ResponseType(typeof(ActivityViewModel))]
        public IHttpActionResult GetActivity(int id)
        {
            var activity = _repo.GetById(id);
            if (activity == null) return NotFound();

            return Ok(_mapper.Map<ActivityViewModel>(activity));
        }
        
        // PUT: api/Activities/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutActivity(int id, ActivityViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Update(id, _mapper.Map<Activity>(model));
            if(dbModel == null) return NotFound();
            _uow.Save();
            
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Activities
        [ResponseType(typeof(ActivityViewModel))]
        public IHttpActionResult PostActivity(ActivityViewModel model)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);

            var dbModel = _repo.Create(_mapper.Map<Activity>(model));
            _uow.Save();

            return Created(dbModel.Id.ToString(), _mapper.Map<ActivityViewModel>(dbModel));
        }

        // DELETE: api/Activities/5
        public void DeleteActivity(int id)
        {
            _repo.DeleteByKey(id);
            _uow.Save();
        }
    }
}
