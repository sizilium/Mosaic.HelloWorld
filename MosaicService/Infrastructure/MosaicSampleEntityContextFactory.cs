using System;
using System.ComponentModel.Composition;
using MosaicSample.Infrastructure.Properties;
using VP.FF.PT.Common.Infrastructure.Database;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.Entities;

namespace MosaicSample.Infrastructure
{
    [Export(typeof(IEntityContextFactory))]
    [Export(typeof(IConnectionStringProvider))]
    public class MosaicSampleEntityContextFactory : IEntityContextFactory, IConnectionStringProvider
    {
        private readonly ILogger _logger;
        private readonly Settings _settings;

        [ImportingConstructor]
        public MosaicSampleEntityContextFactory(ILogger logger, Settings settings)
        {
            _logger = logger;
            _logger.Init(GetType());
            _settings = settings;
        }

        /// <summary>
        /// Creates the context.
        /// </summary>
        public IEntityContext CreateContext()
        {
            if (_settings.EnablePersistency)
                return new MosaicSampleEntityContext(_logger);

            return new NoDbEntityContext();
        }

        public string ConnectionString
        {
            get
            {
                if (_settings.EnablePersistency)
                    return ((CommonEntityContext)CreateContext()).Database.Connection.ConnectionString;

                return String.Empty;
            }
        }
    }
}