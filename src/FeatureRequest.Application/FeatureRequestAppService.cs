using System;
using System.Collections.Generic;
using System.Text;
using FeatureRequest.Localization;
using Volo.Abp.Application.Services;

namespace FeatureRequest;

/* Inherit your application services from this class.
 */
public abstract class FeatureRequestAppService : ApplicationService
{
    protected FeatureRequestAppService()
    {
        LocalizationResource = typeof(FeatureRequestResource);
    }
}
