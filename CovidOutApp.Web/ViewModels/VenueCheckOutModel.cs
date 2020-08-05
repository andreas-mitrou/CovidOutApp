using System;

namespace CovidOutApp.Web.ViewModels {
    public class VenueCheckOutViewModel {
       public Guid VenueId {get;set;}
       public string Comments {get;set;}
       public VenueViewModel VenueDetails { get; set; }
    }
}