using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CovidOutApp.Web.ServiceLayer;
using Microsoft.Extensions.Logging;
using CovidOutApp.Web.ViewModels;
using CovidOutApp.Web.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace CovidOutApp.Web.Controllers
{
    
    [Authorize(Roles = "Admin, VenueOwner")] 
    public class VisitorManagementController : BaseController
    {
        private readonly IVisitorManagementService _visitorManagementService;
        private readonly IVenueService _venueService;
        private readonly IVenueRulesService _venueRulesService;
        private readonly ILogger<VisitorManagementController> _logger;
        public VisitorManagementController(IVisitorManagementService managementSevice, 
                                          ILogger<VisitorManagementController> logger,
                                          UserManager<ApplicationUser> userManager,
                                          IVenueService venueService,
                                          IVenueRulesService venueRulesService)
        :base(logger,userManager)
        {
            this._visitorManagementService = managementSevice;
            this._logger = logger;
            this._venueService = venueService;
            this._venueRulesService = venueRulesService;
        }
        
        [HttpPost]
        public IActionResult Filter(DashBoardFilter filter){
            var visitorManagement = new VisitorManagementViewModel();
            
            int capacity = 0;

            try {
                var rules = _venueRulesService.GetRulesByDuration(filter.VenueId, filter.StartDate.Value, filter.EndDate.Value);

                var currentVisitors = this._visitorManagementService.GetVisits(filter.StartDate.Value, filter.EndDate, filter.VenueId);
                
                var vmList = new List<VenueVisitViewModel>();

                foreach (var visit in currentVisitors){
                    vmList.Add(new VenueVisitViewModel {
                        Id = visit.Id,
                        UserId = visit.User.Id,
                        CheckIn = visit.CheckIn,
                        CheckOut = visit.CheckOut,
                        UserName  = visit.User.FirstName,
                        UserLastName = visit.User.LastName
                    });
                }

                visitorManagement.Visitors = vmList.OrderByDescending(x=>x.CheckIn);

                var venueDetails = this._venueService.GetVenueById(filter.VenueId);
            
                capacity = venueDetails.Capacity;

                visitorManagement.CurrentCapacityStatus = new VenueCapacityViewModel{
                    CurrentVisitorsCount = visitorManagement.Visitors != null ? 
                                           visitorManagement.Visitors.Count() :
                                           0,
                    
                    MaxCapacity = capacity,
                    PeopleDensity = 0
                };

                visitorManagement.Filter = new DashBoardFilter(){
                    StartDate = filter.StartDate,
                    EndDate = filter.EndDate,
                    VenueId = filter.VenueId
                    
                };
            }
            catch(Exception ex){
                this._logger.LogError(ex.StackTrace);
                ModelState.AddModelError("Error", ex.Message);
            }
            return View("Index",visitorManagement);

           
        }

        [HttpGet]
        public IActionResult Index(Guid id)
        {
            var visitorManagement = new VisitorManagementViewModel();
            
            int capacity = 0;

            try {
                var fromDate = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,0,0,0);
                var toDate = DateTime.Now;

                var rules = _venueRulesService.GetTodayRules(id);

                var currentVisitors = this._visitorManagementService.GetVisits(fromDate, toDate, id);
                
                var vmList = new List<VenueVisitViewModel>();

                foreach (var visit in currentVisitors){
                    vmList.Add(new VenueVisitViewModel {
                        Id = visit.Id,
                        UserId = visit.User.Id,
                        CheckIn = visit.CheckIn,
                        CheckOut = visit.CheckOut,
                        UserName  = visit.User.FirstName,
                        UserLastName = visit.User.LastName,
                        ExpectedCheckOut =  rules?.MaxStayInHours != null ? 
                                            (DateTime?)visit.CheckIn.AddHours(2):
                                            (DateTime?)null
                    });
                }

                visitorManagement.Visitors = vmList.OrderByDescending(x=>x.CheckIn);

                var venueDetails = this._venueService.GetVenueById(id);
            
                capacity = rules?.Capacity ?? venueDetails.Capacity;

                visitorManagement.CurrentCapacityStatus = new VenueCapacityViewModel{
                    CurrentVisitorsCount = visitorManagement.Visitors != null ? 
                                           visitorManagement.Visitors.Count() :
                                           0,
                    
                    MaxCapacity = capacity,
                    PeopleDensity = currentVisitors.Count() / capacity
                };

                visitorManagement.Filter = new DashBoardFilter(){VenueId = id};
            }
            catch(Exception ex){
                this._logger.LogError(ex.StackTrace);
                ModelState.AddModelError("Error", ex.Message);
            }
            return View(visitorManagement);
        }
    }
}