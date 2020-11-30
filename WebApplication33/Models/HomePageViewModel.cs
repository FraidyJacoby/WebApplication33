using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication33.Data;

namespace WebApplication33.Models
{
    public class HomePageViewModel
    {
        public List<Ad> Ads { get; set; }
        public bool UserIsLoggedIn { get; set; }
        public int UserId { get; set; }
    }
}
