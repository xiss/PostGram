namespace PostGram.Common.Exceptions
{
    public class CommonPostGramException : Exception {
        public CommonPostGramException()
        {
        }
        public CommonPostGramException(string message) : base(message)
        {
        }
    }
}
