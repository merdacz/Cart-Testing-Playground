namespace CartService.Tests
{
    using System;
    using System.IO;
    using System.Text;

    using NMoneys;

    public class Some
    {
        private static readonly Random Random = new Random(4567);

        public static int RandomInt()
        {
            return Random.Next();
        }
        
        public static Money RandomPrice()
        {
            return new Money(Some.RandomInt(), CurrencyIsoCode.USD); 
        }
    }
}