using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WeightTracker.Entities
{
    public class Weight
    {
        [BsonId]
        [BsonElement("_id"), BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("date")]
        public DateTime Date { get; set; }

        [BsonElement("weight"), BsonRepresentation(BsonType.Double)]
        public decimal WeightValue { get; set; }

        [BsonElement("files")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string[]? Files { get; set; }

        [BsonElement("user")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; } = null!;
    }
}
