using System;
using System.Collections.Generic;

namespace CovidOutApp.Web.ViewModels {
    public class VenueImageListViewModel {
       public IEnumerable<VenueImageViewModel> Images { get; set; }
       public ImageUploadViewModel UploadImage {get;set;}
    }
    
}