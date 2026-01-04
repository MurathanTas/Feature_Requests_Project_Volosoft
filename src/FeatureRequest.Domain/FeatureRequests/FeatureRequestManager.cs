using FeatureRequest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;

namespace FeatureRequest.FeatureRequests
{
  
    public class FeatureRequestManager : DomainService
    {
        private readonly IRepository<Entities.FeatureRequest, Guid> _featureRequestRepository;
        private readonly IRepository<FeatureRequestVote, Guid> _voteRepository;

        public FeatureRequestManager(
            IRepository<Entities.FeatureRequest, Guid> featureRequestRepository,
            IRepository<FeatureRequestVote, Guid> voteRepository)
        {
            _featureRequestRepository = featureRequestRepository;
            _voteRepository = voteRepository;
        }


        public async Task<bool> ToggleVoteAsync(Guid featureRequestId, Guid userId)
        {
            var featureRequest = await _featureRequestRepository.GetAsync(featureRequestId);

            if (!featureRequest.CanBeVoted())
            {
                throw new UserFriendlyException("Bu özellik isteği artık oylamaya kapalı.");
            }

            var existingVote = await _voteRepository.FirstOrDefaultAsync(
                v => v.FeatureRequestId == featureRequestId && v.CreatorId == userId);

            if (existingVote == null)
            {
                await _voteRepository.InsertAsync(new FeatureRequestVote 
                { 
                    FeatureRequestId = featureRequestId 
                });
                featureRequest.Upvote();
                await _featureRequestRepository.UpdateAsync(featureRequest);
                return true; 
            }
            else
            {
                await _voteRepository.DeleteAsync(existingVote);
                featureRequest.Downvote();
                await _featureRequestRepository.UpdateAsync(featureRequest);
                return false; 
            }
        }

        public async Task<HashSet<Guid>> GetUserVotesForRequestsAsync(List<Guid> requestIds, Guid userId)
        {
            if (!requestIds.Any())
                return new HashSet<Guid>();

            var votes = await _voteRepository.GetListAsync(
                v => requestIds.Contains(v.FeatureRequestId) && v.CreatorId == userId);
            
            return votes.Select(v => v.FeatureRequestId).ToHashSet();
        }


        public async Task<bool> HasUserVotedAsync(Guid featureRequestId, Guid userId)
        {
            return await _voteRepository.AnyAsync(
                v => v.FeatureRequestId == featureRequestId && v.CreatorId == userId);
        }


        public async Task<List<Guid>> GetVotedRequestIdsAsync(Guid userId)
        {
            var votes = await _voteRepository.GetListAsync(v => v.CreatorId == userId);
            return votes.Select(v => v.FeatureRequestId).ToList();
        }
    }
}
