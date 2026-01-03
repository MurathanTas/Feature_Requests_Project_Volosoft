using System.Threading.Tasks;
using FeatureRequest.FeatureRequests; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    public class CreateModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        [BindProperty]
        public CreateFeatureRequestDto Request { get; set; }

        public CreateModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public void OnGet()
        {
            Request = new CreateFeatureRequestDto();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _featureRequestAppService.CreateAsync(Request);
            return RedirectToPage("Index");
        }
    }
}