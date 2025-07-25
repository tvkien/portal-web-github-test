using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Globalization;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IBubbleSheetClassViewAutoSaveRepository: IRepository<BubbleSheetClassViewAutoSave>
    {
        void BubbleSheetDeleteAllAutoSaveData(string ticket, int classId);
    }
}