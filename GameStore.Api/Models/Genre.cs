namespace GameStore.Api.Models;

public class Genre
{
    public int Id {get; set;}
    public required string Name {get; set;}
    public Image? Image {get; set;}
    public int? ImageId {get; set;}
}
