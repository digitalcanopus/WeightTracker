using WeightMicroservice.Services.Weights.Models;

namespace WeightTracker.Services.Weights.Models
{
    public record WeightDetailsDto
    {
        public string Id { get; set; } = null!;
        public decimal WeightValue { get; set; }
        public DateTime Date { get; set; }
        public List<FileDetailsDto>? Files { get; set; }
        public string UserId { get; set; } = null!;
    }
}
