using CovidOutApp.Web.Repositories;
using CovidOutApp.Web.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CovidOutApp.Web.Data;
using System.Linq;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

namespace CovidOutApp.Web.ServiceLayer {
    public class VenueService: IVenueService {

        private readonly IVenueRepository _venueRepository;
        private readonly IVenueVisitRepository _venueVisitRepository;
        
        private readonly IVenueImageRepository _venueImageRepository;
        private readonly ILogger<VenueService> _logger;

        public VenueService(IVenueRepository repository, IVenueVisitRepository visitRepository, 
                            IVenueImageRepository imageRepository, ILogger<VenueService> logger)
        {   
            this._venueRepository = repository;
            this._venueVisitRepository = visitRepository;
            this._venueImageRepository = imageRepository;
            this._logger = logger;
        }

        public void CreateVenue(Venue venue)
        {
            if (venue == null)
                throw new NullReferenceException("Venue Cannot be null");

            if (venue.Id == null)
                venue.Id = Guid.NewGuid();
            
            
            if (venue.OwnerUserId == null)
                throw new NullReferenceException("Venue should have an owner");

            try {
                this._venueRepository.Add(venue);
            }
            catch(Exception ex){
                _logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public void DeleteVenue(Guid id)
        {
            if (id == Guid.Empty) throw new NullReferenceException("Id cannot be null");

            try {
                var venue = this.GetVenueById(id);

                if (venue == null) 
                    throw new Exception("Venue was not found! Cannot delete");

                this._venueRepository.Delete(venue);
            }
            catch(Exception ex){
                this._logger.LogError(ex.Message,ex.StackTrace);
                throw;
            }
        }
    
        public void EditVenueDetails(Guid id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Venue> FindVenueByName(string name)
        {

           if (String.IsNullOrWhiteSpace(name))
                throw new NullReferenceException ("Name cannot be empty");
        
           try {
               var results =  _venueRepository.Query(x=>x.Name.ToLower().StartsWith(name.ToLower()));
               return results;

           }
           catch(Exception ex){
             _logger.LogError(ex.StackTrace);
             throw;
         }
        }


        public IEnumerable<Venue> GetAllVenues()
        {
            try {
                    return  _venueRepository.GetAll();
            }
            catch (Exception ex){
                _logger.LogError(ex.StackTrace);
                throw;
            }
        }

    
        public Venue GetVenueById(Guid id)
        {
            if (id == null) 
                throw new NullReferenceException("Id cannot be null");
            
            try {
                var venue = this._venueRepository.Find(id);
                
                if (venue == null)
                    throw new Exception("Venue with the current id was not found!");
                
                return venue;
            }
            catch (Exception ex){
                this._logger.LogError(ex.Message, ex.StackTrace);
                throw;
            }
        }

        public bool CheckOutVisitor(Visit venueVisit){
            bool result = false;
            try
            {
                if (venueVisit == null)
                    throw new Exception ("Venue Visit is null");

                _venueVisitRepository.Update(venueVisit); 
                
                result = true;   
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw;
            }
            return result;
        }
        public bool CheckInVisitor(Visit venueVisit){
            bool result = false;

            if (venueVisit == null)
                throw new NullReferenceException("Null visit");
            
            try
            {
                this._venueVisitRepository.Add(venueVisit);
                result = true;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw;
            }

            return result;
        }

        public IEnumerable<Venue> GetVenuesOwnedByUser(string userId)
        {
            if (userId == null || userId == Guid.Empty.ToString())
                throw new Exception("User Id cannot be null");

            try
            {
                var userVenues = this._venueRepository.Query(x=>x.OwnerUserId == userId);
                return userVenues;       
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw;
            } 
        }

        public IEnumerable<Venue> SearchVenue(string name)
        {   
            if (String.IsNullOrEmpty(name))
                return Enumerable.Empty<Venue>();
                
            var venuesResult = this._venueRepository.Query(x=>x.Name.Contains(name));
            return venuesResult;
        }

        public bool UserHasCheckedIn(Venue venue, ApplicationUser user){

             var userVenueVisits = this._venueVisitRepository.Query(x=>x.Venue.Id == venue.Id &&
                                                                 x.User.Id == user.Id); 
             if (userVenueVisits.Count() > 0){
                 foreach (var venueVisit in userVenueVisits)
                 {
                     if (venueVisit.CheckOut < venueVisit.CheckIn){
                         return true;
                     }
                 }
             }

             return false;
            
        }
        public bool UserHasCheckedOut(Venue venue, ApplicationUser user)
        {
            var userVenueVisits = this._venueVisitRepository.Query(x=>x.Venue.Id == venue.Id &&
                                                                 x.User.Id == user.Id);
            if (userVenueVisits.Count() > 0){
                foreach (var visit in userVenueVisits)
                {
                    if (visit.CheckOut == DateTime.MinValue || visit.CheckOut == null){
                        return false;
                    }
                }
            }                                                    
            
            return true;
        }
        
        public IEnumerable<Visit> FindVisitsByUser(ApplicationUser user){
            try
            {
                var userVenueVisits = this._venueVisitRepository.QueryIncludeRelatedData(x=> x.User.Id == user.Id); 

                return  userVenueVisits;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw;
            }
        }
        public IEnumerable<Visit> FindVisitsByVenueIdAndUser(Guid venueId, ApplicationUser user)
        {
            try {
                  var userVenueVisits = this._venueVisitRepository.QueryIncludeRelatedData(
                                                                 x=> x.Venue.Id == venueId &&
                                                                 x.User.Id == user.Id);
                  return userVenueVisits;
            }
            catch (Exception ex){
                this._logger.LogError(ex.StackTrace);
                throw;
            }
        }

        public IEnumerable<CovidOutApp.Web.Models.Image> GetVenueImages(Guid venueId)
        {
            try
            {
                if (venueId == Guid.Empty)
                    throw new Exception("Guid was empty");

                var images = this._venueImageRepository.QueryIncludeRelatedData(x=>x.Venue.Id == venueId);

                return images;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.StackTrace);   
                throw;
            }
        }


        public string GenerateQRCodeFromUrl(string url){
            try
            {
                   QRCodeGenerator qrGenerator = new QRCodeGenerator();
                   QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
                   QRCode qrCode = new QRCode(qrCodeData);
            
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    var filePath = Path.Combine(Globals.QRCODE_DIR, $"{Guid.NewGuid().ToString()}.jpeg");
                    qrCodeImage.Save(filePath,ImageFormat.Jpeg);
            
                    return filePath;       
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.StackTrace);
                throw;
            }
        }    
        private async Task<string> SaveImageToFile(IFormFile postedFile, string directory){
            if (postedFile.Length > 0) {
                
                var extension = Path.GetExtension(postedFile.FileName);

                var filePath = Path.Combine(directory, $"{Guid.NewGuid().ToString()}{extension}");

                using (var fileStream = new FileStream(filePath, FileMode.Create)) {
                    
                    await postedFile.CopyToAsync(fileStream);
                }
                
                return filePath;
            }

            return null;
        }
        
        public bool UpdateVenueQRCode(Guid venueId, string file){
            var result = false;
            try
            {   
                var venue = this._venueRepository.Find(venueId);

                if (!String.IsNullOrEmpty(file)){

                     var filename = System.IO.Path.GetFileName(file);

                     venue.QRCodeImageUrl = $"/QRCODES/{filename}";

                    this._venueRepository.Update(venue);
                
                    result = true;
                }
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.StackTrace);
                throw;
            }

            return result;
        }
        public async Task<bool> AddImageAsync(CovidOutApp.Web.Models.Image imageMetadata, IFormFile imageFile, bool isLogo)
        {
            var imageStored = false;

            if (imageMetadata == null)
                throw new Exception("Image cannot be null");

            try
            {  
                if (String.IsNullOrEmpty(Globals.FILE_UPLOAD_DIR))
                     throw new Exception ("The file upload dir is not specified");

                var filepath = await SaveImageToFile(imageFile, Globals.FILE_UPLOAD_DIR);

                if (String.IsNullOrEmpty(imageMetadata.ImagePath)) {

                    var fileName = Path.GetFileName(filepath);

                    var relativePath = $"/{Path.GetFileName(Globals.FILE_UPLOAD_DIR)}/{fileName}";
                    
                    imageMetadata.ImagePath = relativePath;  
                }

                if (isLogo){
                    var venue = this._venueRepository.Find(imageMetadata.Venue.Id);
                    venue.Logo = imageMetadata.ImagePath;
                    this._venueRepository.Update(venue);
                }  
                else {
                    this._venueImageRepository.Add(imageMetadata);
                }
                
                imageStored = true;
            }
            catch (System.Exception ex) 
            {
                _logger.LogError(ex.StackTrace);    
                throw;
            }

            return imageStored;
        }

        public bool DeleteImage(Guid id)
        {

            bool imageDeleted = false;
            try
            {
                var image = _venueImageRepository.Find(id);
                
                if (image == null)
                    throw new Exception("Image was not found!");

                this._venueImageRepository.Delete(image);

                imageDeleted = true;
                
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.StackTrace);
                throw;
            }

            return imageDeleted;
        }

        public bool DeleteVenueLogoImage(Guid venueId)
        {
           try
           {
               var venue = this.GetVenueById(venueId);

               if(venue == null)
                   throw new Exception("Venue was not found");

               venue.Logo = null;

               this._venueRepository.Update(venue);

               return true;
           }
           catch (System.Exception ex)
           {
               this._logger.LogError(ex.StackTrace);
               throw;
           }
        }
    }
}