namespace WeightMicroservice.Services.Files.Requests
{
    public record SaveFileRequest
    (
        IFormFile File,
        string FileName
    );
}
