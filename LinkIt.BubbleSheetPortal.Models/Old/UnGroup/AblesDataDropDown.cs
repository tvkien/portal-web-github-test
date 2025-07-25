using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class AblesDataDropDown
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? RoundIndex { get; set; }
    }

    public class AblesStudent
    {
        public int StudentId { get; set; }
        public string FullName { get; set; }
        public bool HasTestResult { get; set; }
    }
}