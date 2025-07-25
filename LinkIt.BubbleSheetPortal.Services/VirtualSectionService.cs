using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Models.TestMaker.S3VirtualTest;
using System;
using LinkIt.BubbleSheetPortal.Models.DTOs.VirtualTest;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.DTOs.RetakeAssignment;
using LinkIt.BubbleSheetPortal.Data.Repositories;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class VirtualSectionService
    {
        private readonly IVirtualSectionRepository virtualSectionRepository;
        private readonly IRepository<VirtualSectionBranching> virtualSectionBranchingRepository;

        public VirtualSectionService(IVirtualSectionRepository virtualSectionRepository,
            IRepository<VirtualSectionBranching> virtualSectionBranchingRepository)
        {
            this.virtualSectionRepository = virtualSectionRepository;
            this.virtualSectionBranchingRepository = virtualSectionBranchingRepository;
        }

        public List<VirtualSection> GetVirtualSectionByVirtualTest(int virtualTestId, GetTestPreferencePartialRetakeModel partialRetakeInfo = null)
        {
            if (partialRetakeInfo != null)
            {
                return virtualSectionRepository.GetPartialRetakeSections(partialRetakeInfo.VirtualTestID, partialRetakeInfo.StudentIDs, partialRetakeInfo.GUID).ToList();
            }

            return virtualSectionRepository.Select().Where(en => en.VirtualTestId == virtualTestId).ToList();
        }

        public VirtualSection GetVirtualSectionById(int virtualSectionId)
        {
            return virtualSectionRepository.Select().Where(en => en.VirtualSectionId == virtualSectionId).FirstOrDefault();
        }

        public void Save(VirtualSection item)
        {
            virtualSectionRepository.Save(item);
        }

        public void Delete(VirtualSection item)
        {
            virtualSectionRepository.Delete(item);
        }

        public IQueryable<VirtualSection> Select()
        {
            return virtualSectionRepository.Select();
        }

        public List<VirtualSectionBranching> GetVirtualSectionBranchingByVirtualtestId(int virtualtestTestId)
        {
            if (virtualtestTestId > 0)
            {
                var lst = virtualSectionBranchingRepository.Select()
                .Where(o => o.VirtualTestID == virtualtestTestId).ToList();
                return lst;
            }

            return new List<VirtualSectionBranching>();
        }
        public VirtualSectionBranching InsertVirtualSectionBranching(VirtualSectionBranching obj)
        {
            if (obj != null)
            {
                virtualSectionBranchingRepository.Save(obj);
                return obj;
            }
            return null;
        }
        public List<VirtualSectionBranching> InsertListVirtualSectionBranching(List<VirtualSectionBranching> lstObj)
        {
            var listSectionBranching = virtualSectionBranchingRepository.Select()
                    .Where(o => o.VirtualTestID == lstObj[0].VirtualTestID && o.TestletPath.Equals(lstObj[0].TestletPath))
                    .ToList();
            if (listSectionBranching != null && listSectionBranching.Count > 0)
            {
                for (int i = 0; i < listSectionBranching.Count; i++)
                {
                    virtualSectionBranchingRepository.Delete(new VirtualSectionBranching()
                    {
                        VirtualSectionBranchingID = listSectionBranching[i].VirtualSectionBranchingID
                    });
                }
            }
            if (lstObj != null && lstObj.Count > 0)
            {
                foreach (var item in lstObj)
                {
                    virtualSectionBranchingRepository.Save(item);
                }
                return lstObj;
            }
            return null;
        }

        public void DeleteVirtualSectionBranching(int id)
        {
            virtualSectionBranchingRepository.Delete(new VirtualSectionBranching()
            {
                VirtualSectionBranchingID = id
            });
        }
        public void DeleteFullPathVirtualSectionBranching(int virtualSectionBranchingId)
        {
            var obj = virtualSectionBranchingRepository.Select().FirstOrDefault(o => o.VirtualSectionBranchingID == virtualSectionBranchingId);
            if (obj != null)
            {
                var listSectionBranching = virtualSectionBranchingRepository.Select()
                    .Where(o => o.VirtualTestID == obj.VirtualTestID && o.TestletPath.Equals(obj.TestletPath))
                    .ToList();
                if (listSectionBranching != null && listSectionBranching.Count > 0)
                {
                    for (int i = 0; i < listSectionBranching.Count; i++)
                    {
                        virtualSectionBranchingRepository.Delete(new VirtualSectionBranching()
                        {
                            VirtualSectionBranchingID = listSectionBranching[i].VirtualSectionBranchingID
                        });
                    }
                }
            }
        }

        public List<VirtualSectionBranching> GetSectionBranchingBySectionPathId(int virtualSectionBranchingId)
        {
            var obj = virtualSectionBranchingRepository.Select().FirstOrDefault(o => o.VirtualSectionBranchingID == virtualSectionBranchingId);
            if (obj != null)
            {
                var listSectionBranching = virtualSectionBranchingRepository.Select()
                    .Where(o => o.VirtualTestID == obj.VirtualTestID && o.TestletPath.Equals(obj.TestletPath))
                    .ToList();
                return listSectionBranching;
            }
            return new List<VirtualSectionBranching>();
        }

        public bool IsBranchBySectionScore(int virtualTestId)
        {
            return virtualSectionBranchingRepository.Select()
                .Where(c => c.VirtualTestID == virtualTestId && true == c.IsBranchBySectionScore)
                .Any();
        }

        public BaseResponseModel<bool> UpdateBranchingMethod(UpdateBranchingMethodDto updateBranchingMethod)
        {
            var paths = GetVirtualSectionBranchingByVirtualtestId(updateBranchingMethod.VirtualTestId);
            if (paths?.Count > 0)
            {
                paths.ForEach(path =>
                {
                    path.IsBranchBySectionScore = updateBranchingMethod.IsBranchBySectionScore;
                    virtualSectionBranchingRepository.Save(path);
                });
            }
            return BaseResponseModel<bool>.InstanceSuccess(true);
        }
    }
}
