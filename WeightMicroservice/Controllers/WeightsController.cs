using Mapster;
using Microsoft.AspNetCore.Mvc;
using WeightTracker.Services.Weights;
using WeightTracker.Services.Weights.Requests;
using WeightMicroservice.Services.Weights.Responses;
using WeightMicroservice.Services.Weights.Requests;

namespace WeightTracker.Controllers
{
    public class WeightsController : BaseController
    {
        private readonly IWeightService _weightService;

        public WeightsController(IWeightService weightService)
        {
            _weightService = weightService;
        }

        [HttpGet("~/api/weights")]
        public async Task<IActionResult> GetWeights(CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.GetWeightsAsync(userId!, cancellationToken);

            return Ok(result.Adapt<List<WeightDetailsResponse>>());
        }

        [HttpGet("~/api/weights/{weightId:length(24)}")]
        public async Task<IActionResult> GetWeightById([FromRoute] string weightId,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.GetWeightByIdAsync(userId!, weightId, cancellationToken);

            return result.Match<IActionResult>(
                weight => Ok(weight.Adapt<WeightDetailsResponse>()),
                _ => NotFound());
        }

        [HttpPost("~/api/weights")]
        public async Task<IActionResult> AddWeight([FromForm] AddWeightRequest addWeightRequest,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.AddWeightAsync(userId!, addWeightRequest, cancellationToken);

            return result.Match<IActionResult>(
                weightId => Ok(new { weightId }),
                _ => NotFound());
        }

        [HttpPut("~/api/weights/{weightId:length(24)}")]
        public async Task<IActionResult> EditWeight([FromRoute] string weightId,
            [FromBody] EditWeightRequest editWeightRequest,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.EditWeightAsync(userId!, weightId, editWeightRequest, cancellationToken);

            return result.Match<IActionResult>(
                weightId => Ok(new { weightId }),
                _ => NotFound());
        }

        [HttpPost("~/api/weights/{weightId:length(24)}/files")]
        public async Task<IActionResult> AddFileToWeight([FromRoute] string weightId,
            [FromForm] AddFileRequest addFileRequest,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.AddFileToWeightAsync(userId!, weightId, addFileRequest, cancellationToken);

            return result.Match<IActionResult>(
                fileId => Ok(new { fileId }),
                _ => NotFound());
        }

        [HttpDelete("~/api/weights/{weightId:length(24)}/files/{fileId:length(24)}")]
        public async Task<IActionResult> DeleteFileFromWeight([FromRoute] string weightId,
            [FromRoute] string fileId,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.DeleteFileFromWeightAsync(userId!, weightId, fileId, cancellationToken);

            return result.Match<IActionResult>(
                fileId => Ok(new { fileId }),
                _ => NotFound());
        }

        [HttpDelete("~/api/weights/{weightId:length(24)}")]
        public async Task<IActionResult> DeleteWeight([FromRoute] string weightId,
            CancellationToken cancellationToken = default)
        {
            if (!Request.Headers.TryGetValue("X-User-Id", out var userId))
            {
                return BadRequest("User ID header is missing.");
            }

            var result = await _weightService.DeleteWeightAsync(userId!, weightId, cancellationToken);

            return result.Match<IActionResult>(
                weightId => Ok(new { weightId }),
                _ => NotFound());
        }
    }
}
