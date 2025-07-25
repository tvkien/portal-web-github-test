using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class UserBankAccessDTO
    {
        public int UserID { get; set; }
        public List<int> BankIncludeIds { get; set; }
        public List<int> BankExcludeIds { get; set; }

        public UserBankAccessDTO()
        {
            BankExcludeIds = new List<int>();
            BankIncludeIds = new List<int>();
        }
    }
}
