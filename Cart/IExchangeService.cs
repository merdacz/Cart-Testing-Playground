namespace CartService
{
    using NMoneys;

    public interface IExchangeService
    {
        Money Exchange(Money money, CurrencyIsoCode targetCurrency);
    }
}