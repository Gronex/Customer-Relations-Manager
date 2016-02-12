using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Core.DomainServices.Repositories;

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
            var dbGroup = _context.UserGroups.SingleOrDefault(ug => ug.Id == id);
            if (dbGroup == null) return null;
            dbGroup.Name = group.Name;
            
            _context.Entry(dbGroup).Property(g => g.Name).IsModified = true;

            return dbGroup;
        }

        public void Delete(int id)
        {
            var dbGroup = GetById(id);
            if (dbGroup == null) return;

            if (_context.Entry(dbGroup).State == EntityState.Detached)
                _context.UserGroups.Attach(dbGroup);
            _context.UserGroups.Remove(dbGroup);
        }
    }
}
