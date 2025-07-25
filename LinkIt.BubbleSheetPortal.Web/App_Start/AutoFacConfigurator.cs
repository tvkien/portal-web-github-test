using Autofac;
using Autofac.Features.ResolveAnything;
using Autofac.Integration.Mvc;
using Envoc.Core.Shared.Data;
using FluentValidation;
using LinkIt.BubbleService.Configuration;
using LinkIt.BubbleSheetPortal.Data;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Data.Repositories.ClassPrinting;
using LinkIt.BubbleSheetPortal.Data.Repositories.Helper;
using LinkIt.BubbleSheetPortal.Data.Repositories.Isolating;
using LinkIt.BubbleSheetPortal.Data.Repositories.ManageParent;
using LinkIt.BubbleSheetPortal.Data.Repositories.MasterData;
using LinkIt.BubbleSheetPortal.Data.Repositories.MultipleTestResults;
using LinkIt.BubbleSheetPortal.Data.Repositories.NavigatorReport;
using LinkIt.BubbleSheetPortal.Data.Repositories.StudentPreferences;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover;
using LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemoverLog;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Repositories;
using LinkIt.BubbleSheetPortal.DynamoIsolating.Services;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Repositories;
using LinkIt.BubbleSheetPortal.InteractiveRubric.Services;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.Interfaces;
using LinkIt.BubbleSheetPortal.Network;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using LinkIt.BubbleSheetPortal.Services.CommonServices;
using LinkIt.BubbleSheetPortal.Services.ManageParent;
using LinkIt.BubbleSheetPortal.Services.Modules;
using LinkIt.BubbleSheetPortal.Services.TeacherReviewer;
using LinkIt.BubbleSheetPortal.SimpleQueueService.Services;
using LinkIt.BubbleSheetPortal.Validators;
using LinkIt.BubbleSheetPortal.Validators.Modules;
using LinkIt.BubbleSheetPortal.VaultProvider;
using LinkIt.BubbleSheetPortal.Web.Adapter;
using LinkIt.BubbleSheetPortal.Web.Controllers.Parameters;
using LinkIt.BubbleSheetPortal.Web.Helpers;
using LinkIt.BubbleSheetPortal.Web.Security;
using LinkIt.BubbleSheetPortal.Web.ViewModels;
using LinkIt.BubbleSheetPortal.Web.ViewModels.AuthorGroup;
using LinkIt.BubbleSheetPortal.Web.ViewModels.GenericSheets;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;
using System.Configuration;
using System.Reflection;
using System.Web.Mvc;
using LinkIt.BubbleSheetPortal.Services.EDM;
using S3Library;
using Amazon.SQS;
using LinkIt.BubbleSheetPortal.Data.Repositories.PerformanceBandAutomations;
using LinkIt.BubbleSheetPortal.Services.Reporting;
using LinkIt.BubbleSheetPortal.Data.Repositories.SharingGroup.IService;
using LinkIt.BubbleSheetPortal.Data.Repositories.SharingGroup.Service;
using Amazon.CloudWatchLogs;
using LinkIt.BubbleSheetPortal.Services.MfaServices;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using Amazon;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    public class AutofacConfigurator
    {
        private static Assembly DataAssembly
        {
            get { return typeof(UserRepository).Assembly; }
        }

        public static void Initialize()
        {
            var builder = BuildContainer();
            builder = InitializeServiceContainer(builder);
            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
            var container = builder.Build();

            DependencyResolver.SetResolver(new ProfiledDependencyResolver(new AutofacDependencyResolver(container)));
        }

        private static ContainerBuilder InitializeServiceContainer(ContainerBuilder builder)
        {
            var testMode = bool.Parse(ConfigurationManager.AppSettings["TestMode"] ?? "false");
            var container = AutofacConfiguration.Configure(new BubbleServiceConfiguration
            {
                TestMode = testMode,
                AzureConnectionString = string.Empty,
                ErrorApiKey = string.Empty,
                ErrorApiUrl = string.Empty,
                GraderApiKey = string.Empty,
                GraderApiUrl = string.Empty
            }, builder);

            return container;
        }

        public static void InitializeTesting()
        {
            var builder = BuildContainer();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
        }

        private static ContainerBuilder BuildContainer()
        {
            var containerBuilder = new ContainerBuilder();

            containerBuilder.RegisterModule(new AutofacWebTypesModule());


            containerBuilder.RegisterType<ConnectionStringAdapter>()
               .AsImplementedInterfaces().InstancePerHttpRequest();

            RegisterValidators(containerBuilder);
            RegisterRepositories(containerBuilder);
            RegisterIsolatingRepository(containerBuilder);

            containerBuilder.RegisterModule<ServicesModule>();
            containerBuilder.RegisterModule<ValidatorModule>();

            containerBuilder.RegisterType<UniqueTestCodeValidator>().AsImplementedInterfaces().InstancePerHttpRequest();
            containerBuilder.RegisterType<TestCodeGenerator>().AsSelf().InstancePerHttpRequest();
            containerBuilder.RegisterType<AuthenticationCodeGenerator>().AsSelf().InstancePerHttpRequest();

            containerBuilder.RegisterType<AdminControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<UserGroupManagementControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ManageParentControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<PassageEditorControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TestMakerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<MailBoxControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ComposeControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ErrorCorrectionControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<GroupPrintingControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ManageClassesControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<RestrictionAccessRightsControllerParameters>().PropertiesAutowired().AsSelf();

            containerBuilder.RegisterType<TestAssignmentControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<PrintTestControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<PassThroughControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ItemLibraryManagementControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<RubricControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TestExtractControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<LearningLibraryControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ItemBankControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<LearningLibraryAdminControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<QTIItemControllerParameters>().PropertiesAutowired().AsSelf();

            containerBuilder.RegisterType<QTIItemTagControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TestResultTransferControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TestResultsExportControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<RemoveTestResultsControllerParameters>().PropertiesAutowired().AsSelf();

            containerBuilder.RegisterType<CEEControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<RegistrationControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ShowQtiItemControllerParameters>().PropertiesAutowired().AsSelf();

            containerBuilder.RegisterType<ManageTestControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<CreateTestControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<StudentLoginControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<ChytenReportControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<StudentLookupControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<PopulateReportingControllerParameters>().PropertiesAutowired().AsSelf();

            containerBuilder.RegisterType<SGOManageControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<SGOSelectDataPointControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<SGOScoringPlanTargetControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<SGOAuditTrailParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<SGOProgressMonitorControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<SGOReportControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<SGOPrintControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<NotificationMessageControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<DataLockerControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<DataLockerForStudentControllerParameters>().PropertiesAutowired().AsSelf();

            containerBuilder.RegisterType<FormsAuthenticationService>().As<IFormsAuthenticationService>();
            containerBuilder.RegisterType<TeacherReviewerService>().As<ITeacherReviewerService>();
            containerBuilder.RegisterType<RosterUploadService>().As<IRosterUploadService>();
            containerBuilder.RegisterType<HelpResourceService>().As<IHelpResourceService>();
            containerBuilder.RegisterType<ConversionSetService>().As<IConversionSetService>();
            containerBuilder.RegisterType<MfaService>().As<IMfaService>();

            containerBuilder.RegisterType<StudentTestLaunchControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TDLSManageControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TLDSReportControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<AnswerViewerControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<AblesReportControllerParameter>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TLDSDigitalSection23ControllerParameters>().PropertiesAutowired().AsSelf();
            containerBuilder.RegisterType<TestUtilitiesDefineTemplatesControllerParameters>().PropertiesAutowired().AsSelf();



            containerBuilder.RegisterType<ManageParentService>()
           .As<IManageParentService>()
           .InstancePerDependency();

            RegisterDynamoIsolating(containerBuilder);

            RegisterRubricModule(containerBuilder);

            RegisterEDMIntergrationService(containerBuilder);

            containerBuilder.RegisterType<AnswerAttachmentService>()
                .As<IAnswerAttachmentService>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<AnswerAttachmentRepository>()
                .As<IAnswerAttachmentRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<ReportingHttpClient>().As<IReportingHttpClient>()
                .InstancePerHttpRequest();

            containerBuilder.Register<IAmazonCognitoIdentityProvider>(c =>
            {
                return new AmazonCognitoIdentityProviderClient();
            }).SingleInstance();

            containerBuilder.RegisterS3Service();
            containerBuilder.RegisterControllers();

            return containerBuilder;
        }

        private static void RegisterRubricModule(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterType<RubricQuestionCategoryRepository>()
            .As<IRubricQuestionCategoryRepository>()
            .InstancePerHttpRequest();

            containerBuilder.RegisterType<RubricCategoryTierRepository>()
           .As<IRubricCategoryTierRepository>()
           .InstancePerHttpRequest();

            containerBuilder.RegisterType<RubricCategoryTagRepository>()
            .As<IRubricCategoryTagRepository>()
            .InstancePerHttpRequest();

            containerBuilder.RegisterType<RubricTestResultScoreRepository>()
           .As<IRubricTestResultScoreRepository>()
           .InstancePerHttpRequest();

            containerBuilder.RegisterType<RubricTagService>()
      .As<IRubricTagService>()
      .InstancePerHttpRequest();
            containerBuilder.RegisterType<RubricModuleQueryService>()
           .As<IRubricModuleQueryService>()
           .InstancePerHttpRequest();

            containerBuilder.RegisterType<RubricModuleCommandService>()
         .As<IRubricModuleCommandService>()
         .InstancePerHttpRequest();
        }

        private static void RegisterValidators(ContainerBuilder containerBuilder)
        {
            var passwordRegex = ConfigurationManager.AppSettings["PasswordRegex"];
            var passwordMessageRegex = ConfigurationManager.AppSettings["PasswordRequirements"];

            containerBuilder.Register(
                context =>
                    new CreateUserViewModelValidator(context.Resolve<IRepository<User>>(), passwordRegex,
                        passwordMessageRegex))
                .As<IValidator<CreateUserViewModel>>();

            containerBuilder.Register(
                context => new EditUserViewModelValidator(context.Resolve<IRepository<User>>()))
                .As<IValidator<EditUserViewModel>>();

            containerBuilder.Register(
                context =>
                    new AddParentViewModelValidator(context.Resolve<IRepository<User>>(), passwordRegex,
                        passwordMessageRegex))
                .As<IValidator<CreateParentViewModel>>();

            containerBuilder.Register(
                context =>
                    new EditParentViewModelValidator(context.Resolve<IRepository<User>>(), passwordRegex,
                        passwordMessageRegex))
                .As<IValidator<EditParentViewModel>>();

            containerBuilder.Register(
                context =>
                    new AddUserSchoolViewModelValidator(context.Resolve<IRepository<User>>(),
                        context.Resolve<IUserSchoolRepository<UserSchool>>()))
                .As<IValidator<AddUserSchoolViewModel>>();

            containerBuilder.Register(
                context => new AccountInformationViewModelValidator(passwordRegex, passwordMessageRegex))
                .As<IValidator<AccountInformationViewModel>>();

            containerBuilder.Register(
                context => new AssignGenericSheetViewModelValidator())
                .As<IValidator<AssignGenericSheetViewModel>>();

            containerBuilder.Register(
                context => new AssignGenericSheetActSatViewModelValidator())
                .As<IValidator<AssignGenericSheetActSatViewModel>>();

            containerBuilder.Register(
                context => new QtiBankPublishToDistrictViewModelValidator())
                .As<IValidator<QtiBankPublishToDistrictViewModel>>();

            containerBuilder.Register(
                context => new EditPassageViewModelValidator())
                .As<IValidator<EditPassageViewModel>>();

            containerBuilder.Register(
                context => new QtiBankPublishToSchoolViewModelValidator())
                .As<IValidator<QtiBankPublishToSchoolViewModel>>();

            containerBuilder.Register(
                context => new AddAuthorGroupViewModelValidator())
                .As<IValidator<AddAuthorGroupViewModel>>();
            containerBuilder.Register(
                context => new ResourceViewModelValidator())
                .As<IValidator<ResourceViewModel>>();
            containerBuilder.Register(
                context => new ACTReportDataValidator())
                .As<IValidator<ACTReportData>>();
            containerBuilder.Register(
                context => new RegistrationViewModelValidator(passwordRegex, passwordMessageRegex))
                .As<IValidator<RegistrationViewModel>>();
        }

        private static void RegisterRepositories(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAssemblyTypes(DataAssembly)
                .AsClosedTypesOf(typeof(IRepository<>))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterAssemblyTypes(DataAssembly)
                .AsClosedTypesOf(typeof(IUnitOfWorkRepository<>))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterAssemblyTypes(DataAssembly)
                .AsClosedTypesOf(typeof(IReadOnlyRepository<>))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterAssemblyTypes(DataAssembly)
                .AsClosedTypesOf(typeof(IInsertSelect<>))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterAssemblyTypes(DataAssembly)
                .AsClosedTypesOf(typeof(IInsertDeleteRepository<>))
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<BubbleSheetFileRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<UserSchoolRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<UnansweredQuestionsRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<UserSchoolViewRepository>()
                .As<IReadOnlyRepository<UserSchool>>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<BubbleSheetRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<QtiBankRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<EInstructionRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<ClassStudentReadOnlyRepository>()
                .As<IReadOnlyRepository<ClassStudent>>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<ParentConnectRepository>()
                .As<IParentConnectRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<LookupStudentRepository>()
                .As<ILookupStudentRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<CustomAuthorTestRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<DSPDistrictRepository>()
            .As<IDspDistrictRepository>()
            .InstancePerHttpRequest();

            containerBuilder.RegisterType<AnswerAuditRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<AnswerSubAuditRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<QTIOnlineTestSessionAnswerAuditRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<QTIOnlineTestSessionAnswerSubAuditRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<AuthorGroupRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<IQTIGroupRepository>()
              .AsImplementedInterfaces()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<ASPSessionRepository>()
                .AsImplementedInterfaces()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<AuthorGroupUserRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<MasterStandardRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<QTIItemStateStandardRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<ManageTestRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<ImpersonateLogRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<ACTAnswerQuestionRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<ACTReportRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<ChytenReportRepository>()
               .AsImplementedInterfaces()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<StudentProgramRepository>()
               .As<IStudentProgramRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<SGOSelectDataPointRepository>()
               .As<ISGOSelectDataPointRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<TestResultAuditRepository>()
               .As<ITestResultAuditRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<TestResultLogRepository>()
               .As<ITestResultLogRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<TestResultProgramLogRepository>()
               .As<ITestResultProgramLogRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<TestResultScoreLogRepository>()
              .As<ITestResultScoreLogRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<TestResultSubScoreLogRepository>()
               .As<ITestResultSubScoreLogRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<AnswerLogRepository>()
              .As<IAnswerLogRepository>()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<AnswerSubLogRepository>()
               .As<IAnswerSubLogRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<PopulateReportingRepository>()
                .As<IPopulateReportingRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<ProgramRepository>()
                .As<IProgramRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<VulnerabilityRepository>()
                .As<IVulnerabilityRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<AblesReportRepository>()
              .As<IAblesReportRepository>()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<TLDSProfileRepository>()
              .As<ITLDSProfileRepository>()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<HelpResourceRepository>()
              .As<IHelpResourceRepository>().InstancePerHttpRequest();

            containerBuilder.RegisterType<ResultEntryTemplateRepository>()
              .As<IResultEntryTemplateRepository>()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<DataLockerRepository>()
             .As<IDataLockerRepository>()
             .InstancePerHttpRequest();
            containerBuilder.RegisterType<DataLockerForStudentRepository>()
             .As<IDataLockerForStudentRepository>()
             .InstancePerHttpRequest();

            containerBuilder.RegisterType<TestRestrictionModuleRepository>()
             .As<ITestRestrictionModuleRepository>()
             .InstancePerHttpRequest();

            containerBuilder.RegisterType<XLIAreaDistrictModuleRepository>()
            .As<IXLIAreaDistrictModuleRepository>()
            .InstancePerHttpRequest();

            containerBuilder.RegisterType<RosterRequestRepository>()
                .As<IRosterRequestRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<ShortLinkRepository>()
                .As<IShortLinkRepository>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<StudentPreferenceRepository>()
               .As<IStudentPreferenceRepository>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<ClassPrintingGroupRepository>()
               .As<IClassPrintingGroupRepository>()
               .InstancePerHttpRequest();


            containerBuilder.RegisterType<NavigatorReportRepository>()
                 .As<INavigatorReportRepository>()
                 .InstancePerHttpRequest();

            containerBuilder.RegisterType<NavigatorReportDetailRepository>()
                 .As<INavigatorReportDetailRepository>()
                 .InstancePerHttpRequest();

            containerBuilder.RegisterType<NavigatorAttributeRepository>()
          .As<INavigatorAttributeRepository>()
          .InstancePerHttpRequest();

            containerBuilder.RegisterType<NavigatorConfigurationRepository>()
          .As<INavigatorConfigurationRepository>()
          .InstancePerHttpRequest();

            containerBuilder.RegisterType<NavigatorReportLogRepository>()
              .As<INavigatorReportLogRepository>()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<NavigatorReportPublishRepository>()
              .As<INavigatorReportPublishRepository>()
              .InstancePerHttpRequest();

            containerBuilder.RegisterType<BulkHelper>()
               .As<IBulkHelper>()
               .InstancePerHttpRequest();

            containerBuilder.RegisterType<ManageParentRepository>()
              .As<IManageParentRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<DataSetCategoryRepository>()
              .As<IDataSetCategoryRepository>()
              .InstancePerDependency();


            containerBuilder.RegisterType<MultipleTestResultRepository>()
              .As<IMultipleTestResultRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<XLIGroupRepository>()
              .As<IXLIGroupRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<XLIAreaGMRoleRepository>()
              .As<IXLIAreaGMRoleRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<XLIAreaGroupModuleRepository>()
              .As<IXLIAreaGroupModuleRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<XLIAreaGroupRepository>()
              .As<IXLIAreaGroupRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<PerformanceBandAutomationRepository>()
              .As<IPerformanceBandAutomationRepository>()
              .InstancePerDependency();

            containerBuilder.RegisterType<PerformanceBandVirtualTestRepository>()
              .As<IPerformanceBandVirtualTestRepository>()
              .InstancePerDependency();
            
            containerBuilder.RegisterType<BubbleSheetPageRepository>()
                .As<IBubbleSheetPageRepository>()
                .InstancePerDependency();

            containerBuilder.RegisterType<SharingGroupRepository>()
                .As<ISharingGroupRepository>()
                .InstancePerDependency();

            containerBuilder.RegisterType<UserRepository>()
                .As<IUserRepository>()
                .InstancePerHttpRequest();
        }

        private static void RegisterIsolatingRepository(ContainerBuilder containerBuilder)
        {
            containerBuilder.RegisterAmazonService();
            containerBuilder.RegisterType<DynamoDbIsolatingTestTakerRepository>()
                .As<IIsolatingTestTakerRepository>()
                .InstancePerHttpRequest();
        }

        private static void RegisterDynamoIsolating(ContainerBuilder containerBuilder)
        {
            RegisterDynamoRepository(containerBuilder);
            RegisterDynamoService(containerBuilder);
            containerBuilder.RegisterType<LinkIt.BubbleSheetPortal.Web.DynamoDb.VaultDynamoPrefixTableNameProvider>()
               .As<LinkIt.BubbleSheetPortal.DynamoConnector.DynamoPrefixTableNameProvider.IDynamoPrefixTableNameProvider>()
               .InstancePerHttpRequest();
        }

        private static void RegisterDynamoRepository(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) return;
            containerBuilder.RegisterType<QTIOnlineTestSessionAnswerDynamo>()
                .As<IQTIOnlineTestSessionAnswerDynamo>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<QTIOnlineTestSessionDynamo>()
                .As<IQTIOnlineTestSessionDynamo>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<QTITestClassAssignmentDynamo>()
                .As<IQTITestClassAssignmentDynamo>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<SGOManagerLogDynamo>()
                .As<ISGOManagerLogDynamo>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<PostAnswerLogDynamo>()
                .As<IPostAnswerLogDynamo>()
                .InstancePerHttpRequest();
        }

        private static void RegisterDynamoService(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) return;
            containerBuilder.RegisterType<GetOnlineTestSessionAnswerService>()
                .As<IGetOnlineTestSessionAnswerService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<GetOnlineTestSessionStatusIsolatingService>()
                .As<IGetOnlineTestSessionStatusIsolatingService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<GetQTIOnlineTestSessionStatusService>()
                .As<IGetQTIOnlineTestSessionStatusService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<GetTestStateService>()
                .As<IGetTestStateService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<PausedOnlineTestService>()
                .As<IPausedOnlineTestService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<SubmitOnlineTestService>()
                .As<ISubmitOnlineTestService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<UpdateAnswerTextService>()
                .As<IUpdateAnswerTextService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<ReopenFailedTestSessionService>()
                .As<IReopenFailedTestSessionService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<SGOManagerLogService>()
                .As<ISGOManagerLogService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<PostAnswerLogService>()
                .As<IPostAnswerLogService>()
                .InstancePerHttpRequest();
            containerBuilder.RegisterType<TLDSDigitalSection23Service>()
              .InstancePerHttpRequest();
            containerBuilder.RegisterType<ShortLinkService>()
                .As<IShortLinkService>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<NavigatorReportService>()
                .As<INavigatorReportService>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<MessageQueueService>()
                .As<IMessageQueueService>()
                .InstancePerHttpRequest();
            containerBuilder.Register(c =>
            {
                return new AmazonSQSClient();
            }).As<IAmazonSQS>().InstancePerHttpRequest();
        }

        private static void RegisterEDMIntergrationService(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null) return;

            containerBuilder.RegisterType<UrlSignatureService>()
                .As<IUrlSignatureService>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<EDMHttpRequest>()
                .As<IEDMHttpRequest>()
                .InstancePerHttpRequest();

            containerBuilder.RegisterType<DocumentManagement>()
                .As<IDocumentManagement>()
                .InstancePerHttpRequest();
        }

    }
}
