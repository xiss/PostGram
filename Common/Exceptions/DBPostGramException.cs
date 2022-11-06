namespace PostGram.Common.Exceptions
{
    public class DBPostGramException : CriticalPostGramException
    {
        public DBPostGramException()
        {
        }
        public DBPostGramException(string message) : base(message)
        {
        }
    }
}
