using System.Data.Linq;
using System.Linq;
using AutoMapper;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class VirtualQuestionBranchingRepository : IRepository<VirtualQuestionBranching>
    {
        private readonly Table<VirtualQuestionBranchingEntity> _table;

        public VirtualQuestionBranchingRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<VirtualQuestionBranchingEntity>();
            Mapper.CreateMap<VirtualQuestionBranching, VirtualQuestionBranchingEntity>();
        }

        public IQueryable<VirtualQuestionBranching> Select()
        {
            return _table.Select(x => new VirtualQuestionBranching
            {
                VirtualQuestionBranchingID = x.VirtualQuestionBranchingID,
                VirtualQuestionID = x.VirtualQuestionID,
                AnswerChoice = x.AnswerChoice,
                TargetVirtualQuestionID = x.TargetVirtualQuestionID,
                VirtualTestID = x.VirtualTestID,
                Comment = x.Comment
            });
        }

        public void Save(VirtualQuestionBranching item)
        {
            var entity = _table.FirstOrDefault(x => x.VirtualQuestionBranchingID.Equals(item.VirtualQuestionBranchingID));

            if (entity == null)
            {
                entity = new VirtualQuestionBranchingEntity();
                _table.InsertOnSubmit(entity);
            }

            MapObject(item, entity);
            _table.Context.SubmitChanges();
            item.VirtualQuestionBranchingID = entity.VirtualQuestionBranchingID;
        }

        public void Delete(VirtualQuestionBranching item)
        {
            var entity = _table.FirstOrDefault(x => x.VirtualQuestionBranchingID.Equals(item.VirtualQuestionBranchingID));
            if (entity != null)
            {
                _table.DeleteOnSubmit(entity);
                _table.Context.SubmitChanges();
            }
        }

        private void MapObject(VirtualQuestionBranching source, VirtualQuestionBranchingEntity destination)
        {
            destination.VirtualQuestionBranchingID = source.VirtualQuestionBranchingID;
            destination.VirtualQuestionID = source.VirtualQuestionID;
            destination.AnswerChoice = source.AnswerChoice;
            destination.TargetVirtualQuestionID = source.TargetVirtualQuestionID;
            destination.VirtualTestID = source.VirtualTestID;
            destination.Comment = source.Comment;
        }
    }
}
