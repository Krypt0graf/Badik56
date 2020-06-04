using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.Objects
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public string Description { get; set; }
        public List<Image> Image { get; set; }
        public int Count { get; set; }
        public bool Availability { get; set; }
        public Category Category { get; set; }
    }
}
