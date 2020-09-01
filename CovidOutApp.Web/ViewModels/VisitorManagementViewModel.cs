using System.Collections.Generic;
using System;

namespace CovidOutApp.Web.ViewModels{
    public class VisitorManagementViewModel {

        public Guid VenueId {get;set;}
        public IEnumerable<VenueVisitViewModel> Visitors {get;set;}
        public VenueCapacityViewModel CurrentCapacityStatus {get;set;}
        public DashBoardFilter Filter{get;set;}
    }
}

