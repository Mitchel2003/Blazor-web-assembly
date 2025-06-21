using AppWeb.Application.Handlers;
using Microsoft.AspNetCore.Mvc;
using AppWeb.Shared.Inputs;
using AppWeb.Shared.Dtos;
using MediatR;

/* To authentication */
using Microsoft.AspNetCore.Authentication; // For authentication-related functionality
using Microsoft.AspNetCore.Authentication.Cookies; // For cookie authentication
using System.Security.Claims;

namespace AppWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator) => _mediator = mediator;

        [HttpGet]
        public IActionResult Login() => User.Identity!.IsAuthenticated ? RedirectToAction("Index", "Home") : View();

        [HttpGet]
        public IActionResult Register() => View();

        #region Action.login ------------------------------------------------------------
        /**
         * This method handles user loggin by validating the provided information on form.
         * It also checks for existing of cookies to handle authentication.
         */
        [HttpPost]
        public async Task<IActionResult> Login(LoginInput user)
        {
            if (!ModelState.IsValid) return View(user); // Return the view with validation errors if the model state is invalid
            var existingUser = (await _mediator.Send(new GetUsersByEmail(user.Email))).FirstOrDefault(u => u.Password == user.Password);
            if (existingUser == null)
            {
                //Can´t say if the email or password is invalid for security reasons
                ModelState.AddModelError("Email", "Invalid email or password.");
                ViewData["ErrorMessage"] = "Invalid email or password.";
                return View(user);
            }

            var claims = new List<Claim>
            { //Create a claims authIdentity
                new Claim(ClaimTypes.Name, existingUser.Username),
                new Claim(ClaimTypes.Email, existingUser.Email),
                new Claim("UserId", existingUser.Id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var properties = new AuthenticationProperties() { AllowRefresh = true }; /* Allow the authentication cookie to be refreshed */
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), properties);
            return RedirectToAction("Index", "Home"); // Assuming you have a HomeController with an Index action
        }
        #endregion ---------------------------------------------------------------------

        #region Action.register ------------------------------------------------------------
        /**
         * This method handles user registration by validating the provided information and saving it to the database.
         * It also checks for existing users with the same email.
         */
        [HttpPost]
        public async Task<IActionResult> Register(CreateUserInput user)
        {
            if (!ModelState.IsValid) View(user); // Return the view with validation errors if the model state is invalid
            var existingUser = (await _mediator.Send(new GetUsersByEmail(user.Email))).FirstOrDefault();
            if (existingUser != null)
            { //email already exists, add an error to the model state
                ModelState.AddModelError("email", "Email already exists.");
                ViewData["ErrorMessage"] = "Email already exists.";
                return View(user);
            }

            var input = new UserDto(user.Username, user.Email, user.Password);
            var newUser = await _mediator.Send(new CreateUser(input)); //CQRS
            // Redirect to a success page or login page if registration is successful
            if (newUser.Id != 0) return RedirectToAction("Login", "Auth");
            ModelState.AddModelError("", "Registration failed. Please try again.");
            ViewData["ErrorMessage"] = "Registration failed. Please try again."; // Set an error message to be displayed in the view
            return View();
        }
        #endregion ---------------------------------------------------------------------
    }
}
