using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.Infrastructure.Wcf;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.ConfigurationService;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.PlatformEssentials.Entities.DTOs;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow.DTOs;

namespace MosaicSample.CommonServices
{
    [Export(typeof(ICommonServices))]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class CommonServices : ICommonServices
    {
        private readonly ILogger _logger;
        private readonly IPlatformModuleRepository _moduleRepository;
        private readonly ICallbackChannelProvider _callbackChannelProvider;
        private readonly ISafeEventRaiser _eventRaiser;
        private readonly IModuleBusManager _moduleBusManager;

        private static Action<PlatformModuleDTO> _platformModuleStateChanged = delegate { };
        private static Action<string> _moduleInitializationStarted = delegate { };

        private bool _isInitialized;
        private readonly IConfigurationService _configurationService;

        /// <summary>
        // Empty constructor is needed to download service interface at design time.
        /// </summary>
        public CommonServices()
        {
        }

        [ImportingConstructor]
        public CommonServices(
            ILogger logger,
            IPlatformModuleRepository moduleRepository,
            IConfigurationService configurationService,
            ICallbackChannelProvider callbackChannelProvider,
            ISafeEventRaiser eventRaiser,
            IModuleBusManager moduleBusManager)
        {
            _logger = logger;
            _moduleRepository = moduleRepository;
            _configurationService = configurationService;
            _callbackChannelProvider = callbackChannelProvider;
            _eventRaiser = eventRaiser;
            _moduleBusManager = moduleBusManager;

            _moduleBusManager.ModuleInitializationStarted += HandleModuleInitializationStarted;
        }

        public void SubscribeEvents()
        {
            Initialize();
            try
            {
                _logger.DebugFormat("Remote subscriber subscribes event on '{0}'.", this);
                var subscriber = _callbackChannelProvider.GetCallbackChannel<ICommonServicesEvents>();
                _moduleInitializationStarted += subscriber.ModuleInitializationStarted;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                throw new FaultException<CommonServicesFault>(new CommonServicesFault(), e.Message);
            }
        }

        public void SubscribeModuleStateEvents()
        {
            Initialize();

            try
            {
                _logger.DebugFormat("Remote subscriber subscribes ModuleState event on '{0}'.", this);
                var subscriber = _callbackChannelProvider.GetCallbackChannel<ICommonServicesEvents>();
                _platformModuleStateChanged += subscriber.PlatformModuleStateChanged;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                throw new FaultException<CommonServicesFault>(new CommonServicesFault(), e.Message);
            }

            foreach (var platformModule in _moduleRepository.Modules)
            {
                _eventRaiser.Raise(ref _platformModuleStateChanged, ((PlatformModule)platformModule).ToDTO());
            }
        }

        public void UnsubscribeEvents()
        {
            try
            {
                _logger.DebugFormat("Remote subscriber unsubscribes event on '{0}'.", this);
                var subscriber = _callbackChannelProvider.GetCallbackChannel<ICommonServicesEvents>();
                _platformModuleStateChanged -= subscriber.PlatformModuleStateChanged;
                _moduleInitializationStarted -= subscriber.ModuleInitializationStarted;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                throw new FaultException<CommonServicesFault>(new CommonServicesFault(), e.Message);
            }
        }

        public string LineControlVersion
        {
            get
            {
                try
                {
                    if (Assembly.GetEntryAssembly() != null)
                    {
                        return Versioning.GetVersion(Assembly.GetEntryAssembly());
                    }

                    return "<simulation>";
                }
                catch (Exception e)
                {
                    _logger.Error("Unable to read out LineControl SW Version", e);
                    throw new FaultException<CommonServicesFault>(new CommonServicesFault(), "Unable to read out LineControl SW Version");
                }
            }
        }

        public bool IsAliveAndInitialized
        {
            get
            {
                if (!_moduleRepository.Modules.Any())
                    return false;

                if (!_configurationService.IsInitialized)
                    return false;

                return _moduleBusManager.HasFinishedInitializationOfAllModules;
            }
        }

        public void StartAll()
        {
            _moduleRepository.Modules.ForEach(m => m.Start());
        }

        public void StopAll()
        {
            _moduleRepository.Modules.ForEach(m => m.Stop());
        }

        public ModuleGraphDTO GetModuleGraph()
        {
            try
            {
                return _moduleBusManager.GraphDto;
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                throw new FaultException<CommonServicesFault>(new CommonServicesFault(), e.Message);
            }
        }

        public PlatformModuleDTO[] RequestStates()
        {
            try
            {
                return (from pm in _moduleRepository.Modules.OfType<PlatformModule>() select pm.ToDTO()).ToArray();
            }
            catch (Exception e)
            {
                _logger.Error(e.Message, e);
                throw new FaultException<CommonServicesFault>(new CommonServicesFault(), e.Message);
            }
        }

        private void Initialize()
        {
            if (_isInitialized || _moduleRepository.Modules.IsNullOrEmpty())
            {
                return;
            }

            _logger.DebugFormat("Trying to initialize '{0}'.", this);

            foreach (var platformModule in _moduleRepository.Modules)
            {
                platformModule.CurrentItemCountChangedEvent += OnModuleCurrentItemCountChanged;
                platformModule.ModuleStateChangedEvent += PlatformModuleModuleStateChangedEvent;
            }

            _isInitialized = true;
        }

        private void OnModuleCurrentItemCountChanged(object sender, ItemCountChangedEventArgs countEventArgs)
        {
            _eventRaiser.Raise(ref _platformModuleStateChanged, ((PlatformModule)sender).ToDTO());
        }

        private void PlatformModuleModuleStateChangedEvent(IPlatformModule sender, PlatformModuleState newState)
        {
            _eventRaiser.Raise(ref _platformModuleStateChanged, ((PlatformModule)sender).ToDTO());
        }

        private void HandleModuleInitializationStarted(string moduleName)
        {
            _logger.DebugFormat("{0} notifies remote subscribers about start of initialization of module '{1}'.", this, moduleName);
            _eventRaiser.Raise(ref _moduleInitializationStarted, moduleName);
        }
    }
}
