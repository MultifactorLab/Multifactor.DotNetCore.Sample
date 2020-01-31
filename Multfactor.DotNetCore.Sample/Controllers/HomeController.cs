using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Multfactor.DotNetCore.Sample.Controllers
{
    [Authorize] //authorized users only
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var claimsIdentity = User.Identity as ClaimsIdentity;
            var roleClaim = claimsIdentity.FindFirst("Role");
            ViewData["Role"] = roleClaim.Value;

            return View();
        }
    }
}
