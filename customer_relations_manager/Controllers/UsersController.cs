using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using AutoMapper;
using customer_relations_manager.ViewModels;
using customer_relations_manager.ViewModels.User;
using Core.DomainModels.UserGroups;
using Core.DomainModels.Users;
using Core.DomainServices;
using Core.DomainServices.Repositories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace customer_relations_manager.Controllers
{
    [Authorize(Roles = nameof(UserRole.Super))]
    public class UsersController : CrmApiController
    {
        // Standard asp.net classes to manage users.
        private readonly ApplicationUserManager _userManager;
        private readonly IUnitOfWork _uow;
        private readonly IMapper _mapper;

        public UsersController(ApplicationUserManager userManager, 
            IUnitOfWork uow,
            IMapper mapper)
        {
            _userManager = userManager;
            _uow = uow;
            _mapper = mapper;
        }

        // GET: api/users
        [HttpGet]
        public IHttpActionResult GetAll(int page = 1, int pageSize = 10)
        {
            CorrectPageInfo(ref page, ref pageSize);
            var users = _userManager.Users.ToList();

            var userModels = users
                .Where(u => u.Active).OrderBy(pe => pe.Id)
                .ThenBy(u => u.LastName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(u =>
                {
                    var roles = _userManager.GetRoles(u.Id);
                    //Selects the highest value of the roles the user has, resulting in the most rights
                    var role = roles.Select(r => Enum.Parse(typeof (UserRole), r)).OfType<UserRole>().Max();
                    return _mapper.Map<User, UserOverviewViewModel>(u,
                        opts => opts.AfterMap((_, res) => res.Role = role));
                });
            
            return Ok(new PaginationEnvelope<UserOverviewViewModel>
            {
                Data = userModels,
                ItemCount = users.Count(u => u.Active),
                PageSize = pageSize,
                PageNumber = page
            });
        }

        // GET: api/users/{id}
        [HttpGet]
        public async Task<IHttpActionResult> Get(string id)
        {
            var user = _userManager.FindById(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(id);
            //Selects the highest value of the roles the user has, resulting in the most rights
            var role = roles.Select(r => Enum.Parse(typeof(UserRole), r)).OfType<UserRole>().Max();

            var userModel = _mapper.Map<User, UserViewModel>(user,
                opts => opts.AfterMap((_, res) => res.Role = role));

            return Ok(userModel);
        }

        // POST: api/users
        [HttpPost]
        public async Task<IHttpActionResult> Post(UserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            model.Email = model.Email.ToLower();

            // Find existing, or create new
            var user = await _userManager.FindByEmailAsync(model.Email) 
                ?? new User { UserName = model.Email, Email = model.Email, Active = true};
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            UpdateUserGroups(model, user);

            if (user.Active)
            {
                var result = await _userManager.CreateAsync(user, "Password1");
                    //TODO: Dont set default, but let users choose after email link
                if (!result.Succeeded)
                    return BadRequest(string.Join(", ", result.Errors));
            }
            user.Active = true;
            
            // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
            // Send an email with this link
            // var callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");

            var roles = CalculateNewRoles(model.Role).Select(r => r.ToString()).ToList();
            roles.ForEach(role => _userManager.AddToRole(user.Id, role));
            _uow.Save();

            model.Id = user.Id;
            return Created(user.Id, model);
        }

        // PUT: api/users/{id}
        [HttpPut]
        public async Task<IHttpActionResult> Put(string id, UserViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            UpdateUserGroups(model, user);

            var userRoles = _userManager.GetRoles(id)
                .Select(r => Enum.Parse(typeof (UserRole), r))
                .OfType<UserRole>()
                .ToList();

            var roles = CalculateNewRoles(model.Role).ToList();
            var toRemove = userRoles.Except(roles).Select(r => r.ToString()).ToArray();
            _userManager.RemoveFromRoles(id, toRemove);

            var toAdd = roles.Except(userRoles).Select(r => r.ToString()).ToArray();
            _userManager.AddToRoles(id, toAdd);

            _uow.Save();
            return Ok();
        }

        // DELETE: api/users/{id}
        [HttpDelete]
        public void RemoveUser(string id)
        {
            var user = _userManager.FindById(id);
            if (user == null) return;

            user.Active = false;
            user.EndDate = DateTime.UtcNow.Date;

            var roles = _userManager.GetRoles(id).ToArray();
            _userManager.RemoveFromRoles(id, roles);

            _uow.Save();
        }

        private static void UpdateUserGroups(UserViewModel model, User user)
        {

            var newGroupIds = model.Groups.Select(g => g.Id).ToList();

            user.Groups
                .Where(g => !newGroupIds.Contains(g.UserGroupId))
                .ToList()
                .ForEach(grp => user.Groups.Remove(grp));
            var groupsToBeAddedTo = newGroupIds.Except(user.Groups.Select(g => g.UserGroupId));
            
            foreach (var grp in groupsToBeAddedTo)
            {
                user.Groups.Add(new UserGroupUser { User = user, UserGroupId = grp });
            }
        }

        public IEnumerable<UserRole> CalculateNewRoles(UserRole targetRole)
        {
            var roles = new List<UserRole>();

            // Starting at the role we want, we run all the way to the lowest role in the system
            // To give all the same rights as anyone under one self
            for (var r = targetRole; r >= 0; r--)
            {
                roles.Add(r);
            }
            return roles;
        }
    }
}
