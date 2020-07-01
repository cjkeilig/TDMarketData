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
                RequestId = "1",
                Parameters = new Dictionary<string, string>
                    {
                        { "credential",  DictionaryToQueryString(credentialDict) },
                        { "token", userPrincipal.StreamerInfo.Token },
                        { "version", "1.0" }
                    }
            };

            return JObject.FromObject(streamingRequest);
        }

        public static JObject GetQuoteStreamingRequest(string[] symbols, string account, string source)
        {
            var streamingRequest = new TDStreamingRequest
            {
                Service = "QUOTE",
                RequestId = "2",
                Command = "SUBS",
                Account = account,
                Source = source,
                Parameters = new Dictionary<string, string>
                {
                    { "keys", string.Join(",", symbols) },
                    { "fields", "0,1,2,3,4,5,6,7,8" }
                }
            };

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
            return (long)dateDiff;
        }
    }
}
