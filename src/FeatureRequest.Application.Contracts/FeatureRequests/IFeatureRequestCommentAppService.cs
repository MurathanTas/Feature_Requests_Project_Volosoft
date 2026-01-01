using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace FeatureRequest.FeatureRequests
{
    public interface IFeatureRequestCommentAppService : ICrudAppService<
        FeatureRequestCommentDto,
        Guid,
        PagedAndSortedResultRequestDto,
        CreateCommentDto>
    {
        Task<List<FeatureRequestCommentDto>> GetCommentsAsync(Guid featureRequestId);
    }
}
