namespace CartService.Tests
{
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Xunit;

    public class Cart_AddItem
    {
        private readonly CartFixture fixture = new CartFixture();
            
        [Fact]
        public void When_adding_an_existing_item_again__Should_merge_it_with_existing_entry()
        {
            Cart sut = fixture.WithProduct();

            sut.AddItem(fixture.ExistingItem.ProductId, 1);

            sut.Items.Should().HaveCount(1);
        }

        [Fact]
        public void When_adding_an_existing_item_for_which_price_has_changed__ShouldNotifyUser()
        {
            Cart sut = fixture.WithProduct();
            fixture.ChangePrice(fixture.ExistingItem.ProductId);
            
            sut.AddItem(fixture.ExistingItem.ProductId, 1);

            A.CallTo(() => fixture.UserFeedback.Send(A<string>.Ignored)).MustHaveHappened();
        }
    }
}