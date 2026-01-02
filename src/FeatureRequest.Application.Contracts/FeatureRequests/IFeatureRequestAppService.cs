using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FeatureRequest.FeatureRequests
{
    public interface IFeatureRequestAppService : ICrudAppService<
            FeatureRequestDto,
            Guid,                           
            PagedAndSortedResultRequestDto, 
            CreateFeatureRequestDto,        
            UpdateFeatureRequestDto>
    {
        Task UpvoteAsync(Guid id);
        Task DownvoteAsync(Guid id);
        

        Task<PagedResultDto<FeatureRequestDto>> GetPagedRequestsAsync(GetFeatureRequestsInput input);
        

        Task<PagedResultDto<FeatureRequestDto>> GetPagedFilteredListAsync(GetAdminFeatureRequestsInput input);
        
        Task UpdateStatusAsync(Guid id, FeatureRequestStatus status);
        Task<List<FeatureRequestDto>> GetMyRequestsAsync();
        Task<List<FeatureRequestDto>> GetMyVotedRequestsAsync();
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
    }
}

