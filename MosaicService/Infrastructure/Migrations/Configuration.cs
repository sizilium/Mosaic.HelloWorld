
namespace MosaicSample.Infrastructure
{
    using System.Data.Entity.Migrations;

    public class Configuration : DbMigrationsConfiguration<MosaicSampleEntityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(MosaicSampleEntityContext context)
        {
            //  This method will be called after migrating to the latest version.
        }
    }
}
