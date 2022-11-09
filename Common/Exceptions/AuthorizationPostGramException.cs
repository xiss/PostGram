namespace PostGram.Common.Exceptions
{
    public class AuthorizationPostGramException : CommonPostGramException
    {
        public AuthorizationPostGramException()
        {
        }

        public AuthorizationPostGramException(string message) : base(message)
        {
        }

        public AuthorizationPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}