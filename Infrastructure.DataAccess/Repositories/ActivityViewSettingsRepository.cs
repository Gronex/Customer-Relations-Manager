using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainModels.ViewSettings;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Infrastructure.DataAccess.Exceptions;
using Infrastructure.DataAccess.Extentions;

namespace Infrastructure.DataAccess.Repositories
{
    public class ActivityViewSettingsRepository : IActivityViewSettingsRepository
    {
        private readonly IApplicationContext _context;
        private readonly IGenericRepository<ActivityViewSettings> _repo;

        public ActivityViewSettingsRepository(IApplicationContext context, IGenericRepository<ActivityViewSettings> repo)
        {
            _context = context;
            _repo = repo;
        }

        public IEnumerable<ActivityViewSettings> GetAll(string userName)
        {
            var user = _context.Users.SingleOrExcept(u => u.UserName == userName).Id;
            var result = _context.Roles
                .Where(r => r.Users.Any(u => u.UserId == user))
                .Any(r => r.Name == nameof(UserRole.Super))
                ? _repo.Get()
                : _repo.Get(avs => !avs.Private || avs.OwnerId == user);
            return result;
        }

        public ActivityViewSettings GetById(int id)
        {
            return _context.ActivityViewSettings.SingleOrExcept(avs => avs.Id == id);
        }

        public ActivityViewSettings Create(ActivityViewSettings model, string userName)
        {
            var settings = Correct(model, userName);

            return _repo.Insert(settings);
        }

        public ActivityViewSettings Update(int id, ActivityViewSettings model, string userName)
        {
            return _repo.Update(pvs =>
            {
                if (!HasRights(userName, pvs.OwnerId)) throw new NotAllowedException();

                var settings = Correct(model, userName);

                pvs.Name = model.Name;
                pvs.Private = model.Private;

                pvs.UserGroups.ReplaceCollection(settings.UserGroups);
                pvs.Users.ReplaceCollection(settings.Users);
            }, true, id);
        }

        public void Delete(int id, string userName)
        {
            if (!HasRights(userName, _context.ActivityViewSettings.SingleOrDefault(avs => avs.Id == id)?.OwnerId))
                throw new NotAllowedException();

            var data = _context.ActivityViewSettings.SingleOrDefault(pvs => pvs.Id == id);
            if (data == null) return;

            // HACK: To avoid conflict with cascade delete, since it does not remove the references
            data.UserGroups.Clear();
            data.Users.Clear();
            _repo.DeleteByKey(id);
        }

        private bool HasRights(string userName, string userId = null)
        {
            var user = _context.Users.Single(u => u.UserName == userName);
            return user.Id == userId || user.Roles
                .Join(_context.Roles.Where(r => r.Name == nameof(UserRole.Super)),
                    ur => ur.RoleId, r => r.Id,
                    (ur, r) => r)
                .Any();
        }

        public ActivityViewSettings Correct(ActivityViewSettings model, string userName = null)
        {
            var grpIds = model.UserGroups?.Select(g => g.Id);
            var userEmails = model.Users?.Select(u => u.Email);
            return new ActivityViewSettings
            {
                Name = model.Name,
                Private = model.Private,
                OwnerId = userName == null ? null : _context.Users.Single(u => u.UserName == userName).Id,
                UserGroups =
                    grpIds != null
                        ? _context.UserGroups.Where(ug => grpIds.Contains(ug.Id)).ToList()
                        : new List<UserGroup>(),
                Users =
                    userEmails != null
                        ? _context.Users.Where(us => userEmails.Contains(us.Email)).ToList()
                        : new List<User>()
            };
        }
    }
}
