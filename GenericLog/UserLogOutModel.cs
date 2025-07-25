using System;

namespace GenericLog
{
    public class UserLogOutModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int? DistrictId { get; set; }
        public string Reason { get; set; }
        public string DateTime { get; set; }               
    }
}
