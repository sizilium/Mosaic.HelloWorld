using MosaicSample.Shell.CommonServices;
using System;
using System.ComponentModel.Composition;
using System.ServiceModel;
using System.Threading.Tasks;
using VP.FF.PT.Common.GuiEssentials.Wcf;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.Entities.DTOs;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow.DTOs;

namespace MosaicSample.Shell.ViewModels
{
    [Export(typeof(IProvideInitializationState))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CommonServicesAdapter : IProvideInitializationState, ICommonServicesCallback
    {
        private readonly IClient<ICommonServices> _client;
        private readonly ILogger _logger;

        [ImportingConstructor]
        public CommonServicesAdapter(
            IDuplexClientFactory<ICommonServices, ICommonServicesCallback> serviceFactory,
            ILogger logger)
        {
            _client = serviceFactory.CreateClient(this);
            _logger = logger;
            ModuleInInitializationStarted += n => { };
            _logger.Init(GetType());
        }

        private event Action<string> ModuleInInitializationStarted;

        public async Task SubscribeForInitializationEvents(Action<string> handleModuleInInitialization)
        {
            ModuleInInitializationStarted += handleModuleInInitialization;

            // wait forever until the server becomes available; any errors related to the WCF communication
            // should not propagate to the UI and display as error message there. Instead, we just retry
            // until it eventually completes.
            while (true)
            {
                try
                {
                    await _client.InvokeOnService(c => c.SubscribeEventsAsync());
                    return;
                }
                catch (CommunicationException ex)
                {
                    _logger.Error("Subscription impossible.", ex);
                }
                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        public async Task<bool> AreAllModulesInitialized()
        {
            _logger.Debug(string.Format("Checking the common service if all modules were initialized."));
            bool result = await _client.InvokeOnService(c => c.get_IsAliveAndInitializedAsync());
            _logger.Debug(string.Format("Check if all modules were initialized on common service returned '{0}'.", result));
            if (result)
                return true;
            return await RequestInitializationStateOfAllModulesDelayed();
        }

        private async Task<bool> RequestInitializationStateOfAllModulesDelayed()
        {
            await Task.Delay(2500);
            return await AreAllModulesInitialized();
        }

        public async Task<ModuleGraphDTO> GetModuleGraph()
        {
            return await _client.InvokeOnService(c => c.GetModuleGraphAsync());
        }

        public Task UnsubscribeFromInitializationEvents(Action<string> handleModuleInInitialization)
        {
            ModuleInInitializationStarted -= handleModuleInInitialization;
            return Task.FromResult(0);
        }

        public void UnsubscribeEvents()
        {
            _client.InvokeOnService(c => c.UnsubscribeEventsAsync());
        }

        public void HtsSafetyStateChanged(string state)
        {
        }

        public void PlatformStateChanged(PlatformStateDTO platformState)
        {
        }

        public void PptSafetyStateChanged(string state)
        {
        }

        public void PptPlcVersionChanged(string version)
        {
        }

        public void PlatformModuleStateChanged(PlatformModuleDTO platformModule)
        {
        }

        public void ResetIgnoreDownstreamModules()
        {
        }

        public void AllModulesInitialized()
        {
        }

        public void ModuleInitializationStarted(string moduleName)
        {
            _logger.DebugFormat("Received notification from common service that initialization of module '{0}' started.", moduleName);
            ModuleInInitializationStarted(moduleName);
        }

        public void ModuleManualModeChanged(bool controllerManualMode, string moduleName)
        {
        }

        public void SimulationModeChanged(string moduleName, bool controllerSimulationMode)
        {
        }

        public void RequestShutdown()
        {
        }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return string.Format("{0}#{1}", GetType().Name, GetHashCode());
        }
    }
}