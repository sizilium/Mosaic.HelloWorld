using System.ComponentModel.Composition;
using System.Linq;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.Simulation;
using VP.FF.PT.Common.Simulation.Alarms;
using VP.FF.PT.Common.Simulation.Equipment;
using VP.FF.PT.Common.Simulation.HardwareAbstraction;

namespace MosaicSample.DemoModuleA.Simulation
{
    public class SimulatedBehavior : ISimulatedBehavior
    {
        private readonly BarcodeReaderEquipment _readerEquipment;
        private readonly DemoModuleA _mosaicModule;

        [ImportingConstructor]
        public SimulatedBehavior(IPlatformModuleRepository moduleRepository, BarcodeReaderEquipment readerEquipment)
        {
            _readerEquipment = readerEquipment;
            _mosaicModule = moduleRepository.GetModulesByType<DemoModuleA>().First();
        }

        public void Initialize(IModuleSimulatorRepository moduleSimulatorRepository, SimulationAlarmHandler simulationAlarmHandler)
        {
            var moduleSimulator = moduleSimulatorRepository.GetModule("SimA");
            moduleSimulator.Initialize(10, "SimA");

            SimulatedBarcodeReader mosaicBarcodeReader = _mosaicModule.Equipments.OfType<SimulatedBarcodeReader>().FirstOrDefault();
            _readerEquipment.Initialize(1, mosaicBarcodeReader);
            moduleSimulator.AddEquipment(_readerEquipment);
            moduleSimulator.ReactOnPlatformModuleState(_mosaicModule);
        }
    }
}