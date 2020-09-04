using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.ViewModels;

namespace CovidOutApp.Web.Data
{
    public class ApplicationDbContext :  IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options){}
        public DbSet<Venue> Venues {get;set;}
        public DbSet<Visit> Visits {get;set;}
        public DbSet<Image> Images {get;set;}
        public DbSet<VenueRegistrationApplication> VenueRegistrationApplications { get; set; }
        public DbSet<VenueRules> VenueRules {get;set;}
        public DbSet<CovidOutApp.Web.ViewModels.VenueRulesViewModel> VenueRulesViewModel { get; set; }
    }
}
   