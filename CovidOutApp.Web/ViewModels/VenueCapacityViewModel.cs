namespace CovidOutApp.Web.ViewModels{
    public class VenueCapacityViewModel {
         public int CurrentVisitorsCount {get;set;}
         public int MaxCapacity {get;set;}
         public double PeopleDensity {get;set;}
         public string StatusIcon {get;set;}
         public VenueRulesViewModel Rules{get;set;}
          
    }
}