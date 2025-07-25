using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SchoolMetaService
    {
        private readonly IRepository<SchoolMeta> repository;

        public SchoolMetaService(IRepository<SchoolMeta> repository)
        {
            this.repository = repository;
        }

        public IQueryable<SchoolMeta> GetSchoolMetaBySchoolId(int schoolId)
        {
            return repository.Select().Where(x => x.SchoolID.Equals(schoolId));
        }

        public void AddOrUpdateSchoolMeta(int schoolId, string metaName, string metaValue)
        {
            var schoolMetaData = repository.Select().FirstOrDefault(x => x.SchoolID == schoolId && x.Name.ToLower() == metaName.ToLower());
            if (schoolMetaData != null)
            {
                if (string.IsNullOrWhiteSpace(metaValue))
                {
                    repository.Delete(schoolMetaData);
                    return;
                }
                schoolMetaData.Data = metaValue;
                repository.Save(schoolMetaData);
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(metaValue))
                {
                    schoolMetaData = new SchoolMeta()
                    {
                        SchoolID = schoolId,
                        Name = metaName,
                        Data = metaValue
                    };
                    repository.Save(schoolMetaData);
                }
            }
        }
    }
}
