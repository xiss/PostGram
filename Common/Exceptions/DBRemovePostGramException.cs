namespace PostGram.Common.Exceptions
{
    public class DBRemovePostGramException : DBPostGramException
    {
        public DBRemovePostGramException()
        {
        }

        public DBRemovePostGramException(string message) : base(message)
        {
        }
    }
}