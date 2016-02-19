using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Comments;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Microsoft.AspNet.Identity.EntityFramework;

namespace UnitTests.Stubs
{
    public class AppContextStub : IApplicationContext
    {
        public void Dispose()
        {
            // Do nothing
        }

        public DbSet<Opportunity> Opportunities { get; set; } = new TestDbSet<Opportunity>();
        public DbSet<UserGroup> UserGroups { get; set; } = new TestDbSet<UserGroup>();
        public DbSet<ProductionGoal> Goals { get; set; } = new TestDbSet<ProductionGoal>();
        public DbSet<OpportunityCategory> OpportunityCategories { get; set; } = new TestDbSet<OpportunityCategory>();
        public DbSet<Activity> Activities { get; set; } = new TestDbSet<Activity>();
        public DbSet<ActivityCategory> ActivityCategories { get; set; } = new TestDbSet<ActivityCategory>();
        public DbSet<Person> Persons { get; set; } = new TestDbSet<Person>();
        public DbSet<Company> Companies { get; set; } = new TestDbSet<Company>();
        public DbSet<Department> Departments { get; set; } = new TestDbSet<Department>();
        public DbSet<FileIndex> FileIndices { get; set; } = new TestDbSet<FileIndex>();
        public DbSet<Stage> Stages { get; set; } = new TestDbSet<Stage>();
        public DbSet<OpportunityComment> OpportunityComments { get; set; } = new TestDbSet<OpportunityComment>();
        public DbSet<ActivityComment> ActivityComments { get; set; } = new TestDbSet<ActivityComment>();
        public IDbSet<User> Users { get; set; } = new TestDbSet<User>();
        public IDbSet<IdentityRole> Roles { get; set; } = new TestDbSet<IdentityRole>();
        public DbSet<T> Set<T>() where T : class
        {
            var prop = typeof (IApplicationContext).GetProperties().SingleOrDefault(p => p.PropertyType == typeof (T));
            return prop?.GetValue(this) as DbSet<T>;
        }

        public DbEntityEntry<T> Entry<T>(T entity) where T : class
        {
            throw new NotImplementedException();
        }
    }
}
