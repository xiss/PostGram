using System.Runtime.Serialization;

namespace PostGram.Common.Exceptions
{
    public class AttachPostGramException : CriticalPostGramException
    {
        public AttachPostGramException()
        {
        }
        public AttachPostGramException(string message) : base(message)
        {
        }
        public AttachPostGramException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
