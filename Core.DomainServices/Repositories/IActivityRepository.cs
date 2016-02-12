using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Activities;

namespace Core.DomainServices.Repositories
{
    public interface IActivityRepository
    {
        IQueryable<Activity> GetAll();
        Activity GetById(int id);
        Task<Activity> GetByIdAsync(int id);
    }
}
