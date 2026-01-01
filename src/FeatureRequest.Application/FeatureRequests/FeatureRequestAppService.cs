using FeatureRequest.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace FeatureRequest.FeatureRequests
{
    public class FeatureRequestAppService :
        CrudAppService<
            Entities.FeatureRequest,
            FeatureRequestDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateFeatureRequestDto,
            UpdateFeatureRequestDto>,
        IFeatureRequestAppService
    {
        private readonly IRepository<FeatureRequestVote, Guid> _voteRepository;

        public FeatureRequestAppService(IRepository<Entities.FeatureRequest, Guid> repository, IRepository<FeatureRequestVote, Guid> voteRepository)
            : base(repository)
        {
            _voteRepository = voteRepository;
        }

        public async Task UpvoteAsync(Guid id)
        {
            await UpdateVoteAsync(id);
        }

        public async Task DownvoteAsync(Guid id)
        {
            await UpdateVoteAsync(id);
        }

        private async Task UpdateVoteAsync(Guid id)
        {
            if (CurrentUser.Id == null)
            {
                throw new UserFriendlyException("Oy vermek için giriş yapmalısınız!");
            }

            var existingVote = await _voteRepository.FirstOrDefaultAsync(v =>
                v.FeatureRequestId == id && v.CreatorId == CurrentUser.Id);

            var featureRequest = await Repository.GetAsync(id);

            if (existingVote == null)
            {

                await _voteRepository.InsertAsync(new FeatureRequestVote
                {
                    FeatureRequestId = id
                });

                featureRequest.Upvote();
            }
            else
            {

                await _voteRepository.DeleteAsync(existingVote);

                featureRequest.Downvote();
            }

            await Repository.UpdateAsync(featureRequest);
        }

        public async Task<List<FeatureRequestDto>> GetTopRequestsAsync(int count)
        {
            var queryable = await Repository.GetQueryableAsync();

            var query = queryable
                         .OrderByDescending(x => x.VoteCount)
                         .Take(count);

            var entities = await AsyncExecuter.ToListAsync(query);

            return ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);
        }

       
    }
}
