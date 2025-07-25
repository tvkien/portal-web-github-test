using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class ArtifactAllStudentModels
    {
        public List<StudentInVirtualTestClass> Students { get; set; }
        public List<string> Tabs { get; set; }
        public List<TestResultScoreAllArtifact> Artifacts { get; set; }
    }

}
