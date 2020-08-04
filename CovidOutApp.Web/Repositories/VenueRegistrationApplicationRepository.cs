using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CovidOutApp.Web.Repositories {
    public class VenueRegistrationApplicationRepository : GenericRepository<VenueRegistrationApplication>, 
                                                          IVenueRegistrationApplicationRepository
    {
        public VenueRegistrationApplicationRepository(ILogger<GenericRepository<VenueRegistrationApplication>> logger,ApplicationDbContext db) 
        : base(logger, db){}

        public Guid CreateApplication(VenueRegistrationApplication application)
        {
            var itemId = Guid.Empty;

            using(this._dbContext){
                 var item = this._dbContext.VenueRegistrationApplications.Add(application);
                 this._dbContext.SaveChanges();
                 
                 itemId =  application.Id;
            }

            return itemId;
        }
        
        public  IEnumerable<VenueRegistrationApplication> GetAllApplicationsWithRelatedData(){
           return  this._dbContext.
                            VenueRegistrationApplications.
                            Include(x=>x.AppliedBy).
                            Include(x=>x.ApprovedBy);
                           
        }
        public VenueRegistrationApplication FindApplicationIncludeRelatedDataById(Guid id){
           return  this._dbContext.
                            VenueRegistrationApplications.
                            Include(x=>x.AppliedBy).
                            Include(x=>x.ApprovedBy).
                            SingleOrDefault(x=>x.Id == id);
                             
        }
    }
}