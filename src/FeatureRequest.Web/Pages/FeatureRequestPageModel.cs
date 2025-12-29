using FeatureRequest.Localization;
using Volo.Abp.AspNetCore.Mvc.UI.RazorPages;

namespace FeatureRequest.Web.Pages;

/* Inherit your PageModel classes from this class.
 */
public abstract class FeatureRequestPageModel : AbpPageModel
{
    protected FeatureRequestPageModel()
    {
        LocalizationResourceType = typeof(FeatureRequestResource);
    }
}
