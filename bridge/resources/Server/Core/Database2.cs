using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Reflection;
using Extend;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization.Attributes;
using System.Runtime.Serialization;
using Main;
using Managers;
using GTANetworkAPI;
using GTANetworkInternals;
using GTANetworkMethods;
using Model.Database;
using Models;
using Interfaces;
using Model.Logs;

namespace Mongo
{
    public class CMongoDB
    {
        IMongoDatabase database;
        MongoClient client;
        MongoClient Client { get => client; }

        public static readonly BsonDocument DummyBsonDocument = new BsonDocument();

        public IMongoCollection<Account> Accounts { private set; get; }
        public IMongoCollection<Configuration> Configurations { private set; get; }

        public IMongoDatabase Database { get => database; set => database = value; }

        bool IsCollectionExists(string name)
        {
            IAsyncCursor<string> collections = Database.ListCollectionNames();
            bool bExists = false;
            collections.ForEachAsync((collection) =>
            {
                if (collection == name)
                    bExists = true;
            });
            return bExists;
        }

        bool CreateCollectionIfNotExists(string name)
        {
            bool exists = IsCollectionExists(name);
            if(!exists)
            {
                Database.CreateCollection(name);
                return true;
            }
            return false;
        }

        public long CountDocuments<T>(IMongoCollection<T> collection) =>
            collection.CountDocuments(DummyBsonDocument);
        
        public bool IsCollectionEmpty<T>(IMongoCollection<T> collection) =>
            CountDocuments(collection) == 0;

        void CreateDefault()
        {
            CreateCollectionIfNotExists("Accounts");
            CreateCollectionIfNotExists("Configuration");

            Logs.GetAllLogsCollections().ForEach(c => CreateCollectionIfNotExists(c));
        }

        void Test()
        {
            /*Accounts.InsertOne(new Account
            {
                Name = "Test",
                Password = "testowe",
            });*/

            List<Account> accounts = Accounts.Find(a => true).ToListAsync().Result;

            accounts.ForEach(account => Console.WriteLine("Account id {0}",account.AccountId));
            accounts.ForEach(account => {

                //CItem a = account.Inventory.GiveItem(1);
            });
            //CDebug.Debug("GetNextAccountId", Account.GetNextAccountId());

            /*foreach (Account account in accounts)
            {
                CDebug.Debug("time", account.RegisterTime);
                CDebug.Debug("account", account.Serialize());
                account.Licenses.Add(new License { LicenseId = 1 });
                account.Save(collection, "Licenses");

                account.Reset(collection, "Clothes");

                account.Money = 5;
                account.Xp = 67;
                account.Save("Money", "Xp");
                Console.WriteLine("zapisano");

                foreach (License license in account.Licenses)
                {
                    Console.WriteLine("license {0} {1}", license.SuspendedDateTime, license.LicenseId);
                }
                Console.WriteLine("account {0} {1}", account.Name, account.Licenses.Count);
            }*/


            /*await collection.InsertOneAsync(new Accounts { Name = "Jack" });

            var list = await collection.Find(x => x.Name == "Jack")
                .ToListAsync();

            foreach (var person in list)
            {
                Console.WriteLine(person.Name);
            }*/
        }

        public CMongoDB()
        {
            client = new MongoClient("mongodb://localhost:27017");
            Database = Client.GetDatabase("gta5");
        }
        public void OnMongoReady()
        {
            Accounts = Account.Collection;
            Configurations = Configuration.Collection;

            CreateDefault();
            Globals.Config.Initialize();
            Test();
        }

        public static void Initialize()
        {
            Globals.Mongo = new CMongoDB();
        }
    }
}
