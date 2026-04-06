using Cms.Contract.Repositories.Content.Interfaces;
using Cms.Contract.Repositories.ContentType.Interfaces;
using Cms.Contract.Services.CMS.Dtos;
using Cms.Contract.Services.CMS.Interfaces;
using Cms.Contract.Services.RBAC.Interfaces;
using Cms.Contract.Services.UnitOfWork.Interfaces;

namespace Cms.Application.Services.CMS
{
    public class CMSService : ICMSService
    {
        private const string DefaultContentStatus = "Draft";
        private const string ContentViewPermissionCode = "Content.View";
        private const string ContentCreatePermissionCode = "Content.Create";
        private const string ContentEditPermissionCode = "Content.Edit";
        private const string ContentDeletePermissionCode = "Content.Delete";

        private static readonly HashSet<string> AllowedFieldTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "text",
            "number",
            "date",
            "textarea"
        };

        private readonly IUnitOfWork _unitOfWork;
        private readonly IContentRepository _contentRepository;
        private readonly IContentTypeRepository _contentTypeRepository;
        private readonly IRBACService _rbacService;

        public CMSService(
            IUnitOfWork unitOfWork,
            IContentRepository contentRepository,
            IContentTypeRepository contentTypeRepository,
            IRBACService rbacService
        )
        {
            _unitOfWork = unitOfWork;
            _contentRepository = contentRepository;
            _contentTypeRepository = contentTypeRepository;
            _rbacService = rbacService;
        }

        public async Task<CreateContentResponseDto> CreateContentAsync(CreateContentRequestDto requestDto)
        {
            if (requestDto.TypeId == Guid.Empty || !await _contentRepository.ContentTypeExistsAsync(requestDto.TypeId))
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.TypeNotFound
                };
            }

            if (requestDto.OwnerId == Guid.Empty)
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.OwnerRequired
                };
            }

            var departmentId = await _contentRepository.GetDepartmentIdByTypeIdAsync(requestDto.TypeId);
            var canCreateInDepartment = departmentId.HasValue && await _rbacService.CanAccessDepartmentAsync(
                requestDto.OwnerId,
                ContentCreatePermissionCode,
                departmentId.Value
            );

            if (!canCreateInDepartment)
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.PermissionDenied
                };
            }

            if (requestDto.FieldValues == null || !requestDto.FieldValues.Any())
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.FieldValueRequired
                };
            }

            var fieldDefinitions = await _contentRepository.GetFieldDefinitionsByTypeIdAsync(requestDto.TypeId);
            var fieldDefinitionMap = fieldDefinitions.ToDictionary(field => field.FieldId);

            var providedFieldIds = requestDto.FieldValues
                .Select(fieldValue => fieldValue.FieldId)
                .ToHashSet();

            if (providedFieldIds.Any(fieldId => !fieldDefinitionMap.ContainsKey(fieldId)))
            {
                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.FieldNotFound
                };
            }

            foreach (var fieldDefinition in fieldDefinitions.Where(field => field.IsRequired))
            {
                var providedFieldValue = requestDto.FieldValues
                    .FirstOrDefault(fieldValue => fieldValue.FieldId == fieldDefinition.FieldId);

                if (providedFieldValue == null || string.IsNullOrWhiteSpace(providedFieldValue.FieldValue))
                {
                    return new CreateContentResponseDto
                    {
                        Result = CreateContentResult.RequiredFieldMissing
                    };
                }
            }

            _unitOfWork.BeginTransaction();

            try
            {
                var contentId = await _contentRepository.CreateContentAsync(
                    requestDto.TypeId,
                    requestDto.OwnerId,
                    DefaultContentStatus
                );

                var revisionId = await _contentRepository.CreateContentRevisionAsync(contentId, 1);

                foreach (var fieldValue in requestDto.FieldValues)
                {
                    await _contentRepository.CreateContentFieldValueAsync(
                        revisionId,
                        fieldValue.FieldId,
                        fieldValue.FieldValue?.Trim()
                    );
                }

                _unitOfWork.Commit();

                return new CreateContentResponseDto
                {
                    Result = CreateContentResult.Success,
                    ContentId = contentId,
                    RevisionId = revisionId
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<List<GetContentsResponseDto>> ListContentsAsync(Guid typeId, Guid accountId)
        {
            if (typeId == Guid.Empty || !await _contentRepository.ContentTypeExistsAsync(typeId))
            {
                return [];
            }

            var departmentId = await _contentRepository.GetDepartmentIdByTypeIdAsync(typeId);
            var canViewDepartment = departmentId.HasValue && await _rbacService.CanAccessDepartmentAsync(
                accountId,
                ContentViewPermissionCode,
                departmentId.Value
            );

            if (!canViewDepartment)
            {
                return [];
            }

            var rows = await _contentRepository.ListContentsAsync(typeId);

            return rows
                .GroupBy(
                    row => new
                    {
                        row.ContentId,
                        row.RevisionId,
                        row.Version,
                        row.OwnerUsername,
                        row.Status,
                        row.CreatedAt
                    }
                )
                .Select(group => new GetContentsResponseDto
                {
                    ContentId = group.Key.ContentId,
                    RevisionId = group.Key.RevisionId,
                    Version = group.Key.Version,
                    OwnerUsername = group.Key.OwnerUsername,
                    Status = group.Key.Status,
                    CreatedAt = group.Key.CreatedAt,
                    FieldValues = group
                        .OrderBy(x => x.SortOrder)
                        .ThenBy(x => x.FieldCode)
                        .Select(x => new GetContentFieldValueDto
                        {
                            FieldId = x.FieldId,
                            FieldName = GetFieldDisplayName(x.FieldCode),
                            FieldType = x.FieldType,
                            FieldValue = x.FieldValue
                        })
                        .ToList()
                })
                .OrderByDescending(x => x.CreatedAt)
                .ToList();
        }

        public async Task<UpdateContentResponseDto> UpdateContentAsync(UpdateContentRequestDto requestDto, Guid accountId)
        {
            if (requestDto.ContentId == Guid.Empty || !await _contentRepository.ContentExistsAsync(requestDto.ContentId))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.ContentNotFound
                };
            }

            if (requestDto.RevisionId == Guid.Empty || !await _contentRepository.ContentRevisionExistsAsync(requestDto.RevisionId))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.RevisionNotFound
                };
            }

            if (!await _contentRepository.ContentRevisionBelongsToContentAsync(requestDto.ContentId, requestDto.RevisionId))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.RevisionNotFound
                };
            }

            var departmentId = await _contentRepository.GetDepartmentIdByContentIdAsync(requestDto.ContentId);
            var canEditInDepartment = departmentId.HasValue && await _rbacService.CanAccessDepartmentAsync(
                accountId,
                ContentEditPermissionCode,
                departmentId.Value
            );

            if (!canEditInDepartment)
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.PermissionDenied
                };
            }

            if (requestDto.FieldValues == null || !requestDto.FieldValues.Any())
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.FieldValueRequired
                };
            }

            var typeId = await _contentRepository.GetTypeIdByRevisionIdAsync(requestDto.RevisionId);
            if (!typeId.HasValue)
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.RevisionNotFound
                };
            }

            var fieldDefinitions = await _contentRepository.GetFieldDefinitionsByTypeIdAsync(typeId.Value);
            var fieldDefinitionMap = fieldDefinitions.ToDictionary(field => field.FieldId);

            var providedFieldIds = requestDto.FieldValues
                .Select(fieldValue => fieldValue.FieldId)
                .ToHashSet();

            if (providedFieldIds.Any(fieldId => !fieldDefinitionMap.ContainsKey(fieldId)))
            {
                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.FieldNotFound
                };
            }

            foreach (var fieldDefinition in fieldDefinitions.Where(field => field.IsRequired))
            {
                var providedFieldValue = requestDto.FieldValues
                    .FirstOrDefault(fieldValue => fieldValue.FieldId == fieldDefinition.FieldId);

                if (providedFieldValue == null || string.IsNullOrWhiteSpace(providedFieldValue.FieldValue))
                {
                    return new UpdateContentResponseDto
                    {
                        Result = UpdateContentResult.RequiredFieldMissing
                    };
                }
            }

            _unitOfWork.BeginTransaction();

            try
            {
                foreach (var fieldValue in requestDto.FieldValues)
                {
                    await _contentRepository.UpdateContentFieldValueAsync(
                        requestDto.RevisionId,
                        fieldValue.FieldId,
                        fieldValue.FieldValue?.Trim()
                    );
                }

                _unitOfWork.Commit();

                return new UpdateContentResponseDto
                {
                    Result = UpdateContentResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<DeleteContentResponseDto> DeleteContentAsync(DeleteContentRequestDto requestDto, Guid accountId)
        {
            if (requestDto.ContentId == Guid.Empty || !await _contentRepository.ContentExistsAsync(requestDto.ContentId))
            {
                return new DeleteContentResponseDto
                {
                    Result = DeleteContentResult.ContentNotFound
                };
            }

            var departmentId = await _contentRepository.GetDepartmentIdByContentIdAsync(requestDto.ContentId);
            var canDeleteInDepartment = departmentId.HasValue && await _rbacService.CanAccessDepartmentAsync(
                accountId,
                ContentDeletePermissionCode,
                departmentId.Value
            );

            if (!canDeleteInDepartment)
            {
                return new DeleteContentResponseDto
                {
                    Result = DeleteContentResult.PermissionDenied
                };
            }

            _unitOfWork.BeginTransaction();

            try
            {
                await _contentRepository.DeleteContentFieldValuesByContentIdAsync(requestDto.ContentId);
                await _contentRepository.DeleteContentRevisionsByContentIdAsync(requestDto.ContentId);
                await _contentRepository.DeleteContentAsync(requestDto.ContentId);

                _unitOfWork.Commit();

                return new DeleteContentResponseDto
                {
                    Result = DeleteContentResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<List<ContentTypeLookupDto>> ListContentTypeLookupsAsync(ListContentTypeLookupsRequestDto req, Guid accountId)
        {
            if (req.DepartmentId == Guid.Empty)
            {
                return [];
            }

            var canAccessDepartment = await _rbacService.CanAccessDepartmentAsync(
                accountId,
                ContentViewPermissionCode,
                req.DepartmentId
            );

            if (!canAccessDepartment)
            {
                return [];
            }

            var rows = await _contentTypeRepository.ListContentTypeLookupsAsync(req.DepartmentId);

            return rows
                .Select(x => new ContentTypeLookupDto
                {
                    TypeId = x.TypeId,
                    TypeName = x.TypeName
                })
                .ToList();
        }

        public async Task<List<GetContentFieldsResponseDto>> ListContentFieldsAsync(Guid typeId, Guid accountId)
        {
            if (typeId == Guid.Empty)
            {
                return [];
            }

            var departmentId = await _contentTypeRepository.GetDepartmentIdByTypeIdAsync(typeId);
            if (!departmentId.HasValue)
            {
                return [];
            }

            var canAccessDepartment = await _rbacService.CanAccessDepartmentAsync(
                accountId,
                ContentViewPermissionCode,
                departmentId.Value
            );

            if (!canAccessDepartment)
            {
                return [];
            }

            var rows = await _contentTypeRepository.ListContentFieldsAsync(typeId);

            return rows
                .Select(x => new GetContentFieldsResponseDto
                {
                    FieldId = x.FieldId,
                    FieldName = GetFieldDisplayName(x.FieldCode),
                    FieldCode = x.FieldCode,
                    FieldType = x.FieldType,
                    IsRequired = x.IsRequired
                })
                .ToList();
        }

        public async Task<CreateContentTypeResponseDto> CreateContentTypeAsync(CreateContentTypeRequestDto req)
        {
            if (req.DepartmentId == Guid.Empty)
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.DepartmentRequired
                };
            }

            var typeName = req.TypeName?.Trim();
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.TypeNameRequired
                };
            }

            if (await _contentTypeRepository.TypeNameExistsAsync(req.DepartmentId, typeName))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.TypeNameDuplicated
                };
            }

            if (req.Fields == null || !req.Fields.Any())
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldRequired
                };
            }

            var normalizedFields = req.Fields
                .Select(x => new CreateContentTypeFieldDto
                {
                    FieldName = x.FieldName?.Trim() ?? string.Empty,
                    FieldType = x.FieldType?.Trim().ToLowerInvariant() ?? string.Empty,
                    IsRequired = x.IsRequired
                })
                .ToList();

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldName)))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldNameRequired
                };
            }

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldType)))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldTypeRequired
                };
            }

            var duplicatedFieldNames = normalizedFields
                .GroupBy(x => x.FieldName, StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1);

            if (duplicatedFieldNames)
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldNameDuplicated
                };
            }

            if (normalizedFields.Any(x => !AllowedFieldTypes.Contains(x.FieldType)))
            {
                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.FieldTypeInvalid
                };
            }

            _unitOfWork.BeginTransaction();

            try
            {
                var typeId = await _contentTypeRepository.CreateContentTypeAsync(
                    req.DepartmentId,
                    typeName,
                    GenerateTypeCode()
                );

                for (var i = 0; i < normalizedFields.Count; i++)
                {
                    var field = normalizedFields[i];
                    var fieldId = await _contentTypeRepository.CreateContentFieldAsync(
                        GenerateFieldCode(field.FieldName),
                        field.FieldType,
                        field.IsRequired
                    );

                    await _contentTypeRepository.CreateContentTypeFieldAsync(
                        typeId,
                        fieldId,
                        i
                    );
                }

                _unitOfWork.Commit();

                return new CreateContentTypeResponseDto
                {
                    Result = CreateContentTypeResult.Success,
                    TypeId = typeId,
                    TypeName = typeName
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<UpdateContentTypeResponseDto> UpdateContentTypeAsync(UpdateContentTypeRequestDto req)
        {
            if (req.TypeId == Guid.Empty || !await _contentTypeRepository.TypeExistsAsync(req.TypeId))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNotFound
                };
            }

            var typeName = req.TypeName?.Trim();
            if (string.IsNullOrWhiteSpace(typeName))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNameRequired
                };
            }

            var departmentId = await _contentTypeRepository.GetDepartmentIdByTypeIdAsync(req.TypeId);
            if (!departmentId.HasValue)
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNotFound
                };
            }

            if (req.Fields == null || !req.Fields.Any())
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldRequired
                };
            }

            var normalizedFields = req.Fields
                .Select(x => new UpdateContentTypeFieldDto
                {
                    FieldId = x.FieldId,
                    FieldName = x.FieldName?.Trim() ?? string.Empty,
                    FieldType = x.FieldType?.Trim().ToLowerInvariant() ?? string.Empty,
                    IsRequired = x.IsRequired
                })
                .ToList();

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldName)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldNameRequired
                };
            }

            if (normalizedFields.Any(x => string.IsNullOrWhiteSpace(x.FieldType)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldTypeRequired
                };
            }

            if (normalizedFields
                .GroupBy(x => x.FieldName, StringComparer.OrdinalIgnoreCase)
                .Any(g => g.Count() > 1))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldNameDuplicated
                };
            }

            if (normalizedFields.Any(x => !AllowedFieldTypes.Contains(x.FieldType)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldTypeInvalid
                };
            }

            var currentFields = (await _contentTypeRepository.ListContentFieldsAsync(req.TypeId)).ToList();
            var currentFieldIds = currentFields.Select(x => x.FieldId).ToHashSet();
            var requestedExistingFieldIds = normalizedFields
                .Where(x => x.FieldId.HasValue)
                .Select(x => x.FieldId!.Value)
                .ToList();

            if (requestedExistingFieldIds.Any(x => !currentFieldIds.Contains(x)))
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.FieldNotFound
                };
            }

            var removedFieldIds = currentFields
                .Where(x => !requestedExistingFieldIds.Contains(x.FieldId))
                .Select(x => x.FieldId)
                .ToList();

            foreach (var fieldId in removedFieldIds)
            {
                if (await _contentTypeRepository.ContentFieldValueExistsAsync(fieldId))
                {
                    return new UpdateContentTypeResponseDto
                    {
                        Result = UpdateContentTypeResult.FieldInUse
                    };
                }
            }

            foreach (var field in normalizedFields.Where(x => x.FieldId.HasValue))
            {
                var currentField = currentFields.First(x => x.FieldId == field.FieldId!.Value);

                if (!string.Equals(currentField.FieldType, field.FieldType, StringComparison.OrdinalIgnoreCase)
                    && await _contentTypeRepository.ContentFieldValueExistsAsync(currentField.FieldId))
                {
                    return new UpdateContentTypeResponseDto
                    {
                        Result = UpdateContentTypeResult.FieldInUse
                    };
                }
            }

            var duplicatedTypeId = await _contentTypeRepository.GetTypeIdByDepartmentIdAndTypeNameAsync(
                departmentId.Value,
                typeName
            );

            if (duplicatedTypeId.HasValue && duplicatedTypeId.Value != req.TypeId)
            {
                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.TypeNameDuplicated
                };
            }

            _unitOfWork.BeginTransaction();

            try
            {
                await _contentTypeRepository.UpdateContentTypeAsync(req.TypeId, typeName);

                for (var i = 0; i < normalizedFields.Count; i++)
                {
                    var field = normalizedFields[i];

                    if (field.FieldId.HasValue)
                    {
                        await _contentTypeRepository.UpdateContentFieldAsync(
                            field.FieldId.Value,
                            GenerateFieldCode(field.FieldName),
                            field.FieldType,
                            field.IsRequired
                        );

                        await _contentTypeRepository.UpdateContentTypeFieldSortOrderAsync(
                            req.TypeId,
                            field.FieldId.Value,
                            i
                        );

                        continue;
                    }

                    var newFieldId = await _contentTypeRepository.CreateContentFieldAsync(
                        GenerateFieldCode(field.FieldName),
                        field.FieldType,
                        field.IsRequired
                    );

                    await _contentTypeRepository.CreateContentTypeFieldAsync(
                        req.TypeId,
                        newFieldId,
                        i
                    );
                }

                foreach (var fieldId in removedFieldIds)
                {
                    await _contentTypeRepository.DeleteContentTypeFieldAsync(req.TypeId, fieldId);
                    await _contentTypeRepository.DeleteContentFieldAsync(fieldId);
                }

                _unitOfWork.Commit();

                return new UpdateContentTypeResponseDto
                {
                    Result = UpdateContentTypeResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<DeleteContentTypeResponseDto> DeleteContentTypeAsync(DeleteContentTypeRequestDto req)
        {
            if (req.TypeId == Guid.Empty || !await _contentTypeRepository.TypeExistsAsync(req.TypeId))
            {
                return new DeleteContentTypeResponseDto
                {
                    Result = DeleteContentTypeResult.TypeNotFound
                };
            }

            if (await _contentTypeRepository.ContentExistsByTypeIdAsync(req.TypeId))
            {
                return new DeleteContentTypeResponseDto
                {
                    Result = DeleteContentTypeResult.ContentTypeInUse
                };
            }

            var fields = (await _contentTypeRepository.ListContentFieldsAsync(req.TypeId)).ToList();

            foreach (var field in fields)
            {
                if (await _contentTypeRepository.ContentFieldValueExistsAsync(field.FieldId))
                {
                    return new DeleteContentTypeResponseDto
                    {
                        Result = DeleteContentTypeResult.FieldInUse
                    };
                }
            }

            _unitOfWork.BeginTransaction();

            try
            {
                foreach (var field in fields)
                {
                    await _contentTypeRepository.DeleteContentTypeFieldAsync(req.TypeId, field.FieldId);
                    await _contentTypeRepository.DeleteContentFieldAsync(field.FieldId);
                }

                await _contentTypeRepository.DeleteContentTypeAsync(req.TypeId);

                _unitOfWork.Commit();

                return new DeleteContentTypeResponseDto
                {
                    Result = DeleteContentTypeResult.Success
                };
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        private static string GenerateTypeCode()
        {
            return $"CT-{DateTime.UtcNow:yyyyMMddHHmmssfff}-{Guid.NewGuid().ToString("N")[..8]}";
        }

        private static string GenerateFieldCode(string fieldName)
        {
            return $"{fieldName}__{Guid.NewGuid().ToString("N")[..8]}";
        }

        private static string GetFieldDisplayName(string fieldCode)
        {
            const string separator = "__";
            var separatorIndex = fieldCode.LastIndexOf(separator, StringComparison.Ordinal);

            return separatorIndex > 0
                ? fieldCode[..separatorIndex]
                : fieldCode;
        }
    }
}
