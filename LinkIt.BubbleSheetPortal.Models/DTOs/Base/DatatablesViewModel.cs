using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Base
{
    public class DatatablesViewModel<T>
    {
        public int TotalRecord { get; set; }
        public IEnumerable<T> Data { get; set; }
    }
}
