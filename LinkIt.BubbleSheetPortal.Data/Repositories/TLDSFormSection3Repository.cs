using System;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSFormSection3Repository : ITLDSFormSection3Repository
    {
        private readonly Table<TLDSFormSection3Entity> _table;
        private readonly TLDSContextDataContext _tldsContext;
        public TLDSFormSection3Repository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            _table = _tldsContext.GetTable<TLDSFormSection3Entity>();
        }

        public void Delete(TLDSFormSection3 item)
        {
            throw new NotImplementedException();
        }

        public void Save(TLDSFormSection3 item)
        {
            var entity = _table.FirstOrDefault(x => x.TLDSFormSection3ID == item.TLDSFormSection3ID);
            if (entity == null)
            {
                entity = _table.FirstOrDefault(x => x.TLDSProfileLinkID == item.TLDSProfileLinkID);

                if (entity == null)
                {
                    entity = new TLDSFormSection3Entity
                    {
                        TLDSProfileLinkID = item.TLDSProfileLinkID,
                        GuardianName = item.GuardianName,
                        Relationship = item.Relationship,
                        PreferredLanguage = item.PreferredLanguage,
                        IsAborigial = item.IsAborigial,
                        HaveSiblingInSchool = item.HaveSiblingInSchool,
                        NameAndGradeOfSibling = item.NameAndGradeOfSibling,
                        Wishes = item.Wishes,
                        InformationSchool = item.InformationSchool,
                        HelpInformation = item.HelpInformation,
                        Interested = item.Interested,
                        ConditionImprovement = item.ConditionImprovement,
                        OtherInformation = item.OtherInformation,
                    };
                    _table.InsertOnSubmit(entity);
                    _table.Context.SubmitChanges();
                    return;
                }
            }

            entity.GuardianName = item.GuardianName;
            entity.Relationship = item.Relationship;
            entity.PreferredLanguage = item.PreferredLanguage;
            entity.IsAborigial = item.IsAborigial;
            entity.HaveSiblingInSchool = item.HaveSiblingInSchool;
            entity.NameAndGradeOfSibling = item.NameAndGradeOfSibling;
            entity.Wishes = item.Wishes;
            entity.InformationSchool = item.InformationSchool;
            entity.HelpInformation = item.HelpInformation;
            entity.Interested = item.Interested;
            entity.ConditionImprovement = item.ConditionImprovement;
            entity.OtherInformation = item.OtherInformation;
            _table.Context.SubmitChanges();
        }

        public IQueryable<TLDSFormSection3> Select()
        {
            var query = _table.Select(x => new TLDSFormSection3
            {
                TLDSFormSection3ID = x.TLDSFormSection3ID,
                TLDSProfileLinkID = x.TLDSProfileLinkID,
                GuardianName = x.GuardianName,
                Relationship = x.Relationship,
                PreferredLanguage = x.PreferredLanguage,
                IsAborigial = x.IsAborigial,
                HaveSiblingInSchool = x.HaveSiblingInSchool,
                NameAndGradeOfSibling = x.NameAndGradeOfSibling,
                Wishes = x.Wishes,
                InformationSchool = x.InformationSchool,
                HelpInformation = x.HelpInformation,
                Interested = x.Interested,
                ConditionImprovement = x.ConditionImprovement,
                OtherInformation = x.OtherInformation,
                IsSubmitted = x.IsSubmitted
            });

            return query;
        }

        public bool UpdateTldsForm(TLDSFormSection3 item)
        {
            var tldsSection3 = _table.FirstOrDefault(x => x.TLDSFormSection3ID == item.TLDSFormSection3ID);
            if (tldsSection3 != null)
            {
                tldsSection3.GuardianName = item.GuardianName;
                tldsSection3.Relationship = item.Relationship;
                tldsSection3.PreferredLanguage = item.PreferredLanguage;
                tldsSection3.IsAborigial = item.IsAborigial;
                tldsSection3.HaveSiblingInSchool = item.HaveSiblingInSchool;
                tldsSection3.NameAndGradeOfSibling = item.NameAndGradeOfSibling;
                tldsSection3.Wishes = item.Wishes;
                tldsSection3.InformationSchool = item.InformationSchool;
                tldsSection3.HelpInformation = item.HelpInformation;
                tldsSection3.Interested = item.Interested;
                tldsSection3.ConditionImprovement = item.ConditionImprovement;
                tldsSection3.OtherInformation = item.OtherInformation;
                _table.Context.SubmitChanges();
                return true;
            }
            return false;
        }

        public bool SubmittedForm(TLDSFormSection3 item)
        {
            var tldsSection3 = _table.FirstOrDefault(x => x.TLDSProfileLinkID == item.TLDSProfileLinkID);
            if (tldsSection3 == null)
            {
                tldsSection3 = new TLDSFormSection3Entity();
                _table.InsertOnSubmit(tldsSection3);

                tldsSection3.TLDSProfileLinkID = item.TLDSProfileLinkID;
                tldsSection3.GuardianName = item.GuardianName;
                tldsSection3.Relationship = item.Relationship;
                tldsSection3.PreferredLanguage = item.PreferredLanguage;
                tldsSection3.IsAborigial = item.IsAborigial;
                tldsSection3.HaveSiblingInSchool = item.HaveSiblingInSchool;
                tldsSection3.NameAndGradeOfSibling = item.NameAndGradeOfSibling;
                tldsSection3.Wishes = item.Wishes;
                tldsSection3.InformationSchool = item.InformationSchool;
                tldsSection3.HelpInformation = item.HelpInformation;
                tldsSection3.Interested = item.Interested;
                tldsSection3.ConditionImprovement = item.ConditionImprovement;
                tldsSection3.OtherInformation = item.OtherInformation;
                tldsSection3.IsSubmitted = true;
                _table.Context.SubmitChanges();
                return true;
            }
            else
            {
                tldsSection3.GuardianName = item.GuardianName;
                tldsSection3.Relationship = item.Relationship;
                tldsSection3.PreferredLanguage = item.PreferredLanguage;
                tldsSection3.IsAborigial = item.IsAborigial;
                tldsSection3.HaveSiblingInSchool = item.HaveSiblingInSchool;
                tldsSection3.NameAndGradeOfSibling = item.NameAndGradeOfSibling;
                tldsSection3.Wishes = item.Wishes;
                tldsSection3.InformationSchool = item.InformationSchool;
                tldsSection3.HelpInformation = item.HelpInformation;
                tldsSection3.Interested = item.Interested;
                tldsSection3.ConditionImprovement = item.ConditionImprovement;
                tldsSection3.OtherInformation = item.OtherInformation;
                tldsSection3.IsSubmitted = true;
                _table.Context.SubmitChanges();
                return true;
            }
        }
    }
}
