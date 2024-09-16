﻿using PostGram.Common.Constants;
using PostGram.Common.Dtos.Attachment;
using System.ComponentModel.DataAnnotations;
using PostGram.Common.Interfaces.Base.Commands;

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
    public virtual ICollection<MetadataDto> Attachments { get; init; } = new List<MetadataDto>();
}