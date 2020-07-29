using CovidOutApp.Web.Repositories;
using CovidOutApp.Web.Models;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using CovidOutApp.Web.Data;

namespace CovidOutApp.Web.ServiceLayer {
    public class VenueService: IVenueService {

        private readonly IVenueRepository _venueRepository;
        private readonly ILogger<VenueService> _logger;

        public VenueService(IVenueRepository repository, ILogger<VenueService> logger)
        {   
            this._venueRepository = repository;
            this._logger = logger;
        }

        public void CreateVenue(Venue venue)
        {
            if (venue == null)
                throw new NullReferenceException("Venue Cannot be null");

            if (venue.Id == null)
                venue.Id = Guid.NewGuid();
            
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


        
        public IEnumerable<Venue> SearchVenue(string name)
        {
            throw new NotImplementedException();
        }
    }
}