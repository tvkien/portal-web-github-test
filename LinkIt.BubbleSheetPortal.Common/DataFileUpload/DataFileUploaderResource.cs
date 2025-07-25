using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common.Enum;

namespace LinkIt.BubbleSheetPortal.Common.DataFileUpload
{
    public class DataFileUploaderResource
    {
        public DataFileUploaderResource()
        {
            ResourceFileName = string.Empty;
            QtiSchemaID = 0;
            MediaFileList = new List<string>();
            QtiItemId = 0;
            ProcessingStep = new StringBuilder();
            ProcessingStep.Append("");
            PassageList = new List<string>();
            DataFileUploadFileType = DataFileUploadFileTypeEnum.Undefined;
            DataFileUploadPassageIdList = new List<int>();
            QTI3pPassageIdList = new List<int>();
            GUIDList = new List<string>();
            PassageIdentifierRefList = new List<string>();
        }
        public string ResourceFileName { get; set; }
        public string Title { get; set; }
        public int QtiSchemaID { get; set; }//the schema of the resource file type 
        public string OriginalContent { get; set; }
        public string XmlContent { get; set; }
        public string InteractionType { get; set; }
        public string Error { get; set; }// a message error will be presented to user

        public bool IsValidQuestionResourceFile
        {
            get { return !string.IsNullOrEmpty(XmlContent); } 
        }

        public List<string> MediaFileList { get; set; }
        public int QtiItemId { get; set; }
        public int QuestionOrder { get; set; }
        public string ErrorDetail { get; set; } // detail of error, such as exception, will not be presented to user
        public StringBuilder ProcessingStep { get; set; }
        public List<string> PassageList { get; set; }
        public List<int> QTI3pPassageIdList { get; set; }

        public DataFileUploadFileTypeEnum DataFileUploadFileType;
        public List<int> DataFileUploadPassageIdList;
        public List<string> GUIDList { get; set; }
        public int DOK { get; set; }
        public string Identifier { get; set; }
        public string PassageTitle { get; set; }
        //For passage attributes
        public string PassageType { get; set; }
        public string Genre { get; set; }
        public string Lexile { get; set; }
        public string Spache { get; set; }
        public string DaleChall { get; set; }
        public string RMM { get; set; }

        //for certica
        public decimal PValue { get; set; }
        public string GradeLevel { get; set; }
        public string Subject { get; set; }
        public string BloomsTaxonomy { get; set; }
        public string ABStandardGUIDs { get; set; }
        public string Difficulty { get; set; }
        public string ContentFocus { get; set; }
        public string DOKCode { get; set; }

        //for passage certica
        public string PGradeLevel { get; set; }
        public string PSubject { get; set; }
        public string PStimulus { get; set; }
        public string PContentArea { get; set; }
        public string PTextType { get; set; }
        public string PTextSubType { get; set; }
        public string PassageSource { get; set; }
        public string PWordCount { get; set; }
        public string PEthnicity { get; set; }
        public string PCommissionedStatus { get; set; }
        public string PFleschKincaid { get; set; }
        public string PGender { get; set; }
        public string PMultiCultural { get; set; }
        public string PCopyrightYear { get; set; }
        public string PCopyrightOwner { get; set; }
        public string PSourceTitle { get; set; }
        public string PAuthor { get; set; }

        //for certica save identifierref
        public List<string> PassageIdentifierRefList { get; set; }
    }
}
