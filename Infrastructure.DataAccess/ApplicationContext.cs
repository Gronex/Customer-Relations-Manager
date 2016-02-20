using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
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

namespace Infrastructure.DataAccess
{
    public class ApplicationContext : IdentityDbContext<User>, IApplicationContext
    {
        public ApplicationContext(DbConnection connection) : base(connection, true)
        {
            
        }

        public ApplicationContext() : base("DefaultConnection")
        {
            
        }

        public static ApplicationContext Create()
        {
            return new ApplicationContext();
        }
        
        public DbSet<Opportunity> Opportunities { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<ProductionGoal> Goals { get; set; }
        public DbSet<OpportunityCategory> OpportunityCategories { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ActivityCategory> ActivityCategories { get; set; }
        public DbSet<Person> Persons { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<FileIndex> FileIndices { get; set; }
        public DbSet<Stage> Stages { get; set; }
        public DbSet<OpportunityComment> OpportunityComments { get; set; }
        public DbSet<ActivityComment> ActivityComments { get; set; }
        
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // The DateTime type in .NET has the same range and precision as datetime2 in SQL Server.
            // Configure DateTime type to use SQL server datetime2 instead.
            // TODO: Cannot be there when testing 
            modelBuilder.Properties<DateTime>().Configure(c => c.HasColumnType("datetime2"));

            // Multi column keys
            modelBuilder.Entity<UserGroupUser>()
                .HasKey(ugu => new { ugu.UserId, ugu.UserGroupId });

            modelBuilder.Entity<UserGroupOpportunity>()
                .HasKey(ugo => new { ugo.OpportunityId, ugo.UserGroupId });


            // Consider setting max length on string properties.
            // http://dba.stackexchange.com/questions/48408/ef-code-first-uses-nvarcharmax-for-all-strings-will-this-hurt-query-performan
            // Default max length
            modelBuilder.Properties<string>().Configure(c => c.HasMaxLength(100));

            // Set max length where it makes sense.
            modelBuilder.Entity<Comment>().Property(c => c.Text).HasMaxLength(500);
            modelBuilder.Entity<FileIndex>().Property(fp => fp.FilePath).HasMaxLength(256);
            modelBuilder.Entity<Opportunity>().Property(o => o.Description).HasMaxLength(500);

            modelBuilder.Entity<Opportunity>().HasRequired(o => o.Owner).WithMany(u => u.Opportunities).WillCascadeOnDelete(false);
            
            base.OnModelCreating(modelBuilder);
        }

        public void SetModified<T>(T entity)
            where T : class
        {
            Entry(entity).State = EntityState.Modified;
        }
    }
}
