namespace PostGram.Common.Exceptions
{
    public class NotFoundPostGramException : CommonPostGramException
    {
        public NotFoundPostGramException()
        {
        }

        public NotFoundPostGramException(string message) : base(message)
        {
        }

        public NotFoundPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}