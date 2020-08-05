using System;
using System.Collections.Generic;

namespace CovidOutApp.Web.Models {
    public class Venue {
        public Guid Id { get; set; }
        public string Name {get;set;}
        public string Address {get;set;}
        public string City { get; set; }
        public string Email { get; set; }
        public string Telephone { get; set; } 
        public int Capacity {get;set;}
        public TimeSpan TimeOpens {get;set;}
        public TimeSpan TimeCloses {get;set;}
        public string Logo {get;set;}
        public string OwnerUserId {get;set;}
        ICollection<Image> Images {get;set;}
    }
}