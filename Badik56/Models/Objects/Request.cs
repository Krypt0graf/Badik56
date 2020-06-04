using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Request
    {
        public int Id { get; set; }
        public User User { get; set; }
        public string Theme { get; set; }
        public string Message { get; set; }
    }
}
