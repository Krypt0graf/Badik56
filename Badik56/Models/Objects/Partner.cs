using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Partner : User
    {
        public string Code { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public double Balance { get; set; }
    }
}
