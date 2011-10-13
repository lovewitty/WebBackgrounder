﻿using System;
using System.Threading;

namespace WebBackgrounder
{
    public class JobWorkersManager : IDisposable
    {
        readonly IJobHost _host;
        readonly Timer _timer;
        readonly IJobCoordinator _coordinator;
        readonly IJob _jobWorker; // We'll make this an enumeration later.

        public JobWorkersManager(IJob jobWorker, IJobHost host) : this(jobWorker, host, new SingleServerJobCoordinator())
        {
        }

        public JobWorkersManager(IJob jobWorker, IJobCoordinator coordinator) : this(jobWorker, new AspNetTaskHost(), coordinator)
        {
        }

        public JobWorkersManager(IJob jobWorker, IJobHost host, IJobCoordinator coordinator)
        {
            _jobWorker = jobWorker;
            _host = host;
            _coordinator = coordinator;
            _timer = new Timer(OnTimerElapsed);
        }

        public void Start()
        {
            _timer.Next(_jobWorker.Interval);
        }

        public void Stop()
        {
            _timer.Dispose();
        }

        void OnTimerElapsed(object sender)
        {
            _timer.Stop();

            try
            {
                RunTask(_jobWorker);
            }
            catch (Exception e)
            {
                // TODO: Log this manually.
            }
            finally
            {
                _timer.Next(_jobWorker.Interval); // Start up again.
            }
        }

        public void RunTask(IJob job)
        {
            lock (_host)
            {
                if (_host.ShuttingDown)
                {
                    return;
                }
                _coordinator.PerformWork(job);
            }
        }

        public void Dispose()
        {
            Stop();
        }
    }
}

