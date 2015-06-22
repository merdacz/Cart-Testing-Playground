namespace CartService.Tests
{
    using System.Collections.Generic;

    using NMoneys;

    public class FakeExchangeService : IExchangeService
    {
        private readonly IDictionary<string, decimal> rates = new Dictionary<string, decimal>();

        public void DefineExchangeRate(CurrencyIsoCode source, CurrencyIsoCode target, decimal rate)
        {
            var key = CreateUniqueHash(source, target);
            this.rates[key] = rate;
        }

        public Money Exchange(Money money, CurrencyIsoCode targetCurrency)
        {
            var rateKey = CreateUniqueHash(money.CurrencyCode, targetCurrency);
            var rate = 1m;
            if (this.rates.ContainsKey(rateKey))
            {
                rate = this.rates[rateKey];
            }

            var newAmount = money.Amount * rate;
            var targetMoney = new Money(newAmount, targetCurrency);
            return targetMoney;
        }

        private static string CreateUniqueHash(CurrencyIsoCode source, CurrencyIsoCode target)
        {
            return source + ">" + target;
        }
    }
}