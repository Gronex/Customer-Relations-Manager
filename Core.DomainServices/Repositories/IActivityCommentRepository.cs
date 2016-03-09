using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;

namespace Core.DomainServices.Repositories
{
    public interface IActivityCommentRepository
    {
        PaginationEnvelope<ActivityComment> GetAll(
            int activityId, 
            Func<IQueryable<ActivityComment>, IOrderedQueryable<ActivityComment>> orderBy, 
            int? page = null, 
            int? pageSize = null);
        IEnumerable<ActivityComment> GetAll(int activityId);
        ActivityComment GetById(int activityId, int id);
        ActivityComment Update(int activityId, string userName, int id, string comment);
        ActivityComment Create(int activityId, string userName, string comment);
        void DeleteByKey(int activityId, int id);
    }
}
