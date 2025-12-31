using System;
using System.Threading.Tasks;
using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    public class EditModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        [BindProperty]
        public UpdateFeatureRequestDto Request { get; set; }

        public EditModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public async Task OnGetAsync()
        {
            var requestDto = await _featureRequestAppService.GetAsync(Id);

            Request = new UpdateFeatureRequestDto
            {
                Title = requestDto.Title,
                Description = requestDto.Description,
                CategoryId = requestDto.CategoryId,
                Status = requestDto.Status
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await _featureRequestAppService.UpdateAsync(Id, Request);

            return RedirectToPage("Detail", new { id = Id });
        }
    }
}