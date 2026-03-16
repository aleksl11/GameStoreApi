using GameStore.Api.Data;
using GameStore.Api.Dtos.Genres;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GenresEndpoints
{   
    const string GetGenreEndpoint = "GetGenre";

    public static void MapGenresEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/genres");

        //GET
        group.MapGet("/", async (GameStoreContext dbContext) =>
            await dbContext.Genres.Select(genre => new GenreDto(
                genre.Id,
                genre.Name,
                genre.ImageId
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
                    genre.Name,
                    genre.ImageId
                )
            );
        }).WithName(GetGenreEndpoint);

        //POST
        group.MapPost("/add", async (AddGenreDto newGenre, GameStoreContext dbContext) =>
        {
            var newImage = newGenre.Image;
            var imageId = (int?)null;
            if (newImage != null && newImage.File.Length > 0)
            {
                imageId = await ImagesEndpoints.SaveImageToDatabase(newImage, dbContext);
            }

            Genre genre = new()
            {
                Name = newGenre.Name,
                ImageId = imageId
            };

            dbContext.Genres.Add(genre);
            await dbContext.SaveChangesAsync();

            GenreDto genreDto = new(
                genre.Id,
                genre.Name,
                genre.ImageId
            );

            return Results.CreatedAtRoute(GetGenreEndpoint, new {id = genre.Id}, genreDto);
        }).RequireAuthorization("AdminOnly")
        .DisableAntiforgery();

        //PUT
        group.MapPut("/update/{id}", async (int id, UpdateGenreDto updateGenre, GameStoreContext dbContext) =>
        {
            var existingGenre = await dbContext.Genres.FindAsync(id);

            if (existingGenre is null)
            {
                return Results.NotFound();
            }

            existingGenre.Name = updateGenre.Name;

            var newImage = updateGenre.Image;
            if (newImage != null && newImage.File.Length > 0)
            {
                var imageId = await ImagesEndpoints.SaveImageToDatabase(newImage, dbContext);
                await  dbContext.Images
                        .Where(image => image.Id == existingGenre.ImageId)
                        .ExecuteDeleteAsync();
                
                existingGenre.ImageId = imageId;
            }

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization("AdminOnly");

        //DELETE    
        group.MapDelete("/delete/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var genre = await dbContext.Genres
                .Include(g => g.Image)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (genre is null)
                return Results.NotFound();

            if (genre.Image != null)
                dbContext.Images.Remove(genre.Image);

            dbContext.Genres.Remove(genre);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization("AdminOnly");
    }

}
