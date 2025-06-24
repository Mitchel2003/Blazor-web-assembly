using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Server.Models;
using System.Diagnostics;

namespace AppWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger) { _logger = logger; }

        public IActionResult Index() => View();
        public IActionResult Privacy() => View();

        #region Actions ------------------------------------------------------------
        /** 
         * This method handles the error page.
         * It returns an ErrorViewModel with the current request ID for debugging purposes.
         */
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });

        /**
         * This method handles user logout.
         * by clearing the authentication cookie and redirecting to the login page.
         */
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Auth");
        }
        #endregion ---------------------------------------------------------------------
    }
}
