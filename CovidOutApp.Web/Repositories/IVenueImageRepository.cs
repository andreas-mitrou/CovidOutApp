using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CovidOutApp.Web.Models;

namespace CovidOutApp.Web.Repositories {
    public interface IVenueImageRepository:IGenericRepository<Image>
    {
        IEnumerable<Image> QueryIncludeRelatedData(Expression<Func<Image, bool>> predicate);
    }
}