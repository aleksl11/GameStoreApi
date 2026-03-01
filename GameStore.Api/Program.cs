using GameStore.Api.Data;
using GameStore.Api.Endpoints;
using GameStore.Api.Models;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddValidation();

builder.AddGameStoreDb();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication().AddCookie(IdentityConstants.ApplicationScheme);

builder.Services.AddIdentityCore<User>()
    .AddEntityFrameworkStores<GameStoreContext>()
    .AddApiEndpoints();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.mapGamesEndpoints();
app.mapGenresEndpoints();

app.MapIdentityApi<User>();

app.MigrateDb();

app.Run();
