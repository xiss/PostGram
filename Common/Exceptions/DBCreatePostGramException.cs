namespace PostGram.Common.Exceptions
{
    public class DBCreatePostGramException : DBPostGramException
    {
        public DBCreatePostGramException()
        {
        }

        public DBCreatePostGramException(string message) : base(message)
        {
        }
    }
}