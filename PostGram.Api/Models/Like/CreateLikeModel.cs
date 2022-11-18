﻿using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Like
{
    public class CreateLikeModel
    {
        [Required]
        public bool IsLike { get; set; }
        public  Guid? CommentId { get; set; }
        public  Guid? PostId { get; set; }
    }
}
