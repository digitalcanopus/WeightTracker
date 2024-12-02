using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WeightTracker.Entities
{
    public class File
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("original_name")]
        public string OriginalName { get; set; } = null!;

        [BsonElement("extension")]
        public string Extension { get; set; } = null!;
    }
}
