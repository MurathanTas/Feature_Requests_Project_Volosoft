using FeatureRequest.Entities;
using FeatureRequest.Permissions;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;

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
        private readonly IIdentityUserRepository _userRepository;

        public FeatureRequestAppService(
            IRepository<Entities.FeatureRequest, Guid> repository,
            IRepository<FeatureRequestVote, Guid> voteRepository,
            IIdentityUserRepository userRepository) 
            : base(repository)
        {
            _voteRepository = voteRepository;
            _userRepository = userRepository;
        }


        [Authorize]
        public override async Task<FeatureRequestDto> CreateAsync(CreateFeatureRequestDto input)
        {
            return await base.CreateAsync(input);
        }

        [Authorize]
        public override async Task<FeatureRequestDto> UpdateAsync(Guid id, UpdateFeatureRequestDto input)
        {
            var entity = await Repository.GetAsync(id);
            
            if (entity.CreatorId != CurrentUser.Id)
            {
                throw new Volo.Abp.Authorization.AbpAuthorizationException("Bu özellik isteğini düzenleme yetkiniz yok.");
            }
            
            return await base.UpdateAsync(id, input);
        }

        [Authorize(FeatureRequestPermissions.FeatureRequests.Delete)]
        public override async Task DeleteAsync(Guid id)
        {
            await base.DeleteAsync(id);
        }


        public override async Task<FeatureRequestDto> GetAsync(Guid id)
        {
            var dto = await base.GetAsync(id);

            if (dto.CreatorId.HasValue)
            {
                var user = await _userRepository.FindAsync(dto.CreatorId.Value);
                if (user != null) dto.CreatorUserName = user.UserName;
            }

            if (CurrentUser.Id.HasValue)
            {
                dto.IsVoted = await _voteRepository.AnyAsync(v =>
                    v.FeatureRequestId == id && v.CreatorId == CurrentUser.Id);
            }

            return dto;
        }

     
        public override async Task<PagedResultDto<FeatureRequestDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var result = await base.GetListAsync(input);

            if (CurrentUser.Id.HasValue && result.Items.Any())
            {
                var requestIds = result.Items.Select(x => x.Id).ToList();

                var myVotes = await _voteRepository.GetListAsync(v =>
                    requestIds.Contains(v.FeatureRequestId) && v.CreatorId == CurrentUser.Id);

                foreach (var dto in result.Items)
                {
                    dto.IsVoted = myVotes.Any(v => v.FeatureRequestId == dto.Id);

                    if (dto.CreatorId.HasValue)
                    {
                        var user = await _userRepository.FindAsync(dto.CreatorId.Value);
                        if (user != null) dto.CreatorUserName = user.UserName;
                    }
                }
            }

            return result;
        }

        public async Task<List<FeatureRequestDto>> GetTopRequestsAsync(int count, FeatureRequestCategory? category = null)
        {
            var queryable = await Repository.GetQueryableAsync();
            
            if (category.HasValue)
            {
                queryable = queryable.Where(x => x.CategoryId == category.Value);
            }
            
            var query = queryable
                .OrderByDescending(x => x.VoteCount)
                .ThenByDescending(x => x.CreationTime)
                .Take(count);
            var entities = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);

            if (CurrentUser.Id.HasValue && dtos.Any())
            {
                var requestIds = dtos.Select(x => x.Id).ToList();
                var myVotes = await _voteRepository.GetListAsync(v =>
                    requestIds.Contains(v.FeatureRequestId) && v.CreatorId == CurrentUser.Id);

                foreach (var dto in dtos)
                {
                    dto.IsVoted = myVotes.Any(v => v.FeatureRequestId == dto.Id);
                    if (dto.CreatorId.HasValue)
                    {
                        var user = await _userRepository.FindAsync(dto.CreatorId.Value);
                        if (user != null) dto.CreatorUserName = user.UserName;
                    }
                }
            }

            return dtos;
        }


        [Authorize]
        public async Task UpvoteAsync(Guid id)
        {
            await UpdateVoteAsync(id);
        }

        [Authorize]
        public async Task DownvoteAsync(Guid id)
        {
            await UpdateVoteAsync(id);
        }

        private async Task UpdateVoteAsync(Guid id)
        {
            var existingVote = await _voteRepository.FirstOrDefaultAsync(v =>
                v.FeatureRequestId == id && v.CreatorId == CurrentUser.Id);
            var featureRequest = await Repository.GetAsync(id);

            if (existingVote == null)
            {
                await _voteRepository.InsertAsync(new FeatureRequestVote { FeatureRequestId = id });
                featureRequest.Upvote();
            }
            else
            {
                await _voteRepository.DeleteAsync(existingVote);
                featureRequest.Downvote();
            }
            await Repository.UpdateAsync(featureRequest);
        }
    }
}
