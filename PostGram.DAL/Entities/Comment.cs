﻿namespace PostGram.DAL.Entities
{
    public class Comment 
    {
        public Guid Id { get; set; } 
        public Guid AuthorId { get; set; }
        public virtual User Author { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public string Body { get; set; } = null!;
        public DateTimeOffset? Edited { get; set; }
        public bool IsDeleted { get; set; } = false;
        public Guid PostId { get; set; }
        public Post Post { get; set; } = null!;
        public Comment? QuotedComment { get; set; } = null;
        public Guid? QuotedCommentId { get; set; } = null;
        public string? QuotedText { get; set; } = null;
        public ICollection<Like> Likes { get; set; } = new List<Like>();
    }
}