using System;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using System.Threading;
using MosaicSample.Infrastructure.Properties;
using MosaicSample.MosaicService;
using FluentAssertions;
using NUnit.Framework;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.Simulation;
using VP.FF.PT.Common.Simulation.Alarms;
using VP.FF.PT.Common.TestInfrastructure;

namespace MosaicSample.InMemoryIntegrationTests
{
    [TestFixture]
    [Category("IntegrationTests")]
    public class SmokeTests
    {
        private IntegrationTestHelper _testHelper;
        private static Bootstrapper _bootstrapper;

        [SetUp]
        public void SetUp()
        {
            _testHelper = new IntegrationTestHelper();
        }

        /// <summary>
        /// Try to start and stop Mosaic through the MefBootstrapper and do following validations:
        /// 1. startup must not crash
        /// 2. there should no Error or Warn log messages
        /// 3. all modules must be created and initialized
        /// 4. the simulation-api must be available
        /// 5. shutdown must not crash
        /// </summary>
        [Test]
        public void StartupAndShutdownMosaic()
        {
            _testHelper.Run(() =>
            {
                AppDomain currentDomain = AppDomain.CurrentDomain;
                currentDomain.UnhandledException += UnhandledExceptionHandler;
                //Thread.CurrentThread.Name = "Main";

                _bootstrapper = new Bootstrapper();
                _bootstrapper.AdditionalCatalogs.Catalogs.Add(new AssemblyCatalog(typeof(Bootstrapper).Assembly));
                var runAction = new Action(_bootstrapper.Run);
                runAction.ShouldNotThrow();

                var test = new IntegrationTestHelper(_bootstrapper.Container) {LoggingHelper = new LoggingHelper()};

                test.VerifyNoLogErrors();
                VerifyAllModulesInitialized(test);
                VerifySimulationApi(_bootstrapper.Container);
                var stopAction = new Action(_bootstrapper.Stop);

                stopAction.ShouldNotThrow();
            }, TimeSpan.FromSeconds(100));
        }

        private void VerifyAllModulesInitialized(IntegrationTestHelper test)
        {
            VerifyModuleInitialized<EntryStation.EntryStation>(test);
            VerifyModuleInitialized<DemoModuleA.DemoModuleA>(test);
            VerifyModuleInitialized<DemoModuleB.DemoModuleB>(test);
            VerifyModuleInitialized<DemoModuleC.DemoModuleC>(test, 2);
            VerifyModuleInitialized<FinishingStationModule.FinishingStationModule>(test);
        }

        void VerifyModuleInitialized<TModule>(IntegrationTestHelper test, int numberOfModules = 1) where TModule : IPlatformModule
        {
            var moduleRepository = test.Container.GetExportedValue<IPlatformModuleRepository>();
            var entryStation = moduleRepository.GetModulesByType<TModule>();
            entryStation.Should().NotBeNullOrEmpty();
            entryStation.Should().HaveCount(numberOfModules);
            entryStation.First().IsInitialized.Should().BeTrue();
        }

        private void VerifySimulationApi(CompositionContainer container)
        {
            container.GetExportedValue<ITaktManager>().Should().NotBeNull("simulation-api must be available in integration tests");
            container.GetExportedValue<IModuleSimulatorRepository>().Should().NotBeNull("simulation-api must be available in integration tests");
            container.GetExportedValue<SimulationAlarmHandler>().Should().NotBeNull("simulation-api must be available in integration tests");
            container.GetExportedValues<ITaktPartsRepository>().Should().NotBeNullOrEmpty("simulation-api must be available in integration tests");

            container.GetExportedValue<IModuleSimulatorRepository>().Modules.Should().NotBeEmpty("there must be simulated behavior");
        }

        private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            _bootstrapper.LogApplicationCrash((Exception)unhandledExceptionEventArgs.ExceptionObject);
        }
    }
}
