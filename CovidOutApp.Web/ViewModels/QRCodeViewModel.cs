using System;

namespace CovidOutApp.Web.ViewModels {
    public class QRCodeViewModel {
       public Guid VenueId {get;set;}
       public string ImageUrl { get; set; }
    }
}