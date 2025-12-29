using Volo.Abp.Modularity;

namespace FeatureRequest;

[DependsOn(
    typeof(FeatureRequestApplicationModule),
    typeof(FeatureRequestDomainTestModule)
)]
public class FeatureRequestApplicationTestModule : AbpModule
{

}
