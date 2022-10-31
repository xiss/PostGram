using System.Runtime.Serialization;

namespace PostGram.Common.Exceptions
{
    //TODO Все ли тут ок?
    // Может не стоит создавать свойства для логина итд, а просто указывать это в messege? 
    // При обработке исключения не понятно какое именно свойство нужно использовать.
    // Либо делать свойства закрытыми и вставлять их message при обращении к последнему
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
