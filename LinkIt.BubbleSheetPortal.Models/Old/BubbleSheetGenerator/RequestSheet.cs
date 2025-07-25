using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models.BubbleSheetGenerator
{
    [Serializable]
    public class RequestSheet //: ValidatableEntity<RequestSheet>
    {
        public RequestSheet()
        {
            barcode1 = new List<string>();
            CorrectAnswers = new List<int>();
            Roster = new List<string>();
            TemplateText = new Dictionary<string, string>();
            PerSheetTemplateText = new List<Dictionary<string, string>>();
            Questions = new List<Question>();
            BubbleSize = 12;
        }

        private List<string> barcode1;
        private List<DocumentDetails> documentDetails = new List<DocumentDetails>();
        public string ApiKey { get; set; }
        public int BubbleSize { get; set; }
        public int SheetCount { get; set; }
        public int QuestionCount { get; set; }
        public int NumberOfGraphExtraPages { get; set; }
        public int NumberOfPlainExtraPages { get; set; }
        public int NumberOfLinedExtraPages { get; set; }
        public int NumberOfPrimaryExtraPages { get; set; }
        public List<Question> Questions { get; set; }
        public List<QuestionSection> QuestionSections { get; set; }
        public List<int> CorrectAnswers { get; set; }
        public string Template { get; set; }
        public string OutputFormat { get; set; }
        public List<string> Roster { get; set; }
        public string Orientation { get; set; }
        public Dictionary<string, string> TemplateText { get; set; }
        public List<Dictionary<string, string>> PerSheetTemplateText { get; set; }
        public bool PreviewImage { get; set; }
        public string PostToOnCompleted { get; set; }
        public string PostToOnError { get; set; }
        public bool IsRoster { get { return Roster != null && Roster.Count > 0; } }
        public List<string> Barcode1
        {
            get { return barcode1; }
            set { barcode1 = value ?? new List<string>(); }
        }
        public string Barcode2 { get; set; }
        public int DistrictId { get; set; }
        public List<DocumentDetails> DocumentDetails
        {
            get { return documentDetails; }
            set { documentDetails = new List<DocumentDetails>(value ?? new List<DocumentDetails>()); }
        }
        public bool IsGridStype { get; set; }
        public bool IsExtraPagesOnly { get; set; }
        public BubbleSheetPreference Preference { get; set; }

    }
}
