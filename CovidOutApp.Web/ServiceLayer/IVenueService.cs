using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;
using Microsoft.AspNetCore.Http;

namespace CovidOutApp.Web.ServiceLayer {
    public interface IVenueService
    {   
        IEnumerable<Venue> SearchVenue(string name);
        IEnumerable<Venue> GetAllVenues();
        IEnumerable<Venue> GetVenuesOwnedByUser(string userId);
        Venue GetVenueById(Guid id);
        void CreateVenue(Venue venue);
        void DeleteVenue (Guid id);
        void EditVenueDetails(Guid id);
        bool CheckInVisitor(Visit venueVisit); 
        bool CheckOutVisitor(Visit venueVisit);
        IEnumerable<Visit> FindVisitsByVenueIdAndUser(Guid venueId, ApplicationUser user);
        IEnumerable<Visit> FindVisitsByUser(ApplicationUser user);
        bool UserHasCheckedOut(Venue venue, ApplicationUser user);
        bool UserHasCheckedIn(Venue venue, ApplicationUser user);
        IEnumerable<Image> GetVenueImages(Guid venueId);

        Task<bool> AddImageAsync (Image imageMetadata, IFormFile imageFile,  bool isLogo);
        bool DeleteImage (Guid id);
        bool DeleteVenueLogoImage(Guid venueId);
        string GenerateQRCodeFromUrl(string url);
        bool UpdateVenueQRCode(Guid venueId, string url);
    }
}