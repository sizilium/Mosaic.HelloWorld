using System.ComponentModel.Composition;
using MosaicSample.UI.EntryStation.ViewModels;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.UI.EntryStation
{
    // 40 is the unique module id and matches the app.config module section
    [Export("50", typeof(IModuleFactory))]
    public class ModuleFactory : ModuleFactoryBase
    {
        [Import]
        private ExportFactory<EntryStationViewModel> _exportFactory;

        protected override IModuleScreen CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}