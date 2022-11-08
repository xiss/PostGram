namespace PostGram.Common.Exceptions
{
    public class PostNotFoundPostGramException : PostPostGramException
    {
        public PostNotFoundPostGramException()
        {
        }

        public PostNotFoundPostGramException(string message) : base(message)
        {
        }
    }
}