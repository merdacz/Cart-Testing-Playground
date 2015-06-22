namespace CartService
{
    using System;

    using NMoneys;

    public interface IPricingService
    {
        Money GetPrice(Guid productId);
    }
}