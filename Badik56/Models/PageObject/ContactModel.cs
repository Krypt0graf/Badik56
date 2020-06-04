using Badik56.Models.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Badik56.Models.PageObjects
{
    public class ContactModel
    {
        public List<Category> Categories { get; set; }

        public Request Request { get; set; }
        public Contact Contacts { get; set; }
        public User User { get; set; }
    }
}
