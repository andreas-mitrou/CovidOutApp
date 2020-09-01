using System;
using System.Collections.Generic;
using CovidOutApp.Web.Models;

namespace CovidOutApp.Web.ServiceLayer {
    public interface IVenueRulesService {
        bool Add(VenueRules rule);
        bool Update(VenueRules rule);
        bool Delete(VenueRules rule);
        IEnumerable<VenueRules> GetRulesByVenueId(Guid Id);
        IEnumerable<VenueRules> GetRulesByDuration(Guid id,DateTime from, DateTime to);
        VenueRules GetTodayRules(Guid id);
    }
}