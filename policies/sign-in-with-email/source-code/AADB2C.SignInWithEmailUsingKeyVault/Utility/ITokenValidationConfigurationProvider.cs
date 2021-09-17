namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public interface ITokenValidationConfigurationProvider
    {
        public abstract string BuildSerializedOidcConfig(string issuer, string jwksUri);

        public abstract string BuildSerializedJwks();
    }
}
