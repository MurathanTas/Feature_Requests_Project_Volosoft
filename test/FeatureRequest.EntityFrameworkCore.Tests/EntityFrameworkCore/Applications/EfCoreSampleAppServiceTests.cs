using FeatureRequest.Samples;
using Xunit;

namespace FeatureRequest.EntityFrameworkCore.Applications;

[Collection(FeatureRequestTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<FeatureRequestEntityFrameworkCoreTestModule>
{

}
