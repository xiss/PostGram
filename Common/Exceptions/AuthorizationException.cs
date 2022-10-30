using System.Runtime.Serialization;

namespace PostGram.Common.Exceptions
{
    //TODO Все ли тут ок?
    [Serializable]
    public class AuthorizationException : Exception
    {
        public string? Login { get; set; }
        public Guid? Id { get; set; }

        //TODO Вот так ок?
        public AuthorizationException(string? login = null, Guid? id = null, string? message = null, Exception? inner = null) : base(message, inner)
        {
            Login = login;
            Id = id;
        }

        protected AuthorizationException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
