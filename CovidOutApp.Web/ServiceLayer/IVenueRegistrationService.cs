using System;
using System.Collections.Generic;
using CovidOut.Web.ServiceLayer.Verification;
using CovidOutApp.Web.Models;


namespace CovidOutApp.Web.ServiceLayer
{
    public interface IVenueRegistrationService
    {
       Guid ApplyToRegisterVenue(VenueRegistrationApplication application);
       bool ApproveVenueRegistration(Guid applicationId, ApplicationUser approvedBy);
       bool RejectVenueRegistration(Guid applicationId);
       bool VerifyVenue(Guid applicationId, IVerification verification);
       IEnumerable<VenueRegistrationApplication> GetAllApplications();
       IEnumerable<VenueRegistrationApplication> GetAllApplicationsByUser();
       VenueRegistrationApplication FindApplicationByVenueId(Guid id);
       VenueRegistrationApplication FindApplicationById(Guid id);
    }
}