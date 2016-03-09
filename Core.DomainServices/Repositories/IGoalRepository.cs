using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Users;

namespace Core.DomainServices.Repositories
{
    public interface IGoalRepository
    {
        IEnumerable<ProductionGoal> GetAll(string userId);
        ProductionGoal GetById(string userId, int id);
        ProductionGoal Create(string userId, ProductionGoal model);
        ProductionGoal Update(string userId, int id, ProductionGoal model);
        void Delete(string userId, int id);
    }
}
