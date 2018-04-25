namespace Zidium.Core
{
    public class TokenNotValidException : UserFriendlyException
    {
        public TokenNotValidException(string message) : base (message) {}
    }
}
