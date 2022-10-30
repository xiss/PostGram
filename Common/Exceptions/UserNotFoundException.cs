using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PostGram.Common.Exceptions
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public string? Login { get; set; }
        public Guid? Id { get; set; }

        //TODO Вот так ок?
        public UserNotFoundException(string? login = null, Guid? id = null, string? message = null, Exception? inner = null) : base(message, inner)
        {
            Login = login;
            Id = id;
        }

        protected UserNotFoundException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
