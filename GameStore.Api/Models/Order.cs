using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Models;

public class Order
{
    public int Id {get; set;}

    public Game? Game {get; set;}
    public int GameId {get; set;}
    public decimal Price {get; set;}
    public string Status {get; set;} = "Unknown";
}
