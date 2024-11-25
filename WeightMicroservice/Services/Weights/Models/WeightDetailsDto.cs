namespace WeightTracker.Services.Weights.Models
{
    public record WeightDetailsDto
    (
        string Id,
        decimal WeightValue,
        DateTime Date,
        string[]? Files,
        string UserId
    );
}
