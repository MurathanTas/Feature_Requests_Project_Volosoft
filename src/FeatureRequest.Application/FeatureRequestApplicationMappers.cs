using FeatureRequest.Entities;
using FeatureRequest.FeatureRequests;
using Riok.Mapperly.Abstractions;
using Volo.Abp.Mapperly;

namespace FeatureRequest;

[Mapper]
public partial class FeatureRequestToDtoMapper : MapperBase<Entities.FeatureRequest, FeatureRequestDto>
{
    public override partial FeatureRequestDto Map(Entities.FeatureRequest source);
    public override partial void Map(Entities.FeatureRequest source, FeatureRequestDto destination);
}

[Mapper]
public partial class CreateDtoToFeatureRequestMapper : MapperBase<CreateFeatureRequestDto, Entities.FeatureRequest>
{
    public override partial Entities.FeatureRequest Map(CreateFeatureRequestDto source);
    public override partial void Map(CreateFeatureRequestDto source, Entities.FeatureRequest destination);
}

[Mapper]
public partial class UpdateDtoToFeatureRequestMapper : MapperBase<UpdateFeatureRequestDto, Entities.FeatureRequest>
{
    public override partial Entities.FeatureRequest Map(UpdateFeatureRequestDto source);
    public override partial void Map(UpdateFeatureRequestDto source, Entities.FeatureRequest destination);
}


[Mapper]
public partial class CommentToDtoMapper : MapperBase<FeatureRequestComment, FeatureRequestCommentDto>
{
    public override partial FeatureRequestCommentDto Map(FeatureRequestComment source);
    public override partial void Map(FeatureRequestComment source, FeatureRequestCommentDto destination);
}

[Mapper]
public partial class CreateCommentDtoToEntityMapper : MapperBase<CreateCommentDto, FeatureRequestComment>
{
    public override partial FeatureRequestComment Map(CreateCommentDto source);
    public override partial void Map(CreateCommentDto source, FeatureRequestComment destination);
}