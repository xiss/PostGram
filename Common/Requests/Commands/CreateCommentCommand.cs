﻿using PostGram.BLL.Interfaces.Base.Commands;
using PostGram.Common.Constants;
using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record CreateCommentCommand : ICommand
{
    [Required]
    [StringLength(ModelValidation.CommentLength)]
    public string Body { get; init; } = string.Empty;
    [Required]
    public Guid PostId { get; init; }
    public Guid? QuotedCommentId { get; init; } = null;
    public string? QuotedText { get; set; } = null;
}