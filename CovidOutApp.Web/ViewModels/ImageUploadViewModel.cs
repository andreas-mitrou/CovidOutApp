using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Microsoft.AspNetCore.Http;

namespace CovidOutApp.Web.ViewModels {
    public class ImageUploadViewModel {
        
        // public IFormFile Image {get;set;}
        public string Title { get; set; }
        public Guid VenueId {get;set;} 

        [Display(Name = "Is it a logo image?")]
        public bool IsLogoImage {get;set;}
    }
    
}