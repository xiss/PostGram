namespace PostGram.Common.Exceptions
{
    public class CommonPostGramException : Exception
    {
        public CommonPostGramException()
        {
        }

        public CommonPostGramException(string message) : base(message)
        {
        }

        public CommonPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}