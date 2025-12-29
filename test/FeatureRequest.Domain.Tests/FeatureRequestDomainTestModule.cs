using Volo.Abp.Modularity;

namespace FeatureRequest;

[DependsOn(
    typeof(FeatureRequestDomainModule),
    typeof(FeatureRequestTestBaseModule)
)]
public class FeatureRequestDomainTestModule : AbpModule
{

}
