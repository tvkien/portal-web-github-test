using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class XliFunctionAccess
    {
        public XliFunctionAccess()
        {
            DistrictLibraryAccessible = false;
            CerticaLibraryAccessible = false;
            ProgressLibraryAccessible = false;
        }
        public bool DistrictLibraryAccessible { get; set; }
        public bool CerticaLibraryAccessible { get; set; }
        public bool ProgressLibraryAccessible { get; set; }
    }
}
