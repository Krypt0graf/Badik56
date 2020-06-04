using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Rules
    {
        public int Id { get; set; }
        public string AboutText { get; set; }
        public List<Rule> ListRule { get; set; }
    }
}
