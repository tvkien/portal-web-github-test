using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiItem
    {
        private string xmlContent = string.Empty;

        public int QtiItemId { get; set; }
        public string XmlContent
        {
            get { return xmlContent; }
            set { xmlContent = value.ConvertNullToEmptyString(); }
        }
        public int TotalRow { get; set; }
        public string GroupName { get; set; }
        public string BankName { get; set; }
        public string Topic { get; set; }
        public string Skill { get; set; }
        public string Other { get; set; }
        public string Standard { get; set; }
        public string DistrictTag { get; set; }
        public string Description { get; set; }
        public string Title {  get; set; }
        public int? QTIBankID { get; set; }

        public int? QTIGroupID { get; set; }
    }
}
