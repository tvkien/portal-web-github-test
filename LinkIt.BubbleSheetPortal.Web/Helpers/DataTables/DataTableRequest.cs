using System;
using System.Collections.Generic;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.DataTables
{
    public class DataTableRequest
    {
        private List<DataTableColumn> columnSearch = new List<DataTableColumn>();
        private int take = 1;
        private int skip = 0;

        public int TableEcho { get; set; }

        public int Skip
        {
            get { return skip; }
            set { skip = Math.Max(value, 0); }
        }

        public int Take
        {
            get { return take; }
            set { take = Math.Max(value, 1); }
        }

        public string AllSearch { get; set; }

        public List<DataTableColumn> Columns
        {
            get { return columnSearch; }
            set { columnSearch = value ?? new List<DataTableColumn>(); }
        }
    }
}
