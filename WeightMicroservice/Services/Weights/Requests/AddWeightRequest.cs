namespace WeightTracker.Services.Weights.Requests
{
    public record AddWeightRequest
    (
        decimal WeightValue,
        DateTime Date,
        IFormFile[]? Files,
        string UserId
    );
}
