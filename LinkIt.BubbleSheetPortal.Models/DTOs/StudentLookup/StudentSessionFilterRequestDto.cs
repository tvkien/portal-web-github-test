using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentLookup
{
    public class StudentSessionFilterRequestDto : GenericDataTableRequest
    {
        public int StudentId { get; set; }
    }
}
