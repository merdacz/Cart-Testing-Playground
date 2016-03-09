namespace CartService.Tests
{
    using FluentAssertions;

    using NMoneys;

    using Xunit;

    public class Cart_CalculateTotal
    {
        private readonly CartFixture fixture = new CartFixture();

        [Fact]
        public void Empty_cart_total__Should_be_zero()
        {
            Cart sut = fixture.WithoutElements();
            
            var result = sut.CalculateTotal();

            result.Should().Be(new Money(0, Currency.Usd));
        }

        [Fact]
        public void Item_in_default_currency_price__Should_match_total()
        {
            var itemPrice = new Money(10, CurrencyIsoCode.USD);
            Cart sut = fixture
                .WithDefaultCurrency(CurrencyIsoCode.USD)
                .WithProduct(itemPrice);
            
            var result = sut.CalculateTotal();

            result.Should().Be(itemPrice);
        }

        [Fact]
        public void Items_with_non_default_currency__Should_be_exchanged_before_summing_up()
        {
            Cart sut = fixture
                .WithProduct(new Money(5, CurrencyIsoCode.GBP), 3)
                .WithProduct(new Money(10, CurrencyIsoCode.EUR), 2);
            
            var result = sut.CalculateTotal();

            result.Should().Be(new Money(35, Currency.Usd));
        }
    }
}
