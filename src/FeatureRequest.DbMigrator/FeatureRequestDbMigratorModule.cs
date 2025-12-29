using FeatureRequest.EntityFrameworkCore;
using Volo.Abp.Autofac;
using Volo.Abp.Modularity;

namespace FeatureRequest.DbMigrator;

[DependsOn(
    typeof(AbpAutofacModule),
    typeof(FeatureRequestEntityFrameworkCoreModule),
    typeof(FeatureRequestApplicationContractsModule)
    )]
public class FeatureRequestDbMigratorModule : AbpModule
{
}
