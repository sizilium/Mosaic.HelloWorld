using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleA
{
    [Export(typeof(IDemoModuleAService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class DemoModuleAService : IDemoModuleAService
    {
        private readonly IPlatformModuleRepository _modules;

        /// <summary>
        /// Empty constructor for service generation
        /// </summary>
        public DemoModuleAService()
        {
        }

        [ImportingConstructor]
        public DemoModuleAService(IPlatformModuleRepository moduleRepository)
        {
            _modules = moduleRepository;
        }

        public bool KeepAlive()
        {
            return _modules.GetModulesByType<DemoModuleA>().First().KeepAlive();
        }

        public bool IsInitialized
        {
            get { return _modules.GetModulesByType<DemoModuleA>().First().IsInitialized; }
        }

        public void Start()
        {
            _modules.GetModulesByType<DemoModuleA>().First().Start();
        }

        public void Stop()
        {
            _modules.GetModulesByType<DemoModuleA>().First().Stop();
        }

        public void ResetAlarms(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleA>(moduleInstance).ResetAlarms();
        }

        public void SubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleA>(moduleInstance).SubscribeEvents(() => new IDemoModuleAServiceFault());
        }

        public void UnsubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<DemoModuleA>(moduleInstance).UnsubscribeEvents(() => new IDemoModuleAServiceFault());
        }
    }
}
