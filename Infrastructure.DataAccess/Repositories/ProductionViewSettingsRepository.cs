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

        public IEnumerable<ProductionViewSettings> GetAll(string userName)
        {
            var user = _context.Users.SingleOrExcept(u => u.UserName == userName).Id;
            var result = _context.Roles.Where(r => r.Users.Any(u => u.UserId == user)).Any(r => r.Name == nameof(UserRole.Super)) 
                ? _repo.Get() 
                : _repo.Get(pvs => !pvs.Private || pvs.OwnerId == user);
            return result;
        }

        public ProductionViewSettings GetById(int id)
        {
            return _context.ProductionViewSettings.SingleOrExcept(pvs => pvs.Id == id);
        }

        public ProductionViewSettings Create(ProductionViewSettings model, string userName)
        {
            var settings = Correct(model, userName);
            
            return _repo.Insert(settings);
        }

        public ProductionViewSettings Update(int id, ProductionViewSettings model, string userName)
        {
            return _repo.Update(pvs =>
            {
                if (!HasRights(userName, pvs.OwnerId)) throw new NotAllowedException();
                
                var settings = Correct(model, userName);
                
                pvs.Name = model.Name;
                pvs.Private = model.Private;
                pvs.Weighted = model.Weighted;

                pvs.Categories.ReplaceCollection(settings.Categories);
                pvs.Departments.ReplaceCollection(settings.Departments);
                pvs.Stages.ReplaceCollection(settings.Stages);
                pvs.UserGroups.ReplaceCollection(settings.UserGroups);
                pvs.Users.ReplaceCollection(settings.Users);
            }, true, id);
        }

        public void Delete(int id, string userName)
        {
            if(!HasRights(userName, _context.ProductionViewSettings.SingleOrDefault(pvs => pvs.Id == id)?.OwnerId))
                throw new NotAllowedException();

            var data = _context.ProductionViewSettings.SingleOrDefault(pvs => pvs.Id == id);
            if (data == null) return;

            // HACK: To avoid conflict with cascade delete, since it does not remove the references
            data.Departments.Clear();
            data.Categories.Clear();
            data.Stages.Clear();
            data.UserGroups.Clear();
            data.Users.Clear();
            _repo.DeleteByKey(id);
        }

        private bool HasRights(string userName, string userId = null)
        {
            var user = _context.Users.Single(u => u.UserName == userName);
            return user.Id == userId || user.Roles.Join(_context.Roles.Where(r => r.Name == nameof(UserRole.Super)),
                ur => ur.RoleId, r => r.Id,
                (ur, r) => r).Any();
        }

        public ProductionViewSettings Correct(ProductionViewSettings model, string userName = null)
        {
            var catIds = model.Categories?.Select(c => c.Id);
            var depIds = model.Departments?.Select(d => d.Id);
            var stageIds = model.Stages?.Select(s => s.Id);
            var grpIds = model.UserGroups?.Select(g => g.Id);
            var userEmails = model.Users?.Select(u => u.Email);
            return new ProductionViewSettings
            {
                Name = model.Name,
                Private = model.Private,
                Weighted = model.Weighted,
                OwnerId = userName == null ? null : _context.Users.Single(u => u.UserName == userName).Id,
                Categories = catIds != null ? _context.OpportunityCategories.Where(oc => catIds.Contains(oc.Id)).ToList() : new List<OpportunityCategory>(),
                Departments = depIds != null ?  _context.Departments.Where(od => depIds.Contains(od.Id)).ToList() : new List<Department>(),
                Stages = stageIds != null ? _context.Stages.Where(os => stageIds.Contains(os.Id)).ToList() : new List<Stage>(),
                UserGroups = grpIds != null ? _context.UserGroups.Where(ug =>grpIds.Contains(ug.Id)).ToList() : new List<UserGroup>(),
                Users = userEmails != null ? _context.Users.Where(us => userEmails.Contains(us.Email)).ToList() : new List<User>()
            };
        }
    }
}
