using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PostGram.DAL.Entities
{
    public class Attache
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
        public Guid  AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public string FilePath { get; set; } = null!;
    }
}
