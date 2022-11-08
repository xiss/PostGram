namespace PostGram.Common.Exceptions
{
    public class UserNotFoundPostGramException : AuthorizationPostGramException
    {
        public UserNotFoundPostGramException()
        {
        }

        public UserNotFoundPostGramException(string message) : base(message)
        {
        }
    }
}