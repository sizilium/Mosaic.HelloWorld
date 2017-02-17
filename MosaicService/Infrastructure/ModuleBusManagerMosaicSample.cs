using System.ComponentModel.Composition;
using System.Linq;
using VP.FF.PT.Common.Infrastructure;
using VP.FF.PT.Common.PlatformEssentials.Entities;
using VP.FF.PT.Common.PlatformEssentials.HardwareAbstraction;
using VP.FF.PT.Common.PlatformEssentials.ItemFlow;

namespace MosaicSample.Infrastructure
{
    [Export(typeof(IShutdown))]
    [Export(typeof(IModuleBusManager))]
    [PartCreationPolicy(CreationPolicy.Shared)]
    public class ModuleBusManagerMosaicSample : ModuleBusManager, IShutdown
    {
        public IStackLight StackLight { get; set; }

        public ModuleBusManagerMosaicSample()
        {
            StreamType = 1;
        }

        public override void Activate()
        {
            base.Activate();

            if (HasFinishedInitializationOfAllModules)
            {
                foreach (var module in ModuleRepository.Modules)
                {
                    module.AlarmManager.AlarmsChanged += EvaluateStacklight;
                    module.ModuleStateChangedEvent += (sender, state) => EvaluateStacklight();
                }

                if (ModuleRepository.Modules.Any(m => m.AlarmManager.HasErrors || m.State != PlatformModuleState.Run))
                    StackLight.IndicateError();
                else
                    StackLight.IndicateRun();
            }
        }

        private void EvaluateStacklight()
        {
            if (ModuleRepository.Modules.Any(m => m.AlarmManager.HasErrors || m.State != PlatformModuleState.Run))
                StackLight.IndicateError();
            else
                StackLight.IndicateRun();
        }

        public void Shutdown()
        {
            StackLight.IndicateError();
        }
    }
}