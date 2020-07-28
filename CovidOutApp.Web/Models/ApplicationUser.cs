using Microsoft.AspNetCore.Identity;

namespace CovidOutApp.Web.Models {
    public class ApplicationUser : IdentityUser {
    public string Title {get;set;}
    [PersonalData]   
    public string FirstName { get; set; }
 
    [PersonalData]  
    public string LastName { get; set; }

    public bool IsVenueOwner {get;set;}
    }
}