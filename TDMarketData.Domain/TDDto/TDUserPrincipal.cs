using Newtonsoft.Json;

namespace TDMarketData.Domain
{
    public class TDUserPrincipal
    {
        [JsonProperty("authToken")]
        public string AuthToken { get; set; }

        [JsonProperty("userId")]
        public string UserId { get; set; }

        [JsonProperty("userCdDomainId")]
        public string UserCdDomainId { get; set; }

        [JsonProperty("primaryAccountId")]
        public string PrimaryAccountId { get; set; }

        [JsonProperty("lastLoginTime")]
        public string LastLoginTime { get; set; }

        [JsonProperty("tokenExpirationTime")]
        public string TokenExpirationTime { get; set; }

        [JsonProperty("loginTime")]
        public string LoginTime { get; set; }

        [JsonProperty("accessLevel")]
        public string AccessLevel { get; set; }

        [JsonProperty("stalePassword")]
        public bool StalePassword { get; set; }

        [JsonProperty("streamerInfo")]
        public StreamerInfo StreamerInfo { get; set; }

        [JsonProperty("professionalStatus")]
        public string ProfessionalStatus { get; set; }

        [JsonProperty("quotes")]
        public Quotes Quotes { get; set; }

        [JsonProperty("streamerSubscriptionKeys")]
        public StreamerSubscriptionKeys StreamerSubscriptionKeys { get; set; }

        [JsonProperty("accounts")]
        public Account[] Accounts { get; set; }
    }

    public partial class Account
    {
        [JsonProperty("accountId")]
        public string AccountId { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("accountCdDomainId")]
        public string AccountCdDomainId { get; set; }

        [JsonProperty("company")]
        public string Company { get; set; }

        [JsonProperty("segment")]
        public string Segment { get; set; }

        [JsonProperty("surrogateIds")]
        public string SurrogateIds { get; set; }

        [JsonProperty("preferences")]
        public Preferences Preferences { get; set; }

        [JsonProperty("acl")]
        public string Acl { get; set; }

        [JsonProperty("authorizations")]
        public Authorizations Authorizations { get; set; }
    }

    public partial class Authorizations
    {
        [JsonProperty("apex")]
        public bool Apex { get; set; }

        [JsonProperty("levelTwoQuotes")]
        public bool LevelTwoQuotes { get; set; }

        [JsonProperty("stockTrading")]
        public bool StockTrading { get; set; }

        [JsonProperty("marginTrading")]
        public bool MarginTrading { get; set; }

        [JsonProperty("streamingNews")]
        public bool StreamingNews { get; set; }

        [JsonProperty("optionTradingLevel")]
        public string OptionTradingLevel { get; set; }

        [JsonProperty("streamerAccess")]
        public bool StreamerAccess { get; set; }

        [JsonProperty("advancedMargin")]
        public bool AdvancedMargin { get; set; }

        [JsonProperty("scottradeAccount")]
        public bool ScottradeAccount { get; set; }
    }

    public partial class Preferences
    {
        [JsonProperty("expressTrading")]
        public bool ExpressTrading { get; set; }

        [JsonProperty("directOptionsRouting")]
        public bool DirectOptionsRouting { get; set; }

        [JsonProperty("directEquityRouting")]
        public bool DirectEquityRouting { get; set; }

        [JsonProperty("defaultEquityOrderLegInstruction")]
        public string DefaultEquityOrderLegInstruction { get; set; }

        [JsonProperty("defaultEquityOrderType")]
        public string DefaultEquityOrderType { get; set; }

        [JsonProperty("defaultEquityOrderPriceLinkType")]
        public string DefaultEquityOrderPriceLinkType { get; set; }

        [JsonProperty("defaultEquityOrderDuration")]
        public string DefaultEquityOrderDuration { get; set; }

        [JsonProperty("defaultEquityOrderMarketSession")]
        public string DefaultEquityOrderMarketSession { get; set; }

        [JsonProperty("defaultEquityQuantity")]
        public long DefaultEquityQuantity { get; set; }

        [JsonProperty("mutualFundTaxLotMethod")]
        public string MutualFundTaxLotMethod { get; set; }

        [JsonProperty("optionTaxLotMethod")]
        public string OptionTaxLotMethod { get; set; }

        [JsonProperty("equityTaxLotMethod")]
        public string EquityTaxLotMethod { get; set; }

        [JsonProperty("defaultAdvancedToolLaunch")]
        public string DefaultAdvancedToolLaunch { get; set; }

        [JsonProperty("authTokenTimeout")]
        public string AuthTokenTimeout { get; set; }
    }

    public partial class Quotes
    {
        [JsonProperty("isNyseDelayed")]
        public bool IsNyseDelayed { get; set; }

        [JsonProperty("isNasdaqDelayed")]
        public bool IsNasdaqDelayed { get; set; }

        [JsonProperty("isOpraDelayed")]
        public bool IsOpraDelayed { get; set; }

        [JsonProperty("isAmexDelayed")]
        public bool IsAmexDelayed { get; set; }

        [JsonProperty("isCmeDelayed")]
        public bool IsCmeDelayed { get; set; }

        [JsonProperty("isIceDelayed")]
        public bool IsIceDelayed { get; set; }

        [JsonProperty("isForexDelayed")]
        public bool IsForexDelayed { get; set; }
    }

    public partial class StreamerInfo
    {
        [JsonProperty("streamerBinaryUrl")]
        public string StreamerBinaryUrl { get; set; }

        [JsonProperty("streamerSocketUrl")]
        public string StreamerSocketUrl { get; set; }

        [JsonProperty("token")]
        public string Token { get; set; }

        [JsonProperty("tokenTimestamp")]
        public string TokenTimestamp { get; set; }

        [JsonProperty("userGroup")]
        public string UserGroup { get; set; }

        [JsonProperty("accessLevel")]
        public string AccessLevel { get; set; }

        [JsonProperty("acl")]
        public string Acl { get; set; }

        [JsonProperty("appId")]
        public string AppId { get; set; }
    }

    public partial class StreamerSubscriptionKeys
    {
        [JsonProperty("keys")]
        public Key[] Keys { get; set; }
    }

    public partial class Key
    {
        [JsonProperty("key")]
        public string KeyKey { get; set; }
    }
}