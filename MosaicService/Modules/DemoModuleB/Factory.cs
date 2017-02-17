using System.ComponentModel.Composition;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.DemoModuleB
{
    // 20 is the unique module id and matches the app.config module section
    [Export("20", typeof(IPlatformModuleFactory))]
    public class Factory : PlatformModuleFactoryBase
    {
        [Import]
        private ExportFactory<DemoModuleB> _exportFactory;

        protected override IPlatformModule CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}