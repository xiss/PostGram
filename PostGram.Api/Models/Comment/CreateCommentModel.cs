﻿using System.ComponentModel.DataAnnotations;

namespace PostGram.Api.Models.Comment
{
    public class CreateCommentModel
    {
        [Required]
        public string Body { get; set; } = null!;
        [Required]
        public Guid PostId { get; set; }
    }
}