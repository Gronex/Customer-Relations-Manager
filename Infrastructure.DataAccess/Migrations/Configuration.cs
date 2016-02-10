using System.CodeDom;
using System.Collections.Generic;
using Core.DomainModels.Users;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Infrastructure.DataAccess.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<Infrastructure.DataAccess.ApplicationContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(Infrastructure.DataAccess.ApplicationContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var superAdmin = new User
            {
                UserName = "test@test.com",
                Email = "test@test.com",
                FirstName = "Test",
                LastName = "User"
            };

            var roles = new List<UserRole>() {UserRole.Super, UserRole.Executive, UserRole.Standard};


            roles.ForEach(r => InsertRole(context, r));

            InsertUser(context, superAdmin, roles);

        }

        private static void InsertRole(ApplicationContext context, UserRole role)
        {
            if (context.Roles.Any(r => r.Name == role.ToString())) return;

            var roleStore = new RoleStore<IdentityRole>(context);
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var identityRole = new IdentityRole(role.ToString());

            roleManager.Create(identityRole);
        }

        private static void InsertUser(ApplicationContext context, User user, IEnumerable<UserRole> roles)
        {
            if (context.Users.Any(u => u.UserName == user.Email)) return;

            var userStore = new UserStore<User>(context);
            var userManager = new UserManager<User>(userStore);
            userManager.Create(user, "Password1");

            var res = userManager.AddToRoles(user.Id, roles.Select(r => r.ToString()).ToArray());
            if (!res.Succeeded)
            {
                throw new Exception(string.Join(", ", res.Errors));
            }
        }
    }
}
