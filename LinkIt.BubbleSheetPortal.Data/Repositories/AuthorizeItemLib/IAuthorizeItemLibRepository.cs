using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.AuthorizeItemLib
{
    public interface IAuthorizeItemLibRepository : IReadOnlyRepository<XLIFunctionMap>
    {
        IQueryable<LibraryTypeResult> GetAuthorizeItemLibBylibraryTypes(AuthorizeUser authorizeUser);

        IQueryable<LibraryTypeResult> GetAuthorizeItemLibBylibraryTypes(int userId, int roleId, int districtId,
            List<string> XLIFunctions);
    }
}
