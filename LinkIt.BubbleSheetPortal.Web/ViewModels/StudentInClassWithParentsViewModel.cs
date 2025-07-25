using System.Collections.Generic;
using System.Text;
namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentInClassWithParentsViewModel
    {

        public int ID { get; set; }
        public int StudentID { get; set; }
        public string FirstName
        {
            get;
            set;
        }
        //public string MiddleName
        //{
        //    get;
        //    set;
        //}
        public string LastName
        {
            get;
            set;
        }

        public string Gender { get; set; }
        public string LocalId { get; set; }

        public int ParentCount
        {
            get
            {
                if (_parentList != null)
                {

                    return _parentList.Count;
                }
                return 0;
            }
        }
        public string Emails
        {
            get
            {
                if (_parentList != null && _parentList.Count > 0)
                {
                    StringBuilder email = new StringBuilder();

                    for (int i = 0; i < _parentList.Count; i++)
                    {
                        if (_parentList[i].Email.Length > 0)
                        {
                            email.Append(_parentList[i].FirstName);
                            email.Append(" ");
                            email.Append(_parentList[i].LastName);
                            email.Append(": ");
                            email.Append(_parentList[i].Email);
                            if (i < _parentList.Count - 1)
                            {
                                email.Append(", ");
                            }
                            
                        }
                        
                        
                    }
                    return email.ToString();
                }
                else
                {

                    return string.Empty;
                }
            }
        }
        public string Phones
        {
            get
            {

                if (_parentList != null && _parentList.Count > 0)
                {
                    StringBuilder phone = new StringBuilder();

                    for (int i = 0; i < _parentList.Count; i++)
                    {
                        if (_parentList[i].Phone.Length > 0)
                        {
                            phone.Append(_parentList[i].FirstName);
                            phone.Append(" " );
                            phone.Append(_parentList[i].LastName);
                            phone.Append(": ");
                            phone.Append(_parentList[i].Phone);

                            if (i < _parentList.Count - 1)
                            {
                                phone.Append(", ");
                                
                            }
                        }
                    }
                    return phone.ToString();
                }
                else
                {

                    return string.Empty;
                }
            }
        }
        public string ParentDisplay
        {
            get
            {
                if (_parentList != null && _parentList.Count > 0)
                {
                    StringBuilder parentDisplay = new StringBuilder();

                    for (int i = 0; i < _parentList.Count; i++)
                    {
                        parentDisplay.Append(_parentList[i].FirstName);
                        parentDisplay.Append(" ");
                        parentDisplay.Append(_parentList[i].LastName);
                        if (i < _parentList.Count - 1)
                        {
                            parentDisplay.Append(", ");
                        }
                    }
                    return parentDisplay.ToString();
                }
                else
                {

                    return string.Empty;
                }
            }
        }
        private List<StudentParentViewModel> _parentList;
        public List<StudentParentViewModel> ParentList
        {
            get { return _parentList; }
            set { _parentList = value; }
        }
    }
}