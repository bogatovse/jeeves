using System;
using System.Net;
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
using Jeeves.Server.Representations.Requests;
using NUnit.Framework;

namespace Jeeves.Server.IntegrationTests.Tests.REST
{
    [TestFixture(Category = "/api/v1/challenges/{challengeId}/attempts")]
    public class AttemptsControllerTests
    {
        [TestCase(5)]
        [TestCase(30)]
        public async Task GetAllShouldReturnExactlyTheSameAttempts(int attemptsCount)
        {
            //Arrange
            var expectedAttempts = Fakers.Attempt.Generate(attemptsCount);
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.GetAttemptsAsync()).Returns(expectedAttempts);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts");
            var attempts = JsonSerializer.Deserialize<JeevesAttempt[]>(await response.Content.ReadAsStringAsync());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            attempts.Should().BeEquivalentTo(expectedAttempts);
        }

        [Test]
        public async Task GetAllShouldReturnNoContentStatusCodeIfAttemptsDontExist()
        {
            //Arrange
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.GetAttemptsAsync()).Returns(new ChallengeAttempt[0]);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts");
        
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task GetByIdShouldReturnSpecifiedAttempt(int attemptNumber)
        {
            //Arrange
            var attempts = Fakers.Attempt.Generate(10);
            var expectedAttempt = attempts[attemptNumber];
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.FindAttemptAsync(expectedAttempt.Id)).Returns(expectedAttempt);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts/{expectedAttempt.Id}");
            var attempt = JsonSerializer.Deserialize<JeevesAttempt>(await response.Content.ReadAsStringAsync());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            attempt.Should().BeEquivalentTo(expectedAttempt);
        }
        
        [Test]
        public async Task GetByIdShouldReturnNotFoundStatusCodeIfAttemptDoesntExist()
        {
            //Arrange
            var attemptId = Guid.NewGuid();
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.FindAttemptAsync(attemptId)).Returns<ChallengeAttempt>(null);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts/{attemptId}");
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteByIdShouldReturnNoContentStatusCode()
        {
            //Arrange
            var attempts = Fakers.Attempt.Generate(10);
            var deletedAttempt = attempts[3];
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.DeleteAttemptAsync(deletedAttempt.Id)).Returns(Task.CompletedTask);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts/{deletedAttempt.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task DeleteByIdShouldDeleteSpecifiedAttempt(int attemptNumber)
        {
            //Arrange
            var attempts = Fakers.Attempt.Generate(10);
            var deletedAttempt = attempts[attemptNumber];
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.DeleteAttemptAsync(deletedAttempt.Id)).ReturnsLazily(() => Task.FromResult(attempts.Remove(deletedAttempt)));
            A.CallTo(() => attemptsRepository.FindAttemptAsync(deletedAttempt.Id)).ReturnsLazily(() => Task.FromResult(attempts.Find(u => u.Id == deletedAttempt.Id)));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts/{deletedAttempt.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            using var getResponse = await client.GetAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts/{deletedAttempt.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteByIdShouldReturnNotFoundStatusCodeIfAttemptDoesntExist()
        {
            //Arrange
            var deletedAttempt = Fakers.Attempt.Generate();
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.FindAttemptAsync(deletedAttempt.Id)).Returns(Task.FromResult<ChallengeAttempt>(null));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/challenges/{Guid.NewGuid()}/attempts/{deletedAttempt.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task PostShouldReturnCreatedStatusCode()
        {
            //Arrange
            var attempt = Fakers.Attempt.Generate();
            var challengeId = Guid.NewGuid();
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.CreateAttemptAsync(A<ChallengeAttempt>.Ignored)).Returns(Task.FromResult(attempt));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            var createAttemptRequest = new CreateAttempt() {Solution = new byte[0]};

            //Act
            using var response = await client.PostAsync($"/api/v1/challenges/{challengeId}/attempts", JsonSerializer.Serialize(createAttemptRequest));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.PathAndQuery.Should().Be($"/api/v1/challenges/{challengeId}/attempts/{attempt.Id}");
        }
        
        [Test]
        public async Task PostShouldCreateAttempt()
        {
            //Arrange
            var attempt = Fakers.Attempt.Generate();
            var challengeId = Guid.NewGuid();
            var attemptsRepository = A.Fake<IAttemptsRepository>();
            A.CallTo(() => attemptsRepository.CreateAttemptAsync(A<ChallengeAttempt>.Ignored)).Returns(Task.FromResult(attempt));
            A.CallTo(() => attemptsRepository.FindAttemptAsync(attempt.Id)).Returns(Task.FromResult(attempt));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => attemptsRepository));
            using var client = factory.CreateClient();
            var createAttemptRequest = new CreateAttempt() {Solution = new byte[0]};

            //Act
            using var response = await client.PostAsync($"/api/v1/challenges/{challengeId}/attempts", JsonSerializer.Serialize(createAttemptRequest));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            using var getResponse = await client.GetSuccessAsync($"/api/v1/challenges/{challengeId}/attempts/{attempt.Id}");
            var actualAttempt = JsonSerializer.Deserialize<JeevesAttempt>(await getResponse.Content.ReadAsStringAsync());
            actualAttempt.Should().BeEquivalentTo(attempt);
        }
    }
}