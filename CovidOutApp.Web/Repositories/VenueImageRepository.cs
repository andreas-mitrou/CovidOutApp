using CovidOutApp.Web.Data;
using CovidOutApp.Web.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace CovidOutApp.Web.Repositories {
    public class VenueImageRepository : GenericRepository<Image>, IVenueImageRepository
    {
        public VenueImageRepository(ILogger<GenericRepository<Image>> logger, ApplicationDbContext db) 
        : base(logger, db){}

        public IEnumerable<Image> QueryIncludeRelatedData(Expression<Func<Image, bool>> predicate)
        {
            return this.dbSet.Include(c=>c.Venue).Where(predicate);
        }
    }
}