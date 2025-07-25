using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Base
{
    public class GenericDataTableRequestBase
    {
        public string sSortDir_0 { get; set; }
        public int iColumns { get; set; }
        public int iDisplayStart { get; set; }
        public int iDisplayLength { get; set; }
        public string sColumns { get; set; }
        public int? iSortCol_0 { get; set; }
        public int sEcho { get; set; }
        public string sSearch { get; set; }
        public string SortColumns
        {
            get
            {

                if (!string.IsNullOrWhiteSpace(sColumns) && iSortCol_0.HasValue)
                {
                    var columns = sColumns.Split(',');
                    return $"{columns[iSortCol_0.Value]} {(sSortDir_0.Equals("desc") ? "DESC" : "ASC")}";
                }
                return "";
            }
        }
    }
}
