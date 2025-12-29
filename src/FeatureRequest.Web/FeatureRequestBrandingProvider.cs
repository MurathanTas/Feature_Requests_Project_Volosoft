using Microsoft.Extensions.Localization;
using FeatureRequest.Localization;
using Volo.Abp.Ui.Branding;
using Volo.Abp.DependencyInjection;

namespace FeatureRequest.Web;

[Dependency(ReplaceServices = true)]
public class FeatureRequestBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<FeatureRequestResource> _localizer;

    public FeatureRequestBrandingProvider(IStringLocalizer<FeatureRequestResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
