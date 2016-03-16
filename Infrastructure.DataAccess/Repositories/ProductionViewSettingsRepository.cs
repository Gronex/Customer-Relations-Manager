using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Infrastructure.DataAccess.Extentions;

namespace Infrastructure.DataAccess.Repositories
{
    public class ProductionViewSettingsRepository : IProductionViewSettingsRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<ProductionViewSettings> _repo;

        public ProductionViewSettingsRepository(IApplicationContext context, IGenericRepository<ProductionViewSettings> repo)
        {
            _context = context;
            _repo = repo;
        }

        public IEnumerable<ProductionViewSettings> GetAll()
        {
            return _repo.Get();
        }

        public ProductionViewSettings GetById(int id)
        {
            return _context.ProductionViewSettings.SingleOrExcept(pvs => pvs.Id == id);
        }

        public ProductionViewSettings Create(ProductionViewSettings model, string userName)
        {
            var settings = new ProductionViewSettings
            {
                Name = model.Name,
                Private = model.Private,
                Weighted = model.Weighted,
                OwnerId = _context.Users.Single(u => u.UserName == userName).Id,
                Categories = new List<OpportunityCategory>(),
                Departments = new List<Department>(),
                Stages = new List<Stage>(),
                UserGroups = new List<UserGroup>(),
                Users = new List<User>()
            };

            return _repo.Insert(settings);
        }

        public ProductionViewSettings Update(int id, ProductionViewSettings model, string userName)
        {
            return _repo.Update(pvs =>
            {
                if (!HasRights(userName, pvs.OwnerId)) throw new NotAllowedException();

                pvs.Name = model.Name;
                pvs.Private = model.Private;
                pvs.Weighted = model.Weighted;
            }, id);
        }

        public void Delete(int id, string userName)
        {
            if(!HasRights(userName, _context.ProductionViewSettings.SingleOrDefault(pvs => pvs.Id == id)?.OwnerId))
                throw new NotAllowedException();
            _repo.DeleteByKey(id);
        }

        private bool HasRights(string userName, string userId = null)
        {
            var user = _context.Users.Single(u => u.UserName == userName);
            return user.Id == userId || user.Roles.Join(_context.Roles.Where(r => r.Name == nameof(UserRole.Super)),
                ur => ur.RoleId, r => r.Id,
                (ur, r) => r).Any();
        }
    }
}
