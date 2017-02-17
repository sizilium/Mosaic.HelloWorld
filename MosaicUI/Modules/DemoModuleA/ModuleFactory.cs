using System.ComponentModel.Composition;
using MosaicSample.UI.DemoModuleA.ViewModels;
using VP.FF.PT.Common.GuiEssentials;
using VP.FF.PT.Common.WpfInfrastructure.CaliburnIntegration;

namespace MosaicSample.UI.DemoModuleA
{
    // 10 is the unique module id and matches the app.config module section
    [Export("10", typeof(IModuleFactory))]
    public class ModuleFactory : ModuleFactoryBase
    {
        [Import]
        private ExportFactory<DemoModuleAViewModel> _exportFactory;

        protected override IModuleScreen CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}