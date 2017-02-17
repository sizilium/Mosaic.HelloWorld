using System;
using System.ComponentModel.Composition;
using System.Linq;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.PlatformEssentials.Events;
using VP.FF.PT.Common.PlatformEssentials.JobManagement;

namespace MosaicSample.Infrastructure
{
    [Export(typeof(IJobManager))]
    public class MosaicSampleJobManager : JobManager
    {
        [ImportingConstructor]
        public MosaicSampleJobManager(IEventAggregator eventAggregator)
        {
            eventAggregator.GetEvent<TryStartJobEvent>().Subscribe(AcceptJob);
        }

        private void AcceptJob(TryStartJobEvent newJobEvent)
        {
            var job = Jobs.GetJobs(j => j.JobId == newJobEvent.JobId).First();
            job.JobItems.First().State = JobItemState.InProduction;
        }
    }
}