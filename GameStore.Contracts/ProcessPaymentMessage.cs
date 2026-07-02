namespace GameStore.Contracts;

public record ProcessPaymentMessage(
    int OrderId, 
    decimal Amount, 
    string GameTitle,
    string UserEmail
);

public record PaymentCompletedEvent(
    int OrderId, 
    bool IsSuccess
);