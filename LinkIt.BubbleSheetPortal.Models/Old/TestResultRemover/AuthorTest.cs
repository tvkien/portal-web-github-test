using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.TestResultRemover
{
    public class AuthorTest
    {
        private string nameLast = string.Empty;
        private string nameFirst = string.Empty;
        private string userName = string.Empty;

        public int UserId { get; set; }
        public int ClassId { get; set; }
        public int DistrictId { get; set; }
        public int VirtualTestSourceId { get; set; }

        public string NameLast
        {
            get { return nameLast; }
            set { nameLast = value; }
        }
        public string NameFirst
        {
            get { return nameFirst; }
            set { nameFirst = value; }
        }
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        public string CustomAuthor 
        {
            get { return nameLast + " " + nameFirst + "(" + userName + ")"; }
        }
    }
}
