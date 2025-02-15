
namespace Anko.OrdersService.Core.Services;

public interface IWorkflowEngine
{
    Task StartOrderWorkflowFor(string orderId);

    // Task ConfirmPayment(string orderIdentifier, decimal paymentAmount);
    //
    // Task CancelOrder(string orderIdentifier);
    //
    // Task OrderReadyForDelivery(string orderIdentifier);
    //
    // Task OrderCollected(string orderIdentifier);
    //
    // Task OrderDelivered(string orderIdentifier);
}