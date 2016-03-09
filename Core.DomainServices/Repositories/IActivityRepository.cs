using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Activities;

namespace Core.DomainServices.Repositories
{
    public interface IActivityRepository
    {

        PaginationEnvelope<Activity> GetAll(Func<IQueryable<Activity>, IOrderedQueryable<Activity>> orderBy, int? page = null, int? pageSize = null);
        IEnumerable<Activity> GetAll();
        Activity GetById(int id);
        Activity Update(int id, Activity activity);
        Activity Create(Activity activity);
        void DeleteByKey(int id);
    }
}
