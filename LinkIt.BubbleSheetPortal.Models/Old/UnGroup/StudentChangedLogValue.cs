using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentChangedLogValue
    {
        private string valueChanged = string.Empty;
        private string oldValue = string.Empty;
        private string newValue = string.Empty;

        public int LogValueID { get; set; }
        public int LogID { get; set; }

        public string ValueChanged
        {
            get { return valueChanged; }
            set { valueChanged = value.ConvertNullToEmptyString(); }
        }

        public string OldValue
        {
            get { return oldValue; }
            set { oldValue = value.ConvertNullToEmptyString(); }
        }

        public string NewValue
        {
            get { return newValue; }
            set { newValue = value.ConvertNullToEmptyString(); }
        }
    }
}