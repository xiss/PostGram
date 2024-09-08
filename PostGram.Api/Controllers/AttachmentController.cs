﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Interfaces.Services;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;

    public AttachmentController(IAttachmentService attachmentService)
    {
        _attachmentService = attachmentService;
    }

    // TODO Зачем добапвлять ссылки?
    //public static string GetLinkForAvatar(IUrlHelper urlHelper, Guid userId)
    //{
    //    return urlHelper.ControllerAction<AttachmentController>(nameof(GetAvatarForUser), new { userId })!;
    //}

    //public static string GetLinkForPostContent(IUrlHelper urlHelper, Guid postContentId)
    //{
    //    return urlHelper.ControllerAction<AttachmentController>(nameof(GetPostContent), new { postContentId })!;
    //}

    [HttpGet]
    public async Task<ActionResult> GetAvatarForUser(Guid userId, bool download = false)
    {
        FileInfoDto internalModel = await _attachmentService.GetAvatarForUser(userId);
        return GetAttachmentStream(internalModel, download);
    }

    [HttpGet]
    public async Task<ActionResult> GetPostContent(Guid postContentId, bool download = false)
    {
        FileInfoDto model = await _attachmentService.GetPostContent(postContentId, this.GetCurrentUserId());
        return GetAttachmentStream(model, download);
    }

    [HttpPost]
    public async Task UploadAttachment(IFormFile file)
    {
        await _attachmentService.UploadFile(file);
    }

    [HttpPost]
    public async Task UploadAttachments([FromForm] List<IFormFile> files)
    {
        foreach (var file in files)
        {
            await _attachmentService.UploadFile(file);
        }
    }

    private FileStreamResult GetAttachmentStream(FileInfoDto model, bool download)
    {
        FileStream stream = new FileStream(model.Path, FileMode.Open);
        if (download)
            return File(stream, model.MimeType, $"{model.Name}.{Path.GetExtension(model.Name)}");
        return File(stream, model.MimeType);
    }
}