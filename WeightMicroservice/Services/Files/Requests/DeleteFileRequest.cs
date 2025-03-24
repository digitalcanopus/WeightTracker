namespace WeightMicroservice.Services.Files.Requests
{
    public record DeleteFileRequest
    (
        string FileName,
        string FileExtension
    );
}
