using System.ComponentModel.Composition;
using MosaicSample.UI.DemoModuleB.ViewModels;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.UI.DemoModuleB
{
    // 20 is the unique module id and matches the app.config module section
    [Export("20", typeof(IModuleFactory))]
    public class ModuleFactory : ModuleFactoryBase
    {
        [Import]
        private ExportFactory<DemoModuleBViewModel> _exportFactory;

        protected override IModuleScreen CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}