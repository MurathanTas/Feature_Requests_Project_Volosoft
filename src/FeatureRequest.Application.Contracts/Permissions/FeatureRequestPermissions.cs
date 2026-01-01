namespace FeatureRequest.Permissions;

public static class FeatureRequestPermissions
{
    public const string GroupName = "FeatureRequest";

    //Add your own permission names. Example:
    //public const string MyPermission1 = GroupName + ".MyPermission1";

    public static class FeatureRequests
    {
        public const string Default = GroupName + ".FeatureRequests";
        public const string Create = Default + ".Create";
        public const string Edit = Default + ".Edit";
        public const string Delete = Default + ".Delete"; 
    }
}
