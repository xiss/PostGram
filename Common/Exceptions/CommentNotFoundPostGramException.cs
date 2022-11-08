namespace PostGram.Common.Exceptions
{
    public class CommentNotFoundPostGramException : CommentPostGramException
    {
        public CommentNotFoundPostGramException()
        {
        }

        public CommentNotFoundPostGramException(string message) : base(message)
        {
        }
    }
}