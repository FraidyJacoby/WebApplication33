using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApplication33.Models;
using WebApplication33.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WebApplication33.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=AdsWithUsers;Integrated Security=true;";

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var vm = new HomePageViewModel();
            var db = new UsersDb(_connectionString);
            vm.Ads = db.GetAds();
            vm.UserIsLoggedIn = User.Identity.IsAuthenticated;
            if (vm.UserIsLoggedIn)
            {
                var user = db.GetByEmail(User.Identity.Name);
                vm.UserId = user.Id;
            }
            return View(vm);
        }

        [Authorize]
        public IActionResult NewAd()
        {
            var db = new UsersDb(_connectionString);
            var userId = db.GetByEmail(User.Identity.Name).Id;
            var vm = new NewAdViewModel
            {
                UserId = userId
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult NewAd(Ad ad)
        {
            var db = new UsersDb(_connectionString);
            db.AddAd(ad);
            return Redirect("/");
        }

        public IActionResult DeleteAd(int id)
        {
            var db = new UsersDb(_connectionString);
            db.DeleteAd(id);
            return Redirect("/");
        }

    }
}
