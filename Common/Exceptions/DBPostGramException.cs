namespace PostGram.Common.Exceptions
{
    public class DbPostGramException : CriticalPostGramException
    {
        public DbPostGramException()
        {
        }

        public DbPostGramException(string message) : base(message)
        {
        }

        public DbPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}