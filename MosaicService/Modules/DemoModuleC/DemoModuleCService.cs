using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleC
{
    [Export(typeof(IDemoModuleCService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DemoModuleCService : IDemoModuleCService
    {
        private readonly IPlatformModuleRepository _modules;

        /// <summary>
        /// Empty constructor for service generation
        /// </summary>
        public DemoModuleCService()
        {
        }

        [ImportingConstructor]
        public DemoModuleCService(IPlatformModuleRepository moduleRepository)
        {
            _modules = moduleRepository;
        }

        public bool KeepAlive()
        {
            return _modules.GetModulesByType<DemoModuleC>().First().KeepAlive();
        }

        public bool IsInitialized
        {
            get { return _modules.GetModulesByType<DemoModuleC>().First().IsInitialized; }
        }

        public void Start(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleC>(moduleInstance).Start();
        }

        public void Stop(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleC>(moduleInstance).Stop();
        }

        public void ResetAlarms(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleC>(moduleInstance).ResetAlarms();
        }

        public void SubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleC>(moduleInstance).SubscribeEvents(() => new IDemoModuleCServiceFault());
        }

        public void UnsubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleC>(moduleInstance).UnsubscribeEvents(() => new IDemoModuleCServiceFault());
        }
    }
}
