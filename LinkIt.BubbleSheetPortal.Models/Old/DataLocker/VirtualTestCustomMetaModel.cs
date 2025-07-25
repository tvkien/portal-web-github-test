using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    /// <summary>
    /// The model of VirtualTestCustomMetaData.MetaData
    /// </summary>
    public class VirtualTestCustomMetaModel
    {
        public static readonly string DataTypeSelectList = "SelectList";
        public static readonly string DataTypeNumeric = "Numeric";
        public static readonly string DataTypeAlphabetic = "Alphabetic";
        public static readonly string DataTypeAlphanumeric = "Alphanumeric";
        public static readonly string DataTypeFreeForm = "FreeForm";
        public static readonly string DataTypeArtifact = "Artifact";
        public static readonly string DataTypeNoteComment = "Note_Comment";
        public string Description { get; set; }
        public string DataType { get; set; }//one of above constant        
        public List<VirtualTestCustomMetaSelectListOptionModel> SelectListOptions { get; set; }
        public List<string> ListArtifactTag { get; set; }
        
        public decimal? MaxValue { get; set; }
        public decimal? MinValue { get; set; }
        public int? DecimalScale { get; set; }
        public int? MaxLength { get; set; }
        public bool? UpperCaseOnly { get; set; }
        public string UploadFileTypes { get; set; }
        public int MaxFileSize { get; set; }

        public bool IsAutoCalculation { get; set; }
        public string DerivedName { get; set; } //sum, average, percent
        public string Expression { get; set; }
        public List<EntryResultArtifactFileTypeGroupViewModel> EntryResultArtifactFileTypeGroupViewModel { get; set; }

        public string FormatOption { get; set; }
        public string DisplayOption { get; set; }

        public string DataHostPot { get; set; }

        public List<VirtualTestCustomMetaNoteCommentModel> ListNoteComment { get; set; }
        public string DefaultValue { get; set; }
        public string NoteType { get; set; }
        public int? Order { get; set; }
        public string GetJsonString()
        {
            return this.SerializeToJson();
        }
    }

    public class EntryResultArtifactFileTypeGroupViewModel
    {
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public int MaxFileSizeInBytes { get; set; }
        public List<string> SupportFileType { get; set; }
    }

    public class VirtualTestCustomMetaSelectListOptionModel
    {
        public string Option { get; set; }
        public string Label { get; set; }
        public int Order { get; set; }        
    }

    public class VirtualTestCustomMetaNoteCommentModel
    {
        public string NoteName { get; set; }
        public string DefaultValue { get; set; }
        public string Description { get; set; }
        public string NoteType { get; set; }
        public int? Order { get; set; }
    }
    public class VirtualTestCustomSubScoreModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
