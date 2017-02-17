using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.PlatformEssentials;
using VP.FF.PT.Common.Simulation;
using VP.FF.PT.Common.Simulation.Alarms;
using VP.FF.PT.Common.Simulation.Equipment;
using VP.FF.PT.Common.Simulation.HardwareAbstraction;

namespace MosaicSample.EntryStation.Simulation
{
    public class SimulatedBehavior : ISimulatedBehavior
    {
        private readonly CompositionContainer _container;
        private readonly EntryStation _mosaicModule;

        [ImportingConstructor]
        public SimulatedBehavior(IPlatformModuleRepository moduleRepository, CompositionContainer container)
        {
            _mosaicModule = moduleRepository.GetModuleByType<EntryStation>(1);
            _container = container;
        }

        public void Initialize(IModuleSimulatorRepository moduleSimulatorRepository, SimulationAlarmHandler simulationAlarmHandler)
        {
            if (_mosaicModule == null)
                return;

            var moduleSimulator = moduleSimulatorRepository.GetModule("BRC");
            moduleSimulator.Initialize(10, _mosaicModule.Name);

            SimulatedJunction mosaicControlledJunction = _mosaicModule.Equipments.OfType<SimulatedJunction>().FirstOrDefault();
            var junctionEquipment = _container.GetExportedValue<JunctionEquipment>();
            junctionEquipment.Initialize(9, moduleSimulator, mosaicControlledJunction,
                moduleSimulatorRepository.GetModule("SimA"),
                moduleSimulatorRepository.GetModule("SimB"));
            moduleSimulator.AddEquipment(junctionEquipment);
            moduleSimulator.ReactOnPlatformModuleState(_mosaicModule);

            _mosaicModule.BarcodeEventHandler += (sender, args) =>
            {
                var simItem = new SimulatedItem();

                simItem.ItemId = (ulong) HashHelper.ConvertStringToLong(args.Barcode);
                simItem.Metadata.Add("barcode", args.Barcode);

                moduleSimulator.AddItem(simItem);
            };
        }
    }
}