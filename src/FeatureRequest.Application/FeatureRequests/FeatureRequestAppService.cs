using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public FeatureRequestAppService(IRepository<Entities.FeatureRequest, Guid> repository)
            : base(repository)
        {

        }

        public async Task UpvoteAsync(Guid id)
        {
            await UpdateVoteAsync(id, fr => fr.Upvote());
        }

        public async Task DownvoteAsync(Guid id)
        {
            await UpdateVoteAsync(id, fr => fr.Downvote());
        }

        private async Task UpdateVoteAsync(Guid id, Action<Entities.FeatureRequest> voteAction)
        {
            var featureRequest = await Repository.GetAsync(id);

            voteAction(featureRequest);

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
