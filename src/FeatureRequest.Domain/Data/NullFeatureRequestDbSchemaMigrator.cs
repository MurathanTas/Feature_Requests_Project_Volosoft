using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;

namespace FeatureRequest.Data;

/* This is used if database provider does't define
 * IFeatureRequestDbSchemaMigrator implementation.
 */
public class NullFeatureRequestDbSchemaMigrator : IFeatureRequestDbSchemaMigrator, ITransientDependency
{
    public Task MigrateAsync()
    {
        return Task.CompletedTask;
    }
}
