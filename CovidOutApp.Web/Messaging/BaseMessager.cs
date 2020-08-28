using System.Collections.Generic;

namespace CovidOutApp.Web.Messaging
{
    public abstract class BaseMessager<TSender, TRecipient>{
        
        public TSender MessageSender {get;set;}
        public IEnumerable<TRecipient> Recipients {get;set;}
        public Importance MessageLevel{get;set;}
        public string MessageBody {get;set;}

    }
}