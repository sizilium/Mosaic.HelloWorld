using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ServiceModel;
using Cimpress.ACS.FP3.UIInfrastructure.AlarmManagementService;
using Cimpress.ACS.FP3.UIInfrastructure.LogService;
using VP.FF.PT.Common.GuiEssentials.Wcf;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Services.LogFiltering;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;
using VP.FF.PT.Common.WpfInfrastructure.Threading;

namespace Cimpress.ACS.FP3.UIInfrastructure.AlarmService
{
    [Export(typeof(IDuplexCommunicationObjectFactory<IAlarmManagementService, IAlarmManagementServiceCallback>))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class AlarmManagementServiceClientFactory : IDuplexCommunicationObjectFactory<IAlarmManagementService, IAlarmManagementServiceCallback>
    {
        public IAlarmManagementService CreateCommunicationObject(IAlarmManagementServiceCallback callbackInstance)
        {
            return new AlarmManagementServiceClient(new InstanceContext(callbackInstance));
        }
    }

    /// <summary>
    /// The <see cref="WcfAlarmManagementFactory"/> is capable to create a 
    /// <see cref="AlarmSummaryViewModel"/> and wire it up with all elements needed
    /// to communicate with Sabers alarm mangement over WCF.
    /// </summary>
    [Export(typeof(IAlarmManagementFactory))]
    public class WcfAlarmManagementFactory : IAlarmManagementFactory
    {
        private readonly IDuplexClientFactory<IAlarmManagementService, IAlarmManagementServiceCallback> _alarmManagerClientFactory;
        private readonly IClientFactory<ILogService> _logServiceClientFactory;
        private readonly IProvideLogFilters _provideLogFilterRules;
        private readonly IDispatcher _dispatcher;
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new <see cref="WcfAlarmManagementFactory"/> instance.
        /// </summary>
        /// <param name="alarmManagerClientFactory">An instance capable of creating a duplex client to communicate with the alarm management wcf service.</param>
        /// <param name="logServiceClientFactory">An instance capable of creating a client to communicate with the log wcf service.</param>
        /// <param name="provideLogFilterRules">An instance capable of getting filter rules for the log part of the alarm management view model.</param>
        /// <param name="dispatcher">A dispatcher.</param>
        /// <param name="logger">The logger.</param>
        [ImportingConstructor]
        public WcfAlarmManagementFactory(
            IDuplexClientFactory<IAlarmManagementService, IAlarmManagementServiceCallback> alarmManagerClientFactory,
            IClientFactory<ILogService> logServiceClientFactory,
            IProvideLogFilters provideLogFilterRules,
            IDispatcher dispatcher,
            ILogger logger)
        {
            _alarmManagerClientFactory = alarmManagerClientFactory;
            _logServiceClientFactory = logServiceClientFactory;
            _provideLogFilterRules = provideLogFilterRules;
            _dispatcher = dispatcher;
            _logger = logger;
            _logger.Init("AlarmManagement");
        }

        public AlarmSummaryViewModel CreateAlarmSummaryViewModel(string module)
        {
            var logServiceAdapter = new LogServiceAdapter(_logServiceClientFactory, _logger);
            var alarmServiceAdapter = new AlarmManagerServiceAdapter(module, _alarmManagerClientFactory, _dispatcher, _logger);
            return new AlarmSummaryViewModel(module, alarmServiceAdapter, alarmServiceAdapter, logServiceAdapter, _provideLogFilterRules, _logger);
        }

        public AlarmSummaryViewModel CreateAlarmSummaryViewModel(ICollection<string> modules)
        {
            var logServiceAdapter = new LogServiceAdapter(_logServiceClientFactory, _logger);
            var alarmServiceAdapter = new AlarmManagerServiceAdapter(modules, _alarmManagerClientFactory, _dispatcher, _logger);
            return new AlarmSummaryViewModel(modules, alarmServiceAdapter, alarmServiceAdapter, logServiceAdapter, _provideLogFilterRules, _logger);
        }
    }
}