using FeatureRequest.FeatureRequests;
using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.Domain.Entities.Auditing;

namespace FeatureRequest.Entities
{
    public class FeatureRequest : AuditedAggregateRoot<Guid>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public FeatureRequestStatus Status { get; set; }
        public int VoteCount { get; set; }

        public void Upvote()
        {
            VoteCount++;
        }
        public void Downvote()
        {
            if (VoteCount > 0)
            {
                VoteCount--;
            }
        }

        public FeatureRequestCategory CategoryId { get; set; }

        public ICollection<FeatureRequestComment> Comments { get; set; }
        public ICollection<FeatureRequestVote> Votes { get; set; }


    }
}
