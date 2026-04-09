using Cms.Contract.Services.CMS.Dtos;

namespace Cms.Contract.Services.CMS.Interfaces
{
    public interface ICMSService
    {
        Task<CreateContentResponseDto> CreateContentAsync(CreateContentRequestDto requestDto);

        Task<List<GetContentsResponseDto>> ListContentsAsync(Guid typeId, Guid accountId);

        Task<UpdateContentResponseDto> UpdateContentAsync(UpdateContentRequestDto requestDto, Guid accountId);

        Task<DeleteContentResponseDto> DeleteContentAsync(DeleteContentRequestDto requestDto, Guid accountId);

        Task<List<ContentTypeLookupDto>> ListContentTypeLookupsAsync(ListContentTypeLookupsRequestDto requestDto, Guid accountId);

        Task<List<GetContentFieldsResponseDto>> ListContentFieldsAsync(Guid typeId, Guid accountId);

        Task<CreateContentTypeResponseDto> CreateContentTypeAsync(CreateContentTypeRequestDto req, Guid accountId);

        Task<UpdateContentTypeResponseDto> UpdateContentTypeAsync(UpdateContentTypeRequestDto req, Guid accountId);

        Task<DeleteContentTypeResponseDto> DeleteContentTypeAsync(DeleteContentTypeRequestDto req, Guid accountId);
    }
}
