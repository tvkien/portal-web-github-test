using LinkIt.BubbleSheetPortal.Models.DataFileUpload;
using System.Linq;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class DataFileUploadPassageService
    {
        private readonly IReadOnlyRepository<DataFileUploadPassage> _datafileUpdatePassageRepository;
        public DataFileUploadPassageService(IReadOnlyRepository<DataFileUploadPassage> datafileUpdatePassageRepository)
        {
            _datafileUpdatePassageRepository = datafileUpdatePassageRepository;
        }
        public DataFileUploadPassage GetDataFilePassageByUploadLogId(int dataFileUploadPassageId)
        {
            return _datafileUpdatePassageRepository.Select().Where(x => x.DataFileUploadPassageID == dataFileUploadPassageId).FirstOrDefault();
        }  
    }
}
