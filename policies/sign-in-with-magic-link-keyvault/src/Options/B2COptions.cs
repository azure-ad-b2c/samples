namespace PMVInvite.Options
{
    public class B2COptions
    {
        /// <summary>
        /// Configuration section name.
        /// </summary>
        public const string SectionName = "AzureAdB2C";

        /// <summary>
        /// Configuration attribute name.
        /// </summary>
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string Domain { get; set; }
        public string Instance { get; set; }
        public string RopcPolicy { get; set; }
    }
}
