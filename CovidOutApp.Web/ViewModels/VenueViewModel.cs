using System;
using System.Collections.Generic;


namespace CovidOutApp.Web.ViewModels {
    public class VenueViewModel {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string City { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; } 
        public int Capacity {get;set;}
        public TimeSpan Open {get;set;}
        public TimeSpan Close {get;set;}
        public IEnumerable<string> Images {get;set;}
    }
}