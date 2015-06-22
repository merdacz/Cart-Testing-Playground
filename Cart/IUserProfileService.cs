namespace CartService
{
    using System.Collections;

    using NMoneys;

    public interface IUserProfileService
    {
        CurrencyIsoCode GetDefaultCurrency();
    }
}