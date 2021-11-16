namespace PMVInvite.Options
{
    public class EmailOptions
    {
        /// <summary>
        /// Configuration section name.
        /// </summary>
        public const string SectionName = "EmailConfiguration";

        /// <summary>
        /// Configuration attribute name.
        /// </summary>
        public string SMTPServer { get; set; }
        public int SMTPPort { get; set; }
        public string SMTPUsername { get; set; }
        public string SMTPPassword { get; set; }
        public bool SMTPUseSSL { get; set; }
        public string SMTPFromAddress { get; set; }
        public string SMTPSubject { get; set; }
    }
}