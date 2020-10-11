using System;
using System.Text.Json;
using System.Threading.Tasks;
using FakeItEasy;
using FluentAssertions;
using Jeeves.Server.Domain;
using Jeeves.Server.IntegrationTests.Extensions;
using Jeeves.Server.IntegrationTests.Factories;
using Jeeves.Server.IntegrationTests.Fakes;
using Jeeves.Server.Repositories;
using Jeeves.Server.Representations;
using Microsoft.AspNetCore.Http;
using NUnit.Framework;

namespace Jeeves.Server.IntegrationTests
{
    [TestFixture]
    public class UsersControllerTests
    {
        [TestCase(5)]
        [TestCase(30)]
        public async Task ShouldReturnExactlyTheSameUsers(int usersCount)
        {
            //Arrange
            var expectedUsers = Fakers.User.Generate(usersCount);
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.GetUsersAsync()).Returns(expectedUsers);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync("/api/v1/users");
            var users = JsonSerializer.Deserialize<JeevesUser[]>(await response.Content.ReadAsStringAsync());

            //Assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            users.Should().BeEquivalentTo(expectedUsers);
        }
        
        [Test]
        public async Task ShouldReturnNoContentIfUsersDontExist()
        {
            //Arrange
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.GetUsersAsync()).Returns(new User[0]);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync("/api/v1/users");
        
            //Assert
            response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task ShouldReturnUserById(int userNumber)
        {
            //Arrange
            var users = Fakers.User.Generate(10);
            var expectedUser = users[userNumber];
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.FindUserAsync(expectedUser.Id)).Returns(expectedUser);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync($"/api/v1/users/{expectedUser.Id}");
            var user = JsonSerializer.Deserialize<JeevesUser>(await response.Content.ReadAsStringAsync());

            //Assert
            response.StatusCode.Should().Be(StatusCodes.Status200OK);
            user.Should().BeEquivalentTo(expectedUser);
        }
        
        [Test]
        public async Task ShouldReturnNotFoundIfUserDoesntExist()
        {
            //Arrange
            var userId = Guid.NewGuid();
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.FindUserAsync(userId)).Returns<User>(null);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetAsync($"/api/v1/users/{userId}");
            
            //Assert
            response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task ShouldDeleteUserById(int userNumber)
        {
            //Arrange
            var users = Fakers.User.Generate(10);
            var deletedUser = users[userNumber];
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.DeleteUserAsync(deletedUser.Id)).Returns(Task.CompletedTask);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/users/{deletedUser.Id}");

            //Assert
            response.StatusCode.Should().Be(StatusCodes.Status204NoContent);
            A.CallTo(() => usersRepository.DeleteUserAsync(deletedUser.Id)).MustHaveHappenedOnceExactly();
        }
    }
}