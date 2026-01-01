using System;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;

namespace FeatureRequest.Data
{
    public class FeatureRequestDataSeedContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IdentityUserManager _userManager;
        private readonly IGuidGenerator _guidGenerator;

        public FeatureRequestDataSeedContributor(
            IdentityUserManager userManager,
            IGuidGenerator guidGenerator)
        {
            _userManager = userManager;
            _guidGenerator = guidGenerator;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
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
        }
    }
}