using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DTOs.Commons;
using LinkIt.BubbleSheetPortal.Models.DTOs.ManageParent;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ParentService
    {
        private readonly IRepository<ParentDto> _parentRepository;
        private readonly IRepository<ParentMetaDto> _parentMetaRepository;

        public ParentService(IRepository<ParentDto> parentRepository, IRepository<ParentMetaDto> parentMetaRepository)
        {
            _parentRepository = parentRepository;
            _parentMetaRepository = parentMetaRepository;
        }

        public List<ParentMetaDto> GetParentMetasByParentId(int parentId)
        {
            if (parentId > 0)
            {
                return _parentMetaRepository.Select().Where(o => o.ParentID == parentId).ToList();
            }
            return new List<ParentMetaDto>();
        }

        public ParentDto SaveParent(ParentDto parent)
        {
            if (parent != null)
            {
                _parentRepository.Save(parent);
                return parent;
            }
            return null;
        }
        public void SaveParentMetas(int parentId, List<MetaDataKeyValueDto> parentMetaDatas)
        {
            if (parentId > 0 && parentMetaDatas != null && parentMetaDatas.Count > 0)
            {
                foreach (var parentMeta in parentMetaDatas)
                {
                    if (!string.IsNullOrEmpty(parentMeta.Name))
                    {
                        var currentParentMeta = _parentMetaRepository.Select().FirstOrDefault(o => o.ParentID == parentId && o.Name.Equals(parentMeta.Name));
                        if (currentParentMeta != null)
                        {
                            currentParentMeta.Data = parentMeta.Value;
                            _parentMetaRepository.Save(currentParentMeta);

                        }
                        else if (!string.IsNullOrEmpty(parentMeta.Value))
                        {
                            _parentMetaRepository.Save(new ParentMetaDto()
                            {
                                ParentID = parentId,
                                Name = parentMeta.Name,
                                Data = parentMeta.Value
                            });
                        }
                    }
                }
            }
        }

        public ParentDto GetParentViaUserId(int userId)
        {
            return _parentRepository.Select().FirstOrDefault(o => o.UserID == userId);
        }

    }
}
