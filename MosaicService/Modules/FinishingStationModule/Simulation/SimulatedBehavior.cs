using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.Simulation;
using VP.FF.PT.Common.Simulation.Alarms;
using VP.FF.PT.Common.Simulation.Equipment;
using VP.FF.PT.Common.Simulation.HardwareAbstraction;

namespace MosaicSample.FinishingStationModule.Simulation
{
    public class SimulatedBehavior : ISimulatedBehavior
    {
        private readonly CompositionContainer _container;
        private readonly FinishingStationModule _mosaicModule;
        
        [ImportingConstructor]
        public SimulatedBehavior(IPlatformModuleRepository moduleRepository, CompositionContainer container)
        {
            _container = container;
            _mosaicModule = moduleRepository.GetModulesByType<FinishingStationModule>().First();
        }

        public void Initialize(IModuleSimulatorRepository moduleSimulatorRepository, SimulationAlarmHandler simulationAlarmHandler)
        {
            var moduleSimulator = moduleSimulatorRepository.GetModule("FIN");
            moduleSimulator.Initialize(10, "FIN");

            SimulatedBarcodeReader mosaicBarcodeReader2 = _mosaicModule.Equipments.OfType<SimulatedBarcodeReader>().FirstOrDefault();
            var readerEquipment2 = _container.GetExportedValue<BarcodeReaderEquipment>();
            readerEquipment2.Initialize(1, mosaicBarcodeReader2);
            moduleSimulator.AddEquipment(readerEquipment2);
            moduleSimulator.ReactOnPlatformModuleState(_mosaicModule);
        }
    }
}