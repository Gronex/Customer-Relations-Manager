using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace UnitTests.Stubs
{
    public class ActivityRepoStub : IActivityRepository
    {
        private readonly IEnumerable<Activity> _activities = new List<Activity>()
            {
                new Activity {Id = 1, Name = "Do that one thing", Done = false },
                new Activity {Id = 2, Name = "Do that other thing", Done = true },
                new Activity {Id = 3, Name = "Do all the rest", Done = false },
            };

        public IQueryable<Activity> GetAll()
        {
            return _activities.AsQueryable();
        }

        public Activity GetById(int id)
        {
            return _activities.SingleOrDefault(a => a.Id == id);
        }

        public Task<Activity> GetByIdAsync(int id)
        {
            return Task.FromResult(_activities.SingleOrDefault(a => a.Id == id));
        }
    }
}
