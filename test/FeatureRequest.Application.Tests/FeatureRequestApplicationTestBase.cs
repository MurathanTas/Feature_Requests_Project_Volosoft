using Volo.Abp.Modularity;

namespace FeatureRequest;

public abstract class FeatureRequestApplicationTestBase<TStartupModule> : FeatureRequestTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
