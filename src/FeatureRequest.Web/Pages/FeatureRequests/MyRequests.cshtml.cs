using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    [Authorize]
    public class MyRequestsModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        public List<FeatureRequestDto> RequestList { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Filter { get; set; } = "mine";

        public MyRequestsModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public async Task OnGetAsync()
        {
            RequestList = Filter switch
            {
                "voted" => await _featureRequestAppService.GetMyVotedRequestsAsync(),
                _ => await _featureRequestAppService.GetMyRequestsAsync()
            };
        }
    }
}
