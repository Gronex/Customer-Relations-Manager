using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices;

namespace Infrastructure.DataAccess
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly ApplicationContext _context;

        public UserGroupRepository(ApplicationContext context)
        {
            _context = context;
        }

        public IQueryable<UserGroup> GetAll()
        {
            return _context.UserGroups;
        }

        public UserGroup GetById(int id)
        {
            return _context.UserGroups.SingleOrDefault(ug => ug.Id == id);
        }

        public UserGroup Create(UserGroup group)
        {
            var dbGroup = _context.UserGroups.Add(group);

            return dbGroup;
        }

        public UserGroup Update(int id, UserGroup group)
        {
            if (!_context.UserGroups.Any(ug => ug.Id == id)) return null;
            group.Id = id;
            var dbGroup = _context.UserGroups.Attach(group);
            _context.Entry(group).State = EntityState.Modified;

            return dbGroup;
        }

        public void Delete(int id)
        {
            var dbGroup = GetById(id);

            if (_context.Entry(dbGroup).State == EntityState.Detached)
                _context.UserGroups.Attach(dbGroup);
            _context.UserGroups.Remove(dbGroup);
        }
    }
}
