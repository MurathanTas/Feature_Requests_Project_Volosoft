using System;
using System.Threading.Tasks;
using FeatureRequest.Permissions;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.PermissionManagement;

namespace FeatureRequest.Data
{
    public class FeatureRequestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IdentityUserManager _userManager;
        private readonly IGuidGenerator _guidGenerator;
        private readonly IPermissionManager _permissionManager;

        public FeatureRequestDataSeedContributor(
            IdentityUserManager userManager,
            IGuidGenerator guidGenerator,
            IPermissionManager permissionManager)
        {
            _userManager = userManager;
            _guidGenerator = guidGenerator;
            _permissionManager = permissionManager;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            // Seed admin user
            var adminEmail = "murat@gmail.com";
            var adminPassword = "Password123!";
            var userName = "murat";

            var user = await _userManager.FindByEmailAsync(adminEmail);

            if (user == null)
            {
                user = new IdentityUser(
                    _guidGenerator.Create(),
                    userName,
                    adminEmail,
                    tenantId: null
                );

                var result = await _userManager.CreateAsync(user, adminPassword);

                if (result.Succeeded)
                {
                    user.SetEmailConfirmed(true);
                    await _userManager.UpdateAsync(user);
                    await _userManager.AddToRoleAsync(user, "admin");
                }
            }

            // Seed admin permissions
            await _permissionManager.SetForRoleAsync(
                "admin",
                FeatureRequestPermissions.FeatureRequests.UpdateStatus,
                true
            );
        }
    }
}
