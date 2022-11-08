namespace PostGram.Common.Exceptions
{
    public class SessionIsInactivePostGramException : AuthorizationPostGramException
    {
        public SessionIsInactivePostGramException()
        {
        }

        public SessionIsInactivePostGramException(string message) : base(message)
        {
        }
    }
}