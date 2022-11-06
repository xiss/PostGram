namespace PostGram.Common.Exceptions
{
    public class AttachFileExistPostGramException : AttachPostGramException
    {
        public AttachFileExistPostGramException()
        {
        }
        public AttachFileExistPostGramException(string message) : base(message)
        {
        }
    }
}
