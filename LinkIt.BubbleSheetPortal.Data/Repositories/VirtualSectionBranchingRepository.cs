using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;
using LinkIt.BubbleSheetPortal.Models.Interfaces;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualSectionBranchingRepository : IRepository<VirtualSectionBranching>
    {
        private readonly SectionBranchingDataContext _context;

        private readonly Table<VirtualSectionBranchingEntity> _table;

        public VirtualSectionBranchingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _context = SectionBranchingDataContext.Get(connectionString);
            _table = _context.GetTable<VirtualSectionBranchingEntity>();
        }

        public IQueryable<VirtualSectionBranching> Select()
        {
            return _table.Select(
                x => new VirtualSectionBranching
                {
                    VirtualSectionBranchingID = x.VirtualSectionBranchingID,
                    VirtualTestID = x.VirtualTestID,
                    TestletPath = x.TestletPath,
                    LowScore = x.LowScore,
                    HighScore = x.HighScore,
                    TargetVirtualSectionID = x.TargetVirtualSectionID,
                    CreatedUserID = x.CreatedUserID,
                    CreatedDateTime = x.CreatedDateTime,
                    IsBranchBySectionScore = x.IsBranchBySectionScore
                }
                );
        }

        public void Save(VirtualSectionBranching item)
        {
            var entity = _table.FirstOrDefault(x => x.VirtualSectionBranchingID.Equals(item.VirtualSectionBranchingID));

            if (entity == null)
            {
                entity = new VirtualSectionBranchingEntity();
                _table.InsertOnSubmit(entity);
            }

            MapModelToEntity(entity, item);
            _table.Context.SubmitChanges();
            item.VirtualSectionBranchingID = entity.VirtualSectionBranchingID;
        }

        public void Delete(VirtualSectionBranching item)
        {
            var entity = _table.FirstOrDefault(x => x.VirtualSectionBranchingID.Equals(item.VirtualSectionBranchingID));

            if (entity != null)
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

        private void MapModelToEntity(VirtualSectionBranchingEntity entity, VirtualSectionBranching item)
        {
            entity.VirtualSectionBranchingID = item.VirtualSectionBranchingID;
            //entity.VirtualSectionID = item.VirtualSectionID;
            entity.VirtualTestID = item.VirtualTestID;
            entity.TestletPath = item.TestletPath;
            entity.LowScore = item.LowScore;
            entity.HighScore = item.HighScore;
            entity.TargetVirtualSectionID = item.TargetVirtualSectionID;
            //entity.IsDefault = item.IsDefault;
            entity.CreatedUserID = item.CreatedUserID;
            entity.CreatedDateTime = item.CreatedDateTime;
            entity.IsBranchBySectionScore = item.IsBranchBySectionScore;
        }
    }
}
