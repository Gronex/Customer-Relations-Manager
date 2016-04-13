using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using AutoMapper;
using customer_relations_manager.ViewModels;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace customer_relations_manager.Controllers.Activities
{
    [Authorize(Roles = nameof(UserRole.Standard))]
    [Route("api/activities/{activityId}/comments")]
    public class CommentsController : CrmApiController
    {
        private readonly IActivityCommentRepository _repo;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public CommentsController(IActivityCommentRepository repo, IUnitOfWork uow, IMapper mapper)
        {
            _repo = repo;
            _uow = uow;
            _mapper = mapper;
        }

        [ResponseType(typeof(PaginationEnvelope<CommentViewModel>))]
        [HttpGet]
        public PaginationEnvelope<CommentViewModel> GetAll([FromUri] int activityId, int? page = null, int? pageSize = null)
        {
            CorrectPageInfo(ref page, ref pageSize);

            return _repo.GetAll(activityId, cs => cs.OrderBy(c => c.Sent).ThenBy(c => c.Id), page, pageSize)
                .MapData(_mapper.Map<CommentViewModel>);
        }

        public IHttpActionResult Post(int activityId, [FromBody]string comment)
        {
            if(string.IsNullOrWhiteSpace(comment)) return BadRequest();

            var dbComment = _repo.Create(activityId, User.Identity.Name, comment);
            if(dbComment == null) return NotFound();
            _uow.Save();
            return Created(dbComment.Id.ToString(), _mapper.Map<CommentViewModel>(dbComment));
        }
    }
}
