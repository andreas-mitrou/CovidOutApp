using System;
using System.Collections.Generic;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;

namespace CovidOutApp.Web.ServiceLayer {
    public interface IVenueService
    {   
        IEnumerable<Venue> SearchVenue(string name);
        IEnumerable<Venue> GetAllVenues();
        Venue GetVenueById(Guid id);
        void CreateVenue(Venue venue);
        void DeleteVenue (Guid id);
        void EditVenueDetails(Guid id);
    }
}