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
    public class FeatureRequestCommentAppService :
        CrudAppService<
            FeatureRequestComment,
            FeatureRequestCommentDto,
            Guid,
            PagedAndSortedResultRequestDto,
            CreateCommentDto>,
        IFeatureRequestCommentAppService
    {
        private readonly IIdentityUserRepository _userRepository;

        public FeatureRequestCommentAppService(
            IRepository<FeatureRequestComment, Guid> repository,
            IIdentityUserRepository userRepository) 
            : base(repository)
        {
            _userRepository = userRepository;

        }

        [Authorize]
        public override async Task<FeatureRequestCommentDto> CreateAsync(CreateCommentDto input)
        {
            return await base.CreateAsync(input);
        }

        public async Task<List<FeatureRequestCommentDto>> GetCommentsAsync(Guid featureRequestId)
        {
            var comments = await Repository.GetListAsync(c => c.FeatureRequestId == featureRequestId);

            var commentDtos = ObjectMapper.Map<List<FeatureRequestComment>, List<FeatureRequestCommentDto>>(comments);

            var creatorIds = commentDtos
                .Where(d => d.CreatorId.HasValue)
                .Select(d => d.CreatorId!.Value)
                .Distinct()
                .ToList();

            if (!creatorIds.Any())
            {
                return commentDtos.OrderByDescending(c => c.CreationTime).ToList();
            }
         
            var genericRepo = (IRepository<IdentityUser, Guid>)_userRepository;

            var users = await genericRepo.GetListAsync(u => creatorIds.Contains(u.Id));

            var userDict = users.ToDictionary(u => u.Id, u => u.UserName);

            foreach (var dto in commentDtos)
            {
                if (dto.CreatorId.HasValue && userDict.TryGetValue(dto.CreatorId.Value, out var userName))
                {
                    dto.CreatorUserName = userName;
                }
                else
                {
                    dto.CreatorUserName = "Anonim";
                }
            }

            return commentDtos.OrderByDescending(c => c.CreationTime).ToList();
        }
    }
}