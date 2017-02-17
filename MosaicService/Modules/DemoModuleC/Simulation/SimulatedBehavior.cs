using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.Simulation;
using VP.FF.PT.Common.Simulation.Alarms;
using VP.FF.PT.Common.Simulation.Equipment;
using VP.FF.PT.Common.Simulation.HardwareAbstraction;

namespace MosaicSample.DemoModuleC.Simulation
{
    public class SimulatedBehavior : ISimulatedBehavior
    {
        private readonly CompositionContainer _container;
        private readonly DemoModuleC _mosaicModuleC1;
        private readonly DemoModuleC _mosaicModuleC2;

        [ImportingConstructor]
        public SimulatedBehavior(IPlatformModuleRepository moduleRepository, CompositionContainer container)
        {
            _container = container;
            _mosaicModuleC1 = moduleRepository.GetModuleByType<DemoModuleC>(1);
            _mosaicModuleC2 = moduleRepository.GetModuleByType<DemoModuleC>(2);
        }

        public void Initialize(IModuleSimulatorRepository moduleSimulatorRepository, SimulationAlarmHandler simulationAlarmHandler)
        {
            var moduleSimulatorC1 = moduleSimulatorRepository.GetModule("SimC1");
            moduleSimulatorC1.Initialize(10, "SimC1");

            SimulatedBarcodeReader mosaicBarcodeReader = _mosaicModuleC1.Equipments.OfType<SimulatedBarcodeReader>().FirstOrDefault();
            var readerEquipment1 = _container.GetExportedValue<BarcodeReaderEquipment>();
            readerEquipment1.Initialize(1, mosaicBarcodeReader);
            moduleSimulatorC1.AddEquipment(readerEquipment1);

            SimulatedJunction mosaicControlledJunction = _mosaicModuleC1.Equipments.OfType<SimulatedJunction>().FirstOrDefault();
            var junctionEquipment1 = _container.GetExportedValue<JunctionEquipment>();
            junctionEquipment1.Initialize(9, moduleSimulatorC1, mosaicControlledJunction,
                moduleSimulatorRepository.GetModule("SimB"),
                moduleSimulatorRepository.GetModule("FIN"));
            moduleSimulatorC1.AddEquipment(junctionEquipment1);
            moduleSimulatorC1.ReactOnPlatformModuleState(_mosaicModuleC1);

            var moduleSimulatorC2 = moduleSimulatorRepository.GetModule("SimC2");
            moduleSimulatorC2.Initialize(10, "SimC2");

            SimulatedBarcodeReader mosaicBarcodeReader2 = _mosaicModuleC2.Equipments.OfType<SimulatedBarcodeReader>().FirstOrDefault();
            var readerEquipment2 = _container.GetExportedValue<BarcodeReaderEquipment>();
            readerEquipment2.Initialize(1, mosaicBarcodeReader2);
            moduleSimulatorC2.AddEquipment(readerEquipment2);
            moduleSimulatorC2.ReactOnPlatformModuleState(_mosaicModuleC2);
        }
    }
}