using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CovidOutApp.Web.Repositories {
    public class VenueRulesRepository : GenericRepository<VenueRules>, IVenueRulesRepository
    {
        public VenueRulesRepository(ILogger<GenericRepository<VenueRules>> logger, ApplicationDbContext db) 
        : base(logger, db)
        {
        }

        public IEnumerable<VenueRules> GetRuleQueryFetchAdditionalData(Expression<Func<VenueRules, bool>> rulesQuery)
        {
          return this.dbSet.Include(c => c.Venue).Where(rulesQuery);
        }
    }
}