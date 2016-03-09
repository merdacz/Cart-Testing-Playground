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

        public IEnumerable<CartItem> Items => this.items.Values;

        public void AddItem(Guid productId, int quantity)
        {
            var currentPrice = this.pricingService.GetPrice(productId);
            if (this.items.ContainsKey(productId))
            {
                this.UpdateExistingItem(productId, quantity, currentPrice);
            }
            else
            {
                this.CreateNewItem(productId, quantity, currentPrice);
            }
        }

        public Money CalculateTotal()
        {
            var targetCurrency = this.userProfileService.GetDefaultCurrency();
            var total = new Money(0, targetCurrency);
            foreach (var item in this.items.Values)
            {
                var itemTotal = item.CalculateCurrentItemTotal(this.pricingService);
                Money convertedAmount = ConvertToCurrency(itemTotal, targetCurrency);

                total = total + convertedAmount;
            }

            return total;
        }

        private Money ConvertToCurrency(Money itemTotal, CurrencyIsoCode targetCurrency)
        {
            if (itemTotal.CurrencyCode == targetCurrency)
            {
                return itemTotal;
            }
            else
            {
                return this.exchangeService.Exchange(itemTotal, targetCurrency);
            }
        }

        private void CreateNewItem(Guid productId, int quantity, Money currentPrice)
        {
            var item = new CartItem(productId, quantity, currentPrice);
            this.items.Add(productId, item);
        }

        private void UpdateExistingItem(Guid productId, int quantity, Money currentPrice)
        {
            var item = this.items[productId];
            item.Increase(quantity);
            if (item.LastPrice != currentPrice)
            {
                item.UpdatePrice(currentPrice);
                this.userFeedback.Send("LastPrice dropped or increased for " + productId);
            }
        }

    }
}
