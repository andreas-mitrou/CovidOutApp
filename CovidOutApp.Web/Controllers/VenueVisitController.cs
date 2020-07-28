using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CovidOutApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using CovidOutApp.Web.ViewModels;
using CovidOutApp.Web.Models;

namespace CovidOutApp.Web.Controllers
{
    public class VenueVisitController : BaseController
    {
        public VenueVisitController(ILogger<VenueVisitController> logger, UserManager<ApplicationUser> userManager) : 
            base(logger, userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(VenueVisitViewModel visit) {

            if (!ModelState.IsValid) return View(visit);

            var user = await GetUserIdAsync();

            visit.UserId = user.Id;

            return RedirectToAction("Index");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult SearchVenues(string term){
            try {
                 IEnumerable<Venue> results = new List<Venue>();
            
                IEnumerable<Venue> venues = new List<Venue> 
                    { new Venue {Id = Guid.NewGuid(), Name="Lexington"},
                    new Venue {Id = Guid.NewGuid(), Name="Barrio"},
                    new Venue {Id = Guid.NewGuid(), Name="Carrio"}
                    };

                if (!String.IsNullOrEmpty(term)){
                    results = venues.Where(x=> x.Name.StartsWith(term));
                }
                return Ok(results);
            }
            catch (Exception ex){
                return Problem("Errpr",ex.StackTrace);
            }          
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
