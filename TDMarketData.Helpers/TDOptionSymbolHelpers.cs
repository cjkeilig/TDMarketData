using System;
using System.Collections.Generic;
using System.Text;

namespace TDMarketData.BackTesting.Data.Helpers
{
    public static class TDOptionSymbolHelpers
    {
        public static int GetCallPutIndex(string optionSymbol)
        {
            var callIndex = optionSymbol.LastIndexOf("C");
            var putIndex = optionSymbol.LastIndexOf("P");

            var callPutIndex = callIndex >= putIndex ? callIndex : putIndex;

            return callPutIndex;
        }

        public static char GetCallPut(string optionSymbol)
        {
            var callPutIndex = GetCallPutIndex(optionSymbol);
            return optionSymbol[callPutIndex];
        }

        public static double GetStrike(string o)
        {
            var callPutIndex = GetCallPutIndex(o);
            return double.Parse(o.Substring(callPutIndex + 1, o.Length - callPutIndex - 1));
        }

        public static int GetExpirationStartIndex(string o)
        {
            var callPutIndex = GetCallPutIndex(o);
            var expirationStart = callPutIndex - 6;

            return expirationStart;
        }

        public static string GetExpiration(string o)
        {
            var expirationStart = GetExpirationStartIndex(o);
            var expiration = o.Substring(expirationStart, 6);

            return expiration;
        }

        public static string GetUnderlyingSymbol(string o)
        {
            var callPutIndex = GetCallPutIndex(o);
            var expirationStart = callPutIndex - 6;

            var underlyingSymbol = o.Substring(1, expirationStart - 1);

            return underlyingSymbol;
        }
    }
}
