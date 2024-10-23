﻿using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class AddAvatarToUserHandler : ICommandHandler<AddAvatarToUserCommand>
{
    private readonly IClaimsProvider _claimsProvider;
    private readonly DataContext _dataContext;
    private readonly IUserService _userService;
    private readonly IAttachmentService _attachmentService;
    private readonly TimeProvider _timeProvider;

    public AddAvatarToUserHandler(IClaimsProvider claimsProvider, DataContext dataContext,
    IUserService userService, IAttachmentService attachmentService, TimeProvider timeProvider)
    {
        _claimsProvider = claimsProvider;
        _dataContext = dataContext;
        _userService = userService;
        _attachmentService = attachmentService;
        _timeProvider = timeProvider;
    }

    public async Task Execute(AddAvatarToUserCommand command)
    {
        await _attachmentService.ApplyFile(command.Metadata.TempId.ToString());

        Guid userId = _claimsProvider.GetCurrentUserId();
        User user = await _userService.GetUserById(userId);
        if (user.Avatar != null)
            throw new UnprocessableRequestPostGramException($"User {user.Id} already has avatar {user.AvatarId}. Delete it before add new.");
        var avatar = new Avatar()
        {
            UserId = user.Id,
            AuthorId = user.Id,
            Id = command.Metadata.TempId,
            MimeType = command.Metadata.MimeType,
            Name = command.Metadata.Name,
            Size = command.Metadata.Size,
            Created = _timeProvider.GetUtcNow()
        };
        user.Avatar = avatar;

        _dataContext.Avatars.Add(avatar);
        await _dataContext.SaveChangesAsync();
    }
}