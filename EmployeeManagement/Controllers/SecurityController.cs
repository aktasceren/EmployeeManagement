using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeManagement.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Controllers
{
    public class SecurityController : Controller
    {
        private readonly EmployeeContext _context;

        public SecurityController(EmployeeContext context)
        {
            _context = context;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string Email, string Password)
        {
            if (!string.IsNullOrEmpty(Email) && string.IsNullOrEmpty(Password))
            {
                return RedirectToAction("Login");
            }

            //Check the email and password from the database  
            var admin = _context.Admin.Where(a => a.Email == Email).FirstOrDefault();

            if (admin != null)
            {
                if (Password == admin.Password)
                {
                    //var claims = new List<Claim>
                    //{
                    //    new Claim(ClaimTypes.Name, admin.Email)
                    //};
                    //var useridentity = new ClaimsIdentity(claims, "Login");
                    ////Create the identity for the user  
                    var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, Email)
                }, "Login");

                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(principal);

                    return RedirectToAction("Index", "Home");
                }
            }
            {
                ViewBag.Message = "Invalid Email or Password";
                return View();
            }
        }

        public IActionResult Logout()
        {
            var logout = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}

