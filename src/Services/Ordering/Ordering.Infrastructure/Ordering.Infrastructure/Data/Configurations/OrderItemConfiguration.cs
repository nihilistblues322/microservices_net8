﻿namespace Ordering.Infrastructure.Data.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.HasKey(o => o.Id);
        builder.Property(o => o.Id)
            .HasConversion(orderItemId => orderItemId.Value, dbId => OrderItemId.Of(dbId));
        builder.HasOne<Product>().WithMany().HasForeignKey(o => o.ProductId);
        builder.Property(o => o.Price).IsRequired();
        builder.Property(o => o.Quantity).IsRequired();
    }
}