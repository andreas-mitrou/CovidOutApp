using System;

namespace CovidOutApp.Web.ViewModels {
    public class VenueVisitViewModel {
        public Guid Id { get; set; }
        public Guid VenueId {get;set;}
        public string VenueName { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime? CheckOut { get; set; }
        public string VisitorComments {get;set;}
        public string UserId {get;set;} 
        public string UserName {get;set;}
        public string UserLastName {get;set;}
        public DateTime? ExpectedCheckOut {get;set;}

    }
}