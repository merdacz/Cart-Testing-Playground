namespace CartService
{
    using System;

    using NMoneys;

    public class CartItem
    {
        public CartItem(Guid productId, int initialQuantity, Money lastPrice)
        {
            this.ProductId = productId;
            this.LastPrice = lastPrice;
            this.Quantity = initialQuantity;
        }

        public Guid ProductId { get; private set; }

        public Money LastPrice { get; private set; }

        public int Quantity { get; private set; }

        public Money CalculateCurrentItemTotal(IPricingService pricingService)
        {
            var currentPrice = pricingService.GetPrice(this.ProductId);
            this.LastPrice = currentPrice;
            return this.LastPrice * this.Quantity;
        }

        public void Increase(int delta)
        {
            this.Quantity = this.Quantity + delta;
        }

        public void Decrease(int delta)
        {
            if (this.Quantity < delta)
            {
                this.Quantity = 0;
            }
            else
            {
                this.Quantity = this.Quantity - delta;
            }
        }

        public void UpdatePrice(Money currentPrice)
        {
            this.LastPrice = currentPrice;
        }
    }
}