using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.FinishingStationModule
{
    [Export(typeof(IFinishingStationModuleService))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class FinishingStationModuleService : IFinishingStationModuleService
    {
        private readonly IPlatformModuleRepository _modules;

        /// <summary>
        /// Empty constructor for service generation
        /// </summary>
        public FinishingStationModuleService()
        {
        }

        [ImportingConstructor]
        public FinishingStationModuleService(IPlatformModuleRepository moduleRepository)
        {
            _modules = moduleRepository;
        }

        public bool KeepAlive()
        {
            return _modules.GetModulesByType<FinishingStationModule>().First().KeepAlive();
        }

        public bool IsInitialized
        {
            get { return _modules.GetModulesByType<FinishingStationModule>().First().IsInitialized; }
        }

        public void Start()
        {
            _modules.GetModulesByType<FinishingStationModule>().First().Start();
        }

        public void Stop()
        {
            _modules.GetModulesByType<FinishingStationModule>().First().Stop();
        }

        public void ResetAlarms(int moduleInstance)
        {
            _modules.GetModuleByType<FinishingStationModule>(moduleInstance).ResetAlarms();
        }

        public void SubscribeEvents(int moduleInstance)
        {
            var module = _modules.GetModuleByType<FinishingStationModule>(moduleInstance);
            module.SubscribeEvents(() => new FinishingStationModuleServiceFault());

            IFinishingStationModuleServiceEvents callbackChannel =
                OperationContext.Current.GetCallbackChannel<IFinishingStationModuleServiceEvents>();

            module.ZebraIpChanged += callbackChannel.ZebraPrinterIpChanged;
            module.ZplTemplateChanged += callbackChannel.ZplTemplateChanged;
            module.RaiseZplTemplateChanged();
            module.RaiseZebraIpChangedEvent();

        }

        public void UnsubscribeEvents(int moduleInstance)
        {
            _modules.GetModuleByType<FinishingStationModule>(moduleInstance).UnsubscribeEvents(() => new FinishingStationModuleServiceFault());
        }

        public void UpdateZebraPrinterIp(int moduleInstance, string ipAddress)
        {
            _modules.GetModuleByType<FinishingStationModule>(moduleInstance).UpdateZebraPrinterIp(ipAddress);
        }

        public void UpdateZplTemplateUrl(int moduleInstance, string url)
        {
            _modules.GetModuleByType<FinishingStationModule>(moduleInstance).UpdateZplTemplateUrl(url);
        }

    }
}
