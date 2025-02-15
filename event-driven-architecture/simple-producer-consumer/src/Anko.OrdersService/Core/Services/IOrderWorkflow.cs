namespace Anko.OrdersService.Core.Services;

public interface IOrderWorkflow
{

    Task SubmitOrder();
    /*Task WaitForPotentialCancellation();
    
    Task CancelOrder();

    Task TakePayment();

    Task ReceivePaymentFor(decimal paymentAmount);*/
}