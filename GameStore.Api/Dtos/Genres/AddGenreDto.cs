using System.ComponentModel.DataAnnotations;

namespace GameStore.Api.Dtos.Genres;

public record AddGenreDto(
    [Required][StringLength(50)] string Name
);