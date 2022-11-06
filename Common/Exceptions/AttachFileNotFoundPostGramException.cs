namespace PostGram.Common.Exceptions
{
    public class AttachFileNotFoundPostGramException : AttachPostGramException
    {
        public AttachFileNotFoundPostGramException()
        {
        }
        public AttachFileNotFoundPostGramException(string message) : base(message)
        {
        }
    }
}
