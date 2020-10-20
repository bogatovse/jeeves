using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FakeItEasy;
using Jeeves.Server.Domain;
using Jeeves.Server.Workers;
using NUnit.Framework;

namespace Jeeves.Server.UnitTests
{
    public class WorkersForemanTests
    {
        [Test]
        public async Task ExecuteShouldCallOnStartedForRunningTask()
        {
            //Arrange
            var attempt = new ChallengeAttempt();
            var workerCreator = A.Fake<IWorkerCreator>();
            var worker = A.Fake<IWorker>();
            var listener = A.Fake<IChallengeAttemptListener>();
            using var completedEvent = new ManualResetEvent(false);
            A.CallTo(() => listener.OnCompletedAsync()).ReturnsLazily(() => Task.FromResult(completedEvent.Set()));
            A.CallTo(() => workerCreator.CreateWorkerAsync(A<CancellationToken>.Ignored)).Returns(worker);
            A.CallTo(() => worker.ExecuteAsync(attempt, listener, A<CancellationToken>.Ignored)).Returns(Task.CompletedTask);
            using var foreman = new WorkersForeman(workerCreator);

            //Act
            await foreman.StartAsync(CancellationToken.None);
            await foreman.EnqueueWorkAsync(attempt, listener, CancellationToken.None);
            completedEvent.WaitOne(TimeSpan.FromSeconds(5));
            await foreman.StopAsync(CancellationToken.None);
                
            //Assert
            A.CallTo(() => listener.OnStartedAsync()).MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public async Task ExecuteShouldCallOnCompletedForSuccessfulTask()
        {
            //Arrange
            var attempt = new ChallengeAttempt();
            var workerCreator = A.Fake<IWorkerCreator>();
            var worker = A.Fake<IWorker>();
            var listener = A.Fake<IChallengeAttemptListener>();
            using var completedEvent = new ManualResetEvent(false);
            A.CallTo(() => listener.OnCompletedAsync()).ReturnsLazily(() => Task.FromResult(completedEvent.Set()));
            A.CallTo(() => workerCreator.CreateWorkerAsync(A<CancellationToken>.Ignored)).Returns(worker);
            A.CallTo(() => worker.ExecuteAsync(attempt, listener, A<CancellationToken>.Ignored)).Returns(Task.CompletedTask);
            using var foreman = new WorkersForeman(workerCreator);

            //Act
            await foreman.StartAsync(CancellationToken.None);
            await foreman.EnqueueWorkAsync(attempt, listener, CancellationToken.None);
            completedEvent.WaitOne(TimeSpan.FromSeconds(5));
            await foreman.StopAsync(CancellationToken.None);
                
            //Assert
            A.CallTo(() => listener.OnCompletedAsync()).MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public async Task ExecuteShouldCallOnFailedForFailedTask()
        {
            //Arrange
            var attempt = new ChallengeAttempt();
            var workerCreator = A.Fake<IWorkerCreator>();
            var worker = A.Fake<IWorker>();
            var listener = A.Fake<IChallengeAttemptListener>();
            using var completedEvent = new ManualResetEvent(false);
            A.CallTo(() => listener.OnCompletedAsync()).ReturnsLazily(() => Task.FromResult(completedEvent.Set()));
            A.CallTo(() => workerCreator.CreateWorkerAsync(A<CancellationToken>.Ignored)).Returns(worker);
            A.CallTo(() => worker.ExecuteAsync(attempt, listener, A<CancellationToken>.Ignored)).Throws<Exception>();
            using var foreman = new WorkersForeman(workerCreator);

            //Act
            await foreman.StartAsync(CancellationToken.None);
            await foreman.EnqueueWorkAsync(attempt, listener, CancellationToken.None);
            completedEvent.WaitOne(TimeSpan.FromSeconds(5));
            await foreman.StopAsync(CancellationToken.None);
                
            //Assert
            A.CallTo(() => listener.OnFailedAsync(A<Exception>.Ignored)).MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public async Task ExecuteShouldCallOnCancelledWhenHostIsTerminating()
        {
            //Arrange
            var attempt = new ChallengeAttempt();
            var workerCreator = A.Fake<IWorkerCreator>();
            var worker = A.Fake<IWorker>();
            var listener = A.Fake<IChallengeAttemptListener>();
            using var completedEvent = new ManualResetEvent(false);
            A.CallTo(() => workerCreator.CreateWorkerAsync(A<CancellationToken>.Ignored)).Returns(worker);
            A.CallTo(() => worker.ExecuteAsync(attempt, listener, A<CancellationToken>.Ignored)).ReturnsLazily(async args =>
            {
                var cancellationToken = args.GetArgument<CancellationToken>(2);
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                }
                cancellationToken.ThrowIfCancellationRequested();
            });
            using var foreman = new WorkersForeman(workerCreator);

            //Act
            await foreman.StartAsync(CancellationToken.None);
            await foreman.EnqueueWorkAsync(attempt, listener, CancellationToken.None);
            await foreman.StopAsync(CancellationToken.None);
                
            //Assert
            A.CallTo(() => listener.OnCancelledAsync()).MustHaveHappenedOnceExactly();
        }
        
        [Test]
        public async Task StopWorkShouldCancelRunningTask()
        {
            //Arrange
            using var startedEvent = new ManualResetEvent(false);
            var attempt = new ChallengeAttempt();
            var workerCreator = A.Fake<IWorkerCreator>();
            var worker = A.Fake<IWorker>();
            var listener = A.Fake<IChallengeAttemptListener>();
            using var completedEvent = new ManualResetEvent(false);
            A.CallTo(() => workerCreator.CreateWorkerAsync(A<CancellationToken>.Ignored)).Returns(worker);
            A.CallTo(() => worker.ExecuteAsync(attempt, listener, A<CancellationToken>.Ignored)).ReturnsLazily(async args =>
            {
                var cancellationToken = args.GetArgument<CancellationToken>(2);
                while (!cancellationToken.IsCancellationRequested)
                {
                    await Task.Delay(1000, cancellationToken);
                    startedEvent.Set();
                }
                cancellationToken.ThrowIfCancellationRequested();
            });
            using var foreman = new WorkersForeman(workerCreator);

            //Act
            await foreman.StartAsync(CancellationToken.None);
            await foreman.EnqueueWorkAsync(attempt, listener, CancellationToken.None);
            startedEvent.WaitOne(TimeSpan.FromSeconds(5));
            await foreman.StopWorkAsync(attempt.Id);
                
            //Assert
            A.CallTo(() => listener.OnCancelledAsync()).MustHaveHappenedOnceExactly();
        }
    }
}