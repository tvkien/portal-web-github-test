using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.SGO;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class SGOMilestoneService 
    {
        private readonly IRepository<SGOMilestone> repository;

        public SGOMilestoneService(IRepository<SGOMilestone> repository)
        {
            this.repository = repository;
        }

        public void CreateMilestone(int sgoId, int userId)
        {
            if (sgoId > 0 && userId > 0)
            {
                var obj = new SGOMilestone()
                {
                    MilestoneDate = DateTime.UtcNow,
                    SGOID = sgoId,
                    SGOStatusID = (int)SGOStatusType.Draft,
                    UserID = userId
                };
                repository.Save(obj);
            }
        }
        public void CreateMilestoneWithStatus(int sgoId, int userId, int status)
        {
            if (sgoId > 0 && userId > 0 && status > 0)
            {
                var obj = new SGOMilestone()
                {
                    MilestoneDate = DateTime.UtcNow,
                    SGOID = sgoId,
                    SGOStatusID = status,
                    UserID = userId
                };
                repository.Save(obj);
            }
        }
    }
}
