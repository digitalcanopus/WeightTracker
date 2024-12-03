namespace ApiGateway.Models.Weights
{
    public record EditWeightRequest
    (
        decimal WeightValue,
        DateTime Date,
        string UserId
    );
}
