﻿using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Models;
using PostGram.Api.Services;
using PostGram.Common.Exceptions;
using LogLevel = NLog.LogLevel;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;
        private readonly NLog.Logger _logger;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        [HttpPost]
        public async Task<IEnumerable<MetadataModel>> UploadFiles([FromForm] List<IFormFile> files)
        {
            List<MetadataModel> modelList = new List<MetadataModel>();
            foreach (var file in files)
            {
                modelList.Add(await _attachmentService.UploadFile(file));
            }

            return modelList;
        }

        [HttpPost]
        public async Task<MetadataModel> UploadFile(IFormFile file)
        {
            return await _attachmentService.UploadFile(file);
        }

        [HttpGet]
        public async Task<ActionResult> GetAttachment(Guid attahmentId)
        {
            try
            {
                AttachmentModel model = await _attachmentService.GetAttachment(attahmentId);
                return File(await System.IO.File.ReadAllBytesAsync(model.FilePath), model.MimeType);
            }
            catch (AttachPostGramException e)
            {
                _logger.Log(LogLevel.Error, e);
                return StatusCode(500, e.Message);
            }
        }
    }
}