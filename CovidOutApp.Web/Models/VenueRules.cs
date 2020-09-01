using System;

namespace CovidOutApp.Web.Models {
    public class VenueRules {
        public Guid Id { get; set; }
        public DateTime AffectedDateFrom {get;set;}
        public DateTime AffectedDateTo {get;set;}
        public double MaxStayInHours {get;set;}
        public int Capacity {get;set;}
        public Venue Venue {get;set;}        
    }
}