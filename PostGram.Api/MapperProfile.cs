using AutoMapper;
using PostGram.Api.Models.Attachment;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;
using PostGram.Api.Models.Subscription;
using PostGram.Api.Models.User;
using PostGram.DAL.Entities;

namespace PostGram.Api
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            //User
            CreateMap<CreateUserModel, User>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PasswordHash, m => m.MapFrom(s => Common.HashHelper.GetHashSha256(s.Password)))
                .ForMember(d => d.BirthDate, m => m.MapFrom(s => s.BirthDate.UtcDateTime));
            CreateMap<User, UserModel>();

            //Attachment
            CreateMap<Avatar, AttachmentModel>();
            CreateMap<PostContent, AttachmentModel>();
            CreateMap<MetadataModel, PostContent>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.Id, m => m.MapFrom(s => s.TempId));

            //Post
            CreateMap<CreatePostModel, Post>()
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()))
                .ForMember(d => d.PostContents, m => m.MapFrom(s => new List<PostContent>()))
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow));
            CreateMap<Post, PostModel>()
                .ForMember(d => d.Content, m => m.MapFrom(s => s.PostContents))
                .ForMember(d => d.DislikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == false)))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == true)))
                .ForMember(d => d.CommentCount, m => m.MapFrom(s => s.Comments.Count));

            //Comment
            CreateMap<CreateCommentModel, Comment>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));

            CreateMap<Comment, CommentModel>()
                .ForMember(d => d.DislikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == false)))
                .ForMember(d => d.LikeCount, m => m.MapFrom(s => s.Likes.Count(l => l.IsLike == true)));

            //Like
            CreateMap<CreateLikeModel, Like>()
                .ForMember(d => d.Created, m => m.MapFrom(s => DateTimeOffset.UtcNow))
                .ForMember(d => d.Id, m => m.MapFrom(s => Guid.NewGuid()));
            CreateMap<Like, LikeModel>();

            //Subscriptions
            CreateMap<Subscription, SubscriptionModel>();
        }
    }
}