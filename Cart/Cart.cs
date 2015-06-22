namespace CartService
{
    using System;
    using System.Collections.Generic;

    using NMoneys;

    public class Cart
    {
        private readonly IExchangeService exchangeService;

        private readonly IUserProfileService userProfileService;

        private readonly IPricingService pricingService;

        private readonly IUserFeedback userFeedback;

        private readonly IDictionary<Guid, CartItem> items = new Dictionary<Guid, CartItem>();

        public Cart(
            IExchangeService exchangeService,
            IUserProfileService userProfileService,
            IPricingService pricingService,
            IUserFeedback userFeedback)
        {
            this.exchangeService = exchangeService;
            this.userProfileService = userProfileService;
            this.pricingService = pricingService;
            this.userFeedback = userFeedback;
        }

        public IEnumerable<CartItem> Items
        {
            get
            {
                foreach (var item in this.items.Values)
                {
                    yield return item;
                }
            }
        } 

        public void AddItem(Guid productId, int quantity)
        {
            var currentPrice = this.pricingService.GetPrice(productId);
            if (this.items.ContainsKey(productId))
            {
                var item = this.items[productId];
                item.Increase(quantity);
                if (item.LastPrice != currentPrice)
                {
                    item.UpdatePrice(currentPrice);
                    this.userFeedback.Send("LastPrice dropped or increased for " + productId);
                }
            }
            else
            {
                var item = new CartItem(productId, quantity, currentPrice);
                this.items.Add(productId, item);
            }
        }

        public Money CalculateTotal()
        {
            var targetCurrency = this.userProfileService.GetDefaultCurrency();
            var total = new Money(0, targetCurrency);
            foreach (var item in this.items.Values)
            {
                var itemTotal = item.CalculateCurrentItemTotal(this.pricingService);
                if (itemTotal.CurrencyCode == targetCurrency)
                {
                    total = total + itemTotal;
                }
                else
                {
                    var convertedAmount = this.exchangeService.Exchange(itemTotal, targetCurrency);
                    total = total + convertedAmount;
                }
            }

            return total;
        }
    }
}
