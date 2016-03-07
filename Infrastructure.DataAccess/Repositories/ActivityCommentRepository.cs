using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class ActivityCommentRepository : IActivityCommentRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<ActivityComment> _repo;

        public ActivityCommentRepository(IApplicationContext context, IGenericRepository<ActivityComment> repo)
        {
            _context = context;
            _repo = repo;
        }

        public PaginationEnvelope<ActivityComment> GetAll(int activityId, Func<IQueryable<ActivityComment>, IOrderedQueryable<ActivityComment>> orderBy, int? page = null, int? pageSize = null)
        {
            return _repo.GetPaged(cs => cs.OrderBy(c => c.Sent).ThenBy(c => c.Id), page, pageSize,
                ac => ac.ActivityId == activityId);
        }

        public IEnumerable<ActivityComment> GetAll(int activityId)
        {
            return _repo.Get(ac => ac.ActivityId == activityId);
        }

        public ActivityComment GetById(int activityId, int id)
        {
            throw new NotImplementedException();
        }

        public ActivityComment Update(int activityId, string userName, int id, ActivityComment comment)
        {
            throw new NotImplementedException();
        }

        public ActivityComment Create(int activityId, string userName, ActivityComment comment)
        {
            comment.ActivityId = activityId;

            comment.Activity = null;
            comment.User = null;
            comment.UserId = _context.Users.SingleOrDefault(u => u.UserName == userName)?.Id;
            if (comment.UserId == null || !_context.Activities.Any(a => a.Id == activityId)) return null;
            comment.Sent = DateTime.UtcNow;
            return _repo.Insert(comment);
        }

        public void DeleteByKey(int activityId, int id)
        {
            throw new NotImplementedException();
        }
    }
}
