using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Data;

public static class DataExtentions
{
    public static void MigrateDb(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GameStoreContext>();
        dbContext.Database.Migrate();
    }

    public static void AddGameStoreDb(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<GameStoreContext>(options => options
            .UseSqlServer(
                builder.Configuration.GetConnectionString("GameStore"),
                sqlOptions => sqlOptions.EnableRetryOnFailure
                (
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null
                )    
            )
            .UseSeeding((context, _) =>
            {
                if (!context.Set<Genre>().Any())
                {
                    var iconsPath = Path.Combine(AppContext.BaseDirectory, "Data", "DefaultIcons");
                    
                    Image CreateImage(string fileName)
                    {
                        var filePath = Path.Combine(iconsPath, fileName);
                        return new Image
                        {
                            Data = File.ReadAllBytes(filePath),
                            FileExtention = Path.GetExtension(fileName),
                            UploadDate = DateTime.UtcNow,
                            Description = $"Icon for {Path.GetFileNameWithoutExtension(fileName)}"
                        };
                    }

                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Fighting", Image = CreateImage("fighting_icon.png") },
                        new Genre { Name = "Action", Image = CreateImage("action_icon.png") },
                        new Genre { Name = "Adventure", Image = CreateImage("adventure_icon.png") },
                        new Genre { Name = "Role-Playing Game (RPG)", Image = CreateImage("rpg_icon.png") },
                        new Genre { Name = "Platformer", Image = CreateImage("platformer_icon.png") },
                        new Genre { Name = "First Person Shooter (FPS)", Image = CreateImage("fps_icon.png") },
                        new Genre { Name = "Puzzle", Image = CreateImage("puzzle_icon.png") },
                        new Genre { Name = "Sandbox", Image = CreateImage("sandbox_icon.png") },
                        new Genre { Name = "Real-Time Strategy (RTS)", Image = CreateImage("rts_icon.png") }
                    );

                    context.SaveChanges();
                }

                if (!context.Set<Game>().Any()) 
                {
                    var gamesSqlPath = Path.Combine(AppContext.BaseDirectory, "Data", "Games.sql");
                    
                    if (File.Exists(gamesSqlPath))
                    {
                        var sql = File.ReadAllText(gamesSqlPath);
                        context.Database.ExecuteSqlRaw(sql);
                    }
                    else
                    {
                        Console.WriteLine($"WARNING: Seed file not found at {gamesSqlPath}");
                    }
                }
            })
        );
    }
}
