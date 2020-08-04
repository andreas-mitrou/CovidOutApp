using System;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Enums;

namespace CovidOutApp.Web.ViewModels
{
    public class VenueRegistrationApplicationViewModel {
        public Guid Id { get; set; }
        public Guid VenueId {get;set;}
        public VenueViewModel VenueDetails{get;set;}
        public ApplicationUser AppliedBy { get; set; }
        public DateTime DateCreated {get;set;}
        public DateTime DateAppoved {get;set;}
        public ApplicationUser ApprovedBy {get;set;}
        public Verification_Method VerificationMethod {get;set;}
        
    }
      
}


