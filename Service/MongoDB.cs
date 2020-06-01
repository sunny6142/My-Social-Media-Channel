using Microsoft.AspNetCore.Authentication;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Authentication;
using My_Social_Media_Channel.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;

namespace My_Social_Media_Channel.Service
{
    public class MongoDB
    {
        private IMongoDatabase db;

        public const string DefaultDatabase = "MySocialMediaChannel";

        public string table { get; set; }

        public MongoDB(string table, string database = DefaultDatabase)
        {
            string connectionString = "mongodb+srv://sunny6142:Qwerty123@maddycluster-iqpjw.mongodb.net";
            var client = new MongoClient(connectionString);
            db = client.GetDatabase(database);
            this.table = table;
        }
        public void InsertRecord<T>(T record)
        {
            var collection = db.GetCollection<T>(table);
            collection.InsertOne(record);
        }

        public List<T> LoadRecord<T>()
        {
            var collection = db.GetCollection<T>(table);

            return collection.Find(new BsonDocument()).ToList();
        }

        public T LoadRecordById<T>(Guid id)
        {
            var collection = db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq("_id", id);

            return collection.Find(filter).FirstOrDefault();
        }
        public List<T> LoadRecordByCondition<T>(string where, Guid condition)
        {
            var collection = db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq(where, condition);

            return collection.Find(filter).ToList();
        }

        public T LoadUserInfo<T>(User user)
        {
            var collection = db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq("Email", user.Email);

            return collection.Find(filter).FirstOrDefault();
        }

        public void UpdateRecordById<T>(Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var option = new ReplaceOptions();
            option.IsUpsert = true;

            var result = collection.ReplaceOne(new BsonDocument("_id", id),
                                        record, new ReplaceOptions { IsUpsert = true});
        }
        public void DeleteRecordById<T>(Guid id, T record)
        {
            var collection = db.GetCollection<T>(table);

            var filter = Builders<T>.Filter.Eq("_id", id);

            collection.DeleteOne(filter);
        }

    }
}