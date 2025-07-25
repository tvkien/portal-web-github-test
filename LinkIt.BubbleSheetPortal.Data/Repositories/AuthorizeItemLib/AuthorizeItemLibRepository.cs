using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.AuthorizeItemLib
{
    public class AuthorizeItemLibRepository : IAuthorizeItemLibRepository
    {
        private readonly Table<XLIFunction> table;
        private readonly AssessmentDataContext _assessmentDataContext;
        public AuthorizeItemLibRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = AssessmentDataContext.Get(connectionString).GetTable<XLIFunction>();
            _assessmentDataContext = AssessmentDataContext.Get(connectionString);
        }

        public IQueryable<XLIFunctionMap> Select()
        {
            return table.Select(x =>
            new XLIFunctionMap()
            {
                XLIFunctionID = x.XLIFunctionID,
                Name = x.Name,
                Description = x.Description

            });
        }

        public IQueryable<LibraryTypeResult> GetAuthorizeItemLibBylibraryTypes(AuthorizeUser authorizeUser)
        {
            int districtId = authorizeUser.DistrictID;
            int userId = authorizeUser.UserID;
            int roleId = authorizeUser.RoleID;
            string libraryType = string.Join(",", authorizeUser.LibraryType);

            return _assessmentDataContext.GetAuthorizeItemLibByLibraryTypes(districtId, userId, roleId, libraryType)
                .Select(o => new LibraryTypeResult()
                {
                    LibraryType = o.LibraryType,
                    IsAuthorize = o.IsAuthorize ?? false

                }).AsQueryable();
        }
        public IQueryable<LibraryTypeResult> GetAuthorizeItemLibBylibraryTypes(int userId, int roleId, int districtId, List<string> XLIFunctions)
        {
            string libraryType = string.Join(",", XLIFunctions);

            return _assessmentDataContext.GetAuthorizeItemLibByLibraryTypes(districtId, userId, roleId, libraryType)
                .Select(o => new LibraryTypeResult()
                {
                    LibraryType = o.LibraryType,
                    IsAuthorize = o.IsAuthorize ?? false

                }).AsQueryable();
        }

    }
}
