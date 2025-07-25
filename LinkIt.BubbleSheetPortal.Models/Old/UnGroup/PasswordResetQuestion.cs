using Envoc.Core.Shared.Extensions;
using Envoc.Core.Shared.Model;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class PasswordResetQuestion : ValidatableEntity<PasswordResetQuestion>
    {
        private string question = string.Empty;

        public int Id { get; set; }

        public string Question
        {
            get { return question; }
            set { question = value.ConvertNullToEmptyString(); }
        }

        public int? Type { get; set; }
    }
}