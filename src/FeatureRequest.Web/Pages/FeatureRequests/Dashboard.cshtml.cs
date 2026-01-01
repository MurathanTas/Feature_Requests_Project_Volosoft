using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    public class DashboardModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        public DashboardStatisticsDto Statistics { get; set; }

        public DashboardModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public async Task OnGetAsync()
        {
            Statistics = await _featureRequestAppService.GetDashboardStatisticsAsync();
        }
    }
}
