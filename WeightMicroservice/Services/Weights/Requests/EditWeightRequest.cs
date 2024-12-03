namespace WeightTracker.Services.Weights.Requests
{
    public record EditWeightRequest
    (
        decimal WeightValue,
        DateTime Date,
        string UserId
    );
}
