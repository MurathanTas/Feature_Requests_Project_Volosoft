using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using FeatureRequest.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.Admin.FeatureRequests
{
    [Authorize(FeatureRequestPermissions.FeatureRequests.UpdateStatus)]
    public class IndexModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        public IReadOnlyList<FeatureRequestDto> RequestList { get; private set; } = [];
        public long TotalCount { get; private set; }
        public int TotalPages { get; private set; }

        [BindProperty(SupportsGet = true)]
        public FeatureRequestStatus? SelectedStatus { get; set; }

        [BindProperty(SupportsGet = true)]
        public FeatureRequestCategory? SelectedCategory { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        
        [BindProperty(SupportsGet = true)]
        public int PageSize { get; set; } = GetFeatureRequestsInput.DefaultPageSize;

        public IndexModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public async Task OnGetAsync()
        {
            var input = new GetAdminFeatureRequestsInput
            {
                Status = SelectedStatus,
                Category = SelectedCategory,
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize
            };
            
            var result = await _featureRequestAppService.GetPagedFilteredListAsync(input);
            
            RequestList = result.Items.ToList();
            TotalCount = result.TotalCount;
            TotalPages = PageSize > 0 ? (int)((TotalCount + PageSize - 1) / PageSize) : 1;
            
            if (CurrentPage > TotalPages && TotalPages > 0)
                CurrentPage = TotalPages;
        }
    }
}
