using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using customer_relations_manager.ViewModels.User;
using Core.DomainModels.Users;
using Microsoft.AspNet.Identity;

namespace customer_relations_manager.Controllers
{
    [Authorize]
    public class AccountController : CrmApiController
    {
        private readonly ApplicationUserManager _userManager;

        public AccountController(ApplicationUserManager userManager)
        {
            _userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("api/account/conformemail")]
        public async Task<IHttpActionResult> ConfirmEmail([FromBody] UserCredentialsViewModel model, [FromUri] string code)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest();

            var user = await _userManager.FindByNameAsync(model.UserName);
            if (user == null) return BadRequest();

            if (user.EmailConfirmed && user.PasswordHash != null)
                return BadRequest();


            IdentityResult emailResult;
            IdentityResult passwordResult;
            try
            {
                emailResult = await _userManager.ConfirmEmailAsync(user.Id, code);
                if(emailResult.Succeeded)
                    passwordResult = await _userManager.AddPasswordAsync(user.Id, model.Password);
                else
                    passwordResult = new IdentityResult();
            }
            catch (InvalidOperationException ioe)
            {
                //Meant not to inform of the actual error to not tell
                //a malicious user if the userId exists
                return BadRequest(ioe.Message);
            }

            if (emailResult.Succeeded && passwordResult.Succeeded)
                return Ok();

            return BadRequest(string.Join("\n", emailResult.Errors.Union(passwordResult.Errors)));
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("api/account/forgotpassword")]
        public async Task<IHttpActionResult> ForgotPassword([FromUri]string userName)
        {
            if (string.IsNullOrWhiteSpace(userName)) return BadRequest();

            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || !await _userManager.IsEmailConfirmedAsync(user.Id))
                return Ok();

            var code = await _userManager.GeneratePasswordResetTokenAsync(user.Id);
            var callbackUrl = $"{GetHostUri()}/#/account/resetpassword?userName={user.UserName}&code={HttpContext.Current.Server.UrlEncode(code)}";
            await _userManager.SendEmailAsync(user.Id, "Reset Password", callbackUrl);

            return Ok();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("api/account/resetpassword")]
        public async Task<IHttpActionResult> ResetPassword([FromBody] UserCredentialsViewModel model, string code)
        {
            if (model == null || !ModelState.IsValid) return BadRequest(ModelState);
            if (string.IsNullOrWhiteSpace(code)) return BadRequest();

            var user = await _userManager.FindByNameAsync(model.UserName);

            if (user == null) return Ok();

            var result = await _userManager.ResetPasswordAsync(user.Id, code, model.Password);
            if (result.Succeeded) return Ok();

            return BadRequest(string.Join("\n", result.Errors));
        }

    }
}
