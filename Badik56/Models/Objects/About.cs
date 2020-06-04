using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class About
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public Baner Baner { get; set; } = null;
    }
}
