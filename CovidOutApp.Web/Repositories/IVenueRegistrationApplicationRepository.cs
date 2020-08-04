using CovidOutApp.Web.Models;
using System;
using System.Collections.Generic;

namespace CovidOutApp.Web.Repositories
{
    public interface IVenueRegistrationApplicationRepository: IGenericRepository<VenueRegistrationApplication>
    {
        Guid CreateApplication(VenueRegistrationApplication application);
        VenueRegistrationApplication FindApplicationIncludeRelatedDataById(Guid id);
        IEnumerable<VenueRegistrationApplication> GetAllApplicationsWithRelatedData();
    }
}