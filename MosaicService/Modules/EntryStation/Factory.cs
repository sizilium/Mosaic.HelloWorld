using System.ComponentModel.Composition;
using VP.FF.PT.Common.PlatformEssentials;

namespace MosaicSample.EntryStation
{
    // 50 is the unique module id and matches the app.config module section
    [Export("50", typeof(IPlatformModuleFactory))]
    public class Factory : PlatformModuleFactoryBase
    {
        [Import]
        private ExportFactory<EntryStation> _exportFactory;

        protected override IPlatformModule CreateModuleInstance()
        {
            return _exportFactory.CreateExport().Value;
        }
    }
}

