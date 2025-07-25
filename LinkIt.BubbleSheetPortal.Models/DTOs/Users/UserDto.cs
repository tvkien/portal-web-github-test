namespace LinkIt.BubbleSheetPortal.Models.DTOs.Users
{
    public class UserDto
    {
        public int UserId { get; set; }
        public string EmailAddress { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int? DistrictId { get; set; }
        public int? StateId { get; set; }
        public int RoleId { get; set; }
        public int? UserStatusId { get; set; }
    }
    public class PopulationUser
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName
        {
            get
            {
                return string.Format("{0}, {1} ({2})", LastName, FirstName, Name);
            }
        }
    }
}
