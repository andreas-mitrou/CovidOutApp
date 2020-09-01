using System;
using System.Collections.Generic;
using System.Linq;
using CovidOut.Web.ServiceLayer.Verification;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;
using Microsoft.Extensions.Logging;

namespace CovidOutApp.Web.ServiceLayer 
{
    public class VisitorManagementService : IVisitorManagementService
    {
        private readonly ILogger<VisitorManagementService> _logger;
        private readonly IVenueVisitRepository _visitRepository;
        private readonly IVenueRepository _venueRepository;

        public VisitorManagementService(ILogger<VisitorManagementService> logger, IVenueVisitRepository visitRepository, IVenueRepository venueRepository){
            this._logger = logger;
            this._visitRepository = visitRepository;
            this._venueRepository= venueRepository;
        }

    
        public double? CalculatePeopleDensityRatio(DateTime fromDate, DateTime endDate, Guid venueId)
        {
            double? result = null;

           var visits = this.GetVisits(fromDate, endDate, venueId);

           var venueDetails = this._venueRepository.Find(venueId);

           if (venueDetails != null){
              result =  visits.Count() / venueDetails.Capacity;
           }

            return result;
        }

        public IEnumerable<Visit> GetVisits(DateTime fromDate, DateTime? endDate, Guid venueId)
        {
             if (venueId == Guid.Empty)
                throw new Exception("Venue should have an Id");

                if (endDate ==null)
                    endDate = DateTime.Now;

                var results= this._visitRepository.QueryIncludeRelatedData(x=> x.CheckIn >= fromDate && 
                                                             x.CheckIn <= endDate &&
                                                             x.Venue.Id == venueId);

                return results;
        }
    }
    
    }