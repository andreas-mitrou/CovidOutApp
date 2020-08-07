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
using Microsoft.AspNetCore.Authorization;
using CovidOutApp.Web.ServiceLayer;

namespace CovidOutApp.Web.Controllers
{
    [Authorize]
    public class VenueVisitController : BaseController
    {
        private readonly ILogger<VenueVisitController> _logger;
        private readonly IVenueService _venueService;

        public VenueVisitController(ILogger<VenueVisitController> logger,
                                    IVenueService venueService,
                                    UserManager<ApplicationUser> userManager) : 
            base(logger, userManager)
        {
            this._logger = logger;
            this._venueService = venueService;
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
        public async Task<IActionResult> CheckIn(VenueCheckInViewModel checkIn){

            var venueViewModel = new VenueViewModel();
            
            try
            {
               var currentUser = await GetUserIdAsync();

               var venueVisit = new Visit {
                   CheckIn = DateTime.Now,
                   Venue =  this._venueService.GetVenueById(checkIn.VenueId),
                   User = currentUser
               };
               
               var allUserVisits = this._venueService.FindVisitsByUser( currentUser);

               foreach (var userVisit in allUserVisits)
               {
                   if (userVisit.CheckOut == DateTime.MinValue ||
                       userVisit.CheckOut == null) {
                            throw new Exception($"You have already checked in { userVisit.Venue.Name }");
                        }
               }
               
               this._venueService.CheckInVisitor(venueVisit);              
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error", ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View(checkIn);
            }

            return RedirectToAction("Index");
        }

        [HttpGet("[controller]/[action]/{venueId}")]
        public IActionResult CheckIn(Guid venueId){

            var venueCheckInViewModel = new VenueCheckInViewModel();

            try
            {
                var venueDb = this._venueService.GetVenueById(venueId);
                
                if (venueDb == null)
                    throw new Exception("venue was not found");

                venueCheckInViewModel.VenueId = venueDb.Id;

                var venueViewModel = new VenueViewModel {
                    Name = venueDb.Name,
                    Email = venueDb.Email,
                    Telephone = venueDb.Telephone,
                    Address = venueDb.Address,
                    Capacity = venueDb.Capacity
                };

                venueCheckInViewModel.VenueDetails = venueViewModel;
                     
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error", ex.Message);
                ModelState.AddModelError("Error", ex.Message);
            }

            return View(venueCheckInViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> CheckOut(VenueCheckOutViewModel checkOut){

            var venueViewModel = new VenueViewModel();
            try
            {
               var userVisits = this._venueService.FindVisitsByVenueIdAndUser(checkOut.VenueId, await GetUserIdAsync());

                foreach (var visit in userVisits)
                {
                    if (visit.CheckOut == DateTime.MinValue || visit.CheckOut == null){
                        
                        visit.CheckOut = DateTime.Now;
                        visit.UserComments = checkOut.Comments;

                        this._venueService.CheckOutVisitor(visit);
                        
                        break;    
                    }
                }     
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error", ex.Message);
                ModelState.AddModelError("Error", ex.Message);
                return View(checkOut);
            }

            return RedirectToAction("Index");
        }

        
        [HttpGet("[controller]/[action]/{venueId}")]
        public async Task<IActionResult> CheckOut(Guid venueId) {

            var venueCheckOutViewModel = new VenueCheckOutViewModel();

            try
            {
                var venueDb = this._venueService.GetVenueById(venueId);

                var  userHasCheckedOut = this._venueService.UserHasCheckedOut(venueDb, await GetUserIdAsync());

                if (userHasCheckedOut)
                    throw new Exception("User hae already checked out");

                if (venueDb == null)
                    throw new Exception("Venue was not found");

                    venueCheckOutViewModel.VenueId = venueDb.Id;

                    var venueViewModel = new VenueViewModel {
                        Name = venueDb.Name,
                        Email = venueDb.Email,
                        Telephone = venueDb.Telephone,
                        Address = venueDb.Address,
                        Capacity = venueDb.Capacity
                    };

                    venueCheckOutViewModel.VenueDetails = venueViewModel;
                     
            }
            catch (System.Exception ex)
            {
                _logger.LogError("Error", ex.Message);
                ModelState.AddModelError("Error", ex.Message);
            }

            return View(venueCheckOutViewModel);
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
        

        public IActionResult SearchVenue(){
            return View();
        }

        [HttpPost]
        public IActionResult SearchVenue(string term){

            List<VenueViewModel> results = new List<VenueViewModel>();
            
            try
            {
                var resultsDb = this._venueService.SearchVenue(term); 

                foreach (var venue in resultsDb)
                {
                    results.Add(new VenueViewModel {
                        Id = venue.Id,
                        Name = venue.Name,
                        Address = venue.Address,
                        Telephone = venue.Telephone
                    });
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                ModelState.AddModelError("SearchError", ex.Message);
            }

            return PartialView("SearchVenueResults", results);
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
