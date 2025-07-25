using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Models.AuthorizeItemLib;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Common.Enum;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Data.Repositories.AuthorizeItemLib;

namespace LinkIt.BubbleSheetPortal.Services.AuthorizeItemLibServices
{
    public class AuthorizeItemLibService : IAuthorizeItemLibService
    {
        #region Fields
        private readonly IReadOnlyRepository<XLIFunctionMap> _xliFunctionRepository;
        private readonly IReadOnlyRepository<XLIFunctionDistrictSchoolMap> _xLIFunctionDistrictSchoolRepository;
        private readonly IReadOnlyRepository<XLIFunctionSchoolMap> _xLIFunctionSchoolRepository;
        private readonly IReadOnlyRepository<XLIFunctionSchoolRoleMap> _xLIFunctionSchoolRoleRepository;
        private readonly IReadOnlyRepository<School> _schoolRepository;
        private readonly IReadOnlyRepository<UserSchool> _userSchoolRepository;
        private readonly IAuthorizeItemLibRepository _authorizeItemLibRepository;
        private readonly IQTIItemRepository _qtiItemQtiItemRepository;
        #endregion

        #region Ctor
        public AuthorizeItemLibService(IReadOnlyRepository<XLIFunctionMap> xliFunctionRepository,
        IReadOnlyRepository<XLIFunctionDistrictSchoolMap> xLIFunctionDistrictSchoolRepository,
        IReadOnlyRepository<XLIFunctionSchoolMap> xLIFunctionSchoolRepository,
        IReadOnlyRepository<XLIFunctionSchoolRoleMap> xLIFunctionSchoolRoleRepository,
        IReadOnlyRepository<School> schoolRepository,
        IReadOnlyRepository<UserSchool> userSchoolRepository,
        IAuthorizeItemLibRepository _authorizeItemLibRepository,
        IQTIItemRepository qtiItemQtiItemRepository)
        {
            this._xliFunctionRepository = xliFunctionRepository;
            this._xLIFunctionDistrictSchoolRepository = xLIFunctionDistrictSchoolRepository;
            this._xLIFunctionSchoolRepository = xLIFunctionSchoolRepository;
            this._xLIFunctionSchoolRoleRepository = xLIFunctionSchoolRoleRepository;
            this._schoolRepository = schoolRepository;
            this._userSchoolRepository = userSchoolRepository;
            this._authorizeItemLibRepository = _authorizeItemLibRepository;
            this._qtiItemQtiItemRepository = qtiItemQtiItemRepository;
        }

        #endregion
        public List<LibraryTypeResult> GetAuthorizeItemLibBylibraryTypes(AuthorizeUser authorizeUser)
        {
            //if (authorizeUser.RoleID == (int)Permissions.DistrictAdmin
            //    || authorizeUser.RoleID == (int)Permissions.Publisher)
            //    return _xliFunctionRepository.Select().Select(l =>
            //    new LibraryTypeResult
            //    {
            //        LibraryType = l.Name,
            //        IsAuthorize = true
            //    }
            //    ).ToList();

           return _authorizeItemLibRepository.GetAuthorizeItemLibBylibraryTypes(authorizeUser).ToList();
        }            

       
        private IQueryable<int> GetSchoolIdsByUserId(AuthorizeUser authorizeUser)
        {
            if (authorizeUser.RoleID == (int)Permissions.DistrictAdmin
                || authorizeUser.RoleID == (int)Permissions.Publisher)
            {
                var query = _schoolRepository.Select().Where(o => o.DistrictId == authorizeUser.DistrictID);
                return query.Select(o => o.Id);

            }
            else if (authorizeUser.RoleID == (int)Permissions.Teacher || authorizeUser.RoleID == (int)Permissions.SchoolAdmin)
            {
                var query = _userSchoolRepository.Select().Where(o => o.UserId == authorizeUser.UserID);

                return query.Select(o => o.SchoolId.Value);
            }

            return null;
        }

        public XliFunctionAccess GetXliFunctionAccess(int userId, int roleId, int districtId)
        {
            var xliFunctions = new List<string>
            {
                Constanst.XLIFunction.DistrictLibrary,
                Constanst.XLIFunction.CerticaLibrary,
                Constanst.XLIFunction.ProgressLibrary
            };
            var authorizedLibraries = _authorizeItemLibRepository.GetAuthorizeItemLibBylibraryTypes(userId, roleId, districtId, xliFunctions).ToList();
            var result = new XliFunctionAccess();
            if (
               authorizedLibraries.Any(
                   x =>
                       x.IsAuthorize == true &&
                       x.LibraryType.ToUpper() == Constanst.XLIFunction.DistrictLibrary.ToUpper()))
            {
                result.DistrictLibraryAccessible = true;
            }

            if (
              authorizedLibraries.Any(
                  x =>
                      x.IsAuthorize == true &&
                      x.LibraryType.ToUpper() == Constanst.XLIFunction.CerticaLibrary.ToUpper()))
            {
                if (_qtiItemQtiItemRepository.CheckAccessQTI3p(userId, districtId, Qti3pLicensesEnum.Certica))
                {
                    result.CerticaLibraryAccessible = true;
                }
                
            }


            if (
              authorizedLibraries.Any(
                  x =>
                      x.IsAuthorize == true &&
                      x.LibraryType.ToUpper() == Constanst.XLIFunction.ProgressLibrary.ToUpper()))
            {
                if (_qtiItemQtiItemRepository.CheckAccessQTI3p(userId, districtId, Qti3pLicensesEnum.Progress))
                {
                    result.ProgressLibraryAccessible = true;
                }
            }

            return result;
        }

    }
}
