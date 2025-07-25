using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.SGO
{
    public class ArrayOfSGOStudentData
    {
        public List<SGOStudentData> ListStudentData { get; set; }

        public ArrayOfSGOStudentData()
        {
            ListStudentData = new List<SGOStudentData>();
        }
    }
}