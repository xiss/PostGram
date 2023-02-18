namespace PostGram.Common.Exceptions
{
    public class BadRequestPostGramException : PostGramException
    {
        public BadRequestPostGramException()
        {
        }

        public BadRequestPostGramException(string message) : base(message)
        {
        }

        public BadRequestPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}