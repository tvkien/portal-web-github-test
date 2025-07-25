using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QTITestStudentAssignmentData
    {
        public int QTITestStudentAssignmentId { get; set; }
        public int StudentId { get; set; }
        public int QTITestClassAssignmentId { get; set; }
    }
}
