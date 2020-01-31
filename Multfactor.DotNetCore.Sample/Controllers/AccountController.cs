using Microsoft.AspNetCore.Mvc;
using Multfactor.DotNetCore.Sample.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace Multfactor.DotNetCore.Sample.Controllers
{
    public class AccountController : Controller
    {
        private IdentityService _identityService;
        private MultifactorService _multifactorService;

        public AccountController(IdentityService identityService, MultifactorService multifactorService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _multifactorService = multifactorService ?? throw new ArgumentNullException(nameof(multifactorService));
        }

        [HttpGet("/account/login")]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("/account/login")]
        public async Task<IActionResult> Login([Required]string login, [Required]string password)
        {
            if (ModelState.IsValid)
            {
                var isValidUser = _identityService.ValidateUser(login, password, out string role);

                if (isValidUser)
                {
                    var claims = new Dictionary<string, string> //add role to token claims
                    {
                        { "Role", role }
                    };

                    var url = await _multifactorService.GetAccessPage(login, claims);
                    return RedirectPermanent(url);
                }
            }

            return View();
        }

        [HttpGet("/account/logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return Redirect("/");
        }

        [HttpPost("/account/mfa")]
        public IActionResult MultifactorCallback(string accessToken)
        {
            //сохраняем токен доступа в куки и переводим пользователя в авторизованную зону
            Response.Cookies.Append("jwt", accessToken);
            return LocalRedirect("/");
        }
    }
}