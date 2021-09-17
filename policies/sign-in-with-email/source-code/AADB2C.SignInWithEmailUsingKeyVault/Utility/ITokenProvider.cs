
namespace AADB2C.SignInWithEmailUsingKeyVault.Utility
{
    public interface ITokenProvider
    {
        public abstract string BuildSerializedIdToken(string issuer, string audience, int duration, string userEmail);
    }
}
