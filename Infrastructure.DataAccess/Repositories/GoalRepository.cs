using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class GoalRepository : IGoalRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<ProductionGoal> _repo;

        public GoalRepository(IApplicationContext context, IGenericRepository<ProductionGoal> repo)
        {
            _context = context;
            _repo = repo;
        }

        public IEnumerable<ProductionGoal> GetAll(string userId)
        {
            return _context.Users.Any(u => u.Id == userId) ? _repo.Get(g => g.UserId == userId) : null;
        }

        public ProductionGoal GetById(string userId, int id)
        {
            return _context.Goals.SingleOrDefault(g => g.Id == id && g.UserId == userId);
        }

        public ProductionGoal Create(string userId, ProductionGoal model)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
                return null;
            model.User = user;
            return _context.Goals.Add(model);
        }

        public ProductionGoal Update(string userId, int id, ProductionGoal model)
        {
            return _repo.UpdateBy(g =>
            {
                g.Goal = model.Goal;
                g.Year = model.Year;
                g.Month = model.Month;
            }, g => g.Id == id && g.UserId == userId);
        }

        public void Delete(string userId, int id)
        {
            _repo.DeleteBy(g => g.Id == id && g.UserId == userId);
        }
    }
}
