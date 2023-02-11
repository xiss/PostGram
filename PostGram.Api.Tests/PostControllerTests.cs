using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;
using PostGram.Api.Controllers;
using PostGram.Api.Models.Comment;
using PostGram.Api.Models.Like;
using PostGram.Api.Models.Post;
using PostGram.Api.Models.User;
using PostGram.Api.Services;
using PostGram.Common.Constants;
using System.Security.Claims;

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
            Mock<IPostService> service = new Mock<IPostService>();
            PostController controller = GetMockPostController(service);

            // Act
            Guid result = await controller.CreateComment(_fixture.Create<CreateCommentModel>());

            // Assert
            Assert.IsAssignableFrom<Guid>(result);
        }

        [Fact]
        public async Task CreateLike_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            PostController controller = GetMockPostController(service);

            // Act
            Guid result = await controller.CreateLike(_fixture.Create<CreateLikeModel>());

            // Assert
            Assert.IsAssignableFrom<Guid>(result);
        }

        [Fact]
        public async Task CreatePost_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            PostController controller = GetMockPostController(service);

            // Act
            Guid result = await controller.CreatePost(_fixture.Create<CreatePostModel>());

            // Assert
            Assert.IsAssignableFrom<Guid>(result);
        }

        [Fact]
        public async Task DeleteComment_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            PostController controller = GetMockPostController(service);

            // Act
            Guid result = await controller.DeleteComment(_fixture.Create<Guid>());

            // Assert
            Assert.IsAssignableFrom<Guid>(result);
        }

        [Fact]
        public async Task DeletePost_Result_Valid()
        {
            // Arrange
            Mock<IPostService> service = new Mock<IPostService>();
            PostController controller = GetMockPostController(service);

            // Act
            Guid result = await controller.DeletePost(_fixture.Create<Guid>());

            // Assert
            Assert.IsAssignableFrom<Guid>(result);
        }

        [Fact]
        public async Task GetComment_AvatarLinkWithAvatar_Generated()
        {
            // Arrange
            Mock<IPostService> service = GetMockPostService_GetComment();
            PostController controller = GetMockPostController(service);

            // Act
            CommentModel result = await controller.GetComment(_fixture.Create<Guid>());

            // Assert
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
            Assert.IsAssignableFrom<CommentModel>(result);
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
            Assert.True(result.Length != 0, "result.Length == 0");
            foreach (var commentModel in result)
            {
                Assert.Equal(TestUrl, commentModel.Author.Avatar?.Link);
            }
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
            Assert.True(result.Length != 0, "result.Length == 0");
            foreach (var commentModel in result)
            {
                Assert.Null(commentModel.Author.Avatar?.Link);
            }
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
            Assert.IsAssignableFrom<CommentModel[]>(result);
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
            Assert.True(result.Content.Count != 0, "result.Content.Count == 0");
            foreach (var attachment in result.Content)
            {
                Assert.Equal(TestUrl, attachment.Link);
            }
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
            Assert.IsAssignableFrom<PostModel>(result);
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
            Assert.True(result.Count != 0, "result.Count == 0");
            result.ForEach(post => Assert.Equal(TestUrl, post.Author.Avatar?.Link));
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
            Assert.True(result.Count != 0, "result.Count == 0");
            result.ForEach(post => Assert.Null(post.Author.Avatar?.Link));
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
            Assert.True(result.Count != 0, "result.Count == 0");
            foreach (var attachment in result.SelectMany(post => post.Content))
            {
                Assert.Equal(TestUrl, attachment.Link);
            }
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
            Assert.IsAssignableFrom<List<PostModel>>(result);
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
            Assert.IsAssignableFrom<CommentModel>(result);
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
            Assert.IsAssignableFrom<LikeModel>(result);
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
            foreach (var attachment in result.Content)
            {
                Assert.Equal(TestUrl, attachment.Link);
            }
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
            Assert.IsAssignableFrom<PostModel>(result);
        }

        private PostController GetMockPostController(Mock<IPostService> service)
        {
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
            Mock<IPostService> service = new Mock<IPostService>();
            service
                .Setup(s => s.GetComment(It.IsAny<Guid>(), _testUserId))
                .ReturnsAsync(_fixture.Create<CommentModel>());
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