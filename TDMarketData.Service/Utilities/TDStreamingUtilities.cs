using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TDMarketData.Domain;
using TDMarketData.Domain.TDStreaming;

namespace TDMarketData.Service.Utilities
{
    public static class TDStreamingUtilities
    {
        public static int ReqId { get; set; }
        public static JObject GetAdminLoginRequest(TDUserPrincipal userPrincipal)
        {
            var account = userPrincipal.Accounts[0];

            var request = new JObject();

            var tokenTimeStampAsMs = MillisecondsSinceEpoc(userPrincipal.StreamerInfo.TokenTimestamp).ToString();

            var credentialDict = new Dictionary<string, string>
            {
                { "userid",  account.AccountId},
                {"token", userPrincipal.StreamerInfo.Token},
                {"company", account.Company},
                {"segment", account.Segment},
                {"cddomain", account.AccountCdDomainId},
                {"usergroup", userPrincipal.StreamerInfo.UserGroup},
                {"accesslevel", userPrincipal.StreamerInfo.AccessLevel},
                {"authorized", "Y"},
                {"timestamp", tokenTimeStampAsMs},
                {"appid", userPrincipal.StreamerInfo.AppId},
                {"acl", userPrincipal.StreamerInfo.Acl }
                };



            var streamingRequest = new TDStreamingRequest
            {
                Service = "ADMIN",
                Command = "LOGIN",
                Account = account.AccountId,
                Source = userPrincipal.StreamerInfo.AppId,
                RequestId = ReqId.ToString(),
                Parameters = new Dictionary<string, string>
                    {
                        { "credential",  DictionaryToQueryString(credentialDict) },
                        { "token", userPrincipal.StreamerInfo.Token },
                        { "version", "1.0" }
                    }
            };

            ReqId++;
            return JObject.FromObject(streamingRequest);
        }

        public static JObject GetQuoteStreamingRequest(string[] symbols, string account, string source)
        {
            var streamingRequest = new TDStreamingRequest
            {
                Service = "QUOTE",
                RequestId = ReqId.ToString(),
                Command = "SUBS",
                Account = account,
                Source = source,
                Parameters = new Dictionary<string, string>
                {
                    { "keys", string.Join(",", symbols) },
                    { "fields", "0,1,2,3,4,5,6,7,8" }
                }
            };

            ReqId++;

            return JObject.FromObject(streamingRequest);
        }

        public static string DictionaryToQueryString(Dictionary<string, string> kvps)
        {

            var queryStringArray = kvps.Select(kvp => Uri.EscapeUriString(kvp.Key) + "=" + Uri.EscapeUriString(kvp.Value)).ToArray();

            return string.Join("&", queryStringArray);
        }

        public static long MillisecondsSinceEpoc(string timestamp)
        {

            var date = DateTime.Parse(timestamp);

            var dateDiff = date.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;

            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            DateTime tokenDate = Convert.ToDateTime(timestamp);
            TimeSpan tokenEpoch = tokenDate.ToUniversalTime() - epoch;
            long timestamp2 = (long)Math.Floor(tokenEpoch.TotalMilliseconds);

            return timestamp2;
        }

        public static JObject GetEquityTimeSaleStreamingRequest(string[] vs, string accountId, string source)
        {
            var streamingRequest = new TDStreamingRequest
            {
                Service = "TIMESALE_EQUITY",
                RequestId = ReqId.ToString(),
                Command = "SUBS",
                Account = accountId,
                Source = source,
                Parameters = new Dictionary<string, string>
                {
                    { "keys", string.Join(",", vs) },
                    { "fields", "0,1,2,3,4" }
                }
            };

            ReqId++;

            return JObject.FromObject(streamingRequest);
        }

        public static JObject GetLevel2OptionStreamingRequest(string[] vs, string accountId, string source)
        {
            var streamingRequest = new TDStreamingRequest
            {
                Service = "OPTION_BOOK",
                RequestId = ReqId.ToString(),
                Command = "SUBS",
                Account = accountId,
                Source = source,
                Parameters = new Dictionary<string, string>
                {
                    { "keys", "SPY_070620C313" }, //string.Join(",", vs)
                    { "fields", "0,1,2,3,4" }
                }
            };

            ReqId++;

            return JObject.FromObject(streamingRequest);
        }

        public static JObject GetLevel1OptionStreamingRequest(string[] symbol, string accountId, string source)
        {
            var streamingRequest = new TDStreamingRequest
            {
                Service = "OPTION",
                RequestId = ReqId.ToString(),
                Command = "SUBS",
                Account = accountId,
                Source = source,
                Parameters = new Dictionary<string, string>
                {
                    { "keys", string.Join(",", symbol) },
                    { "fields", "0,2,3,4,20,21" }
                }
            };

            ReqId++;

            return JObject.FromObject(streamingRequest);
        }
    }
}
