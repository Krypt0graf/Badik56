using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Category
    {
        public int Id { get; set; }
        public int CountProducts { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public DateTime LastEdit { get; set; }
    }
}
