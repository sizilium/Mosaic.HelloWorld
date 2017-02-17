using System;
using System.Linq;
using MosaicSample.Infrastructure;
using MosaicSample.Infrastructure.Properties;
using VP.FF.PT.Common.Infrastructure.Bootstrapper;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction.ItemTracking;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow;
using VP.FF.PT.Common.Simulation;
using Common.WebUiService;

namespace MosaicSample.MosaicService
{
    public class Bootstrapper : MefBootstrapper
    {
        private IWebUiService _webUiService;

        // TODO: find a way to move this code into the project (unfortunately the code get's not called before the bootstrapping then)
        static Bootstrapper()
        {
            if (Settings.Default.IsSimulation)
            {
                GlobalRegistrationBuilder.Builder.ForType<EntryStation.EntryStation>()
                    .ImportProperty(p => p.Junction, builder => builder.AsContractName("simulation"));

                GlobalRegistrationBuilder.Builder.ForType<DemoModuleA.DemoModuleA>()
                    .ImportProperty(p => p.BarcodeReader, builder => builder.AsContractName("simulation"));

                GlobalRegistrationBuilder.Builder.ForType<DemoModuleB.DemoModuleB>()
                    .ImportProperty(p => p.Junction, builder => builder.AsContractName("simulation"));
                GlobalRegistrationBuilder.Builder.ForType<DemoModuleB.DemoModuleB>()
                    .ImportProperty(p => p.BarcodeReader, builder => builder.AsContractName("simulation"));

                GlobalRegistrationBuilder.Builder.ForType<DemoModuleC.DemoModuleC>()
                    .ImportProperty(p => p.Junction, builder => builder.AsContractName("simulation"));
                GlobalRegistrationBuilder.Builder.ForType<DemoModuleC.DemoModuleC>()
                    .ImportProperty(p => p.BarcodeReader, builder => builder.AsContractName("simulation"));

                GlobalRegistrationBuilder.Builder.ForType<FinishingStationModule.FinishingStationModule>()
                    .ImportProperty(p => p.BarcodeReader, builder => builder.AsContractName("simulation"));
            }
            else
            {
                GlobalRegistrationBuilder.Builder.ForType<EntryStation.EntryStation>()
                    .ImportProperty(p => p.Junction);

                GlobalRegistrationBuilder.Builder.ForType<DemoModuleA.DemoModuleA>()
                    .ImportProperty(p => p.BarcodeReader);

                GlobalRegistrationBuilder.Builder.ForType<DemoModuleB.DemoModuleB>()
                    .ImportProperty(p => p.Junction);
                GlobalRegistrationBuilder.Builder.ForType<DemoModuleB.DemoModuleB>()
                    .ImportProperty(p => p.BarcodeReader);

                GlobalRegistrationBuilder.Builder.ForType<DemoModuleC.DemoModuleC>()
                    .ImportProperty(p => p.Junction);
                GlobalRegistrationBuilder.Builder.ForType<DemoModuleC.DemoModuleC>()
                    .ImportProperty(p => p.BarcodeReader);

                GlobalRegistrationBuilder.Builder.ForType<FinishingStationModule.FinishingStationModule>()
                    .ImportProperty(p => p.BarcodeReader);
            }

            if (Settings.Default.IsSimulatedStacklight)
            {
                GlobalRegistrationBuilder.Builder.ForType<ModuleBusManagerMosaicSample>()
                    .ImportProperty(p => p.StackLight, builder => builder.AsContractName("simulation"));
            }
            else
            {
                GlobalRegistrationBuilder.Builder.ForType<ModuleBusManagerMosaicSample>()
                    .ImportProperty(p => p.StackLight);
            }

            GlobalRegistrationBuilder.Builder
                .ForTypesMatching(t => t.GetProperties().Any(p => p.PropertyType.IsEquivalentTo(typeof(IBarcodeReader))))
                .ImportProperties(p => p.PropertyType.IsEquivalentTo(typeof(IBarcodeReader)));
        }

        private readonly ServiceHostLauncher _serviceHostLauncher = new ServiceHostLauncher();

        public override void Stop()
        {
            Logger.Info("Stopping WCF Services");

            if (_serviceHostLauncher != null)
            {
                _serviceHostLauncher.Stop(Logger);
            }

            base.Stop();
        }

        /// <summary>
        /// Will be called as soon as the MEF container construction is completed.
        /// </summary>
        protected override void OnInitialized()
        {
            try
            {
                // ToDo: Replace hardcoded webUiPort with app.config solution
                int webUiPort = 5005;

                _webUiService = Container.GetExportedValue<IWebUiService>();

                _webUiService?.Start(webUiPort, false);
                Logger?.Info(string.Format("webUiService started on port - {0}", webUiPort));
            }
            catch (Exception e)
            {
                Logger?.Info(string.Format("No WebUiService attached - {0}", e.Message));
            }

            var moduleBusManagers = Container.GetExportedValues<IModuleBusManager>();
            foreach (var moduleBusManager in moduleBusManagers)
            {
                moduleBusManager.Construct();
                moduleBusManager.Initialize();
                moduleBusManager.Activate();
            }

#if DEBUG
            _serviceHostLauncher.Run(Logger, Container, true);
#else
                _serviceHostLauncher.Run(Logger, Container);
#endif

            var simulationInitializer = Container.GetExportedValue<ISimulationInitializer>();
            simulationInitializer.Initialize();
        }
    }
}