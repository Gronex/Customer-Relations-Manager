using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Customers;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Extentions;

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
                a.DueDate = activity.DueDate.ToUniversalTime().Date;
                a.DueTimeStart = activity.DueTimeStart?.ToUniversalTime();
                if (activity.DueTimeEnd.HasValue && !activity.DueTimeStart.HasValue)
                    a.DueTimeEnd = null;
                else
                    a.DueTimeEnd = activity.DueTimeEnd?.ToUniversalTime();

                if (a.PrimaryResponsible.Email != activity.PrimaryResponsible.Email)
                {
                    a.PrimaryResponsibleId = _context.Users
                        .SingleOrExcept(u => u.Email == activity.PrimaryResponsible.Email).Id;
                }

                var newResponsibles = activity.SecondaryResponsibles.Select(c => c.Email);
                var updatedResponsibles = _context.Users
                    .Where(p => newResponsibles.Any(c => p.Email == c));
                Update(a.SecondaryResponsibles, updatedResponsibles);


                if (a.CompanyId != activity.CompanyId)
                {
                    // if we get null that is fine, then we just remove the company
                    a.Company = _context.Companies.SingleOrDefault(c => c.Id == activity.CompanyId);
                    if (a.Company == null) a.CompanyId = null;
                }

                if (a.CompanyId.HasValue)
                {
                    a.PrimaryContact = _context.Persons
                        .Where(p => p.CompanyId == a.CompanyId && p.CompanyId.HasValue)
                        .SingleOrDefault(p => p.Id == activity.PrimaryContactId);

                    var newContacts = activity.SecondaryContacts.Select(c => c.Id);
                    var updatedContacts = _context.Persons
                        .Where(p => p.CompanyId == a.CompanyId)
                        .Where(p => newContacts.Any(c => p.Id == c));
                    Update(a.SecondaryContacts, updatedContacts);
                }
                else
                {
                    a.PrimaryContact = null;
                    a.PrimaryContactId = null;
                    foreach (var contact in a.SecondaryContacts)
                    {
                        a.SecondaryContacts.Remove(contact);
                    }
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
            activity.DueDate = activity.DueDate.ToUniversalTime().Date;
            activity.DueTimeStart = activity.DueTimeStart?.ToUniversalTime();
            if (activity.DueTimeEnd.HasValue && !activity.DueTimeStart.HasValue)
                activity.DueTimeEnd = null;
            else
                activity.DueTimeEnd = activity.DueTimeEnd?.ToUniversalTime();

            activity.PrimaryResponsible = _context.Users.SingleOrExcept(u => u.Email == activity.PrimaryResponsible.Email);
            activity.Category = _context.ActivityCategories.SingleOrExcept(c => c.Name == activity.Category.Name);

            // if we get null that is fine, then we just don't set the company
            activity.Company = _context.Companies.SingleOrDefault(c => c.Id == activity.CompanyId);
            activity.PrimaryContact = _context.Persons
                .Where(p => p.CompanyId == activity.CompanyId && p.CompanyId.HasValue)
                .SingleOrDefault(p => p.Id == activity.PrimaryContactId);

            // Update secondary responsibles
            var newResponsibles = activity.SecondaryResponsibles.Select(c => c.Email);
            var updatedResponsibles = _context.Users
                .Where(p => newResponsibles.Any(c => p.Email == c));
            activity.SecondaryResponsibles = new List<User>();
            Update(activity.SecondaryResponsibles, updatedResponsibles);

            // Update secondary contacts
            var newContacts = activity.SecondaryContacts.Select(c => c.Id);
            var updatedContacts = _context.Persons
                .Where(p => p.CompanyId == activity.CompanyId)
                .Where(p => newContacts.Any(c => p.Id == c));
            activity.SecondaryContacts = new List<Person>();
            Update(activity.SecondaryContacts, updatedContacts);

            return _repo.Insert(activity);
        }

        public void DeleteByKey(int id)
        {
            _repo.DeleteByKey(id);
        }

        private static void Update<T>(ICollection<T> inDb, IQueryable<T> updated)
        {
            var toRemove = inDb.Except(updated).ToList();
            var toAdd = updated.ToList().Except(inDb).ToList();

            toRemove.ForEach(p => inDb.Remove(p));
            toAdd.ForEach(inDb.Add);
        }
    }
}
