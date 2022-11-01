namespace PostGram.Common.Exceptions
{
    public class CriticalPostGramException : Exception
    {
        public CriticalPostGramException()
        {
        }
        public CriticalPostGramException(string message) : base(message)
        {
        }
    }
}
