using System;
using Microsoft.AspNetCore.Identity;

namespace CovidOutApp.Web.Models {
    public class Visit {
        public Guid Id { get; set; }
        public Venue Venue {get;set;}
        public ApplicationUser User {get;set;}
        public DateTime CheckIn {get;set;}
        public DateTime CheckOut {get;set;}
        public string UserComments {get;set;}
    }
}