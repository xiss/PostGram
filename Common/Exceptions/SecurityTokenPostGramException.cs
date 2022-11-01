namespace PostGram.Common.Exceptions
{
    public class SecurityTokenPostGramException : AuthorizationPostGramException
    {
        public SecurityTokenPostGramException()
        {
        }
        public SecurityTokenPostGramException(string message) : base(message)
        {
        }
    }
}
