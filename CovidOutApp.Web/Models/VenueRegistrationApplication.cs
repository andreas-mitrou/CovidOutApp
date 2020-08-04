using System;
using CovidOutApp.Web.Enums;

namespace CovidOutApp.Web.Models
{
    public class VenueRegistrationApplication{
       public Guid Id { get; set; }
       public Guid VenueId {get;set;}
       public ApplicationUser AppliedBy { get; set; }
       public DateTime DateCreated {get;set;}
       public DateTime DateAppoved {get;set;}
       public ApplicationUser ApprovedBy {get;set;}
       public Verification_Method Verification {get;set;}
    }
}