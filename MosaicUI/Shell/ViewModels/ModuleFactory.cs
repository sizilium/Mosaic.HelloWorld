using System.ComponentModel.Composition;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.Shell.ViewModels
{
    // 0 is the unique module id and matches the app.config module section
    [Export("0", typeof(IModuleFactory))]
    public class ModuleFactory : ModuleFactoryBase
    {
        [Import]
        private ExportFactory<HomeScreenViewModel> _exportFactory;

        protected override IModuleScreen CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}