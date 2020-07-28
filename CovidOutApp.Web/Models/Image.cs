using System;

namespace CovidOutApp.Web.Models {
    public class Image {
        public Guid Id {get;set;}
        public string Name {get;set;}
        public string ImagePath {get;set;}
        public Venue Venue {get;set;}
    }
}