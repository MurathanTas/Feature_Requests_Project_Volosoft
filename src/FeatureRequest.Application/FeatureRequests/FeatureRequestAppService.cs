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
        private readonly FeatureRequestManager _featureRequestManager;
        private readonly IRepository<FeatureRequestComment, Guid> _commentRepository;
        private readonly IIdentityUserRepository _userRepository;

        public FeatureRequestAppService(
            IRepository<Entities.FeatureRequest, Guid> repository,
            FeatureRequestManager featureRequestManager,
            IRepository<FeatureRequestComment, Guid> commentRepository,
            IIdentityUserRepository userRepository)
            : base(repository)
        {
            _featureRequestManager = featureRequestManager;
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
                dto.IsVoted = await _featureRequestManager.HasUserVotedAsync(id, CurrentUser.Id.Value);
            }

            return dto;
        }


        public override async Task<PagedResultDto<FeatureRequestDto>> GetListAsync(PagedAndSortedResultRequestDto input)
        {
            var result = await base.GetListAsync(input);

            await EnrichWithUserDataAsync(result.Items.ToList());

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

            await EnrichWithUserDataAsync(dtos);

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
            // Tüm vote iş mantığı FeatureRequestManager'a devredildi
            await _featureRequestManager.ToggleVoteAsync(id, CurrentUser.Id!.Value);
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

            await EnrichWithCreatorNamesAsync(dtos);

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
            var votedIds = await _featureRequestManager.GetUserVotesForRequestsAsync(requestIds, CurrentUser.Id!.Value);

            foreach (var dto in dtos)
            {
                dto.IsVoted = votedIds.Contains(dto.Id);
            }

            return dtos;
        }

        [Authorize]
        public async Task<List<FeatureRequestDto>> GetMyVotedRequestsAsync()
        {
            var votedRequestIds = await _featureRequestManager.GetVotedRequestIdsAsync(CurrentUser.Id!.Value);

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
            }

            await EnrichWithCreatorNamesAsync(dtos);

            return dtos;
        }

        public async Task<DashboardStatisticsDto> GetDashboardStatisticsAsync()
        {
            
            var queryable = await Repository.GetQueryableAsync();

            var totalComments = await _commentRepository.CountAsync();

            var totalRequests = await AsyncExecuter.CountAsync(queryable);


            var totalVotes = await AsyncExecuter.SumAsync(queryable, x => (int?)x.VoteCount) ?? 0;

            var result = new DashboardStatisticsDto
            {
                TotalRequests = totalRequests,
                TotalVotes = totalVotes,
                TotalComments = totalComments,
                CategoryStats = new List<CategoryStatDto>(),
                StatusStats = new List<StatusStatDto>(),
                TopVotedRequests = new List<FeatureRequestDto>()
            };

          
            var categoryGroupQuery = queryable
                .GroupBy(x => x.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    Count = g.Count(),
                    TotalVotes = g.Sum(x => (int?)x.VoteCount) ?? 0
                });

            var categoryResults = await AsyncExecuter.ToListAsync(categoryGroupQuery);

            // Sonuçları DTO'ya çevir 
            foreach (var item in categoryResults)
            {
                result.CategoryStats.Add(new CategoryStatDto
                {
                    Category = item.CategoryId.ToString(), // Enum ismini string yapar
                    RequestCount = item.Count,
                    TotalVotes = item.TotalVotes
                });
            }

            // Durum İstatistikleri        
            var statusGroupQuery = queryable
                .GroupBy(x => x.Status)
                .Select(g => new
                {
                    Status = g.Key,
                    Count = g.Count()
                });

            var statusResults = await AsyncExecuter.ToListAsync(statusGroupQuery);

            foreach (var item in statusResults)
            {
                result.StatusStats.Add(new StatusStatDto
                {
                    Status = item.Status.ToString(),
                    Count = item.Count
                });
            }

            // En Çok Oy Alan 5 İstek
            var topRequestsQuery = queryable
                .OrderByDescending(x => x.VoteCount)
                .ThenByDescending(x => x.CreationTime) // Oylar eşitse yeni olana öncelik ver
                .Take(5);

            var topRequestsEntities = await AsyncExecuter.ToListAsync(topRequestsQuery);

            result.TopVotedRequests = ObjectMapper.Map<List<Entities.FeatureRequest>, List<FeatureRequestDto>>(topRequestsEntities);

            // Eğer Top 5 listesinde de kullanıcı isimlerini göstermek istiyorsan:
            await EnrichWithCreatorNamesAsync(result.TopVotedRequests);

            return result;
        }


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
                var votedIds = await _featureRequestManager.GetUserVotesForRequestsAsync(requestIds, CurrentUser.Id.Value);

                foreach (var dto in dtos)
                {
                    dto.IsVoted = votedIds.Contains(dto.Id);
                }
            }

            await EnrichWithCreatorNamesAsync(dtos);
        }

        private async Task EnrichWithCreatorNamesAsync(List<FeatureRequestDto> dtos)
        {
            var creatorIds = dtos
                .Where(d => d.CreatorId.HasValue)
                .Select(d => d.CreatorId!.Value)
                .Distinct()
                .ToList();

            if (!creatorIds.Any()) return;

            var userRepository = (IRepository<IdentityUser, Guid>)_userRepository;
            var queryable = await userRepository.GetQueryableAsync();
 
            var users = await AsyncExecuter.ToListAsync(
                queryable.Where(u => creatorIds.Contains(u.Id))
            );

            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

            foreach (var dto in dtos)
            {
                if (dto.CreatorId.HasValue && userDict.TryGetValue(dto.CreatorId.Value, out var userName))
                {
                    dto.CreatorUserName = userName;
                }
            }
        }

    }
}
