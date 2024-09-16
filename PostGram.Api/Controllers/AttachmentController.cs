using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using PostGram.Api.Helpers;
using PostGram.BLL.Features.GetComment;
using PostGram.BLL.Interfaces.Providers;
using PostGram.BLL.Interfaces.Services;
using PostGram.Common.Configs;
using PostGram.Common.Dtos;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Exceptions;
using PostGram.Common.Interfaces.Base.Queries;
using PostGram.Common.Requests.Queries;
using PostGram.DAL;
using PostGram.DAL.Entities;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace PostGram.Api.Controllers;

[ApiExplorerSettings(GroupName = Common.Constants.Api.EndpointApiName)]
[Route("api/[controller]/[action]")]
[ApiController]
[Authorize]
public class AttachmentController : ControllerBase
{
    private readonly IAttachmentService _attachmentService;
    private readonly IQueryHandler<GetAvatarForUserQuery, FileInfoDto> _getAvatarForUserHandler;
    private readonly IQueryHandler<GetPostContentQuery, FileInfoDto> _getPostContentHandler;


    public AttachmentController(IAttachmentService attachmentService, 
        IQueryHandler<GetAvatarForUserQuery, FileInfoDto> getAvatarForUserHandler, 
        IQueryHandler<GetPostContentQuery, FileInfoDto> getPostContentHandler)
    {
        _attachmentService = attachmentService;
        _getAvatarForUserHandler = getAvatarForUserHandler;
        _getPostContentHandler = getPostContentHandler;
    }

    [HttpGet]
    public async Task<FileStreamResult> GetAvatarForUser(GetAvatarForUserQuery query)
    {
        FileInfoDto fileInfo = await _getAvatarForUserHandler.Execute(query);
        return GetAttachmentStream(fileInfo, query.IsDownload);
    }

    [HttpGet]
    public async Task<FileStreamResult> GetPostContent(GetPostContentQuery query)
    {
        FileInfoDto fileInfo = await _getPostContentHandler.Execute(query);
        return GetAttachmentStream(fileInfo, query.IsDownload);
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
        return download
            ? File(model.FileStream, model.MimeType, $"{model.Name}.{Path.GetExtension(model.Name)}")
            : File(model.FileStream, model.MimeType);
    }

}









