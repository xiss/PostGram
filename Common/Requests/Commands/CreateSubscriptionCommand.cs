﻿using System.ComponentModel.DataAnnotations;
using PostGram.BLL.Interfaces.Base.Commands;

namespace PostGram.Common.Requests.Commands;

public record CreateSubscriptionCommand : ICommand
{
    [Required]
    public Guid MasterId { get; init; }
}