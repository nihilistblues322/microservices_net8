﻿namespace Ordering.Application.Orders.Commands.CreateOrder;

public record CreateOrderCommand(OrderDto Order) : ICommand<CreateOrderResult>;

public record CreateOrderResult(Guid Id);

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Order.OrderName).NotEmpty().WithMessage("Name is required.");
        RuleFor(x => x.Order.CustomerId).NotNull().WithMessage("Customer is required.");
        RuleFor(x => x.Order.OrderItems).NotEmpty().WithMessage("OrderItems is required.");
    }
}