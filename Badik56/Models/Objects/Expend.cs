using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Expend
    {
        public int Id { get; set; }
        public User User { get; set; }
        public List<Product> Products { get; set; }
        public double Sum { get; set; }
    }
}
