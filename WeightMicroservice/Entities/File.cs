using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WeightTracker.Entities
{
    public class File
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        [BsonElement("file_path")]
        public string FilePath { get; set; } = null!;
    }
}
