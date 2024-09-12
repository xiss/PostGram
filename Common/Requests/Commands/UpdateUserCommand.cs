using PostGram.Common.Constants;
using PostGram.Common.Interfaces.Base.Commands;

using System.ComponentModel.DataAnnotations;

namespace PostGram.Common.Requests.Commands;

public record UpdateUserCommand : ICommand
{
    [Required]
    public Guid UserId { get; init; }
    public DateTimeOffset? NewBirthDate { get; init; }

    public bool? NewIsPrivate { get; init; }
    [StringLength(ModelValidation.UserNameLength)]
    public string? NewName { get; init; }
    [StringLength(ModelValidation.UserNicknameLength)]
    public string? NewNickname { get; init; }
    [StringLength(ModelValidation.UserPatronymicLength)]
    public string? NewPatronymic { get; init; }
    [StringLength(ModelValidation.UserSurnameLength)]
    public string? NewSurname { get; init; }
}