using System;
using System.Threading.Tasks;
using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Volo.Abp.ObjectMapping;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    [Authorize]
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

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id == Guid.Empty)
            {
                return RedirectToPage("Index");
            }

            var requestDto = await _featureRequestAppService.GetAsync(Id);

            if (requestDto == null)
            {
                return NotFound();
            }

            //Request = ObjectMapper.Map<FeatureRequestDto, UpdateFeatureRequestDto>(requestDto);
            Request = new UpdateFeatureRequestDto
            {
                Title = requestDto.Title,
                Description = requestDto.Description,
                CategoryId = requestDto.CategoryId,
                Status = requestDto.Status
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            await _featureRequestAppService.UpdateAsync(Id, Request);

            return RedirectToPage("Detail", new { id = Id });
        }
    }
}
