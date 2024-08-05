using System;
using System.Collections.Generic;

namespace HorizontalNeftMobilaApp
{
    public partial class Employee
    {
        public long Id { get; set; }

        public string FirstName { get; set; } = null;

        public string LastName { get; set; } = null;

        public string Phone { get; set; } = null;

        public virtual ICollection<Asset> Assets { get; set; } = new List<Asset>();
    }

}

