using AutoMapper;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.DataLocker;
using LinkIt.BubbleSheetPortal.Models.DTOs.DistrictDecode;
using LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport;
using LinkIt.BubbleSheetPortal.Models.SSO;
using LinkIt.BubbleSheetPortal.Models.StudentPreferenceDTOs;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.TestMaker;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public static class AutoMapperConfigurator
    {
        public static void Initialize()
        {
            Mapper.CreateMap<User, IUserCookieData>()
                .IgnoreAllNonExisting();

            Mapper.CreateMap<User, EditUserViewModel>()
                .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id));

            Mapper.CreateMap<EditUserViewModel, User>()
                .ForMember(x => x.Id, x => x.Ignore());

            Mapper.CreateMap<User, EditParentViewModel>()
                .ForMember(x => x.UserId, x => x.MapFrom(y => y.Id));

            Mapper.CreateMap<EditParentViewModel, User>()
                .ForMember(x => x.Id, x => x.Ignore())
                .ForMember(x => x.RoleId, x => x.Ignore());


            Mapper.CreateMap<StudentPreferencesEntity, StudentPreferenceDto>();
            Mapper.CreateMap<StudentPreferenceDto, StudentPreferencesEntity>();
            Mapper.CreateMap<StudentPreferenceDetailEntity, StudentPreferenceDetailDto>();
            Mapper.CreateMap<StudentPreferenceDetailDto, StudentPreferenceDetailEntity>();


            Mapper.CreateMap<SSOUserMapping, SSOUserMappingEntity>();
            #region Navigator report

            Mapper.CreateMap<NavigatorReportLogDto, NavigatorReportLogEntity>();
            Mapper.CreateMap<NavigatorReportLogEntity, NavigatorReportLogDto>();

            Mapper.CreateMap<NavigatorReportDTO, NavigatorReportEntity>();
            Mapper.CreateMap<NavigatorReportEntity, NavigatorReportDTO>().ForMember(c => c.ReceivedDate, x => x.MapFrom(c => c.CreatedTime));


            Mapper.CreateMap<NavigatorReportUploadFileResponseDto, NavigatorReportGetUploadedReportsInfoResult>();
            Mapper.CreateMap<NavigatorReportGetUploadedReportsInfoResult, NavigatorReportUploadFileResponseDto>();

            Mapper.CreateMap<NavigatorUserDto, UserManage>();
            Mapper.CreateMap<UserManage, NavigatorUserDto>();

            Mapper.CreateMap<NavigatorUserDto, NavigatorReportGetAssociateUserByReportIdsResult>();
            Mapper.CreateMap<NavigatorReportGetAssociateUserByReportIdsResult, NavigatorUserDto>();

            Mapper.CreateMap<NavigatorReportGetFileFromDBDto, NavigatorReportGetReportFilesResult>();
            Mapper.CreateMap<NavigatorReportGetReportFilesResult, NavigatorReportGetFileFromDBDto>();

            Mapper.CreateMap<NavigatorReportGetFileFromDBDto, NavigatorReportGetReportFilesByClassIdResult>();
            Mapper.CreateMap<NavigatorReportGetReportFilesByClassIdResult, NavigatorReportGetFileFromDBDto>();

            Mapper.CreateMap<NavigatorReportUploadFileFormDataDTO, NavigatorRecordExistResultDto>();
            #endregion

            Mapper.CreateMap<AssessmentArtifactFileTypeGroupDTO, AssessmentArtifactFileTypeGroupViewModel>();
            Mapper.CreateMap<AssessmentArtifactFileTypeGroupDTO, AssessmentArtifactFileTypeGroupEditScoreModel>();
            Mapper.CreateMap<AssessmentArtifactFileTypeGroupDTO, EntryResultArtifactFileTypeGroupViewModel>();            
        }
    }
}
