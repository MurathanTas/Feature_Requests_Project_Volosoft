using System;
using System.Collections.Generic;
using System.Text;
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
        Task<List<FeatureRequestDto>> GetTopRequestsAsync(int count, FeatureRequestCategory? category = null);
        Task UpdateStatusAsync(Guid id, FeatureRequestStatus status);
        Task<List<FeatureRequestDto>> GetFilteredListAsync(FeatureRequestStatus? status, FeatureRequestCategory? category);
        Task<List<FeatureRequestDto>> GetMyRequestsAsync();
        Task<List<FeatureRequestDto>> GetMyVotedRequestsAsync();
        Task<DashboardStatisticsDto> GetDashboardStatisticsAsync();
    }
}
