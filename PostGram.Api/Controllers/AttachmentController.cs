using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostGram.Api.Helpers;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Services;

namespace PostGram.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    // [Authorize]

    public class AttachmentController : ControllerBase
    {
        private readonly IAttachmentService _attachmentService;

        public AttachmentController(IAttachmentService attachmentService)
        {
            _attachmentService = attachmentService;
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
        public async Task<ActionResult> GetPostContent(Guid postContentId, bool download = false)
        {
            FileInfoModel model = await _attachmentService.GetPostContent(postContentId);
            return RenderAttachment(model, download);
        }

        [HttpGet]
        public async Task<ActionResult> GetAvatarForUser(Guid userId, bool download = false)
        {

            FileInfoModel internalModel = await _attachmentService.GetAvatarForUser(userId);
            return RenderAttachment(internalModel, download);
        }

        public static string GetLinkForPostContent(IUrlHelper urlHelper, Guid postContentId)
        {
            return urlHelper.ControllerAction<AttachmentController>(nameof(GetPostContent), new { postContentId })!;
        }

        public static string GetLinkForAvatar(IUrlHelper urlHelper, Guid userId)
        {
            return urlHelper.ControllerAction<AttachmentController>(nameof(GetAvatarForUser), new { userId })!;
        }

        private FileStreamResult RenderAttachment(FileInfoModel model, bool download)
        {
            FileStream stream = new FileStream(model.Path, FileMode.Open);
            if (download)
                return File(stream, model.MimeType, $"{model.Name}.{Path.GetExtension(model.Name)}");
            return File(stream, model.MimeType);
        }
    }
}