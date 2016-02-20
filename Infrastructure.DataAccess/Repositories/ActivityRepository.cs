using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IApplicationContext _context;

        public ActivityRepository(IApplicationContext context)
        {
            _context = context;
        }

        public IQueryable<Activity> GetAll()
        {
            return _context.Activities;
        }

        public Activity GetById(int id)
        {
            return _context.Activities.SingleOrDefault(a => a.Id == id);
        }

        public Task<Activity> GetByIdAsync(int id)
        {
            return _context.Activities.SingleOrDefaultAsync(a => a.Id == id);
        }
    }
}
