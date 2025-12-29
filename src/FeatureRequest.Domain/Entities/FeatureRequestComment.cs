using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace FeatureRequest.Entities
{
    public class FeatureRequestComment : CreationAuditedEntity<Guid>
    {
        public string CommentText { get; set; }
        public Guid FeatureRequestId { get; set; }
    }
}
