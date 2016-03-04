using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Customers;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class ActivityRepository : IActivityRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<Activity> _repo;

        public ActivityRepository(IApplicationContext context, IGenericRepository<Activity> repo)
        {
            _context = context;
            _repo = repo;
        }

        public PaginationEnvelope<Activity> GetAll(Func<IQueryable<Activity>, IOrderedQueryable<Activity>> orderBy, int? page = null, int? pageSize = null)
        {
            return _repo.GetPaged(orderBy, page, pageSize);
        }

        public IEnumerable<Activity> GetAll()
        {
            return _repo.Get();
        }

        public Activity GetById(int id)
        {
            return _context.Activities.SingleOrDefault(a => a.Id == id);
        }

        public Activity Update(int id, Activity activity)
        {
            return _repo.Update(a =>
            {
                a.Name = activity.Name;
                a.Done = activity.Done;
                a.DueDate = activity.DueDate;
                a.DueTime = activity.DueTime;

                if (a.Responsible.Email != activity.Responsible.Email)
                {
                    var dbResp = _context.Users.SingleOrDefault(u => u.Email == activity.Responsible.Email);
                    if (dbResp != null) a.ResponsibleId = dbResp.Id;
                }

                // people are updated on their own

                if (a.CompanyId != activity.CompanyId)
                {
                    // if we get null that is fine, then we just remove the company
                    a.Company = _context.Companies.SingleOrDefault(c => c.Id == activity.CompanyId);
                    if (a.Company == null) a.CompanyId = null;
                }

                if (!a.CompanyId.HasValue)
                {
                    foreach (var contact in a.Contacts)
                    {
                        a.Contacts.Remove(contact);
                    }
                }
                else
                {
                    var newContacts = activity.Contacts.Select(c => c.Id);
                    var updatedContacts = _context.Persons
                        .Where(p => p.CompanyId == a.CompanyId)
                        .Where(p => newContacts.Any(c => p.Id == c));
                    
                    UpdateContacts(a.Contacts, updatedContacts);
                }


                if (a.Category.Name != activity.Category.Name)
                {
                    var category = _context.ActivityCategories.SingleOrDefault(c => c.Name == activity.Category.Name);
                    if (category != null) a.CategoryId = category.Id;
                }
                
            }, id);
        }

        public Activity Create(Activity activity)
        {
            var dbResp = _context.Users.SingleOrDefault(u => u.Email == activity.Responsible.Email);
            if (dbResp == null) return null;
            activity.Responsible = dbResp;
            // if we get null that is fine, then we just don't set the company
            activity.Company = _context.Companies.SingleOrDefault(c => c.Id == activity.CompanyId);

            var category = _context.ActivityCategories.SingleOrDefault(c => c.Name == activity.Category.Name);
            if (category == null) return null;
            activity.Category = category;

            var newContacts = activity.Contacts.Select(c => c.Id);
            var updatedContacts = _context.Persons
                .Where(p => p.CompanyId == activity.CompanyId)
                .Where(p => newContacts.Any(c => p.Id == c));

            activity.Contacts = new List<Person>();

            UpdateContacts(activity.Contacts, updatedContacts);

            return _repo.Insert(activity);
        }

        public void DeleteByKey(int id)
        {
            _repo.DeleteByKey(id);
        }

        private static void UpdateContacts(ICollection<Person> inDb, IQueryable<Person> updated)
        {
            var toRemove = inDb.Except(updated).ToList();
            var toAdd = updated.ToList().Except(inDb).ToList();
            
            toRemove.ForEach(p => inDb.Remove(p));
            toAdd.ForEach(inDb.Add);
        }
    }
}
