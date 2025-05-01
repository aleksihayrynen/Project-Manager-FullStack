using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ProjectManager.Models
{
    public class DB_SaveableObject
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public ObjectId _id { get; set; }

        //public ObjectId _oid { get { new ObjectId _id; } }

        }
}
