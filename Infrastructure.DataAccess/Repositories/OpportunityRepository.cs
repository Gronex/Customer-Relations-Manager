using System.Collections.Generic;
using System.Linq;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Microsoft.AspNet.Identity;

namespace Infrastructure.DataAccess.Repositories
{
    public class OpportunityRepository : IOpportunityRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<Opportunity> _repo;
        private readonly UserManager<User> _userManager;

        public OpportunityRepository(IApplicationContext context, IGenericRepository<Opportunity> repo, UserManager<User> userManager)
        {
            _context = context;
            _repo = repo;
            _userManager = userManager;
        }

        public IEnumerable<Opportunity> GetAll()
        {
            return _repo.Get();
        }

        public Opportunity GetById(int id)
        {
            return _repo.GetByKey(id);
        }

        public Opportunity Create(Opportunity model, string userName)
        {
            // If the owner os not set, then use the current user
            model.Owner = model.Owner == null ? _context.Users.SingleOrDefault(u => u.UserName == userName) 
                : _context.Users.SingleOrDefault(u => u.Id == model.Owner.Id);
            var stage =_context.Stages.SingleOrDefault(s => s.Id == model.Stage.Id);
            var category = _context.OpportunityCategories.SingleOrDefault(c => c.Id == model.Category.Id);
            var department = _context.Departments.SingleOrDefault(d => d.Id == model.Department.Id);

            if (model.Owner == null || stage == null || category == null)  return null;

            // If the id is 0 we want to create a new company
            if (model.Company.Id > 0)
            {
                model.Company = _context.Companies.SingleOrDefault(c => c.Id == model.Company.Id);
                if (model.Company == null) return null;
            }
            
            model.Department = department;
            model.Category = category;
            model.Stage = stage;


            // Copy groups of the owner to the opportunity
            var groups = model.Owner.Groups.Select(g => g.UserGroup);
            foreach (var group in groups)
            {
                model.UserGroups.Add(new UserGroupOpportunity
                {
                    UserGroup = group,
                    Opportunity = model
                });
            }

            return _repo.Insert(model);
        }

        public Opportunity Update(int id, Opportunity model)
        {
            return _repo.Update(o =>
            {
                o.Name = model.Name;
                o.Description = model.Description;
                o.Amount = model.Amount;
                o.Department = o.Department;
                o.EndDate = model.EndDate;
                o.StartDate = model.StartDate;
                o.ExpectedClose = model.ExpectedClose;
                o.HourlyPrice = model.HourlyPrice;
                o.Percentage = model.Percentage;

                if (model.Company.Id != o.CompanyId)
                {
                    o.Company = model.Company.Id > 0 
                        ? _context.Companies.SingleOrDefault(c => c.Id == model.Company.Id) 
                        : _context.Companies.Add(model.Company);
                }
                
                if (model.Owner.Id != o.OwnerId)
                    o.Owner = _context.Users.SingleOrDefault(u => u.Id == model.Owner.Id);
                if (model.Stage.Id != o.StageId)
                    o.Stage = _context.Stages.SingleOrDefault(s => s.Id == model.Stage.Id);
                if (model.Category.Id != o.CategoryId)
                    o.Category = _context.OpportunityCategories.SingleOrDefault(c => c.Id == model.Category.Id);
            }, id);
        }

        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
        }
    }
}
