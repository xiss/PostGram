namespace PostGram.Common.Exceptions
{
    public class SessionNotFoundPostGramException : AuthorizationPostGramException
    {
        public SessionNotFoundPostGramException()
        {
        }
        public SessionNotFoundPostGramException(string message) : base(message)
        {
        }
    }
}
