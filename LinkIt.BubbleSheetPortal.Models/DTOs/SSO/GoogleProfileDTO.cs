using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.SSO
{
    public class GoogleProfileDTO
    {
        [JsonProperty("email")]
        public string Email { get; set; }
    }
}
