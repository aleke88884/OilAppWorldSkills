using System;
using System.Collections.Generic;

namespace HorizontalNeftMobilaApp
{
    public partial class Department
    {
        public long Id { get; set; }

        public string Name { get; set; } = null;

        public virtual ICollection<DepartmentLocation> DepartmentLocations { get; set; } = new List<DepartmentLocation>();
    }

}

