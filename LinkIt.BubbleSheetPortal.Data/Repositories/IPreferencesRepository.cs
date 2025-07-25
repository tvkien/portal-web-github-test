using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Data.Repositories
{
    public interface IPreferencesRepository : IRepository<Preferences>
    {
        void InsertMultipleRecord(List<Preferences> items);
    }
}
