using AutoMapper;
using PostGram.Common.Dtos;
using PostGram.Common.Dtos.Attachment;
using PostGram.Common.Requests.Commands;
using PostGram.DAL.Entities;

namespace PostGram.BLL
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //User
            CreateMap<CreateUserCommand, User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => Common.HashHelper.GetHashSha256(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));
            CreateMap<User, UserDto>();

            //Attachment
            CreateMap<Avatar, AttachmentDto>();
            CreateMap<PostContent, AttachmentDto>();
            CreateMap<MetadataDto, PostContent>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.Id, m => m.MapFrom(s => s.TempId));

            //Post
            CreateMap<CreatePostCommand, Post>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PostContents, m => m.MapFrom(s => new List<PostContent>()))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow));
            CreateMap<Post, PostDto>()
                .ForMember(d => d.Content, m => m.MapFrom(s => s.PostContents))
                .ForMember(d => d.DislikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == false)))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == true)))
                .ForMember(d => d.CommentCount, m => m.MapFrom(s => s.Comments.Count));

            //Comment
            CreateMap<CreateCommentCommand, Comment>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));

            CreateMap<Comment, CommentDto>()
                .ForMember(d => d.DislikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == false)))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == true)));

            //Like
            CreateMap<CreateLikeCommand, Like>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));
            CreateMap<Like, LikeDto>();

            //Subscriptions
            CreateMap<Subscription, SubscriptionDto>();
        }
    }
}