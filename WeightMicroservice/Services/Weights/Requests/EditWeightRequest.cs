namespace WeightTracker.Services.Weights.Requests
{
    public record EditWeightRequest
    (
        decimal WeightValue,
        DateTime Date,
        IFormFile[]? Files,
        string UserId
    );
}
