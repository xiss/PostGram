namespace PostGram.Common.Exceptions
{
    public class AttachTempDirectoryIsNullPostGramException : AttachPostGramException
    {
        public AttachTempDirectoryIsNullPostGramException()
        {
        }

        public AttachTempDirectoryIsNullPostGramException(string message) : base(message)
        {
        }
    }
}