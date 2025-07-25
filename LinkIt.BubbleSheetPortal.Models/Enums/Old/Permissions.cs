namespace LinkIt.BubbleSheetPortal.Models
{
    public enum Permissions
    {
        API = 1,
        Teacher = 2,
        DistrictAdmin = 3,
        System = 4,
        Publisher = 5,
        StateAdministrator = 7,
        SchoolAdmin = 8,
        DistrictProspect = 12,
        LinkItAdmin = 13,
        SchoolProspect = 14,
        TeacherProspect = 15,
        PartnerProspect = 16,
        Director = 20,
        RegionalAdministrator = 23,
        InstructionalSpeciliast = 24,
        Advisor = 25,
        Parent = 26,
        NetworkAdmin = 27,
        Student = 28
    }

    public enum TestPreferenceLevel
    {
        Enterprise = 1,
        District = 2,
        School = 3,
        TestDesign = 4,
        User = 5,
        TestAssignment = 6,
        SurveyEnterprise = 7,
        SurveyDistrict = 8
    }

    public enum DataLockerPreferencesLevel
    {
        Enterprise = 1,
        District = 2,
        School = 3,
        Form = 4,
        Publishing = 5
    }

    public enum StudentPreferenceLevel
    {
        Enterprise = 1,
        District = 2,
        School = 3,
        TestAssignment = 4
    }

    public enum LockBankStatus
    {
        Open = 1,
        Restricted = 2
    }

    public enum APIAccountType
    {
        District = 1,
        User = 2
    }

    public enum UploadFileType
    {
        Rubric = 1
    }

    public enum DisplayAssignmentType
    {
        ClassAssignment = 1,
        StudentAssignment,
        StudentGroupAssingnment
    }

    public enum ExportTemplates
    {
        AssessmentItem = 1,
        AssessmentAchievedDetail,
        AssessmentItemResponse,
        ASSMNT_ITEMR_ACADEMIC_STDS,
        ASSMNT_SUBTEST_ACADEMIC_STDS,
        ASSESSMENT_FACT,
        ASSESSMENT_RESPONSE,
        ASSESSMENT_ACC_MOD_FACT,
        USER,
        TEST,
        QUESTION,
        TESTRESULT,
        POINTSEARNED,
        STUDENTRESPONSE,
        CLASSTESTASSIGNMENT,
        ROSTER
    }

    public enum S3PortalLinkType
    {
        ExportExtractTestResult = 100
    }

    public enum ClassUserLoeType
    {
        Primary = 1,
        Delegate,
        StudentTeachers,
        DepartmentHeads,
        CoTeachers,
        Secondary,
        Other
    }

    public enum SGOStatusType
    {
        Draft = 1,
        PreparationSubmittedForApproval,
        PreparationApproved,
        PreparationDenied,
        Cancelled,
        EvaluationSubmittedForApproval,
        SGOApproved,
        SGODenied,
        TeacherAcknowledged
    }

    public enum SGOStudentFilterType
    {
        Gender = 1,
        Race,
        Program,
        DistrictTerm,
        Class,
        State,
        District
    }

    public enum SGOStepEnum
    {
        SGOHome = 1,
        StudentPopulation,
        DataPoints,
        PreparednessGroups,
        ScoringPlan,
        AdminReview,
        ProgressMonitoring,
        FinalSignoff
    }

    public enum SGOPermissionEnum
    {
        NotAvalible = 1,
        ReadOnly,
        FullUpdate,
        MinorUpdate
    }

    public enum SecurityPreferenceLevel
    {
        Enterprise = 1,
        District = 2,
        User = 3
    }
}
