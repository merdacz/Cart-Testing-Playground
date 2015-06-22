namespace CartService.Tests
{
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using NMoneys;

    using Xunit;

    public class CartTests
    {
        [Fact]
        public void EmptyCartTotalIsZero()
        {
            var fixture = new CartFixture()
                .WithoutElements()
                .WithDefaultCurrency(CurrencyIsoCode.USD);
            var sut = fixture.CreateSut();

            var result = sut.CalculateTotal();

            result.Should().Be(new Money(0, Currency.Usd));
        }

        [Fact]
        public void SomeItemsWithinDefaultCurrencyShouldJustSumUp()
        {
            var fixture = new CartFixture();
            fixture.AddManyProductsWithDefaultCurrency();
            var sut = fixture.CreateSut();
            var expectedTotal = fixture.Items.Sum(i => (i.Price * i.Quantity).Amount);
            var expectedCurrency = fixture.DefaultCurrency;

            var result = sut.CalculateTotal();

            result.Should().Be(new Money(expectedTotal, expectedCurrency));
        }

        [Fact]
        public void ItemsWithDifferentCurrencyShouldbeExchangedBeforeSummingUp()
        {
            Cart sut = new CartFixture()
                .AddSingleProduct(new Money(5, CurrencyIsoCode.GBP), 3)
                .AddSingleProduct(new Money(10, CurrencyIsoCode.EUR), 2);
            
            var result = sut.CalculateTotal();

            result.Should().Be(new Money(35, Currency.Usd));
        }

        [Fact]
        public void WhenAddingAnExistingItemAgainShouldMergeItWithPreviousOne()
        {
            var fixture = new CartFixture();
            fixture.AddManyProductsWithDefaultCurrency();
            
            var sut = fixture.CreateSut();
            var countBeforeAddition = sut.Items.Count();
            var existingItem = sut.Items.First();
            sut.AddItem(existingItem.ProductId, 1);

            sut.Items.Should().HaveCount(countBeforeAddition);
        }

        [Fact]
        public void WhenAddingAnExistingItemForWhichPriceHasChangedUserShouldBeNotified()
        {
            var fixture = new CartFixture();
            fixture.AddManyProductsWithDefaultCurrency();

            var sut = fixture.CreateSut();
            var existingItem = sut.Items.First();
            fixture.ChangePrice(existingItem.ProductId);
            sut.AddItem(existingItem.ProductId, 1);
    
            A.CallTo(() => fixture.UserFeedback.Send(A<string>.Ignored)).MustHaveHappened();
        }
    }
}
