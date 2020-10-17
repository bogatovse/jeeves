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
    [TestFixture(Category = "/api/v1/challenges")]
    public class ChallengesControllerTests
    {
        [TestCase(5)]
        [TestCase(30)]
        public async Task GetAllShouldReturnExactlyTheSameChallenges(int challengesCount)
        {
            //Arrange
            var expectedChallenges = Fakers.Challenge.Generate(challengesCount);
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.GetChallengesAsync()).Returns(expectedChallenges);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync("/api/v1/challenges");
            var challenges = JsonSerializer.Deserialize<JeevesChallenge[]>(await response.Content.ReadAsStringAsync());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            challenges.Should().BeEquivalentTo(expectedChallenges);
        }

        [Test]
        public async Task GetAllShouldReturnNoContentStatusCodeIfChallengesDontExist()
        {
            //Arrange
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.GetChallengesAsync()).Returns(new Challenge[0]);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync("/api/v1/challenges");
        
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task GetByIdShouldReturnSpecifiedChallenge(int challengeNumber)
        {
            //Arrange
            var challenges = Fakers.Challenge.Generate(10);
            var expectedChallenge = challenges[challengeNumber];
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.FindChallengeAsync(expectedChallenge.Id)).Returns(expectedChallenge);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetSuccessAsync($"/api/v1/challenges/{expectedChallenge.Id}");
            var challenge = JsonSerializer.Deserialize<JeevesChallenge>(await response.Content.ReadAsStringAsync());

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            challenge.Should().BeEquivalentTo(expectedChallenge);
        }
        
        [Test]
        public async Task GetByIdShouldReturnNotFoundStatusCodeIfChallengeDoesntExist()
        {
            //Arrange
            var challengeId = Guid.NewGuid();
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.FindChallengeAsync(challengeId)).Returns<Challenge>(null);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.GetAsync($"/api/v1/challenges/{challengeId}");
            
            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteByIdShouldReturnNoContentStatusCode()
        {
            //Arrange
            var challenges = Fakers.Challenge.Generate(10);
            var deletedChallenge = challenges[3];
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.DeleteChallengeAsync(deletedChallenge.Id)).Returns(Task.CompletedTask);
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/challenges/{deletedChallenge.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }
        
        [TestCase(1)]
        [TestCase(4)]
        [TestCase(9)]
        public async Task DeleteByIdShouldDeleteSpecifiedChallenge(int challengeNumber)
        {
            //Arrange
            var challenges = Fakers.Challenge.Generate(10);
            var deletedChallenge = challenges[challengeNumber];
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.DeleteChallengeAsync(deletedChallenge.Id)).ReturnsLazily(() => Task.FromResult(challenges.Remove(deletedChallenge)));
            A.CallTo(() => challengesRepository.FindChallengeAsync(deletedChallenge.Id)).ReturnsLazily(() => Task.FromResult(challenges.Find(u => u.Id == deletedChallenge.Id)));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/challenges/{deletedChallenge.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NoContent);
            using var getResponse = await client.GetAsync($"/api/v1/challenges/{deletedChallenge.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task DeleteByIdShouldReturnNotFoundStatusCodeIfChallengeDoesntExist()
        {
            //Arrange
            var deletedChallenge = Fakers.Challenge.Generate();
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.FindChallengeAsync(deletedChallenge.Id)).Returns(Task.FromResult<Challenge>(null));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            
            //Act
            using var response = await client.DeleteAsync($"/api/v1/challenges/{deletedChallenge.Id}");

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
        
        [Test]
        public async Task PostShouldReturnCreatedStatusCode()
        {
            //Arrange
            var challenge = Fakers.Challenge.Generate();
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.CreateChallengeAsync(A<Challenge>.Ignored)).Returns(Task.FromResult(challenge));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            var createChallengeRequest = new CreateChallenge() { Name = challenge.Name };

            //Act
            using var response = await client.PostAsync("/api/v1/challenges/", JsonSerializer.Serialize(createChallengeRequest));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            response.Headers.Location.PathAndQuery.Should().Be($"/api/v1/challenges/{challenge.Id}");
        }
        
        [Test]
        public async Task PostShouldCreateChallenge()
        {
            //Arrange
            var challenge = Fakers.Challenge.Generate();
            var challengesRepository = A.Fake<IChallengesRepository>();
            A.CallTo(() => challengesRepository.CreateChallengeAsync(A<Challenge>.Ignored)).Returns(Task.FromResult(challenge));
            A.CallTo(() => challengesRepository.FindChallengeAsync(challenge.Id)).Returns(Task.FromResult(challenge));
            using var factory = new JeevesWebApplicationFactory(services => services.SwapSingleton(provider => challengesRepository));
            using var client = factory.CreateClient();
            var createChallengeRequest = new CreateChallenge() { Name = challenge.Name };

            //Act
            using var response = await client.PostAsync("/api/v1/challenges/", JsonSerializer.Serialize(createChallengeRequest));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            using var getResponse = await client.GetSuccessAsync($"/api/v1/challenges/{challenge.Id}");
            var actualChallenge = JsonSerializer.Deserialize<JeevesChallenge>(await getResponse.Content.ReadAsStringAsync());
            actualChallenge.Should().BeEquivalentTo(challenge);
        }
    }
}
