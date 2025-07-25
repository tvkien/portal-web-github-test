using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class TestResultScoreNote
    {
        public int TestResultScoreNoteID { get; set; }

        public int TestResultScoreID { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public NoteContents NoteContents
        {
            get
            {
                if (!string.IsNullOrEmpty(Note))
                {
                    return new JavaScriptSerializer().Deserialize<NoteContents>(Note);
                }
                return new NoteContents();
            }
            
        }

        public string NoteKey { get; set; }
        public int LineIndex { get; set; }
    }

    public class TestResultScoreNoteViewModel
    {
        public int TestResultScoreNoteID { get; set; }

        public int TestResultScoreID { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public string NoteKey { get; set; }

        public int StudentID { get; set; }

        public int ClassID { get; set; }

        public int VirtualTestID { get; set; }
        public int LineIndex { get; set; }
    }

    public class NoteContents
    {
        public List<NoteContent> Notes { get; set; }
    }

    public class NoteContent
    {
        public string NoteType { get; set; }
        public string NoteDate { get; set; }
        public string Content { get; set; }
    }
}
