using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace UserMicroservice.Entities
{
    public class User
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("username")]
        public string Username { get; set; } = null!;

        [BsonElement("password_hash")]
        public string PasswordHash { get; set; } = null!;
    }
}
