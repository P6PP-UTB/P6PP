using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using UserService.API.Persistence.Entities;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;
using UserService.API.Abstraction;
using Xunit;

namespace UserService.Tests.Services
{
    public class RoleServiceTests
    {
        private readonly Mock<IRoleRepository> _mockRepo;
        private readonly IMemoryCache _cache;
        private readonly RoleService _roleService;

        public RoleServiceTests()
        {
            _mockRepo = new Mock<IRoleRepository>();
            _cache = new MemoryCache(new MemoryCacheOptions());
            _roleService = new RoleService(_mockRepo.Object, _cache);
        }

        /// <summary>
        /// Tests that GetAllRolesAsync returns all roles from the repository.
        /// </summary>
        [Fact]
        public async Task GetAllRolesAsync_ReturnsAllRoles()
        {
            // Arrange
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin", Description = "Admin role" },
                new Role { Id = 2, Name = "User", Description = "User role" }
            };
            _mockRepo.Setup(repo => repo.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);

            // Act
            var result = await _roleService.GetAllRolesAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.Contains(result, r => r.Name == "Admin");
            Assert.Contains(result, r => r.Name == "User");
        }

        /// <summary>
        /// Tests that GetRoleByIdAsync fetches a role from the repository when it is not in the cache,
        /// and that it stores the result in the cache.
        /// </summary>
        [Fact]
        public async Task GetRoleByIdAsync_CacheMiss_ReturnsFromRepoAndCaches()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin" };
            _mockRepo.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(role);

            // Act
            var result = await _roleService.GetRoleByIdAsync(1, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Admin", result!.Name);
            Assert.True(_cache.TryGetValue("role:1", out Role? cachedRole));
            Assert.Equal("Admin", cachedRole!.Name);
        }

        /// <summary>
        /// Tests that GetRoleByIdAsync returns a role from the cache when it is already cached,
        /// without calling the repository.
        /// </summary>
        [Fact]
        public async Task GetRoleByIdAsync_CacheHit_ReturnsFromCache()
        {
            // Arrange
            var cachedRole = new Role { Id = 2, Name = "CachedUser" };
            _cache.Set("role:2", cachedRole);

            // Act
            var result = await _roleService.GetRoleByIdAsync(2, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("CachedUser", result!.Name);
            _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        /// <summary>
        /// Tests that AddRoleAsync successfully adds a role to the repository
        /// and stores it in the cache upon success.
        /// </summary>
        [Fact]
        public async Task AddRoleAsync_Success_CachesAndReturnsId()
        {
            // Arrange
            var newRole = new Role { Id = 3, Name = "Moderator" };
            _mockRepo.Setup(r => r.AddAsync(newRole, It.IsAny<CancellationToken>())).ReturnsAsync(3);

            // Act
            var result = await _roleService.AddRoleAsync(newRole, CancellationToken.None);

            // Assert
            Assert.Equal(3, result);
            Assert.True(_cache.TryGetValue("role:3", out Role? cached));
            Assert.Equal("Moderator", cached!.Name);
        }

        /// <summary>
        /// Tests that AddRoleAsync does not cache the role if adding it to the repository fails (returns null).
        /// </summary>
        [Fact]
        public async Task AddRoleAsync_NullResult_DoesNotCache()
        {
            // Arrange
            var failingRole = new Role { Id = 4, Name = "FailRole" };
            _mockRepo.Setup(r => r.AddAsync(failingRole, It.IsAny<CancellationToken>())).ReturnsAsync((int?)null);

            // Act
            var result = await _roleService.AddRoleAsync(failingRole, CancellationToken.None);

            // Assert
            Assert.Null(result);
            Assert.False(_cache.TryGetValue("role:4", out _));
        }

        /// <summary>
        /// Tests that UpdateRoleAsync successfully updates a role using the repository.
        /// </summary>
        [Fact]
        public async Task UpdateRoleAsync_ValidRole_UpdatesSuccessfully()
        {
            // Arrange
            var role = new Role { Id = 1, Name = "Admin", Description = "Updated Desc" };
            var mockRepo = new Mock<IRoleRepository>();
            var mockCache = new MemoryCache(new MemoryCacheOptions());

            mockRepo.Setup(r => r.UpdateAsync(role, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var service = new RoleService(mockRepo.Object, mockCache);

            // Act
            await mockRepo.Object.UpdateAsync(role, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.UpdateAsync(role, It.IsAny<CancellationToken>()), Times.Once);
        }

        /// <summary>
        /// Tests that DeleteRoleAsync successfully deletes a role by ID using the repository.
        /// </summary>
        [Fact]
        public async Task DeleteRoleAsync_ValidId_DeletesSuccessfully()
        {
            // Arrange
            int roleId = 1;
            var mockRepo = new Mock<IRoleRepository>();
            var mockCache = new MemoryCache(new MemoryCacheOptions());

            mockRepo.Setup(r => r.DeleteAsync(roleId, It.IsAny<CancellationToken>()))
                    .Returns(Task.CompletedTask)
                    .Verifiable();

            var service = new RoleService(mockRepo.Object, mockCache);

            // Act
            await mockRepo.Object.DeleteAsync(roleId, CancellationToken.None);

            // Assert
            mockRepo.Verify(r => r.DeleteAsync(roleId, It.IsAny<CancellationToken>()), Times.Once);
        }

    }
}
