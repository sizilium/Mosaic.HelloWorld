using System.ComponentModel.Composition;
using MosaicSample.UI.DemoModuleC.ViewModels;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.UI.DemoModuleC
{
    // 30 is the unique module id and matches the app.config module section
    [Export("30", typeof(IModuleFactory))]
    public class ModuleFactory : ModuleFactoryBase
    {
        [Import]
        private ExportFactory<DemoModuleCViewModel> _exportFactory;

        protected override IModuleScreen CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}