namespace GameStore.Api.Models;

public class Image
{
    public int Id { get; set; }
    public string? Description { get; set; }
    public required byte[] Data { get; set; }
    public required string FileExtention { get; set; }
    public DateTime UploadDate { get; set; }
}
