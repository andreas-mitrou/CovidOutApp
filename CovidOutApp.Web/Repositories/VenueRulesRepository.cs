using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.Extensions.Logging;

namespace CovidOutApp.Web.Repositories {
    public class VenueRulesRepository : GenericRepository<VenueRules>, IVenueRulesRepository
    {
        public VenueRulesRepository(ILogger<GenericRepository<VenueRules>> logger, ApplicationDbContext db) 
        : base(logger, db)
        {
        }
    }
}