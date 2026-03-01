using GameStore.Api.Data;
using GameStore.Api.Dtos.Games;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameEndpoint = "GetGame";
    // private static readonly List<GameDetailsDto> games = [
    //     new (1, "Street Fighter II", 1, 19.99M, new DateOnly(1992, 7, 15)),
    //     new (2, "Super Mario World", 5, 14.99M, new DateOnly(1990, 11, 21)),
    //     new (3, "The Legend of Zelda: Ocarina of Time", 3, 29.99M, new DateOnly(1998, 11, 21)),
    //     new (4, "Doom", 6, 9.99M, new DateOnly(1993, 12, 10)),
    //     new (5, "Final Fantasy VII", 4, 15.99M, new DateOnly(1997, 1, 31)),
    //     new (6, "Tetris", 7, 4.99M, new DateOnly(1989, 6, 14)),
    //     new (7, "Minecraft", 8, 26.95M, new DateOnly(2011, 11, 18)),
    //     new (8, "Half-Life 2", 6, 9.99M, new DateOnly(2004, 11, 16)),
    //     new (9, "Castlevania: Symphony of the Night", 4, 19.99M, new DateOnly(1997, 3, 20)),
    //     new (10, "Starcraft", 9, 14.99M, new DateOnly(1998, 3, 31))
    // ]; 

    public static void mapGamesEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/games");
        // GET
        group.MapGet("/", async (GameStoreContext dbContext) => 
            await dbContext.Games
            .Include(game => game.Genre)
            .Select(game => new GameSummaryDto(
                game.Id,
                game.Name,
                game.Genre!.Name,
                game.Price,
                game.ReleaseDate
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
                    game.ReleaseDate
                )
            );
        }).WithName(GetGameEndpoint);

        //POST
        group.MapPost("/add", async (AddGameDto newGame, GameStoreContext dbContext) =>
        {
            
            Game game = new()
            {
                Name = newGame.Name,
                GenreId = newGame.GenreId,
                Price = newGame.Price,
                ReleaseDate = newGame.ReleaseDate
            };

            dbContext.Games.Add(game);
            await dbContext.SaveChangesAsync();

            GameDetailsDto gameDto = new(
                game.Id,
                game.Name,
                game.GenreId,
                game.Price,
                game.ReleaseDate
            );

            return Results.CreatedAtRoute(GetGameEndpoint, new {id = game.Id}, gameDto);
        }).RequireAuthorization();

        //PUT
        group.MapPut("/update/{id}", async (int id, UpdateGameDto updatedGame, GameStoreContext dbContext) =>
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

            await dbContext.SaveChangesAsync();

            return Results.NoContent();
        }).RequireAuthorization();

        //DELETE
        group.MapDelete("/delete/{id}", async (int id, GameStoreContext dbContext) =>
        {
            await dbContext.Games
                        .Where(game => game.Id == id)
                        .ExecuteDeleteAsync();

            return Results.NoContent();
        }).RequireAuthorization();
    }
}
