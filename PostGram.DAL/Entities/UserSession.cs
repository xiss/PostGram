using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostGram.DAL.Entities
{
    public class UserSession
    {
        public Guid Id { get; set; }
        public Guid RefreshTokenId { get; set; }

        public Guid UserId { get; set; }
        public virtual User User { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
