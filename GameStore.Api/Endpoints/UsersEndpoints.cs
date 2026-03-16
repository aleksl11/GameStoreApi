using System.Security.Claims;
using GameStore.Api.Dtos.Users;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace GameStore.Api.Endpoints;

public static class UsersEndpoints
{
    public static void MapUsersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/users")
            .WithTags("Users");

        //GET
        group.MapGet("/me", async (ClaimsPrincipal user, UserManager<User> userManager) =>
        {
            // user here is the same as context.User
            var appUser = await userManager.GetUserAsync(user);

            if (appUser is null)
                return Results.Unauthorized();

            var roles = await userManager.GetRolesAsync(appUser);

            return Results.Ok(new UserDto(
                appUser.UserName ?? "no username",
                appUser.Email ?? "no email",
                roles.FirstOrDefault() ?? "User"
            ));
        })
        .RequireAuthorization()
        .WithName("GetCurrentUser");
    }
}
