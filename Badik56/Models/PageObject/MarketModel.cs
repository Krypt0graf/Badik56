using Badik56.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.PageObjects
{
    public class MarketModel
    {
        public List<Category> Categories { get; set; }
        public string SelectedCategory { get; set; }
        public List<Product> Products { get; set; }
        public User User { get; set; }
        public Contact Contacts { get; set; }

    }
}
