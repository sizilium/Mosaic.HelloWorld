using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.EntryStation
{
    [Export(typeof(IEntryStationService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class EntryStationService : IEntryStationService
    {
        private readonly IPlatformModuleRepository _modules;

        /// <summary>
        /// Empty constructor for service generation
        /// </summary>
        public EntryStationService()
        {
        }

        [ImportingConstructor]
        public EntryStationService(IPlatformModuleRepository moduleRepository)
        {
            _modules = moduleRepository;
        }

        public bool KeepAlive()
        {
            return _modules.GetModulesByType<EntryStation>().First().KeepAlive();
        }

        public bool IsInitialized
        {
            get { return _modules.GetModulesByType<EntryStation>().First().IsInitialized; }
        }

        public void Start()
        {
            _modules.GetModulesByType<EntryStation>().First().Start();
        }

        public void Stop()
        {
            _modules.GetModulesByType<EntryStation>().First().Stop();
        }

        public void ResetAlarms(int moduleInstance)
        {
            _modules.GetModuleByType<EntryStation>(moduleInstance).ResetAlarms();
        }

        public void SubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<EntryStation>(moduleInstance).SubscribeEvents(() => new IEntryStationServiceFault());
        }

        public void UnsubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<EntryStation>(moduleInstance).UnsubscribeEvents(() => new IEntryStationServiceFault());
        }
    }
}
