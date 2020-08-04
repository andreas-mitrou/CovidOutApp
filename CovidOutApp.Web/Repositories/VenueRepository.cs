using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;

namespace CovidOutApp.Web.Repositories {
    public class VenueRepository : GenericRepository<Venue>, IVenueRepository
    {
        public VenueRepository(ILogger<GenericRepository<Venue>> logger, ApplicationDbContext db) 
        : base(logger, db){}

       public IEnumerable<Venue> SearchByLocation(double lat, double lon){
           throw new NotImplementedException();
       } 
    }
}