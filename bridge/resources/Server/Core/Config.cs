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
using Mongo;

namespace Main
{
    using Spawn = Tuple<Vector3, float, string>;
    using PublicVehicle = Tuple<Vector3, float, VehicleHash>;

    public class ExamQuestion
    {
        public int examId;
        public string question;
        public string a;
        public string b;
        public string c;
        public string d;
        public ExamQuestion(int examId, string question, string a, string b, string c, string d)
        {
            this.examId = examId;
            this.question = question;
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;
        }
    }

    public class Config 
    {
        Configuration config = null;
        IMongoCollection<Configuration> Configuration;

        public void Initialize()
        {
            Configuration = Globals.Mongo.Configurations;

            List<Configuration> configs = Configuration.Find(a => true).ToListAsync().Result;
            if(configs.Count == 1)
            {
                config = configs.First();
                return;
            }
            config = new Configuration();
            Globals.Mongo.Configurations.DeleteMany(CMongoDB.DummyBsonDocument);
            InitializeDefaultConfig();
        }

        public void InitializeDefaultConfig()
        {
            if (!Globals.Mongo.IsCollectionEmpty(Globals.Mongo.Configurations)) return;

            config.Data["Jaieś ustawienia1"] = true;
            config.Data["Jaieś ustawienia2"] = 2;
            config.Data["Jaieś ustawienia3"] = new { a = 1, b = 2, c = 3 };

            config.Spawnpoints["Testowy spawn"] = new Spawn(new Vector3(0,0,0), 0.0f, "");

            config.PublicVehicles.Add(new PublicVehicle(new Vector3(166.16, -352.73, 53.46), 0.0f, VehicleHash.Adder));
            config.PublicVehicles.Add(new PublicVehicle(new Vector3(154.16, -352.73, 43.46), 0.0f, VehicleHash.Adder));
            config.ExamsQuestions.Add(new ExamQuestion(1, "Pytanie1", "a","b","c","d"));
            config.ExamsQuestions.Add(new ExamQuestion(1, "Pytanie2", "a","b","c","d"));
            config.ExamsQuestions.Add(new ExamQuestion(1, "Pytanie3", "a","b","c","d"));


            const float xpMultiplier = 1.2f;
            const long xpStart = 20;

            config.Levels = new List<Tuple<int, long>>(100);

            config.Levels.Add(new Tuple<int, long>(0, 0));
            long prv = 0;
            long cur = 0;
            for (int i = 1; i < 100; i++)
            {
                cur = prv + (long)(xpStart * Math.Pow(xpMultiplier, (i - 1)));
                config.Levels.Add(new Tuple<int, long>(i, cur));
                prv = cur;
            }
            Globals.Mongo.Configurations.InsertOne(config);
            //config.Save("Data");
        }

        public object GetConfig(string key, object defaultValue = null)
        {
            if (config == null) return null;

            object value;
            if(config.Data.TryGetValue(key, out value))
            {
                return value;
            }
            return defaultValue;
        }

        public object this[string key]
        {
            get
            {
                if (config == null) return null;
                return GetConfig(key);
            }
        }
    }
}
