using Volo.Abp.Modularity;

namespace FeatureRequest;

/* Inherit from this class for your domain layer tests. */
public abstract class FeatureRequestDomainTestBase<TStartupModule> : FeatureRequestTestBase<TStartupModule>
    where TStartupModule : IAbpModule
{

}
