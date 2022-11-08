namespace PostGram.Common.Exceptions
{
    public class PostPostGramException : CommonPostGramException
    {
        public PostPostGramException()
        {
        }

        public PostPostGramException(string message) : base(message)
        {
        }
    }
}