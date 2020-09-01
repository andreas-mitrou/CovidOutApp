using System;

namespace CovidOutApp.Web.ViewModels {
    public class VenueRulesViewModel {
        public Guid Id { get; set; }
        public DateTime ValidFrom {get;set;}
        public DateTime? ValidTo {get;set;}
        double MaxStayInHours {get;set;}
        public int AllowedCapacity {get;set;}
        public Guid VenueId {get;set;}
    }

}