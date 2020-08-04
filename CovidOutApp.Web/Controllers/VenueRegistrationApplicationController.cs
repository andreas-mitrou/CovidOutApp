using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CovidOutApp.Web.Data;
using CovidOutApp.Web.ViewModels;
using CovidOutApp.Web.ServiceLayer;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using CovidOutApp.Web.ViewModels;
using CovidOutApp.Web.Models;


namespace CovidOutApp.Web.Controllers
{
    [Authorize(Roles = "Admin, VenueOwner")] 
    public class VenueRegistrationApplicationController : BaseController{
    
        private readonly  UserManager<ApplicationUser> _userManager;
        private readonly IVenueRegistrationService _venueRegistrationService;
        private readonly IVenueService _venueService;
        private readonly ILogger<VenueRegistrationApplicationController> _logger;
        private readonly ApplicationDbContext _context;

        public VenueRegistrationApplicationController(ApplicationDbContext context, 
                               IVenueRegistrationService venueRegService, 
                               IVenueService venueService,
                               UserManager<ApplicationUser> userManager, 
                               ILogger<VenueRegistrationApplicationController> logger) : base (logger, userManager)
        {
             _venueRegistrationService = venueRegService;
             _venueService = venueService;
             _logger = logger;
             _context = context;
             _userManager = userManager;

        }

        // GET: VenueRegistrationApplication
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> Index()
        {
            List<VenueRegistrationApplicationViewModel> applications = new List<VenueRegistrationApplicationViewModel>();
            try {
                var applicationsDb = _venueRegistrationService.GetAllApplications();
               
                foreach (var app in applicationsDb)
                {
                    try
                    {
                        var venueDetails = _venueService.GetVenueById(app.VenueId);
                        
                        var venueRegistrationVM = new VenueRegistrationApplicationViewModel();
                        venueRegistrationVM.Id = app.Id;
                        venueRegistrationVM.DateCreated = app.DateCreated;
                        venueRegistrationVM.DateAppoved = app.DateAppoved;
                        venueRegistrationVM.ApprovedBy = app.ApprovedBy;
                        venueRegistrationVM.VenueDetails = new VenueViewModel {
                            Name = venueDetails.Name,
                            Address = venueDetails.Address
                        };
                        
                        applications.Add(venueRegistrationVM);
                    }
                    catch (System.Exception ex)
                    {
                         _logger.LogError(ex.StackTrace);
                          ModelState.AddModelError("Error", ex.StackTrace);
                    }
                }

               applications = applications.OrderByDescending(x=>x.DateCreated).ToList();

            }
            catch(Exception ex){
                _logger.LogError(ex.Source);
                ModelState.AddModelError("Error",ex.StackTrace);
            }
            
            return View(applications);
        }

        // GET: VenueRegistrationApplication/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            VenueRegistrationApplicationViewModel applicationVM = new VenueRegistrationApplicationViewModel();
            try
            {
                var applicationDb = this._venueRegistrationService.FindApplicationById(id.Value);
                
                if (applicationDb == null)
                    throw new NullReferenceException("Was not found");

                applicationVM.AppliedBy = applicationDb.AppliedBy;
                applicationVM.DateCreated = applicationDb.DateCreated;
                applicationVM.DateAppoved = applicationDb.DateAppoved;
                applicationVM.Id = applicationDb.Id;
                applicationVM.VenueId = applicationDb.VenueId;
                applicationVM.ApprovedBy = applicationDb.ApprovedBy;

                var venueDbDetails =  this._venueService.GetVenueById(applicationDb.VenueId);
                applicationVM.VenueDetails = new VenueViewModel {
                    Name = venueDbDetails.Name,
                    Address = venueDbDetails.Address,
                    City  = venueDbDetails.City
                };
                
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("error", ex.Message);
            }
            
        
            return View(applicationVM);
        }

        // GET: VenueRegistrationApplication/Create
        [HttpGet("[controller]/[action]/{venueId}")]
        public IActionResult Create(Guid venueId)
        {
            var venueApplyingFor = this._venueService.GetVenueById(venueId);
            
            ViewData["VenueName"] = venueApplyingFor.Name;
            ViewData["VenueAddress"] = venueApplyingFor.Address;

            var vm = new VenueRegistrationApplicationViewModel {
                VenueId = venueApplyingFor.Id
            };
            
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(string id)
        {
            if (id == null){
                throw new NullReferenceException("Id cannot be null");
            }
            try {
                 var guidCode = Guid.Parse(id);
            
                 var applicationDb = this._venueRegistrationService.FindApplicationById(guidCode);

                 this._venueRegistrationService.ApproveVenueRegistration(applicationDb.Id, await this.GetUserIdAsync());

                 return RedirectToAction("Index");
            }
            catch (Exception ex){
                this._logger.LogError(ex.StackTrace);
                return RedirectToAction("Details",new { id = id });
            }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,VenueId,DateCreated,DateAppoved")]
                                    VenueRegistrationApplicationViewModel venueRegistrationApplicationViewModel)
        {
            if (ModelState.IsValid)
            {
                try {
                    var venueRegistration = new VenueRegistrationApplication {
                        Id = Guid.NewGuid(),
                        VenueId = venueRegistrationApplicationViewModel.VenueId,
                        DateCreated = DateTime.Now,
                        AppliedBy = await this.GetUserIdAsync()
                    };

                    this._venueRegistrationService.ApplyToRegisterVenue(venueRegistration);

                    return RedirectToAction("Index");
                }
                catch (Exception ex){
                    this._logger.LogError(ex.Message);
                    ModelState.AddModelError("ApplyRegistration","Problem Creating the registration");      
                }
            }

            return View(venueRegistrationApplicationViewModel);
        }

        // GET: VenueRegistrationApplication/Edit/5
        // public async Task<IActionResult> Edit(Guid? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var venueRegistrationApplicationViewModel = await _context.VenueRegistrationApplicationViewModel.FindAsync(id);
        //     if (venueRegistrationApplicationViewModel == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(venueRegistrationApplicationViewModel);
        // }

        // // POST: VenueRegistrationApplication/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(Guid id, [Bind("Id,VenueId,DateCreated,DateAppoved")] VenueRegistrationApplicationViewModel venueRegistrationApplicationViewModel)
        // {
        //     if (id != venueRegistrationApplicationViewModel.Id)
        //     {
        //         return NotFound();
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(venueRegistrationApplicationViewModel);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!VenueRegistrationApplicationViewModelExists(venueRegistrationApplicationViewModel.Id))
        //             {
        //                 return NotFound();
        //             }
        //             else
        //             {
        //                 throw;
        //             }
        //         }
        //         return RedirectToAction(nameof(Index));
        //     }
        //     return View(venueRegistrationApplicationViewModel);
        // }

        // // GET: VenueRegistrationApplication/Delete/5
        // public async Task<IActionResult> Delete(Guid? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var venueRegistrationApplicationViewModel = await _context.VenueRegistrationApplicationViewModel
        //         .FirstOrDefaultAsync(m => m.Id == id);
        //     if (venueRegistrationApplicationViewModel == null)
        //     {
        //         return NotFound();
        //     }

        //     return View(venueRegistrationApplicationViewModel);
        // }

        // // POST: VenueRegistrationApplication/Delete/5
        // [HttpPost, ActionName("Delete")]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> DeleteConfirmed(Guid id)
        // {
        //     var venueRegistrationApplicationViewModel = await _context.VenueRegistrationApplicationViewModel.FindAsync(id);
        //     _context.VenueRegistrationApplicationViewModel.Remove(venueRegistrationApplicationViewModel);
        //     await _context.SaveChangesAsync();
        //     return RedirectToAction(nameof(Index));
        // }

        // private bool VenueRegistrationApplicationViewModelExists(Guid id)
        // {
        //     return _context.VenueRegistrationApplicationViewModel.Any(e => e.Id == id);
        // }
    }
}
