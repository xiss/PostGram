﻿namespace PostGram.DAL.Entities
{
    public class Post : CreationBase
    {
        public string Header { get; set; } = null!;
        public string Body { get; set; } = null!;
        public DateTimeOffset? Edited { get; set; }
        public bool IsDeleted { get; set; } = false;
        public virtual ICollection<PostContent> PostContents { get; set; } = new List<PostContent>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}