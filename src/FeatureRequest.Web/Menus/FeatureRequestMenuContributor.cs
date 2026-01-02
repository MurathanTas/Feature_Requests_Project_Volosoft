using System.Threading.Tasks;
using FeatureRequest.Localization;
using FeatureRequest.MultiTenancy;
using FeatureRequest.Permissions;
using Microsoft.Extensions.DependencyInjection;
using Volo.Abp.Identity.Web.Navigation;
using Volo.Abp.SettingManagement.Web.Navigation;
using Volo.Abp.TenantManagement.Web.Navigation;
using Volo.Abp.UI.Navigation;

namespace FeatureRequest.Web.Menus;

public class FeatureRequestMenuContributor : IMenuContributor
{
    public async Task ConfigureMenuAsync(MenuConfigurationContext context)
    {
        if (context.Menu.Name == StandardMenus.Main)
        {
            await ConfigureMainMenuAsync(context);
        }
    }

    private async Task ConfigureMainMenuAsync(MenuConfigurationContext context)
    {
        var administration = context.Menu.GetAdministration();
        var l = context.GetLocalizer<FeatureRequestResource>();

        context.Menu.Items.Insert(
            0,
            new ApplicationMenuItem(
                FeatureRequestMenus.Home,
                l["Menu:Home"],
                "~/",
                icon: "fas fa-home",
                order: 0
            )
        );

        context.Menu.Items.Insert(
            1,
            new ApplicationMenuItem(
                FeatureRequestMenus.FeatureRequests,
                l["Menu:FeatureRequests"],
                "/FeatureRequests",
                icon: "fas fa-lightbulb",
                order: 1
            )
        );

        context.Menu.Items.Insert(
            1,
            new ApplicationMenuItem(
                "FeatureRequest.Dashboard",
                l["Menu:Dashboard"],
                "/FeatureRequests/Dashboard",
                icon: "fas fa-chart-pie",
                order: 1
            )
        );

        var currentUser = context.ServiceProvider.GetRequiredService<Volo.Abp.Users.ICurrentUser>();
        if (currentUser.IsAuthenticated)
        {
            context.Menu.Items.Insert(
                2,
                new ApplicationMenuItem(
                    "FeatureRequest.MyRequests",
                    l["Menu:MyRequests"],
                    "/FeatureRequests/MyRequests",
                    icon: "fas fa-clipboard-list",
                    order: 2
                )
            );
        }

        var hasAdminPermission = await context.IsGrantedAsync(FeatureRequestPermissions.FeatureRequests.UpdateStatus);
        
        if (hasAdminPermission)
        {
            administration.AddItem(
                new ApplicationMenuItem(
                    "FeatureRequest.Admin",
                    l["Menu:AdminFeatureRequests"],
                    "/Admin/FeatureRequests",
                    icon: "fas fa-tasks",
                    order: 0
                )
            );
        }

        administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        administration.TryRemoveMenuItem(SettingManagementMenuNames.GroupName);
        
        var identityMenu = administration.GetMenuItemOrNull(IdentityMenuNames.GroupName);
        if (identityMenu != null)
        {
            identityMenu.TryRemoveMenuItem(IdentityMenuNames.Roles);
        }
        
        if (!hasAdminPermission)
        {
            administration.TryRemoveMenuItem(IdentityMenuNames.GroupName);
        }
    }
}