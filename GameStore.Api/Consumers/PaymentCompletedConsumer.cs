using MassTransit;
using GameStore.Contracts;
using GameStore.Api.Data;

namespace GameStore.Api.Consumers;

public class PaymentCompletedConsumer : IConsumer<PaymentCompletedEvent>
{
    private readonly GameStoreContext _dbContext;
    private readonly ILogger<PaymentCompletedConsumer> _logger;

    public PaymentCompletedConsumer(GameStoreContext dbContext, ILogger<PaymentCompletedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentCompletedEvent> context)
    {
        var msg = context.Message;
        
        var order = await _dbContext.Orders.FindAsync(msg.OrderId);
        
        if (order == null)
        {
            _logger.LogWarning($"API: order not found {msg.OrderId}");
            return;
        }

        order.Status = msg.IsSuccess ? "Finished" : "Canceled";
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation($"API: Order number {msg.OrderId} changed its status to {order.Status}");
    }
}