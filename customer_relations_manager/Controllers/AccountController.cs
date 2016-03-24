using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Core.DomainModels.Users;
using Microsoft.AspNet.Identity;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class AccountController : ApiController
    {
        private readonly ApplicationUserManager _userManager;

        public AccountController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IHttpActionResult> ConfirmEmail([FromUri] string userId, [FromUri] string code)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
                return BadRequest();
            IdentityResult result;
            try
            {
                result = await _userManager.ConfirmEmailAsync(userId, code);
            }
            catch (InvalidOperationException ioe)
            {
                //Meant not to inform of the actual error to not tell
                //a malicious user if the userId exists
                return BadRequest(ioe.Message);
            }

            if (result.Succeeded)
                return Ok();

            return BadRequest(string.Join("\n", result.Errors));
        }

    }
}
