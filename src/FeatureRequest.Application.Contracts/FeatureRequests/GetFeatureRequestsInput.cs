using System;
using Volo.Abp.Application.Dtos;

namespace FeatureRequest.FeatureRequests
{
    
    public class GetFeatureRequestsInput : PagedAndSortedResultRequestDto
    {
        public const int DefaultPageSize = 10;
        public const int MinPageSize = 5;
        public const int MaxPageSize = 50;

        public FeatureRequestCategory? Category { get; set; }

        public GetFeatureRequestsInput()
        {
            MaxResultCount = DefaultPageSize;
            SkipCount = 0;
        }
    }

    public class GetAdminFeatureRequestsInput : GetFeatureRequestsInput
    {
        public FeatureRequestStatus? Status { get; set; }
    }
}
