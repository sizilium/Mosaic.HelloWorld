using System.ComponentModel.Composition;
using MosaicSample.UI.FinishingStationModule.ViewModels;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.UI.FinishingStationModule
{
    // 40 is the unique module id and matches the app.config module section
    [Export("40", typeof(IModuleFactory))]
    public class ModuleFactory : ModuleFactoryBase
    {
        [Import]
        private ExportFactory<FinishingStationModuleViewModel> _exportFactory;

        protected override IModuleScreen CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}