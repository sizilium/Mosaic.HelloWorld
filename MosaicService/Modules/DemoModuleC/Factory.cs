using System.ComponentModel.Composition;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleC
{
    // 30 is the unique module id and matches the app.config module section
    [Export("30", typeof(IPlatformModuleFactory))]
    public class Factory : PlatformModuleFactoryBase
    {
        [Import]
        private ExportFactory<DemoModuleC> _exportFactory;

        protected override IPlatformModule CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}