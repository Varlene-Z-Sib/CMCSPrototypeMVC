using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CMCSPrototypeMVC.Controllers
{
    public class AccountController : Controller
    {
        // Mock user list for demo purposes
        private readonly List<User> usersList = new List<User>
        {
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "lecturer", Password = "lecturer123", Role = "Lecturer" }
        };

        // GET: /Account/Index
        public IActionResult Index()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = usersList.FirstOrDefault(u => u.Username == username && u.Password == password);
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                // Redirect based on role
                return user.Role switch
                {
                    "Admin" => RedirectToAction("ReviewClaims", "Claims"),
                    "Lecturer" => RedirectToAction("SubmitClaim", "Claims"),
                    _ => RedirectToAction("Index", "Home")
                };
            }

            ViewBag.Error = "Invalid username or password.";
            return View("Index");
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Account");
        }

        // GET: /Account/AccessDenied
        public IActionResult AccessDenied()
        {
            return View();
        }
    }

    // Models/User.cs
    public class User
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }  // "Admin" or "Lecturer"
    }
}
