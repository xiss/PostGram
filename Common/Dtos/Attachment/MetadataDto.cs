﻿using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Dtos.Attachment;

public record MetadataDto
{
    [Required]
    public Guid TempId { get; init; }
    [Required]
    public string Name { get; init; } = string.Empty;
    [Required]
    public string MimeType { get; init; } = string.Empty;
    [Required]
    public long Size { get; init; }
}