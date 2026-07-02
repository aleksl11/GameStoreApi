namespace GameStore.Api.Dtos.Orders;

public record class BuyGameRequest
{
    public int GameId { get; set; }
    public decimal Price { get; set; }
    public string? UserEmail { get; set;}
}