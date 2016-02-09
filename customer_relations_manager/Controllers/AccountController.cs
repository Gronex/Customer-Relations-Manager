using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.UI.WebControls;
using customer_relations_manager.ViewModels.Authentication;
using Core.DomainModels.Users;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace customer_relations_manager.Controllers
{
    public class AccountController : ApiController
    {
        // Standard asp.net classes to manage users.
        private readonly ApplicationUserManager _userManager;
        private readonly IAuthenticationManager _authenticationManager;

        public AccountController(ApplicationUserManager userManager, IAuthenticationManager authenticationManager)
        {
            _userManager = userManager;
            _authenticationManager = authenticationManager;
        }

        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [Route("api/account/register")]
        public async Task<IHttpActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return BadRequest("Invalid model");

            model.Email = model.Email.ToLower();
            var user = new User { UserName = model.Email, Email = model.Email, FirstName = model.FirstName, LastName = model.LastName};
            
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // Comment the following line to prevent log in until the user is confirmed.
                //await _signInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                //var callbackUrl = await SendEmailConfirmationTokenAsync(user.Id, "Confirm your account");
                    
                return Ok();
                //return RedirectToAction("Index", "Home");
            }
            return BadRequest(string.Join(", ", result.Errors));

            // If we got this far, something failed, redisplay form
        }
    }
}
