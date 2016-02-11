using System.Linq;
using Core.DomainModels.UserGroups;

namespace Core.DomainServices
{
    public interface IUserGroupRepository
    {
        IQueryable<UserGroup> GetAll();

        UserGroup GetById(int id);

        UserGroup Create(UserGroup group);

        UserGroup Update(int id, UserGroup group);

        void Delete(int id);
    }
}
