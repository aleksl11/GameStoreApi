using GameStore.Api.Data;
using GameStore.Api.Dtos.Images;
using GameStore.Api.Dtos.Games;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpoint = "GetGame";

    public static void MapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games")
            .WithTags("Games");

        // GET
        group.MapGet("/", async (GameStoreContext dbContext) => 
            await dbContext.Games
            .Include(game => game.Genre)
            .Select(game => new GameSummaryDto(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate,
                game.ImageId
            ))
            .AsNoTracking()
            .ToListAsync()
        );

        group.MapGet("/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games.FindAsync(id);

            return game is null ?  Results.NotFound() : Results.Ok(
                new GameDetailsDto(
                    game.Id,
                    game.Name,
                    game.GenreId,
                    game.Price,
                    game.ReleaseDate,
                    game.ImageId
                )
            );
        }).WithName(GetGameEndpoint);

        //POST
        group.MapPost("/add", async ([FromForm] AddGameDto newGame, GameStoreContext dbContext) =>
        {
            var newImage = newGame.Image;
            var imageId = (int?)null;
            if (newImage != null && newImage.File.Length > 0)
            {
                imageId = await ImagesEndpoints.SaveImageToDatabase(newImage, dbContext);
            }
            
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate,
                ImageId = imageId
            };

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate,
                game.ImageId
            );

            return Results.CreatedAtRoute(GetGameEndpoint, new {id = game.Id}, gameDto);
        }).RequireAuthorization("AdminOnly")
        .DisableAntiforgery();

        //PUT
        group.MapPut("/update/{id}", async (int id, [FromForm] UpdateGameDto updatedGame, GameStoreContext dbContext) =>
        {
            var existingGame = await dbContext.Games.FindAsync(id);

            if (existingGame is null)
            {
                return Results.NotFound();
            }

            existingGame.Name = updatedGame.Name;
            existingGame.GenreId = updatedGame.GenreId;
            existingGame.Price = updatedGame.Price;
            existingGame.ReleaseDate = updatedGame.ReleaseDate;

            var newImage = updatedGame.Image;
            if (newImage != null && newImage.File.Length > 0)
            {
                var imageId = await ImagesEndpoints.SaveImageToDatabase(newImage, dbContext);
                await  dbContext.Images
                        .Where(image => image.Id == existingGame.ImageId)
                        .ExecuteDeleteAsync();

                existingGame.ImageId = imageId;
            }


            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization("AdminOnly")
        .DisableAntiforgery();

        //DELETE
        group.MapDelete("/delete/{id}", async (int id, GameStoreContext dbContext) =>
        {
            var game = await dbContext.Games
                .Include(g => g.Image)
                .FirstOrDefaultAsync(g => g.Id == id);

            if (game is null)
                return Results.NotFound();

            if (game.Image != null)
                dbContext.Images.Remove(game.Image);

            dbContext.Games.Remove(game);
            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization("AdminOnly");
    }
}
