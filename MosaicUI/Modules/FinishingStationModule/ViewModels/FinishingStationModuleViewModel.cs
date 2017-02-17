using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading.Tasks;
using MosaicSample.UI.FinishingStationModule.ServiceReference;
using Cimpress.ACS.FP3.UIInfrastructure.AlarmService;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.GuiEssentials.Wcf;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.Entities.DTOs;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;
using IEventAggregator = Caliburn.Micro.IEventAggregator;

namespace MosaicSample.UI.FinishingStationModule.ViewModels
{
    [Export]
    [CallbackBehavior]
    public class FinishingStationModuleViewModel : ModuleControlScreen, IFinishingStationModuleServiceCallback
    {
        private readonly ILogger _logger;
        private readonly IAlarmManagementFactory _alarmManagementFactory;
        private readonly IEventAggregator _eventAggregator;
        private string _zebraIp;
        private string _zplTemplateUrl;

        private FinishingStationModuleServiceClient _client;

        private IDisposable _keepAlive;

        [ImportingConstructor]
        public FinishingStationModuleViewModel(
            ILogger logger,
            IAlarmManagementFactory alarmManagementFactory,
            IEventAggregator eventAggregator)
        {
            logger.Init(GetType());
            _logger = logger;
            _alarmManagementFactory = alarmManagementFactory;
            _eventAggregator = eventAggregator;
            _messageViewModel = new MessageViewModel();
        }

        public async override Task Initialize()
        {
            _logger.Info("Initialize() of FinishingModuleViewModel called");

            DisplayName = "Finishing Module (" + ModuleKey + ")";

            await EnsureValidWcfClient();

            AlarmSummaryViewModel = _alarmManagementFactory.CreateAlarmSummaryViewModel(ModuleKey);

            _eventAggregator.Subscribe(this);
        }

        private async Task EnsureValidWcfClient()
        {
            await this.StartWcfClient<FinishingStationModuleServiceClient, FinishingStationModuleServiceFault, IFinishingStationModuleService>(_client, c => new FinishingStationModuleServiceClient(c), _logger, async c =>
            {
                _client = c;
                try
                {
                    if (_keepAlive != null)
                    {
                        _keepAlive.Dispose();
                    }
                    if (!await c.get_IsInitializedAsync())
                    {
                        throw new Exception("Module is not initialized.");
                    }
                    await c.SubscribeEventsAsync(ModuleInstance);
                    _keepAlive = this.KeepAlive<FinishingStationModuleServiceFault>(c.KeepAliveAsync, _logger);
                    IsEnabled = true;
                }
                catch (Exception ex)
                {
                    DisableModule(ex, _logger);
                }
            });
        }

        public string ZPLTemplateFileLocation
        {
            
            get { return _zplTemplateUrl; }

            set
            {
                WcfCommunicationHelper.WcfCall<FinishingStationModuleServiceFault>(
                    () => _client.UpdateZplTemplateUrlAsync(ModuleInstance, value), EnsureValidWcfClient, _logger);
            }
        }

        public void ZplTemplateChanged(string url)
        {
            _zplTemplateUrl = url;
            NotifyOfPropertyChange(() => ZPLTemplateFileLocation);
        }

        public string ZebraPrinterIP
        {
            get { return _zebraIp; }
            set
            {
                WcfCommunicationHelper.WcfCall<FinishingStationModuleServiceFault>(
                    () => _client.UpdateZebraPrinterIpAsync(ModuleInstance, value), EnsureValidWcfClient, _logger);
            }
        }

        public void ZebraPrinterIpChanged(string ip)
        {
            _zebraIp = ip;
            NotifyOfPropertyChange(() => ZebraPrinterIP);
        }

        public override string IconKey
        {
            get
            {
                return "FinishingModule";
            }
        }

        public override int SortOrder
        {
            get
            {
                return 10;
            }
        }

        public override async Task Shutdown()
        {
            _logger.Info("Shutdown() of FinishingModuleViewModel called");
            if (_keepAlive != null)
            {
                _keepAlive.Dispose();
            }

            await WcfCommunicationHelper.WcfCall<FinishingStationModuleServiceFault>(() => _client.UnsubscribeEventsAsync(ModuleInstance), () => Task.FromResult(true), _logger);
            _client.Close();

        }

        public void ModuleStateChanged(PlatformModuleDTO module)
        {
            RefreshPlatformModule(module);
        }

        public void MetricsChanged(MetricsDTO metrics)
        {
            ThroughputMin = 0;
            ThroughputMax = 1;
            Throughput = metrics.Throughput;
            UpTime = (int)metrics.UpTime.TotalMinutes;
            DownTime = (int)metrics.DownTime.TotalMinutes;
            OverallItemCount = metrics.OverallItemCount;
        }

        public async Task ResetAlarms(string module)
        {
            base.ResetAlarms();

            await WcfCommunicationHelper.WcfCall<FinishingStationModuleServiceFault>(() => _client.ResetAlarmsAsync(ModuleInstance), EnsureValidWcfClient, _logger);
        }

        public override async void Start()
        {
            base.Start();

            await WcfCommunicationHelper.WcfCall<FinishingStationModuleServiceFault>(() => _client.StartAsync(), EnsureValidWcfClient, _logger);
        }

        public override async void Stop()
        {
            base.Stop();

            await WcfCommunicationHelper.WcfCall<FinishingStationModuleServiceFault>(() => _client.StopAsync(), EnsureValidWcfClient, _logger);
        }
    }
}