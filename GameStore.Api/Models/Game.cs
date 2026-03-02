using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Models;

public class Game
{
    public int Id {get; set;}

    public required string Name {get; set;}
    public Genre? Genre {get; set;}
    public int GenreId {get; set;}

    [Precision(18,2)]
    public required decimal Price {get; set;}
    public required DateOnly ReleaseDate {get; set;}
}
