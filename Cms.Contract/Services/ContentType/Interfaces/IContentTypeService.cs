using Cms.Contract.Common.Pagination;
using Cms.Contract.Services.ContentType.Dtos;

namespace Cms.Contract.Services.ContentType.Interfaces
{
    public interface IContentTypeService
    {
        Task<List<ContentTypeLookupDto>> ListContentTypeLookupsAsync(ListContentTypeLookupsRequestDto requestDto, Guid accountId);

        Task<List<GetContentFieldsResponseDto>> ListContentFieldsAsync(Guid typeId, Guid accountId);

        Task<CreateContentTypeResponseDto> CreateContentTypeAsync(CreateContentTypeRequestDto req);

        Task<UpdateContentTypeResponseDto> UpdateContentTypeAsync(UpdateContentTypeRequestDto req);

        Task<DeleteContentTypeResponseDto> DeleteContentTypeAsync(DeleteContentTypeRequestDto req);
    }
}
