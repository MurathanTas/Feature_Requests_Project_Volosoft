using System;
using Volo.Abp.Application.Dtos;

namespace FeatureRequest.FeatureRequests
{
    public class FeatureRequestCommentDto : CreationAuditedEntityDto<Guid>
    {
        public string CommentText { get; set; }
        public Guid FeatureRequestId { get; set; }

        public string CreatorUserName { get; set; }
    }
}