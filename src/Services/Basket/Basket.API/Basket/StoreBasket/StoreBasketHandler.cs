namespace Basket.API.Basket.StoreBasket;

public record StoreBasketCommand(ShoppingCart Cart) : ICommand<StoreBasketResult>;

public record StoreBasketResult(string UserName);

public class StoreBasketCommandValidator : AbstractValidator<StoreBasketCommand>
{
    public StoreBasketCommandValidator()
    {
        RuleFor(x => x.Cart).NotEmpty().WithMessage("Cart cannot be empty.");
        RuleFor(x => x.Cart.UserName).NotEmpty().WithMessage("UserName is required.");
    }
};

public class StoreBasketCommandHandler(
    IBasketRepository repository,
    DiscountProtoService.DiscountProtoServiceClient proto,
    ILogger<StoreBasketCommandHandler> logger)
    : ICommandHandler<StoreBasketCommand, StoreBasketResult>
{
    public async Task<StoreBasketResult> Handle(StoreBasketCommand command, CancellationToken cancellationToken)
    {
        await DeductDiscount(command.Cart, cancellationToken);

        var savedCart = await repository.StoreBasketAsync(command.Cart, cancellationToken);

        return new StoreBasketResult(savedCart.UserName);
    }

    private async Task DeductDiscount(ShoppingCart cart, CancellationToken cancellationToken)
    {
        foreach (var item in cart.Items)
        {
            int discountAmount = 0;
            
            try
            {
                var discount = await proto.GetDiscountAsync(new GetDiscountRequest { ProductName = item.ProductName },
                    deadline: DateTime.UtcNow.AddMilliseconds(500), cancellationToken: cancellationToken);

                discountAmount = discount.Amount;
            }
            catch (Grpc.Core.RpcException ex)
            {
                logger.LogWarning(ex,
                    "Failed to retrieve discount for product {ProductName}. Applying zero discount.",
                    item.ProductName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex,
                    "Unexpected error occurred while retrieving discount for product {ProductName}.",
                    item.ProductName);
            }

            item.Price -= discountAmount;
        }
    }
}