using System;
using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualTestFileService
    {
        private readonly IRepository<VirtualTestFile> repository;

        public VirtualTestFileService(IRepository<VirtualTestFile> repository)
        {
            this.repository = repository;
        }

        public IQueryable<VirtualTestFile> GetVirtualTestFiles()
        {
            return repository.Select();
        }

        public VirtualTestFile GetFirstOrDefaultByVirtualTest(int virtualTestId)
        {
            return repository.Select().FirstOrDefault(x => x.VirtualTestId == virtualTestId);
        }

        public VirtualTestFile GetByFileKey(string fileKey)
        {
            return repository.Select().FirstOrDefault(x => x.FileKey == fileKey);
        }

        public bool DeleteVirtualTestFile(int testId)
        {
            var vVirtualTestFile = repository.Select().FirstOrDefault(o => o.VirtualTestId == testId);
            if (vVirtualTestFile != null)
            {
                repository.Delete(vVirtualTestFile);
                return true;
            }
            return false;
        }

        public bool Save(VirtualTestFile item)
        {
            try
            {
                repository.Save(item);
                return true;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
    }
}