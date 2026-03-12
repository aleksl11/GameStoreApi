using System.ComponentModel.DataAnnotations;
using GameStore.Api.Dtos.Images;

namespace GameStore.Api.Dtos.Games;

public class AddGameDto
{
    [Required]
    [StringLength(50)] 
    public string Name { get; set; } = string.Empty;

    public int GenreId { get; set; }

    [Range(1, 500)] 
    public decimal Price { get; set; }

    [Required] 
    public DateOnly ReleaseDate { get; set; }

    // This being nullable now works correctly with [FromForm]
    public ImageDto? Image { get; set; }
}