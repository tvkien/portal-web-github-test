using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class BankProperty
    {
        public string Name{get; set;}
        public int Id { get; set; }
        public int SubjectID { get; set; }
        public string SubjectName { get; set; }

        public int GradeId { get; set; }
        public string GradeName { get; set; }

        public int CreatedByUserId { get; set; }
        public string Author { get; set; }
        
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int StateId { get; set; }
        public bool Archived { get; set; }
    }
}
