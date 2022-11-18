namespace PostGram.Common.Exceptions
{
    public class UnprocessableRequestPostGramException : CommonPostGramException
    {
        public UnprocessableRequestPostGramException()
        {
        }

        public UnprocessableRequestPostGramException(string message) : base(message)
        {
        }

        public UnprocessableRequestPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}