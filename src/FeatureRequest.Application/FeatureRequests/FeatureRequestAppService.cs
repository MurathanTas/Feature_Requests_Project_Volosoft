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
        private readonly IRepository<FeatureRequestComment, Guid> _commentRepository;
        private readonly IIdentityUserRepository _userRepository;

        public FeatureRequestAppService(
            IRepository<Entities.FeatureRequest, Guid> repository,
            IRepository<FeatureRequestVote, Guid> voteRepository,
            IRepository<FeatureRequestComment, Guid> commentRepository,
            IIdentityUserRepository userRepository) 
            : base(repository)
        {
            _voteRepository = voteRepository;
            _commentRepository = commentRepository;
            _userRepository = userRepository;
            
            GetPolicyName = FeatureRequestPermissions.FeatureRequests.Default;
            GetListPolicyName = FeatureRequestPermissions.FeatureRequests.Default;
            CreatePolicyName = FeatureRequestPermissions.FeatureRequests.Create;
            UpdatePolicyName = FeatureRequestPermissions.FeatureRequests.Edit;
            DeletePolicyName = FeatureRequestPermissions.FeatureRequests.Delete;
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

        public async Task<PagedResultDto<FeatureRequestDto>> GetPagedRequestsAsync(GetFeatureRequestsInput input)
        {
            NormalizePaginationInput(input);
            
            var queryable = await Repository.GetQueryableAsync();
            
            if (input.Category.HasValue)
            {
                queryable = queryable.Where(x => x.CategoryId == input.Category.Value);
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);
            
            var query = queryable
                .OrderByDescending(x => x.VoteCount)
                .ThenByDescending(x => x.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);
                
            var entities = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);

            await EnrichWithUserDataAsync(dtos);

            return new PagedResultDto<FeatureRequestDto>(totalCount, dtos);
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
            var featureRequest = await Repository.GetAsync(id);
            
            // Onaylanmış, tamamlanmış veya reddedilmiş isteklere oy verilemez
            var nonVotableStatuses = new[] { 
                FeatureRequestStatus.Approved, 
                FeatureRequestStatus.Completed, 
                FeatureRequestStatus.Rejected 
            };
            if (nonVotableStatuses.Contains(featureRequest.Status))
            {
                throw new Volo.Abp.UserFriendlyException("Bu özellik isteği artık oylamaya kapalı.");
            }
            
            var existingVote = await _voteRepository.FirstOrDefaultAsync(v =>
                v.FeatureRequestId == id && v.CreatorId == CurrentUser.Id);

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

        [Authorize(FeatureRequestPermissions.FeatureRequests.UpdateStatus)]
        public async Task UpdateStatusAsync(Guid id, FeatureRequestStatus status)
        {
            var entity = await Repository.GetAsync(id);
            entity.Status = status;
            await Repository.UpdateAsync(entity);
        }

        [Authorize(FeatureRequestPermissions.FeatureRequests.UpdateStatus)]
        public async Task<List<FeatureRequestDto>> GetFilteredListAsync(FeatureRequestStatus? status, FeatureRequestCategory? category)
        {
            var queryable = await Repository.GetQueryableAsync();

            if (status.HasValue)
            {
                queryable = queryable.Where(x => x.Status == status.Value);
            }

            if (category.HasValue)
            {
                queryable = queryable.Where(x => x.CategoryId == category.Value);
            }

            var query = queryable
                .OrderByDescending(x => x.VoteCount)
                .ThenByDescending(x => x.CreationTime);

            var entities = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);

            foreach (var dto in dtos)
            {
                if (dto.CreatorId.HasValue)
                {
                    var user = await _userRepository.FindAsync(dto.CreatorId.Value);
                    if (user != null) dto.CreatorUserName = user.UserName;
                }
            }

            return dtos;
        }

        [Authorize(FeatureRequestPermissions.FeatureRequests.UpdateStatus)]
        public async Task<PagedResultDto<FeatureRequestDto>> GetPagedFilteredListAsync(GetAdminFeatureRequestsInput input)
        {
            NormalizePaginationInput(input);
            
            var queryable = await Repository.GetQueryableAsync();

            if (input.Status.HasValue)
            {
                queryable = queryable.Where(x => x.Status == input.Status.Value);
            }

            if (input.Category.HasValue)
            {
                queryable = queryable.Where(x => x.CategoryId == input.Category.Value);
            }

            var totalCount = await AsyncExecuter.CountAsync(queryable);

            var query = queryable
                .OrderByDescending(x => x.VoteCount)
                .ThenByDescending(x => x.CreationTime)
                .Skip(input.SkipCount)
                .Take(input.MaxResultCount);

            var entities = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);

            await EnrichWithCreatorNamesAsync(dtos);

            return new PagedResultDto<FeatureRequestDto>(totalCount, dtos);
        }

        [Authorize]
        public async Task<List<FeatureRequestDto>> GetMyRequestsAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            var query = queryable
                .Where(x => x.CreatorId == CurrentUser.Id)
                .OrderByDescending(x => x.CreationTime);

            var entities = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);

            var requestIds = dtos.Select(x => x.Id).ToList();
            var myVotes = await _voteRepository.GetListAsync(v =>
                requestIds.Contains(v.FeatureRequestId) && v.CreatorId == CurrentUser.Id);

            foreach (var dto in dtos)
            {
                dto.IsVoted = myVotes.Any(v => v.FeatureRequestId == dto.Id);
            }

            return dtos;
        }

        [Authorize]
        public async Task<List<FeatureRequestDto>> GetMyVotedRequestsAsync()
        {
            var myVotes = await _voteRepository.GetListAsync(v => v.CreatorId == CurrentUser.Id);
            var votedRequestIds = myVotes.Select(v => v.FeatureRequestId).ToList();

            var queryable = await Repository.GetQueryableAsync();
            var query = queryable
                .Where(x => votedRequestIds.Contains(x.Id))
                .OrderByDescending(x => x.VoteCount)
                .ThenByDescending(x => x.CreationTime);

            var entities = await AsyncExecuter.ToListAsync(query);
            var dtos = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(entities);

            foreach (var dto in dtos)
            {
                dto.IsVoted = true;
                if (dto.CreatorId.HasValue)
                {
                    var user = await _userRepository.FindAsync(dto.CreatorId.Value);
                    if (user != null) dto.CreatorUserName = user.UserName;
                }
            }

            return dtos;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            var queryable = await Repository.GetQueryableAsync();
            var allRequests = await AsyncExecuter.ToListAsync(queryable);
            var allComments = await _commentRepository.GetListAsync();

            var result = new DashboardStatisticsDto
            {
                TotalRequests = allRequests.Count,
                TotalVotes = allRequests.Sum(x => x.VoteCount),
                TotalComments = allComments.Count
            };

            // Kategori istatistikleri
            var categories = Enum.GetValues<FeatureRequestCategory>();
            foreach (var category in categories)
            {
                var requestsInCategory = allRequests.Where(x => x.CategoryId == category).ToList();
                result.CategoryStats.Add(new CategoryStatDto
                {
                    Category = category.ToString(),
                    RequestCount = requestsInCategory.Count,
                    TotalVotes = requestsInCategory.Sum(x => x.VoteCount)
                });
            }

            // Durum istatistikleri
            var statuses = Enum.GetValues<FeatureRequestStatus>();
            foreach (var status in statuses)
            {
                result.StatusStats.Add(new StatusStatDto
                {
                    Status = status.ToString(),
                    Count = allRequests.Count(x => x.Status == status)
                });
            }

            // En çok oy alan 5 istek
            var topRequests = allRequests
                .OrderByDescending(x => x.VoteCount)
                .Take(5)
                .ToList();

            result.TopVotedRequests = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(topRequests);

            return result;
        }

        #region Private Helper Methods

        private static void NormalizePaginationInput(GetFeatureRequestsInput input)
        {
            if (input.SkipCount < 0)
                input.SkipCount = 0;

            if (input.MaxResultCount < GetFeatureRequestsInput.MinPageSize)
                input.MaxResultCount = GetFeatureRequestsInput.MinPageSize;

            if (input.MaxResultCount > GetFeatureRequestsInput.MaxPageSize)
                input.MaxResultCount = GetFeatureRequestsInput.MaxPageSize;
        }

        private async Task EnrichWithUserDataAsync(List<FeatureRequestDto> dtos)
        {
            if (!dtos.Any()) return;

            var requestIds = dtos.Select(x => x.Id).ToList();

            if (CurrentUser.Id.HasValue)
            {
                var myVotes = await _voteRepository.GetListAsync(v =>
                    requestIds.Contains(v.FeatureRequestId) && v.CreatorId == CurrentUser.Id);

                foreach (var dto in dtos)
                {
                    dto.IsVoted = myVotes.Any(v => v.FeatureRequestId == dto.Id);
                }
            }

            await EnrichWithCreatorNamesAsync(dtos);
        }

        private async Task EnrichWithCreatorNamesAsync(List<FeatureRequestDto> dtos)
        {
            foreach (var dto in dtos)
            {
                if (dto.CreatorId.HasValue)
                {
                    var user = await _userRepository.FindAsync(dto.CreatorId.Value);
                    if (user != null) dto.CreatorUserName = user.UserName;
                }
            }
        }

        #endregion
    }
}
