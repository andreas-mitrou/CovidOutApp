using System;

namespace CovidOutApp.Web.ViewModels {
    public class VenueImageViewModel {
        public Guid? Id { get; set; }
        public string FilePath {get;set;}
        public string Title { get; set; }
        public string VenueId {get;set;} 

        public bool isLogoImage {get;set;}
    }
    
}