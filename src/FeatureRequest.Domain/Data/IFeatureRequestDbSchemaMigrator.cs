using System.Threading.Tasks;

namespace FeatureRequest.Data;

public interface IFeatureRequestDbSchemaMigrator
{
    Task MigrateAsync();
}
