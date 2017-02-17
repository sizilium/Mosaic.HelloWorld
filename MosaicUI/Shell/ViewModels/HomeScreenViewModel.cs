using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Caliburn.Micro;
using MosaicSample.Shell.CommonServices;
using Cimpress.ACS.FP3.UIInfrastructure.AlarmService;
using Cimpress.ACS.FP3.UIInfrastructure.Properties;
using QuickGraph.Objects;
using VP.FF.PT.Common.GuiEssentials.Graph;
using VP.FF.PT.Common.GuiEssentials.Wcf;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.ConfigurationService;
using VP.FF.PT.Common.PlatformEssentials.Entities.DTOs;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow.DTOs;
using VP.FF.PT.Common.ShellBase.ViewModels;
using VP.FF.PT.Common.WpfInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model;
using VP.FF.PT.Common.WpfInfrastructure.Screens.Model.Graph;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;
using ModuleGraph = QuickGraph.Objects.ModuleGraph;
using MosaicSample.Shell.ConfigurationService;

namespace MosaicSample.Shell.ViewModels
{
    [Export]
    [CallbackBehavior]
    public sealed class HomeScreenViewModel : HomeScreenBaseViewModel, ICommonServicesCallback
    {
        private readonly IAlarmManagementFactory _alarmManagementFactory;
        public const string VertexPositionsConfig = "HomeScreenVertexPositions";

        private IDisposable _keepAliveCommonServices;
        private IDisposable _keepAliveConfigurationService;

        private CommonServicesClient _commonServicesClient;
        private ConfigurationServiceClient _configurationServiceClient;

        [Import]
        private IScreenNavigation _navigation = null;

        private AlarmSummaryViewModel _alarmSummaryViewModel;
        private readonly Settings _settings;
        private int _currentItemCount;
        private int _downTime;
        private bool _isPending;

        private ModuleGraph _moduleGraph;

        private double _throughput;
        private double _throughputMax;
        private double _throughputMin;
        private int _upTime;
        private bool _canMoveVertices;
        private bool _canForceEdges;
        private string _lineControlVersion;
        private string _plcVersion;

        [ImportingConstructor]
        public HomeScreenViewModel(ILogger logger, IEventAggregator eventAggregator, IAlarmManagementFactory alarmManagementFactory)
        {
            _alarmManagementFactory = alarmManagementFactory;
            DisplayName = "Home";
            CanMoveVertices = true;
            CanForceEdges = false;

            _logger = logger;
            _logger.Init(GetType());
            _logger.Info("CTOR of HomeScreenViewModel called");
            eventAggregator.Subscribe(this);

            ModuleGraph = new ModuleGraph();
        }

        public HomeScreenViewModel()
        {
            if (DesignTimeHelper.IsInDesignModeStatic)
            {
                // UseTestData();
            }
        }

        /// <summary>
        /// Activates this instance.
        /// </summary>
        protected async override void OnActivate()
        {
            base.OnActivate();

            if (AlarmSummaryViewModel != null)
                await AlarmSummaryViewModel.Activate();
        }

        /// <summary>
        /// Called when deactivating.
        /// </summary>
        /// <param name="close">Inidicates whether this instance will be closed.</param>
        protected async override void OnDeactivate(bool close)
        {
            if (AlarmSummaryViewModel != null)
                await AlarmSummaryViewModel.Deactivate();
        }

        public override async Task Initialize()
        {
            await EnsureValidConfigurationServiceWcfConnection();
            await EnsureValidCommonServicesWcfConnection();

            IList<ModuleVertexViewModelBase> nodePositions = new List<ModuleVertexViewModelBase>();
            
            string positionsXml = String.Empty;

            try
            {
                positionsXml = await _configurationServiceClient.GetValueAsync(VertexPositionsConfig);
            }
            catch (FaultException e)
            {
                _logger.Error("Could not load vertex positions from remote config", e);
            }

            if (!String.IsNullOrEmpty(positionsXml))
            {
                var serializer = new XmlSerializer(typeof(List<ModuleVertexViewModelBase>));
                var stringReaderPaper = new StringReader(positionsXml);
                nodePositions = (List<ModuleVertexViewModelBase>)serializer.Deserialize(stringReaderPaper);
            }

            var graphDto = await GetModuleGraphFromServer();
            ModuleGraph = GraphBuilder.BuildGraph(_logger, graphDto, nodePositions, true);
            
            _logger.Debug("Creating AlarmSummaryViewModel for HomeScreen");

            var modules = new Collection<string>();
            foreach (var vertice in ModuleGraph.Vertices)
                modules.Add(vertice.ID);

            AlarmSummaryViewModel = _alarmManagementFactory.CreateAlarmSummaryViewModel(modules);
            AlarmSummaryViewModel.CurrentAlarmsChanged += AlarmSummaryViewModelCurrentAlarmsChanged;
            await AlarmSummaryViewModel.Activate();
        }

        public async Task<ModuleGraphDTO> GetModuleGraphFromServer()
        {
            return await _commonServicesClient.GetModuleGraphAsync();
        }

        public async void GlobalStart()
        {
            await
                WcfCommunicationHelper.WcfCall<CommonServicesFault>(() => _commonServicesClient.StartAllAsync(),
                    EnsureValidCommonServicesWcfConnection, _logger);
        }

        public async void GlobalStop()
        {
            await
                WcfCommunicationHelper.WcfCall<CommonServicesFault>(() => _commonServicesClient.StopAllAsync(),
                    EnsureValidCommonServicesWcfConnection, _logger);
        }
        
        public AlarmSummaryViewModel AlarmSummaryViewModel
        {
            get { return _alarmSummaryViewModel; }

            set
            {
                if (_alarmSummaryViewModel != value)
                {
                    _alarmSummaryViewModel = value;
                    NotifyOfPropertyChange(() => AlarmSummaryViewModel);
                }
            }
        }

        public int CurrentItemCount
        {
            get { return _currentItemCount; }

            set
            {
                if (value != _currentItemCount)
                {
                    _currentItemCount = value;
                    NotifyOfPropertyChange(() => CurrentItemCount);
                }
            }
        }

        public int DownTime
        {
            get { return _downTime; }

            set
            {
                if (_downTime != value)
                {
                    _downTime = value;
                    NotifyOfPropertyChange(() => DownTime);
                }
            }
        }

        public bool HasGenericPlcView
        {
            get { return false; }
        }

        public bool IsPending
        {
            get { return _isPending; }

            set
            {
                if (_isPending != value)
                {
                    _isPending = value;
                    NotifyOfPropertyChange(() => IsPending);
                }
            }
        }

        public ModuleGraph ModuleGraph
        {
            get
            {
                return _moduleGraph;
            }

            set
            {
                if (value != _moduleGraph)
                {
                    _moduleGraph = value;
                    NotifyOfPropertyChange(() => ModuleGraph);
                }
            }
        }

        public double Throughput
        {
            get { return _throughput; }

            set
            {
                if (!_throughput.Equals(value))
                {
                    _throughput = value;
                    NotifyOfPropertyChange(() => Throughput);
                }
            }
        }

        public double ThroughputMax
        {
            get { return _throughputMax; }

            set
            {
                if (!_throughputMax.Equals(value))
                {
                    _throughputMax = value;
                    NotifyOfPropertyChange(() => ThroughputMax);
                }
            }
        }

        public double ThroughputMin
        {
            get { return _throughputMin; }

            set
            {
                if (!_throughputMin.Equals(value))
                {
                    _throughputMin = value;
                    NotifyOfPropertyChange(() => ThroughputMin);
                }
            }
        }

        public int UpTime
        {
            get { return _upTime; }

            set
            {
                if (_upTime != value)
                {
                    _upTime = value;
                    NotifyOfPropertyChange(() => UpTime);
                }
            }
        }

        public bool CanMoveVertices
        {
            get { return _canMoveVertices; }
            set
            {
                _canMoveVertices = value;
                NotifyOfPropertyChange(() => CanMoveVertices);
            }
        }

        public bool CanForceEdges
        {
            get { return _canForceEdges; }
            set
            {
                _canForceEdges = value;
                NotifyOfPropertyChange(() => CanForceEdges);
            }
        }

        public void ModuleInitializationStarted(string moduleName)
        {
        }

        public void ModuleManualModeChanged(bool controllerManualMode, string moduleName)
        {
            foreach (var vertice in ModuleGraph.Vertices.OfType<ModuleVertexViewModel>().Where(vertice => vertice.ID == moduleName))
            {
                vertice.IsManualControllerMode = controllerManualMode;
            }
        }

        public void SimulationModeChanged(string moduleName, bool controllerSimulationMode)
        {
            foreach (var vertice in ModuleGraph.Vertices.OfType<ModuleVertexViewModel>().Where(vertice => vertice.ID == moduleName))
            {
                vertice.IsSimulationControllerMode = controllerSimulationMode;
            }
        }

        public void RequestShutdown()
        {
            Environment.Exit(0);
        }

        public void OnNewVertexPositions()
        {
            List<ModuleVertexViewModelBase> positions = ModuleGraph.Vertices.Select(vertex => new ModuleVertexViewModelBase { ID = vertex.ID, Position = vertex.Position }).ToList();

            var serializer = new XmlSerializer(typeof(List<ModuleVertexViewModelBase>));
            var stringWriter = new StringWriter();
            serializer.Serialize(stringWriter, positions);

            try
            {
                _configurationServiceClient.SetValueAsync(VertexPositionsConfig, stringWriter.ToString());
            }
            catch (FaultException e)
            {
                _logger.Error("Could not save vertex positions to remote config", e);
            }
        }

        public void OnVertexSelected(object sender, VertexClickEventArgs vertex)
        {
            _navigation.NavigateToScreen(vertex.Vertex.ID);
        }

        public void OnEdgeSelected(object sender, EdgeClickEventArgs edge)
        {
        }

        public override async Task Shutdown()
        {
        }

        private string _platformState;

        public string PlatformState
        {
            get { return _platformState; }

            set
            {
                if (value != _platformState)
                {
                    _platformState = value;
                    NotifyOfPropertyChange(() => PlatformState);
                }
            }
        }

        private async Task EnsureValidConfigurationServiceWcfConnection()
        {
            await
                WcfCommunicationHelper.StartWcfClient<ConfigurationServiceClient, ConfigurationFault, ConfigurationService.IConfigurationService>(this, _configurationServiceClient,
                    c => new ConfigurationServiceClient(), async c =>
                    {
                        _configurationServiceClient = c;
                        try
                        {
                            if (_keepAliveConfigurationService != null)
                            {
                                _keepAliveConfigurationService.Dispose();
                            }
                            if (!await c.get_IsInitializedAsync())
                            {
                                IsEnabled = false;
                                return;
                            }
                            _keepAliveConfigurationService = WcfCommunicationHelper.KeepAlive<ConfigurationFault>(c.KeepAliveAsync, _logger);
                            IsEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error connecting to configuration service.", ex);
                            IsEnabled = false;
                        }
                    });
        }

        private async Task EnsureValidCommonServicesWcfConnection()
        {
            await
                WcfCommunicationHelper.StartWcfClient<CommonServicesClient, CommonServicesFault, ICommonServices>(this, _commonServicesClient, c => new CommonServicesClient(new InstanceContext(this)),
                    async c =>
                    {
                        _commonServicesClient = c;
                        try
                        {
                            if (_keepAliveCommonServices != null)
                            {
                                _keepAliveCommonServices.Dispose();
                            }
                            if (!await c.get_IsAliveAndInitializedAsync())
                            {
                                IsEnabled = false;
                                return;
                            }
                            await c.SubscribeEventsAsync();
                            await c.SubscribeModuleStateEventsAsync();
                            var result = await _commonServicesClient.RequestStatesAsync();
                            UpdateModuleStates(result);
                            _keepAliveCommonServices = WcfCommunicationHelper.KeepAlive<CommonServicesFault>(c.get_IsAliveAndInitializedAsync, _logger);
                            IsEnabled = true;
                        }
                        catch (Exception ex)
                        {
                            _logger.Error("Error connecting to common services.", ex);
                            IsEnabled = false;
                        }
                    });
        }

        private void AlarmSummaryViewModelCurrentAlarmsChanged(ObservableCollection<Alarm> alarms)
        {
            _commonServicesClient.RequestStatesAsync()
                                 .ContinueWith(t => UpdateModuleStates(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion)
                                 .ConfigureAwait(true);
        }

        private void UpdateModuleStates(IEnumerable<PlatformModuleDTO> moduleStates)
        {
            foreach (var moduleState in moduleStates)
            {
                PlatformModuleStateChanged(moduleState);
            }
        }

        public void PlatformModuleStateChanged(PlatformModuleDTO platformModule)
        {
            var module = (ModuleVertexViewModel)ModuleGraph.Vertices.FirstOrDefault(v => v.ID == platformModule.Name);

            GraphHelper.UpdateVertex(module, platformModule);
        }

        public void PlatformStateChanged(PlatformStateDTO platformState)
        {
        }

        public void ResetIgnoreDownstreamModules()
        {
        }
    }
}