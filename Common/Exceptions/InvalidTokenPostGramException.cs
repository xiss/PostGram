namespace PostGram.Common.Exceptions
{
    public class InvalidTokenPostGramException : AuthorizationPostGramException
    {
        public InvalidTokenPostGramException()
        {
        }
        public InvalidTokenPostGramException(string message) : base(message)
        {
        }
    }
}
