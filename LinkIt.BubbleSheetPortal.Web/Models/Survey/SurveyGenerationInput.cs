
namespace LinkIt.BubbleSheetPortal.Web.Models.Survey
{
    public class SurveyGenerationInput
    {
        public int SchoolId { get; set; }
        public int VirtualTestId { get; set; }
        public string ClientKey { get; set; }        
        public string ReturnURL { get; set; }
    }
}
