using System;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSFormSection2Repository : ITLDSFormSection2Repository
    {
        private readonly Table<TLDSFormSection2Entity> _table;
        private readonly TLDSContextDataContext _tldsContext;
        public TLDSFormSection2Repository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            _table = _tldsContext.GetTable<TLDSFormSection2Entity>();
        }

        public void Delete(TLDSFormSection2 item)
        {
            throw new NotImplementedException();
        }

        public void Save(TLDSFormSection2 item)
        {
            var entity = _table.FirstOrDefault(x => x.TLDSFormSection2ID == item.TLDSFormSection2ID);
            if (entity == null)
            {
                entity = _table.FirstOrDefault(x => x.TLDSProfileLinkID == item.TLDSProfileLinkID);

                if (entity == null)
                {
                    entity = new TLDSFormSection2Entity
                    {
                        TLDSProfileLinkID = item.TLDSProfileLinkID,
                        GuardianName = item.GuardianName,
                        Relationship = item.Relationship,
                        Favourite = item.Favourite,
                        Strengths = item.Strengths,
                        Weaknesses = item.Weaknesses,
                        Interested = item.Interested,
                        Expected = item.Expected,
                        Drawing = item.Drawing,
                    };
                    _table.InsertOnSubmit(entity);
                    _table.Context.SubmitChanges();
                    return;
                }
            }

            entity.GuardianName = item.GuardianName;
            entity.Relationship = item.Relationship;
            entity.Favourite = item.Favourite;
            entity.Strengths = item.Strengths;
            entity.Weaknesses = item.Weaknesses;
            entity.Interested = item.Interested;
            entity.Expected = item.Expected;
            entity.Drawing = item.Drawing;

            _table.Context.SubmitChanges();
        }

        public IQueryable<TLDSFormSection2> Select()
        {
            var query = _table.Select(x => new TLDSFormSection2
            {
                TLDSFormSection2ID = x.TLDSFormSection2ID,
                TLDSProfileLinkID = x.TLDSProfileLinkID,
                GuardianName = x.GuardianName,
                Relationship = x.Relationship,
                Favourite = x.Favourite,
                Strengths = x.Strengths,
                Weaknesses = x.Weaknesses,
                Interested = x.Interested,
                Expected = x.Expected,
                Drawing = x.Drawing,
                IsSubmitted = x.IsSubmitted
            });

            return query;
        }

        public bool UpdateTldsForm(TLDSFormSection2 item)
        {
            var tldsSection2 = _table.FirstOrDefault(x => x.TLDSFormSection2ID == item.TLDSFormSection2ID);
            if (tldsSection2 != null)
            {
                tldsSection2.GuardianName = item.GuardianName;
                tldsSection2.Relationship = item.Relationship;
                tldsSection2.Favourite = item.Favourite;
                tldsSection2.Strengths = item.Strengths;
                tldsSection2.Weaknesses = item.Weaknesses;
                tldsSection2.Interested = item.Interested;
                tldsSection2.Expected = item.Expected;
                tldsSection2.Drawing = item.Drawing;
                _table.Context.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool SubmittedForm(TLDSFormSection2 item)
        {
            var tldsSection2 = _table.FirstOrDefault(x => x.TLDSProfileLinkID == item.TLDSProfileLinkID);
            if (tldsSection2 == null)
            {
                tldsSection2 = new TLDSFormSection2Entity();
                _table.InsertOnSubmit(tldsSection2);

                tldsSection2.TLDSProfileLinkID = item.TLDSProfileLinkID;
                tldsSection2.GuardianName = item.GuardianName;
                tldsSection2.Relationship = item.Relationship;
                tldsSection2.Favourite = item.Favourite;
                tldsSection2.Strengths = item.Strengths;
                tldsSection2.Weaknesses = item.Weaknesses;
                tldsSection2.Interested = item.Interested;
                tldsSection2.Expected = item.Expected;
                tldsSection2.Drawing = item.Drawing;
                tldsSection2.IsSubmitted = true;
                _table.Context.SubmitChanges();
                return true;
            }
            else
            {
                tldsSection2.GuardianName = item.GuardianName;
                tldsSection2.Relationship = item.Relationship;
                tldsSection2.Favourite = item.Favourite;
                tldsSection2.Strengths = item.Strengths;
                tldsSection2.Weaknesses = item.Weaknesses;
                tldsSection2.Interested = item.Interested;
                tldsSection2.Expected = item.Expected;
                tldsSection2.Drawing = item.Drawing;
                tldsSection2.IsSubmitted = true;
                _table.Context.SubmitChanges();
                return true;
            }
        }
    }
}
