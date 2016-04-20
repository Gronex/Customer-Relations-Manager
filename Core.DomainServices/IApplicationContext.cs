using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using Core.DomainModels.Activities;
using Core.DomainModels.Authorization;
using Core.DomainModels.Comments;
using Core.DomainModels.Customers;
using Core.DomainModels.Opportunity;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainModels.ViewSettings;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Core.DomainServices
{
    public interface IApplicationContext : IDisposable
    {
        DbSet<Opportunity> Opportunities { get; set; }
        DbSet<UserGroup> UserGroups { get; set; }
        DbSet<ProductionGoal> Goals { get; set; }
        DbSet<OpportunityCategory> OpportunityCategories { get; set; }
        DbSet<Activity> Activities { get; set; }
        DbSet<ActivityCategory> ActivityCategories { get; set; }
        DbSet<Person> Persons { get; set; }
        DbSet<Company> Companies { get; set; }
        DbSet<Department> Departments { get; set; }
        DbSet<FileIndex> FileIndices { get; set; }
        DbSet<Stage> Stages { get; set; }
        DbSet<OpportunityComment> OpportunityComments { get; set; }
        DbSet<ActivityComment> ActivityComments { get; set; }
        DbSet<ProductionViewSettings> ProductionViewSettings { get; set; }
        DbSet<ActivityViewSettings> ActivityViewSettings { get; set; }
        DbSet<RefreshToken> RefreshTokens { get; set; }
        DbSet<Client> Clients { get; set; }

        // Identity stuff
        IDbSet<User> Users { get; set; }
        IDbSet<IdentityRole> Roles { get; set; }

        DbSet<T> Set<T>() where T : class;

        void SetState<T>(T entity, EntityState state)
            where T : class;

        int SaveChanges();
        Task<int> SaveChangesAsync();
    }
}
