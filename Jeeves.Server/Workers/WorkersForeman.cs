using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Jeeves.Server.Domain;
using Jeeves.Server.Workers.Tasks;
using Microsoft.Extensions.Hosting;

namespace Jeeves.Server.Workers
{
    public interface IWorkListener
    {
        Task OnStartedAsync();
        Task OnCompletedAsync();
        Task OnFailedAsync(Exception e);
        Task OnCancelledAsync();
    }

    public interface IChallengeAttemptListener : IWorkListener
    {
        Task OnMessageReceivedAsync(string message);
        Task OnMessagesReceivedAsync(IEnumerable<string> messages);
        Task OnCpuUsageChangedAsync();
        Task OnRamUsageChangedAsync();
    }
    
    public interface IWorkersForeman
    {
        Task EnqueueWorkAsync(ChallengeAttempt attempt, IChallengeAttemptListener listener, CancellationToken cancellationToken);
        Task StopWorkAsync(Guid workId);
    }
    
    public class WorkersForeman : BackgroundService, IWorkersForeman
    {
        private readonly IWorkerCreator _workerCreator;
        private readonly BufferBlock<ChallengeAttemptTask> _workQueue;
        private readonly ConcurrentDictionary<Guid, (Task WorkTask, CancellationTokenSource Cancellation)> _runningWorkers;

        public WorkersForeman(IWorkerCreator workerCreator)
        {
            _workQueue = new BufferBlock<ChallengeAttemptTask>();
            _workerCreator = workerCreator;
            _runningWorkers = new ConcurrentDictionary<Guid, (Task WorkTask, CancellationTokenSource Cancellation)>();
        }
        
        public async Task EnqueueWorkAsync(ChallengeAttempt attempt, IChallengeAttemptListener listener, CancellationToken cancellationToken)
        {
            var attemptTask = new ChallengeAttemptTask { Attempt = attempt, Listener = listener };
            await _workQueue.SendAsync(attemptTask, cancellationToken);
        }

        public async Task StopWorkAsync(Guid workId)
        {
            //TODO: It should also remove work from queue if it's there
            if (_runningWorkers.Remove(workId, out var work))
            {
                try
                {
                    work.Cancellation.Cancel();
                    await work.WorkTask;
                }
                finally
                {
                    work.Cancellation.Dispose();
                }
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var attemptTask = await _workQueue.ReceiveAsync(stoppingToken);
                    var cancellation = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
                    //TODO: Limit number of workers
                    var worker = await _workerCreator.CreateWorkerAsync(stoppingToken);
                    var task = ExecuteAsync(worker, attemptTask.Attempt, attemptTask.Listener, cancellation.Token);
                    _runningWorkers.TryAdd(attemptTask.Attempt.Id, (task, cancellation));
                }
            }
            finally
            {
                _workQueue.Complete();
                await StopAllWork();
            }
        }

        private async Task ExecuteAsync(IWorker worker, ChallengeAttempt attempt, IChallengeAttemptListener listener, CancellationToken cancellationToken)
        {
            try
            {
                await listener.OnStartedAsync();
                await worker.ExecuteAsync(attempt, listener, cancellationToken);
                await listener.OnCompletedAsync();
            }
            catch (OperationCanceledException)
            {
                await listener.OnCancelledAsync();
            }
            catch (Exception e)
            {
                await listener.OnFailedAsync(e);
            }
            finally
            {
                worker.Dispose();
            }
        }
        
        private async Task StopAllWork()
        {
            await Task.WhenAll(_runningWorkers.Select(worker => StopWorkAsync(worker.Key)));
        }
    }
}