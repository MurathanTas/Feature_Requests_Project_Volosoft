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
            0,
            new ApplicationMenuItem(
                FeatureRequestMenus.FeatureRequests,
                l["Özellik İstekleri"],
                "/FeatureRequests",
                icon: "fas fa-lightbulb",
                order: 1
            )
        );

        // İsteklerim - Sadece giriş yapmış kullanıcılar için
        var currentUser = context.ServiceProvider.GetRequiredService<Volo.Abp.Users.ICurrentUser>();
        if (currentUser.IsAuthenticated)
        {
            context.Menu.Items.Insert(
                2,
                new ApplicationMenuItem(
                    "FeatureRequest.MyRequests",
                    "İsteklerim",
                    "/FeatureRequests/MyRequests",
                    icon: "fas fa-clipboard-list",
                    order: 2
                )
            );
        }

        // Admin Menu - Özellik İstekleri Yönetimi
        if (await context.IsGrantedAsync(FeatureRequestPermissions.FeatureRequests.UpdateStatus))
        {
            administration.AddItem(
                new ApplicationMenuItem(
                    "FeatureRequest.Admin",
                    "Özellik İstekleri Yönetimi",
                    "/Admin/FeatureRequests",
                    icon: "fas fa-tasks",
                    order: 0
                )
            );
        }

        if (MultiTenancyConsts.IsEnabled)
        {
            administration.SetSubItemOrder(TenantManagementMenuNames.GroupName, 1);
        }
        else
        {
            administration.TryRemoveMenuItem(TenantManagementMenuNames.GroupName);
        }

        administration.SetSubItemOrder(IdentityMenuNames.GroupName, 2);
        administration.SetSubItemOrder(SettingManagementMenuNames.GroupName, 3);
    }
}