using System.Linq;
using Core.DomainModels.UserGroups;
using Core.DomainServices;
using Core.DomainServices.Repositories;

namespace Infrastructure.DataAccess.Repositories
{
    public class UserGroupRepository : IUserGroupRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<UserGroup> _repo;

        public UserGroupRepository(IApplicationContext context, IGenericRepository<UserGroup> repo)
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
