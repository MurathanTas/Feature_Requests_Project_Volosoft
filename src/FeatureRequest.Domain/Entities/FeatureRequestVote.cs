using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace FeatureRequest.Entities
{
    public class FeatureRequestVote : CreationAuditedEntity<Guid>
    {
        public Guid FeatureRequestId { get; set; }
    }
}
