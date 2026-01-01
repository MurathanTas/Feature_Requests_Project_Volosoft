using FeatureRequest.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;

namespace FeatureRequest.Permissions
{
    public class FeatureRequestPermissionDefinitionProvider : PermissionDefinitionProvider
    {
        public override void Define(IPermissionDefinitionContext context)
        {
            var myGroup = context.AddGroup(FeatureRequestPermissions.GroupName, L("Permission:FeatureRequest"));

            var featureRequestsPermission = myGroup.AddPermission(FeatureRequestPermissions.FeatureRequests.Default, L("Permission:FeatureRequests"));

            featureRequestsPermission.AddChild(FeatureRequestPermissions.FeatureRequests.Create, L("Permission:FeatureRequests.Create"));
            featureRequestsPermission.AddChild(FeatureRequestPermissions.FeatureRequests.Edit, L("Permission:FeatureRequests.Edit"));
            featureRequestsPermission.AddChild(FeatureRequestPermissions.FeatureRequests.Delete, L("Permission:FeatureRequests.Delete"));
        }

        private static LocalizableString L(string name)
        {
            return LocalizableString.Create<FeatureRequestResource>(name);
        }
    }
}