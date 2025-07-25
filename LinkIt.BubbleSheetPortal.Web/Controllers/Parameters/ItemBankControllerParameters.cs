using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.AuthorizeItemLibServices;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ItemBankControllerParameters
    {
        public VirtualQuestionService VirtualQuestionServices { get; set; }
        public QtiBankService QtiBankServices { get; set; }
        public QtiGroupService QtiGroupServices { get; set; }
        public NWEAItemService NWEAItemServices { get; set; }
        public QTI3Service QTI3Services { get; set; }
        public StateService StateServices { get; set; }
        public QTIITemService QTIITemServices { get; set; }
        public UserService UserService { get; set; }
        public QTIRefObjectService PassageService { get; set; }

        public TopicService TopicService { get; set; }
        public LessonOneService LessonOneService { get; set; }
        public LessonTwoService LessonTwoService { get; set; }
        public ItemTagService ItemTagService { get; set; }
        public AuthorizeItemLibService AuthItemLibService { get; set; }
        public Qti3pPassageService Qti3pPassageService { get; set; }
        public UserSchoolService UserSchoolService { get; set; }
        public MasterStandardService MasterStandardService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public DistrictService DistrictService { get; set; }
        public QTI3pItemToPassageService QTI3pItemToPassageService { get; set; }
        public QTI3pSourceService QTI3pSourceService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public QtiItemRefObjectService QtiItemRefObjectService { get; set; }
        public QTIItemTopicService QTIItemTopicService { get; set; }
        public QTIItemLessonOneService QTIItemLessonOneService { get; set; }
        public QTIItemLessonTwoService QTIItemLessonTwoService { get; set; }
        public QtiItemItemTagService QtiItemItemTagService { get; set; }

        //public MasterStandardService MasterStandardService { get; set; }

        public SchoolService SchoolService { get; set; }
        public QTIRefObjectService qtiRefObjectService { get; set; }

        public AuthorGroupService AuthorGroupService { get; set; }
        public DataFileUploadPassageService dataFileUploadPassageService { get; set; }
    }
}
