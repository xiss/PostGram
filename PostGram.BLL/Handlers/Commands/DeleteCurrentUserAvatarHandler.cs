﻿using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Commands;
using PostGram.Common.Requests.Commands;
using PostGram.DAL;
using PostGram.DAL.Entities;

namespace PostGram.BLL.Handlers.Commands;

public class DeleteCurrentUserAvatarHandler : ICommandHandler<DeleteCurrentUserAvatarCommand>
{
    private readonly IClaimsProvider _claimsProvider;
    private readonly DataContext _dataContext;
    private readonly IUserService _userService;
    private readonly IAttachmentService _attachmentService;

    public DeleteCurrentUserAvatarHandler(IClaimsProvider claimsProvider, DataContext dataContext,
        IUserService userService, IAttachmentService attachmentService)
    {
        _claimsProvider = claimsProvider ;
        _dataContext = dataContext;
        _userService = userService;
        _attachmentService = attachmentService;
    }

    public async Task Execute(DeleteCurrentUserAvatarCommand command)
    {
        Guid userId = _claimsProvider.GetCurrentUserId();

        User user = await _userService.GetUserById(userId);
        if (user.Avatar == null)
            throw new NotFoundPostGramException($"User {userId} doesn't have avatar.");
        Guid avatarId = user.AvatarId!.Value;

        _dataContext.Avatars.Remove(user.Avatar);
        await _dataContext.SaveChangesAsync();

        _attachmentService.DeleteFile(avatarId);
    }
}