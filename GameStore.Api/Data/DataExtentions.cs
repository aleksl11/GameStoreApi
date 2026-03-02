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
        // var connString = builder.Configuration.GetConnectionString("GameStore");
        // builder.Services.AddSqlite<GameStoreContext>(
        //     connString,
        //     optionsAction: options => options.UseSeeding((context, _) =>
        //     {
        //         if (!context.Set<Genre>().Any())
        //         {
        //             context.Set<Genre>().AddRange(
        //                 new Genre { Name = "Fighting" },
        //                 new Genre { Name = "Action" },
        //                 new Genre { Name = "Adventure" },
        //                 new Genre { Name = "Role-Playing Game (RPG)" },
        //                 new Genre { Name = "Platformer" },
        //                 new Genre { Name = "First Person Shooter (FPS)" },
        //                 new Genre { Name = "Puzzle" },
        //                 new Genre { Name = "Sandbox" },
        //                 new Genre { Name = "Real-Time Strategy" }
        //             );

        //             context.SaveChanges();
        //         }
        //     })
        // );
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
                    context.Set<Genre>().AddRange(
                        new Genre { Name = "Fighting" },
                        new Genre { Name = "Action" },
                        new Genre { Name = "Adventure" },
                        new Genre { Name = "Role-Playing Game (RPG)" },
                        new Genre { Name = "Platformer" },
                        new Genre { Name = "First Person Shooter (FPS)" },
                        new Genre { Name = "Puzzle" },
                        new Genre { Name = "Sandbox" },
                        new Genre { Name = "Real-Time Strategy" }
                    );

                    context.SaveChanges();
                }
            })
        );
    }
}
