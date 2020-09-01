using System;
using System.Collections.Generic;
using System.Linq;
using CovidOutApp.Web.Models;
using CovidOutApp.Web.Repositories;

namespace CovidOutApp.Web.ServiceLayer
{
    public class VenueRulesService : IVenueRulesService
    {
        
        private readonly IVenueRulesRepository _rulesRepository;

        public VenueRulesService (IVenueRulesRepository rulesRepository){
            this._rulesRepository = rulesRepository;
        }
        public bool Add(VenueRules rule)
        {
            if (rule ==null)
                throw new NullReferenceException("Cannot be null");
            
            try
            {
                this._rulesRepository.Add(rule);
                return true;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        public bool Delete(VenueRules rule)
        {
            if (rule ==null)
                throw new NullReferenceException("Cannot be null");
            
            try
            {
                this._rulesRepository.Delete(rule);
                return true;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        public IEnumerable<VenueRules> GetRulesByVenueId(Guid Id)
        {
            return this._rulesRepository.Query(x=>x.Venue.Id == Id);
        }

        public IEnumerable<VenueRules> GetRulesByDuration(Guid id,DateTime from, DateTime to)
        {
            var results= this._rulesRepository.Query(x=> x.AffectedDateFrom >= from && x.AffectedDateTo < to);

            return results;
        }
        
          public VenueRules GetTodayRules(Guid id)
        {
            var from = new DateTime(DateTime.Now.Year, DateTime.Now.Month,DateTime.Now.Day,
                                    0,0,0,0);
            var to  = from.AddDays(1);

            var results = GetRulesByDuration(id,from,to);
            
            return results.FirstOrDefault();
        
        }
          


        public bool Update(VenueRules rule)
        {
             if (rule ==null)
                throw new NullReferenceException("Cannot be null");
            
            try
            {
                this._rulesRepository.Update(rule);
                return true;
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}