using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserBank
    {
        private string bankName = string.Empty;

        public int Id { get; set; }
        public int SubjectId { get; set; }
        public int UserId { get; set; }

        public string BankName
        {
            get { return bankName; }
            set { bankName = value.ConvertNullToEmptyString(); }
        }
    }
}
