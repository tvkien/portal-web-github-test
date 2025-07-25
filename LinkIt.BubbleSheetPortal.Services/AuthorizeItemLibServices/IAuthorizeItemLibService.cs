using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;

namespace LinkIt.BubbleSheetPortal.Services.AuthorizeItemLibServices
{
    public interface IAuthorizeItemLibService
    {
        List<LibraryTypeResult> GetAuthorizeItemLibBylibraryTypes(AuthorizeUser authorizeUser);

    }
}
