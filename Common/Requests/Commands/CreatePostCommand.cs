﻿using PostGram.Common.Constants;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Base.Commands;

using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record CreatePostCommand : ICommand
{
    [Required]
    [StringLength(ModelValidation.PostHeaderLength)]
    public string Header { get; init; } = string.Empty;

    [Required]
    [StringLength(ModelValidation.PostBodyLength)]
    public string Body { get; init; } = string.Empty;

    [Required]
    public virtual ICollection<MetadataModel> Attachments { get; init; } = new List<MetadataModel>();
}