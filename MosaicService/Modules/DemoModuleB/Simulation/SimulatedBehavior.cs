using System.ComponentModel.Composition;
using System.Linq;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.Simulation;
using VP.FF.PT.Common.Simulation.Alarms;
using VP.FF.PT.Common.Simulation.Equipment;
using VP.FF.PT.Common.Simulation.HardwareAbstraction;

namespace MosaicSample.DemoModuleB.Simulation
{
    public class SimulatedBehavior : ISimulatedBehavior
    {
        private readonly BarcodeReaderEquipment _readerEquipment;
        private readonly JunctionEquipment _junctionEquipment;
        private readonly DemoModuleB _mosaicModule;

        [ImportingConstructor]
        public SimulatedBehavior(IPlatformModuleRepository moduleRepository, BarcodeReaderEquipment readerEquipment, JunctionEquipment junctionEquipment)
        {
            _readerEquipment = readerEquipment;
            _junctionEquipment = junctionEquipment;
            _mosaicModule = moduleRepository.GetModulesByType<DemoModuleB>().First();
        }

        public void Initialize(IModuleSimulatorRepository moduleSimulatorRepository, SimulationAlarmHandler simulationAlarmHandler)
        {
            var moduleSimulator = moduleSimulatorRepository.GetModule("SimB");
            moduleSimulator.Initialize(10, "SimB");

            SimulatedBarcodeReader mosaicBarcodeReader = _mosaicModule.Equipments.OfType<SimulatedBarcodeReader>().FirstOrDefault();
            _readerEquipment.Initialize(1, mosaicBarcodeReader);
            moduleSimulator.AddEquipment(_readerEquipment);

            SimulatedJunction mosaicControlledJunction = _mosaicModule.Equipments.OfType<SimulatedJunction>().First();
            _junctionEquipment.Initialize(9, moduleSimulator, mosaicControlledJunction, 
                moduleSimulatorRepository.GetModule("SimC1"),
                moduleSimulatorRepository.GetModule("SimC2"));
            moduleSimulator.AddEquipment(_junctionEquipment);
            moduleSimulator.ReactOnPlatformModuleState(_mosaicModule);
        }
    }
}