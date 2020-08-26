using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CovidOutApp.Web.Data;
using CovidOutApp.Web.ViewModels;
using CovidOutApp.Web.Models;
using Microsoft.AspNetCore.Authorization;
using CovidOutApp.Web.Repositories;
using CovidOutApp.Web.ServiceLayer;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using QRCoder;
using System.Drawing;
using System.IO;

namespace CovidOutApp.Web.Controllers
{
    [Authorize(Roles = "Admin, VenueOwner")] 
    public class VenueController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IVenueService _venueService;

        private readonly IVenueRegistrationService _venueRegService;
        private readonly ILogger<VenueController> _logger;

        public VenueController(ApplicationDbContext context, 
                               IVenueService venueService, 
                               IVenueRegistrationService venueRegistrationService,
                               UserManager<ApplicationUser> userManager, 
                               ILogger<VenueController> logger) : base (logger,userManager)
        {
            _venueService = venueService;
            _venueRegService = venueRegistrationService;
            _logger = logger;
            _context = context;
        }

        // GET: Venue
        public async Task<IActionResult> Index()
        {
            var venues = new List<VenueViewModel>();

            IEnumerable<Venue> venuesDb = null;
        
            try
            {
                 if (User.IsInRole("Admin")){
                    venuesDb = this._venueService.GetAllVenues().ToList();
                }
                else if (User.IsInRole("VenueOwner")){
                    var user = await this.GetUserIdAsync();
                    venuesDb = this._venueService.GetVenuesOwnedByUser(user.Id.ToString()).ToList();
                }
                
                if (venuesDb != null){
                        Action<Venue> mapToViewModel = venue => {
                        
                        var venueItem = new VenueViewModel {Name = venue.Name, Id = venue.Id};
                        
                        var relatedApplication = _venueRegService.FindApplicationByVenueId(venue.Id);
                        
                        if (relatedApplication != null){
                            if (relatedApplication.ApprovedBy != null){
                                venueItem.IsApproved = true;
                            }
                            else {
                                  venueItem.IsApproved = false;
                            }
                        }
                        else {
                            venueItem.IsApproved = null;
                        }
    
                        venues.Add(venueItem);
                    }; 

                    venuesDb.ToList().ForEach(mapToViewModel);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                ModelState.AddModelError("Error", ex.Message);
            }

            return View(venues);
        }

        [HttpPost]
        public IActionResult DownloadQRCode(QRCodeViewModel qrCode){
            var venueId = qrCode.VenueId;  
            
            try
            {              
                var venue = this._venueService.GetVenueById(venueId);
                
                if (venue == null){
                    throw new Exception("venue was not found");
                }

                var qrCodeFilePath = venue.QRCodeImageUrl;

                var qrCodeFileName = System.IO.Path.GetFileName(qrCodeFilePath);
              
                var fullFilePath = Path.Combine (Globals.QRCODE_DIR, qrCodeFileName);

                var bytes = System.IO.File.ReadAllBytes(fullFilePath);

                string contentType = "application/jpeg";

                return File(bytes,contentType,qrCodeFileName);

            }
            catch (System.Exception ex)
            {                
                this._logger.LogError(ex.StackTrace);
                TempData["Error"] = ex.Message;
               
                return RedirectToAction("QRCode", new { id = venueId});
            }
        }
        [HttpGet]
        public IActionResult QRCode(string id){
            
            var qrCode = new QRCodeViewModel();

            try {
                 if (id == null)
                     throw new Exception("The venue cannot be found");

                 var venue = this._venueService.GetVenueById(Guid.Parse(id));

                 if (venue == null) 
                    throw new Exception("The venue cannot be found");

                qrCode.ImageUrl = venue.QRCodeImageUrl;
                qrCode.VenueId = venue.Id;

                if (TempData["Error"] != null){
                    ViewBag.Error = TempData["Error"] as string;
                }
                 
            }
            catch(Exception ex){
                this._logger.LogError(ex.StackTrace);  
                ModelState.AddModelError("Error",ex.Message);
            }

           return View (qrCode);
        }

        [HttpPost]
        public IActionResult QRCode(QRCodeViewModel qrCode){

            if (ModelState.IsValid){
                 try {
                    var url =  Url.Action("Details", "Venue", new { id = qrCode.VenueId });
                    
                    var qrcode_file = this._venueService.GenerateQRCodeFromUrl(url);
                    
                    this._venueService.UpdateVenueQRCode(qrCode.VenueId, qrcode_file);

                    return RedirectToAction("Index");   
                }
                catch(Exception ex){
                    this._logger.LogError(ex.Message);
                    ModelState.AddModelError("Εrror",ex.Message);
                }
            }

            return View(qrCode);
           
        }

        [HttpPost] 
        public async Task<IActionResult> UploadImage(ImageUploadViewModel image, IFormFile file){
            
            if (ModelState.IsValid){

                if (image.VenueId == null){
                    throw new Exception ("Venue id cannot be null");
                }

                try {
                     if (file != null) {

                          var imageDb = new CovidOutApp.Web.Models.Image();
                          imageDb.Id = Guid.NewGuid();
                          imageDb.Name = image.Title;
                          imageDb.Venue = this._venueService.GetVenueById(image.VenueId);
                   
                          bool successImageStored = await this._venueService.AddImageAsync(imageDb, file, image.IsLogoImage);
                     
                     }     
                        return new RedirectToActionResult("Images","Venue", new { id = image.VenueId});
                    
                }
                catch (Exception ex){
                    this._logger.LogError(ex.StackTrace);
                    ModelState.AddModelError("Error", ex.Message);
                }
            }

            return View (image);
        }


        public IActionResult DeleteImageLogo(Guid? venueId){
            if (venueId == null){
                return NotFound();
            }

            try {
                this._venueService.DeleteVenueLogoImage(venueId.Value);
                return RedirectToAction("Images", new {id = venueId});
            }
            catch(Exception ex){
                this._logger.LogError(ex.StackTrace);
                ViewBag.ErrorText = ex.Message;
            }

            return RedirectToAction("Images", new {id = venueId.Value});
        }

        public IActionResult DeleteImage(Guid? id, string venueId){
            if (id == null)
                return NotFound();

            try
            {   
                this._venueService.DeleteImage(id.Value);   
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.Message);
            }

            return RedirectToAction("Images", new {id = venueId});
        }

        public IActionResult Images(Guid? id){
            
            var imageList = new VenueImageListViewModel();

            var imagesVM = new List<VenueImageViewModel>();
            
            ViewBag.VenueId = id;
            
            try
            {
                if (id == null)
                    throw new Exception("Guid is null");

                var images = this._venueService.GetVenueImages(id.Value);
        
                foreach(var image in images){

                    var imageVM = new VenueImageViewModel();
                    imageVM.Title = image.Name;
                    imageVM.FilePath = image.ImagePath;
                    imageVM.Id = image.Id;
                    imageVM.VenueId = id.Value.ToString();
                    imageVM.isLogoImage = false;

                    imagesVM.Add(imageVM);
                }

                var venueDetails = this._venueService.GetVenueById(id.Value);

                if (venueDetails != null && !String.IsNullOrEmpty(venueDetails.Logo)){

                    imagesVM.Add(new VenueImageViewModel {
                        Title = "Logo",
                        FilePath = venueDetails.Logo,
                        VenueId = id.Value.ToString(),
                        isLogoImage = true
                    });
                }

                imageList.Images = imagesVM;

                imageList.UploadImage = new ImageUploadViewModel {
                    VenueId = id.Value
                };
            }
            catch (System.Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }
            
            return View(imageList);
        }

        // GET: Venue/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {         
            try { 

                var dbVenueItem = this._venueService.GetVenueById(id.Value);
        
                if (dbVenueItem == null)  return NotFound();    

                var currentUser = await GetUserIdAsync();

                if (currentUser != null){
                    
                    ViewBag.UserHasCheckedIn = this._venueService.UserHasCheckedIn(dbVenueItem, currentUser);
                
                    ViewBag.UserHasCheckedOut  = this._venueService.UserHasCheckedOut(dbVenueItem, currentUser); 
                }

                var venueImages = this._venueService.GetVenueImages(id.Value);

                var venueImagesVM = new List<VenueImageViewModel>();

                foreach (var img in venueImages)
                {
                    venueImagesVM.Add(new VenueImageViewModel {
                        Title = img.Name, 
                        FilePath = img.ImagePath , 
                        Id = img.Id }
                    );
                }
                VenueViewModel venueVM = new VenueViewModel {
                            Id = dbVenueItem.Id,
                            Name = dbVenueItem.Name,
                            Address = dbVenueItem.Address,
                            City = dbVenueItem.City,
                            Telephone = dbVenueItem.Telephone,
                            Email = dbVenueItem.Email,
                            Capacity = dbVenueItem.Capacity,
                            Open = dbVenueItem.TimeOpens,
                            Close = dbVenueItem.TimeCloses,
                            Logo = dbVenueItem.Logo,
                            Images = venueImagesVM
                };

                return View(venueVM);           
                
                }
                catch (Exception ex){
                    ModelState.AddModelError("CouldNotGetDetails" ,ex.Message);
                    return View(null);
                }
        }

        // GET: Venue/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Venue/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VenueViewModel venue)
        {
            ModelState.Remove("Id");
            
            if (ModelState.IsValid)
            {
                var dbVenue = new Venue();
                dbVenue.Id = Guid.NewGuid();
                dbVenue.Name = venue.Name;
                dbVenue.Address = venue.Address;
                dbVenue.City = venue.City;
                dbVenue.Email = venue.Email;
                dbVenue.Telephone = venue.Telephone;
                dbVenue.Capacity = venue.Capacity;
                dbVenue.TimeOpens = venue.Open;
                dbVenue.TimeCloses = venue.Close;

                try {
                       var userDetails = await GetUserIdAsync();
                       dbVenue.OwnerUserId = userDetails.Id;
                      
                      _venueService.CreateVenue(dbVenue);
                      
                      return RedirectToAction(nameof(Index));
                }
                catch(Exception ex){
                    this._logger.LogError(ex.StackTrace);
                    ModelState.AddModelError("GeneralError",ex.Message);
                    return View(venue);
                }
               
            }
            return View(venue);
        }

        // GET: Venue/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var venueDb = await _context.Venues.FindAsync(id);

            if (venueDb == null)
            {
                return NotFound();
            }

            var venueViewModel = new VenueViewModel {
                Id = venueDb.Id,
                Name = venueDb.Name,
                Address = venueDb.Address,
                Telephone = venueDb.Telephone,
                Email = venueDb.Email,
                City = venueDb.City,
                Capacity = venueDb.Capacity,
                Open = venueDb.TimeOpens,
                Close = venueDb.TimeCloses
            };
            return View(venueViewModel);
        }

        // POST: Venue/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id,  VenueViewModel venueViewModel)
        {
            if (id != venueViewModel.Id)            
                return NotFound();
            
            if (ModelState.IsValid)
            {
                try
                {
                    var dbVenue = new Venue {
                        Name = venueViewModel.Name,
                        Id = id,
                        Address = venueViewModel.Address,
                        City = venueViewModel.City,
                        Email = venueViewModel.Email,
                        Telephone = venueViewModel.Telephone,
                        Capacity = venueViewModel.Capacity,
                        TimeOpens = venueViewModel.Open,
                        TimeCloses = venueViewModel.Close
                    };

                    _context.Update(dbVenue);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VenueExists(venueViewModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(venueViewModel);
        }

        // GET: Venue/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            try {
                this._venueService.DeleteVenue(id.Value);
            }
            catch(Exception ex){
                ModelState.AddModelError("DeleteError", ex.Message);
            }
            
            return RedirectToAction("Index");
        }

        // POST: Venue/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var venueViewModel = await _context.Venues.FindAsync(id);
            _context.Venues.Remove(venueViewModel);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VenueExists(Guid id)
        {
            return _context.Venues.Any(e => e.Id == id);
        }
    }
}
