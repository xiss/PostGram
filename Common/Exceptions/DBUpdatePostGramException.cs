namespace PostGram.Common.Exceptions
{
    public class DBUpdatePostGramException : DBPostGramException
    {
        public DBUpdatePostGramException()
        {
        }
        public DBUpdatePostGramException(string message) : base(message)
        {
        }
    }
}
