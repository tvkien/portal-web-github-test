using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using System.Collections.Generic;
using VirtualSection = LinkIt.BubbleSheetPortal.Models.VirtualSection;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualSectionRepository : IVirtualSectionRepository
    {
        private readonly TestDataContext _testDataContext;

        private readonly Table<VirtualSectionEntity> _tableVirtualSectionEntity;

        public VirtualSectionRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _testDataContext = TestDataContext.Get(connectionString);
            _tableVirtualSectionEntity = _testDataContext.GetTable<VirtualSectionEntity>();
        }

        public IQueryable<VirtualSection> Select()
        {
            return _tableVirtualSectionEntity.Select(
                x => new VirtualSection
                         {
                             AudioRef = x.AudioRef,
                             ConversionSetId = x.ConversionSetID,
                             Instruction = x.Instruction,
                             MediaReference = x.MediaReference,
                             MediaSource = x.MediaSource,
                             Order = x.Order,
                             Title = x.Title,
                             VideoRef = x.VideoRef,
                             VirtualSectionId = x.VirtualSectionID,
                             VirtualTestId = x.VirtualTestID,
                             SubjectId = x.SubjectID,
                             Mode = x.Mode
                         }
                );
        }

        public IEnumerable<VirtualSection> GetPartialRetakeSections(int virtualTestId, string studentIds, string guid)
        {
            var result = _testDataContext.GetSectionsForPartialRetake(virtualTestId, studentIds, guid);
            return result.Select(x => new VirtualSection
            {
                VirtualSectionId = x.VirtualSectionID,
                VirtualTestId = x.VirtualTestID,
                SubjectId = x.SubjectID,
                VideoRef = x.VideoRef,
                AudioRef = x.AudioRef,
                ConversionSetId = x.ConversionSetID,
                Instruction = x.Instruction,
                MediaReference = x.MediaReference,
                MediaSource = x.MediaSource,
                Order = x.Order,
                Title = x.Title,
                Mode = x.Mode
            }).ToArray();
        }

        public void Save(VirtualSection item)
        {
            var entity = _tableVirtualSectionEntity.FirstOrDefault(x => x.VirtualSectionID.Equals(item.VirtualSectionId));

            if (entity == null)
            {
                entity = new VirtualSectionEntity();
                _tableVirtualSectionEntity.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            _tableVirtualSectionEntity.Context.SubmitChanges();
            item.VirtualSectionId = entity.VirtualSectionID;
        }

        public void Delete(VirtualSection item)
        {
            var entity = _tableVirtualSectionEntity.FirstOrDefault(x => x.VirtualSectionID.Equals(item.VirtualSectionId));

            if (entity != null)
            {
                _tableVirtualSectionEntity.DeleteOnSubmit(entity);
                _tableVirtualSectionEntity.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualSectionEntity entity, VirtualSection item)
        {
            entity.AudioRef = item.AudioRef;
            entity.ConversionSetID = item.ConversionSetId;
            entity.Instruction = item.Instruction;
            entity.MediaReference = item.MediaReference;
            entity.MediaSource = item.MediaSource;
            entity.Order = item.Order;
            entity.Title = item.Title;
            entity.VideoRef = item.VideoRef;
            entity.VirtualSectionID = item.VirtualSectionId;
            entity.VirtualTestID = item.VirtualTestId;
            entity.SubjectID = item.SubjectId;
            entity.Mode = item.Mode;
        }
    }
}
