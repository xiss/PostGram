namespace PostGram.Common.Exceptions
{
    public class PostGramException : Exception
    {
        public PostGramException()
        {
        }

        public PostGramException(string message) : base(message)
        {
        }

        public PostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}