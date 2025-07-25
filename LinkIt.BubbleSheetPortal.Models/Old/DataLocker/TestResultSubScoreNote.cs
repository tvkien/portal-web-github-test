using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class TestResultSubScoreNote
    {
        public int TestResultSubScoreNoteID { get; set; }

        public int TestResultSubScoreID { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public string NoteKey { get; set; }

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

    }

    public class TestResultSubScoreNoteViewModel
    {
        public int TestResultSubScoreNoteID { get; set; }

        public int TestResultSubScoreID { get; set; }

        public string Name { get; set; }

        public string Note { get; set; }

        public string NoteKey { get; set; }

        public int StudentID { get; set; }

        public int ClassID { get; set; }

        public int VirtualTestID { get; set; }

        public string SubScoreName { get; set; }
        public int LineIndex { get; set; }
    }
}
