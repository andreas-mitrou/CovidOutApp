using System;

namespace CovidOutApp.Web.ViewModels {
    public class VenueCheckInViewModel {
       public Guid VenueId {get;set;}
       public VenueViewModel VenueDetails { get; set; }
    }
}