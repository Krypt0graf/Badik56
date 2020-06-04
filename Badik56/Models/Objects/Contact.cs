using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Contact
    {
        public int Id { get; set; }
        public string EmailInfo { get; set; } = "";
        public string EmailMarketing { get; set; } = "";
        public string PhoneInfo { get; set; } = "";
        public string PhoneMarketing { get; set; } = "";
        public string MapStringScript { get; set; } = "";
        public string VKLink { get; set; } = "";
        public string OKLink { get; set; } = "";
        public string FacebookLink { get; set; } = "";
        public string InstagramLink { get; set; } = "";
        public string Address { get; set; } = "";

        public DateTime LastDateEdit { get; set; }
    }
}
