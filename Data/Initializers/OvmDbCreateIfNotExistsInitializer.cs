using System.Data.Entity;

namespace Data.Initializers
{
    public class OvmDbCreateIfNotExistsInitializer : CreateDatabaseIfNotExists<OvmDbContext>
    {
        protected override void Seed(OvmDbContext context)
        {
            Seeder.Seed(context);

            base.Seed(context);
        }
    }
}
