using WeightMicroservice.Services.Files.Models;

namespace WeightMicroservice.Services.Weights.Responses
{
    public record WeightDetailsResponse
    (
        string Id,
        decimal WeightValue,
        DateTime Date,
        List<FileDetailsDto>? Files
    );
}
