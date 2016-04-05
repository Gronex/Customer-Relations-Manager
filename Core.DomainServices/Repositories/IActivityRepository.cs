using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainServices.Filters;

namespace Core.DomainServices.Repositories
{
    public interface IActivityRepository
    {
        PaginationEnvelope<Activity> GetAll(
        string userName,
        PagedSearchFilter filter);
        
        IEnumerable<Activity> GetAll(int amount, string userName, string find);
        Activity GetById(int id);
        Activity Update(int id, Activity activity);
        Activity Create(Activity activity);
        void DeleteByKey(int id);
    }
}
