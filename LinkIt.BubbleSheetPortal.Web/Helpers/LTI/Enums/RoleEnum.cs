using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils;
using Newtonsoft.Json;
using System;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums
{
    [JsonConverter(typeof(RoleConverter))]
    public enum RoleEnum
    {
        Unknown = 0,

        #region Core Context Roles

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Administrator")]
        [Uri("Administrator")]
        ContextAdministrator,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#ContentDeveloper")]
        [Uri("ContentDeveloper")]
        ContextContentDeveloper,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Instructor")]
        [Uri("Instructor")]
        ContextInstructor,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Learner")]
        [Uri("Learner")]
        ContextLearner,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Mentor")]
        [Uri("Mentor")]
        ContextMentor,

        #endregion

        #region Non-Core Context Roles

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Manager")]
        [Uri("Manager")]
        ContextManager,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Member")]
        [Uri("Member")]
        ContextMember,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/membership#Officer")]
        [Uri("Officer")]
        ContextOfficer,

        #endregion

        #region Core Institution Roles

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Administrator")]
        InstitutionAdministrator,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Guest")]
        InstitutionGuest,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#None")]
        InstitutionNone,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Other")]
        InstitutionOther,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Staff")]
        InstitutionStaff,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Student")]
        InstitutionStudent,

        #endregion

        #region Non-Core Institution Roles

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Alumni")]
        InstitutionAlumni,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Faculty")]
        InstitutionFaculty,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Instructor")]
        InstitutionInstructor,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Learner")]
        InstitutionLearner,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Member")]
        InstitutionMember,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Mentor")]
        InstitutionMentor,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#Observer")]
        InstitutionObserver,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/institution/person#ProspectiveStudent")]
        InstitutionProspectiveStudent,

        #endregion

        #region Core System Roles

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#Administrator")]
        SystemAdministrator,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#None")]
        SystemNone,

        #endregion

        #region Non-Core System Roles

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#AccountAdmin")]
        SystemAccountAdmin,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#Creator")]
        SystemCreator,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#SysAdmin")]
        SystemSysAdmin,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#SysSupport")]
        SystemSysSupport,

        [Uri("http://purl.imsglobal.org/vocab/lis/v2/system/person#User")]
        SystemUser,

        #endregion
    }
}
