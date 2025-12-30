using System;
using System.Collections.Generic;
using System.Text;
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
    }
}
