using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PostGram.DAL.Entities
{
    public class Attachment : CreationBase
    {
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public long Size { get; set; }
        public string FilePath { get; set; } = null!;
    }
}
