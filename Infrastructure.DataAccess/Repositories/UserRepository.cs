using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Extentions;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Infrastructure.DataAccess.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IApplicationContext _context;

        public UserRepository(IApplicationContext context)
        {
            _context = context;
        }

        public PaginationEnvelope<Core.DomainServices.DTOs.UserRole> GetAll(string orderBy, int? page, int? pageSize)
        {
            var roles = _context.Roles.Select(r => new { r.Name, r.Id, r.Users });
            var tempUsers = _context.Users.Where(u => u.Active).SelectMany(u => u.Roles.Select(r => new { Role = r.RoleId, User = u }));

            var users = roles.Join(tempUsers, r => r.Id, u => u.Role, (r, u) => new
            {
                Role = r.Name,
                User = u.User
            })
                .GroupBy(ur => ur.User)
                //Selects the highest value of the roles the user has, resulting in the most rights
                .Select(g => g.OrderByDescending(ur =>
                    nameof(UserRole.Super).ToLower() == ur.Role
                        ? UserRole.Super
                        : nameof(UserRole.Executive).ToLower() == ur.Role
                            ? UserRole.Executive
                            : UserRole.Standard).FirstOrDefault())
                .OrderBy(orderBy);

            var userCount = users.Count();
            if (page.HasValue && pageSize.HasValue)
                users = users
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return new PaginationEnvelope<Core.DomainServices.DTOs.UserRole>
            {
                PageSize = pageSize ?? -1,
                PageNumber = page ?? -1,
                ItemCount = userCount,
                Data = users.ToList().Select(u => new Core.DomainServices.DTOs.UserRole {RoleName = (UserRole)Enum.Parse(typeof(UserRole), u.Role), User = u.User})
            };
        }

        public Core.DomainServices.DTOs.UserRole GetById(string id)
        {
            var user = _context.Users.SingleOrExcept(u => u.Active && u.Id == id);
            var roles = _context.Roles.Where(r => r.Users.Any(u => u.UserId == id));
            var role = roles.OrderByDescending(SelectRole()).FirstOrExcept();
            return new Core.DomainServices.DTOs.UserRole
            {
                User = user,
                RoleName = role.Name.Parse<UserRole>()
            };
        }

        private Expression<Func<IdentityRole, UserRole>> SelectRole()
        {
            return ur =>
                nameof(UserRole.Super).ToLower() == ur.Name
                    ? UserRole.Super
                    : nameof(UserRole.Executive).ToLower() == ur.Name
                        ? UserRole.Executive
                        : UserRole.Standard;
        }
    }
}
