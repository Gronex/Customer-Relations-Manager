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
        private readonly IGenericRepository<UserGroup> _repo;

        public UserGroupRepository(ApplicationContext context, IGenericRepository<UserGroup> repo)
        {
            _context = context;
            _repo = repo;
        }

        public IQueryable<UserGroup> GetAll()
        {
            return _context.UserGroups;
        }

        public UserGroup GetById(int id)
        {
            return _repo.GetByKey(id);
        }

        public UserGroup Create(UserGroup group)
        {
            return _context.UserGroups.Add(group);
        }

        public UserGroup Update(int id, UserGroup group)
        {
            return _repo.Update(ug =>
            {
                ug.Name = group.Name;
            }, id);
        }

        public void Delete(int id)
        {
            _repo.DeleteByKey(id);
        }
    }
}
