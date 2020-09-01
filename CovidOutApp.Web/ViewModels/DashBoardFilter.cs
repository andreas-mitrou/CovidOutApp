using System;

namespace CovidOutApp.Web.ViewModels {
    public class DashBoardFilter {
        public Guid VenueId {get;set;}
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }
} 