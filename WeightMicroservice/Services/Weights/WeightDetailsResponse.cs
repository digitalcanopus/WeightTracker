namespace WeightTracker.Services.Weights
{
    public record WeightDetailsResponse
    (
        string Id,
        decimal WeightValue,
        DateTime Date,
        string[] Files,
        string UserId
    );
}
