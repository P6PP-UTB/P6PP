using Microsoft.Extensions.Caching.Memory;
using Moq;
using UserService.API.Persistence.Entities;
using UserService.API.Persistence.Repositories;
using UserService.API.Services;
using UserService.API.Abstraction;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace UserService.Tests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IRoleRepository> _mockRoleRepo;
        private readonly IMemoryCache _memoryCache;
        private readonly UserService.API.Services.UserService _userService;

        public UserServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockRoleRepo = new Mock<IRoleRepository>();
            _memoryCache = new MemoryCache(new MemoryCacheOptions());
            _userService = new UserService.API.Services.UserService(_mockUserRepo.Object, _memoryCache, _mockRoleRepo.Object);
        }

        [Fact]
        public async Task GetAllUsersAsync_ReturnsUsersWithRoles()
        {
            var roles = new List<Role>
            {
                new Role { Id = 1, Name = "Admin" },
                new Role { Id = 2, Name = "User" }
            };

            var users = new List<User>
            {
                new User { Id = 1, RoleId = 1 },
                new User { Id = 2, RoleId = 2 }
            };

            _mockRoleRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);
            _mockUserRepo.Setup(r => r.GetAllAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(users);
            _mockUserRepo.Setup(r => r.GetTotalUserCountAsync(It.IsAny<CancellationToken>())).ReturnsAsync(2);

            var result = await _userService.GetAllUsersAsync(1, 10, CancellationToken.None);

            Assert.Equal(2, result.TotalCount);
            Assert.All(result.Users, u => Assert.NotNull(u.Role));
        }

        [Fact]
        public async Task GetUserByIdAsync_CachesCorrectly()
        {
            var user = new User { Id = 1, RoleId = 1 };
            var role = new Role { Id = 1, Name = "Admin" };

            _mockUserRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(user);
            _mockRoleRepo.Setup(r => r.GetByIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(role);

            var result = await _userService.GetUserByIdAsync(1, CancellationToken.None);

            Assert.NotNull(result);
            Assert.Equal(1, result!.Id);
            Assert.NotNull(result.Role);
        }

        [Fact]
        public async Task AddUserAsync_SuccessfullyAddsAndCaches()
        {
            var user = new User { Email = "test@example.com" };
            var roles = new List<Role> { new Role { Id = 5, Name = "User" } };

            _mockRoleRepo.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(roles);
            _mockUserRepo.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(123);

            var result = await _userService.AddUserAsync(user, CancellationToken.None);

            Assert.Equal(123, result);
            Assert.Equal(5, user.RoleId);
        }
    }
}
