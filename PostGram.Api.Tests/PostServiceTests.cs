using AutoFixture;
using AutoFixture.AutoMoq;
using EntityFrameworkCore.AutoFixture.InMemory;
using Microsoft.EntityFrameworkCore;
using Moq;
using PostGram.Api.Tests.Customizations;
using PostGram.Common.Enums;
using PostGram.Common.Exceptions;
using PostGram.DAL;
using PostGram.DAL.Entities;
using System.Diagnostics;
using PostGram.BLL;
using PostGram.BLL.Services;
using PostGram.Common.Dtos.Comment;
using PostGram.Common.Dtos.Like;
using PostGram.Common.Dtos.Post;
using PostGram.Common.Requests;
//TODO Перенести в отдельный проект
namespace PostGram.Api.Tests;

public class PostServiceTests
{
    private readonly IFixture _fixture;

    public PostServiceTests()
    {
        _fixture = new Fixture();

        _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
            .ForEach(b => _fixture.Behaviors.Remove(b));
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());

        _fixture.Customize(new CompositeCustomization(
                new InMemoryCustomization()
                {
                    Configure = options => options
                        .LogTo(message => Debug.WriteLine(message))
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors()
                },
                new AutoMoqCustomization(),
                new AutoMapperCustomization(typeof(MapperProfile)),
                new PostCustomization(),
                new UserCustomization(),
                new CommentCustomization()
            )
        );
    }

    [Fact]
    public async Task CreateComment_AuthorIdIsCreatorId_True()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid userId = Guid.NewGuid();
        Guid postId = await AddPost(context);
        CreateCommentModel model = GetCreateCommentModel(postId);
        PostService service = GetPostService(context);

        // Act
        Guid commentId = await service.CreateComment(model, userId);

        // Assert
        Assert.Equal(userId, context.Comments.FirstOrDefault(c => c.Id == commentId)?.AuthorId);
    }

    [Fact]
    public async Task CreateComment_DBSaveException_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();
        Guid postId = await AddPost(mock.Object);
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);
        CreateCommentModel model = GetCreateCommentModel(postId);

        // Act
        Task<Guid> TestFunc() => service.CreateComment(model, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<PostGramException>(TestFunc);
    }

    [Fact]
    public async Task CreateComment_PostDeleted_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context, true);
        Post? comment = context.Posts.FirstOrDefault(p => p.Id == postId);
        if (comment != null)
        {
            comment.IsDeleted = true;
        }
        await context.SaveChangesAsync();
        PostService service = GetPostService(context);
        CreateCommentModel model = GetCreateCommentModel(postId);

        // Act
        Task<Guid> Func() => service.CreateComment(model, _fixture.Create<Guid>());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(Func);
    }

    [Fact]
    public async Task CreateComment_PostNotExist_Exception()
    {
        // Arrange
        PostService service = GetPostService();
        CreateCommentModel model = GetCreateCommentModel();

        // Act
        Task<Guid> TestFunc() => service.CreateComment(model, _fixture.Create<Guid>());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task CreateComment_QuotedCommentDeleted_Valid()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        string quotedText = _fixture.Create<string>();
        Guid postId = await AddPost(context);
        Guid commentId = await AddComment(context, postId, quotedText);

        CreateCommentModel model = GetCreateCommentModel(postId, commentId, quotedText: quotedText);
        PostService service = GetPostService(context);

        // Act
        Guid result = await service.CreateComment(model, _fixture.Create<Guid>());
        Guid expected = context.Comments.First(c => c.QuotedCommentId == commentId).Id;

        // Assert
        Assert.Equal(expected, result);
    }

    [Fact]
    public async Task CreateComment_QuotedCommentDoesntContainQuotedText_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        Guid commentId = await AddComment(context, postId);
        PostService service = GetPostService(context);
        CreateCommentModel model = GetCreateCommentModel(postId, commentId, quotedText: _fixture.Create<string>());

        // Act
        Task<Guid> TestFunc() => service.CreateComment(model, _fixture.Create<Guid>());

        // Assert
        await Assert.ThrowsAsync<UnprocessableRequestPostGramException>(TestFunc);
    }

    [Theory]
    [InlineData("103ed95a-6ea1-43ed-bf0a-cc5ead5f841f", null)]
    [InlineData(null, "test")]
    public async Task CreateComment_QuotedCommentIdAndQuote_Exception(string? quotedCommentId, string? quotedText)
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid? quotedCommentIdGuid = quotedCommentId == null ? null : Guid.Parse(quotedCommentId);
        Guid postId = await AddPost(context);
        // add comment to quote
        await AddComment(context, postId, quotedText, commentId: quotedCommentIdGuid);
        PostService service = GetPostService(context);

        CreateCommentModel model = GetCreateCommentModel(postId, quotedCommentIdGuid, quotedText: quotedText);

        // Act
        Task<Guid> TestFunc() => service.CreateComment(model, _fixture.Create<Guid>());

        // Assert
        await Assert.ThrowsAsync<UnprocessableRequestPostGramException>(TestFunc);
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("103ed95a-6ea1-43ed-bf0a-cc5ead5f841f", "test")]
    public async Task CreateComment_QuotedCommentIdAndQuote_Valid(string? quotedCommentId, string? quotedText)
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid? quotedCommentIdGuid = quotedCommentId == null ? null : Guid.Parse(quotedCommentId);
        Guid postId = await AddPost(context);
        // add comment to quote
        await AddComment(context, postId, quotedText, commentId: quotedCommentIdGuid);
        PostService service = GetPostService(context);

        CreateCommentModel model = GetCreateCommentModel(postId, quotedCommentIdGuid, quotedText: quotedText);

        // Act
        Guid result = await service.CreateComment(model, _fixture.Create<Guid>());

        // Assert
        Assert.IsType<Guid>(result);
    }

    [Fact]
    public async Task CreateComment_QuotedCommentNotExist_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        PostService service = GetPostService(context);
        CreateCommentModel model = GetCreateCommentModel(postId, Guid.NewGuid(), quotedText: _fixture.Create<string>());

        // Act
        Task<Guid> TestFunc() => service.CreateComment(model, _fixture.Create<Guid>());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task CreateLike_AuthorIdIsCreatorId_True()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid userId = Guid.NewGuid();
        Guid postId = await AddPost(context);
        CreateLikeModel model = GetCreateLikeModel(postId, LikableEntities.Post);
        PostService service = GetPostService(context);

        // Act
        Guid likeId = await service.CreateLike(model, userId);

        // Assert
        Assert.Equal(userId, context.Likes.FirstOrDefault(c => c.Id == likeId)?.AuthorId);
    }

    [Fact]
    public async Task CreateLike_DBSaveException_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();
        Guid postId = await AddPost(mock.Object);
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ThrowsAsync(new DbUpdateException());
        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);
        CreateLikeModel model = GetCreateLikeModel(postId);

        // Act
        Task<Guid> TestFunc() => service.CreateLike(model, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<PostGramException>(TestFunc);
    }

    [Theory]
    [InlineData(LikableEntities.Post)]
    [InlineData(LikableEntities.Comment)]
    public async Task CreateLike_ForEntity_Valid(LikableEntities likableEntity)
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid entity = Guid.NewGuid();
        switch (likableEntity)
        {
            case LikableEntities.Post:
                entity = await AddPost(context);
                break;

            case LikableEntities.Comment:
                Guid postId = await AddPost(context);
                entity = await AddComment(context, postId);
                break;
        }
        Guid userId = await AddUser(context);
        CreateLikeModel model = GetCreateLikeModel(entity, likableEntity);
        PostService service = GetPostService(context);

        // Act
        Guid result = await service.CreateLike(model, userId);

        // Assert
        Assert.Equal(context.Likes.FirstOrDefault(l => l.AuthorId == userId && l.EntityId == entity)?.Id, result);
    }

    [Theory]
    [InlineData(LikableEntities.Post)]
    [InlineData(LikableEntities.Comment)]
    public async Task CreateLike_ForNotExistEntity_Exception(LikableEntities likableEntity)
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid userId = await AddUser(context);
        CreateLikeModel model = GetCreateLikeModel(Guid.NewGuid(), likableEntity);
        PostService service = GetPostService(context);

        // Act
        Task<Guid> TestFunc() => service.CreateLike(model, userId);

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task CreateLike_LikeAlreadyExist_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid userId = await AddUser(context);
        Guid postId = await AddPost(context);
        Guid likeId = await AddLike(context, userId, postId, LikableEntities.Post);
        CreateLikeModel model = GetCreateLikeModel(postId, LikableEntities.Post);
        PostService service = GetPostService(context);

        // Act
        Task<Guid> TestFunc() => service.CreateLike(model, userId);

        // Assert
        await Assert.ThrowsAsync<UnprocessableRequestPostGramException>(TestFunc);
    }

    [Fact]
    public async Task CreatePost_AuthorIdIsCreatorId_True()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid userId = Guid.NewGuid();
        CreatePostModel model = GetCreatePostModel();
        PostService service = GetPostService(context);

        // Act
        Guid postId = await service.CreatePost(model, userId);

        // Assert
        Assert.Equal(userId, context.Posts.FirstOrDefault(c => c.Id == postId)?.AuthorId);
    }

    [Fact]
    public async Task CreatePost_DBSaveException_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());
        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);

        // Act
        Task<Guid> TestFunc() => service.CreatePost(_fixture.Create<CreatePostModel>(), Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<PostGramException>(TestFunc);
    }

    [Fact]
    public async Task CreatePost_PostContent_Added()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        CreatePostModel model = GetCreatePostModel();
        PostService service = GetPostService(context);

        // Act
        Guid postId = await service.CreatePost(model, Guid.NewGuid());

        // Assert
        Assert.All(model.Attachments, m => Assert.Equal(m.TempId, context.PostContents.FirstOrDefault(pc => pc.Id == m.TempId)?.Id));
    }

    [Fact]
    public async Task DeleteComment_AuthorIdNotIsUserId_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        Guid commentId = await AddComment(context, postId);
        PostService service = GetPostService(context);

        // Act
        Task<Guid> TestFunc() => service.DeleteComment(commentId, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<AuthorizationPostGramException>(TestFunc);
    }

    [Fact]
    public async Task DeleteComment_DBSaveException_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();
        Guid commentId = await AddComment(mock.Object);
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new DbUpdateException());
        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);
        Guid? userId = mockContext.Comments.FirstOrDefault(c => c.Id == commentId)?.AuthorId;

        // Act
        Task<Guid> TestFunc() => service.DeleteComment(commentId, userId.Value);

        // Assert
        await Assert.ThrowsAsync<PostGramException>(TestFunc);
    }

    [Fact]
    public async Task DeleteComment_ExistComment_Valid()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        Guid commentId = await AddComment(context, postId);
        Guid? userId = context.Comments.FirstOrDefault(c => c.Id == commentId)?.AuthorId;
        PostService service = GetPostService(context);

        // Act
        Guid? result = userId == null ? null : await service.DeleteComment(commentId, userId.Value);
        Comment? deletedComment = context.Comments.IgnoreQueryFilters().FirstOrDefault(c => c.Id == commentId);

        // Assert
        Assert.Equal(commentId, result);
        Assert.Equal(true, deletedComment?.IsDeleted);
    }

    [Fact]
    public async Task DeleteComment_NotExistComment_Exception()
    {
        // Arrange
        PostService service = GetPostService();

        // Act
        Task<Guid> TestFunc() => service.DeleteComment(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task DeletePost_AuthorIdNotIsUserId_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        PostService service = GetPostService(context);

        // Act
        Task<Guid> TestFunc() => service.DeletePost(postId, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<AuthorizationPostGramException>(TestFunc);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task DeletePost_DBSaveException_Exception(bool withPostContent)
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();

        Guid postId = await AddPost(mock.Object);
        bool secondCall = false;
        mock.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).Callback(() =>
        {
            if (withPostContent || secondCall)
            {
                throw new DbUpdateException();
            }
            secondCall = true;
        });
        await using DataContext mockContext = mock.Object;
        Guid? userId = mockContext.Posts.FirstOrDefault(c => c.Id == postId)?.AuthorId;
        PostService service = GetPostService(mockContext);

        // Act
        Task<Guid> TestFunc() => service.DeletePost(postId, userId.Value);

        // Assert
        await Assert.ThrowsAsync<PostGramException>(TestFunc);
    }

    [Fact]
    public async Task DeletePost_ExistPost_Valid()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        Guid? userId = context.Posts.FirstOrDefault(c => c.Id == postId)?.AuthorId;
        PostService service = GetPostService(context);

        // Act
        Guid? result = userId == null ? null : await service.DeletePost(postId, userId.Value);
        Post? deletedPost = context.Posts.IgnoreQueryFilters().FirstOrDefault(c => c.Id == postId);

        // Assert
        Assert.Equal(postId, result);
        Assert.Equal(true, deletedPost?.IsDeleted);
    }

    [Fact]
    public async Task DeletePost_NotExistPost_Exception()
    {
        // Arrange
        PostService service = GetPostService();

        // Act
        Task<Guid> TestFunc() => service.DeletePost(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task DeletePost_PostContent_Deleted()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid postId = await AddPost(context);
        Guid? userId = context.Posts.FirstOrDefault(c => c.Id == postId)?.AuthorId;
        PostService service = GetPostService(context);

        // Act
        await service.DeletePost(postId, userId.Value);

        // Assert
        List<PostContent> deletedPostContents = context.PostContents.Where(pc => pc.PostId == postId).ToList();
        Assert.Empty(deletedPostContents);
    }

    [Fact]
    public async Task GetComment_DeletedComment_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid commentId = await AddComment(context);
        Comment? comment = context.Comments.FirstOrDefault(c => c.Id == commentId);
        comment.IsDeleted = true;
        await context.SaveChangesAsync();

        // Act
        Task<CommentDto> TestFunc() => service.GetComment(commentId, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task GetComment_ExistComment_Valid()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        Guid commentId = await AddComment(context);
        PostService service = GetPostService(context);

        // Act
        CommentDto comment = await service.GetComment(commentId, Guid.NewGuid());

        // Assert
        Assert.IsType<CommentDto>(comment);
        Assert.Equal(commentId, comment.Id);
    }

    [Fact]
    public async Task GetComment_LikeByUser_Filled()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid commentId = await AddComment(context);
        Guid userId = await AddUser(context);
        Guid likeId = await AddLike(context, userId, commentId, LikableEntities.Comment);

        // Act
        CommentDto comment = await service.GetComment(commentId, userId);

        // Assert
        Assert.IsType<LikeDto>(comment.LikeByUser);
        Assert.Equal(likeId, comment.LikeByUser?.Id);
    }

    [Fact]
    public async Task GetComment_NotExistComment_Exception()
    {
        // Arrange
        PostService service = GetPostService();

        // Act
        Task<CommentDto> TestFunc() => service.GetComment(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task GetCommentsForPost_CommentsNotExist_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid postId = await AddPost(context);
        IQueryable<Comment> comments = context.Comments.Where(c => c.PostId == postId);
        context.Comments.RemoveRange(comments);
        await context.SaveChangesAsync();

        // Act
        Task<CommentDto[]> TestFunc() => service.GetCommentsForPost(postId, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task GetCommentsForPost_ExistComments_Valid()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid postId = await AddPost(context);

        // Act
        CommentDto[] result = await service.GetCommentsForPost(postId, Guid.NewGuid());
        Comment[] expectedResult =
            context.Comments.Where(c => c.PostId == postId).OrderBy(c => c.Created).ToArray();

        // Assert
        Assert.IsType<CommentDto[]>(result);
        Assert.Equal(expectedResult.Length, result.Length);
        for (int i = 0; i < expectedResult.Length; i++)
        {
            Assert.Equal(expectedResult[i].Id, result[i].Id);
        }
    }

    [Fact]
    public async Task GetCommentsForPost_LikeByUser_Filled()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid postId = await AddPost(context);
        Guid commentId = await AddComment(context, postId);
        Guid userId = await AddUser(context);
        Guid likeId = await AddLike(context, userId, commentId, LikableEntities.Comment);

        // Act
        CommentDto[] result = await service.GetCommentsForPost(postId, userId);

        // Assert
        Assert.IsType<CommentDto[]>(result);
        Assert.Equal(likeId, result.FirstOrDefault(c => c.LikeByUser != null)?.LikeByUser?.Id);
    }

    [Fact]
    public async Task GetCommentsForPost_PostNotExist_Exception()
    {
        // Arrange
        PostService service = GetPostService();

        // Act
        Task<CommentDto[]> TestFunc() => service.GetCommentsForPost(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task GetPost_LikeByUser_Filled()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid userId = await AddUser(context);

        Guid postId = await AddPost(context, authorId: userId);
        Guid likeId = await AddLike(context, userId, postId, LikableEntities.Post);

        // Act
        PostDto result = await service.GetPost(postId, userId);

        // Assert
        Assert.IsType<LikeDto>(result.LikeByUser);
        Assert.Equal(likeId, result.LikeByUser?.Id);
    }

    [Fact]
    public async Task GetPost_PostAuthorIsNotUser_Exception()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid userId = await AddUser(context);
        Guid postId = await AddPost(context);

        // Act
        Task<PostDto> TestFunc() => service.GetPost(postId, userId);

        // Assert
        await Assert.ThrowsAsync<AuthorizationPostGramException>(TestFunc);
    }

    [Fact]
    public async Task GetPost_PostNotExist_Exception()
    {
        // Arrange
        PostService service = GetPostService();

        // Act
        Task<PostDto> TestFunc() => service.GetPost(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Theory]
    [InlineData(10, 0)]
    [InlineData(0, 0)]
    [InlineData(0, 10)]
    public async Task GetPosts_PostsNotExist_Exception(int takeAmount, int skipAmount)
    {
        // Arrange
        PostService service = GetPostService();

        // Act
        Task<List<PostDto>> TestFunc() => service.GetPosts(takeAmount, skipAmount, Guid.NewGuid());

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task GetPosts_LikesByUser_Filled()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        List<Guid> expectedLikes = new List<Guid>();

        Guid userId = await AddUser(context);
        Guid postId = await AddPost(context, authorId: userId);
        expectedLikes.Add(await AddLike(context, userId, postId, LikableEntities.Post));

        // Act
        List<PostDto> result = await service.GetPosts(1, 0, userId);

        // Assert
        Assert.All(result, x => Assert.IsType<LikeDto>(x.LikeByUser));
        Assert.All(result, x => Assert.Contains(expectedLikes, e => e == x.LikeByUser?.Id));
    }

    [Fact]
    public async Task GetPosts_PostContent_Include()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);

        Guid userId = await AddUser(context);
        Guid postId = await AddPost(context, authorId: userId);
        List<Guid> expectedGuids = context.PostContents.Where(x => x.PostId == postId).Select(x => x.Id).ToList();

        // Act
        List<PostDto> result = await service.GetPosts(1, 0, userId);

        // Assert
        Assert.Equal(expectedGuids, result.FirstOrDefault()?.Content.Select(x => x.Id));
    }

    [Fact]
    public async Task GetPosts_Author_Include()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);

        Guid userId = await AddUser(context);
        Guid postId = await AddPost(context, authorId: userId);

        // Act
        List<PostDto> result = await service.GetPosts(1, 0, userId);

        // Assert
        Assert.Equal(userId, result.FirstOrDefault()?.Author.Id);
    }

    [Fact]
    public async Task UpdateComment_DBSaveException_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();

        Guid postId = await AddPost(mock.Object);
        Guid userId = await AddUser(mock.Object);
        Guid commentId = await AddComment(mock.Object, postId, userId: userId);

        mock.Setup(x => x
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => throw new DbUpdateException());

        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);
        UpdateCommentModel model = GetUpdateCommentModel(commentId);

        // Act
        Task<CommentDto> TestFunc() => service.UpdateComment(model, userId);

        // Assert
        await Assert.ThrowsAsync<PostGramException>(TestFunc);
    }

    [Fact]
    public async Task UpdateComment_CommentNotExist_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();

        Guid userId = await AddUser(mock.Object);

        mock.Setup(x => x
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => throw new DbUpdateException());

        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);
        UpdateCommentModel model = GetUpdateCommentModel(Guid.NewGuid());

        // Act
        Task<CommentDto> TestFunc() => service.UpdateComment(model, userId);

        // Assert
        await Assert.ThrowsAsync<NotFoundPostGramException>(TestFunc);
    }

    [Fact]
    public async Task UpdateComment_CreatedByAnotherUser_Exception()
    {
        // Arrange
        Mock<DataContext> mock = _fixture.Create<Mock<DataContext>>();

        Guid postId = await AddPost(mock.Object);
        Guid userId = await AddUser(mock.Object);
        Guid commentId = await AddComment(mock.Object, postId);

        mock.Setup(x => x
                .SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Callback(() => throw new DbUpdateException());

        await using DataContext mockContext = mock.Object;
        PostService service = GetPostService(mockContext);
        UpdateCommentModel model = GetUpdateCommentModel(commentId);

        // Act
        Task<CommentDto> TestFunc() => service.UpdateComment(model, userId);

        // Assert
        await Assert.ThrowsAsync<AuthorizationPostGramException>(TestFunc);
    }

    [Fact]
    public async Task UpdateComment_NewBody_Valid()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);

        Guid postId = await AddPost(context);
        Guid userId = await AddUser(context);
        Guid commentId = await AddComment(context, postId, userId: userId);
        UpdateCommentModel model = GetUpdateCommentModel(commentId);

        // Act
        CommentDto result = await service.UpdateComment(model, userId);

        // Assert
        Assert.Equal(model.NewBody, result.Body);
        Assert.NotNull(result.Edited);
        Assert.True(result.Edited - DateTimeOffset.Now < TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task UpdateComment_LikeByUser_Filled()
    {
        // Arrange
        await using DataContext context = _fixture.Create<DataContext>();
        PostService service = GetPostService(context);
        Guid postId = await AddPost(context);
        Guid userId = await AddUser(context);
        Guid commentId = await AddComment(context, postId, userId: userId);
        UpdateCommentModel model = GetUpdateCommentModel(commentId);
        Guid likeId = await AddLike(context, userId, commentId, LikableEntities.Comment);

        // Act
        CommentDto comment = await service.UpdateComment(model, userId);

        // Assert
        Assert.IsType<LikeDto>(comment.LikeByUser);
        Assert.Equal(likeId, comment.LikeByUser?.Id);
    }

    private async Task<Guid> AddComment(
        DataContext context,
        Guid? postId = null,
        string? body = null,
        Guid? quotedCommentId = null,
        string? quotedText = null,
        bool isDeleted = false,
        Guid? commentId = null,
        Guid? userId = null)
    {
        Comment comment = _fixture.Create<Comment>();
        comment.Id = quotedCommentId ?? Guid.NewGuid();
        comment.PostId = postId ?? Guid.NewGuid();
        comment.Post = postId == null ? _fixture.Create<Post>() : context.Posts.First(p => p.Id == postId);
        comment.Body = _fixture.Create<string>() + body;
        comment.IsDeleted = isDeleted;
        comment.Id = commentId ?? Guid.NewGuid();
        comment.QuotedCommentId = quotedCommentId;
        comment.QuotedText = quotedText;

        if (userId != null)
            comment.Author = context.Users.First(u => u.Id == userId);

        if (quotedCommentId != null)
            comment.QuotedComment = context.Comments.First(c => c.Id == quotedCommentId);

        await context.Comments.AddAsync(comment);
        await context.SaveChangesAsync();
        return comment.Id;
    }

    private async Task<Guid> AddLike(
        DataContext context,
        Guid? authorId = null,
        Guid? entityId = null,
        LikableEntities entityType = LikableEntities.Post)
    {
        Like like = _fixture.Create<Like>();
        like.EntityId = entityId ?? Guid.NewGuid();
        like.EntityType = entityType;

        if (authorId != null)
        {
            User author = context.Users.First(u => u.Id == authorId);
            like.Author = author;
            like.AuthorId = author.Id;
        }

        if (entityId != null)
        {
            switch (entityType)
            {
                case LikableEntities.Post:
                    Post? post = context.Posts.FirstOrDefault(p => p.Id == like.EntityId);
                    post?.Likes.Add(like);
                    break;

                case LikableEntities.Comment:
                    Comment? comment = context.Comments.FirstOrDefault(c => c.Id == like.EntityId);
                    comment?.Likes.Add(like);
                    break;
            }
        }

        await context.Likes.AddAsync(like);
        await context.SaveChangesAsync();
        return like.Id;
    }

    private async Task<Guid> AddPost(DataContext context, bool isDeleted = false, Guid? authorId = null)
    {
        Post post = _fixture.Create<Post>();
        post.IsDeleted = isDeleted;
        if (authorId != null)
        {
            post.AuthorId = authorId.Value;
            post.Author = null;
        }
        await context.Posts.AddAsync(post);
        await context.SaveChangesAsync();
        return post.Id;
    }

    private async Task<Guid> AddUser(DataContext context)
    {
        User user = _fixture.Create<User>();
        await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return user.Id;
    }

    private CreateCommentModel GetCreateCommentModel(
        Guid? postId = null,
        Guid? quotedCommentId = null,
        string? body = null,
        string? quotedText = null)
    {
        return _fixture
            .Build<CreateCommentModel>()
            .With(c => c.PostId, postId ?? _fixture.Create<Guid>())
            .With(c => c.QuotedCommentId, quotedCommentId)
            .With(c => c.QuotedText, quotedText)
            .With(c => c.Body, body ?? _fixture.Create<string>())
            .Create();
    }

    private CreateLikeModel GetCreateLikeModel(
        Guid entityId,
        LikableEntities? entityType = LikableEntities.Post,
        bool isLike = true)
    {
        return _fixture.Build<CreateLikeModel>()
            .With(c => c.EntityId, entityId)
            .With(c => c.EntityType, entityType)
            .With(c => c.IsLike, isLike)
            .Create();
    }

    private CreatePostModel GetCreatePostModel(string? header = null, string? body = null)
    {
        return _fixture.Build<CreatePostModel>()
            .With(c => c.Body, body ?? _fixture.Create<string>())
            .With(c => c.Header, header ?? _fixture.Create<string>())
            .Create();
    }

    private UpdateCommentModel GetUpdateCommentModel(Guid commentId)
    {
        return _fixture.Build<UpdateCommentModel>()
            .With(u => u.Id, commentId)
            .Create();
    }

    private PostService GetPostService(DataContext? context = null)
    {
        context ??= _fixture.Create<DataContext>();
        _fixture.Inject(context);
        return _fixture.Create<PostService>();
    }
}