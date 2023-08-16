using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using ShoppingCart_6.Models;
using System.Security.Claims;
using ShoppingCart_6.Data;
using Microsoft.AspNetCore.Identity;

namespace ShoppingCart_6.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
       
        public AccountController(AppDbContext context)
        {
            _context = context; 
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = _context.Registers.FirstOrDefault(x => x.UserName == username && x.Password == password);
         
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
                    new Claim(ClaimTypes.Name, user.UserName)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true, 
                    ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(20) 
                };

                // Sign in the user
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                if (user.UserType == 0)
                {
                    return RedirectToAction("Index", "Orders");
                }
                else if (user.UserType == 1)
                {
                    return RedirectToAction("Index", "Products");
                }
            }

            ModelState.AddModelError("", "Invalid credentials");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Register register)
        {
            var user = _context.Registers.Where(x => x.UserName == register.UserName).Any();
            if (user)
            {
                ViewData["UserExist"] = "User Already Exist";
                return View();
            }
            _context.Registers.Add(register);
            await _context.SaveChangesAsync();

            return RedirectToAction("Login");

        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}
