using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Bogus;
using FakeItEasy;
using FluentAssertions;
using Jeeves.Server.Domain;
using Jeeves.Server.IntegrationTests.Extensions;
using Jeeves.Server.IntegrationTests.Factories;
using Jeeves.Server.IntegrationTests.Fakes;
using Jeeves.Server.Repositories;
using Jeeves.Server.Representations;
using Jeeves.Server.Representations.Requests;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using NUnit.Framework;

namespace Jeeves.Server.IntegrationTests.Tests.REST
{
    [TestFixture(Category = "/api/v1/users")]
    public class UsersControllerTests
    {
        [TestCase(5)]
        [TestCase(30)]
        public async Task GetAllShouldReturnExactlyTheSameUsers(int usersCount)
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            users.Should().BeEquivalentTo(expectedUsers);
        }

        [Test]
        public async Task GetAllShouldReturnNoContentStatusCodeIfUsersDontExist()
        {
            //Arrange
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.GetUsersAsync()).Returns(new User[0]);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync("/api/v1/users");
        
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task GetByIdShouldReturnSpecifiedUser(int userNumber)
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
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            user.Should().BeEquivalentTo(expectedUser);
        }
        
        [Test]
        public async Task GetByIdShouldReturnNotFoundStatusCodeIfUserDoesntExist()
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
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteByIdShouldReturnNoContentStatusCode()
        {
            //Arrange
            var users = Fakers.User.Generate(10);
            var deletedUser = users[3];
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.DeleteUserAsync(deletedUser.Id)).Returns(Task.CompletedTask);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/users/{deletedUser.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task DeleteByIdShouldDeleteSpecifiedUser(int userNumber)
        {
            //Arrange
            var users = Fakers.User.Generate(10);
            var deletedUser = users[userNumber];
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.DeleteUserAsync(deletedUser.Id)).ReturnsLazily(() => Task.FromResult(users.Remove(deletedUser)));
            A.CallTo(() => usersRepository.FindUserAsync(deletedUser.Id)).ReturnsLazily(() => Task.FromResult(users.Find(u => u.Id == deletedUser.Id)));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/users/{deletedUser.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            using var getResponse = await client.GetAsync($"/api/v1/users/{deletedUser.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteByIdShouldReturnNotFoundStatusCodeIfUserDoesntExist()
        {
            //Arrange
            var deletedUser = Fakers.User.Generate();
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.FindUserAsync(deletedUser.Id)).Returns(Task.FromResult<User>(null));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/users/{deletedUser.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task PostShouldReturnCreatedStatusCode()
        {
            //Arrange
            var user = Fakers.User.Generate();
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.CreateUserAsync(A<User>.Ignored)).Returns(Task.FromResult(user));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            var createUserRequest = new CreateUser { FirstName = user.FirstName, LastName = user.LastName };

            //Act
            using var response = await client.PostAsync("/api/v1/users/", JsonSerializer.Serialize(createUserRequest));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.PathAndQuery.Should().Be($"/api/v1/users/{user.Id}");
        }
        
        [Test]
        public async Task PostShouldCreateUser()
        {
            //Arrange
            var expectedUser = Fakers.User.Generate();
            var usersRepository = A.Fake<IUsersRepository>();
            A.CallTo(() => usersRepository.CreateUserAsync(A<User>.Ignored)).Returns(Task.FromResult(expectedUser));
            A.CallTo(() => usersRepository.FindUserAsync(expectedUser.Id)).Returns(Task.FromResult(expectedUser));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => usersRepository));
            using var client = factory.CreateClient();
            var createUserRequest = new CreateUser { FirstName = expectedUser.FirstName, LastName = expectedUser.LastName };

            //Act
            using var response = await client.PostAsync("/api/v1/users/", JsonSerializer.Serialize(createUserRequest));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            using var getResponse = await client.GetSuccessAsync($"/api/v1/users/{expectedUser.Id}");
            var actualUser = JsonSerializer.Deserialize<JeevesUser>(await getResponse.Content.ReadAsStringAsync());
            actualUser.Should().BeEquivalentTo(expectedUser);
        }
    }
}
