namespace PostGram.Common.Exceptions
{
    public class CommentPostGramException : CommonPostGramException
    {
        public CommentPostGramException()
        {
        }

        public CommentPostGramException(string message) : base(message)
        {
        }
    }
}