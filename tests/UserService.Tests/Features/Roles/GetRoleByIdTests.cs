using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using UserService.API.Features.Roles;
using UserService.API.Persistence.Entities;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;
using UserService.API.Abstraction;
using Xunit;

namespace UserService.Tests.Features.Roles
{
    public class GetRoleByIdTests
    {
        private readonly GetRoleByIdHandler _handler;
        private readonly GetRoleByIdValidator _validator;


        public GetRoleByIdTests()
        {
            var mockRepo = new Mock<IRoleService>();
            var mockCache = new Mock<IMemoryCache>();
            object dummy;
            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out dummy)).Returns(false);
            mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());


            mockRepo.Setup(r => r.GetRoleByIdAsync(1, It.IsAny<CancellationToken>()))
                    .ReturnsAsync(new Role { Id = 1, Name = "Admin", Description = "Admin role" });
            //var service = new RoleService(mockRepo.Object, mockCache.Object);
            _handler = new GetRoleByIdHandler(mockRepo.Object);
            _validator = new GetRoleByIdValidator();
        }

        /// <summary>
        /// Tests that a valid role ID request returns a successful result,
        /// including the expected role data.
        /// </summary>
        [Fact]
        public async Task GetRoleById_ValidRequest_ReturnsRole()
        {
            var request = new GetRoleByIdRequest(1);
            var validation = await _validator.ValidateAsync(request);
            var result = await _handler.HandleAsync(request, CancellationToken.None);

            Assert.True(validation.IsValid);
            Assert.True(result.Success);
            Assert.NotNull(result.Data);
            Assert.Equal("Admin", result.Data.Name);
        }

        /// <summary>
        /// Tests that providing an invalid role ID (e.g. 0) triggers validation errors
        /// and does not proceed to data fetching.
        /// </summary>
        [Fact]
        public async Task GetRoleById_InvalidId_ReturnsValidationError()
        {
            var request = new GetRoleByIdRequest(0);
            var validation = await _validator.ValidateAsync(request);

            Assert.False(validation.IsValid);
            Assert.Contains(validation.Errors, e => e.PropertyName == "Id");
        }

        /// <summary>
        /// Tests that requesting a non-existent role returns a failed result
        /// with an appropriate "Role not found" message.
        /// </summary>
        [Fact]
        public async Task GetRoleById_NotFound_ReturnsFailure()
        {
            var mockRepo = new Mock<IRoleService>();
            var mockCache = new Mock<IMemoryCache>();
            object dummy;
            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out dummy)).Returns(false);
            mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            //mockRepo.Setup(r => r.GetByIdAsync(404, It.IsAny<CancellationToken>())).ReturnsAsync((Role)null);
            //var service = new RoleService(mockRepo.Object, mockCache.Object);
            var handler = new GetRoleByIdHandler(mockRepo.Object);

            var request = new GetRoleByIdRequest(404);
            var result = await handler.HandleAsync(request, CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("Role not found", result.Message);
        }
    }
}
