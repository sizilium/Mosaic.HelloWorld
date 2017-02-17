using System.Data.Entity;
using VP.FF.PT.Common.Infrastructure.Database;
using VP.FF.PT.Common.Infrastructure.Logging;
using VP.FF.PT.Common.PlatformEssentials.Entities;

namespace MosaicSample.Infrastructure
{
    public class MosaicSampleEntityContext : CommonEntityContext
    {
        public MosaicSampleEntityContext()
            : base("name=MosaicSampleEntityDB", new ConsoleOutLogger())
        {
            Database.SetInitializer(new CreateOrMigrateDatabaseInitializer<MosaicSampleEntityContext, Configuration>());
        }

        public MosaicSampleEntityContext(ILogger logger)
            : base("name=MosaicSampleEntityDB", logger)
        {
           //Database.SetInitializer(new Q100EntityContextDatabaseInitializer());
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}