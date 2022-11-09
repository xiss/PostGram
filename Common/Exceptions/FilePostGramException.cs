namespace PostGram.Common.Exceptions
{
    public class FilePostGramException : CriticalPostGramException
    {
        public FilePostGramException()
        {
        }

        public FilePostGramException(string message) : base(message)
        {
        }

        public FilePostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}