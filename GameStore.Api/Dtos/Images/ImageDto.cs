namespace GameStore.Api.Dtos.Images;

public record class ImageDto
{
    public string Description { get; init; } = string.Empty;
    public IFormFile File { get; init; } = null!;
}
