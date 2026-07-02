using GameStore.Contracts;
using GameStore.Api.Dtos.Orders;
using GameStore.Api.Data;
using GameStore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MassTransit;

namespace GameStore.Api.Endpoints;

public static class OrdersEndpoints
{
    public static void MapOrdersEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/orders")
            .WithTags("Orders");

        //POST buy
        group.MapPost("/buy", async ( 
            [FromBody] BuyGameRequest request,
            GameStoreContext dbContext,
            IPublishEndpoint publishEndpoint
        ) => 
        {
            if (request.UserEmail == null)
            {
                return Results.BadRequest(new { Message = "User Email cannot be empty" });
            }
            var game = await dbContext.Games
                .Where(g => g.Id == request.GameId)
                .FirstOrDefaultAsync();
        
            if (game == null)
            {
                return Results.BadRequest(new { Message = "Game with this Id does not exist" });
            }

            var order = new Order 
            { 
                GameId = request.GameId, 
                Price = request.Price, 
                Status = "Pending payment" 
            };

            dbContext.Orders.Add(order);
            await dbContext.SaveChangesAsync();


            var message = new ProcessPaymentMessage(order.Id, order.Price, game.Name, request.UserEmail);
            await publishEndpoint.Publish(message);

            return Results.Accepted($"/orders/{order.Id}/status", new { OrderId = order.Id });
        })
        .RequireAuthorization();
        
        //GET status
        group.MapGet("/{id}/status", async (int id, GameStoreContext dbContext) =>
        {
            var order = await dbContext.Orders.FindAsync(id);
            
            if (order is null) 
                return Results.NotFound();

            return Results.Ok(new { OrderId = order.Id, order.Status });
        })
        .RequireAuthorization();

        //GET invoice
        group.MapGet("/{id}/invoice", (int id) =>
        {
            var filePath = $"/app/shared/faktura_{id}.pdf";

            if (!File.Exists(filePath))
            {
                return Results.NotFound(new { Message = "Invoice not found" });
            }

            var bytes = File.ReadAllBytes(filePath);
            return Results.File(bytes, "application/pdf", $"faktura_{id}.pdf");
        })
        .RequireAuthorization();
    }
}
