using NUnit.Framework;
using CovidOutApp.Web;
using Microsoft.Extensions.DependencyInjection;
using CovidOutApp.Web.ServiceLayer;
using CovidOutApp.Web.Repositories;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using System;

namespace CovidOutApp.Tests
{
    public class VenueService_Tests
    {
        IVenueService srv;

        private ServiceProvider LoadDependencies(IServiceCollection services){
             services.AddLogging();
             services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(
                    "DataSource=D:\\Projects\\CovidOutApp\\CovidOutApp.Web\\app.db"
                    ));
       
             services.AddTransient<IVenueRepository, VenueRepository>();

    
            var serviceProvider = services.BuildServiceProvider();
            return serviceProvider;
        }

        [SetUp]
        public void Setup()
        {
            var services = new ServiceCollection();
            var serviceProvider = LoadDependencies(services);

            var venueRepository = serviceProvider.GetService<IVenueRepository>();
            var venueVisitRepository = serviceProvider.GetService<IVenueVisitRepository>();
            var logger = serviceProvider.GetService<ILogger<VenueService>>();
           
            this.srv = new VenueService(venueRepository, venueVisitRepository, logger);
        }

        [Test]
        public void CreateVenue_InputVenueIsNull_Throw()
        {
            Assert.Throws<NullReferenceException>(() => srv.CreateVenue(null));
        }
        [Test]
        public void DeleteVenue_InputIdIsEmpty_Throw(){
            Assert.Throws<NullReferenceException>(() => srv.DeleteVenue(Guid.Empty));

        }
    }
}