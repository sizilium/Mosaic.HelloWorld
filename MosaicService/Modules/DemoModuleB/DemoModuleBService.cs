using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleB
{
    [Export(typeof(IDemoModuleBService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DemoModuleBService : IDemoModuleBService
    {
        private readonly IPlatformModuleRepository _modules;

        /// <summary>
        /// Empty constructor for service generation
        /// </summary>
        public DemoModuleBService()
        {
        }

        [ImportingConstructor]
        public DemoModuleBService(IPlatformModuleRepository moduleRepository)
        {
            _modules = moduleRepository;
        }

        public bool KeepAlive()
        {
            return _modules.GetModulesByType<DemoModuleB>().First().KeepAlive();
        }

        public bool IsInitialized
        {
            get { return _modules.GetModulesByType<DemoModuleB>().First().IsInitialized; }
        }

        public void Start()
        {
            _modules.GetModulesByType<DemoModuleB>().First().Start();
        }

        public void Stop()
        {
            _modules.GetModulesByType<DemoModuleB>().First().Stop();
        }

        public void ResetAlarms(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleB>(moduleInstance).ResetAlarms();
        }

        public void SubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleB>(moduleInstance).SubscribeEvents(() => new IDemoModuleBServiceFault());
        }

        public void UnsubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleB>(moduleInstance).UnsubscribeEvents(() => new IDemoModuleBServiceFault());
        }
    }
}
