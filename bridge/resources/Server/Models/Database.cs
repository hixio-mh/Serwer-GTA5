using System;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Reflection;
using System.Collections.Generic;
using Extend;
using MongoDB.Bson.Serialization.Attributes;
using Main;
using Managers;
using GTANetworkAPI;
using Mongo;
using Vehicle = GTANetworkAPI.Vehicle;
using Model.Database;
using Interfaces;
using Logic.Inventory;
using Models;

namespace Model.Database
{
    public class MongoResult<U>
    {
        public ObjectId Id { get; set; }

        private static string collectionName;
        private static string CollectionName {
            get
            {
                if (collectionName != null)
                    return collectionName;

                if (collectionName == "")
                    return null;

                FieldInfo propertyInfo = typeof(U).GetField("CollectionName");
                if (propertyInfo == null)
                    collectionName = "";
                else
                    collectionName = (string)propertyInfo.GetValue(null);

                Console.WriteLine("This type {0} {1}", typeof(U), collectionName);
                //collectionName = 
                return collectionName;
            }
        }
        [BsonIgnore]
        private static IMongoCollection<U> collection = null;

        [BsonIgnore]
        public static IMongoCollection<U> Collection
        {
            get
            {
                if (CollectionName == "" || CollectionName == null) return null;

                if (collection == null)
                {
                    collection = Globals.Mongo.Database.GetCollection<U>(CollectionName);
                }
                return collection;
            }
            private set { }
        }

        public bool Save(params string[] vs)
        {
            if (vs == null)
            {
                throw new ArgumentNullException(nameof(vs));
            }

            if (ReferenceEquals(Collection, null)) return false;

            FilterDefinition<U> filter = Builders<U>.Filter.Eq("_id", Id);

            string name;

            UpdateDefinitionBuilder<U> update = Builders<U>.Update;
            UpdateDefinition<U> updateDefinition = null;
            PropertyInfo propety;
            for (int i = 0; i < vs.Length; i++)
            {
                name = vs[i];
                propety = typeof(U).GetProperty(name);
                if (propety == null)
                    return false;

                if (updateDefinition == null)
                    updateDefinition = update.Set(name, propety.GetValue(this));
                else
                    updateDefinition = updateDefinition.Set(name, propety.GetValue(this));

            }
            collection.UpdateOne(filter, updateDefinition);

            return true;
        }

        public bool Reset<T>(IMongoCollection<T> collection, params string[] vs)
        {
            if (vs == null)
            {
                throw new ArgumentNullException(nameof(vs));
            }

            FilterDefinition<T> filter = Builders<T>.Filter.Eq("_id", Id);

            string name;
            PropertyInfo propety;
            UpdateDefinitionBuilder<T> update = Builders<T>.Update;
            UpdateDefinition<T> updateDefinition = null;
            for (int i = 0; i < vs.Length; i++)
            {
                name = vs[i];
                propety = typeof(T).GetProperty(name);
                if (propety == null)
                    return false;

                if (updateDefinition == null)
                    updateDefinition = update.Unset(name);
                else
                    updateDefinition = updateDefinition.Unset(name);

            }
            collection.UpdateOne(filter, updateDefinition);

            return true;
        }
    }
}
