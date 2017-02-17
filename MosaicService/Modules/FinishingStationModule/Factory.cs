
using System.ComponentModel.Composition;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.FinishingStationModule
{
    // 40 is the unique module id and matches the app.config module section
    [Export("40", typeof(IPlatformModuleFactory))]
    public class Factory : PlatformModuleFactoryBase
    {
        [Import]
        private ExportFactory<FinishingStationModule> _exportFactory;

        protected override IPlatformModule CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}

