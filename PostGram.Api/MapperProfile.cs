using AutoMapper;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Post;
using PostGram.Api.Models.User;
using PostGram.DAL.Entities;

namespace PostGram.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            CreateMap<CreateUserModel, User>()
                .ForMember(u => u.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(u => u.PasswordHash, m => m.MapFrom(s => Common.HashHelper.GetHash(s.Password)))
                .ForMember(u => u.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));

            CreateMap<User, UserModel>();

            CreateMap<Avatar, AttachmentModel>();

            CreateMap<Attachment, AttachmentModel>();

            CreateMap<CreatePostModel, Post>()
                .ForMember(p => p.Id, m => m.MapFrom(p => Guid.NewGuid()))
                .ForMember(p => p.Attachments, m => m.MapFrom(p => new List<Attachment>()))
                .ForMember(p => p.Created, m => m.MapFrom(p => DateTimeOffset.UtcNow));

            CreateMap<CreateCommentModel, Comment>()
                .ForMember(c => c.Created, m => m.MapFrom(c => DateTimeOffset.UtcNow))
                .ForMember(c => c.Id, m => m.MapFrom(c => Guid.NewGuid()));

            CreateMap<Post, PostModel>()
                .ForMember(p => p.Attachments, m => m.MapFrom(p => new string[p.Attachments.Count - 1]));

            CreateMap<Comment, CommentModel>();
        }
    }
}