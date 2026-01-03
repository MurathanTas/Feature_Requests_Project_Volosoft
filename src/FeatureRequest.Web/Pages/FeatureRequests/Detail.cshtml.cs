using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    [Authorize]
    public class DetailModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;
        private readonly IFeatureRequestCommentAppService _commentAppService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public FeatureRequestDto RequestDetail { get; set; }

        public List<FeatureRequestCommentDto> Comments { get; set; }

        [BindProperty]
        public CreateCommentDto NewComment { get; set; }

        public DetailModel(
            IFeatureRequestAppService featureRequestAppService,
            IFeatureRequestCommentAppService commentAppService)
        {
            _featureRequestAppService = featureRequestAppService;
            _commentAppService = commentAppService;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            if (Id == Guid.Empty)
            {
                return RedirectToPage("Index");
            }

            RequestDetail = await _featureRequestAppService.GetAsync(Id);

            if (RequestDetail == null)
            {
                return NotFound();
            }

            Comments = await _commentAppService.GetCommentsAsync(Id);

            NewComment = new CreateCommentDto { FeatureRequestId = Id };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RequestDetail = await _featureRequestAppService.GetAsync(NewComment.FeatureRequestId);
                Comments = await _commentAppService.GetCommentsAsync(NewComment.FeatureRequestId);
                return Page();
            }

            await _commentAppService.CreateAsync(NewComment);

            return RedirectToPage(new { id = NewComment.FeatureRequestId });
        }
    }
}
