using FeatureRequest.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace FeatureRequest.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class FeatureRequestController : AbpControllerBase
{
    protected FeatureRequestController()
    {
        LocalizationResource = typeof(FeatureRequestResource);
    }
}
