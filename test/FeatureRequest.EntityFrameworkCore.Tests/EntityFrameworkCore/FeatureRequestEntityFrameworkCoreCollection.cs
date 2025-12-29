using Xunit;

namespace FeatureRequest.EntityFrameworkCore;

[CollectionDefinition(FeatureRequestTestConsts.CollectionDefinitionName)]
public class FeatureRequestEntityFrameworkCoreCollection : ICollectionFixture<FeatureRequestEntityFrameworkCoreFixture>
{

}
