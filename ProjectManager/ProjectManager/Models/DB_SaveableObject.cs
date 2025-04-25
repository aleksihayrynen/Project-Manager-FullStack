using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProjectManager.Models
{
        public class DB_SaveableObject
        {
            [BsonId]
            public ObjectId _id { get; set; }
        }
}
