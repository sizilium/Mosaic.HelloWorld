using System.ComponentModel.Composition;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleA
{
    // 10 is the unique module id and matches the app.config module section
    [Export("10", typeof(IPlatformModuleFactory))]
    public class Factory : PlatformModuleFactoryBase
    {
        [Import]
        private ExportFactory<DemoModuleA> _exportFactory;

        protected override IPlatformModule CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}
