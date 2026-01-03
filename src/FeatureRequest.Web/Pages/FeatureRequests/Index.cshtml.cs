using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    public class IndexModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;
        
        private const int MinPageSize = 5;
        private const int MaxPageSize = 50;

        public IReadOnlyList<FeatureRequestDto> RequestList { get; private set; } = [];
        public long TotalCount { get; private set; }
        public int TotalPages { get; private set; }
        
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

        public async Task<IActionResult> OnGetAsync()
        {
            // Parametre validasyonu
            if (CurrentPage < 1)
                CurrentPage = 1;
            
            if (PageSize < MinPageSize)
                PageSize = MinPageSize;
            else if (PageSize > MaxPageSize)
                PageSize = MaxPageSize;
            
            var input = new GetFeatureRequestsInput
            {
                Category = SelectedCategory,
                SkipCount = (CurrentPage - 1) * PageSize,
                MaxResultCount = PageSize
            };
            
            var result = await _featureRequestAppService.GetPagedRequestsAsync(input);
            
            RequestList = result.Items.ToList();
            TotalCount = result.TotalCount;
            TotalPages = PageSize > 0 ? (int)((TotalCount + PageSize - 1) / PageSize) : 1;
            
            if (CurrentPage > TotalPages && TotalPages > 0)
            {
                CurrentPage = TotalPages;
                return RedirectToPage(new { CurrentPage, PageSize, SelectedCategory });
            }
            
            return Page();
        }
    }
}
