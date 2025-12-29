using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using FeatureRequest.Data;
using Volo.Abp.DependencyInjection;

namespace FeatureRequest.EntityFrameworkCore;

public class EntityFrameworkCoreFeatureRequestDbSchemaMigrator
    : IFeatureRequestDbSchemaMigrator, ITransientDependency
{
    private readonly IServiceProvider _serviceProvider;

    public EntityFrameworkCoreFeatureRequestDbSchemaMigrator(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task MigrateAsync()
    {
        /* We intentionally resolve the FeatureRequestDbContext
         * from IServiceProvider (instead of directly injecting it)
         * to properly get the connection string of the current tenant in the
         * current scope.
         */

        await _serviceProvider
            .GetRequiredService<FeatureRequestDbContext>()
            .Database
            .MigrateAsync();
    }
}
