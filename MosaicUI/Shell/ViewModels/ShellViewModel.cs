using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Reflection;
using System.Text;
using Cimpress.ACS.FP3.UIInfrastructure.Properties;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow.DTOs;
using VP.FF.PT.Common.ShellBase.ViewModels;
using VP.FF.PT.Common.WpfInfrastructure;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;
using VP.FF.PT.Common.WpfInfrastructure.ScreenActivation;
using VP.FF.PT.Common.WpfInfrastructure.Screens.ViewModels;
using EventAggregator = Caliburn.Micro.EventAggregator;
using IEventAggregator = Caliburn.Micro.IEventAggregator;

namespace MosaicSample.Shell.ViewModels
{
    [Export(typeof (IShell))]
    [Export(typeof (IScreenNavigation))]
    public class ShellViewModel : ShellBaseViewModel, IScreenNavigation 
    {
        private readonly IProvideInitializationState _provideInitializationState;
        private const string DefaultTitle = "Mosaic Hello World";
        private HomeScreenViewModel _homeScreen;

        // the ctor is needed for design time preview only
        public ShellViewModel()
            :base(null, new NullLogger(), new EventAggregator())
        {
            if (DesignTimeHelper.IsInDesignModeStatic)
            {
                UseTestData();
            }
        }

        [ImportingConstructor]
        public ShellViewModel(
            IProvideStatesForScreenActivation states,
            IProvideInitializationState provideInitializationState,
            IEventAggregator eventAggregator,
            ILogger logger)
            :base(states, logger, eventAggregator)
        {
            _provideInitializationState = provideInitializationState;
            AppDomain currentDomain = AppDomain.CurrentDomain;

            CacheViewsByDefault = true;
            currentDomain.UnhandledException += UnhandledExceptionHandler;

            _states = states;
            
            _logger = logger;
            _logger.Init(GetType());
            _logger.Info("starting Shell now...");
            _eventAggregator = eventAggregator;

            Title = DefaultTitle + " (UI-SW-Ver. " + MosaicVersion + ")";
        }

        public new void NavigateToScreen(string moduleKey)
        {
            try
            {
                CurrentScreen = ModuleRepository.GetModule(moduleKey);
                AdminConsoleViewModel.CurrentModuleKey = moduleKey;
                _eventAggregator.Publish(AdminConsoleViewModel);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormat("Can't navigate to screen {0}", moduleKey, ex);
            }
        }

        public HomeScreenViewModel HomeScreenViewModel
        {
            get { return _homeScreen; }
            set
            {
                _homeScreen = value;
                NotifyOfPropertyChange(() => HomeScreenViewModel);
            }
        }

        public override double ScalingFactor
        {
            get { return Settings.Default.ScalingFactor; }
        }

        public override void NavigateToHome()
        {
            NavigateToScreen(HomeScreenViewModel);
        }

        private void UseTestData()
        {
            HeadBarViewModel.BreadcrumbBarViewModel = new BreadcrumbBarViewModel();
        }

        public override async void TryInitializeModules()
        {
            _logger.Debug(string.Format("{0} tries to initialize the view models", this));
            _states.ChangeToLoadingState();
            try
            {
                Action<string> updateInitializationMessage = m => ModuleInInitialization = m;
                await _provideInitializationState.SubscribeForInitializationEvents(updateInitializationMessage);
                bool modulesInitialized = await _provideInitializationState.AreAllModulesInitialized();
                if (modulesInitialized)
                {
                    InitializeScreens();
                    await _provideInitializationState.UnsubscribeFromInitializationEvents(updateInitializationMessage);
                    _states.ChangeToContentState();
                    LoginCommand.CanExecute(null);
                    _logger.Debug(string.Format("{0} successfully initialized the modules.", this));
                }
            }
            catch (Exception exception)
            {
                string errorMessage = new StringBuilder()
                    .AppendLine("An error occured while intializing the Remote UI:")
                    .Append(exception).ToString();
                _logger.ErrorFormat("{0} failed to initalize the modules: '{1}'", this, errorMessage);
                _states.ChangeToErrorState(errorMessage);
            }
        }

        protected override async void InitializeScreens()
        {
            ModuleGraphDTO moduleGraphDto = await _provideInitializationState.GetModuleGraph();

            var initializer = new GuiModuleInitializer(_logger, ModuleRepository, new List<IConfigurationScreen>(), _container);
            initializer.CreateModules(moduleGraphDto);
            initializer.InitializeModules();

            HomeScreenViewModel = ModuleRepository.GetModuleByType<HomeScreenViewModel>(0);

            _logger.Info("all screens initialized");

            InitializeBreadcrumbBar();
        }

        private void InitializeBreadcrumbBar()
        {
            HeadBarViewModel.BreadcrumbBarViewModel = new BreadcrumbBarViewModel(HomeScreenViewModel);
            HeadBarViewModel.HomeScreenViewModel = HomeScreenViewModel;
            NavigateToHome();
        }

        private string MosaicVersion
        {
            get
            {
                return Versioning.GetVersion(Assembly.GetExecutingAssembly());
            }
        }
    }
}