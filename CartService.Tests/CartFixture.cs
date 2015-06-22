namespace CartService.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using NMoneys;

    public class CartFixture
    {
        private readonly FakeExchangeService exchangeService = new FakeExchangeService();

        private readonly IUserProfileService userProfileService = A.Fake<IUserProfileService>();

        private readonly IPricingService pricingService = A.Fake<IPricingService>();

        private readonly IUserFeedback userFeedback = A.Fake<IUserFeedback>();

        private readonly IList<CartItemFixture> itemsFixtures = new List<CartItemFixture>();

        private CurrencyIsoCode defaultCurrency = CurrencyIsoCode.USD;

        public CurrencyIsoCode DefaultCurrency
        {
            get
            {
                return this.defaultCurrency;
            }
        }

        public IEnumerable<CartItemFixture> Items
        {
            get
            {
                return this.itemsFixtures.Select(item => item);
            }
        }

        public IUserFeedback UserFeedback
        {
            get
            {
                return this.userFeedback;
            }
        }

        public static implicit operator Cart(CartFixture fixture)
        {
            return fixture.CreateSut();
        }

        public CartFixture WithoutElements()
        {
            this.itemsFixtures.Clear();
            return this;
        }

        public CartFixture AddManyProductsWithDefaultCurrency()
        {
            int many = 3;
            for (int i = 1; i <= many; i++)
            {
                this.itemsFixtures.Add(new CartItemFixture(Guid.NewGuid(), 1, new Money(5 * i, this.defaultCurrency)));
            }

            return this;
        }

        public CartFixture AddSingleProduct(Money price, decimal exchangeRate = 1)
        {
            this.itemsFixtures.Add(new CartItemFixture(Guid.NewGuid(), 1, price));
            this.exchangeService.DefineExchangeRate(price.CurrencyCode, this.defaultCurrency, exchangeRate);
            
            return this;
        }

        public CartFixture WithDefaultCurrency(CurrencyIsoCode currency)
        {
            this.defaultCurrency = currency;

            return this;
        }

        public void ChangePrice(Guid productId)
        {
            var item = this.itemsFixtures.FirstOrDefault(i => i.ProductId == productId);
            if (item == null)
            {
                throw new InvalidOperationException("Attempted to change the price for the non-existing product. ");
            }

            var newPrice = item.Price + new Money(1, item.Price.CurrencyCode);
            item.UpdatePrice(newPrice);

            A.CallTo(() => this.pricingService.GetPrice(productId)).Returns(newPrice);
        }

        public Cart CreateSut()
        {
            A.CallTo(() => this.userProfileService.GetDefaultCurrency()).Returns(this.defaultCurrency);

            var result = new Cart(this.exchangeService, this.userProfileService, this.pricingService, this.userFeedback);
            foreach (var item in this.itemsFixtures)
            {
                var productId = item.ProductId;
                A.CallTo(() => this.pricingService.GetPrice(productId)).Returns(item.Price);
                result.AddItem(item.ProductId, item.Quantity);
            }

            return result;
        }

        public class CartItemFixture
        {
            public CartItemFixture(Guid productId, int quantity, Money price)
            {
                this.ProductId = productId;
                this.Price = price;
                this.Quantity = quantity;
            }

            public Guid ProductId { get; private set; }

            public Money Price { get; private set; }

            public int Quantity { get; private set; }

            public void UpdatePrice(Money newPrice)
            {
                this.Price = newPrice;
            }
        }
    }
}