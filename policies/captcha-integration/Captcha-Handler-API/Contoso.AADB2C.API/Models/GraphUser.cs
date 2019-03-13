namespace Contoso.AADB2C.API.Models
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Welcome
    {
        [JsonProperty("odata.metadata")]
        public string OdataMetadata { get; set; }

        [JsonProperty("odata.type")]
        public string OdataType { get; set; }

        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("deletionTimestamp")]
        public object DeletionTimestamp { get; set; }

        [JsonProperty("accountEnabled")]
        public bool AccountEnabled { get; set; }

        [JsonProperty("ageGroup")]
        public object AgeGroup { get; set; }

        [JsonProperty("assignedLicenses")]
        public object[] AssignedLicenses { get; set; }

        [JsonProperty("assignedPlans")]
        public object[] AssignedPlans { get; set; }

        [JsonProperty("city")]
        public object City { get; set; }

        [JsonProperty("companyName")]
        public object CompanyName { get; set; }

        [JsonProperty("consentProvidedForMinor")]
        public object ConsentProvidedForMinor { get; set; }

        [JsonProperty("country")]
        public object Country { get; set; }

        [JsonProperty("creationType")]
        public string CreationType { get; set; }

        [JsonProperty("department")]
        public object Department { get; set; }

        [JsonProperty("dirSyncEnabled")]
        public object DirSyncEnabled { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("employeeId")]
        public object EmployeeId { get; set; }

        [JsonProperty("facsimileTelephoneNumber")]
        public object FacsimileTelephoneNumber { get; set; }

        [JsonProperty("givenName")]
        public object GivenName { get; set; }

        [JsonProperty("immutableId")]
        public object ImmutableId { get; set; }

        [JsonProperty("isCompromised")]
        public object IsCompromised { get; set; }

        [JsonProperty("jobTitle")]
        public object JobTitle { get; set; }

        [JsonProperty("lastDirSyncTime")]
        public object LastDirSyncTime { get; set; }

        [JsonProperty("legalAgeGroupClassification")]
        public object LegalAgeGroupClassification { get; set; }

        [JsonProperty("mail")]
        public object Mail { get; set; }

        [JsonProperty("mailNickname")]
        public string MailNickname { get; set; }

        [JsonProperty("mobile")]
        public object Mobile { get; set; }

        [JsonProperty("onPremisesDistinguishedName")]
        public object OnPremisesDistinguishedName { get; set; }

        [JsonProperty("onPremisesSecurityIdentifier")]
        public object OnPremisesSecurityIdentifier { get; set; }

        [JsonProperty("otherMails")]
        public object[] OtherMails { get; set; }

        [JsonProperty("passwordPolicies")]
        public string PasswordPolicies { get; set; }

        [JsonProperty("passwordProfile")]
        public object PasswordProfile { get; set; }

        [JsonProperty("physicalDeliveryOfficeName")]
        public object PhysicalDeliveryOfficeName { get; set; }

        [JsonProperty("postalCode")]
        public object PostalCode { get; set; }

        [JsonProperty("preferredLanguage")]
        public object PreferredLanguage { get; set; }

        [JsonProperty("provisionedPlans")]
        public object[] ProvisionedPlans { get; set; }

        [JsonProperty("provisioningErrors")]
        public object[] ProvisioningErrors { get; set; }

        [JsonProperty("proxyAddresses")]
        public object[] ProxyAddresses { get; set; }

        [JsonProperty("refreshTokensValidFromDateTime")]
        public DateTimeOffset RefreshTokensValidFromDateTime { get; set; }

        [JsonProperty("showInAddressList")]
        public object ShowInAddressList { get; set; }

        [JsonProperty("signInNames")]
        public SignInName[] SignInNames { get; set; }

        [JsonProperty("sipProxyAddress")]
        public object SipProxyAddress { get; set; }

        [JsonProperty("state")]
        public object State { get; set; }

        [JsonProperty("streetAddress")]
        public object StreetAddress { get; set; }

        [JsonProperty("surname")]
        public object Surname { get; set; }

        [JsonProperty("telephoneNumber")]
        public object TelephoneNumber { get; set; }

        [JsonProperty("usageLocation")]
        public object UsageLocation { get; set; }

        [JsonProperty("userIdentities")]
        public object[] UserIdentities { get; set; }

        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [JsonProperty("userType")]
        public string UserType { get; set; }
    }



    public partial class Welcome
    {
        public static Welcome FromJson(string json) => JsonConvert.DeserializeObject<Welcome>(json, Contoso.AADB2C.API.Models.Converter.Settings);
    }

    public static class Serialize
    {
        public static string ToJson(this Welcome self) => JsonConvert.SerializeObject(self, Contoso.AADB2C.API.Models.Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters = {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }
}