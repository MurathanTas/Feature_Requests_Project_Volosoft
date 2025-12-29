using System.Threading.Tasks;
using Shouldly;
using Xunit;

namespace FeatureRequest.Pages;

public class Index_Tests : FeatureRequestWebTestBase
{
    [Fact]
    public async Task Welcome_Page()
    {
        var response = await GetResponseAsStringAsync("/");
        response.ShouldNotBeNull();
    }
}
