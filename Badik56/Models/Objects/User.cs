using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class User
    {
        public int Id { get; set; }
        public string IP { get; set; } = "";
        public string Email { get; set; } = "";
        public string Name { get; set; } = "";
        public string Phone { get; set; } = "";
        public DateTime DateCreate { get; set; }
        public List<IP> IPs { get; set; }
        public List<Click> Clicks { get; set; }
    }
}
