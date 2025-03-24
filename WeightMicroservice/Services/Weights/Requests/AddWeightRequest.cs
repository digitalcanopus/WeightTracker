namespace WeightTracker.Services.Weights.Requests
{
    public record AddWeightRequest
    (
        decimal WeightValue,
        DateTime Date,
        List<IFormFile> Files
    );
}
