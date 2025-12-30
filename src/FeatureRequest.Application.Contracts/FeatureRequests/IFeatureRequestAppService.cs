using System;
using System.Collections.Generic;
using System.Text;
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
        
    }
}
