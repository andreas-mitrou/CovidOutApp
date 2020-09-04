using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CovidOutApp.Web.Data;
using CovidOutApp.Web.ViewModels;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.ServiceLayer;
using System.Collections.Generic;

namespace CovidOutApp.Web.Controllers
{
    public class VenueRulesController : BaseController
    {        
        private readonly IVenueRulesService _venueRulesService;
        private readonly IVenueService _venueService;
        public VenueRulesController(ILogger<VenueRulesController> logger, 
                                    UserManager<ApplicationUser> userManager,
                                    IVenueRulesService rulesService,
                                    IVenueService venueService) 
        : base(logger, userManager)
        {
            this._venueRulesService = rulesService;
            this._venueService = venueService;
        }   

        // GET: VenueRulesContoller
        public async Task<IActionResult> Index(Guid venueId)
        {
            var rules = new List<VenueRulesViewModel>();

            if (venueId == Guid.Empty)
                throw new Exception("A venue id is required");
            
            try{
                  var rulesDb = this._venueRulesService.GetRulesByVenueId(venueId);
                  foreach(var dbRule in rulesDb){
                      rules.Add(new VenueRulesViewModel{
                          Id = dbRule.Id,
                          VenueId = venueId,
                          ValidFrom = dbRule.AffectedDateFrom,
                          ValidTo = dbRule.AffectedDateTo,
                          AllowedCapacity = dbRule.Capacity                          
                      });
                  }
             }
            catch(Exception ex){
                this._logger.LogError(ex.StackTrace);
                ModelState.AddModelError("Error",ex.Message);
            }
           
            return View(rules);
        }

        // GET: VenueRulesContoller/Details/5
        
        // GET: VenueRulesContoller/Create
        public IActionResult Create(Guid venueId)
        {
            ViewBag.venueId = venueId;
            return View();
        }

        // POST: VenueRulesContoller/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ValidFrom,ValidTo,AllowedCapacity,VenueId")] VenueRulesViewModel venueRulesViewModel)
        {
            if (ModelState.IsValid)
            {
               try {
                   var rulesDb = new VenueRules();
                    rulesDb.AffectedDateFrom = venueRulesViewModel.ValidFrom;
                    rulesDb.AffectedDateTo = venueRulesViewModel.ValidTo.Value;
                    rulesDb.Capacity = venueRulesViewModel.AllowedCapacity;
                    rulesDb.MaxStayInHours = venueRulesViewModel.MaxStayInHours;
                    
                    var venueItem = this._venueService.GetVenueById(venueRulesViewModel.VenueId);
                    if (venueItem == null)
                        throw new Exception("Venue was not found");

                    rulesDb.Venue = venueItem;
                    this._venueRulesService.Add(rulesDb);

                    return RedirectToAction("Index", new {venueId = venueRulesViewModel.VenueId });
               }
               catch(Exception ex){
                    this._logger.LogError(ex.StackTrace);
                    ModelState.AddModelError("Error", ex.Message);
               }
            }
            return View(venueRulesViewModel);
        }

        // GET: VenueRulesContoller/Edit/5
        // public async Task<IActionResult> Edit(Guid? id)
        // {
        //     if (id == null)
        //     {
        //         return NotFound();
        //     }

        //     var venueRulesViewModel = await _context.VenueRulesViewModel.FindAsync(id);
        //     if (venueRulesViewModel == null)
        //     {
        //         return NotFound();
        //     }
        //     return View(venueRulesViewModel);
        // }

        // // POST: VenueRulesContoller/Edit/5
        // // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // [HttpPost]
        // [ValidateAntiForgeryToken]
        // public async Task<IActionResult> Edit(Guid id, [Bind("Id,ValidFrom,ValidTo,AllowedCapacity,VenueId")] VenueRulesViewModel venueRulesViewModel)
        // {
        //     if (id != venueRulesViewModel.Id)
        //     {
        //         return NotFound();
        //     }

        //     if (ModelState.IsValid)
        //     {
        //         try
        //         {
        //             _context.Update(venueRulesViewModel);
        //             await _context.SaveChangesAsync();
        //         }
        //         catch (DbUpdateConcurrencyException)
        //         {
        //             if (!VenueRulesViewModelExists(venueRulesViewModel.Id))
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
        //     return View(venueRulesViewModel);
        // }

        // GET: VenueRulesContoller/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venueRulesDbItems = this._venueRulesService.GetRuleQueryFetchAdditionalData(x=>x.Id == id.Value);
           
            if (venueRulesDbItems == null)
            {
                return NotFound();
            }

             var venueRulesDb = venueRulesDbItems.FirstOrDefault();

             var venueRuleViewModel = new VenueRulesViewModel();
             venueRuleViewModel.ValidFrom = venueRulesDb.AffectedDateFrom;
             venueRuleViewModel.ValidTo = venueRulesDb.AffectedDateTo;
             venueRuleViewModel.Id = venueRulesDb.Id;
             venueRuleViewModel.VenueId = venueRulesDb.Venue.Id;
             venueRuleViewModel.AllowedCapacity = venueRulesDb.Capacity;
             venueRuleViewModel.MaxStayInHours = venueRulesDb.MaxStayInHours;
            
             return View(venueRuleViewModel);
        }

        // // POST: VenueRulesContoller/Delete/5
         [HttpPost, ActionName("Delete")]
         [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(VenueRulesViewModel ruleViewModel)
        {            
            try {
                var rule = this._venueRulesService.GetRuleByRuleId(ruleViewModel.Id);
                
                if (rule == null)
                    throw new Exception($"Rule with {ruleViewModel.Id} was not found!");

                this._venueRulesService.Delete(rule);

                return RedirectToAction("Index", new {venueId = ruleViewModel.VenueId});
            }
            catch(Exception ex){
                ModelState.AddModelError("Erro",ex.Message);
                _logger.LogError(ex.StackTrace);
            }

            return View("Delete", ruleViewModel);  
        }
    }
}
