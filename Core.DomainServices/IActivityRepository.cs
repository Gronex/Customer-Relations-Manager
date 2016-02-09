using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;

namespace Core.DomainServices
{
    public interface IActivityRepository
    {
        IQueryable<Activity> GetAll();
        Activity GetById(int id);
        Task<Activity> GetByIdAsync(int id);
    }
}
