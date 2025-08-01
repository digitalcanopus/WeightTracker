﻿namespace ApiGateway.Models.Weights
{
    public record AddWeightRequest 
    (
        decimal WeightValue,
        DateTime Date,
        List<IFormFile> Files
    );
}
