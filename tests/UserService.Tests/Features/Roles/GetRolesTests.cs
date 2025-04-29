using System.Collections.Generic;
using System.Linq;
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
    public class GetRolesTests
    {
        private readonly GetRolesHandler _handler;

        public GetRolesTests()
        {
            var mockRepo = new Mock<IRoleService>();
            var mockCache = new Mock<IMemoryCache>();
            object dummy;
            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out dummy)).Returns(false);
            mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            mockRepo.Setup(r => r.GetAllRolesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Role>
            {
                new Role { Id = 1, Name = "Admin", Description = "Admin role" },
                new Role { Id = 2, Name = "User", Description = "User role" }
            });

            //var service = new RoleService(mockRepo.Object, mockCache.Object);
            _handler = new GetRolesHandler(mockRepo.Object);
        }

        /// <summary>
        /// Tests that the handler successfully returns a list of roles
        /// when roles are available in the repository.
        /// </summary>
        [Fact]
        public async Task GetRoles_ReturnsListOfRoles()
        {
            var result = await _handler.HandleAsync(CancellationToken.None);

            Assert.True(result.Success);
            Assert.NotEmpty(result.Data);
            Assert.Contains(result.Data, r => r.Name == "Admin");
        }

        /// <summary>
        /// Tests that the handler returns a failure result with an appropriate message
        /// when the repository returns an empty list of roles.
        /// </summary>
        [Fact]
        public async Task GetRoles_NoRolesFound_ReturnsFailure()
        {
            var mockRepo = new Mock<IRoleService>();
            var mockCache = new Mock<IMemoryCache>();
            object dummy;
            mockCache.Setup(c => c.TryGetValue(It.IsAny<object>(), out dummy)).Returns(false);
            mockCache.Setup(c => c.CreateEntry(It.IsAny<object>())).Returns(Mock.Of<ICacheEntry>());

            mockRepo.Setup(r => r.GetAllRolesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<Role>());

            //var service = new RoleService(mockRepo.Object, mockCache.Object);
            var handler = new GetRolesHandler(mockRepo.Object);

            var result = await handler.HandleAsync(CancellationToken.None);

            Assert.False(result.Success);
            Assert.Equal("No roles found", result.Message);
            Assert.Empty(result.Data);
        }
    }
}
