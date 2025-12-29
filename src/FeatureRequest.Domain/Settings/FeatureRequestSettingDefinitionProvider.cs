using Volo.Abp.Settings;

namespace FeatureRequest.Settings;

public class FeatureRequestSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(FeatureRequestSettings.MySetting1));
    }
}
