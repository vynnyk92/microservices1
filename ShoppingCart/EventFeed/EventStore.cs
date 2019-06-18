using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShoppingCart.EventFeed
{
    public class EventStore : IEventStore
    {


        private readonly string connString = "Server=GOLPE\\SQLEXPRESS;Database=ShoppingCartStore;Trusted_Connection=True;MultipleActiveResultSets=true";

        private const string readItemsSql = @"Select * from EventStore where EventStore.Id >= @minId and EventStore.Id <= @maxId";

        private const string writeEventSql = @"insert into EventStore(Name, OccurredAt, Content) values (@Name, @OccurredAt, @Content)";

        public async Task<IEnumerable<Event>> GetEvents(long firstEventSequenceNumber, long lastEventSequenceNumber)
        {
            using (var conn = new SqlConnection(connString))
            {
                var items = await conn.QueryAsync<dynamic>(readItemsSql, new { minId = firstEventSequenceNumber, maxId = lastEventSequenceNumber }).ConfigureAwait(false);

                return items.Select(row =>
                {
                    var cont = JsonConvert.DeserializeObject(row.Content);
                    return new Event(row.Id, row.Name ,row.OccuredAt, cont);
                });
            }
        }

        public async Task Raise(string eventName, object content)
        {
            
            var jsonContent = JsonConvert.SerializeObject(content);
            using (var conn = new SqlConnection(connString))
            {
                await conn.ExecuteAsync(writeEventSql, new { Name = eventName, OccuredAt = DateTimeOffset.Now, Content = jsonContent });
            }
        }
    }
}
