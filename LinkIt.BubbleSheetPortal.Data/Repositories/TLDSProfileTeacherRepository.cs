using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.DTOs.TLDS;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public class TLDSProfileTeacherRepository : ITLDSProfileTeacherRepository
    {
        private readonly Table<TLDSProfileTeacherEntity> table;
        private readonly TLDSContextDataContext _tldsContext;
        public TLDSProfileTeacherRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _tldsContext = TLDSContextDataContext.Get(connectionString);
            table = _tldsContext.GetTable<TLDSProfileTeacherEntity>();
        }

        public void Delete(TLDSProfileTeacherDTO item)
        {
            throw new System.NotImplementedException();
        }

        public void Save(TLDSProfileTeacherDTO item)
        {
            var entity = table.FirstOrDefault(x => x.TLDSProfileTeacherID.Equals(item.TLDSProfileTeacherID));
            if (entity == null)
            {
                entity = new TLDSProfileTeacherEntity();
                table.InsertOnSubmit(entity);
            }
            entity.TLDSUserMetaID = item.TLDSUserMetaID;
            entity.TLDSLevelQualificationID = item.TLDSLevelQualificationID;
            entity.EducatorName = item.EducatorName;
            entity.Position = item.Position;
            table.Context.SubmitChanges();
            item.TLDSProfileTeacherID = entity.TLDSProfileTeacherID;
        }

        public IQueryable<TLDSProfileTeacherDTO> Select()
        {
            return table.Select(x => new TLDSProfileTeacherDTO
            {
                TLDSProfileTeacherID = x.TLDSProfileTeacherID,
                TLDSUserMetaID = x.TLDSUserMetaID,
                TLDSLevelQualificationID = x.TLDSLevelQualificationID,
                EducatorName = x.EducatorName,
                Position = x.Position
            });
        }

        public List<TLDSProfileTeacherDTO> GetAllByUserMetaID(int tldsUserMetaID)
        {
            return _tldsContext.TLDSProfileTeacher_GetAll(tldsUserMetaID)
                               .Select(x => new TLDSProfileTeacherDTO
                               {
                                   TLDSProfileTeacherID = x.TLDSProfileTeacherID,
                                   TLDSUserMetaID = x.TLDSUserMetaID,
                                   TLDSLevelQualificationID = x.TLDSLevelQualificationID,
                                   EducatorName = x.EducatorName,
                                   TLDSLevelQualificationName = x.Name,
                                   Position = x.Position
                               })
                               .ToList();
        }

        public bool Remove(int teacherProfileID) 
        {
            var teacherProfile = table.FirstOrDefault(m => m.TLDSProfileTeacherID == teacherProfileID);

            if (teacherProfile != null)
            {
                table.DeleteOnSubmit(teacherProfile);
                table.Context.SubmitChanges();
                return true;
            }

            return false;
        }
    }
}
