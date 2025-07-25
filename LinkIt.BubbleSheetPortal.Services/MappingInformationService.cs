using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class MappingInformationService
    {
        private readonly IRepository<MappingInformation> repository;

        public MappingInformationService(IRepository<MappingInformation> repository)
        {
            this.repository = repository;
        }

        public MappingInformation GetMappingById(int mapId)
        {
            return repository.Select().FirstOrDefault(m => m.MapID == mapId);
        }

        public IQueryable<MappingInformation> GetMappingList()
        {
            return repository.Select();
        }

        public void DeleteMapping(MappingInformation deletedMapping)
        {
            repository.Delete(deletedMapping);
        }

        public int InsertNewMapping(MappingInformation newMapping)
        {
            repository.Save(newMapping);

            return newMapping.MapID;
        }

        public bool IsMappingNameExisted(string mappingName)
        {
            return repository.Select().Any(m => m.Name == mappingName);
        }

        public MappingInformation GetFirstMappingByUserId(int userId)
        {
            return repository.Select().FirstOrDefault(m => m.UserID == userId);
        }

        public void DeleteAllMappingBeUserId(int userId)
        {
            List<MappingInformation> mappingList = repository.Select().Where(m => m.UserID == userId).ToList();
            foreach (MappingInformation mapping in mappingList)
            {
                repository.Delete(mapping);
            }
        }

        public void SaveMapping(MappingInformation mapping)
        {
            repository.Save(mapping);
        }
    }
}