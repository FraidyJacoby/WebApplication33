using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebApplication33.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using WebApplication33.Models;

namespace WebApplication33.Controllers
{
    public class AccountController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=AdsWithUsers;Integrated Security=true;";

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string email, string password)
        {
            var db = new UsersDb(_connectionString);
            var user = db.LogIn(email, password);
            if(user != null)
            {
                var claims = new List<Claim>
            {
                new Claim("user", email)
            };

                HttpContext.SignInAsync(new ClaimsPrincipal(
                    new ClaimsIdentity(claims, "Cookies", "user", "role"))).Wait();

            }
            

            return Redirect("/");
        }

        public IActionResult LogOut()
        {
            HttpContext.SignOutAsync().Wait();
            return Redirect("/");
        }

        public IActionResult SignUp()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SignUp(User user, string password)
        {
            var db = new UsersDb(_connectionString);
            db.SignUp(user, password);

            Response.Cookies.Append("signedUp", "true");

            return Redirect("/");
        }

        public IActionResult MyAccount()
        {
            var db = new UsersDb(_connectionString);
            var userId = db.GetByEmail(User.Identity.Name).Id;
            var vm = new MyAccountViewModel
            {
                MyAds = db.GetAdsById(userId)
            };

            return View(vm);
        }
    }
}
