using System.ComponentModel.Composition;
using System.Configuration;
using MosaicSample.Infrastructure.Properties;
using VP.FF.PT.Common.PlatformEssentials.ConfigurationService;

namespace MosaicSample.Infrastructure
{
    [Export(typeof(IApplicationSettingsBaseFactory))]
    public class ApplicationSettingsFactory : IApplicationSettingsBaseFactory
    {
        public ApplicationSettingsBase CreateDefault()
        {
            return Settings.Default;
        }
    }
}