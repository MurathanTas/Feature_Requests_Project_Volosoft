using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    public class IndexModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        public List<FeatureRequestDto> RequestList { get; set; }

        public IndexModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public async Task OnGetAsync()
        {
            var result = await _featureRequestAppService.GetListAsync(new Volo.Abp.Application.Dtos.PagedAndSortedResultRequestDto());

            RequestList = (List<FeatureRequestDto>)result.Items;
        }
    }
}
