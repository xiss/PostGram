using AutoFixture;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using PostGram.Api.Controllers;
using PostGram.Common.Constants;
using System.Security.Claims;
using PostGram.Common.Interfaces.Services;
using PostGram.Common.Models.Comment;
using PostGram.Common.Models.Like;
using PostGram.Common.Models.Post;
using PostGram.Common.Models.User;

namespace PostGram.Api.Tests
{
    public class PostControllerTests
    {
        private const string TestUrl = "testUrl";
        private readonly IFixture _fixture;
        private readonly Guid _testUserId;

        public PostControllerTests()
        {
            _testUserId = Guid.NewGuid();
            _fixture = new Fixture();
        }

        [Fact]
        public async Task CreateComment_Result_Valid()
        {
            // Arrange
            PostController controller = GetMockPostController();

            // Act
            Guid result = await controller.CreateComment(_fixture.Create<CreateCommentModel>());

            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public async Task CreateLike_Result_Valid()
        {
            // Arrange
            PostController controller = GetMockPostController();

            // Act
            Guid result = await controller.CreateLike(_fixture.Create<CreateLikeModel>());

            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public async Task CreatePost_Result_Valid()
        {
            // Arrange
            PostController controller = GetMockPostController();

            // Act
            Guid result = await controller.CreatePost(_fixture.Create<CreatePostModel>());

            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public async Task DeleteComment_Result_Valid()
        {
            // Arrange
            PostController controller = GetMockPostController();

            // Act
            Guid result = await controller.DeleteComment(_fixture.Create<Guid>());

            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public async Task DeletePost_Result_Valid()
        {
            // Arrange
            PostController controller = GetMockPostController();

            // Act
            Guid result = await controller.DeletePost(_fixture.Create<Guid>());

            // Assert
            Assert.IsType<Guid>(result);
        }

        [Fact]
        public async Task GetComment_AvatarLinkWithAvatar_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetComment();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result =  await controller.GetComment(_fixture.Create<Guid>());

            // Assert
            service.VerifyAll();
            Assert.Equal(TestUrl, result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task GetComment_AvatarLinkWithoutAvatar_Null()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetComment(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(_fixture
                    .Build<CommentModel>()
                    .With(m => m.Author, _fixture
                        .Build<UserModel>()
                        .Without(m => m.Avatar)
                        .Create())
                    .Create());
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result = await controller.GetComment(_fixture.Create<Guid>());

            // Assert
            service.VerifyAll();
            Assert.Null(result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task GetComment_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetComment();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result = await controller.GetComment(_fixture.Create<Guid>());

            // Assert
            service.VerifyAll();
            Assert.IsType<CommentModel>(result);
        }

        [Fact]
        public async Task GetCommentsForPost_AvatarLinkWithAvatar_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetCommentsForPost();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel[] result = await controller.GetCommentsForPost(It.IsAny<Guid>());

            // Assert
            service.VerifyAll();
            Assert.NotEmpty(result);
            Assert.All(result, post => Assert.Equal(TestUrl, post.Author.Avatar?.Link)); ;
        }

        [Theory]
        [InlineData(10, 0)]
        public async Task GetCommentsForPost_AvatarLinkWithoutAvatar_Null(int take, int skip)
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetCommentsForPost(It.IsAny<Guid>(), _testUserId))
                .ReturnsAsync(Enumerable
                    .Range(skip, take)
                    .Select(x => _fixture
                    .Build<CommentModel>()
                    .With(m => m.Author, _fixture
                        .Build<UserModel>()
                        .Without(m => m.Avatar)
                        .Create())
                    .Create())
                    .ToArray());
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel[] result = await controller.GetCommentsForPost(_fixture.Create<Guid>());

            // Assert
            service.VerifyAll();
            Assert.NotEmpty(result);
            Assert.All(result, post => Assert.Null(post.Author.Avatar?.Link));
        }

        [Fact]
        public async Task GetCommentsForPost_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetCommentsForPost();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel[] result = await controller.GetCommentsForPost(It.IsAny<Guid>());

            // Assert
            service.VerifyAll();
            Assert.IsType<CommentModel[]>(result);
        }

        [Fact]
        public async Task GetPost_AvatarLinkWithAvatar_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetPost();
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.GetPost(Guid.Empty);

            // Assert
            service.VerifyAll();
            Assert.Equal(TestUrl, result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task GetPost_AvatarLinkWithoutAvatar_Null()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetPost(It.IsAny<Guid>(), _testUserId))
                .ReturnsAsync(_fixture
                    .Build<PostModel>()
                    .With(m => m.Author, _fixture
                        .Build<UserModel>()
                        .Without(m => m.Avatar)
                        .Create())
                    .Create());
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.GetPost(Guid.Empty);

            // Assert
            service.VerifyAll();
            Assert.Null(result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task GetPost_PostContentLinks_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetPost();
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.GetPost(Guid.Empty);

            // Assert
            service.VerifyAll();
            Assert.NotEmpty(result.Content);
            Assert.All(result.Content, postContent => Assert.Equal(TestUrl, postContent.Link));
        }

        [Fact]
        public async Task GetPost_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetPost();
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.GetPost(Guid.Empty);

            // Assert
            service.VerifyAll();
            Assert.IsType<PostModel>(result);
        }

        [Theory]
        [InlineData(10, 0)]
        public async Task GetPosts_AvatarLinkWithAvatar_Generated(int take, int skip)
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetPosts();
            PostController controller = GetMockPostController(service);

            // Act
            List<PostModel> result = await controller.GetPosts(take, skip);

            // Assert
            service.VerifyAll();
            Assert.NotEmpty(result);
            Assert.All(result, post => Assert.Equal(TestUrl, post.Author.Avatar?.Link));
        }

        [Theory]
        [InlineData(10, 0)]
        public async Task GetPosts_AvatarLinkWithoutAvatar_Null(int take, int skip)
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetPosts(take, skip, _testUserId))
                .ReturnsAsync(Enumerable.Range(skip, take).Select(x => _fixture
                    .Build<PostModel>()
                    .With(m => m.Author, _fixture
                        .Build<UserModel>()
                        .Without(m => m.Avatar)
                        .Create())
                    .Create()).ToList());
            PostController controller = GetMockPostController(service);

            // Act
            List<PostModel> result = await controller.GetPosts(take, skip);

            // Assert
            service.VerifyAll();
            Assert.NotEmpty(result);
            Assert.All(result, post => Assert.Null(post.Author.Avatar?.Link));
        }

        [Theory]
        [InlineData(10, 0)]
        public async Task GetPosts_PostContentLinks_Generated(int take, int skip)
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetPosts();
            PostController controller = GetMockPostController(service);

            // Act
            List<PostModel> result = await controller.GetPosts(take, skip);

            // Assert
            service.VerifyAll();
            Assert.NotEmpty(result);
            Assert.All(result.SelectMany(post => post.Content), attachmentModel => Assert.Equal(TestUrl, attachmentModel.Link));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(10)]
        public async Task GetPosts_RangeRequest_Valid(int take)
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetPosts(take, It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(_fixture.CreateMany<PostModel>(take).ToList);
            PostController controller = GetMockPostController(service);

            // Act
            List<PostModel> result = await controller.GetPosts(take, 0);

            // Assert
            service.VerifyAll();
            Assert.InRange(result.Count, 0, take);
        }

        [Theory]
        [InlineData(10, 0)]
        public async Task GetPosts_Result_Valid(int take, int skip)
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetPosts();
            PostController controller = GetMockPostController(service);

            // Act
            List<PostModel> result = await controller.GetPosts(take, skip);

            // Assert
            service.VerifyAll();
            Assert.IsType<List<PostModel>>(result);
        }

        [Fact]
        public async Task UpdateComment_AvatarLinkWithAvatar_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_UpdateComment();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result = await controller.UpdateComment(_fixture.Create<UpdateCommentModel>());

            // Assert
            service.VerifyAll();
            Assert.Equal(TestUrl, result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task UpdateComment_AvatarLinkWithoutAvatar_Null()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.UpdateComment(It.IsAny<UpdateCommentModel>(), _testUserId))
                .ReturnsAsync(_fixture
                    .Build<CommentModel>()
                    .With(m => m.Author, _fixture
                        .Build<UserModel>()
                        .Without(m => m.Avatar)
                        .Create())
                    .Create());
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result = await controller.UpdateComment(_fixture.Create<UpdateCommentModel>());

            // Assert
            service.VerifyAll();
            Assert.Null(result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task UpdateComment_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_UpdateComment();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result = await controller.UpdateComment(_fixture.Create<UpdateCommentModel>());

            // Assert
            service.VerifyAll();
            Assert.IsType<CommentModel>(result);
        }

        [Fact]
        public async Task UpdateLike_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.UpdateLike(It.IsAny<UpdateLikeModel>(), It.IsAny<Guid>()))
                .ReturnsAsync(_fixture.Create<LikeModel>());
            PostController controller = GetMockPostController(service);

            // Act
            LikeModel result = await controller.UpdateLike(_fixture.Create<UpdateLikeModel>());

            // Assert
            service.VerifyAll();
            Assert.IsType<LikeModel>(result);
        }

        [Fact]
        public async Task UpdatePost_AvatarLinkWithAvatar_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_UpdatePost();
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.UpdatePost(_fixture.Create<UpdatePostModel>());

            // Assert
            service.VerifyAll();
            Assert.Equal(TestUrl, result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task UpdatePost_AvatarLinkWithoutAvatar_Null()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.UpdatePost(It.IsAny<UpdatePostModel>(), _testUserId))
                .ReturnsAsync(_fixture
                    .Build<PostModel>()
                    .With(m => m.Author, _fixture
                        .Build<UserModel>()
                        .Without(m => m.Avatar)
                        .Create())
                    .Create());
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.UpdatePost(_fixture.Create<UpdatePostModel>());

            // Assert
            service.VerifyAll();
            Assert.Null(result.Author.Avatar?.Link);
        }

        [Fact]
        public async Task UpdatePost_PostContentLinks_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_UpdatePost();
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.UpdatePost(_fixture.Create<UpdatePostModel>());

            // Assert
            service.VerifyAll();
            Assert.All(result.Content, postContent => Assert.Equal(TestUrl, postContent.Link));
        }

        [Fact]
        public async Task UpdatePost_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_UpdatePost();
            PostController controller = GetMockPostController(service);

            // Act
            PostModel result = await controller.UpdatePost(_fixture.Create<UpdatePostModel>());

            // Assert
            service.VerifyAll();
            Assert.IsType<PostModel>(result);
        }

        private PostController GetMockPostController(Mock<IPostService>? service = null)
        {
            service ??= new Mock<IPostService>();

            Mock<IUrlHelper> mockUrlHelper = new Mock<IUrlHelper>();
            mockUrlHelper.Setup(s => s.Action(It.IsAny<UrlActionContext>())).Returns(TestUrl);

            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimNames.UserId, _testUserId.ToString())
            }));

            ControllerContext controllerContext = new ControllerContext()
            { HttpContext = new DefaultHttpContext() { User = user } };

            PostController controller = new PostController(service.Object)
            {
                ControllerContext = controllerContext,
                Url = mockUrlHelper.Object
            };
            return controller;
        }

        private Mock<IPostService> GetMockPostService_GetComment()
        {
            //TODO
            var a = _fixture.Create<CommentModel>();
            var t=  a with{Body = "test"};
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetComment(It.IsAny<Guid>(), _testUserId))
                .ReturnsAsync(t);
            return service;
        }

        private Mock<IPostService> GetMockPostService_GetCommentsForPost()
        {
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetCommentsForPost(It.IsAny<Guid>(), _testUserId))
                .ReturnsAsync(_fixture.CreateMany<CommentModel>(10).ToArray);
            return service;
        }

        private Mock<IPostService> GetMockPostService_GetPost()
        {
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetPost(It.IsAny<Guid>(), _testUserId))
                .ReturnsAsync(_fixture.Create<PostModel>());
            return service;
        }

        private Mock<IPostService> GetMockPostService_GetPosts()
        {
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetPosts(It.IsAny<int>(), It.IsAny<int>(), _testUserId))
                .ReturnsAsync(_fixture.Create<List<PostModel>>());
            return service;
        }

        private Mock<IPostService> GetMockPostService_UpdateComment()
        {
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.UpdateComment(It.IsAny<UpdateCommentModel>(), _testUserId))
                .ReturnsAsync(_fixture.Create<CommentModel>());
            return service;
        }

        private Mock<IPostService> GetMockPostService_UpdatePost()
        {
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.UpdatePost(It.IsAny<UpdatePostModel>(), _testUserId))
                .ReturnsAsync(_fixture.Create<PostModel>());
            return service;
        }
    }
}