using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Application.Dtos;

namespace FeatureRequest.FeatureRequests
{
    public class FeatureRequestDto : AuditedEntityDto<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public FeatureRequestStatus Status { get; set; }
        public FeatureRequestCategory CategoryId { get; set; }
        public int VoteCount { get; set; }
        public string CreatorUserName { get; set; }
    }
}
