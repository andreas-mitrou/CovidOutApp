using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;
using Microsoft.AspNetCore.Http;

namespace CovidOutApp.Web.ServiceLayer {

    public interface IVisitorManagementService
    {   
        IEnumerable<Visit> GetVisits(DateTime fromDate, DateTime? endDate, Guid venueId);
        double? CalculatePeopleDensityRatio(DateTime fromDate, DateTime endDate, Guid venueId);
    }    
}