using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainServices;

namespace Infrastructure.DataAccess.Activities
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly ApplicationContext _context;

        public ActivityRepository(ApplicationContext context)
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
