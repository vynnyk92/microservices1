using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    public class EventStore : IEventStore
    {
        private static long currentSequenceNumber = 0;

        //db simulation
        private static readonly IList<Event> db = new List<Event>();

        public IEnumerable<Event> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
           return  
                db.Where(e =>
            e.SequenceNumber >= firstEventSequenceNumber &&
            e.SequenceNumber <= lastEventSequenceNumber)
          .OrderBy(e => e.SequenceNumber);
        }

        public void Raise(string eventName, object content)
        {
            var seqNumber = Interlocked.Increment(ref currentSequenceNumber);

            db.Add(new Event(
                seqNumber, 
                DateTime.Now, 
                eventName, 
                content
           ));
        }
    }
}
