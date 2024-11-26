using Mapster;
using Microsoft.AspNetCore.Mvc;
using OneOf;
using WeightTracker.Services.Weights;
using WeightTracker.Services.Weights.Requests;

namespace WeightTracker.Controllers
{
    public class WeightsController : BaseController
    {
        private readonly IWeightService _weightService;

        public WeightsController(IWeightService weightService) => _weightService = weightService;

        [HttpGet("~/api/weights")]
        public async Task<IActionResult> GetWeights(CancellationToken cancellationToken = default)
        {
            var result = await _weightService.GetWeightsAsync(cancellationToken);

            return result.Match<IActionResult>(
                weights => Ok(weights.Adapt<List<WeightDetailsResponse>>()),
                _ => NotFound());
        }

        [HttpGet("~/api/weights/{weightId:length(24)}")]
        public async Task<IActionResult> GetWeightById([FromRoute] string weightId,
            CancellationToken cancellationToken = default)
        {
            var result = await _weightService.GetWeightByIdAsync(weightId, cancellationToken);

            return result.Match<IActionResult>(
                weight => Ok(weight.Adapt<WeightDetailsResponse>()),
                _ => NotFound());
        }

        [HttpPost("~/api/weights")]
        public async Task<IActionResult> AddWeight([FromBody] AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _weightService.AddWeightAsync(addWeightRequest, cancellationToken);

            return result.Match<IActionResult>(
                weight => Ok(weight.Adapt<WeightDetailsResponse>()),
                _ => NotFound());
        }

        [HttpPut("~/api/weights/{weightId:length(24)}")]
        public async Task<IActionResult> EditWeight([FromRoute] string weightId,
            [FromBody] EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default)
        {
            var result = await _weightService.EditWeightAsync(weightId, editWeightRequest, cancellationToken);

            return result.Match<IActionResult>(
                weight => Ok(weight.Adapt<WeightDetailsResponse>()),
                _ => NotFound());
        }

        [HttpDelete("~/api/weights/{weightId:length(24)}/files/{fileId:length(24)}")]
        public async Task<IActionResult> DeleteFileFromWeight()
        {
            return Ok();
        }

        [HttpDelete("~/api/weights/{weightId:length(24)}")]
        public async Task<IActionResult> DeleteWeight()
        {
            return Ok();
        }
    }
}
