using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using CovidOutApp.Web.Models;
namespace CovidOutApp.Web.Repositories {
    public interface IVenueRulesRepository: IGenericRepository<VenueRules>{
        IEnumerable<VenueRules> GetRuleQueryFetchAdditionalData(Expression<Func<VenueRules,bool>> rulesQuery);
    }
}