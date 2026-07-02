using MassTransit;
using GameStore.Contracts;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

public class PaymentProcessorConsumer : IConsumer<ProcessPaymentMessage>
{
    private readonly ILogger<PaymentProcessorConsumer> _logger;

    public PaymentProcessorConsumer(ILogger<PaymentProcessorConsumer> logger /*, AppDbContext dbContext */)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<ProcessPaymentMessage> context)
    {
        var msg = context.Message;
        _logger.LogInformation($"Worker: Began payment for order number: {msg.OrderId}");

        // Payment simulation
        await Task.Delay(300000); 
        bool isSuccess = Random.Shared.NextDouble() > 0.1;

        if (isSuccess)
        {
            _logger.LogInformation($"Worker: Payment accepted. Generating invoice...");
            
            string filePath = $"/app/shared/faktura_{msg.OrderId}.pdf";

            Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.Header().Text($"Faktura dla zamówienia #{msg.OrderId}").SemiBold().FontSize(36);
                    page.Content().Column(x =>
                    {
                        x.Item().Text($"Gra: {msg.GameTitle}");
                        x.Item().Text($"Kwota: {msg.Amount} PLN");
                        x.Item().Text($"Email kupującego: {msg.UserEmail}");
                    });
                });
            }).GeneratePdf(filePath);
            
            await context.Publish(new PaymentCompletedEvent(msg.OrderId, true));
            _logger.LogInformation($"Worker: Invoice generated");

        }
        else
        {
            _logger.LogWarning($"Worker: Payment rejected");
            await context.Publish(new PaymentCompletedEvent(msg.OrderId, false));
        }
    }
}