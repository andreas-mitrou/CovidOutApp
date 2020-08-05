using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CovidOutApp.Web.Repositories {
    public class VenueVisitRepository : GenericRepository<Visit>, IVenueVisitRepository
    {
        public VenueVisitRepository(ILogger<GenericRepository<Visit>> logger, ApplicationDbContext db) 
        : base(logger, db){}

        public IEnumerable<Visit> QueryIncludeRelatedData(Expression<Func<Visit, bool>> predicate)
        {
            return this.dbSet.Include(c=>c.Venue).Where(predicate);
        }
    }
}