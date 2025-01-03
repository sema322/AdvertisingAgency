
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using AdvertisingAgency.Models;
using AdvertisingAgency;

namespace AdvertisingAgency.Controllers
{
    public class AccountController : Controller
    {

        private readonly AdvertisingAgencyContext _context;

        public AccountController(AdvertisingAgencyContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> LoginAsync(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Login", model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);

            if (user is null)
            {
                ViewBag.Error = "Некорректные логин и (или) пароль";
                return View("Login", model);
            }

            await AuthenticateAsync(user);

            if (user.Role == "Client")
            {
                return RedirectToAction("Create", "AdvRequest");
            }
            else
            {
                return RedirectToAction("Index", "Home"); 
            }
        }

        private async Task AuthenticateAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim (ClaimTypes.NameIdentifier, user. Id. ToString()),
                new Claim (ClaimsIdentity.DefaultNameClaimType, user. Login),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
            
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Register", model);
            }

            // Проверка на существующего пользователя
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login);
            if (existingUser != null)
            {
                ModelState.AddModelError("Login", "Пользователь с таким логином уже существует");
                return View("Register", model);
            }

            // Создание нового пользователя
            var user = new User
            {
                Login = model.Login,
                Password = model.Password,
                Role = "Client" 
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var client = new Client
            {
                UserID = user.Id,
                Name = model.Name,
                Surname = model.Surname,
                Phone = model.Phone
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            await AuthenticateAsync(user);

            return RedirectToAction("Create", "AdvRequest");
        }

    }


}
