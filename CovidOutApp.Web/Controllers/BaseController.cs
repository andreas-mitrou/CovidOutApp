using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CovidOutApp.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace CovidOutApp.Web.Controllers
{
 public abstract class BaseController : Controller
    {   
        protected readonly ILogger<BaseController> _logger;
        protected readonly UserManager<ApplicationUser>  _userManager;

        public BaseController(ILogger<BaseController> logger, UserManager<ApplicationUser> userManager){
            _logger = logger; 
            _userManager = userManager;
        }

        protected async Task<ApplicationUser> GetUserIdAsync() => await _userManager.GetUserAsync(this.User);
    }
}