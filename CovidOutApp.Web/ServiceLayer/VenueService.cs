using CovidOutApp.Web.Repositories;
using CovidOutApp.Web.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CovidOutApp.Web.Data;
using System.Linq;

namespace CovidOutApp.Web.ServiceLayer {
    public class VenueService: IVenueService {

        private readonly IVenueRepository _venueRepository;
        private readonly IVenueVisitRepository _venueVisitRepository;
        private readonly ILogger<VenueService> _logger;

        public VenueService(IVenueRepository repository, IVenueVisitRepository visitRepository, ILogger<VenueService> logger)
        {   
            this._venueRepository = repository;
            this._venueVisitRepository = visitRepository;
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
            throw new NotImplementedException();
        }

        public bool UserHasCheckedId(Venue venue, ApplicationUser user){
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
                    if (visit.CheckOut < visit.CheckIn){
                        return false;
                    }
                }
            }                                                    
            
            return true;
        }

        public Visit FindVisitByVenueIdAndUser(Guid venueId, ApplicationUser user)
        {
            try {
                  var userVenueVisits = this._venueVisitRepository.QueryIncludeRelatedData(
                                                                 x=> x.Venue.Id == venueId &&
                                                                 x.User.Id == user.Id);
                  return userVenueVisits.SingleOrDefault();
            }
            catch (Exception ex){
                this._logger.LogError(ex.StackTrace);
                throw;
            }
        }
    }
}