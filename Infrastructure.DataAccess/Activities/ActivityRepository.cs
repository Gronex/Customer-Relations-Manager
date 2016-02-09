using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainServices;

namespace Infrastructure.DataAccess.Activities
{
    public class ActivityRepository : IActivityRepository
    {
        public ActivityRepository()
        {
            
        }

        public IEnumerable<Activity> GetAll()
        {
            throw new NotImplementedException();
        }

        public Activity GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
