using System;
using System.Collections.Generic;
using System.Text;

namespace HorizontalNeftMobilaApp
{
    public class Location
    {
        public long Id { get; set; }

        public string Name { get; set; } = null;

        public virtual ICollection<DepartmentLocation> DepartmentLocations { get; set; } = new List<DepartmentLocation>();

    }
}
