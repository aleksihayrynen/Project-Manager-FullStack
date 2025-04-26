using MongoDB.Bson;
using MongoDB.Driver;
using SharpCompress.Common;
using System.Diagnostics.Eventing.Reader;
using System.Security.AccessControl;
using System.Security.Principal;

/* Package install command on Package manager 
Install-Package MongoDB.Bson;
Install-Package MongoDB.Driver.Core;
Install-Package MongoDB.Driver
*/

namespace ProjectManager.Models.Services
{
    public static class MongoManipulator
    {
        private static string? DATABASE_NAME;
        private static string? DATABASE_ADDRESS;
        private static MongoServerAddress? address;
        private static MongoClientSettings? settings;
        private static MongoClient? client;
        private static IMongoDatabase? database;
        private static IConfiguration? config;

        public static void Initialize(IConfiguration configuration)
        {
            config = configuration;
            var sections = config.GetSection("ConnectionStrings");
            DATABASE_ADDRESS = sections.GetValue<string>("MongoAddress");
            DATABASE_NAME = sections.GetValue<string>("MongoDatabaseName");
            address = new MongoServerAddress(DATABASE_ADDRESS);
            settings = new MongoClientSettings() { Server = address };
            client = new MongoClient(settings);
            database = client.GetDatabase(DATABASE_NAME);

        }
        private static IMongoDatabase GetDB()
        {
            if (database == null)
                throw new Exception("Database is uninitialized");
            else
                return database;
        }


        public static async Task Save<T>(T entity) where T : DB_SaveableObject
        {
            string collectionName = typeof(T).Name;
            var collection = GetDB().GetCollection<T>(collectionName);
            if (entity._id == ObjectId.Empty)
            {
                await collection.InsertOneAsync(entity);
            }
            else
            {
                var filter = Builders<T>.Filter.Eq(e => e._id, entity._id);
                var options = new ReplaceOptions { IsUpsert = true };
                await collection.ReplaceOneAsync(filter, entity, options);
            }
        }

        public static async Task<T> GetObjectById<T>(T entity) where T : DB_SaveableObject
        {
            string collectionName = typeof(T).Name;
            var collection = GetDB().GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(e => e._id, entity._id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public static async Task<T> GetObjectByField<T>(string fieldName, string value) where T : DB_SaveableObject
        {
            string collectionName = typeof(T).Name;
            var collection = GetDB().GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(fieldName, value);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Retrieves a list of objects from the database that match the specified filter.
        /// Example usage:
        /// <code>
        /// Filter .Eq for single .And for multiple
        /// 
        /// var filter = Builders&lt;Expense&gt;.Filter.And(
        ///     Builders&lt;Expense&gt;.Filter.Eq("UserId", userId),
        ///     Builders&lt;Expense&gt;.Filter.Gte("Date", startDate),
        ///     Builders&lt;Expense&gt;.Filter.Lte("Date", endDate)
        /// );
        /// return await MongoManipulator.GetAllObjectsByFilter(filter);
        /// </code>
        /// </summary>
        /// <returns>A list of objects matching the filter.</returns>
        public static async Task<List<T>> GetAllObjectsByFilter<T>(FilterDefinition<T> filter) where T : DB_SaveableObject
        {
            string collectionName = typeof(T).Name;
            var collection = GetDB().GetCollection<T>(collectionName);
            return await collection.Find(filter).ToListAsync();
        }

        public static async Task<List<T>> GetAllObjects<T>() where T : DB_SaveableObject
        {
            string collectionName = typeof(T).Name;
            var collection = GetDB().GetCollection<T>(collectionName);
            return await collection.Find(_ => true).ToListAsync();
        }

        private static async Task Delete<T>(T entity) where T : DB_SaveableObject
        {
            string collectionName = typeof(T).Name;
            var collection = GetDB().GetCollection<T>(collectionName);
            var filter = Builders<T>.Filter.Eq(e => e._id, entity._id);
            await collection.DeleteOneAsync(filter);
        }

        public static async Task DeleteHelper<T>(T entity) where T : DB_SaveableObject
        {
            await Delete(entity);
        }

    }
}
