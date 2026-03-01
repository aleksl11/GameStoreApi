using GameStore.Api.Data;
using GameStore.Api.Dtos.Genres;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{   
    const string GetGenreEndpoint = "GetGenre";

    public static void mapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        //GET
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres.Select(genre => new GenreDto(
                genre.Id,
                genre.Name
            ))
            .AsNoTracking()
            .ToListAsync()
        );

        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var genre = await dbContext.Genres.FindAsync(id);

            return genre is null ? Results.NotFound() : Results.Ok(
                new GenreDto(
                    genre.Id,
                    genre.Name
                )
            );
        }).WithName(GetGenreEndpoint);

        //POST
        group.MapPost("/add", async (AddGenreDto newGenre, GameStoreContext dbContext) =>
        {
            Genre genre = new()
            {
                Name = newGenre.Name
            };

            dbContext.Genres.Add(genre);

            GenreDto genreDto = new(
                genre.Id,
                genre.Name
            );

            return Results.CreatedAtRoute(GetGenreEndpoint, new {id = genre.Id}, genreDto);
        });

        //PUT
        group.MapPut("/update/{id}", async (int id, UpdateGenreDto updateGenre, GameStoreContext dbContext) =>
        {
            var existingGenre = await dbContext.Genres.FindAsync(id);

            if (existingGenre is null)
            {
                return Results.NotFound();
            }

            existingGenre.Name = updateGenre.Name;

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        });

        //DELETE    
        group.MapDelete("/delete/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Genres
                        .Where(genre => genre.Id == id)
                        .ExecuteDeleteAsync();

            return Results.NoContent();
        });
    }

}
