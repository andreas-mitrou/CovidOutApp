using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CovidOutApp.Web.Models;

namespace CovidOutApp.Web.Repositories {
    public interface IVenueVisitRepository : IGenericRepository<Visit> {
        public IEnumerable<Visit> QueryIncludeRelatedData(Expression<Func<Visit, bool>> predicate);
    }

}