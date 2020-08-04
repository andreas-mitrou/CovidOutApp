using System;
using System.Collections.Generic;
using System.Linq;
using CovidOut.Web.ServiceLayer.Verification;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;
using Microsoft.Extensions.Logging;

namespace CovidOutApp.Web.ServiceLayer 
{
    public class VenueRegistrationService : IVenueRegistrationService
    {
        private readonly IVenueRegistrationApplicationRepository _reg_application_repo;
        private readonly IVenueRepository venue_repo;
        private readonly ILogger<VenueRegistrationService> _logger;
        public VenueRegistrationService(IVenueRegistrationApplicationRepository repo, IVenueRepository venueRepository, ILogger<VenueRegistrationService> logger)
        {
            _reg_application_repo = repo;
        }
        public Guid ApplyToRegisterVenue(VenueRegistrationApplication application)
        {
            var result = Guid.Empty;

            if (application == null)
                throw new NullReferenceException("Application object cannot be null");
            
            try
            {
               result = this._reg_application_repo.CreateApplication(application);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.StackTrace);
                throw;
            }
           
           return result;
        }

        public bool ApproveVenueRegistration(Guid applicationId, ApplicationUser approvedBy)
        {
            bool result = false;
            try
            {
                 var application = this._reg_application_repo.Find(applicationId);

                 if (application == null)
                    throw new NullReferenceException("Wasn't found");
                
                 application.DateAppoved = DateTime.Now;
                
                 application.ApprovedBy = approvedBy;
                
                 _reg_application_repo.Update(application);
                
                 result = true;
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.StackTrace);
                
                result = false;
            }
            
            return result;
        }

        public IEnumerable<VenueRegistrationApplication> GetAllApplications()
        {
            try
            {
                var applications = this._reg_application_repo.GetAllApplicationsWithRelatedData();
                return applications;
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.StackTrace);
                throw;
            } 
           
        }

        public IEnumerable<VenueRegistrationApplication> GetAllApplicationsByUser()
        {
            throw new NotImplementedException();
        }

        public bool RejectVenueRegistration(Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public bool VerifyVenue(Guid applicationId, IVerification verification)
        {
            throw new NotImplementedException();
        }
        public VenueRegistrationApplication FindApplicationByVenueId(Guid id){
            VenueRegistrationApplication result = null;
            try
            {
                 var results = this._reg_application_repo.Query(x=> x.VenueId == id);
                 if (results.Count() > 0){
                     result = results.SingleOrDefault();
                 }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }

            return result;
         }

        public VenueRegistrationApplication FindApplicationById(Guid id)
        {
            try
            {
                var application = this._reg_application_repo.FindApplicationIncludeRelatedDataById(id);
                
                if (application == null)
                    throw new NullReferenceException("Application was not found");
                
                return application;
            }
            catch (System.Exception ex)
            {
                this._logger.LogError(ex.StackTrace);
                throw;
            }
        }
    }
}   