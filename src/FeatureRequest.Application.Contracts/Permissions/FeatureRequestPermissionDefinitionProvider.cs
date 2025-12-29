using FeatureRequest.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FeatureRequest.Permissions;

public class FeatureRequestPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(FeatureRequestPermissions.GroupName);
        //Define your own permissions here. Example:
        //myGroup.AddPermission(FeatureRequestPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<FeatureRequestResource>(name);
    }
}
