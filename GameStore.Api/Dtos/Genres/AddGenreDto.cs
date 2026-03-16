using System.ComponentModel.DataAnnotations;
using GameStore.Api.Dtos.Images;

namespace GameStore.Api.Dtos.Genres;

public class AddGenreDto
{
    [Required]
    [StringLength(50)] 
    public string Name { get; set; } = string.Empty;
    public ImageDto? Image { get; set; }
}