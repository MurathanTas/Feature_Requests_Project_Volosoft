using FeatureRequest.FeatureRequests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Threading.Tasks;

namespace FeatureRequest.Web.Pages.FeatureRequests
{
    public class DetailModel : PageModel
    {
        private readonly IFeatureRequestAppService _featureRequestAppService;

        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }

        public FeatureRequestDto RequestDetail { get; set; }

        public DetailModel(IFeatureRequestAppService featureRequestAppService)
        {
            _featureRequestAppService = featureRequestAppService;
        }

        public async Task OnGetAsync()
        {
            if (Id == Guid.Empty)
            {
                Response.Redirect("/FeatureRequests");
                return;
            }

            RequestDetail = await _featureRequestAppService.GetAsync(Id);
        }
    }
}
