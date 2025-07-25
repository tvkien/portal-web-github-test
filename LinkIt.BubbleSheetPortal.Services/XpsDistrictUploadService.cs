using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Models.Old.XpsDistrictUpload;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XpsDistrictUploadService
    {
        private readonly IInsertSelect<XpsDistrictUpload> repository;
        public XpsDistrictUploadService(IInsertSelect<XpsDistrictUpload> repository)
        {
            this.repository = repository;
        }

        public IEnumerable<XpsDistrictUpload> GetXpsDistrictUploadByUploadTypeId(int districtID, int uploadTypeId)
        {
            return repository.Select().Where(x => x.DistrictID == districtID && x.UploadTypeID == uploadTypeId);
        }
    }
}
