using FeatureRequest.Samples;
using Xunit;

namespace FeatureRequest.EntityFrameworkCore.Domains;

[Collection(FeatureRequestTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<FeatureRequestEntityFrameworkCoreTestModule>
{

}
