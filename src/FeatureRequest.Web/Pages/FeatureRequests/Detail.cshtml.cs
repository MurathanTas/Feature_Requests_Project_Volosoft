using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
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

        public async Task OnGetAsync()
        {
            if (Id == Guid.Empty)
            {
                Response.Redirect("/FeatureRequests");
                return;
            }

            RequestDetail = await _featureRequestAppService.GetAsync(Id);

            Comments = await _commentAppService.GetCommentsAsync(Id);

            NewComment = new CreateCommentDto { FeatureRequestId = Id };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                RequestDetail = await _featureRequestAppService.GetAsync(NewComment.FeatureRequestId);
                Comments = await _commentAppService.GetCommentsAsync(NewComment.FeatureRequestId);
                return Page();
            }

            try
            {
                await _commentAppService.CreateAsync(NewComment);
            }
            catch (Exception ex)
            {
            }

            return RedirectToPage(new { id = NewComment.FeatureRequestId });
        }
    }
}
