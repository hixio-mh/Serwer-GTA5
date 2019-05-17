using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Driver;
using Extend;
using MongoDB.Bson.Serialization.Attributes;
using Main;
using Managers;
using GTANetworkAPI;
using Mongo;
using Vehicle = GTANetworkAPI.Vehicle;
using Model.Database;
using Utils;
using System.Linq;

namespace Model.Logs
{
    public class Logs
    {
        public DateTime Date = DateTime.Now;

        public static List<string> GetAllLogsCollections()
        {
            //IEnumerable<Type> subClasses = ReflectionHelper.GetAllSubclassOf(typeof(Logs));
            //List<string> vs = subClasses.ToList().ConvertAll(t => t.Name);

            List<string> vs = new List<string>();
            vs.Add(typeof(LogsMoney).Name);

            return vs;
        }

        internal void Log()
        {
            string CollectionName = GetType().Name;
            IMongoCollection<BsonDocument> Collection = Globals.Mongo.Database.GetCollection<BsonDocument>(CollectionName);
            BsonDocument data = this.ToBsonDocument();
            data.Remove("_t");
            Collection.InsertOneAsync(data);
        }

        public static void TellSomethingImportant(string message)
        {
            ConsoleColor last = Console.BackgroundColor;
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("WAŻNE: {0}", message);
            Console.BackgroundColor = last;
        }
    }

    public class LogsMoney : Logs
    {
        public long AccountId { get; set; }
    }
}
