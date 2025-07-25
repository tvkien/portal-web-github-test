using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels
{
    public class StudentPopulationViewModel
    {
        public int CurrentDistrictId { get; set; }
        public bool IsPublisher { get; set; }
        public bool IsNetworkAdmin { get; set; }

        public List<ListItemsViewModel> ListGender { get; set; }
        public List<int> GenderIdsSeleted { get; set; }

        public List<ListItemsViewModel> ListRace { get; set; }
        public List<int> RaceIdsSelected { get; set; }

        public List<ListItemsViewModel> ListProgram { get; set; }
        public List<int> ProgramIdsSelected { get; set; }

        public List<int> ListDistricIds { get; set; }
       
        public string StrIds
        {
            get
            {
                var ids = string.Empty;
                if (!this.ListDistricIds.Any())
                {
                    return ids;
                }
                ids = this.ListDistricIds.Aggregate(ids, (current, id) => current + (id + ","));
                return ids.TrimEnd(new[] { ',' });
            }
        }

        public string StrGenderIds { get; set; }
        public string StrRaceIds { get; set; }
        public string StrProgramIds { get; set; }
        public string StrDistrictTermIds { get; set; }
        public string StrClassIds { get; set; }
        public string StrStudentIds { get; set; }
        public int SGOID { get; set; }

        public string SGOIntruction { get; set; }

        public int DistrictIdSelected { get; set; }
        public int StateIdSelected { get; set; }

        public List<int> ListTermIdsSelected { get; set; }

        public List<int> ListClassIdsSelected { get; set; }

        public List<int> ListStudentIdsSelected { get; set; }

        public StudentPopulationViewModel()
        {
            ListDistricIds = new List<int>();
            ListRace = new List<ListItemsViewModel>();
            ListProgram = new List<ListItemsViewModel>();
            ListGender = new List<ListItemsViewModel>();
            
            RaceIdsSelected = new List<int>();
            ProgramIdsSelected = new List<int>();
            GenderIdsSeleted = new List<int>();
            ListClassIdsSelected = new List<int>();
            ListTermIdsSelected = new List<int>();
            ListStudentIdsSelected = new List<int>();
        }

        public int PermissionAccess { get; set; }

        public int OwnerUserID { get; set; }
        public int CurrentUserID { get; set; }
        public int LimitDisplayRoleID { get; set; }
    }
}